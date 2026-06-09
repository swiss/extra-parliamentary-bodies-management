using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Common.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class ReportService : IReportService
{
    private readonly Swiss.FCh.DocumentService.Client.IDocumentService _documentService;
    private readonly ITermOfOfficeDateService _termOfOfficeDateService;
    private readonly ICultureService _cultureService;
    private readonly IElectoralListService _electoralListService;
    private readonly ICompareListService _compareListService;
    private readonly IEiamAssignmentService _eiamAssignmentService;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly IGeneralMeasureRepository _generalMeasureRepository;
    private readonly ILogger<ReportService> _logger;
    private readonly IConfiguration _configuration;

    public ReportService(
        Swiss.FCh.DocumentService.Client.IDocumentService documentService,
        ICultureService cultureService,
        ITermOfOfficeDateService termOfOfficeDateService,
        IElectoralListService electoralListService,
        ICompareListService compareListService,
        IEiamAssignmentService eiamAssignmentService,
        ICommitteeRepository committeeRepository,
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        IMasterDataRepository masterDataRepository,
        IGeneralMeasureRepository generalMeasureRepository,
        ILogger<ReportService> logger,
        IConfiguration configuration)
    {
        _documentService = documentService;
        _termOfOfficeDateService = termOfOfficeDateService;
        _cultureService = cultureService;
        _electoralListService = electoralListService;
        _eiamAssignmentService = eiamAssignmentService;
        _compareListService = compareListService;
        _committeeRepository = committeeRepository;
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _masterDataRepository = masterDataRepository;
        _generalMeasureRepository = generalMeasureRepository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<(string fileName, Stream content)> GetReport(ReportFilterParametersDto filterDto)
    {
        ArgumentNullException.ThrowIfNull(filterDto);

        _logger.LogInformation("Generate report of type {ReportType}", filterDto.DocumentType);

        return filterDto.DocumentType switch
        {
            ReportType.ParliamentaryReport => await GenerateParliamentaryReport(filterDto),
            ReportType.AppendixFederalCouncil => await GenerateAppendixFederalCouncil(filterDto),
            ReportType.ElectoralListOnline => await _electoralListService.GenerateDocument(filterDto, "ElectoralList_Internet"),
            ReportType.ElectoralListFC => await _electoralListService.GenerateDocument(filterDto, "ElectoralList_FederalCouncil"),
            ReportType.DecisionFederalCouncil => await GenerateDecisionFederalCouncilReport(filterDto),
            ReportType.Vacancies => await GenerateVacanciesReport(filterDto),
            ReportType.InformationNoteGeneralElection => await GenerateInformationNote(filterDto),
            ReportType.CompareListGeneralElection => await _compareListService.GenerateDocument(filterDto),
            _ => await GenerateParliamentaryReport(filterDto)
        };
    }

    private async Task<(string fileName, Stream content)> GenerateDecisionFederalCouncilReport(ReportFilterParametersDto filterDto)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var template = _cultureService.GetCurrentUiCulture().TwoLetterISOLanguageName == Language.French ? "APG_Decision_Federal_Council_French" : "APG_Decision_Federal_Council_German";

        var nextTermOfOfficeDate = await _termOfOfficeDateService.GetNextTermOfOfficeDate();

        var committees = (await _committeeRepository.GetAllForGeneralElection(departmentId, officeId, committeeId)).ToArray();
        var committeesWithMembers = await _generalElectionCommitteeRepository.GetByFilterForReport(filterDto, departmentId, officeId, committeeId);

        var generalElectionCommitteesWithMembers = committeesWithMembers.Select(ReportMapper.FromGeneralElectionCommitteeToReportGeneralElectionCommitteeDto).ToArray();

        generalElectionCommitteesWithMembers = await ReplaceSelfOrganisedMemberFunctions(generalElectionCommitteesWithMembers);

        var currentExtraParliamentaryCommissions = committees.Where(c => c.ExtraParliamentaryCommission).ToArray();
        var currentReportExtraParliamentaryCommissions = currentExtraParliamentaryCommissions.Select(ReportMapper.FromCommitteeToReportGeneralElectionCommitteeDto).ToArray();

        var extraParliamentaryCommissions = generalElectionCommitteesWithMembers.Where(c => c.ExtraParliamentaryCommission).ToArray();

        var nonReleasedCommissions = generalElectionCommitteesWithMembers.Where(c => c.ReleaseGeneralElection == false).ToArray();

        var marketOrientatedCommissions = extraParliamentaryCommissions.Where(c => c.MarketOrientated == true).ToArray();

        var membersWith12OrMoreYears = SummarizeMembershipsFromPresentAndFuture(currentReportExtraParliamentaryCommissions, extraParliamentaryCommissions);
        var membersWith12OrMoreYearsDto = GetLongerMembershipData(membersWith12OrMoreYears);

        var committeesWithMembersInFederalDuty = generalElectionCommitteesWithMembers
            .Where(c => c.Memberships.Any(m => m.Person?.FederalDuty == true))
            .ToArray();

        var moreThan15MembersCommittees = generalElectionCommitteesWithMembers
            .Where(c => c is { ExtraParliamentaryCommission: true, Memberships.Count: > 15 })
            .ToArray();

        var nonReleasedCommissionsDto = GetNonReleasedCommissions(nonReleasedCommissions);
        var marketOrientatedCommissionsDto = GetMarketOrientatedMembershipData(marketOrientatedCommissions);
        var moreThan15MembersCommitteesDto = GetCommitteesAndMembers(moreThan15MembersCommittees);
        var missingGenderMembersCommitteesDto = GetCommitteesWithGenders(extraParliamentaryCommissions);
        var missingItalianAndFrenchMembersCommitteesDto = GetCommitteesWithLanguages(extraParliamentaryCommissions);
        var committeesWithMembersInFederalDutyDto = GetFederalDutyMembershipsWithOffice(committeesWithMembersInFederalDuty);

        var decisionFederalCouncilReportDto = new DecisionFederalCouncilReportDto
        {
            TermOfOfficeDateRange = nextTermOfOfficeDate.BeginDate.Year + BusinessTexts.Term_Of_Office_Data_Separator + nextTermOfOfficeDate.EndDate?.Year,
            OnlyReleased = filterDto.ReleasedCommittees,
            NonReleasedCommissions = nonReleasedCommissionsDto,
            MarketOrientatedCommissions = marketOrientatedCommissionsDto,
            MoreThan15MembersCommittees = moreThan15MembersCommitteesDto,
            MissingGenderMembersCommittees = missingGenderMembersCommitteesDto,
            MissingLanguageMembersCommittees = missingItalianAndFrenchMembersCommitteesDto,
            LongerDutyMembersCommittees = membersWith12OrMoreYearsDto,
            FederalDutyMembersCommittees = committeesWithMembersInFederalDutyDto
        };

        var documentStream = await _documentService.CreateWordFromTemplate($"Templates/{template}.docx", decisionFederalCouncilReportDto, "decisionFederalCouncil");

        return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd} {BusinessTexts.DecisionFederalCouncil_Filename}.docx", documentStream);
    }

    private async Task<(string fileName, Stream content)> GenerateParliamentaryReport(ReportFilterParametersDto filterDto)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var template = _cultureService.GetCurrentUiCulture().TwoLetterISOLanguageName == Language.French ? "Report_Parliament_French" : "Report_Parliament_German";

        var departments = await _masterDataRepository.GetDepartments();
        departments = departments.Where(d => d.Uri != Department.BkUri).ToArray();

        var nextTermOfOfficeDate = await _termOfOfficeDateService.GetNextTermOfOfficeDate();
        var otherCommitteeTypes = _committeeRepository.GetAll()
            .Where(c => c.TermOfOfficeId != TermOfOffice.Period4YearsInGeneralElectionGuid && c.BeginDate <= DateOnly.FromDateTime(DateTime.Today) && (c.EndDate is null || c.EndDate >= DateOnly.FromDateTime(DateTime.Today)))
            .ToArray();
        var otherReportCommitteeTypes = otherCommitteeTypes.Select(ReportMapper.FromCommitteeToReportGeneralElectionCommitteeDto).ToArray();

        var committees = (await _committeeRepository.GetAllForGeneralElection(departmentId, officeId, committeeId)).ToArray();
        var committeesWithMembers = await _generalElectionCommitteeRepository.GetByFilterForReport(filterDto, departmentId, officeId, committeeId);

        var generalElectionCommitteesWithMembers = committeesWithMembers.Select(ReportMapper.FromGeneralElectionCommitteeToReportGeneralElectionCommitteeDto).ToArray();

        generalElectionCommitteesWithMembers = await ReplaceSelfOrganisedMemberFunctions(generalElectionCommitteesWithMembers);

        var vacanciesCommittees = generalElectionCommitteesWithMembers.Where(c => c.VacanciesGeneralElection > 0).ToArray();

        var currentExtraParliamentaryCommissions = committees.Where(c => c.ExtraParliamentaryCommission).ToArray();
        var currentReportExtraParliamentaryCommissions = currentExtraParliamentaryCommissions.Select(ReportMapper.FromCommitteeToReportGeneralElectionCommitteeDto).ToArray();

        var extraParliamentaryCommissions = generalElectionCommitteesWithMembers.Where(c => c.ExtraParliamentaryCommission).ToArray();
        var membersWith12OrMoreYears = SummarizeMembershipsFromPresentAndFutureByDepartment(committeesWithMembers, departments).ToArray();

        var memberCountWith12OrMoreYears = membersWith12OrMoreYears
            .SelectMany(d => d.Committees!)
            .SelectMany(c => c.Members!)
            .Count();

        var femaleCount = extraParliamentaryCommissions.Sum(c => c.FemaleCount);
        var maleCount = extraParliamentaryCommissions.Sum(c => c.MaleCount);

        var adminCommitteesWithFederalAssemblyMembers = extraParliamentaryCommissions
            .Where(c => c.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid)
            .Count(c => c.Memberships.Any(m => m.Person != null && m.Person!.FederalAssembly));

        var adminCommitteesFederalAssemblyMembers = extraParliamentaryCommissions
            .Where(c => c.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid)
            .SelectMany(c => c.Memberships)
            .Count(m => m.Person != null && m.Person!.FederalAssembly);

        var extraParliamentaryCommissionsWithFederalDutyMembers = extraParliamentaryCommissions
            .Count(c => c.Memberships.Any(m => m.Person != null && m.Person!.FederalDuty));

        var extraParliamentaryCommitteesFederalDutyMembers = extraParliamentaryCommissions
            .SelectMany(c => c.Memberships)
            .Count(m => m.Person != null && m.Person!.FederalDuty);

        var totalMembers = extraParliamentaryCommissions.Sum(c => c.ActiveMemberCount);

        var totalGerman = extraParliamentaryCommissions.Sum(c => c.GermanCount);
        var totalFrench = extraParliamentaryCommissions.Sum(c => c.FrenchCount);
        var totalItalian = extraParliamentaryCommissions.Sum(c => c.ItalianCount);
        var totalRomansh = extraParliamentaryCommissions.Sum(c => c.RomanshCount);

        var frenchAndItalianMissing = extraParliamentaryCommissions.Count(c => c is { FrenchCount: 0, ItalianCount: 0 });
        var frenchMissing = extraParliamentaryCommissions.Count(c => c is { FrenchCount: 0, ItalianCount: > 0 });
        var italianMissing = extraParliamentaryCommissions.Count(c => c is { ItalianCount: 0, FrenchCount: > 0 });

        var femaleUnderStaffed = extraParliamentaryCommissions.Count(c => c.FemaleUnderStaffed);
        var maleUnderStaffed = extraParliamentaryCommissions.Count(c => c.MaleUnderStaffed);

        // get all committees for GE, which are released/validated and did not end before the current termOfOfficeDate
        var releasedGeneralElectionCommittees = committeesWithMembers
            .Where(c => c.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalForwarded || c.CandidateListStateId != CandidateListState.ReadyForFederalCouncilProposalFinalized)
            .ToArray();
        var releasedCommittees = releasedGeneralElectionCommittees.Select(GeneralElectionMapper.FromGeneralElectionCommitteeToCommittee).ToArray();

        // same as above, but not released/validated
        var unreleasedGeneralElectionCommittees = committeesWithMembers
            .Where(c => c.CandidateListStateId != CandidateListState.ReadyForFederalCouncilProposalForwarded && c.CandidateListStateId != CandidateListState.ReadyForFederalCouncilProposalFinalized)
            .ToArray();
        var unreleasedCommittees = unreleasedGeneralElectionCommittees.Select(GeneralElectionMapper.FromGeneralElectionCommitteeToCommittee).ToArray();

        // number of APKs (!) which are new or have ended before the current termOfOfficeDate
        var moreThan15MembersCommittees = generalElectionCommitteesWithMembers.Where(c => c is { ExtraParliamentaryCommission: true, Memberships.Count: > 15 }).ToArray();
        var releasedCommitteesDto = GetCommitteesByDepartmentAndTypes(releasedCommittees, departments);
        var unreleasedCommitteesDto = GetCommitteesByDepartmentAndTypes(unreleasedCommittees, departments);
        var disbandedCommitteesDto = GetDepartmentsOnly(departments);
        var financialImpactsCommitteesDto = GetCommitteesByDepartment(generalElectionCommitteesWithMembers, departments, ReportCommitteeType.StandardBehaviour);
        var vacanciesCommitteesDto = GetCommitteesByDepartment(vacanciesCommittees, departments, ReportCommitteeType.Vacancies);

        var moreThan15MembersCommitteesDto = GetCommitteesByDepartment(moreThan15MembersCommittees, departments, ReportCommitteeType.StandardBehaviour);
        var missingGenderMembersCommitteesDto = await GetCommitteesWithGendersByDepartment(extraParliamentaryCommissions, departments);
        var missingItalianAndFrenchMembersCommitteesDto = await GetCommitteesWithLanguagesByDepartment(extraParliamentaryCommissions, departments);

        var committeesWithMembersInFederalAssembly = GetCommitteesAndMembersByDepartment(extraParliamentaryCommissions.Where(c => c.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid).ToArray(), departments, ReportMembershipType.FederalAssemblyFuture);
        var committeesWithMembersInFederalDuty = GetCommitteesAndMembersByDepartment(extraParliamentaryCommissions, departments, ReportMembershipType.FederalDuty);
        var committeesWithDifferentTermOfOffice = GetCommitteesByDepartment(otherReportCommitteeTypes, departments, ReportCommitteeType.StandardBehaviour);

        var parliamentaryReportDto = new ParliamentaryReportDto
        {
            TermOfOfficeDateRange = nextTermOfOfficeDate.BeginDate.Year + BusinessTexts.Term_Of_Office_Data_Separator + nextTermOfOfficeDate.EndDate?.Year,
            OnlyReleased = filterDto.ReleasedCommittees,
            NumberOfMembers = committees.Sum(c => c.ActiveMemberCount),
            NumberOfCommittees = committees.Length,
            NumberOfAuthoritiesCommissions = committees.Count(c => c.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid),
            NumberOfAdministrationCommissions = committees.Count(c => c.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid),
            NumberOfManagementCommittees = committees.Count(c => c.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid),
            NumberOfFederalAgenciesCommittees = committees.Count(c => c.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid),
            NumberOfVacancies = vacanciesCommittees.Length,
            NumberOfExtraParliamentaryCommissions = extraParliamentaryCommissions.Length,
            MoreThan15Members = generalElectionCommitteesWithMembers.Count(c => c is { ExtraParliamentaryCommission: true, Memberships.Count: > 15 }),
            FemalePercentage = totalMembers > 0 ? Math.Round((decimal)femaleCount / totalMembers * 100, 2) : 0,
            MalePercentage = totalMembers > 0 ? Math.Round((decimal)maleCount / totalMembers * 100, 2) : 0,
            FemaleUnderStaffed = femaleUnderStaffed,
            MaleUnderStaffed = maleUnderStaffed,
            FrenchMissing = frenchMissing,
            ItalianMissing = italianMissing,
            FrenchAndItalianMissing = frenchAndItalianMissing,
            GermanPercentage = totalMembers > 0 ? Math.Round((decimal)totalGerman / totalMembers * 100, 2) : 0,
            FrenchPercentage = totalMembers > 0 ? Math.Round((decimal)totalFrench / totalMembers * 100, 2) : 0,
            ItalianPercentage = totalMembers > 0 ? Math.Round((decimal)totalItalian / totalMembers * 100, 2) : 0,
            RomanshPercentage = totalMembers > 0 ? Math.Round((decimal)totalRomansh / totalMembers * 100, 2) : 0,
            LongerThan12Years = memberCountWith12OrMoreYears,
            AdminCommWithFederalAssemblyMembers = adminCommitteesWithFederalAssemblyMembers,
            AdminCommMembersWithFederalAssembly = adminCommitteesFederalAssemblyMembers,
            CommissionsWithFederalDutyMembers = extraParliamentaryCommissionsWithFederalDutyMembers,
            CommissionsMembersWithFederalDuty = extraParliamentaryCommitteesFederalDutyMembers,
            OtherCommitteeTypes = otherCommitteeTypes.Length,
            ReleasedCommittees = releasedCommitteesDto,
            UnreleasedCommittees = unreleasedCommitteesDto,
            DisbandedCommittees = disbandedCommitteesDto,
            FinancialImpactsCommittees = financialImpactsCommitteesDto,
            VacanciesCommittees = vacanciesCommitteesDto,
            MoreThan15MembersCommittees = moreThan15MembersCommitteesDto,
            MissingGenderMembersCommittees = missingGenderMembersCommitteesDto,
            MissingLanguageMembersCommittees = missingItalianAndFrenchMembersCommitteesDto,
            LongerDutyMembersCommittees = membersWith12OrMoreYears,
            FederalAssemblyMembersCommittees = committeesWithMembersInFederalAssembly,
            FederalDutyMembersCommittees = committeesWithMembersInFederalDuty,
            DifferentTermOfOfficeCommittees = committeesWithDifferentTermOfOffice
        };

        var documentStream = await _documentService.CreateWordFromTemplate($"Templates/{template}.docx", parliamentaryReportDto, "parliamentReport");

        return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd} {BusinessTexts.ParliamentaryReport_Filename}.docx", documentStream);
    }

    private async Task<(string fileName, Stream content)> GenerateAppendixFederalCouncil(ReportFilterParametersDto filterDto)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var template = _cultureService.GetCurrentUiCulture().TwoLetterISOLanguageName == Language.French ? "Appendix_FederalCouncil_French" : "Appendix_FederalCouncil_German";

        var departments = await _masterDataRepository.GetDepartments();
        departments = departments.Where(d => d.Uri != Department.BkUri).ToArray();

        // there must be a general election running, or this report cannot even be started
        var generalElectionTermOfOfficeDate = await _termOfOfficeDateService.GetGeneralElectionTermOfOfficeDate();

        var legislaturePeriods = await _masterDataRepository.GetLegislaturePeriods();
        var nextLegislaturePeriod = legislaturePeriods.FirstOrDefault(lp => lp.StartDate < generalElectionTermOfOfficeDate.BeginDate && lp.EndDate > generalElectionTermOfOfficeDate.BeginDate);

        // present data, needed for one part of the report!
        var committees = await _committeeRepository.GetAllForGeneralElectionWithActiveMembers(departmentId, officeId, committeeId);
        var extraParliamentaryCommittees = committees.Where(c => c.ExtraParliamentaryCommission).ToArray();

        // to be able to use the standard repository function we have to cheat here with the released flag!
        var onlyReleasedCommittees = filterDto.ReleasedCommittees;
        filterDto.ReleasedCommittees = false;

        var reportCommittees = extraParliamentaryCommittees.Select(ReportMapper.FromCommitteeToReportGeneralElectionCommitteeDto).ToArray();

        var generalElectionCommittees = await _generalElectionCommitteeRepository.GetByFilterForReport(filterDto, departmentId, officeId, committeeId);

        // to be able to use the same functions, we map here the GeneralElection data to normal data!
        var generalElectionCommitteesWithMembers = generalElectionCommittees.Select(ReportMapper.FromGeneralElectionCommitteeToReportGeneralElectionCommitteeDto).ToArray();

        generalElectionCommitteesWithMembers = await ReplaceSelfOrganisedMemberFunctions(generalElectionCommitteesWithMembers);

        var extraParliamentaryCommissions = generalElectionCommitteesWithMembers.Where(c => c.ExtraParliamentaryCommission).ToArray();
        var marketOrientatedCommissions = generalElectionCommitteesWithMembers.Where(c => c.MarketOrientated == true).ToArray();

        // get all committees for GE, which are in one of the desired states
        var releasedCommittees = generalElectionCommitteesWithMembers
            .ToArray();
        // unreleased is only relevant, when option "Nur freigegebene" is selected on UI. If not selected, this list should be empty.
        var unreleasedCommittees = Array.Empty<ReportGeneralElectionCommitteeDto>();

        if (onlyReleasedCommittees)
        {
            releasedCommittees = generalElectionCommitteesWithMembers
                .Where(c => c.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalForwarded || c.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalFinalized)
                .ToArray();

            unreleasedCommittees = generalElectionCommitteesWithMembers
                .Where(c => c.CandidateListStateId != CandidateListState.ReadyForFederalCouncilProposalForwarded && c.CandidateListStateId != CandidateListState.ReadyForFederalCouncilProposalFinalized)
                .ToArray();
        }

        // number of APKs (!) which are new or have ended before the current termOfOfficeDate
        var moreThan15MembersCommittees = extraParliamentaryCommissions
            .Where(c => c is { Memberships.Count: > 15 })
            .ToArray();
        var releasedCommitteesDto = GetCommitteesByDepartment(releasedCommittees, departments, ReportCommitteeType.StandardBehaviour);
        var unreleasedCommitteesDto = GetCommitteesByDepartment(unreleasedCommittees, departments, ReportCommitteeType.StandardBehaviour);
        var disbandedCommitteesDto = GetDepartmentsOnly(departments);
        var vacanciesCommittees = releasedCommittees
            .Where(c => c.VacanciesGeneralElection != null)
            .ToArray();

        var selectionProcedureCommittees = generalElectionCommitteesWithMembers
            .Where(c => c.SelectionProcedure != null)
            .ToArray();

        var requirementsProfileCommittees = generalElectionCommitteesWithMembers
            .Where(c => c.Memberships.Any(m => !string.IsNullOrWhiteSpace(m.RequirementsProfile)))
            .Select(c =>
            {
                c.Memberships = c.Memberships
                    .Where(m => m.ElectionTypeId == ElectionType.NewElectionGuid)
                    .ToArray();

                return c;
            })
            .OrderBy(c => c.Committee!.CommitteeNumber)
            .ToArray();

        var moreThan15MembersCommitteesDto = GetCommitteesByDepartment(moreThan15MembersCommittees, departments, ReportCommitteeType.StandardBehaviour);
        var missingGenderMembersCommitteesDto = await GetCommitteesWithGendersByDepartment(extraParliamentaryCommissions, departments);
        var missingItalianAndFrenchMembersCommitteesDto = await GetCommitteesWithLanguagesByDepartment(extraParliamentaryCommissions, departments);

        var missingGenderMembersCommitteesCount = missingGenderMembersCommitteesDto.SelectMany(c => c.Committees!).Count();
        var missingItalianAndFrenchMembersCommitteesCount = missingItalianAndFrenchMembersCommitteesDto.SelectMany(c => c.Committees!).Count();

        var committeesWithMembersWithLongerDutyDto = SummarizeMembershipsFromPresentAndFutureByDepartment(generalElectionCommittees, departments);

        var committeesWithMembersWithShorterDutyDto = GetCommitteesAndMembersByDepartment(generalElectionCommitteesWithMembers, departments, ReportMembershipType.ShorterDuty, generalElectionTermOfOfficeDate);

        var marketOrientatedCommitteesDto = GetCommitteesAndMembersByDepartment(marketOrientatedCommissions, departments, ReportMembershipType.MarketOrientated);

        var committeesWithMembersInFederalAssemblyDto = GetCommitteesAndMembersByDepartment(extraParliamentaryCommissions
            .Where(c => c.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid).ToArray(), departments, ReportMembershipType.FederalAssemblyFuture, null, nextLegislaturePeriod);
        var committeesWithMembersInFederalDutyDto = GetCommitteesAndMembersByDepartment(extraParliamentaryCommissions, departments, ReportMembershipType.FederalDuty);

        var managementCommittees = generalElectionCommitteesWithMembers
            .Where(c => c.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid)
            .ToArray();
        var federalAgencies = generalElectionCommitteesWithMembers
            .Where(c => c.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid)
            .ToArray();

        var managementCommitteesWithMissingGendersDto = await GetCommitteesWithGendersByDepartment(managementCommittees, departments);
        var managementCommitteesWithMissingLanguagesDto = await GetCommitteesWithLanguagePercentagesByDepartment(managementCommittees, departments);
        var federalAgenciesWithMissingGendersDto = await GetCommitteesWithGendersByDepartment(federalAgencies, departments);
        var federalAgenciesWithMissingLanguagesDto = await GetCommitteesWithLanguagePercentagesByDepartment(federalAgencies, departments);
        var committeesWithVacanciesDto = GetCommitteesByDepartment(vacanciesCommittees, departments, ReportCommitteeType.Vacancies);

        var multipleMembershipsDto = GetPersonsWithMultipleMemberships(generalElectionCommitteesWithMembers).ToArray();

        var committeesWithFormerFederalAssemblyMembersDto = GetCommitteesAndMembersByDepartment(generalElectionCommitteesWithMembers, departments, ReportMembershipType.FederalAssemblyCurrent, null, nextLegislaturePeriod);

        var committeesWithSelectionProcedureDto = GetCommitteesByDepartment(selectionProcedureCommittees, departments, ReportCommitteeType.SelectionProcedure);
        var committeesWithMembersRequirementsProfileDto = GetCommitteesAndMembersByDepartment(requirementsProfileCommittees, departments, ReportMembershipType.CompetenceProfile);

        var appendixReportDto = new AppendixFederalCouncilDto
        {
            TermOfOfficeDateRange = generalElectionTermOfOfficeDate.BeginDate.Year + BusinessTexts.Term_Of_Office_Data_Separator + generalElectionTermOfOfficeDate.EndDate?.Year,
            OnlyReleased = filterDto.ReleasedCommittees,
            NumberOfMembers = generalElectionCommitteesWithMembers.Sum(c => c.ActiveMemberCount),
            NumberOfCommittees = generalElectionCommitteesWithMembers.Length,
            NumberOfExtraParliamentaryCommissions = extraParliamentaryCommissions.Length,
            MoreThan15Members = generalElectionCommitteesWithMembers.Count(c => c is { ExtraParliamentaryCommission: true, Memberships.Count: > 15 }),
            MissingGender = missingGenderMembersCommitteesCount,
            MissingLanguage = missingItalianAndFrenchMembersCommitteesCount,
            NumberOfMultipleMembershipsPersons = multipleMembershipsDto.Length,
            ReleasedCommittees = releasedCommitteesDto,
            UnreleasedCommittees = unreleasedCommitteesDto,
            DisbandedCommittees = disbandedCommitteesDto,
            VacanciesCommittees = committeesWithVacanciesDto,
            ShorterDutyMembersCommittees = committeesWithMembersWithShorterDutyDto,
            MoreThan15MembersCommittees = moreThan15MembersCommitteesDto,
            MissingGenderMembersCommittees = missingGenderMembersCommitteesDto,
            MissingLanguageMembersCommittees = missingItalianAndFrenchMembersCommitteesDto,
            MarketOrientatedMembersCommittees = marketOrientatedCommitteesDto,
            LongerDutyMembersCommittees = committeesWithMembersWithLongerDutyDto,
            FederalAssemblyMembersCommittees = committeesWithMembersInFederalAssemblyDto,
            FederalDutyMembersCommittees = committeesWithMembersInFederalDutyDto,
            MissingGenderMembersManagementCommittees = managementCommitteesWithMissingGendersDto,
            MissingLanguageMembersManagementCommittees = managementCommitteesWithMissingLanguagesDto,
            MissingGenderMembersFederalAgenciesCommittees = federalAgenciesWithMissingGendersDto,
            MissingLanguageMembersFederalAgenciesCommittees = federalAgenciesWithMissingLanguagesDto,
            FormerFederalAssemblyMembersCommittees = committeesWithFormerFederalAssemblyMembersDto,
            MultipleMembershipsPersons = multipleMembershipsDto,
            SelectionProcedureCommittees = committeesWithSelectionProcedureDto,
            RequirementsProfileMembersCommittees = committeesWithMembersRequirementsProfileDto
        };

        var documentStream = await _documentService.CreateWordFromTemplate($"Templates/{template}.docx", appendixReportDto, "appendixReport");

        return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd} {BusinessTexts.AppendixFederalCouncil_Filename}.docx", documentStream);
    }

    private async Task<(string fileName, Stream content)> GenerateInformationNote(ReportFilterParametersDto filterDto)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var template = _cultureService.GetCurrentUiCulture().TwoLetterISOLanguageName == Language.French ? "InformationNote_French" : "InformationNote_German";

        var departments = await _masterDataRepository.GetDepartments();
        departments = departments.Where(d => d.Uri != Department.BkUri).ToArray();

        // get relevant master data
        var generalElectionTermOfOfficeDate = await _termOfOfficeDateService.GetGeneralElectionTermOfOfficeDate();
        var currentTermOfOfficeDate = await _termOfOfficeDateService.GetCurrentTermOfOfficeDate();
        var committeeTypes = await _masterDataRepository.GetCommitteeTypes();
        var managementCommitteeType = committeeTypes.FirstOrDefault(c => c.Id == CommitteeType.ManagementCommitteeGuid);

        // this value is not in the database, as it's overwritten!
        var previousExpectedGenderPercentage = 40;

        // present data, needed for the comparision with the future!
        var committees = await _committeeRepository.GetAllForGeneralElectionWithActiveMembers(departmentId, officeId, committeeId);
        var extraParliamentaryCommittees = committees.Where(c => c.ExtraParliamentaryCommission).ToArray();

        var allGeneralElectionCommittees = (await _generalElectionCommitteeRepository.GetByFilterForReport(filterDto, departmentId, officeId, committeeId)).ToArray();
        var allGeneralElectionCommitteesUnfiltered = (await _generalElectionCommitteeRepository.GetAllWithPermissionCheck(departmentId, officeId, committeeId)).ToArray();

        if (filterDto.CommitteesWithActiveMembership)
        {
            allGeneralElectionCommitteesUnfiltered = allGeneralElectionCommitteesUnfiltered
                .Where(c => c.MembershipCandidates.Any(m => m.IsSelected))
                .ToArray();
        }

        var numberOfSelectedCandidates = allGeneralElectionCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected);

        var allAuthoritiesCommittees = allGeneralElectionCommitteesUnfiltered.Where(c => c.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid).ToArray();
        var allAdministrationCommittees = allGeneralElectionCommitteesUnfiltered.Where(c => c.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid).ToArray();
        var allManagementCommittees = allGeneralElectionCommitteesUnfiltered.Where(c => c.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid).ToArray();
        var allFederalAgenciesCommittees = allGeneralElectionCommitteesUnfiltered.Where(c => c.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid).ToArray();
        var allExtraParliamentaryCommittees = allGeneralElectionCommitteesUnfiltered.Where(c => c is { ExtraParliamentaryCommission: true, IsValidated: true }).ToArray();
        var allNonExtraParliamentaryCommittees = allGeneralElectionCommitteesUnfiltered.Where(c => c is { ExtraParliamentaryCommission: false, IsValidated: true }).ToArray();

        // get all committees for GE, which are released and did not end before the current termOfOfficeDate
        var releasedCommittees = allGeneralElectionCommittees
            .Where(c => c.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalForwarded || c.CandidateListStateId != CandidateListState.ReadyForFederalCouncilProposalFinalized)
            .Select(GeneralElectionMapper.FromGeneralElectionCommitteeToCommittee)
            .ToArray();
        var releasedCommitteesDto = GetCommitteesByDepartmentAndTypes(releasedCommittees, departments);

        // same as above, but not released
        var unreleasedCommittees = allGeneralElectionCommitteesUnfiltered
            .Where(c => c.CandidateListStateId != CandidateListState.ReadyForFederalCouncilProposalForwarded && c.CandidateListStateId != CandidateListState.ReadyForFederalCouncilProposalFinalized)
            .ToArray();
        var unreleasedCommitteesDto = GetCommitteesByDepartmentAndTypesForInformationNote(unreleasedCommittees, departments, InformationNoteData.Vacancies);

        var moreThan12YearsCommittes = allGeneralElectionCommittees.Where(c => c.IsValidated).ToArray();
        var moreThan12YearsCommitteesDto = GetCommitteesByDepartmentForMembershipDuration(moreThan12YearsCommittes, departments);

        var genderUnderstaffedCommittees = allGeneralElectionCommittees.Where(c => c.IsValidated && (c.FemaleUnderStaffed || c.MaleUnderStaffed)).ToArray();
        var genderUnderstaffedCommitteesDto = GetCommitteesByDepartmentAndTypesForInformationNote(genderUnderstaffedCommittees, departments, InformationNoteData.Genders);

        var languageUnderstaffedCommittees = allGeneralElectionCommittees.Where(c => c.IsValidated && (c.ItalianUnderStaffed || c.FrenchUnderStaffed)).ToArray();
        var languageUnderstaffedCommitteesDto = GetCommitteesByDepartmentAndTypesForInformationNote(languageUnderstaffedCommittees, departments, InformationNoteData.Languages);

        var allCandidates = allExtraParliamentaryCommittees.SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected);
        var allFemaleCandidates = allExtraParliamentaryCommittees.SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.GenderId == Gender.FemaleGuid);
        var allMaleCandidates = allExtraParliamentaryCommittees.SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.GenderId == Gender.MaleGuid);

        var allMembers = extraParliamentaryCommittees.SelectMany(c => c.Memberships).Count(m => m.IsActive);
        var allFemaleMembers = extraParliamentaryCommittees.SelectMany(c => c.Memberships).Count(m => m.IsActive && m.Person!.GenderId == Gender.FemaleGuid);
        var allMaleMembers = extraParliamentaryCommittees.SelectMany(c => c.Memberships).Count(m => m.IsActive && m.Person!.GenderId == Gender.MaleGuid);

        var allGermanCandidates = allExtraParliamentaryCommittees.SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.LanguageId == Language.GermanGuid);
        var allGermanMembers = extraParliamentaryCommittees.SelectMany(c => c.Memberships).Count(m => m.IsActive && m.Person!.LanguageId == Language.GermanGuid);
        var allFrenchCandidates = allExtraParliamentaryCommittees.SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.LanguageId == Language.FrenchGuid);
        var allFrenchMembers = extraParliamentaryCommittees.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.FrenchGuid);
        var allItalianCandidates = allExtraParliamentaryCommittees.SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.LanguageId == Language.ItalianGuid);
        var allItalianMembers = extraParliamentaryCommittees.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.ItalianGuid);
        var allRomanshCandidates = allExtraParliamentaryCommittees.SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.LanguageId == Language.RomanshGuid);
        var allRomanshMembers = extraParliamentaryCommittees.SelectMany(c => c.Memberships).Count(m => m.Person!.LanguageId == Language.RomanshGuid);

        var allCandidatesManagementCommittees = allManagementCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected);
        var allFemaleCandidatesManagementCommittees = allManagementCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.GenderId == Gender.FemaleGuid);
        var allMaleCandidatesManagementCommittees = allManagementCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.GenderId == Gender.MaleGuid);
        var allGermanCandidatesManagementCommittees = allManagementCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.LanguageId == Language.GermanGuid);
        var allFrenchCandidatesManagementCommittees = allManagementCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.LanguageId == Language.FrenchGuid);
        var allItalianCandidatesManagementCommittees = allManagementCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.LanguageId == Language.ItalianGuid);
        var allRomanshCandidatesManagementCommittees = allManagementCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.LanguageId == Language.RomanshGuid);

        var allCandidatesFederalAgenciesCommittees = allFederalAgenciesCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected);
        var allFemaleCandidatesFederalAgenciesCommittees = allFederalAgenciesCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.GenderId == Gender.FemaleGuid);
        var allMaleCandidatesFederalAgenciesCommittees = allFederalAgenciesCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.GenderId == Gender.MaleGuid);
        var allGermanCandidatesFederalAgenciesCommittees = allFederalAgenciesCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.LanguageId == Language.GermanGuid);
        var allFrenchCandidatesFederalAgenciesCommittees = allFederalAgenciesCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.LanguageId == Language.FrenchGuid);
        var allItalianCandidatesFederalAgenciesCommittees = allFederalAgenciesCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.LanguageId == Language.ItalianGuid);
        var allRomanshCandidatesFederalAgenciesCommittees = allFederalAgenciesCommittees.Where(c => c.IsValidated).SelectMany(c => c.MembershipCandidates).Count(m => m.IsSelected && m.Person!.LanguageId == Language.RomanshGuid);

        var membersWith12OrMoreYears = allExtraParliamentaryCommittees.SelectMany(c => c.MembershipCandidates).Count(m => m is { IsSelected: true, EstimatedTermOfOffice: > 12 });
        var federalDutyMembersWith12OrMoreYears = allExtraParliamentaryCommittees.SelectMany(c => c.MembershipCandidates).Count(m => m is { IsSelected: true, EstimatedTermOfOffice: > 12 } && m.Person!.FederalDuty);

        var multipleMemberships = allGeneralElectionCommittees
            .Where(c => c.IsValidated)
            .SelectMany(c => c.MembershipCandidates
                .Where(m => m is { IsSelected: true, PersonId: not null })
                .Select(m => new
                {
                    m.PersonId,
                    m.Person!.Surname,
                    m.Person!.GivenName,
                    m.Person!.GenderId,
                    CommitteeName = c.GetDescription()
                }))
            .GroupBy(x => new { x.PersonId, x.Surname, x.GivenName })
            .Where(g => g.Count() > 2)
            .Select(g => new InformationNotePersonMembershipCountDto
            {
                PersonId = (Guid)g.First().PersonId!,
                Surname = g.First().Surname,
                GivenName = g.First().GivenName,
                GenderId = g.First().GenderId,
                MembershipCount = g.Count(),
                CommitteeNames = g
                    .Select(x => x.CommitteeName)
                    .Distinct()
                    .Select(name => new InformationNoteCommitteeNameDto { Name = name })
                    .ToArray()
            })
            .OrderBy(p => p.MembershipCount)
            .ThenBy(p => p.Surname)
            .ToArray();

        var nonExtraParliamentaryCommitteesDto = FillNonExtraParliamentaryCommitteeData(allNonExtraParliamentaryCommittees, departments);

        var informationNoteDto = new InformationNoteDto
        {
            TermOfOfficeDateRange = generalElectionTermOfOfficeDate.BeginDate.Year + BusinessTexts.Term_Of_Office_Data_Separator + generalElectionTermOfOfficeDate.EndDate?.Year,
            LastTermOfOfficeDateRange = currentTermOfOfficeDate.BeginDate.Year + BusinessTexts.Term_Of_Office_Data_Separator + currentTermOfOfficeDate.EndDate?.Year,
            NumberOfMembers = numberOfSelectedCandidates,
            CurrentYear = DateOnly.FromDateTime(DateTime.Now).Year.ToString(CultureInfo.InvariantCulture),

            TotalCommittees = allGeneralElectionCommitteesUnfiltered.Length,
            ReleasedCommittees = releasedCommittees.Length,
            UnreleasedCommittees = unreleasedCommittees.Length,
            OneVacanciesTotal = allGeneralElectionCommitteesUnfiltered.Count(c => c.CalculatedVacancies == 1),
            TwoVacanciesTotal = allGeneralElectionCommitteesUnfiltered.Count(c => c.CalculatedVacancies == 2),
            ThreeVacanciesTotal = allGeneralElectionCommitteesUnfiltered.Count(c => c.CalculatedVacancies == 3),
            FourVacanciesTotal = allGeneralElectionCommitteesUnfiltered.Count(c => c.CalculatedVacancies >= 4),
            TotalAuthoritiesCommittees = allAuthoritiesCommittees.Length,
            ReleasedAuthoritiesCommittees = allAuthoritiesCommittees.Count(c => c.IsValidated),
            UnreleasedAuthoritiesCommittees = allAuthoritiesCommittees.Count(c => !c.IsValidated),
            OneVacanciesAuthoritiesCommittees = allAuthoritiesCommittees.Count(c => c.CalculatedVacancies == 1),
            TwoVacanciesAuthoritiesCommittees = allAuthoritiesCommittees.Count(c => c.CalculatedVacancies == 2),
            ThreeVacanciesAuthoritiesCommittees = allAuthoritiesCommittees.Count(c => c.CalculatedVacancies == 3),
            FourVacanciesAuthoritiesCommittees = allAuthoritiesCommittees.Count(c => c.CalculatedVacancies >= 4),
            TotalAdministrationCommittees = allAdministrationCommittees.Length,
            ReleasedAdministrationCommittees = allAdministrationCommittees.Count(c => c.IsValidated),
            UnreleasedAdministrationCommittees = allAdministrationCommittees.Count(c => !c.IsValidated),
            OneVacanciesAdministrationCommittees = allAdministrationCommittees.Count(c => c.CalculatedVacancies == 1),
            TwoVacanciesAdministrationCommittees = allAdministrationCommittees.Count(c => c.CalculatedVacancies == 2),
            ThreeVacanciesAdministrationCommittees = allAdministrationCommittees.Count(c => c.CalculatedVacancies == 3),
            FourVacanciesAdministrationCommittees = allAdministrationCommittees.Count(c => c.CalculatedVacancies >= 4),
            TotalManagementCommittees = allManagementCommittees.Length,
            ReleasedManagementCommittees = allManagementCommittees.Count(c => c.IsValidated),
            UnreleasedManagementCommittees = allManagementCommittees.Count(c => !c.IsValidated),
            OneVacanciesManagementCommittees = allManagementCommittees.Count(c => c.CalculatedVacancies == 1),
            TwoVacanciesManagementCommittees = allManagementCommittees.Count(c => c.CalculatedVacancies == 2),
            ThreeVacanciesManagementCommittees = allManagementCommittees.Count(c => c.CalculatedVacancies == 3),
            FourVacanciesManagementCommittees = allManagementCommittees.Count(c => c.CalculatedVacancies >= 4),
            TotalFederalAgenciesCommittees = allFederalAgenciesCommittees.Length,
            ReleasedFederalAgenciesCommittees = allFederalAgenciesCommittees.Count(c => c.IsValidated),
            UnreleasedFederalAgenciesCommittees = allFederalAgenciesCommittees.Count(c => !c.IsValidated),
            OneVacanciesFederalAgenciesCommittees = allFederalAgenciesCommittees.Count(c => c.CalculatedVacancies == 1),
            TwoVacanciesFederalAgenciesCommittees = allFederalAgenciesCommittees.Count(c => c.CalculatedVacancies == 2),
            ThreeVacanciesFederalAgenciesCommittees = allFederalAgenciesCommittees.Count(c => c.CalculatedVacancies == 3),
            FourVacanciesFederalAgenciesCommittees = allFederalAgenciesCommittees.Count(c => c.CalculatedVacancies >= 4),
            TotalExtraParliamentaryCommittees = allExtraParliamentaryCommittees.Length,
            UnreleasedExtraParliamentaryCommittees = allFederalAgenciesCommittees.Count(c => !c.IsValidated),
            PreviousTotalExtraParliamentaryCommittees = extraParliamentaryCommittees.Length,

            CurrentFemalePercentage = allCandidates == 0 ? 0 : Math.Round((decimal)allFemaleCandidates / allCandidates * 100, 2),
            PreviousFemalePercentage = allMembers == 0 ? 0 : Math.Round((decimal)allFemaleMembers / allMembers * 100, 2),
            ExpectedGenderPercentage = (decimal?)(managementCommitteeType != null ? managementCommitteeType.FemaleThreshold : 0),
            PreviousExpectedGenderPercentage = previousExpectedGenderPercentage,
            UnderstaffedFemaleCommittees = allExtraParliamentaryCommittees.Count(c => c.FemaleUnderStaffed),
            HeavyUnderstaffedFemaleCommittees = allExtraParliamentaryCommittees.Count(c => c.FemaleQuota <= 30),
            PreviousHeavyUnderstaffedFemaleCommittees = extraParliamentaryCommittees.Count(c => c.FemaleQuota <= 30),
            CurrentMalePercentage = Math.Round((decimal)allMaleCandidates / allCandidates * 100, 2),
            PreviousMalePercentage = allMembers == 0 ? 0 : Math.Round((decimal)allMaleMembers / allMembers * 100, 2),
            UnderstaffedMaleCommittees = allExtraParliamentaryCommittees.Count(c => c.MaleUnderStaffed),
            HeavyUnderstaffedMaleCommittees = allExtraParliamentaryCommittees.Count(c => c.MaleQuota <= 30),
            PreviousHeavyUnderstaffedMaleCommittees = extraParliamentaryCommittees.Count(c => c.MaleQuota <= 30),
            CurrentGermanPercentage = allCandidates == 0 ? 0 : Math.Round((decimal)allGermanCandidates / allCandidates * 100, 2),
            PreviousGermanPercentage = allMembers == 0 ? 0 : Math.Round((decimal)allGermanMembers / allMembers * 100, 2),
            CurrentFrenchPercentage = allCandidates == 0 ? 0 : Math.Round((decimal)allFrenchCandidates / allCandidates * 100, 2),
            PreviousFrenchPercentage = allMembers == 0 ? 0 : Math.Round((decimal)allFrenchMembers / allMembers * 100, 2),
            CurrentItalianPercentage = allCandidates == 0 ? 0 : Math.Round((decimal)allItalianCandidates / allCandidates * 100, 2),
            PreviousItalianPercentage = allMembers == 0 ? 0 : Math.Round((decimal)allItalianMembers / allMembers * 100, 2),
            CurrentRomanshPercentage = allCandidates == 0 ? 0 : Math.Round((decimal)allRomanshCandidates / allCandidates * 100, 2),
            PreviousRomanshPercentage = allMembers == 0 ? 0 : Math.Round((decimal)allRomanshMembers / allMembers * 100, 2),

            MissingGermanCommittees = allExtraParliamentaryCommittees.Count(c =>
                !c.MembershipCandidates.Any(m => m.IsSelected && m.Person!.LanguageId == Language.GermanGuid) &&
                (c.MembershipCandidates.Any(m => m.IsSelected && m.Person!.LanguageId == Language.ItalianGuid) ||
                c.MembershipCandidates.Any(m => m.IsSelected && m.Person!.LanguageId == Language.FrenchGuid))),
            PreviousMissingGermanCommittees = extraParliamentaryCommittees.Count(c =>
                !c.Memberships.Any(m => m.IsActive && m.Person!.LanguageId == Language.GermanGuid) &&
                (c.Memberships.Any(m => m.IsActive && m.Person!.LanguageId == Language.ItalianGuid) ||
                c.Memberships.Any(m => m.IsActive && m.Person!.LanguageId == Language.FrenchGuid))),
            MissingFrenchItalianCommittees = allExtraParliamentaryCommittees.Count(c =>
                c.MembershipCandidates.Any(m => m.IsSelected && m.Person!.LanguageId == Language.GermanGuid) &&
                !c.MembershipCandidates.Any(m => m.IsSelected && m.Person!.LanguageId == Language.ItalianGuid) &&
                !c.MembershipCandidates.Any(m => m.IsSelected && m.Person!.LanguageId == Language.FrenchGuid)),
            PreviousMissingFrenchItalianCommittees = extraParliamentaryCommittees.Count(c =>
                c.Memberships.Any(m => m.IsActive && m.Person!.LanguageId == Language.GermanGuid) &&
                !c.Memberships.Any(m => m.IsActive && m.Person!.LanguageId == Language.ItalianGuid) &&
                !c.Memberships.Any(m => m.IsActive && m.Person!.LanguageId == Language.FrenchGuid)),
            MissingFrenchCommittees = allExtraParliamentaryCommittees.Count(c =>
                c.MembershipCandidates.Any(m => m.IsSelected && m.Person!.LanguageId == Language.ItalianGuid) &&
                !c.MembershipCandidates.Any(m => m.IsSelected && m.Person!.LanguageId == Language.FrenchGuid)),
            PreviousMissingFrenchCommittees = extraParliamentaryCommittees.Count(c =>
                c.Memberships.Any(m => m.IsActive && m.Person!.LanguageId == Language.ItalianGuid) &&
                !c.Memberships.Any(m => m.IsActive && m.Person!.LanguageId == Language.FrenchGuid)),
            MissingItalianCommittees = allExtraParliamentaryCommittees.Count(c =>
                !c.MembershipCandidates.Any(m => m.IsSelected && m.Person!.LanguageId == Language.ItalianGuid) &&
                c.MembershipCandidates.Any(m => m.IsSelected && m.Person!.LanguageId == Language.FrenchGuid)),
            PreviousMissingItalianCommittees = extraParliamentaryCommittees.Count(c =>
                !c.Memberships.Any(m => m.IsActive && m.Person!.LanguageId == Language.ItalianGuid) &&
                c.Memberships.Any(m => m.IsActive && m.Person!.LanguageId == Language.FrenchGuid)),

            TotalMembersExtraParliamentaryCommittees = allCandidates,
            MoreThan12Years = membersWith12OrMoreYears,
            MoreThan12YearsFederalDuty = federalDutyMembersWith12OrMoreYears,
            MinimalFemaleThreshold = managementCommitteeType != null ? (decimal)managementCommitteeType.FemaleThreshold! : 0,
            MinimalMaleThreshold = managementCommitteeType != null ? (decimal)managementCommitteeType.MaleThreshold! : 0,
            MinimalGermanThreshold = managementCommitteeType != null ? (decimal)managementCommitteeType.GermanThresholdPercentage! : 0,
            MinimalFrenchThreshold = managementCommitteeType != null ? (decimal)managementCommitteeType.FrenchThresholdPercentage! : 0,
            MinimalItalianThreshold = managementCommitteeType != null ? (decimal)managementCommitteeType.ItalianThresholdPercentage! : 0,
            MinimalRomanshThreshold = managementCommitteeType != null ? (decimal)managementCommitteeType.RomanshThresholdPercentage! : 0,

            CurrentFemaleThresholdManagementCommittees = allCandidatesManagementCommittees == 0 ? 0 : Math.Round((decimal)allFemaleCandidatesManagementCommittees / allCandidatesManagementCommittees * 100, 2),
            CurrentMaleThresholdManagementCommittees = allCandidatesManagementCommittees == 0 ? 0 : Math.Round((decimal)allMaleCandidatesManagementCommittees / allCandidatesManagementCommittees * 100, 2),
            CurrentGermanThresholdManagementCommittees = allCandidatesManagementCommittees == 0 ? 0 : Math.Round((decimal)allGermanCandidatesManagementCommittees / allCandidatesManagementCommittees * 100, 2),
            CurrentFrenchThresholdManagementCommittees = allCandidatesManagementCommittees == 0 ? 0 : Math.Round((decimal)allFrenchCandidatesManagementCommittees / allCandidatesManagementCommittees * 100, 2),
            CurrentItalianThresholdManagementCommittees = allCandidatesManagementCommittees == 0 ? 0 : Math.Round((decimal)allItalianCandidatesManagementCommittees / allCandidatesManagementCommittees * 100, 2),
            CurrentRomanshThresholdManagementCommittees = allCandidatesManagementCommittees == 0 ? 0 : Math.Round((decimal)allRomanshCandidatesManagementCommittees / allCandidatesManagementCommittees * 100, 2),

            CurrentFemaleThresholdFederalAgenciesCommittees = allCandidatesFederalAgenciesCommittees == 0 ? 0 : Math.Round((decimal)allFemaleCandidatesFederalAgenciesCommittees / allCandidatesFederalAgenciesCommittees * 100, 2),
            CurrentMaleThresholdFederalAgenciesCommittees = allCandidatesFederalAgenciesCommittees == 0 ? 0 : Math.Round((decimal)allMaleCandidatesFederalAgenciesCommittees / allCandidatesFederalAgenciesCommittees * 100, 2),
            CurrentGermanThresholdFederalAgenciesCommittees = allCandidatesFederalAgenciesCommittees == 0 ? 0 : Math.Round((decimal)allGermanCandidatesFederalAgenciesCommittees / allCandidatesFederalAgenciesCommittees * 100, 2),
            CurrentFrenchThresholdFederalAgenciesCommittees = allCandidatesFederalAgenciesCommittees == 0 ? 0 : Math.Round((decimal)allFrenchCandidatesFederalAgenciesCommittees / allCandidatesFederalAgenciesCommittees * 100, 2),
            CurrentItalianThresholdFederalAgenciesCommittees = allCandidatesFederalAgenciesCommittees == 0 ? 0 : Math.Round((decimal)allItalianCandidatesFederalAgenciesCommittees / allCandidatesFederalAgenciesCommittees * 100, 2),
            CurrentRomanshThresholdFederalAgenciesCommittees = allCandidatesFederalAgenciesCommittees == 0 ? 0 : Math.Round((decimal)allRomanshCandidatesFederalAgenciesCommittees / allCandidatesFederalAgenciesCommittees * 100, 2),

            UnderstaffedFemaleManagementCommittees = allManagementCommittees.Count(c => c is { FemaleUnderStaffed: true, IsValidated: true }),
            UnderstaffedMaleManagementCommittees = allManagementCommittees.Count(c => c is { MaleUnderStaffed: true, IsValidated: true }),

            TotalMembersWith3Memberships = multipleMemberships.Count(m => m.MembershipCount == 3),
            FemaleMembersWith3Memberships = multipleMemberships.Count(m => m.MembershipCount == 3 && m.GenderId == Gender.FemaleGuid),
            MaleMembersWith3Memberships = multipleMemberships.Count(m => m.MembershipCount == 3 && m.GenderId == Gender.MaleGuid),
            TotalMembersWith4Memberships = multipleMemberships.Count(m => m.MembershipCount == 4),
            FemaleMembersWith4Memberships = multipleMemberships.Count(m => m.MembershipCount == 4 && m.GenderId == Gender.FemaleGuid),
            MaleMembersWith4Memberships = multipleMemberships.Count(m => m.MembershipCount == 4 && m.GenderId == Gender.MaleGuid),
            TotalMembersWith5Memberships = multipleMemberships.Count(m => m.MembershipCount == 5),
            FemaleMembersWith5Memberships = multipleMemberships.Count(m => m.MembershipCount == 5 && m.GenderId == Gender.FemaleGuid),
            MaleMembersWith5Memberships = multipleMemberships.Count(m => m.MembershipCount == 5 && m.GenderId == Gender.MaleGuid),
            TotalMembersWith6Memberships = multipleMemberships.Count(m => m.MembershipCount >= 6),
            FemaleMembersWith6Memberships = multipleMemberships.Count(m => m.MembershipCount >= 6 && m.GenderId == Gender.FemaleGuid),
            MaleMembersWith6Memberships = multipleMemberships.Count(m => m.MembershipCount >= 6 && m.GenderId == Gender.MaleGuid),
            TotalMultipleMembers = multipleMemberships.Length,
            FemaleMultipleMembers = multipleMemberships.Count(m => m.GenderId == Gender.FemaleGuid),
            MaleMultipleMembers = multipleMemberships.Count(m => m.GenderId == Gender.MaleGuid),

            ReleasedCommitteesByDepartmentAndType = releasedCommitteesDto,
            UnreleasedCommitteesByDepartmentAndType = unreleasedCommitteesDto,
            GenderUnderstaffedCommitteesByDepartmentAndType = genderUnderstaffedCommitteesDto,
            LanguageUnderstaffedCommitteesByDepartmentAndType = languageUnderstaffedCommitteesDto,
            LongerDutyCommitteesByDepartmentAndType = moreThan12YearsCommitteesDto,
            NonExtraParliamentCommitteesByDepartmentAndType = nonExtraParliamentaryCommitteesDto,

            PersonWithMultipleMemberships = multipleMemberships
        };

        var documentStream = await _documentService.CreateWordFromTemplate($"Templates/{template}.docx", informationNoteDto, "informationNote");

        return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd} {BusinessTexts.Information_Note_Filename}.docx", documentStream);
    }

    private async Task<ReportGeneralElectionCommitteeDto[]> ReplaceSelfOrganisedMemberFunctions(ReportGeneralElectionCommitteeDto[] generalElectionCommittees)
    {
        // this whole block is necessary because of self organized committees, all functions are made to members there (BKDO-2475)
        var memberFunction = await _masterDataRepository.GetById<Function>(Function.MemberGuid);

        foreach (var committee in generalElectionCommittees)
        {
            if (committee.SelfOrganized == true)
            {
                foreach (var m in committee.Memberships)
                {
                    m.Function = memberFunction;
                }
            }
        }

        return generalElectionCommittees;
    }

    private static List<InformationNoteNonExtraParliamentaryCommitteeDepartmentDto> FillNonExtraParliamentaryCommitteeData(GeneralElectionCommittee[] committees, IEnumerable<Department> departments)
    {
        var departmentList = new List<InformationNoteNonExtraParliamentaryCommitteeDepartmentDto>();

        var dtoDict = new Dictionary<string, InformationNoteNonExtraParliamentaryCommitteeDepartmentDto>();

        foreach (var department in departments)
        {
            var dto = new InformationNoteNonExtraParliamentaryCommitteeDepartmentDto
            {
                Name = department.GetText()
            };

            dtoDict[department.GetText()] = dto;
            departmentList.Add(dto);

            var filteredCommittees = committees
                .Where(c => c.DepartmentId == department.Id)
                .OrderBy(c => c.Committee!.CommitteeNumber)
                .ToArray();

            var groupedCommittees = filteredCommittees
                .GroupBy(c => new { CommitteeTypeId = c.CommitteeType!.Id, CommitteeTypeName = c.CommitteeType.GetText() })
                .ToArray();

            var committeeTypeList = new List<InformationNoteNonExtraParliamentaryCommitteeTypeDto>();

            foreach (var committeeGroup in groupedCommittees)
            {
                var committeeList =
                    committeeGroup
                        .Select(g =>
                            new InformationNoteNonExtraParliamentaryCommitteeData
                            {
                                Name = g.GetDescription(),
                                GermanText = $"{g.GermanCount} ({g.GermanQuota} %)",
                                FrenchText = $"{g.FrenchCount} ({g.FrenchQuota} %)",
                                ItalianText = $"{g.ItalianCount} ({g.ItalianQuota} %)",
                                RomanshText = $"{g.RomanshCount} ({g.RomanshQuota} %)",
                                FemaleText = $"{g.FemaleCount} ({g.FemaleQuota} %)",
                                MaleText = $"{g.MaleCount} ({g.MaleQuota} %)"
                            })
                        .ToArray();

                var committeeTypeDto = new InformationNoteNonExtraParliamentaryCommitteeTypeDto
                {
                    CommitteeType = committeeGroup.Key.CommitteeTypeName,
                    Committees = committeeList.OrderBy(c => c.Name)
                };

                committeeTypeList.Add(committeeTypeDto);
            }

            dtoDict[department.GetText()].CommitteeTypes = committeeTypeList.OrderBy(ct => ct.CommitteeType);
        }
        return departmentList;
    }


    private async Task<(string fileName, Stream content)> GenerateVacanciesReport(ReportFilterParametersDto filterDto)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var departments = await _masterDataRepository.GetDepartments();
        departments = departments.Where(d => d.Uri != Department.BkUri).ToArray();

        var template = _cultureService.GetCurrentUiCulture().TwoLetterISOLanguageName == Language.French ? "Vacancies_Report_French" : "Vacancies_Report_German";

        var nextTermOfOfficeDate = await _termOfOfficeDateService.GetNextTermOfOfficeDate();

        // PP Machts... Funktioniert der noch nach Einschränkung Mitgliedschaft
        var committees = (await _committeeRepository.GetByFilterForReport(filterDto, departmentId, officeId, committeeId)).ToArray();

        var generalElectionCommittees = committees.Select(ReportMapper.FromCommitteeToReportGeneralElectionCommitteeDto).ToArray();

        var reportDepartments = GetCommitteesByDepartment(generalElectionCommittees, departments, ReportCommitteeType.Vacancies);

        var vacanciesReportDto = new VacanciesReportDto
        {
            TermOfOfficeDateRange = nextTermOfOfficeDate.BeginDate.Year + BusinessTexts.Term_Of_Office_Data_Separator + nextTermOfOfficeDate.EndDate?.Year,
            OnlyReleased = filterDto.ReleasedCommittees,
            Departments = reportDepartments
        };

        var documentStream = await _documentService.CreateWordFromTemplate($"Templates/{template}.docx", vacanciesReportDto, "vacanciesReport");

        return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd} {BusinessTexts.Vacancies_Report_Filename}.docx", documentStream);
    }

    private static List<ReportDepartmentWithCommitteeTypeDto> GetCommitteesByDepartmentAndTypes(Committee[] committees, IEnumerable<Department> departments)
    {
        var departmentList = new List<ReportDepartmentWithCommitteeTypeDto>();
        var dtoDict = new Dictionary<string, ReportDepartmentWithCommitteeTypeDto>();

        foreach (var department in departments)
        {
            var dto = new ReportDepartmentWithCommitteeTypeDto
            {
                Name = department.GetText()
            };

            dtoDict[department.GetText()] = dto;
            departmentList.Add(dto);

            var filteredCommittees = committees
                .Where(c => c.DepartmentId == department.Id)
                .OrderBy(c => c.CommitteeNumber)
                .ToArray();

            var groupedCommittees = filteredCommittees
                .GroupBy(c => new { CommitteeTypeId = c.CommitteeType!.Id, CommitteeTypeName = c.CommitteeType.GetText() })
                .ToArray();

            var committeeTypeList = new List<ReportCommitteeTypeDto>();

            foreach (var committeeGroup in groupedCommittees)
            {
                var committeeList =
                    committeeGroup
                        .Select(g => new ReportCommitteeDto { Name = g.GetDescription() })
                        .ToArray();

                var committeeTypeDto = new ReportCommitteeTypeDto
                {
                    CommitteeType = committeeGroup.Key.CommitteeTypeName,
                    Committees = committeeList.OrderBy(c => c.Name)
                };

                committeeTypeList.Add(committeeTypeDto);
            }

            dtoDict[department.GetText()].CommitteeTypes = committeeTypeList.OrderBy(ct => ct.CommitteeType);
        }
        return departmentList;
    }

    private static List<ReportDepartmentWithCommitteeTypeDto> GetCommitteesByDepartmentAndTypesForInformationNote(GeneralElectionCommittee[] committees, IEnumerable<Department> departments, InformationNoteData informationNoteData)
    {
        var departmentList = new List<ReportDepartmentWithCommitteeTypeDto>();
        var dtoDict = new Dictionary<string, ReportDepartmentWithCommitteeTypeDto>();

        foreach (var department in departments)
        {
            var dto = new ReportDepartmentWithCommitteeTypeDto
            {
                Name = department.GetText()
            };

            dtoDict[department.GetText()] = dto;
            departmentList.Add(dto);

            var filteredCommittees = committees
                .Where(c => c.DepartmentId == department.Id)
                .OrderBy(c => c.Committee!.CommitteeNumber)
                .ToArray();

            var groupedCommittees = filteredCommittees
                .GroupBy(c => new { CommitteeTypeId = c.CommitteeType!.Id, CommitteeTypeName = c.CommitteeType.GetText() })
                .ToArray();

            var committeeTypeList = new List<ReportCommitteeTypeDto>();

            foreach (var committeeGroup in groupedCommittees)
            {
                var committeeList = new List<ReportCommitteeDto>();

                foreach (var committee in committeeGroup)
                {
                    var freeText = string.Empty;

                    if (informationNoteData == InformationNoteData.Vacancies)
                    {
                        var vacancies = committee.MinimalMembers - committee.MembershipCandidates.Count(m => m.IsSelected);
                        freeText = string.Format(CultureInfo.InvariantCulture, BusinessTexts.InformationNoteExport_Vacancies, vacancies);
                    }
                    else if (informationNoteData == InformationNoteData.Genders)
                    {
                        freeText = committee.FemaleQuota < committee.MaleQuota ?
                            string.Format(CultureInfo.InvariantCulture, BusinessTexts.InformationNoteExport_Female, committee.FemaleQuota) :
                            string.Format(CultureInfo.InvariantCulture, BusinessTexts.InformationNoteExport_Male, committee.MaleQuota);
                    }
                    else if (informationNoteData == InformationNoteData.Languages)
                    {
                        freeText = committee is { ItalianUnderStaffed: true, FrenchUnderStaffed: true } ?
                            BusinessTexts.InformationNoteExport_FrenchAndItalianMissing :
                            committee.ItalianUnderStaffed ? BusinessTexts.InformationNoteExport_ItalianMissing :
                            committee.FrenchUnderStaffed ? BusinessTexts.InformationNoteExport_FrenchMissing : string.Empty;
                    }

                    var committeeDto = new ReportCommitteeDto
                    {
                        Name = committee.GetDescription(),
                        FreeText = freeText
                    };

                    committeeList.Add(committeeDto);
                }

                var committeeTypeDto = new ReportCommitteeTypeDto
                {
                    CommitteeType = committeeGroup.Key.CommitteeTypeName,
                    Committees = committeeList.OrderBy(c => c.Name)
                };

                committeeTypeList.Add(committeeTypeDto);
            }

            dtoDict[department.GetText()].CommitteeTypes = committeeTypeList.OrderBy(ct => ct.CommitteeType);
        }
        return departmentList;
    }

    private List<ReportDepartmentWithCommitteesDto> GetCommitteesByDepartment(ReportGeneralElectionCommitteeDto[] committees, IEnumerable<Department> departments, ReportCommitteeType type, TermOfOfficeDate? termOfOfficeDate = null)
    {
        var departmentList = new List<ReportDepartmentWithCommitteesDto>();
        var dtoDict = new Dictionary<string, ReportDepartmentWithCommitteesDto>();

        foreach (var department in departments)
        {
            var dto = new ReportDepartmentWithCommitteesDto
            {
                Name = department.GetText()
            };

            dtoDict[department.GetText()] = dto;
            departmentList.Add(dto);

            var filteredCommittees = committees
                .Where(c => c.DepartmentId == department.Id)
                .OrderBy(c => c.CommitteeNumber)
                .ToArray();

            var committeeList = new List<ReportCommitteeDto>();

            foreach (var committee in filteredCommittees)
            {
                var sanitizedSelectionProcedure = ReportExportHtmlSanitizer.Sanitize(committee.SelectionProcedure);
                string freeText;
                var freeText2 = "";
                var membershipCount = committee.Memberships.Count;
                var onlyAddWhenData = false;

                if (type == ReportCommitteeType.SelectionProcedure)
                {
                    freeText = string.Format(CultureInfo.InvariantCulture, BusinessTexts.Report_SelectionProcedure, sanitizedSelectionProcedure.CleanText);
                }
                else if (type == ReportCommitteeType.Vacancies)
                {
                    onlyAddWhenData = true;
                    freeText = string.Join(", ", committee.MembershipAdditionsInGeneralElection.Select(m => m.GetText()));
                    freeText2 = !string.IsNullOrWhiteSpace(committee.LinkHomepageGerman) ? committee.LinkHomepageGerman : !string.IsNullOrWhiteSpace(committee.LinkHomepageFrench) ? committee.LinkHomepageFrench : string.Empty;
                    membershipCount = committee.VacanciesGeneralElection ?? 0;
                }
                else
                {
                    freeText = termOfOfficeDate != null && committee.EndDate < termOfOfficeDate.EndDate && committee.EndDate > termOfOfficeDate.BeginDate ? string.Format(CultureInfo.InvariantCulture, BusinessTexts.Report_EndDateInPast, committee.EndDate.Value.ToShortDateString()) : termOfOfficeDate != null && committee.BeginDate > termOfOfficeDate.BeginDate && committee.BeginDate < termOfOfficeDate.EndDate ? string.Format(CultureInfo.InvariantCulture, BusinessTexts.Report_BeginDateInFuture, committee.BeginDate.ToShortDateString()) : string.Empty;
                }

                if (!onlyAddWhenData || membershipCount > 0)
                {
                    var justificationSanitization = ReportExportHtmlSanitizer.Sanitize(committee.JustificationMembers);
                    var committeeDto = new ReportCommitteeDto
                    {
                        Name = committee.GetDescription(),
                        MemberCount = membershipCount,
                        Justification = justificationSanitization.CleanText,
                        HasOpenJustificationChanges = justificationSanitization.HasOpenChanges,
                        JustificationUrl = BuildCommitteeJustificationUrl(committee.Id),
                        FreeText = freeText,
                        FreeText2 = freeText2
                    };

                    committeeList.Add(committeeDto);
                }
            }

            dtoDict[department.GetText()].Committees = committeeList;
        }
        return departmentList;
    }

    private static List<ReportDepartmentWithCommitteesDto> GetCommitteesByDepartmentForMembershipDuration(GeneralElectionCommittee[] committees, IEnumerable<Department> departments)
    {
        var departmentList = new List<ReportDepartmentWithCommitteesDto>();
        var dtoDict = new Dictionary<string, ReportDepartmentWithCommitteesDto>();

        foreach (var department in departments)
        {
            var dto = new ReportDepartmentWithCommitteesDto
            {
                Name = department.GetText()
            };

            dtoDict[department.GetText()] = dto;
            departmentList.Add(dto);

            var filteredCommittees = committees
                .Where(c => c.DepartmentId == department.Id)
                .OrderBy(c => c.Committee!.CommitteeNumber)
                .ToArray();

            var moreThan12YearsDto = filteredCommittees
                .Select(c => new
                {
                    CommitteeName = c.GetDescription(),
                    Members = c.MembershipCandidates
                        .Where(m => m is { IsSelected: true, EstimatedTermOfOffice: > 12 }),
                    FederalDutyMembers = c.MembershipCandidates
                        .Where(m => m is { IsSelected: true, EstimatedTermOfOffice: > 12 } && m.Person!.FederalDuty)
                })
                .Where(x => x.Members.Any()) // removes empty committees
                .Select(x => new CommitteeNameLongerDutyDto
                {
                    Name = x.CommitteeName,
                    MemberCount = x.Members.Count(),
                    FederalMemberCount = x.FederalDutyMembers.Count()
                })
                .ToArray();

            var committeeList =
                moreThan12YearsDto
                    .Select(x => new ReportCommitteeDto
                    {
                        Name = x.Name,
                        FreeText =
                            x is { MemberCount: > 0, FederalMemberCount: > 0 } && x.MemberCount != x.FederalMemberCount ? string.Format(CultureInfo.InvariantCulture, BusinessTexts.InformationNoteExport_MemberAndFederalMemberCount, x.MemberCount, x.FederalMemberCount) :
                            x is { MemberCount: > 0, FederalMemberCount: > 0 } && x.MemberCount == x.FederalMemberCount ? string.Format(CultureInfo.InvariantCulture, BusinessTexts.InformationNoteExport_FederalMemberCount, x.FederalMemberCount) :
                            string.Format(CultureInfo.InvariantCulture, BusinessTexts.InformationNoteExport_MemberCount, x.MemberCount)
                    })
                    .ToArray();

            dtoDict[department.GetText()].Committees = committeeList;
        }
        return departmentList;
    }

    private async Task<List<ReportDepartmentWithCommitteesAndGendersDto>> GetCommitteesWithGendersByDepartment(ReportGeneralElectionCommitteeDto[] committees, IEnumerable<Department> departments)
    {
        var genderMeasures = (await _generalMeasureRepository.GetGeneralGenderMeasures()).ToArray();

        var departmentList = new List<ReportDepartmentWithCommitteesAndGendersDto>();
        var dtoDict = new Dictionary<string, ReportDepartmentWithCommitteesAndGendersDto>();

        foreach (var department in departments)
        {
            var measureSanitization = ReportExportHtmlSanitizer.Sanitize(genderMeasures.FirstOrDefault(g => g.DepartmentId == department.Id)?.Description);
            var dto = new ReportDepartmentWithCommitteesAndGendersDto
            {
                Name = department.GetText(),
                Measure = measureSanitization.CleanText,
                HasOpenMeasureChanges = measureSanitization.HasOpenChanges,
                MeasureUrl = BuildGeneralMeasuresJustificationUrl()
            };

            dtoDict[department.GetText()] = dto;
            departmentList.Add(dto);

            var filteredCommittees = committees
                .Where(c => c.DepartmentId == department.Id)
                .OrderBy(c => c.Committee!.CommitteeNumber)
                .ToArray();

            var committeeList = new List<ReportCommitteeGenderMissingDto>();

            foreach (var committee in filteredCommittees.Where(c => c.ActiveMemberCount > 0))
            {
                var femalePercentage = Math.Round((decimal)committee.FemaleCount / committee.ActiveMemberCount * 100, 2);
                var malePercentage = Math.Round((decimal)committee.MaleCount / committee.ActiveMemberCount * 100, 2);

                if (femalePercentage < (decimal)committee.CommitteeType?.FemaleThreshold! || malePercentage < (decimal)committee.CommitteeType?.MaleThreshold!)
                {
                    var committeeMeasureSanitization = ReportExportHtmlSanitizer.Sanitize(committee.MeasuresGenders);
                    var justificationSanitization = ReportExportHtmlSanitizer.Sanitize(committee.JustificationGenders);
                    var committeeDto = new ReportCommitteeGenderMissingDto
                    {
                        Name = committee.GetDescription(),
                        MemberCount = committee.Memberships.Count,
                        Measure = committeeMeasureSanitization.CleanText,
                        HasOpenMeasureChanges = committeeMeasureSanitization.HasOpenChanges,
                        Justification = justificationSanitization.CleanText,
                        HasOpenJustificationChanges = justificationSanitization.HasOpenChanges,
                        JustificationUrl = BuildCommitteeJustificationUrl(committee.Id),
                        FemaleMissingPercentage = femalePercentage,
                        MaleMissingPercentage = malePercentage
                    };

                    committeeList.Add(committeeDto);
                }
            }
            dtoDict[department.GetText()].Committees = committeeList;
        }
        return departmentList;
    }

    private IEnumerable<ReportCommitteeGenderMissingDto> GetCommitteesWithGenders(IEnumerable<ReportGeneralElectionCommitteeDto> committees)
    {
        var committeeList = new List<ReportCommitteeGenderMissingDto>();

        foreach (var committee in committees.Where(c => c.ActiveMemberCount > 0))
        {
            var femalePercentage = Math.Round((decimal)committee.FemaleCount / committee.ActiveMemberCount * 100, 2);
            var malePercentage = Math.Round((decimal)committee.MaleCount / committee.ActiveMemberCount * 100, 2);

            if (femalePercentage < (decimal)committee.CommitteeType?.FemaleThreshold! || malePercentage < (decimal)committee.CommitteeType?.MaleThreshold!)
            {
                var measureSanitization = ReportExportHtmlSanitizer.Sanitize(committee.MeasuresGenders);
                var justificationSanitization = ReportExportHtmlSanitizer.Sanitize(committee.JustificationGenders);
                var committeeDto = new ReportCommitteeGenderMissingDto
                {
                    Name = committee.GetDescription(),
                    MemberCount = committee.Memberships.Count,
                    Measure = measureSanitization.CleanText,
                    HasOpenMeasureChanges = measureSanitization.HasOpenChanges,
                    Justification = justificationSanitization.CleanText,
                    HasOpenJustificationChanges = justificationSanitization.HasOpenChanges,
                    JustificationUrl = BuildCommitteeJustificationUrl(committee.Id),
                    FemaleMissingPercentage = femalePercentage,
                    MaleMissingPercentage = malePercentage
                };
                committeeList.Add(committeeDto);
            }

        }

        return committeeList.OrderBy(c => c.Name);
    }

    private async Task<List<ReportDepartmentWithCommitteesAndLanguagesDto>> GetCommitteesWithLanguagesByDepartment(ReportGeneralElectionCommitteeDto[] committees, IEnumerable<Department> departments)
    {
        var languageMeasures = (await _generalMeasureRepository.GetGeneralLanguageMeasures()).ToArray();

        var departmentList = new List<ReportDepartmentWithCommitteesAndLanguagesDto>();
        var dtoDict = new Dictionary<string, ReportDepartmentWithCommitteesAndLanguagesDto>();

        foreach (var department in departments)
        {
            var measureSanitization = ReportExportHtmlSanitizer.Sanitize(languageMeasures.FirstOrDefault(g => g.DepartmentId == department.Id)?.Description);
            var dto = new ReportDepartmentWithCommitteesAndLanguagesDto
            {
                Name = department.GetText(),
                Measure = measureSanitization.CleanText,
                HasOpenMeasureChanges = measureSanitization.HasOpenChanges,
                MeasureUrl = BuildGeneralMeasuresJustificationUrl()
            };

            dtoDict[department.GetText()] = dto;
            departmentList.Add(dto);

            var filteredCommittees = committees
                .Where(c => c.DepartmentId == department.Id)
                .OrderBy(c => c.Committee!.CommitteeNumber)
                .ToArray();

            var committeeList = new List<ReportCommitteeLanguageMissingDto>();

            foreach (var committee in filteredCommittees.Where(c => c.ActiveMemberCount > 0))
            {
                var italianMissing = false;
                var frenchMissing = false;

                if (committee.CommitteeType!.GermanMinimalThreshold > 0)
                {
                    // minimal members per language
                    italianMissing = committee.ItalianCount < committee.CommitteeType!.ItalianMinimalThreshold;
                    frenchMissing = committee.FrenchCount < committee.CommitteeType!.FrenchMinimalThreshold;
                }
                else
                {
                    // minimal member percentage per language
                    var italian = Math.Round((decimal)committee.ItalianCount / committee.ActiveMemberCount * 100, 2);
                    var french = Math.Round((decimal)committee.FrenchCount / committee.ActiveMemberCount * 100, 2);

                    if (italian < (decimal)committee.CommitteeType!.ItalianThresholdPercentage!)
                    {
                        italianMissing = true;
                    }

                    if (french < (decimal)committee.CommitteeType!.FrenchThresholdPercentage!)
                    {
                        frenchMissing = true;
                    }
                }

                if (italianMissing || frenchMissing)
                {
                    var justificationSanitization = ReportExportHtmlSanitizer.Sanitize(committee.JustificationLanguages);
                    var committeeMeasureSanitization = ReportExportHtmlSanitizer.Sanitize(committee.MeasuresLanguages);
                    var committeeDto = new ReportCommitteeLanguageMissingDto
                    {
                        Name = committee.GetDescription(),
                        MemberCount = committee.Memberships.Count,
                        Measure = committeeMeasureSanitization.CleanText,
                        HasOpenMeasureChanges = committeeMeasureSanitization.HasOpenChanges,
                        Justification = justificationSanitization.CleanText,
                        HasOpenJustificationChanges = justificationSanitization.HasOpenChanges,
                        JustificationUrl = BuildCommitteeJustificationUrl(committee.Id),
                        ItalianMissing = italianMissing,
                        FrenchMissing = frenchMissing
                    };

                    committeeList.Add(committeeDto);
                }
            }

            dtoDict[department.GetText()].Committees = committeeList;
        }

        return departmentList;
    }

    private IEnumerable<ReportCommitteeLanguageMissingDto> GetCommitteesWithLanguages(IEnumerable<ReportGeneralElectionCommitteeDto> committees)
    {
        var committeeList = new List<ReportCommitteeLanguageMissingDto>();

        foreach (var committee in committees.Where(c => c.ActiveMemberCount > 0))
        {
            var italianMissing = false;
            var frenchMissing = false;

            if (committee.CommitteeType!.GermanMinimalThreshold > 0)
            {
                // minimal members per language
                italianMissing = committee.ItalianCount < committee.CommitteeType!.ItalianMinimalThreshold;
                frenchMissing = committee.FrenchCount < committee.CommitteeType!.FrenchMinimalThreshold;
            }
            else
            {
                // minimal member percentage per language
                var italian = Math.Round((decimal)committee.ItalianCount / committee.ActiveMemberCount * 100, 2);
                var french = Math.Round((decimal)committee.FrenchCount / committee.ActiveMemberCount * 100, 2);

                if (italian < (decimal)committee.CommitteeType!.ItalianThresholdPercentage!)
                {
                    italianMissing = true;
                }

                if (french < (decimal)committee.CommitteeType!.FrenchThresholdPercentage!)
                {
                    frenchMissing = true;
                }
            }

            if (italianMissing || frenchMissing)
            {
                var measureSanitization = ReportExportHtmlSanitizer.Sanitize(committee.MeasuresLanguages);
                var justificationSanitization = ReportExportHtmlSanitizer.Sanitize(committee.JustificationLanguages);
                var committeeDto = new ReportCommitteeLanguageMissingDto
                {
                    Name = committee.GetDescription(),
                    MemberCount = committee.Memberships.Count,
                    Measure = measureSanitization.CleanText,
                    HasOpenMeasureChanges = measureSanitization.HasOpenChanges,
                    Justification = justificationSanitization.CleanText,
                    HasOpenJustificationChanges = justificationSanitization.HasOpenChanges,
                    JustificationUrl = BuildCommitteeJustificationUrl(committee.Id),
                    ItalianMissing = italianMissing,
                    FrenchMissing = frenchMissing
                };

                committeeList.Add(committeeDto);
            }
        }

        return committeeList.OrderBy(c => c.Name);
    }

    private async Task<List<ReportDepartmentWithCommitteesAndLanguagesDto>> GetCommitteesWithLanguagePercentagesByDepartment(ReportGeneralElectionCommitteeDto[] committees, IEnumerable<Department> departments)
    {
        var languageMeasures = (await _generalMeasureRepository.GetGeneralLanguageMeasures()).ToArray();

        var departmentList = new List<ReportDepartmentWithCommitteesAndLanguagesDto>();
        var dtoDict = new Dictionary<string, ReportDepartmentWithCommitteesAndLanguagesDto>();

        foreach (var department in departments)
        {
            var measureSanitization = ReportExportHtmlSanitizer.Sanitize(languageMeasures.FirstOrDefault(g => g.DepartmentId == department.Id)?.Description);
            var dto = new ReportDepartmentWithCommitteesAndLanguagesDto
            {
                Name = department.GetText(),
                Measure = measureSanitization.CleanText,
                HasOpenMeasureChanges = measureSanitization.HasOpenChanges,
                MeasureUrl = BuildGeneralMeasuresJustificationUrl()
            };

            dtoDict[department.GetText()] = dto;
            departmentList.Add(dto);

            var filteredCommittees = committees
                .Where(c => c.DepartmentId == department.Id)
                .OrderBy(c => c.Committee!.CommitteeNumber)
                .ToArray();

            var committeeList = new List<ReportCommitteeLanguageMissingDto>();

            foreach (var committee in filteredCommittees.Where(c => c.Memberships.Count != 0 && c.ActiveMemberCount > 0))
            {
                var germanMembers = committee.Memberships.Where(m => m.IsSelected && !m.IsDeleted).Select(m => m.Person!).Count(p => p.LanguageId == Guid.Parse(Language.GermanId));
                var frenchMembers = committee.Memberships.Where(m => m.IsSelected && !m.IsDeleted).Select(m => m.Person!).Count(p => p.LanguageId == Guid.Parse(Language.FrenchId));
                var italianMembers = committee.Memberships.Where(m => m.IsSelected && !m.IsDeleted).Select(m => m.Person!).Count(p => p.LanguageId == Guid.Parse(Language.ItalianId));
                var romanshMembers = committee.Memberships.Where(m => m.IsSelected && !m.IsDeleted).Select(m => m.Person!).Count(p => p.LanguageId == Guid.Parse(Language.RomanshId));

                var german = Math.Round((decimal)germanMembers / committee.ActiveMemberCount * 100, 2);
                var french = Math.Round((decimal)frenchMembers / committee.ActiveMemberCount * 100, 2);
                var italian = Math.Round((decimal)italianMembers / committee.ActiveMemberCount * 100, 2);
                var romansh = Math.Round((decimal)romanshMembers / committee.ActiveMemberCount * 100, 2);

                if (italian < (decimal)committee.CommitteeType!.ItalianThresholdPercentage! || french < (decimal)committee.CommitteeType!.FrenchThresholdPercentage! ||
                    german < (decimal)committee.CommitteeType!.GermanThresholdPercentage!)
                {
                    var committeeMeasureSanitization = ReportExportHtmlSanitizer.Sanitize(committee.MeasuresLanguages);
                    var justificationSanitization = ReportExportHtmlSanitizer.Sanitize(committee.JustificationLanguages);
                    var committeeDto = new ReportCommitteeLanguageMissingDto
                    {
                        Name = committee.GetDescription(),
                        MemberCount = committee.Memberships.Count,
                        Measure = committeeMeasureSanitization.CleanText,
                        HasOpenMeasureChanges = committeeMeasureSanitization.HasOpenChanges,
                        Justification = justificationSanitization.CleanText,
                        HasOpenJustificationChanges = justificationSanitization.HasOpenChanges,
                        JustificationUrl = BuildCommitteeJustificationUrl(committee.Id),
                        GermanPercentage = german,
                        FrenchPercentage = french,
                        ItalianPercentage = italian,
                        RomanshPercentage = romansh
                    };

                    committeeList.Add(committeeDto);
                }
            }

            dtoDict[department.GetText()].Committees = committeeList;
        }

        return departmentList;
    }

    private List<ReportDepartmentWithCommitteesAndMembersDto> GetCommitteesAndMembersByDepartment(ReportGeneralElectionCommitteeDto[] committees, IEnumerable<Department> departments, ReportMembershipType type, TermOfOfficeDate? termOfOffice = null, LegislaturePeriod? legislaturePeriod = null)
    {
        var departmentList = new List<ReportDepartmentWithCommitteesAndMembersDto>();
        var dtoDict = new Dictionary<string, ReportDepartmentWithCommitteesAndMembersDto>();

        foreach (var department in departments)
        {
            var dto = new ReportDepartmentWithCommitteesAndMembersDto
            {
                Name = department.GetText()
            };

            dtoDict[department.GetText()] = dto;
            departmentList.Add(dto);

            var filteredCommittees = committees
                .Where(c => c.DepartmentId == department.Id)
                .OrderBy(c => c.Committee!.CommitteeNumber)
                .ToArray();

            var committeeList = new List<ReportCommitteeWithMemberDetailDto>();

            foreach (var committee in filteredCommittees)
            {
                var members = GetMembershipsByType(committee, type, termOfOffice, legislaturePeriod).ToArray();

                if (members.Length > 0)
                {
                    var committeeDto = new ReportCommitteeWithMemberDetailDto
                    {
                        Name = committee.GetDescription(),
                        MemberCount = members.Length,
                        Members = members
                    };

                    committeeList.Add(committeeDto);
                }
            }

            dtoDict[department.GetText()].Committees = committeeList;
        }

        return departmentList;
    }

    private static List<ReportDepartmentDto> GetDepartmentsOnly(IEnumerable<Department> departments)
    {
        var departmentList = new List<ReportDepartmentDto>();

        foreach (var department in departments)
        {
            var dto = new ReportDepartmentDto
            {
                Name = department.GetText()
            };

            departmentList.Add(dto);
        }

        return departmentList;
    }

    private static IEnumerable<ReportCommitteeWithFreeTextDto> GetNonReleasedCommissions(IEnumerable<ReportGeneralElectionCommitteeDto> nonReleasedCommissions)
    {
        var committees = nonReleasedCommissions
            .Select(c => new ReportCommitteeWithFreeTextDto
            {
                Name = c.GetDescription()
            })
            .OrderBy(c => c.Name);

        return committees;
    }

    private static IEnumerable<ReportCommitteeWithFreeTextDto> GetFederalDutyMembershipsWithOffice(IEnumerable<ReportGeneralElectionCommitteeDto> generalElectionCommitteesWithMembers)
    {
        var committees = generalElectionCommitteesWithMembers
            .Select(c => new ReportCommitteeWithFreeTextDto
            {
                Name = c.GetDescription(),
                FreeText = string.Join(", ",
                    c.Memberships
                        .Where(m => m.Person is { FederalDuty: true })
                        .Select(m => $"{m.Person!.Surname}, {m.Person!.GivenName} ({m.Person?.Office?.GetDescription()} {m.Person?.Office?.Department?.GetDescription()})"))
            })
            .OrderBy(c => c.Name);

        return committees;
    }

    private static IEnumerable<ReportCommitteeWithFreeTextDto> GetMarketOrientatedMembershipData(IEnumerable<ReportGeneralElectionCommitteeDto> committees)
    {
        return committees
            .Select(c => new ReportCommitteeWithFreeTextDto
            {
                Name = c.GetDescription(),
                FreeText = string.Join(", ",
                    c.Memberships
                        .OrderBy(m => m.Function!.Sort)
                        .ThenBy(m => m.Surname)
                        .ThenBy(m => m.GivenName)
                        .Select(m => $"{m.Surname} {m.GivenName}, {m.Function!.GetText()}, {m.MaximumEmploymentLevel}%")
                )
            })
            .OrderBy(c => c.Name);
    }

    private static IEnumerable<ReportCommitteeWithFreeTextDto> GetLongerMembershipData(IEnumerable<ReportCommitteeWithMemberDetailDto> committees)
    {
        return committees
            .Select(c => new ReportCommitteeWithFreeTextDto
            {
                Name = c.Name,
                FreeText = string.Join(", ",
                    c.Members!
                        .OrderBy(m => m.Surname)
                        .Select(m => $"{m.Surname} {m.GivenName}")
                )
            })
            .OrderBy(c => c.Name);
    }

    private static IEnumerable<ReportCommitteeWithMemberDetailDto> GetCommitteesAndMembers(IEnumerable<ReportGeneralElectionCommitteeDto> committees)
    {
        return committees
            .Select(c => new ReportCommitteeWithMemberDetailDto
            {
                Name = c.GetDescription(),
                MemberCount = c.ActiveMemberCount
            })
            .OrderBy(c => c.Name);
    }

    private IEnumerable<ReportMembershipDto> GetMembershipsByType(ReportGeneralElectionCommitteeDto committee, ReportMembershipType type, TermOfOfficeDate? termOfOffice = null, LegislaturePeriod? legislaturePeriod = null)
    {
        var legislaturePeriodId = Guid.Empty;
        if (legislaturePeriod != null)
        {
            legislaturePeriodId = legislaturePeriod.Id;
        }

        return type switch
        {
            ReportMembershipType.ShorterDuty => committee.Memberships
                .Where(m => m.EndDate < termOfOffice!.EndDate)
                .OrderBy(m => m.Person!.Surname)
                .ThenBy(m => m.Person!.GivenName)
                .Select(membership => new ReportMembershipDto
                {
                    Surname = membership.Person!.Surname,
                    GivenName = membership.Person!.GivenName,
                    Type = ReportMembershipType.ShorterDuty,
                    Justification = ReportExportHtmlSanitizer.Sanitize(membership.JustificationShorterDuty).CleanText,
                    HasOpenJustificationChanges = ReportExportHtmlSanitizer.Sanitize(membership.JustificationShorterDuty).HasOpenChanges,
                    JustificationUrl = BuildMembershipJustificationUrl(committee.Id, membership.Id),
                    FreeText = string.Format(CultureInfo.InvariantCulture, BusinessTexts.Report_ShorterDutyText, membership.BeginDate.ToShortDateString(), membership.EndDate.ToShortDateString())
                }),

            ReportMembershipType.FederalAssemblyCurrent => committee.Memberships
                // only members NOT belonging in the legislature periode of next GeneralElection
                .Where(m => m.Person != null && m.Person.FederalAssembly && !m.Person.LegislaturePeriods.Any(lp => lp.Id == legislaturePeriodId))
                .OrderBy(m => m.Person!.Surname)
                .ThenBy(m => m.Person!.GivenName)
                .Select(membership => new ReportMembershipDto
                {
                    Surname = membership.Person!.Surname,
                    GivenName = membership.Person!.GivenName,
                    Type = ReportMembershipType.FederalAssemblyCurrent,
                    Function = membership.Person.CouncilId == Council.CouncilOfStateId && membership.Person.GenderId == Gender.FemaleGuid ? BusinessTexts.CouncilOfStateFemale : membership.Person.CouncilId == Council.CouncilOfStateId && membership.Person.GenderId == Gender.MaleGuid ? BusinessTexts.CouncilOfStateMale : membership.Person.CouncilId == Council.NationalCouncilId && membership.Person.GenderId == Gender.FemaleGuid ? BusinessTexts.NationalCouncilFemale : membership.Person.CouncilId == Council.NationalCouncilId && membership.Person.GenderId == Gender.MaleGuid ? BusinessTexts.NationalCouncilMale : string.Empty,
                    FreeText = membership.Person.Council?.GetText(),
                    Justification = ReportExportHtmlSanitizer.Sanitize(membership.JustificationMemberInFederalAssembly).CleanText,
                    HasOpenJustificationChanges = ReportExportHtmlSanitizer.Sanitize(membership.JustificationMemberInFederalAssembly).HasOpenChanges,
                    JustificationUrl = BuildMembershipJustificationUrl(committee.Id, membership.Id),
                }),

            ReportMembershipType.FederalAssemblyFuture => committee.Memberships
               // only members belonging in the legislature periode of next GeneralElection
               .Where(m => m.Person != null && m.Person.FederalAssembly && m.Person.LegislaturePeriods.Any(lp => lp.Id == legislaturePeriodId))
                .OrderBy(m => m.Person!.Surname)
                .ThenBy(m => m.Person!.GivenName)
                .Select(membership => new ReportMembershipDto
                {
                    Surname = membership.Person!.Surname,
                    GivenName = membership.Person!.GivenName,
                    Type = ReportMembershipType.FederalAssemblyFuture,
                    Function = membership.Person.CouncilId == Council.CouncilOfStateId && membership.Person.GenderId == Gender.FemaleGuid ? BusinessTexts.CouncilOfStateFemale : membership.Person.CouncilId == Council.CouncilOfStateId && membership.Person.GenderId == Gender.MaleGuid ? BusinessTexts.CouncilOfStateMale : membership.Person.CouncilId == Council.NationalCouncilId && membership.Person.GenderId == Gender.FemaleGuid ? BusinessTexts.NationalCouncilFemale : membership.Person.CouncilId == Council.NationalCouncilId && membership.Person.GenderId == Gender.MaleGuid ? BusinessTexts.NationalCouncilMale : string.Empty,
                    FreeText = membership.Person.Council?.GetText(),
                    Justification = ReportExportHtmlSanitizer.Sanitize(membership.JustificationMemberInFederalAssembly).CleanText,
                    HasOpenJustificationChanges = ReportExportHtmlSanitizer.Sanitize(membership.JustificationMemberInFederalAssembly).HasOpenChanges,
                    JustificationUrl = BuildMembershipJustificationUrl(committee.Id, membership.Id),
                }),

            ReportMembershipType.FederalDuty => committee.Memberships
                .Where(m => m.Person != null && m.Person!.FederalDuty)
                .OrderBy(m => m.Person!.Surname)
                .ThenBy(m => m.Person!.GivenName)
                .Select(membership => new ReportMembershipDto
                {
                    Surname = membership.Person!.Surname,
                    GivenName = membership.Person!.GivenName,
                    Type = ReportMembershipType.FederalDuty,
                    Function = membership.Person!.Gender!.Uri == Gender.Female ? membership.Function!.GetText() + ", " + string.Join("/", membership.Person!.Occupations.Select(o => o.GetFemaleText(CultureInfo.CurrentUICulture)).Order()) :
                        membership.Function!.GetText() + ", " + string.Join("/", membership.Person!.Occupations.Select(o => o.GetText(CultureInfo.CurrentUICulture)).Order()),
                    FreeText = membership.Person!.Office?.GetText(),
                    FreeText2 = membership.Person!.Office != null ? membership.Person!.Office!.IsCentralFederalAdministration ? BusinessTexts.Office_Central : BusinessTexts.Office_Decentral : string.Empty,
                    Justification = ReportExportHtmlSanitizer.Sanitize(membership.JustificationMemberInFederalDuty).CleanText,
                    HasOpenJustificationChanges = ReportExportHtmlSanitizer.Sanitize(membership.JustificationMemberInFederalDuty).HasOpenChanges,
                    JustificationUrl = BuildMembershipJustificationUrl(committee.Id, membership.Id),
                }),

            ReportMembershipType.MarketOrientated => committee.Memberships
                .OrderBy(m => m.Person!.Surname)
                .ThenBy(m => m.Person!.GivenName)
                .Select(membership => new ReportMembershipDto
                {
                    Surname = membership.Person!.Surname,
                    GivenName = membership.Person!.GivenName,
                    Type = ReportMembershipType.MarketOrientated,
                    Function = membership.Function!.GetText(),
                    FreeText = $"{membership.MaximumEmploymentLevel} %"
                }),

            ReportMembershipType.CompetenceProfile => committee.Memberships
                .Where(m => m.Person != null && !string.IsNullOrWhiteSpace(m.RequirementsProfile))
                .OrderBy(m => m.Person!.Surname)
                .ThenBy(m => m.Person!.GivenName)
                .Select(membership => new ReportMembershipDto
                {
                    Surname = membership.Person!.Surname,
                    GivenName = membership.Person!.GivenName,
                    Type = ReportMembershipType.CompetenceProfile,
                    Function = membership.Function!.GetText(),
                    FreeText = ReportExportHtmlSanitizer.Sanitize(membership.RequirementsProfile).CleanText,
                    HasOpenJustificationChanges = ReportExportHtmlSanitizer.Sanitize(membership.RequirementsProfile).HasOpenChanges,
                    JustificationUrl = BuildMembershipJustificationUrl(committee.Id, membership.Id),
                    FreeText2 = membership.Person!.Gender!.Uri == Gender.Female ? string.Join("/", membership.Person!.Occupations.Select(o => o.GetFemaleText(CultureInfo.CurrentUICulture)).Order()) : string.Join("/", membership.Person!.Occupations.Select(o => o.GetText(CultureInfo.CurrentUICulture)).Order()),
                    FreeText3 = membership.Person!.Employer
                }),

            _ => []
        };
    }

    private static IEnumerable<MultipleMembershipsDto> GetPersonsWithMultipleMemberships(IEnumerable<ReportGeneralElectionCommitteeDto> committees)
    {
        var dtoList = committees
            .SelectMany(c => c.Memberships.Select(m => new { m.Person, CommitteeName = c.GetDescription() }))
            .GroupBy(x => x.Person)
            .Where(g => g.Count() >= 3)
            .Select(g => new MultipleMembershipsDto
            {
                Surname = g.Key!.Surname,
                GivenName = g.Key.GivenName,
                NumberOfMemberships = g.Count(),
                Committees = string.Join(Environment.NewLine, g
                    .Select(x => x.CommitteeName)
                    .Distinct()
                    .OrderBy(name => name))
            });

        return dtoList;
    }

    private IEnumerable<ReportDepartmentWithCommitteesAndMembersDto> SummarizeMembershipsFromPresentAndFutureByDepartment(IEnumerable<GeneralElectionCommittee> committees,
        IEnumerable<Department> departments)
    {
        var filteredCommittees = committees
        .Select(c => new GeneralElectionCommittee
        {
            Id = c.Id,
            Modified = c.Modified,
            ModifiedBy = c.ModifiedBy,
            Created = c.Created,
            CreatedBy = c.CreatedBy,
            BeginDate = c.BeginDate,
            EndDate = c.EndDate,
            TermOfOfficeDateId = c.TermOfOfficeDateId,
            CommitteeId = c.CommitteeId,
            DepartmentId = c.DepartmentId,
            Department = c.Department,
            OfficeId = c.OfficeId,
            Office = c.Office,
            CommitteeLevelId = c.CommitteeLevelId,
            CommitteeLevel = c.CommitteeLevel,
            CommitteeTypeId = c.CommitteeTypeId,
            CommitteeType = c.CommitteeType,
            IsDeleted = c.IsDeleted,
            DescriptionGerman = c.DescriptionGerman,
            DescriptionFrench = c.DescriptionFrench,
            DescriptionItalian = c.DescriptionItalian,
            DescriptionRomansh = c.DescriptionRomansh,
            JustificationMembers = c.JustificationMembers,
            JustificationGenders = c.JustificationGenders,
            JustificationLanguages = c.JustificationLanguages,
            MarketOrientated = c.MarketOrientated,
            MeasuresGenders = c.MeasuresGenders,
            MeasuresLanguages = c.MeasuresLanguages,
            RemarksBaseData = c.RemarksBaseData,
            RemarksBaseDataAdmin = c.RemarksBaseDataAdmin,
            IsValidated = c.IsValidated,
            WasValidatedOnce = c.WasValidatedOnce,
            IsFederalCouncilProposalDirty = c.IsFederalCouncilProposalDirty,
            VacanciesGeneralElection = c.VacanciesGeneralElection,
            SelectionProcedure = c.SelectionProcedure,
            CandidateListStateId = c.CandidateListStateId,
            AssignedToRole = c.AssignedToRole,
            SelfOrganized = c.SelfOrganized,
            MembershipCandidates = c.MembershipCandidates
                .Where(m => m.EstimatedTermOfOffice >= 13)
                .ToList()
        })
        .Where(c => c.ExtraParliamentaryCommission)
        .Where(c => c.MembershipCandidates.Count > 0)
        .ToList();

        var generalElectionCommitteesWithMembers = filteredCommittees.Select(ReportMapper.FromGeneralElectionCommitteeToReportGeneralElectionCommitteeDto).ToArray();

        var groupedByDepartment = generalElectionCommitteesWithMembers
            .GroupBy(c => c.Department)
            .ToArray();

        var result = departments.Select(dept =>
        {
            // Get all committees in this department (even if empty)
            var committeesInDepartment = groupedByDepartment
                .Where(g => g.Key!.Id == dept.Id)
                .SelectMany(g => g)
                .ToArray();

            // Map committees with filtered members, only when there are members
            var committeesDto = committeesInDepartment
                .Where(cwm => cwm.Memberships.Count > 0)
                .Select(cwm => new ReportCommitteeWithMemberDetailDto
                {
                    Name = cwm.GetDescription(),
                    MemberCount = cwm.Memberships.Count,
                    Members = cwm.Memberships
                    .OrderBy(m => m.Person!.Surname)
                    .ThenBy(m => m.Person!.GivenName)
                    .Select(m => new ReportMembershipDto
                    {
                        Surname = m.Person!.Surname,
                        GivenName = m.Person!.GivenName,
                        FreeText = m.EstimatedTermOfOffice == 16 ? $"{m.EstimatedTermOfOffice} {BusinessTexts.Report_Years} ({BusinessTexts.Membership_Until} {m.EndDate})" : $"{m.EstimatedTermOfOffice} {BusinessTexts.Report_Years}",
                        Justification = ReportExportHtmlSanitizer.Sanitize(m.JustificationLongerDuty).CleanText,
                        HasOpenJustificationChanges = ReportExportHtmlSanitizer.Sanitize(m.JustificationLongerDuty).HasOpenChanges,
                        JustificationUrl = BuildMembershipJustificationUrl(cwm.Id, m.Id),
                    })
                .ToArray()
                })
            .ToArray();

            return new ReportDepartmentWithCommitteesAndMembersDto
            {
                Name = dept.GetText(),
                Committees = committeesDto
            };
        });

        return result;
    }

    private static IEnumerable<ReportCommitteeWithMemberDetailDto> SummarizeMembershipsFromPresentAndFuture(IEnumerable<ReportGeneralElectionCommitteeDto> committees,
        IEnumerable<ReportGeneralElectionCommitteeDto> geCommitteesWithMembers)
    {
        // which members we have in the future?
        var reportGeneralElectionCommitteeDtos = geCommitteesWithMembers.ToArray();
        var futureLookup =
            reportGeneralElectionCommitteeDtos
                .SelectMany(fc => fc.Memberships.Select(m => new
                {
                    fc.CommitteeId,
                    m.PersonId
                }))
                .ToHashSet();

        // select only the members in the past, which are also in the future...
        var filteredPresentCommittees =
            committees
                .Select(c => new ReportGeneralElectionCommitteeDto
                {
                    CommitteeId = c.CommitteeId,
                    Memberships = c.Memberships
                        .Where(m => futureLookup.Contains(
                            new { c.CommitteeId, m.PersonId }))
                        .ToArray(),
                    TermOfOfficeDateId = c.TermOfOfficeDateId,
                    TermOfOfficeDate = c.TermOfOfficeDate,
                    IsValidated = c.IsValidated,
                    IsDeleted = c.IsDeleted,
                    DescriptionGerman = c.DescriptionGerman,
                    DescriptionFrench = c.DescriptionFrench,
                    DescriptionItalian = c.DescriptionItalian,
                    DescriptionRomansh = c.DescriptionRomansh,
                    DepartmentId = c.DepartmentId,
                    Department = c.Department,
                    OfficeId = c.OfficeId,
                    Office = c.Office
                })
                // If a committee has no relevant members → remove it
                .Where(c => c.Memberships.Count != 0)
                .ToArray();

        // Combine past/present with the future..
        var allCommittees = filteredPresentCommittees.Concat(reportGeneralElectionCommitteeDtos);

        var committeesWithQualifiedMembers = allCommittees.Select(c =>
        {
            var membersWithDuration = c.Memberships
                .GroupBy(m => m.PersonId)
                .Select(g =>
                {
                    var person = g.First().Person!; // get the Person object from first membership in the group

                    var totalDurationYears = g.Sum(m => MembershipTermCalculator.CalculateEstimatedTermInYears(m.BeginDate, m.EndDate));

                    return new
                    {
                        Person = person,
                        TotalDurationYears = totalDurationYears
                    };
                })
                .Where(m => m.TotalDurationYears >= 12)
                .ToArray();

            return new
            {
                Committee = c,
                QualifiedMembers = membersWithDuration
            };
        });

        // Map committees with filtered members
        var committeesDto = committeesWithQualifiedMembers.Select(cwm => new ReportCommitteeWithMemberDetailDto
        {
            Name = cwm.Committee.GetDescription(),
            MemberCount = cwm.QualifiedMembers.Length,
            Members = cwm.QualifiedMembers.Select(m => new ReportMembershipDto
            {
                Surname = m.Person.Surname,
                GivenName = m.Person.GivenName,
                FreeText = $"{m.TotalDurationYears} {BusinessTexts.Report_Years}"
            }).ToArray()
        })
            .Where(cwm => cwm.MemberCount > 0);

        return committeesDto;
    }

    private string BuildCommitteeJustificationUrl(Guid committeeId)
    {
        return $"{_configuration["FrontendUrl"]}/general-election/committees/{committeeId}?tab=justifications";
    }

    private string BuildMembershipJustificationUrl(Guid committeeId, Guid membershipId)
    {
        return $"{_configuration["FrontendUrl"]}/general-election/committees/{committeeId}/membership-candidate/{membershipId}";
    }

    private string BuildGeneralMeasuresJustificationUrl()
    {
        return $"{_configuration["FrontendUrl"]}/administration/generalMeasures";
    }
}
