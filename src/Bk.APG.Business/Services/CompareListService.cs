using System.Globalization;
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
    private readonly ICultureService _cultureService;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly ILogger<CompareListService> _logger;

    public CompareListService(
        Swiss.FCh.DocumentService.Client.IDocumentService documentService,
        IEiamAssignmentService eiamAssignmentService,
        ICultureService cultureService,
        ICommitteeRepository committeeRepository,
        IMasterDataRepository masterDataRepository,
        ILogger<CompareListService> logger
    )
    {
        _documentService = documentService;
        _eiamAssignmentService = eiamAssignmentService;
        _cultureService = cultureService;
        _committeeRepository = committeeRepository;
        _masterDataRepository = masterDataRepository;
        _logger = logger;
    }

    public async Task<(string fileName, Stream content)> GenerateDocument(ReportFilterParametersDto filterDto)
    {
        ArgumentNullException.ThrowIfNull(filterDto);

        _logger.LogInformation("Generating CompareList GeneralElection document");
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var allCommitteeTypes = await _masterDataRepository.GetCommitteeTypes();
        // exclude the committeeTyp "Vertretungen des Bundes"
        allCommitteeTypes = allCommitteeTypes.Where(ct => ct.Id == CommitteeType.AuthoritiesCommissionGuid || ct.Id == CommitteeType.ManagementCommitteeGuid ||
            ct.Id == CommitteeType.AdministrationCommissionGuid || ct.Id == CommitteeType.FederalAgenciesCommitteeGuid);

        var departments = await _masterDataRepository.GetDepartments();
        // exclude BK, as there are no committees and it should not be in the document
        departments = departments.Where(d => d.Uri != Department.BkUri).ToArray();

        var committeesFromDate1 = (await _committeeRepository.GetByFilterForReport(departmentId, officeId, committeeId, filterDto, filterDto.AnalysisDate1)).ToArray();

        var committeesFromDate2 = (await _committeeRepository.GetByFilterForReport(departmentId, officeId, committeeId, filterDto, filterDto.AnalysisDate2)).ToArray();

        var compareListDto = new CompareListDto
        {
            StartYear = filterDto.AnalysisDate1?.Year.ToString(CultureInfo.InvariantCulture),
            EndYear = filterDto.AnalysisDate2?.Year.ToString(CultureInfo.InvariantCulture)
        };
        var compareCommitteeTypeList = new List<CompareListCommitteeTypeDto>();

        foreach (var committeeType in allCommitteeTypes)
        {
            var compareDepartmentList = new List<CompareListDepartmentDto>();

            var committeeTypeDto = new CompareListCommitteeTypeDto()
            {
                CommitteeTypeId = committeeType.Id,
                Name = committeeType.GetText()
            };

            foreach (var department in departments)
            {
                var departmentDto = FillCompareListDepartmentDto(committeeType, department, committeesFromDate1, committeesFromDate2);
                compareDepartmentList.Add(departmentDto);
            }
            committeeTypeDto.Departments = compareDepartmentList;
            compareCommitteeTypeList.Add(committeeTypeDto);
        }

        compareListDto.CommitteeTypes = compareCommitteeTypeList;

        var template = _cultureService.GetCurrentUiCulture().TwoLetterISOLanguageName == Language.French ? "Compare_List_French" : "Compare_List_German";

        var documentStream = await _documentService.CreateWordFromTemplate($"Templates/{template}.docx", compareListDto, "compareList");

        return ($"{DateTime.Today:yyyyMMdd}_{BusinessTexts.CompareList_FileName}.docx", documentStream);
    }

    private static CompareListDepartmentDto FillCompareListDepartmentDto(CommitteeType committeeType, Department department, Committee[] committeesFromDate1, Committee[] committeesFromDate2)
    {
        var departmentDto = new CompareListDepartmentDto()
        {
            DepartmentId = department.Id,
            Name = department.GetText()
        };

        var compareListCommittees = new List<CompareListCommitteeDto>();

        var filteredCommittees1 = committeesFromDate1.Where(c => c.DepartmentId == department.Id && c.CommitteeTypeId == committeeType.Id).ToArray();
        var filteredCommittees2 = committeesFromDate2.Where(c => c.DepartmentId == department.Id && c.CommitteeTypeId == committeeType.Id).ToArray();

        // loop 1, we add all the matching committees from Date 1.
        foreach (var committee in filteredCommittees1)
        {
            var compareListCommittee = FillCommitteeDto(committee, filteredCommittees2);

            if (compareListCommittee != null)
            {
                compareListCommittees.Add(compareListCommittee);
            }
        }

        // loop 2, we add only the committees, which haven't been added in loop 1
        foreach (var committee in filteredCommittees2)
        {
            if (compareListCommittees.Any(c => c.Id == committee.Id))
            {
                continue;
            }

            var compareListCommittee = FillCommitteeDto(committee, filteredCommittees1);

            if (compareListCommittee != null)
            {
                compareListCommittees.Add(compareListCommittee);
            }
        }
        departmentDto.Committees = compareListCommittees;

        return departmentDto;
    }

    private static CompareListCommitteeDto? FillCommitteeDto(Committee committee, Committee[] compareCommittees)
    {
        var compareCommittee = compareCommittees.FirstOrDefault(c => c.Id == committee.Id);

        // as we have a certain date and not the current data, we cannot use the caluclated variables like committee.GermanUnderStaffed, etc.
        var currentCommitteeType = committee.CommitteeType;
        var activeMemberCountOld = committee.Memberships.Count;
        var activeMemberCountNew = compareCommittee is not null ? compareCommittee.Memberships.Count : 0;
        bool germanUnderStaffedOld;
        bool frenchUnderStaffedOld;
        bool italianUnderStaffedOld;
        var germanUnderStaffedNew = false;
        var frenchUnderStaffedNew = false;
        var italianUnderStaffedNew = false;
        string germanTextOld;
        var germanTextNew = string.Empty;
        string frenchTextOld;
        var frenchTextNew = string.Empty;
        string italianTextOld;
        var italianTextNew = string.Empty;
        var genderPartsOld = new List<string>();
        var genderPartsNew = new List<string>();
        var femaleUnderStaffedNew = false;
        var maleUnderStaffedNew = false;

        var femaleUnderStaffedOld = activeMemberCountOld > 0 && (double)committee.Memberships.Count(m => m.Person!.GenderId == Gender.FemaleGuid) / activeMemberCountOld * 100 < currentCommitteeType!.FemaleThreshold;
        var maleUnderStaffedOld = activeMemberCountOld > 0 && (double)committee.Memberships.Count(m => m.Person!.GenderId == Gender.MaleGuid) / activeMemberCountOld * 100 < currentCommitteeType!.MaleThreshold;

        if (compareCommittee != null)
        {
            femaleUnderStaffedNew = activeMemberCountNew > 0 && (double)compareCommittee.Memberships.Count(m => m.Person!.GenderId == Gender.FemaleGuid) / activeMemberCountNew * 100 < currentCommitteeType!.FemaleThreshold;
            maleUnderStaffedNew = activeMemberCountNew > 0 && (double)compareCommittee.Memberships.Count(m => m.Person!.GenderId == Gender.MaleGuid) / activeMemberCountNew * 100 < currentCommitteeType!.MaleThreshold;
        }

        if (femaleUnderStaffedOld)
        {
            genderPartsOld.Add(CreateGenderPercentageText(activeMemberCountOld, committee.Memberships.Count(m => m.Person!.GenderId == Gender.FemaleGuid), Gender.FemaleGuid));
        }
        if (maleUnderStaffedOld)
        {
            genderPartsOld.Add(CreateGenderPercentageText(activeMemberCountOld, committee.Memberships.Count(m => m.Person!.GenderId == Gender.MaleGuid), Gender.MaleGuid));
        }
        if (femaleUnderStaffedNew)
        {
            genderPartsNew.Add(CreateGenderPercentageText(activeMemberCountNew, compareCommittee!.Memberships.Count(m => m.Person!.GenderId == Gender.FemaleGuid), Gender.FemaleGuid));
        }
        if (maleUnderStaffedNew)
        {
            genderPartsNew.Add(CreateGenderPercentageText(activeMemberCountNew, compareCommittee!.Memberships.Count(m => m.Person!.GenderId == Gender.MaleGuid), Gender.MaleGuid));
        }

        if (currentCommitteeType is { GermanMinimalThreshold: not null })
        {
            // we work with minimum number of members
            germanUnderStaffedOld = activeMemberCountOld > 0 && committee.Memberships.Count(m => m.Person!.LanguageId == Language.GermanGuid) < currentCommitteeType.GermanMinimalThreshold;
            frenchUnderStaffedOld = activeMemberCountOld > 0 && committee.Memberships.Count(m => m.Person!.LanguageId == Language.FrenchGuid) < currentCommitteeType.FrenchMinimalThreshold;
            italianUnderStaffedOld = activeMemberCountOld > 0 && committee.Memberships.Count(m => m.Person!.LanguageId == Language.ItalianGuid) < currentCommitteeType.ItalianMinimalThreshold;
            germanTextOld = germanUnderStaffedOld ? $"DE: {BusinessTexts.Compare_List_Language_Missing}" : BusinessTexts.Compare_List_Language_OK;
            frenchTextOld = frenchUnderStaffedOld ? $"FR: {BusinessTexts.Compare_List_Language_Missing}" : BusinessTexts.Compare_List_Language_OK;
            italianTextOld = italianUnderStaffedOld ? $"IT: {BusinessTexts.Compare_List_Language_Missing}" : BusinessTexts.Compare_List_Language_OK;

            if (compareCommittee != null)
            {
                germanUnderStaffedNew = activeMemberCountNew > 0 && compareCommittee.Memberships.Count(m => m.Person!.LanguageId == Language.GermanGuid) < currentCommitteeType.GermanMinimalThreshold;
                frenchUnderStaffedNew = activeMemberCountNew > 0 && compareCommittee.Memberships.Count(m => m.Person!.LanguageId == Language.FrenchGuid) < currentCommitteeType.FrenchMinimalThreshold;
                italianUnderStaffedNew = activeMemberCountNew > 0 && compareCommittee.Memberships.Count(m => m.Person!.LanguageId == Language.ItalianGuid) < currentCommitteeType.ItalianMinimalThreshold;
                germanTextNew = germanUnderStaffedNew ? $"DE: {BusinessTexts.Compare_List_Language_Missing}" : BusinessTexts.Compare_List_Language_OK;
                frenchTextNew = frenchUnderStaffedNew ? $"FR: {BusinessTexts.Compare_List_Language_Missing}" : BusinessTexts.Compare_List_Language_OK;
                italianTextNew = italianUnderStaffedNew ? $"IT: {BusinessTexts.Compare_List_Language_Missing}" : BusinessTexts.Compare_List_Language_OK;
            }
        }
        else
        {
            // we work with minimal percentage of members
            germanUnderStaffedOld = activeMemberCountOld > 0 && (double)committee.Memberships.Count(m => m.Person!.LanguageId == Language.GermanGuid) / activeMemberCountOld * 100 < currentCommitteeType!.GermanThresholdPercentage;
            frenchUnderStaffedOld = activeMemberCountOld > 0 && (double)committee.Memberships.Count(m => m.Person!.LanguageId == Language.FrenchGuid) / activeMemberCountOld * 100 < currentCommitteeType!.FrenchThresholdPercentage;
            italianUnderStaffedOld = activeMemberCountOld > 0 && (double)committee.Memberships.Count(m => m.Person!.LanguageId == Language.ItalianGuid) / activeMemberCountOld * 100 < currentCommitteeType!.ItalianThresholdPercentage;
            germanTextOld = $"DE: {(double)committee.Memberships.Count(m => m.Person!.LanguageId == Language.GermanGuid) / activeMemberCountOld * 100:F2} %";
            frenchTextOld = $"FR: {(double)committee.Memberships.Count(m => m.Person!.LanguageId == Language.FrenchGuid) / activeMemberCountOld * 100:F2} %";
            italianTextOld = $"IT: {(double)committee.Memberships.Count(m => m.Person!.LanguageId == Language.ItalianGuid) / activeMemberCountOld * 100:F2} %";

            if (compareCommittee != null)
            {
                germanUnderStaffedNew = activeMemberCountNew > 0 && (double)compareCommittee.Memberships.Count(m => m.Person!.LanguageId == Language.GermanGuid) / activeMemberCountNew * 100 < currentCommitteeType!.GermanThresholdPercentage;
                frenchUnderStaffedNew = activeMemberCountNew > 0 && (double)compareCommittee.Memberships.Count(m => m.Person!.LanguageId == Language.FrenchGuid) / activeMemberCountNew * 100 < currentCommitteeType!.FrenchThresholdPercentage;
                italianUnderStaffedNew = activeMemberCountNew > 0 && (double)compareCommittee.Memberships.Count(m => m.Person!.LanguageId == Language.ItalianGuid) / activeMemberCountNew * 100 < currentCommitteeType!.ItalianThresholdPercentage;
                germanTextNew = $"DE: {(double)compareCommittee.Memberships.Count(m => m.Person!.LanguageId == Language.GermanGuid) / activeMemberCountNew * 100:F2} %";
                frenchTextNew = $"FR: {(double)compareCommittee.Memberships.Count(m => m.Person!.LanguageId == Language.FrenchGuid) / activeMemberCountNew * 100:F2} %";
                italianTextNew = $"IT: {(double)compareCommittee.Memberships.Count(m => m.Person!.LanguageId == Language.ItalianGuid) / activeMemberCountNew * 100:F2} %";
            }
        }

        if (germanUnderStaffedOld || frenchUnderStaffedOld || italianUnderStaffedOld || femaleUnderStaffedOld || maleUnderStaffedOld ||
            committee.Memberships.Any(m => m.Person!.FederalAssembly) || committee.Memberships.Any(m => m.Person!.FederalDuty))
        {
            var compareListCommittee = new CompareListCommitteeDto()
            {
                Id = committee.Id,
                CommitteeNumber = committee.CommitteeNumber,
                Name = committee.GetDescription(),
                DepartmentId = committee.DepartmentId,
                CommitteeTypeId = committee.CommitteeTypeId,
                MemberCountOld = committee.Memberships.Count,
                MemberCountNew = compareCommittee is not null ? compareCommittee.Memberships.Count : 0,
                FederalDutyBothDisplay = committee.Memberships.Any(m => m.Person!.FederalDuty) && compareCommittee is not null && compareCommittee.Memberships.Any(m => m.Person!.FederalDuty),
                FederalDutyOldDisplay = committee.Memberships.Any(m => m.Person!.FederalDuty) && compareCommittee is not null && !compareCommittee.Memberships.Any(m => m.Person!.FederalDuty),
                FederalDutyNewDisplay = !committee.Memberships.Any(m => m.Person!.FederalDuty) && compareCommittee is not null && compareCommittee.Memberships.Any(m => m.Person!.FederalDuty),
                FederalDutyCountOld = committee.Memberships.Count(m => m.Person!.FederalDuty),
                FederalDutyCountNew = compareCommittee is not null ? compareCommittee.Memberships.Count(m => m.Person!.FederalDuty) : 0,
                FederalAssemblyBothDisplay = committee.Memberships.Any(m => m.Person!.FederalAssembly) && compareCommittee is not null && compareCommittee.Memberships.Any(m => m.Person!.FederalAssembly),
                FederalAssemblyOldDisplay = committee.Memberships.Any(m => m.Person!.FederalAssembly) && compareCommittee is not null && !compareCommittee.Memberships.Any(m => m.Person!.FederalAssembly),
                FederalAssemblyNewDisplay = !committee.Memberships.Any(m => m.Person!.FederalAssembly) && compareCommittee is not null && compareCommittee.Memberships.Any(m => m.Person!.FederalAssembly),
                FederalAssemblyCountOld = committee.Memberships.Count(m => m.Person!.FederalAssembly),
                FederalAssemblyCountNew = compareCommittee is not null ? compareCommittee.Memberships.Count(m => m.Person!.FederalAssembly) : 0,
                GermanBothDisplay = germanUnderStaffedOld && germanUnderStaffedNew,
                GermanOldDisplay = germanUnderStaffedOld && !germanUnderStaffedNew,
                GermanNewDisplay = !germanUnderStaffedOld && germanUnderStaffedNew,
                FrenchBothDisplay = frenchUnderStaffedOld && frenchUnderStaffedNew,
                FrenchOldDisplay = frenchUnderStaffedOld && !frenchUnderStaffedNew,
                FrenchNewDisplay = !frenchUnderStaffedOld && frenchUnderStaffedNew,
                ItalianBothDisplay = italianUnderStaffedOld && italianUnderStaffedNew,
                ItalianOldDisplay = italianUnderStaffedOld && !italianUnderStaffedNew,
                ItalianNewDisplay = !italianUnderStaffedOld && italianUnderStaffedNew,
                GermanTextOld = germanTextOld,
                GermanTextNew = germanTextNew,
                FrenchTextOld = frenchTextOld,
                FrenchTextNew = frenchTextNew,
                ItalianTextOld = italianTextOld,
                ItalianTextNew = italianTextNew,
                GenderBothDisplay = (femaleUnderStaffedOld || maleUnderStaffedOld) && (femaleUnderStaffedNew || maleUnderStaffedNew),
                GenderOldDisplay = (femaleUnderStaffedOld || maleUnderStaffedOld) && !femaleUnderStaffedNew && !maleUnderStaffedNew,
                GenderNewDisplay = !femaleUnderStaffedOld && !maleUnderStaffedOld && (femaleUnderStaffedNew || maleUnderStaffedNew),
                GenderTextOld = string.Join(", ", genderPartsOld),
                GenderTextNew = string.Join(", ", genderPartsNew)
            };
            return compareListCommittee;
        }
        return null;
    }

    private static string CreateGenderPercentageText(int totalMembers, int membershipCount, Guid genderId)
    {
        var genderText = membershipCount == 1 ? SingleText() : MultiText();

        return $"{membershipCount} {genderText} ({(double)membershipCount / totalMembers * 100:F2} %)";

        string SingleText() => genderId == Gender.FemaleGuid ? BusinessTexts.Compare_List_Woman : BusinessTexts.Compare_List_Man;
        string MultiText() => genderId == Gender.FemaleGuid ? BusinessTexts.Compare_List_Women : BusinessTexts.Compare_List_Men;
    }
}
