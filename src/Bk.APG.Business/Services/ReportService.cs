using System.Globalization;
using System.IO.Compression;
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
    private readonly Swiss.FCh.DocumentService.Client.IDocumentService _documentService;
    private readonly ITermOfOfficeDateService _termOfOfficeDateService;
    private readonly ICultureService _cultureService;
    private readonly IElectoralListService _electoralListService;
    private readonly IEiamAssignmentService _eiamAssignmentService;
    private readonly IDocumentService _documentServiceInternal;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly IGeneralMeasureRepository _generalMeasureRepository;
    private readonly IFormLetterSenderRepository _formLetterSenderRepository;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        Swiss.FCh.DocumentService.Client.IDocumentService documentService,
        ICultureService cultureService,
        ITermOfOfficeDateService termOfOfficeDateService,
        IElectoralListService electoralListService,
        IEiamAssignmentService eiamAssignmentService,
        IDocumentService documentServiceInternal,
        ICommitteeRepository committeeRepository,
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        IMasterDataRepository masterDataRepository,
        IGeneralMeasureRepository generalMeasureRepository,
        IFormLetterSenderRepository formLetterSenderRepository,
        ILogger<ReportService> logger)
    {
        _documentService = documentService;
        _termOfOfficeDateService = termOfOfficeDateService;
        _cultureService = cultureService;
        _electoralListService = electoralListService;
        _eiamAssignmentService = eiamAssignmentService;
        _documentServiceInternal = documentServiceInternal;
        _committeeRepository = committeeRepository;
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _masterDataRepository = masterDataRepository;
        _generalMeasureRepository = generalMeasureRepository;
        _formLetterSenderRepository = formLetterSenderRepository;
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
            ReportType.Vacancies => await GenerateVacanciesReport(filterDto),
            _ => await GenerateParliamentaryReport(filterDto)
        };
    }

    public async Task<(string fileName, Stream content)> CreateFormLetterAsZipFile(FormLetterFilterParameters filterDto)
    {
        _logger.LogInformation("Generate form letter report");

        var reportDto = await FillFormLetterDto(filterDto);

        var zipStream = new MemoryStream();

        if (reportDto.Memberships != null && filterDto.ExportType == "single")
        {
            var template = "FormLetterGeneralElection";

            var allMemberships = reportDto.Memberships.ToList();

            var grouped = reportDto.Memberships.GroupBy(m => m.CommitteeId).ToList();

            using var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true);
            foreach (var group in grouped)
            {
                var currentCommitteeId = group.Key;

                var first = group.First();

                var reducedDataDto = reportDto;
                reducedDataDto.Memberships = allMemberships;

                var reducedMembersDto = reducedDataDto.Memberships.Where(m => m.CommitteeId == currentCommitteeId);
                reducedDataDto.Memberships = reducedMembersDto;

                var fileName = first.CommitteeName;

                if (filterDto.ExportFileType == "word")
                {
                    var zipFile = zip.CreateEntry($"{fileName}.docx", CompressionLevel.Fastest);
                    await using var zipFileStream = zipFile.Open();

                    await using var documentStream = (MemoryStream)await _documentService.CreateWordFromTemplate($"Templates/{template}.docx", reducedDataDto, "formLetter");
                    await documentStream.CopyToAsync(zipFileStream);
                }
                else
                {
                    var zipFile = zip.CreateEntry($"{fileName}.pdf", CompressionLevel.Fastest);
                    await using var zipFileStream = zipFile.Open();

                    await using var documentStream = (MemoryStream)await _documentService.CreatePdfFromTemplate($"Templates/{template}.docx", reducedDataDto, "formLetter");
                    await documentStream.CopyToAsync(zipFileStream);
                }
            }
        }

        zipStream.Position = 0;
        return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd}_{BusinessTexts.FormLetterCompleteExport_Filename}.zip", zipStream);
    }

    public async Task<(string fileName, Stream content)> CreateFormLetterSingleDocument(FormLetterFilterParameters filterDto)
    {
        var template = "FormLetterGeneralElection";

        var reportDto = await FillFormLetterDto(filterDto);

        if (filterDto.ExportFileType == "word")
        {
            await using var documentStream = (MemoryStream)await _documentService.CreateWordFromTemplate($"Templates/{template}.docx", reportDto, "formLetter");

            var stream = new MemoryStream();
            await documentStream.CopyToAsync(stream);
            stream.Position = 0;

            return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd}_{BusinessTexts.FormLetterCompleteExport_Filename}.docx", stream);
        }
        else
        {
            await using var documentStream = (MemoryStream)await _documentService.CreatePdfFromTemplate($"Templates/{template}.docx", reportDto, "formLetter");

            var stream = new MemoryStream();
            await documentStream.CopyToAsync(stream);
            stream.Position = 0;

            return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd}_{BusinessTexts.FormLetterCompleteExport_Filename}.pdf", stream);
        }
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

        var committeesWithMembersInFederalDuty = generalElectionCommitteesWithMembers.Where(c => c.Memberships.Any(m => m.Person?.FederalDuty == true)).ToList();

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

    private async Task<(string fileName, Stream content)> GenerateVacanciesReport(ReportFilterParametersDto filterDto)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var departments = await _masterDataRepository.GetDepartments();
        departments = departments.Where(d => d.Uri != Department.BkUri).ToArray();

        var template = _cultureService.GetCurrentUiCulture().TwoLetterISOLanguageName == "fr" ? "Vacancies_Report_French" : "Vacancies_Report_German";

        var nextTermOfOfficeDate = await _termOfOfficeDateService.GetNextTermOfOfficeDate();

        var committees = (await _committeeRepository.GetByFilterForReport(filterDto, departmentId, officeId, committeeId)).ToArray();

        var generalElectionCommittees = committees.Select(ReportMapper.FromCommitteeToReportGeneralElectionCommitteeDto).ToList();

        var reportDepartments = GetCommitteesByDepartment(generalElectionCommittees, departments, ReportCommitteeType.Vacancies);

        var vacanciesReportDto = new VacanciesReportDto
        {
            TermOfOfficeDateRange = nextTermOfOfficeDate.BeginDate.Year + " - " + nextTermOfOfficeDate.EndDate?.Year,

            Departments = reportDepartments,
        };

        var documentStream = await _documentService.CreateWordFromTemplate($"Templates/{template}.docx", vacanciesReportDto, "vacanciesReport");

        return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd}_{BusinessTexts.Vacancies_Report_Filename}.docx", documentStream);
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
                var freeText2 = "";
                var membershipCount = committee.Memberships.Count;
                var onlyAddWhenData = false;

                if (type == ReportCommitteeType.SelectionProcedure)
                {
                    var cleanSelectionProcedure = committee.SelectionProcedure != null ? Regex.Replace(committee.SelectionProcedure.ToString(), "<.*?>", string.Empty) : string.Empty;
                    freeText = string.Format(BusinessTexts.Report_SelectionProcedure, cleanSelectionProcedure);
                }
                else if (type == ReportCommitteeType.Vacancies)
                {
                    onlyAddWhenData = true;
                    freeText = string.Join(", ", committee.MembershipAdditionsInGeneralElection.Select(m => m.GetText()));
                    freeText2 = !string.IsNullOrWhiteSpace(committee.LinkHomepageGerman) ? committee.LinkHomepageGerman : !string.IsNullOrWhiteSpace(committee.LinkHomepageFrench) ? committee.LinkHomepageFrench : string.Empty;
                    membershipCount = committee.VacanciesGeneralElection != null ? (int)committee.VacanciesGeneralElection : 0;
                }
                else
                {
                    freeText = termOfOfficeDate != null && committee.EndDate < termOfOfficeDate.EndDate && committee.EndDate > termOfOfficeDate.BeginDate ? string.Format(BusinessTexts.Report_EndDateInPast, committee.EndDate.Value.ToShortDateString()) : termOfOfficeDate != null && committee.BeginDate > termOfOfficeDate.BeginDate && committee.BeginDate < termOfOfficeDate.EndDate ? string.Format(BusinessTexts.Report_BeginDateInFuture, committee.BeginDate.ToShortDateString()) : string.Empty;
                }

                if (!onlyAddWhenData || membershipCount > 0)
                {
                    var committeeDto = new ReportCommitteeDto
                    {
                        Name = committee.GetDescription(),
                        MemberCount = membershipCount,
                        Justification = committee.JustificationMembers,
                        FreeText = freeText,
                        FreeText2 = freeText2,
                    };

                    committeeList.Add(committeeDto);
                }
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
                    .ThenBy(m => m.Surname)
                    .ThenBy(m => m.GivenName)
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

    private async Task<FormLetterReportDto> FillFormLetterDto(FormLetterFilterParameters filterDto)
    {
        var allElectionTypes = await _masterDataRepository.GetElectionTypes();
        var electionTypeList = allElectionTypes.Select(e => e.Id).ToList();
        electionTypeList.Remove(ElectionType.MembershipEndedBecauseOfDeathGuid);
        electionTypeList.Remove(ElectionType.PermanentGuid);

        if (filterDto.ElectionTypeIds != null && filterDto.ElectionTypeIds.Any())
        {
            electionTypeList = electionTypeList
                       .Where(id => filterDto.ElectionTypeIds.Contains(id))
                       .ToList();
        }

        var electionTypeListPresent = electionTypeList.ToList();
        electionTypeListPresent.Remove(ElectionType.NewElectionGuid);
        electionTypeListPresent.Remove(ElectionType.ReElectionGuid);

        var electionTypeListFuture = electionTypeList.ToList();
        electionTypeListFuture.Remove(ElectionType.MaximumMembershipDurationGuid);
        electionTypeListFuture.Remove(ElectionType.OtherRetirementReasonGuid);
        electionTypeListFuture.Remove(ElectionType.RetirementGuid);

        var sender = await _formLetterSenderRepository.GetByIdForUpdate(filterDto.FormLetterSenderId);

        var nextTermOfOfficeDate = await _termOfOfficeDateService.GetNextTermOfOfficeDate();
        var currentTermOfOfficeDate = await _termOfOfficeDateService.GetCurrentTermOfOfficeDate();

        filterDto.EndDateCurrentTermOfOfficeDate = currentTermOfOfficeDate.EndDate;

        var newAndReElections = await GetNewAndReelectionMemberships(filterDto, electionTypeListFuture, sender);

        var endedMemberships = await GetEndedMemberships(filterDto, electionTypeListPresent, sender);

        var allRecipients = newAndReElections.Concat(endedMemberships).ToList();

        if (sender != null && nextTermOfOfficeDate != null && currentTermOfOfficeDate != null)
        {
            var signaturePictureExists = false;
            var picBase64 = string.Empty;

            if (sender.SignatureFileReference != null)
            {
                using var signatureStream = await _documentServiceInternal.GetDocument(sender.SignatureFileReference.DocumentStorageId ?? string.Empty);

                if (signatureStream != null && signatureStream.CanSeek)
                {
                    signatureStream.Position = 0;
                    picBase64 = Convert.ToBase64String(signatureStream.ToArray());
                    signaturePictureExists = true;
                }
            }

            var formLetterReportDto = new FormLetterReportDto
            {
                NextTermOfOfficeBeginDate = nextTermOfOfficeDate.BeginDate.ToString("dd.MM.yyyy"),
                NextTermOfOfficeEndDate = nextTermOfOfficeDate.EndDate?.ToString("dd.MM.yyyy") ?? "",
                TermOfOfficeEndDate = currentTermOfOfficeDate.EndDate?.ToString("dd.MM.yyyy") ?? "",
                Memberships = allRecipients,
                HasSignature = signaturePictureExists,
                SenderSignature = picBase64,
                SenderOffice = sender.Office?.DescriptionDe,
                SenderOfficeShort = sender.Office?.TextDe,
                SenderName = sender.GivenName + " " + sender.Surname,
                SenderStreet = sender.StreetGerman,
                SenderZip = sender.Zip,
                SenderCity = sender.CityGerman,
                SenderPhone = sender.Phone,
                SenderEmail = sender.Email,
                SenderWebsite = sender.Website,
            };

            return formLetterReportDto;
        }
        else
        {
            return new FormLetterReportDto();
        }
    }

    private async Task<List<FormLetterMembershipReportDto>> GetNewAndReelectionMemberships(FormLetterFilterParameters filterDto, List<Guid> electionTypeListFuture, FormLetterSender sender)
    {
        var generalElectionTextGerman = "Gesamterneuerungswahl";
        var generalElectionTextFrench = "Renouvellement intégral";
        var generalElectionTextItalian = "Rinnovo integrale";
        var generalElectionTextRomansh = "Renovaziun totala";

        var currentMonthAndYearGerman = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("de-CH")) ?? "";
        var currentMonthAndYearFrench = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("fr-CH")) ?? "";
        var currentMonthAndYearItalian = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("it-CH")) ?? "";
        var currentMonthAndYearRomansh = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("rm-CH")) ?? "";

        var allValidMemberships = await _generalElectionCommitteeRepository.GetAllForFormLetter(filterDto, electionTypeListFuture);

        var newAndReElections = allValidMemberships
            .SelectMany(c => c.MembershipCandidates.Where(m => m.PersonId != null && m.Person!.CorrespondenceAddressId != null).Select(m => new FormLetterMembershipReportDto
            {
                FormLetterType = m.ElectionTypeId == ElectionType.NewElectionGuid ? FormLetterType.NewElection : FormLetterType.ReElection,
                FormLetterLanguage = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? FormLetterLanguage.German :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? FormLetterLanguage.French :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? FormLetterLanguage.Italian : FormLetterLanguage.Romansh,
                SenderDepartment = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.Department!.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.Department!.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.Department!.DescriptionIt : sender.Department!.DescriptionRm,
                SenderOffice = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.Office?.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.Office?.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.Office?.DescriptionIt : sender.Office?.DescriptionRm,
                SenderOfficeShort = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.Office?.TextDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.Office?.TextFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.Office?.TextIt : sender.Office?.TextRm,
                SenderName = sender.GivenName + " " + sender.Surname,
                SenderFunction = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.SenderFunction!.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.SenderFunction!.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.SenderFunction!.DescriptionIt : sender.SenderFunction!.DescriptionRm,
                SenderStreet = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.StreetGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.StreetFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.StreetItalian : sender.StreetRomansh,
                SenderZip = sender.Zip,
                SenderCity = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.CityGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.CityFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.CityItalian : sender.CityRomansh,
                Subject = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? generalElectionTextGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? generalElectionTextFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? generalElectionTextItalian : generalElectionTextRomansh,
                DateLetter = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? currentMonthAndYearGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? currentMonthAndYearFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? currentMonthAndYearItalian : currentMonthAndYearRomansh,
                CommitteeId = c.CommitteeId,
                CommitteeName = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? c.DescriptionGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? c.DescriptionFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? c.DescriptionItalian : c.DescriptionRomansh,
                CorrespondenceLanguageId = m.Person != null ? m.Person!.CorrespondenceLanguageId : Guid.Empty,
                Function = m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextDe :
                    m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.GenderId == Gender.FemaleGuid ? m.Function!.TextFemaleDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextFr :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.GenderId == Gender.FemaleGuid ? m.Function!.TextFemaleFr :
                m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextIt :
                m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.GenderId == Gender.FemaleGuid ? m.Function!.TextFemaleIt :
                m.Person!.CorrespondenceLanguageId == Language.RomanshGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextRm : string.Empty,
                Salutation = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? m.Person!.Salutation!.TextDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? m.Person!.Salutation!.TextFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? m.Person!.Salutation!.TextIt :
                    m.Person!.CorrespondenceLanguageId == Language.RomanshGuid ? m.Person!.Salutation!.TextRm : string.Empty,
                SalutationText = m.Person!.SalutationText ?? string.Empty,
                GivenName = m.Person!.GivenName ?? string.Empty,
                Surname = m.Person!.Surname ?? string.Empty,
                CompanyName = m.Person!.CorrespondenceAddress!.CompanyName ?? string.Empty,
                Street = m.Person!.CorrespondenceAddress.Street ?? string.Empty,
                PoBox = m.Person!.CorrespondenceAddress.PoBox ?? string.Empty,
                Zip = m.Person!.CorrespondenceAddress!.Zip ?? string.Empty,
                City = m.Person!.CorrespondenceAddress!.City ?? string.Empty,
                Country = m.Person!.CorrespondenceAddress.Country == null ? string.Empty : m.Person!.CorrespondenceAddress.Country!.TextDe == "CH" ? string.Empty :
                    m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionIt :
                    m.Person!.CorrespondenceLanguageId == Language.RomanshGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionRm : string.Empty,
            }))
            .ToList();

        return newAndReElections;
    }

    private async Task<List<FormLetterMembershipReportDto>> GetEndedMemberships(FormLetterFilterParameters filterDto, List<Guid> electionTypeListPresent, FormLetterSender sender)
    {
        var generalElectionTextGerman = "Gesamterneuerungswahl";
        var generalElectionTextFrench = "Renouvellement intégral";
        var generalElectionTextItalian = "Rinnovo integrale";
        var generalElectionTextRomansh = "Renovaziun totala";

        var currentMonthAndYearGerman = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("de-CH")) ?? "";
        var currentMonthAndYearFrench = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("fr-CH")) ?? "";
        var currentMonthAndYearItalian = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("it-CH")) ?? "";
        var currentMonthAndYearRomansh = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("rm-CH")) ?? "";

        var allEndedMemberships = await _committeeRepository.GetAllForFormLetter(filterDto, electionTypeListPresent);

        var endedMemberships = allEndedMemberships
            .SelectMany(c => c.Memberships.Select(m => new FormLetterMembershipReportDto
            {
                FormLetterType = m.ElectionTypeId == ElectionType.RetirementGuid ? FormLetterType.Retire : m.ElectionTypeId == ElectionType.MaximumMembershipDurationGuid ? FormLetterType.MaximumMembershipDuration : FormLetterType.OtherRetirement,
                FormLetterLanguage = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? FormLetterLanguage.German :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? FormLetterLanguage.French :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? FormLetterLanguage.Italian : FormLetterLanguage.Romansh,
                SenderDepartment = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.Department!.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.Department!.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.Department!.DescriptionIt : sender.Department!.DescriptionRm,
                SenderOffice = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.Office?.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.Office?.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.Office?.DescriptionIt : sender.Office?.DescriptionRm,
                SenderOfficeShort = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.Office?.TextDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.Office?.TextFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.Office?.TextIt : sender.Office?.TextRm,
                SenderName = sender.GivenName + " " + sender.Surname,
                SenderFunction = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.SenderFunction!.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.SenderFunction!.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.SenderFunction!.DescriptionIt : sender.SenderFunction!.DescriptionRm,
                SenderStreet = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.StreetGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.StreetFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.StreetItalian : sender.StreetRomansh,
                SenderZip = sender.Zip,
                SenderCity = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.CityGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.CityFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.CityItalian : sender.CityRomansh,
                Subject = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? generalElectionTextGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? generalElectionTextFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? generalElectionTextItalian : generalElectionTextRomansh,
                DateLetter = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? currentMonthAndYearGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? currentMonthAndYearFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? currentMonthAndYearItalian : currentMonthAndYearRomansh,
                CommitteeId = c.Id,
                CommitteeName = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? c.DescriptionGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? c.DescriptionFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? c.DescriptionItalian : c.DescriptionRomansh,
                CorrespondenceLanguageId = m.Person != null ? m.Person!.CorrespondenceLanguageId : Guid.Empty,
                Function = m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextDe :
                    m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.GenderId == Gender.FemaleGuid ? m.Function!.TextFemaleDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextFr :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.GenderId == Gender.FemaleGuid ? m.Function!.TextFemaleFr :
                m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextIt :
                m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.GenderId == Gender.FemaleGuid ? m.Function!.TextFemaleIt :
                m.Person!.CorrespondenceLanguageId == Language.RomanshGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextRm : string.Empty,
                Salutation = m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.Salutation != null ? m.Person!.Salutation!.TextDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.Salutation != null ? m.Person!.Salutation!.TextFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.Salutation != null ? m.Person!.Salutation!.TextIt :
                    m.Person!.CorrespondenceLanguageId == Language.RomanshGuid && m.Person!.Salutation != null ? m.Person!.Salutation!.TextRm : string.Empty,
                SalutationText = m.Person!.SalutationText ?? string.Empty,
                GivenName = m.Person!.GivenName ?? string.Empty,
                Surname = m.Person!.Surname ?? string.Empty,
                CompanyName = m.Person!.CorrespondenceAddress!.CompanyName ?? string.Empty,
                Street = m.Person!.CorrespondenceAddress.Street ?? string.Empty,
                PoBox = m.Person!.CorrespondenceAddress.PoBox ?? string.Empty,
                Zip = m.Person!.CorrespondenceAddress!.Zip ?? string.Empty,
                City = m.Person!.CorrespondenceAddress!.City ?? string.Empty,
                Country = m.Person!.CorrespondenceAddress.Country == null ? string.Empty : m.Person!.CorrespondenceAddress.Country!.TextDe == "CH" ? string.Empty :
                    m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionIt :
                    m.Person!.CorrespondenceLanguageId == Language.RomanshGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionRm : string.Empty,
            }))
            .ToList();

        return endedMemberships;
    }
}
