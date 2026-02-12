using System.Globalization;
using System.Text.RegularExpressions;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Common.Resources;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class ReportService : IReportService
{
    private readonly Bk.DocumentService.Client.IDocumentService _documentService;
    private readonly ITermOfOfficeDateService _termOfOfficeDateService;
    private readonly ICultureService _cultureService;
    private readonly IElectoralListService _electoralListService;
    private readonly IEiamAssignmentService _eiamAssignmentService;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly IGeneralMeasureRepository _generalMeasureRepository;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        Bk.DocumentService.Client.IDocumentService documentService,
        ICultureService cultureService,
        ITermOfOfficeDateService termOfOfficeDateService,
        IElectoralListService electoralListService,
        IEiamAssignmentService eiamAssignmentService,
        ICommitteeRepository committeeRepository,
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        IMasterDataRepository masterDataRepository,
        IGeneralMeasureRepository generalMeasureRepository,
        ILogger<ReportService> logger)
    {
        _documentService = documentService;
        _termOfOfficeDateService = termOfOfficeDateService;
        _cultureService = cultureService;
        _electoralListService = electoralListService;
        _eiamAssignmentService = eiamAssignmentService;
        _committeeRepository = committeeRepository;
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _masterDataRepository = masterDataRepository;
        _generalMeasureRepository = generalMeasureRepository;
        _logger = logger;
    }

    public async Task<(string fileName, Stream content)> GetReport(ReportFilterParametersDto filterDto)
    {
        _logger.LogInformation("Generate report of type {ReportType}", filterDto.DocumentType);

        return filterDto.DocumentType switch
        {
            ReportType.ParliamentaryReport => await GenerateParliamentaryReport(filterDto),
            ReportType.AppendixFederalCouncil => await GenerateAppendixFederalCouncil(filterDto),
            ReportType.ElectoralListOnline => await _electoralListService.GenerateDocument(filterDto, "ElectoralList_Internet"),
            ReportType.ElectoralListFC => await _electoralListService.GenerateDocument(filterDto, "ElectoralList_FederalCouncil"),
            ReportType.DecisionFederalCouncil => await GenerateDecisionFederalCouncilReport(filterDto),
            _ => await GenerateParliamentaryReport(filterDto)
        };
    }

    private async Task<(string fileName, Stream content)> GenerateDecisionFederalCouncilReport(ReportFilterParametersDto filterDto)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var departments = await _masterDataRepository.GetDepartments();
        departments = departments.Where(d => d.Uri != Department.BkUri).ToArray();

        var template = _cultureService.GetCurrentUiCulture().TwoLetterISOLanguageName == "fr" ? "APG_Decision_Federal_Council_French" : "APG_Decision_Federal_Council_German";

        var nextTermOfOfficeDate = await _termOfOfficeDateService.GetNextTermOfOfficeDate();

        var committees = (await _committeeRepository.GetAllForGeneralElection(departmentId, officeId, committeeId)).ToArray();
        var committeesWithMembers = await _generalElectionCommitteeRepository.GetByFilterForReport(filterDto, departmentId, officeId, committeeId);

        var generalElectionCommitteesWithMembers = committeesWithMembers.Select(ReportMapper.FromGeneralElectionCommitteeToReportGeneralElectionCommitteeDto).ToList();

        var currentExtraParliamentaryCommissions = committees.Where(c => c.ExtraParliamentaryCommission).ToList();
        var currentReportExtraParliamentaryCommissions = currentExtraParliamentaryCommissions.Select(ReportMapper.FromCommitteeToReportGeneralElectionCommitteeDto).ToList();

        var extraParliamentaryCommissions = generalElectionCommitteesWithMembers.Where(c => c.ExtraParliamentaryCommission).ToList();

        var nonReleasedCommissions = generalElectionCommitteesWithMembers.Where(c => c.ReleaseGeneralElection == false).ToList();

        var marketOrientatedCommissions = extraParliamentaryCommissions.Where(c => c.MarketOrientated == true).ToList();

        var membersWith12OrMoreYears = SummarizeMembershipsFromPresentAndFuture(currentReportExtraParliamentaryCommissions, extraParliamentaryCommissions);
        var membersWith12OrMoreYearsDto = GetLongerMembershipData(membersWith12OrMoreYears);

        var committeesWithMembersInFederalDuty = generalElectionCommitteesWithMembers.Where(c => c.Memberships.Any(m => m.Person!.FederalDuty)).ToList();

        var moreThan15MembersCommittees = generalElectionCommitteesWithMembers.Where(c => c.ExtraParliamentaryCommission && c.Memberships.Count > 15).ToList();

        var nonReleasedCommissionsDto = GetNonReleasedCommissions(nonReleasedCommissions);
        var marketOrientatedCommissionsDto = GetMarketOrientatedMembershipData(marketOrientatedCommissions);
        var moreThan15MembersCommitteesDto = GetCommitteesAndMembers(moreThan15MembersCommittees, ReportMembershipType.MoreThan15Members);
        var missingGenderMembersCommitteesDto = GetCommitteesWithGenders(extraParliamentaryCommissions);
        var missingItalianAndFrenchMembersCommitteesDto = GetCommitteesWithLanguages(extraParliamentaryCommissions);
        var committeesWithMembersInFederalDutyDto = GetFederalDutyMembershipsWithOffice(committeesWithMembersInFederalDuty);

        var decisionFederalCouncilReportDto = new DecisionFederalCouncilReportDto
        {
            TermOfOfficeDateRange = nextTermOfOfficeDate.BeginDate.Year + " - " + nextTermOfOfficeDate.EndDate?.Year,

            NonReleasedCommissions = nonReleasedCommissionsDto,
            MarketOrientatedCommissions = marketOrientatedCommissionsDto,
            MoreThan15MembersCommittees = moreThan15MembersCommitteesDto,
            MissingGenderMembersCommittees = missingGenderMembersCommitteesDto,
            MissingLanguageMembersCommittees = missingItalianAndFrenchMembersCommitteesDto,
            LongerDutyMembersCommittees = membersWith12OrMoreYearsDto,
            FederalDutyMembersCommittees = committeesWithMembersInFederalDutyDto
        };

        var documentStream = await _documentService.CreateWordFromTemplate($"Templates/{template}.docx", decisionFederalCouncilReportDto, "decisionFederalCouncil");

        return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd}_{BusinessTexts.DecisionFederalCouncil_Filename}.docx", documentStream);
    }

    private async Task<(string fileName, Stream content)> GenerateParliamentaryReport(ReportFilterParametersDto filterDto)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var template = _cultureService.GetCurrentUiCulture().TwoLetterISOLanguageName == "fr" ? "Report_Parliament_French" : "Report_Parliament_German";

        var departments = await _masterDataRepository.GetDepartments();
        departments = departments.Where(d => d.Uri != Department.BkUri).ToArray();

        var nextTermOfOfficeDate = await _termOfOfficeDateService.GetNextTermOfOfficeDate();
        var otherCommitteeTypes = _committeeRepository.GetAll()
            .Where(c => c.TermOfOfficeId != TermOfOffice.Period4YearsInGeneralElectionGuid && c.BeginDate <= DateOnly.FromDateTime(DateTime.Today) && (c.EndDate is null || c.EndDate >= DateOnly.FromDateTime(DateTime.Today)))
            .ToList();
        var otherReportCommitteeTypes = otherCommitteeTypes.Select(ReportMapper.FromCommitteeToReportGeneralElectionCommitteeDto).ToList();

        var committees = (await _committeeRepository.GetAllForGeneralElection(departmentId, officeId, committeeId)).ToArray();
        var committeesWithMembers = await _generalElectionCommitteeRepository.GetByFilterForReport(filterDto, departmentId, officeId, committeeId);

        var generalElectionCommitteesWithMembers = committeesWithMembers.Select(ReportMapper.FromGeneralElectionCommitteeToReportGeneralElectionCommitteeDto).ToList();

        var vacanciesCommittees = generalElectionCommitteesWithMembers.Where(c => c.VacanciesGeneralElection > 0).ToArray();

        var currentExtraParliamentaryCommissions = committees.Where(c => c.ExtraParliamentaryCommission).ToList();
        var currentReportExtraParliamentaryCommissions = currentExtraParliamentaryCommissions.Select(ReportMapper.FromCommitteeToReportGeneralElectionCommitteeDto).ToList();

        var extraParliamentaryCommissions = generalElectionCommitteesWithMembers.Where(c => c.ExtraParliamentaryCommission).ToList();

        var disbandedCommittees = _committeeRepository.GetAll().Where(c => c.ExtraParliamentaryCommission &&
                                                                           ((c.BeginDate > nextTermOfOfficeDate.BeginDate && c.BeginDate < nextTermOfOfficeDate.EndDate) || (c.EndDate < nextTermOfOfficeDate.EndDate && c.EndDate > nextTermOfOfficeDate.BeginDate))).ToList();
        var disbandedReportCommittees = disbandedCommittees.Select(ReportMapper.FromCommitteeToReportGeneralElectionCommitteeDto).ToList();

        var membersWith12OrMoreYears = SummarizeMembershipsFromPresentAndFutureByDepartment(currentReportExtraParliamentaryCommissions, extraParliamentaryCommissions, departments);

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

        var femaleUnderStuffed = extraParliamentaryCommissions.Count(c => c.FemaleUnderStuffed);
        var maleUnderStuffed = extraParliamentaryCommissions.Count(c => c.MaleUnderStuffed);

        // get all committees for GE, which are released and did not end before the current termOfOfficeDate
        var releasedCommittees = committees.Where(c => c.ReleaseGeneralElection == true && (c.EndDate is null || c.EndDate > nextTermOfOfficeDate.BeginDate)).ToList();
        // same as above, but not released
        var unreleasedCommittees = committees.Where(c => c.ReleaseGeneralElection == false && (c.EndDate is null || c.EndDate > nextTermOfOfficeDate.BeginDate)).ToList();
        // number of APKs (!) which are new or have ended before the current termOfOfficeDate
        var moreThan15MembersCommittees = generalElectionCommitteesWithMembers.Where(c => c.ExtraParliamentaryCommission && c.Memberships.Count > 15).ToList();
        var releasedCommitteesDto = GetCommitteesByDepartmentAndTypes(releasedCommittees, departments);
        var unreleasedCommitteesDto = GetCommitteesByDepartmentAndTypes(unreleasedCommittees, departments);
        var disbandedCommitteesDto = GetCommitteesByDepartment(disbandedReportCommittees, departments, ReportCommitteeType.StandardBehaviour, nextTermOfOfficeDate);
        var financialImpactsCommitteesDto = GetCommitteesByDepartment(generalElectionCommitteesWithMembers, departments, ReportCommitteeType.StandardBehaviour);
        var vacanciesCommitteesDto = GetCommitteesByDepartment(vacanciesCommittees, departments, ReportCommitteeType.Vacancies);

        var moreThan15MembersCommitteesDto = GetCommitteesByDepartment(moreThan15MembersCommittees, departments, ReportCommitteeType.StandardBehaviour);
        var missingGenderMembersCommitteesDto = await GetCommitteesWithGendersByDepartment(extraParliamentaryCommissions, departments);
        var missingItalianAndFrenchMembersCommitteesDto = await GetCommitteesWithLanguagesByDepartment(extraParliamentaryCommissions, departments);

        var committeesWithMembersInFederalAssembly = GetCommitteesAndMembersByDepartment(extraParliamentaryCommissions.Where(c => c.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid), departments, ReportMembershipType.FederalAssembly);
        var committeesWithMembersInFederalDuty = GetCommitteesAndMembersByDepartment(extraParliamentaryCommissions, departments, ReportMembershipType.FederalDuty);
        var committeesWithDifferentTermOfOffice = GetCommitteesByDepartment(otherReportCommitteeTypes, departments, ReportCommitteeType.StandardBehaviour);

        var parliamentaryReportDto = new ParliamentaryReportDto
        {
            TermOfOfficeDateRange = nextTermOfOfficeDate.BeginDate.Year + " - " + nextTermOfOfficeDate.EndDate?.Year,
            NumberOfMembers = committees.Sum(c => c.ActiveMemberCount),
            NumberOfCommittees = committees.Length,
            NumberOfAuthoritiesCommissions = committees.Count(c => c.CommitteeTypeId == CommitteeType.AuthoritiesCommissionGuid),
            NumberOfAdministrationCommissions = committees.Count(c => c.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid),
            NumberOfManagementCommittees = committees.Count(c => c.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid),
            NumberOfFederalAgenciesCommittees = committees.Count(c => c.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid),
            NumberOfVacancies = vacanciesCommittees.Length,
            NumberOfExtraParliamentaryCommissions = extraParliamentaryCommissions.Count,
            MoreThan15Members = generalElectionCommitteesWithMembers.Count(c => c is { ExtraParliamentaryCommission: true, Memberships.Count: > 15 }),
            FemalePercentage = totalMembers > 0 ? Math.Round((decimal)femaleCount / totalMembers * 100, 2) : 0,
            MalePercentage = totalMembers > 0 ? Math.Round((decimal)maleCount / totalMembers * 100, 2) : 0,
            FemaleUnderStuffed = femaleUnderStuffed,
            MaleUnderStuffed = maleUnderStuffed,
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
            OtherCommitteeTypes = otherCommitteeTypes.Count,
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

        return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd}_{BusinessTexts.ParliamentaryReport_Filename}.docx", documentStream);
    }

    private async Task<(string fileName, Stream content)> GenerateAppendixFederalCouncil(ReportFilterParametersDto filterDto)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var template = _cultureService.GetCurrentUiCulture().TwoLetterISOLanguageName == "fr" ? "Appendix_FederalCouncil_French" : "Appendix_FederalCouncil_German";

        var departments = await _masterDataRepository.GetDepartments();
        departments = departments.Where(d => d.Uri != Department.BkUri);

        // there must be a general election running, or this stop the report!
        var geTermOfOfficeDate = await _termOfOfficeDateService.GetGeneralElectionTermOfOfficeDate();

        var committeeTypes = await _masterDataRepository.GetCommitteeTypes();

        // present data, needed for one part of the report!
        var committees = await _committeeRepository.GetAllForGeneralElectionWithActiveMembers(departmentId, officeId, committeeId);
        var extraParliamentaryCommittees = committees.Where(c => c.ExtraParliamentaryCommission).ToList();

        var reportCommittees = extraParliamentaryCommittees.Select(c => ReportMapper.FromCommitteeToReportGeneralElectionCommitteeDto(c)).ToList();

        var generalElectionCommittees = await _generalElectionCommitteeRepository.GetByFilterForReport(filterDto, departmentId, officeId, committeeId);

        // to be able to use the same functions, we map here the GeneralElection data to normal data!
        var geCommitteesWithMembers = generalElectionCommittees.Select(c => ReportMapper.FromGeneralElectionCommitteeToReportGeneralElectionCommitteeDto(c)).ToList();
        var extraParliamentaryCommissions = geCommitteesWithMembers.Where(c => c.ExtraParliamentaryCommission).ToList();

        var disbandedCommittees = _committeeRepository.GetAll().Where(c => c.ExtraParliamentaryCommission &&
                                                                           ((c.BeginDate > geTermOfOfficeDate.BeginDate && c.BeginDate < geTermOfOfficeDate.EndDate) || (c.EndDate < geTermOfOfficeDate.EndDate && c.EndDate > geTermOfOfficeDate.BeginDate))).ToList();
        var disbandedReportCommittees = disbandedCommittees.Select(c => ReportMapper.FromCommitteeToReportGeneralElectionCommitteeDto(c)).ToList();

        var extraParliamentaryCommissionsWithFederalDutyMembers = extraParliamentaryCommissions
            .Count(c => c.Memberships.Any(m => m.Person!.FederalDuty));
        var extraParliamentaryCommitteesFederalDutyMembers = extraParliamentaryCommissions
            .SelectMany(c => c.Memberships)
            .Count(m => m.Person != null && m.Person.FederalDuty);

        var marketOrientatedCommissions = geCommitteesWithMembers.Where(c => c.MarketOrientated == true).ToList();

        // get all committees for GE, which are released and did not end before the current termOfOfficeDate
        var releasedCommittees = geCommitteesWithMembers.Where(c => c.ReleaseGeneralElection == true && (c.EndDate is null || c.EndDate > geTermOfOfficeDate.BeginDate)).ToList();
        // same as above, but not released
        var unreleasedCommittees = geCommitteesWithMembers.Where(c => c.ReleaseGeneralElection == false && (c.EndDate is null || c.EndDate > geTermOfOfficeDate.BeginDate)).ToList();
        // number of APKs (!) which are new or have ended before the current termOfOfficeDate
        var moreThan15MembersCommittees = geCommitteesWithMembers.Where(c => c.ExtraParliamentaryCommission && c.Memberships.Count > 15).ToList();
        var releasedCommitteesDto = GetCommitteesByDepartment(releasedCommittees, departments, ReportCommitteeType.StandardBehaviour);
        var unreleasedCommitteesDto = GetCommitteesByDepartment(unreleasedCommittees, departments, ReportCommitteeType.StandardBehaviour);
        var disbandedCommitteesDto = GetCommitteesByDepartment(disbandedReportCommittees, departments, ReportCommitteeType.StandardBehaviour);
        var vacanciesCommittees = geCommitteesWithMembers.Where(c => c.VacanciesGeneralElection != null);
        var selectionProcedureCommittees = geCommitteesWithMembers.Where(c => c.SelectionProcedure != null);
        var requirementsProfileCommittees = geCommitteesWithMembers.Where(c => c.Memberships.Any(m => m.RequirementsProfile != null)).ToList();

        var moreThan15MembersCommitteesDto = GetCommitteesByDepartment(moreThan15MembersCommittees, departments, ReportCommitteeType.StandardBehaviour);
        var missingGenderMembersCommitteesDto = await GetCommitteesWithGendersByDepartment(extraParliamentaryCommissions, departments);
        var missingItalianAndFrenchMembersCommitteesDto = await GetCommitteesWithLanguagesByDepartment(extraParliamentaryCommissions, departments);

        var missingGenderMembersCommitteesCount = missingGenderMembersCommitteesDto.SelectMany(c => c.Committees!).Count();
        var missingItalianAndFrenchMembersCommitteesCount = missingItalianAndFrenchMembersCommitteesDto.SelectMany(c => c.Committees!).Count();

        var committeesWithMembersWithLongerDutyDto = SummarizeMembershipsFromPresentAndFutureByDepartment(reportCommittees, extraParliamentaryCommissions, departments);

        var committeesWithMembersWithShorterDutyDto = GetCommitteesAndMembersByDepartment(extraParliamentaryCommissions, departments, ReportMembershipType.ShorterDuty, geTermOfOfficeDate);

        var marketOrientatedCommitteesDto = GetCommitteesAndMembersByDepartment(marketOrientatedCommissions, departments, ReportMembershipType.MarketOrientated);

        var committeesWithMembersInFederalAssemblyDto = GetCommitteesAndMembersByDepartment(extraParliamentaryCommissions
            .Where(c => c.CommitteeTypeId == CommitteeType.AdministrationCommissionGuid), departments, ReportMembershipType.FederalAssembly);
        var committeesWithMembersInFederalDutyDto = GetCommitteesAndMembersByDepartment(extraParliamentaryCommissions, departments, ReportMembershipType.FederalDuty);

        var managementCommittees = geCommitteesWithMembers.Where(c => c.CommitteeTypeId == CommitteeType.ManagementCommitteeGuid);
        var federalAgencies = geCommitteesWithMembers.Where(c => c.CommitteeTypeId == CommitteeType.FederalAgenciesCommitteeGuid);

        var managementCommitteesWithMissingGendersDto = await GetCommitteesWithGendersByDepartment(managementCommittees, departments);
        var managementCommitteesWithMissingLanguagesDto = await GetCommitteesWithLanguagePercentagesByDepartment(managementCommittees, departments);
        var federalAgenciesWithMissingGendersDto = await GetCommitteesWithGendersByDepartment(federalAgencies, departments);
        var federalAgenciesWithMissingLanguagesDto = await GetCommitteesWithLanguagePercentagesByDepartment(federalAgencies, departments);
        var committeesWithVacanciesDto = GetCommitteesByDepartment(vacanciesCommittees, departments, ReportCommitteeType.Vacancies);

        var multipleMembershipsDto = GetPersonsWithMultipleMemberships(geCommitteesWithMembers);

        var committeesWithFormerFederalAssemblyMembersDto = GetCommitteesAndMembersByDepartment(extraParliamentaryCommissions, departments, ReportMembershipType.FederalAssembly);
        var committeesWithSelectionProcedureDto = GetCommitteesByDepartment(selectionProcedureCommittees, departments, ReportCommitteeType.SelectionProcedure);
        var committeesWithMembersRequirementsProfileDto = GetCommitteesAndMembersByDepartment(requirementsProfileCommittees, departments, ReportMembershipType.CompetenceProfile);

        var appendixReportDto = new AppendixFederalCouncilDto
        {
            TermOfOfficeDateRange = geTermOfOfficeDate.BeginDate.Year.ToString() + " - " + geTermOfOfficeDate.EndDate?.Year.ToString(),
            NumberOfMembers = geCommitteesWithMembers.Sum(c => c.ActiveMemberCount),
            NumberOfCommittees = geCommitteesWithMembers.Count,
            NumberOfExtraParliamentaryCommissions = extraParliamentaryCommissions.Count,
            MoreThan15Members = geCommitteesWithMembers.Count(c => c.ExtraParliamentaryCommission && c.Memberships.Count > 15),
            MissingGender = missingGenderMembersCommitteesCount,
            MissingLanguage = missingItalianAndFrenchMembersCommitteesCount,
            NumberOfMultipleMembershipsPersons = multipleMembershipsDto.Count(),
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

        return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd}_{BusinessTexts.AppendixFederalCouncil_Filename}.docx", documentStream);
    }

    private static List<ReportDepartmentWithCommitteeTypeDto> GetCommitteesByDepartmentAndTypes(IEnumerable<Committee> committees, IEnumerable<Department> departments)
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
                .ToList();

            var groupedCommittees = filteredCommittees
                .GroupBy(c => new { CommitteeTypeId = c.CommitteeType!.Id, CommitteeTypeName = c.CommitteeType.GetText() })
                .ToList();

            var committeeTypeList = new List<ReportCommitteeTypeDto>();

            foreach (var committeeGroup in groupedCommittees)
            {
                var committeeList = new List<ReportCommitteeDto>();

                foreach (var committee in committeeGroup)
                {
                    var committeeDto = new ReportCommitteeDto
                    {
                        Name = committee.GetDescription()
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

    private static List<ReportDepartmentWithCommitteesDto> GetCommitteesByDepartment(IEnumerable<ReportGeneralElectionCommitteeDto> committees, IEnumerable<Department> departments, ReportCommitteeType type, TermOfOfficeDate? termOfOfficeDate = null)
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
                .ToList();

            var committeeList = new List<ReportCommitteeDto>();

            foreach (var committee in filteredCommittees)
            {
                var freeText = "";

                if (type == ReportCommitteeType.SelectionProcedure)
                {
                    var cleanSelectionProcedure = committee.SelectionProcedure != null ? Regex.Replace(committee.SelectionProcedure.ToString(), "<.*?>", string.Empty) : string.Empty;
                    freeText = string.Format(BusinessTexts.Report_SelectionProcedure, cleanSelectionProcedure);
                }
                else if (type == ReportCommitteeType.Vacancies)
                {
                    freeText = string.Format(BusinessTexts.Report_Vacancies, committee.VacanciesGeneralElection.ToString());
                }
                else
                {
                    freeText = termOfOfficeDate != null && committee.EndDate < termOfOfficeDate.EndDate && committee.EndDate > termOfOfficeDate.BeginDate ? string.Format(BusinessTexts.Report_EndDateInPast, committee.EndDate.Value.ToShortDateString()) : termOfOfficeDate != null && committee.BeginDate > termOfOfficeDate.BeginDate && committee.BeginDate < termOfOfficeDate.EndDate ? string.Format(BusinessTexts.Report_BeginDateInFuture, committee.BeginDate.ToShortDateString()) : string.Empty;
                }

                var committeeDto = new ReportCommitteeDto
                {
                    Name = committee.GetDescription(),
                    MemberCount = committee.Memberships.Count,
                    Justification = committee.JustificationMembers,
                    FreeText = freeText
                };

                committeeList.Add(committeeDto);
            }

            dtoDict[department.GetText()].Committees = committeeList;
        }
        return departmentList;
    }

    private async Task<List<ReportDepartmentWithCommitteesAndGendersDto>> GetCommitteesWithGendersByDepartment(IEnumerable<ReportGeneralElectionCommitteeDto> committees, IEnumerable<Department> departments)
    {
        var genderMeasures = (await _generalMeasureRepository.GetGeneralGenderMeasures()).ToArray();

        var departmentList = new List<ReportDepartmentWithCommitteesAndGendersDto>();
        var dtoDict = new Dictionary<string, ReportDepartmentWithCommitteesAndGendersDto>();

        foreach (var department in departments)
        {
            var dto = new ReportDepartmentWithCommitteesAndGendersDto
            {
                Name = department.GetText(),
                Measure = genderMeasures.FirstOrDefault(g => g.DepartmentId == department.Id)?.Description
            };

            dtoDict[department.GetText()] = dto;
            departmentList.Add(dto);

            var filteredCommittees = committees
                .Where(c => c.DepartmentId == department.Id)
                .ToList();

            var committeeList = new List<ReportCommitteeGenderMissingDto>();

            foreach (var committee in filteredCommittees)
            {
                if (committee.ActiveMemberCount > 0)
                {
                    var femalePercentage = Math.Round((decimal)committee.FemaleCount / committee.ActiveMemberCount * 100, 2);
                    var malePercentage = Math.Round((decimal)committee.MaleCount / committee.ActiveMemberCount * 100, 2);

                    if (femalePercentage < (decimal)committee.CommitteeType?.FemaleThreshold! || malePercentage < (decimal)committee.CommitteeType?.MaleThreshold!)
                    {
                        var committeeDto = new ReportCommitteeGenderMissingDto
                        {
                            Name = committee.GetDescription(),
                            MemberCount = committee.Memberships.Count,
                            Measure = committee.MeasuresGenders,
                            Justification = committee.JustificationGenders,
                            FemaleMissingPercentage = femalePercentage,
                            MaleMissingPercentage = malePercentage,
                        };
                        committeeList.Add(committeeDto);
                    }
                }
            }
            dtoDict[department.GetText()].Committees = committeeList;
        }
        return departmentList;
    }

    private static List<ReportCommitteeGenderMissingDto> GetCommitteesWithGenders(IEnumerable<ReportGeneralElectionCommitteeDto> committees)
    {
        var committeeList = new List<ReportCommitteeGenderMissingDto>();

        foreach (var committee in committees)
        {
            if (committee.ActiveMemberCount > 0)
            {
                var femalePercentage = Math.Round((decimal)committee.FemaleCount / committee.ActiveMemberCount * 100, 2);
                var malePercentage = Math.Round((decimal)committee.MaleCount / committee.ActiveMemberCount * 100, 2);

                if (femalePercentage < (decimal)committee.CommitteeType?.FemaleThreshold! || malePercentage < (decimal)committee.CommitteeType?.MaleThreshold!)
                {
                    var committeeDto = new ReportCommitteeGenderMissingDto
                    {
                        Name = committee.GetDescription(),
                        MemberCount = committee.Memberships.Count,
                        Measure = committee.MeasuresGenders,
                        Justification = committee.JustificationGenders,
                        FemaleMissingPercentage = femalePercentage,
                        MaleMissingPercentage = malePercentage,
                    };
                    committeeList.Add(committeeDto);
                }
            }
        }

        committeeList = committeeList.OrderBy(c => c.Name).ToList();

        return committeeList;
    }

    private async Task<List<ReportDepartmentWithCommitteesAndLanguagesDto>> GetCommitteesWithLanguagesByDepartment(IEnumerable<ReportGeneralElectionCommitteeDto> committees, IEnumerable<Department> departments)
    {
        var languageMeasures = (await _generalMeasureRepository.GetGeneralLanguageMeasures()).ToArray();

        var departmentList = new List<ReportDepartmentWithCommitteesAndLanguagesDto>();
        var dtoDict = new Dictionary<string, ReportDepartmentWithCommitteesAndLanguagesDto>();

        foreach (var department in departments)
        {
            var dto = new ReportDepartmentWithCommitteesAndLanguagesDto
            {
                Name = department.GetText(),
                Measure = languageMeasures.FirstOrDefault(g => g.DepartmentId == department.Id)?.Description
            };

            dtoDict[department.GetText()] = dto;
            departmentList.Add(dto);

            var filteredCommittees = committees
                .Where(c => c.DepartmentId == department.Id)
                .ToList();

            var committeeList = new List<ReportCommitteeLanguageMissingDto>();

            foreach (var committee in filteredCommittees)
            {
                if (committee.ActiveMemberCount > 0)
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
                        var committeeDto = new ReportCommitteeLanguageMissingDto
                        {
                            Name = committee.GetDescription(),
                            MemberCount = committee.Memberships.Count,
                            Measure = committee.MeasuresLanguages,
                            Justification = committee.JustificationLanguages,
                            ItalianMissing = italianMissing,
                            FrenchMissing = frenchMissing,
                        };

                        committeeList.Add(committeeDto);
                    }
                }
            }

            dtoDict[department.GetText()].Committees = committeeList;
        }
        return departmentList;
    }

    private static List<ReportCommitteeLanguageMissingDto> GetCommitteesWithLanguages(IEnumerable<ReportGeneralElectionCommitteeDto> committees)
    {
        var committeeList = new List<ReportCommitteeLanguageMissingDto>();

        foreach (var committee in committees)
        {
            if (committee.ActiveMemberCount > 0)
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
                    var committeeDto = new ReportCommitteeLanguageMissingDto
                    {
                        Name = committee.GetDescription(),
                        MemberCount = committee.Memberships.Count,
                        Measure = committee.MeasuresLanguages,
                        Justification = committee.JustificationLanguages,
                        ItalianMissing = italianMissing,
                        FrenchMissing = frenchMissing,
                    };

                    committeeList.Add(committeeDto);
                }
            }
        }
        committeeList = committeeList.OrderBy(c => c.Name).ToList();

        return committeeList;
    }

    private async Task<List<ReportDepartmentWithCommitteesAndLanguagesDto>> GetCommitteesWithLanguagePercentagesByDepartment(IEnumerable<ReportGeneralElectionCommitteeDto> committees, IEnumerable<Department> departments)
    {
        var languageMeasures = (await _generalMeasureRepository.GetGeneralLanguageMeasures()).ToArray();

        var departmentList = new List<ReportDepartmentWithCommitteesAndLanguagesDto>();
        var dtoDict = new Dictionary<string, ReportDepartmentWithCommitteesAndLanguagesDto>();

        foreach (var department in departments)
        {
            var dto = new ReportDepartmentWithCommitteesAndLanguagesDto
            {
                Name = department.GetText(),
                Measure = languageMeasures.FirstOrDefault(g => g.DepartmentId == department.Id)?.Description,
            };

            dtoDict[department.GetText()] = dto;
            departmentList.Add(dto);

            var filteredCommittees = committees
                .Where(c => c.DepartmentId == department.Id)
                .ToList();

            var committeeList = new List<ReportCommitteeLanguageMissingDto>();

            foreach (var committee in filteredCommittees)
            {
                if (committee.Memberships.Count != 0 && committee.ActiveMemberCount > 0)
                {
                    var germanMembers = committee.Memberships.Select(m => m.Person!).Count(p => p.LanguageId == Guid.Parse(Language.GermanId));
                    var frenchMembers = committee.Memberships.Select(m => m.Person!).Count(p => p.LanguageId == Guid.Parse(Language.FrenchId));
                    var italianMembers = committee.Memberships.Select(m => m.Person!).Count(p => p.LanguageId == Guid.Parse(Language.ItalianId));
                    var romanshMembers = committee.Memberships.Select(m => m.Person!).Count(p => p.LanguageId == Guid.Parse(Language.RomanshId));

                    var german = Math.Round((decimal)germanMembers / committee.ActiveMemberCount * 100, 2);
                    var french = Math.Round((decimal)frenchMembers / committee.ActiveMemberCount * 100, 2);
                    var italian = Math.Round((decimal)italianMembers / committee.ActiveMemberCount * 100, 2);
                    var romansh = Math.Round((decimal)romanshMembers / committee.ActiveMemberCount * 100, 2);

                    if (italian < (decimal)committee.CommitteeType!.ItalianThresholdPercentage! || french < (decimal)committee.CommitteeType!.FrenchThresholdPercentage! ||
                        german < (decimal)committee.CommitteeType!.GermanThresholdPercentage!)
                    {
                        var committeeDto = new ReportCommitteeLanguageMissingDto
                        {
                            Name = committee.GetDescription(),
                            MemberCount = committee.Memberships.Count,
                            Measure = committee.MeasuresLanguages,
                            Justification = committee.JustificationLanguages,
                            GermanPercentage = german,
                            FrenchPercentage = french,
                            ItalianPercentage = italian,
                            RomanshPercentage = romansh,
                        };

                        committeeList.Add(committeeDto);
                    }
                }
            }

            dtoDict[department.GetText()].Committees = committeeList;
        }

        return departmentList;
    }

    private static List<ReportDepartmentWithCommitteesAndMembersDto> GetCommitteesAndMembersByDepartment(IEnumerable<ReportGeneralElectionCommitteeDto> committees, IEnumerable<Department> departments, ReportMembershipType type, TermOfOfficeDate? termOfOffice = null)
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
                .ToList();

            var committeeList = new List<ReportCommitteeWithMemberDetailDto>();

            foreach (var committee in filteredCommittees)
            {
                var members = GetMembershipsByType(committee, type, termOfOffice);

                var committeeDto = new ReportCommitteeWithMemberDetailDto
                {
                    Name = committee.GetDescription(),
                    MemberCount = members.Count(),
                    Members = members
                };

                committeeList.Add(committeeDto);
            }

            dtoDict[department.GetText()].Committees = committeeList;
        }

        return departmentList;
    }

    private static List<ReportCommitteeWithFreeTextDto> GetNonReleasedCommissions(IEnumerable<ReportGeneralElectionCommitteeDto> nonReleasedCommissions)
    {
        var committees = nonReleasedCommissions
            .Select(c => new ReportCommitteeWithFreeTextDto
            {
                Name = c.GetDescription(),
            })
            .OrderBy(c => c.Name)
            .ToList();

        return committees;
    }

    private static List<ReportCommitteeWithFreeTextDto> GetFederalDutyMembershipsWithOffice(IEnumerable<ReportGeneralElectionCommitteeDto> generalElectionCommitteesWithMembers)
    {
        var committees = generalElectionCommitteesWithMembers
            .Select(c => new ReportCommitteeWithFreeTextDto
            {
                Name = c.GetDescription(),
                FreeText = string.Join(", ",
                    c.Memberships
                     .Where(m => m.Person != null && m.Person.FederalDuty)
                     .Select(m => $"{m.Person!.Surname}, {m.Person!.GivenName} ({m.Person?.Office?.GetDescription()} {m.Person?.Office?.Department?.GetDescription()})"))
            })
            .OrderBy(c => c.Name)
            .ToList();

        committees = committees.OrderBy(c => c.Name).ToList();

        return committees;
    }

    private static List<ReportCommitteeWithFreeTextDto> GetMarketOrientatedMembershipData(IEnumerable<ReportGeneralElectionCommitteeDto> committees)
    {
        return committees
            .Select(c => new ReportCommitteeWithFreeTextDto
            {
                Name = c.GetDescription(),
                FreeText = string.Join(", ",
                    c.Memberships
                    .OrderBy(m => m.Function!.Sort)
                    .Select(m => $"{m.Surname} {m.GivenName}, {m.Function!.GetText()}, {m.MaximumEmploymentLevel}%")
                )
            })
            .OrderBy(c => c.Name)
            .ToList();
    }

    private static List<ReportCommitteeWithFreeTextDto> GetLongerMembershipData(IEnumerable<ReportCommitteeWithMemberDetailDto> committees)
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
            .OrderBy(c => c.Name)
            .ToList();
    }

    private static List<ReportCommitteeWithMemberDetailDto> GetCommitteesAndMembers(IEnumerable<ReportGeneralElectionCommitteeDto> committees, ReportMembershipType type, TermOfOfficeDate? termOfOffice = null)
    {
        return committees
            .Select(c => new ReportCommitteeWithMemberDetailDto
            {
                Name = c.GetDescription(),
                MemberCount = c.ActiveMemberCount
            })
            .OrderBy(c => c.Name)
            .ToList();
    }

    private static IEnumerable<ReportMembershipDto> GetMembershipsByType(ReportGeneralElectionCommitteeDto committee, ReportMembershipType type, TermOfOfficeDate? termOfOffice = null)
    {
        if (type == ReportMembershipType.ShorterDuty)
        {
            return committee.Memberships.Where(m => m.EndDate < termOfOffice!.EndDate).Select(membership => new ReportMembershipDto
            {
                Surname = membership.Person!.Surname,
                GivenName = membership.Person!.GivenName,
                Type = ReportMembershipType.ShorterDuty,
                Justification = membership.JustificationShorterDuty,
                FreeText = string.Format(BusinessTexts.Report_ShorterDutyText, membership.BeginDate.ToShortDateString(), membership.EndDate.ToShortDateString())
            });
        }
        else if (type == ReportMembershipType.FederalAssembly)
        {
            return committee.Memberships.Where(m => m.Person != null && m.Person!.FederalAssembly).Select(membership => new ReportMembershipDto
            {
                Surname = membership.Person!.Surname,
                GivenName = membership.Person!.GivenName,
                Type = ReportMembershipType.FederalAssembly,
                Function = membership.Person.CouncilId == Council.CouncilOfStateId && membership.Person.GenderId == Gender.FemaleGuid ? BusinessTexts.CouncilOfStateFemale :
                    membership.Person.CouncilId == Council.CouncilOfStateId && membership.Person.GenderId == Gender.MaleGuid ? BusinessTexts.CouncilOfStateMale :
                    membership.Person.CouncilId == Council.NationalCouncilId && membership.Person.GenderId == Gender.FemaleGuid ? BusinessTexts.NationalCouncilFemale :
                    membership.Person.CouncilId == Council.NationalCouncilId && membership.Person.GenderId == Gender.MaleGuid ? BusinessTexts.NationalCouncilMale : string.Empty,
                Justification = membership.JustificationMemberInFederalAssembly,
            });
        }
        else if (type == ReportMembershipType.FederalDuty)
        {
            return committee.Memberships.Where(m => m.Person != null && m.Person!.FederalDuty).Select(membership => new ReportMembershipDto
            {
                Surname = membership.Person!.Surname,
                GivenName = membership.Person!.GivenName,
                Type = ReportMembershipType.FederalDuty,
                Function = membership.Person!.Gender!.Uri == Gender.Female ? string.Join(" / ", membership.Person!.Occupations.Select(o => o.GetFemaleText(CultureInfo.CurrentUICulture)).Order()) : string.Join(" / ", membership.Person!.Occupations.Select(o => o.GetText(CultureInfo.CurrentUICulture)).Order()),
                Justification = membership.JustificationMemberInFederalDuty,
            });
        }
        else if (type == ReportMembershipType.MarketOrientated)
        {
            return committee.Memberships.Select(membership => new ReportMembershipDto
            {
                Surname = membership.Person!.Surname,
                GivenName = membership.Person!.GivenName,
                Type = ReportMembershipType.MarketOrientated,
                Function = membership.Function!.GetText(),
                FreeText = $"{membership.MaximumEmploymentLevel} %",
            });
        }
        else if (type == ReportMembershipType.CompetenceProfile)
        {
            return committee.Memberships.Where(m => m.Person != null && m.RequirementsProfile != null).Select(membership =>
                new ReportMembershipDto
                {
                    Surname = membership.Person!.Surname,
                    GivenName = membership.Person!.GivenName,
                    Type = ReportMembershipType.CompetenceProfile,
                    Function = membership.Function!.GetText(),
                    FreeText = membership.RequirementsProfile != null ? Regex.Replace(membership.RequirementsProfile.ToString(), "<.*?>", string.Empty) : string.Empty,
                    FreeText2 = membership.Person!.Gender!.Uri == Gender.Female ? string.Join(" / ", membership.Person!.Occupations.Select(o => o.GetFemaleText(CultureInfo.CurrentUICulture)).Order()) : string.Join(" / ", membership.Person!.Occupations.Select(o => o.GetText(CultureInfo.CurrentUICulture)).Order()),
                    FreeText3 = membership.Person!.Employer,
                });
        }
        else
        {
            return new List<ReportMembershipDto>();
        }
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

    private static List<ReportDepartmentWithCommitteesAndMembersDto> SummarizeMembershipsFromPresentAndFutureByDepartment(IEnumerable<ReportGeneralElectionCommitteeDto> committees,
        IEnumerable<ReportGeneralElectionCommitteeDto> geCommitteesWithMembers, IEnumerable<Department> departments)
    {
        // which members we have in the future?
        var reportGeneralElectionCommitteeDtos = geCommitteesWithMembers.ToList();
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
                        .ToList(),
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
                    Office = c.Office,
                })
                // If a committee has no relevant members → remove it
                .Where(c => c.Memberships.Count != 0)
                .ToList();

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
                .ToList();

            return new
            {
                Committee = c,
                QualifiedMembers = membersWithDuration
            };
        });

        var groupedByDepartment = committeesWithQualifiedMembers
            .GroupBy(c => c.Committee.Department)
            .ToList();

        var result = departments.Select(dept =>
        {
            // Get all committees in this department (even if empty)
            var committeesInDepartment = groupedByDepartment
                .Where(g => g.Key!.Id == dept.Id)
                .SelectMany(g => g)
                .ToList();

            // Map committees with filtered members
            var committeesDto = committeesInDepartment.Select(cwm => new ReportCommitteeWithMemberDetailDto
            {
                Name = cwm.Committee.GetDescription(),
                MemberCount = cwm.QualifiedMembers.Count,
                Members = cwm.QualifiedMembers.Select(m => new ReportMembershipDto
                {
                    Surname = m.Person.Surname,
                    GivenName = m.Person.GivenName,
                    FreeText = $"{m.TotalDurationYears} {BusinessTexts.Report_Years}",
                }).ToList()
            })
                .ToList();

            return new ReportDepartmentWithCommitteesAndMembersDto
            {
                Name = dept.GetText(),
                Committees = committeesDto
            };
        }).ToList();

        return result;
    }

    private static List<ReportCommitteeWithMemberDetailDto> SummarizeMembershipsFromPresentAndFuture(IEnumerable<ReportGeneralElectionCommitteeDto> committees,
        IEnumerable<ReportGeneralElectionCommitteeDto> geCommitteesWithMembers)
    {
        // which members we have in the future?
        var reportGeneralElectionCommitteeDtos = geCommitteesWithMembers.ToList();
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
                        .ToList(),
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
                    Office = c.Office,
                })
                // If a committee has no relevant members → remove it
                .Where(c => c.Memberships.Count != 0)
                .ToList();

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
                .ToList();

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
            MemberCount = cwm.QualifiedMembers.Count,
            Members = cwm.QualifiedMembers.Select(m => new ReportMembershipDto
            {
                Surname = m.Person.Surname,
                GivenName = m.Person.GivenName,
                FreeText = $"{m.TotalDurationYears} {BusinessTexts.Report_Years}",
            }).ToList()
        })
        .Where(cwm => cwm.MemberCount > 0)
        .ToList();

        return committeesDto;
    }
}
