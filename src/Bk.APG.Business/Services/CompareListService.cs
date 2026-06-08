using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Common.Resources;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class CompareListService : ICompareListService
{
    private readonly Swiss.FCh.DocumentService.Client.IDocumentService _documentService;
    private readonly IEiamAssignmentService _eiamAssignmentService;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly ILogger<ElectoralListService> _logger;

    public CompareListService(
        Swiss.FCh.DocumentService.Client.IDocumentService documentService,
        IEiamAssignmentService eiamAssignmentService,
        ICommitteeRepository committeeRepository,
        IMasterDataRepository masterDataRepository,
        ILogger<ElectoralListService> logger
    )
    {
        _documentService = documentService;
        _eiamAssignmentService = eiamAssignmentService;
        _committeeRepository = committeeRepository;
        _masterDataRepository = masterDataRepository;
        _logger = logger;
    }

    public async Task<(string fileName, Stream content)> GenerateDocument(ReportFilterParametersDto filterDto)
    {
        ArgumentNullException.ThrowIfNull(filterDto);

        _logger.LogInformation("Generating CompareList GeneralElection document");
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        //var memberFunction = await _masterDataRepository.GetById<Function>(Function.MemberGuid);

        //var evaluationDate = filterDto.AnalysisDate1 ?? DateOnly.FromDateTime(DateTime.Today);

        var date1 = filterDto.AnalysisDate1;
        var date2 = filterDto.AnalysisDate2;

        var committees1 = (await _committeeRepository.GetByFilterForReport(filterDto, departmentId, officeId, committeeId)).ToArray();

        filterDto.AnalysisDate1 = date2;

        var committees2 = (await _committeeRepository.GetByFilterForReport(filterDto, departmentId, officeId, committeeId)).ToArray();

        filterDto.AnalysisDate1 = date1;

        // var allOtherCommittees = _committeeRepository.GetAll().Where(c => c.BeginDate <= evaluationDate && c.EndDate >= evaluationDate && c.TermOfOfficeId != TermOfOffice.Period4YearsInGeneralElectionGuid);

        //var committees = await _committeeRepository.GetAllForExport(departmentId, officeId, committeeId, filterParameterDto);

        //var generalElectionCommitteesWithMembers = generalElectionCommittees.Select(GeneralElectionMapper.FromGeneralElectionCommitteeToCommittee).ToList();

        //// this whole block is necessary because of self organized committees, all functions are made to members there (BKDO-2475)
        //foreach (var committee in generalElectionCommitteesWithMembers)
        //{
        //    if (committee.SelfOrganized == true)
        //    {
        //        foreach (var m in committee.Memberships)
        //        {
        //            m.Function = memberFunction;
        //        }
        //    }
        //}

        //// This lookup is used for other memberships of a person which does not belong to the current committee.
        //var allMembershipsByPerson = allOtherCommittees
        //    .SelectMany(committee => committee.Memberships)
        //    .Where(membership => !membership.IsDeleted && membership.BeginDate <= evaluationDate && membership.EndDate >= evaluationDate)
        //    .GroupBy(membership => membership.PersonId)
        //    .ToDictionary(membershipsByPerson => membershipsByPerson.Key, membershipsByPerson => membershipsByPerson.ToList());

        var compareListDto = new CompareListDto();

        var documentStream = await _documentService.CreateWordFromTemplate($"Templates/CompareList.docx", compareListDto, "compareList");

        return ($"{DateTime.Today:yyyyMMdd}_{BusinessTexts.CompareList_FileName}.docx", documentStream);
    }
}
