using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Common.Resources;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class ElectoralListService : IElectoralListService
{
    private readonly Swiss.FCh.DocumentService.Client.IDocumentService _documentService;
    private readonly IEiamAssignmentService _eiamAssignmentService;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly ILogger<ElectoralListService> _logger;

    public ElectoralListService(
        Swiss.FCh.DocumentService.Client.IDocumentService documentService,
        IEiamAssignmentService eiamAssignmentService,
        ICommitteeRepository committeeRepository,
        IMasterDataRepository masterDataRepository,
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        ILogger<ElectoralListService> logger
    )
    {
        _documentService = documentService;
        _eiamAssignmentService = eiamAssignmentService;
        _committeeRepository = committeeRepository;
        _masterDataRepository = masterDataRepository;
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _logger = logger;
    }

    public async Task<(string fileName, Stream content)> GenerateDocument(ReportFilterParametersDto filterDto, string listType)
    {
        ArgumentNullException.ThrowIfNull(filterDto);

        _logger.LogInformation("Generating electoral list document of type {ListType}", listType);
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var memberFunction = await _masterDataRepository.GetById<Function>(Function.MemberGuid);

        var evaluationDate = filterDto.AnalysisDate1 ?? DateOnly.FromDateTime(DateTime.Today);

        var allOtherCommittees = _committeeRepository.GetAll().Where(c => c.BeginDate <= evaluationDate && c.EndDate >= evaluationDate && c.TermOfOfficeId != TermOfOffice.Period4YearsInGeneralElectionGuid);

        var generalElectionCommittees = await _generalElectionCommitteeRepository.GetByFilterForReport(filterDto, departmentId, officeId, committeeId);

        var generalElectionCommitteesWithMembers = generalElectionCommittees.Select(GeneralElectionMapper.FromGeneralElectionCommitteeToCommittee).ToList();

        // this whole block is necessary because of self organized committees, all functions are made to members there (BKDO-2475)
        foreach (var committee in generalElectionCommitteesWithMembers)
        {
            if (committee.SelfOrganized == true)
            {
                foreach (var m in committee.Memberships)
                {
                    m.Function = memberFunction;
                }
            }
        }

        // This lookup is used for other memberships of a person which does not belong to the current committee.
        var allMembershipsByPerson = allOtherCommittees
            .SelectMany(committee => committee.Memberships)
            .Where(membership => !membership.IsDeleted && membership.BeginDate <= evaluationDate && membership.EndDate >= evaluationDate)
            .GroupBy(membership => membership.PersonId)
            .ToDictionary(membershipsByPerson => membershipsByPerson.Key, membershipsByPerson => membershipsByPerson.ToList());

        var electoralListDto = new ElectoralListDto
        {
            Departments = generalElectionCommitteesWithMembers
                .GroupBy(committee => committee.Department!.GetText())
                .OrderBy(committeesByDepartment => committeesByDepartment.Key)
                .Select(committeesByDepartment => new ElectoralListDepartmentDto
                {
                    Name = committeesByDepartment.Key,
                    Committees = committeesByDepartment
                        .OrderBy(committee => committee.GetDescription())
                        .Select(committee => new ElectoralListCommitteeDto
                        {
                            Name = committee.GetDescription(),
                            CommitteeType = committee.CommitteeType!.GetText(),
                            SelfOrganized = committee.SelfOrganized == true ? BusinessTexts.Committee_SelfOrganized : string.Empty,
                            Functions = committee.Memberships
                                .Where(membership => !membership.IsDeleted && membership.BeginDate <= evaluationDate && membership.EndDate >= evaluationDate && membership.Person != null)
                                .OrderBy(membership => membership.Function!.Sort)
                                .GroupBy(membership => membership.Function!.GetText())
                                .Select(membershipsByFunction => new ElectoralListFunctionDto
                                {
                                    Name = membershipsByFunction.Key,
                                    Members = membershipsByFunction.Select(membership => new ElectoralListMemberDto
                                    {
                                        TitleOccupation = string.IsNullOrWhiteSpace(membership.Person!.Title)
                                            ? membership.Person!.Occupation
                                            : $"{membership.Person!.Title}, {membership.Person!.Occupation}",
                                        Salutation = membership.Person!.Salutation?.GetText() ?? string.Empty,
                                        Surname = membership.Person!.Surname,
                                        GivenName = membership.Person!.GivenName,
                                        Canton = membership.Person!.CorrespondenceAddress?.Canton?.GetText() ?? string.Empty,
                                        BirthYear = membership.Person!.BirthYear,
                                        BeginDate = membership.BeginDate,
                                        EndDate = membership.EndDate,
                                        ElectionType = membership.ElectionType!.GetText(),
                                        Language = membership.Person!.Language!.GetText(),
                                        FederalAssembly = membership.Person!.FederalAssembly ? BusinessTexts.Common_Yes : BusinessTexts.Common_No,
                                        FederalDuty = membership.Person!.FederalDuty ? BusinessTexts.Common_Yes : BusinessTexts.Common_No,
                                        Interests = membership.Person!.NoInterest || membership.Person!.Interests.Count == 0
                                            ? BusinessTexts.Common_None
                                            : string.Join(" ", membership.Person!.Interests.Select(interest => $"[ {interest.Text} | {interest.LegalForm?.GetText() ?? "-"} | {interest.InterestCommittee?.GetText() ?? "-"} | {interest.InterestFunction?.GetText() ?? "-"} ]")
                                                .Where(x => !string.IsNullOrWhiteSpace(x))),
                                        OtherMemberships = GetOtherMemberships(membership, allMembershipsByPerson)
                                    })
                                }),
                        })
                }),
            TitleLabel = $"{BusinessTexts.ElectoralList_ElectoralList} {BusinessTexts.ResourceManager.GetString(listType)}",
            EvaluationDate = evaluationDate,
            FunctionHeaders = new ElectoralListFunctionHeadersDto
            {
                SalutationSurnameGivenName = BusinessTexts.ElectoralList_SalutationSurnameGivenName,
                TitleOccupation = BusinessTexts.ElectoralList_TitleOccupation,
                Canton = BusinessTexts.ElectoralList_Canton,
                BirthYear = BusinessTexts.ElectoralList_BirthYear,
                BeginDate = BusinessTexts.ElectoralList_BeginDate,
                EndDate = BusinessTexts.ElectoralList_EndDate,
                ElectionType = BusinessTexts.ElectoralList_ElectionType,
                Language = BusinessTexts.ElectoralList_Language,
                FederalAssembly = BusinessTexts.ElectoralList_FederalAssembly,
                FederalDuty = BusinessTexts.ElectoralList_FederalDuty
            },
            InterestsLabel = BusinessTexts.ElectoralList_Interests,
            EvaluationDateLabel = BusinessTexts.ElectoralList_EvaluationDate,
            OtherMembershipsLabel = BusinessTexts.ElectoralList_OtherMemberships
        };

        var documentStream = await _documentService.CreateWordFromTemplate($"Templates/{listType}.docx", electoralListDto, "electoralList");

        return ($"{DateTime.Today:yyyyMMdd}_{BusinessTexts.ElectoralList_ElectoralList}_{BusinessTexts.ResourceManager.GetString(listType)}.docx", documentStream);
    }

    private static string? GetOtherMemberships(Membership currentMembership, Dictionary<Guid, List<Membership>> allMembershipsByPerson)
    {
        if (!allMembershipsByPerson.TryGetValue(currentMembership.PersonId, out var personMemberships))
        {
            return null;
        }

        var otherMemberships = personMemberships
            .Where(x => x.Id != currentMembership.Id)
            .Select(x => $"[ {x.Committee!.GetDescription()} | {x.Committee!.CommitteeType!.GetText()} | {x.Function!.GetText()} ]")
            .OrderBy(x => x)
            .ToList();

        return otherMemberships.Count != 0 ? string.Join(" ", otherMemberships) : null;
    }
}
