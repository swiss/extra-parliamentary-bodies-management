using System.Globalization;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Common.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swiss.FCh.DocumentService.Client.Models;

namespace Bk.APG.Business.Services;

public class DataAnalysisService : IDataAnalysisService
{
    private readonly Swiss.FCh.DocumentService.Client.IDocumentService _documentService;
    private readonly IEiamAssignmentService _eiamAssignmentService;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly ILogger<DataAnalysisService> _logger;
    private readonly IConfiguration _configuration;

    public DataAnalysisService(
        Swiss.FCh.DocumentService.Client.IDocumentService documentService,
        IEiamAssignmentService eiamAssignmentService,
        ICommitteeRepository committeeRepository,
        IPersonRepository personRepository,
        IMasterDataRepository masterDataRepository,
        ILogger<DataAnalysisService> logger,
        IConfiguration configuration)
    {
        _documentService = documentService;
        _eiamAssignmentService = eiamAssignmentService;
        _committeeRepository = committeeRepository;
        _personRepository = personRepository;
        _masterDataRepository = masterDataRepository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<(string fileName, Stream content)> GenerateCommitteeTypeExport(DateOnly dataAnalysisDate)
    {
        _logger.LogInformation("Generate committee type export for date {DataAnalysisDate}", dataAnalysisDate);

        string[] headers =
        [
            BusinessTexts.DataAnalysis_CommitteeType,
            BusinessTexts.DataAnalysis_ActiveCommitteeCount,
            BusinessTexts.DataAnalysis_ActiveMemberCount,
            BusinessTexts.DataAnalysis_FemalePresidentCount,
            BusinessTexts.DataAnalysis_FemalePresidentPercentage,
            BusinessTexts.DataAnalysis_MalePresidentCount,
            BusinessTexts.DataAnalysis_MalePresidentPercentage,
            BusinessTexts.DataAnalysis_WomanCount,
            BusinessTexts.DataAnalysis_WomanPercentage,
            BusinessTexts.DataAnalysis_ManCount,
            BusinessTexts.DataAnalysis_ManPercentage,
            BusinessTexts.DataAnalysis_ItalianCount,
            BusinessTexts.DataAnalysis_ItalianPercentage,
            BusinessTexts.DataAnalysis_GermanCount,
            BusinessTexts.DataAnalysis_GermanPercentage,
            BusinessTexts.DataAnalysis_FrenchCount,
            BusinessTexts.DataAnalysis_FrenchPercentage,
            BusinessTexts.DataAnalysis_RomanshCount,
            BusinessTexts.DataAnalysis_RomanshPercentage,
            BusinessTexts.DataAnalysis_FederalDutyCount,
            BusinessTexts.DataAnalysis_FederalDutyPercentage,
            BusinessTexts.DataAnalysis_NotFederalDutyCount,
            BusinessTexts.DataAnalysis_NotFederalDutyPercentage,
            BusinessTexts.DataAnalysis_CentralFederalAdministrationDutyCount,
            BusinessTexts.DataAnalysis_CentralFederalAdministrationDutyPercentage,
            BusinessTexts.DataAnalysis_DecentralizedFederalAdministrationDutyCount,
            BusinessTexts.DataAnalysis_DecentralizedFederalAdministrationDutyPercentage,
            BusinessTexts.DataAnalysis_FederalAssemblyCount,
            BusinessTexts.DataAnalysis_FederalAssemblyPercentage,
            BusinessTexts.DataAnalysis_NotFederalAssemblyCount,
            BusinessTexts.DataAnalysis_NotFederalAssemblyPercentage
        ];

        var bodyCells = await GetCommitteeTypeData(dataAnalysisDate);
        var spreadsheet = new Spreadsheet
        {
            HeaderCells = headers.Select(header => new Cell { Text = header, Format = CellFormat.Bold, BackgroundColor = "#FFA500", ForegroundColor = "#FFFFFF" }).ToList(),
            BodyCells = bodyCells,
            TableOptions = new TableOptions
            {
                CreateTable = true,
                ShowTotalsRow = true,
                TableRange = $"A1:AE{bodyCells.Count + 1}",
                TotalCells =
                [
                    new TotalCell { Field = BusinessTexts.DataAnalysis_CommitteeType, Label = BusinessTexts.DataAnalysis_Total },
                    TotalSumCell(BusinessTexts.DataAnalysis_ActiveCommitteeCount),
                    TotalSumCell(BusinessTexts.DataAnalysis_ActiveMemberCount),
                    TotalSumCell(BusinessTexts.DataAnalysis_FemalePresidentCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_FemalePresidentPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_MalePresidentCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_MalePresidentPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_WomanCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_WomanPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_ManCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_ManPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_ItalianCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_ItalianPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_GermanCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_GermanPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_FrenchCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_FrenchPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_RomanshCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_RomanshPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_FederalDutyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_FederalDutyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_NotFederalDutyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_NotFederalDutyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_CentralFederalAdministrationDutyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_CentralFederalAdministrationDutyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_DecentralizedFederalAdministrationDutyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_DecentralizedFederalAdministrationDutyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_FederalAssemblyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_FederalAssemblyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_NotFederalAssemblyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_NotFederalAssemblyPercentage)
                ]
            }
        };

        var exportStream = await _documentService.CreateExcel(spreadsheet);

        return (GenerateFileName(dataAnalysisDate, BusinessTexts.DataAnalysis_CommitteeType), exportStream);
    }

    private async Task<IList<IList<Cell>>> GetCommitteeTypeData(DateOnly dataAnalysisDate)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var committees = await _committeeRepository.GetCommitteesForExport(dataAnalysisDate, departmentId, officeId, committeeId);

        var bodyCells = committees
            .OrderBy(x => x.CommitteeType!.GetText())
            .GroupBy(x => x.CommitteeTypeId)
            .Select(group => new List<Cell>
            {
                new() { Text = group.First().CommitteeType!.GetText() }, // Gremiumart
                NumberCell(group.Count()), // Total aktive Gremien
                NumberCell(group.Sum(x => x.Memberships.Count)), // Total aktive Mitglieder
                NumberCell(group.Sum(x => x.FemalePresidentCount)), // # Präsidentinnen
                PercentageCell(Divide(group.Sum(x => x.FemalePresidentCount), group.Sum(x => x.PresidentCount))), // % Präsidentinnen
                NumberCell(group.Sum(x => x.MalePresidentCount)), // # Präsidenten
                PercentageCell(Divide(group.Sum(x => x.MalePresidentCount), group.Sum(x => x.PresidentCount))), // % Präsidenten
                NumberCell(group.Sum(x => x.FemaleCount)), // # Frauen
                PercentageCell(Divide(group.Sum(x => x.FemaleCount), group.Sum(x => x.ActiveMemberCount))), // % Frauen
                NumberCell(group.Sum(x => x.MaleCount)), // # Männer
                PercentageCell(Divide(group.Sum(x => x.MaleCount), group.Sum(x => x.ActiveMemberCount))), // % Männer
                NumberCell(group.Sum(x => x.ItalianCount)), // # Italienisch
                PercentageCell(Divide(group.Sum(x => x.ItalianCount), group.Sum(x => x.ActiveMemberCount))), // % Italienisch
                NumberCell(group.Sum(x => x.GermanCount)), // # Deutsch
                PercentageCell(Divide(group.Sum(x => x.GermanCount), group.Sum(x => x.ActiveMemberCount))), // % Deutsch
                NumberCell(group.Sum(x => x.FrenchCount)), // # Französisch
                PercentageCell(Divide(group.Sum(x => x.FrenchCount), group.Sum(x => x.ActiveMemberCount))), // % Französisch
                NumberCell(group.Sum(x => x.RomanshCount)), // # Rätoromanisch
                PercentageCell(Divide(group.Sum(x => x.RomanshCount), group.Sum(x => x.ActiveMemberCount))), // % Rätoromanisch
                NumberCell(group.Sum(x => x.FederalDutyCount)), // # Im Bundesdienst
                PercentageCell(Divide(group.Sum(x => x.FederalDutyCount), group.Sum(x => x.ActiveMemberCount))), // % Im Bundesdienst
                NumberCell(group.Sum(x => x.NotFederalDutyCount)), // # Nicht im Bundesdienst
                PercentageCell(Divide(group.Sum(x => x.NotFederalDutyCount), group.Sum(x => x.ActiveMemberCount))), // % Nicht im Bundesdienst
                NumberCell(group.Sum(x => x.IsCentralFederalAdministrationCount)), // # Im zentralen Bundesdienst
                PercentageCell(Divide(group.Sum(x => x.IsCentralFederalAdministrationCount), group.Sum(x => x.ActiveMemberCount))), // % Im zentralen Bundesdienst
                NumberCell(group.Sum(x => x.IsDecentralizedFederalAdministrationCount)), // # Im dezentralen Bundesdienst
                PercentageCell(Divide(group.Sum(x => x.IsDecentralizedFederalAdministrationCount), group.Sum(x => x.ActiveMemberCount))), // % Im dezentralen Bundesdienst
                NumberCell(group.Sum(x => x.FederalAssemblyCount)), // # Mitglieder der Bundesversammlung
                PercentageCell(Divide(group.Sum(x => x.FederalAssemblyCount), group.Sum(x => x.ActiveMemberCount))), // % Mitglieder der Bundesversammlung
                NumberCell(group.Sum(x => x.NotFederalAssemblyCount)), // # Nicht Mitglieder der Bundesversammlung
                PercentageCell(Divide(group.Sum(x => x.NotFederalAssemblyCount), group.Sum(x => x.ActiveMemberCount))) // % Nicht Mitglieder der Bundesversammlung
            } as IList<Cell>).ToList();

        return bodyCells;
    }

    public async Task<(string fileName, Stream content)> GenerateCommitteeExport(DateOnly dataAnalysisDate)
    {
        _logger.LogInformation("Generate committee export for date {DataAnalysisDate}", dataAnalysisDate);

        string[] headers =
        [
            BusinessTexts.DataAnalysis_CommitteeName,
            BusinessTexts.DataAnalysis_CommitteeType,
            BusinessTexts.DataAnalysis_Department,
            BusinessTexts.DataAnalysis_Office,
            BusinessTexts.DataAnalysis_Level,
            BusinessTexts.DataAnalysis_ExtraParliamentaryCommission,
            BusinessTexts.DataAnalysis_SupervisionDuty,
            BusinessTexts.DataAnalysis_TermOfOffice,
            BusinessTexts.DataAnalysis_BeginDate,
            BusinessTexts.DataAnalysis_EndDate,
            BusinessTexts.DataAnalysis_ActiveMemberCount,
            BusinessTexts.DataAnalysis_FemalePresidentCount,
            BusinessTexts.DataAnalysis_FemalePresidentPercentage,
            BusinessTexts.DataAnalysis_MalePresidentCount,
            BusinessTexts.DataAnalysis_MalePresidentPercentage,
            BusinessTexts.DataAnalysis_WomanCount,
            BusinessTexts.DataAnalysis_WomanPercentage,
            BusinessTexts.DataAnalysis_ManCount,
            BusinessTexts.DataAnalysis_ManPercentage,
            BusinessTexts.DataAnalysis_ItalianCount,
            BusinessTexts.DataAnalysis_ItalianPercentage,
            BusinessTexts.DataAnalysis_GermanCount,
            BusinessTexts.DataAnalysis_GermanPercentage,
            BusinessTexts.DataAnalysis_FrenchCount,
            BusinessTexts.DataAnalysis_FrenchPercentage,
            BusinessTexts.DataAnalysis_RomanshCount,
            BusinessTexts.DataAnalysis_RomanshPercentage,
            BusinessTexts.DataAnalysis_FederalDutyCount,
            BusinessTexts.DataAnalysis_FederalDutyPercentage,
            BusinessTexts.DataAnalysis_NotFederalDutyCount,
            BusinessTexts.DataAnalysis_NotFederalDutyPercentage,
            BusinessTexts.DataAnalysis_CentralFederalAdministrationDutyCount,
            BusinessTexts.DataAnalysis_CentralFederalAdministrationDutyPercentage,
            BusinessTexts.DataAnalysis_DecentralizedFederalAdministrationDutyCount,
            BusinessTexts.DataAnalysis_DecentralizedFederalAdministrationDutyPercentage,
            BusinessTexts.DataAnalysis_FederalAssemblyCount,
            BusinessTexts.DataAnalysis_FederalAssemblyPercentage,
            BusinessTexts.DataAnalysis_NotFederalAssemblyCount,
            BusinessTexts.DataAnalysis_NotFederalAssemblyPercentage
        ];

        var bodyCells = await GetCommitteeData(dataAnalysisDate);
        var spreadsheet = new Spreadsheet
        {
            HeaderCells = headers.Select(header => new Cell { Text = header, Format = CellFormat.Bold, BackgroundColor = "#FF2D7BCB", ForegroundColor = "#FFFFFF" }).ToList(),
            BodyCells = bodyCells,
            TableOptions = new TableOptions
            {
                CreateTable = true,
                ShowTotalsRow = true,
                TableRange = $"A1:AM{bodyCells.Count + 1}",
                TotalCells =
                [
                    new TotalCell { Field = BusinessTexts.DataAnalysis_CommitteeName, Label = BusinessTexts.DataAnalysis_Total },
                    TotalSumCell(BusinessTexts.DataAnalysis_ActiveMemberCount),
                    TotalSumCell(BusinessTexts.DataAnalysis_FemalePresidentCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_FemalePresidentPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_MalePresidentCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_MalePresidentPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_WomanCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_WomanPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_ManCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_ManPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_ItalianCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_ItalianPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_GermanCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_GermanPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_FrenchCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_FrenchPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_RomanshCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_RomanshPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_FederalDutyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_FederalDutyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_NotFederalDutyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_NotFederalDutyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_CentralFederalAdministrationDutyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_CentralFederalAdministrationDutyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_DecentralizedFederalAdministrationDutyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_DecentralizedFederalAdministrationDutyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_FederalAssemblyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_FederalAssemblyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_NotFederalAssemblyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_NotFederalAssemblyPercentage)
                ]
            }
        };

        var exportStream = await _documentService.CreateExcel(spreadsheet, SpreadsheetOptions.Default);

        return (GenerateFileName(dataAnalysisDate, BusinessTexts.DataAnalysis_Committee), exportStream);
    }

    private async Task<IList<IList<Cell>>> GetCommitteeData(DateOnly dataAnalysisDate)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var committees = await _committeeRepository.GetCommitteesForExport(dataAnalysisDate, departmentId, officeId, committeeId);

        var bodyCells = committees
            .OrderBy(x => x.GetDescription())
            .Select(committee => new List<Cell>
            {
                HyperlinkCell(committee.GetDescription(), $"committees/{committee.Id}"),
                new() { Text = committee.CommitteeType?.GetText() },
                new() { Text = committee.Department?.GetText() },
                new() { Text = committee.Office?.GetText() },
                new() { Text = committee.CommitteeLevel?.GetText() },
                new() { Text = committee.ExtraParliamentaryCommission ? BusinessTexts.Common_Yes : BusinessTexts.Common_No },
                new() { Text = committee.SupervisionDuty.GetValueOrDefault() ? BusinessTexts.Common_Yes : BusinessTexts.Common_No },
                new() { Text = committee.TermOfOffice?.GetText() },
                DateCell(committee.BeginDate),
                DateCell(committee.EndDate),
                NumberCell(committee.Memberships.Count), // Total aktive Mitglieder
                NumberCell(committee.FemalePresidentCount), // # Präsidentinnen
                PercentageCell(Divide(committee.FemalePresidentCount, committee.PresidentCount)), // % Präsidentinnen
                NumberCell(committee.MalePresidentCount), // # Präsidenten
                PercentageCell(Divide(committee.MalePresidentCount, committee.PresidentCount)), // % Präsidenten
                NumberCell(committee.FemaleCount), // # Frauen
                PercentageCell(Divide(committee.FemaleCount, committee.ActiveMemberCount)), // % Frauen
                NumberCell(committee.MaleCount), // # Männer
                PercentageCell(Divide(committee.MaleCount, committee.ActiveMemberCount)), // % Männer
                NumberCell(committee.ItalianCount), // # Italienisch
                PercentageCell(Divide(committee.ItalianCount, committee.ActiveMemberCount)), // % Italienisch
                NumberCell(committee.GermanCount), // # Deutsch
                PercentageCell(Divide(committee.GermanCount, committee.ActiveMemberCount)), // % Deutsch
                NumberCell(committee.FrenchCount), // # Französisch
                PercentageCell(Divide(committee.FrenchCount, committee.ActiveMemberCount)), // % Französisch
                NumberCell(committee.RomanshCount), // # Rätoromanisch
                PercentageCell(Divide(committee.RomanshCount, committee.ActiveMemberCount)), // % Rätoromanisch
                NumberCell(committee.FederalDutyCount), // # Im Bundesdienst
                PercentageCell(Divide(committee.FederalDutyCount, committee.ActiveMemberCount)), // % Im Bundesdienst
                NumberCell(committee.NotFederalDutyCount), // # Nicht im Bundesdienst
                PercentageCell(Divide(committee.NotFederalDutyCount, committee.ActiveMemberCount)), // % Nicht im Bundesdienst
                NumberCell(committee.IsCentralFederalAdministrationCount), // # Im zentralen Bundesdienst
                PercentageCell(Divide(committee.IsCentralFederalAdministrationCount, committee.ActiveMemberCount)), // % Im zentralen Bundesdienst
                NumberCell(committee.IsDecentralizedFederalAdministrationCount), // # Im dezentralen Bundesdienst
                PercentageCell(Divide(committee.IsDecentralizedFederalAdministrationCount, committee.ActiveMemberCount)), // % Im dezentralen Bundesdienst
                NumberCell(committee.FederalAssemblyCount), // # Mitglieder der Bundesversammlung
                PercentageCell(Divide(committee.FederalAssemblyCount, committee.ActiveMemberCount)), // % Mitglieder der Bundesversammlung
                NumberCell(committee.NotFederalAssemblyCount), // # Nicht Mitglieder der Bundesversammlung
                PercentageCell(Divide(committee.NotFederalAssemblyCount, committee.ActiveMemberCount)) // % Nicht Mitglieder der Bundesversammlung
            } as IList<Cell>).ToList();

        return bodyCells;
    }

    public async Task<(string fileName, Stream content)> GenerateMembershipExport(DateOnly dataAnalysisDate)
    {
        _logger.LogInformation("Generate membership export for date {DataAnalysisDate}", dataAnalysisDate);

        string[] headers =
        [
            BusinessTexts.DataAnalysis_CommitteeName,
            // Person
            BusinessTexts.DataAnalysis_LastName,
            BusinessTexts.DataAnalysis_FirstName,
            BusinessTexts.DataAnalysis_City,
            BusinessTexts.DataAnalysis_Canton,
            BusinessTexts.DataAnalysis_BirthYear,
            BusinessTexts.DataAnalysis_Age,
            BusinessTexts.DataAnalysis_Occupation,
            BusinessTexts.DataAnalysis_Language,
            BusinessTexts.DataAnalysis_Gender,
            BusinessTexts.DataAnalysis_FederalDuty,
            BusinessTexts.DataAnalysis_CentralFederalDuty,
            BusinessTexts.DataAnalysis_FederalAssembly,
            // Mitgliedschaft
            BusinessTexts.DataAnalysis_Function,
            BusinessTexts.DataAnalysis_From,
            BusinessTexts.DataAnalysis_To,
            BusinessTexts.DataAnalysis_Status,
            BusinessTexts.DataAnalysis_ElectionOffice,
            BusinessTexts.DataAnalysis_EmploymentLevel,
            BusinessTexts.DataAnalysis_Duty,
            BusinessTexts.DataAnalysis_MemberAdditions,
            // Gremium
            BusinessTexts.DataAnalysis_CommitteeType,
            BusinessTexts.DataAnalysis_Department,
            BusinessTexts.DataAnalysis_Office,
            BusinessTexts.DataAnalysis_Level,
            BusinessTexts.DataAnalysis_ExtraParliamentaryCommission,
            BusinessTexts.DataAnalysis_SupervisionDuty,
            BusinessTexts.DataAnalysis_TermOfOffice,
            BusinessTexts.DataAnalysis_BeginDate,
            BusinessTexts.DataAnalysis_EndDate,
            BusinessTexts.DataAnalysis_ActiveMemberCount
        ];

        var bodyCells = await GetMembershipData(dataAnalysisDate);
        var spreadsheet = new Spreadsheet
        {
            HeaderCells = headers.Select(header => new Cell { Text = header, Format = CellFormat.Bold, BackgroundColor = "#FF099900", ForegroundColor = "#FFFFFF" }).ToList(),
            BodyCells = bodyCells,
            TableOptions = new TableOptions
            {
                CreateTable = true,
                ShowTotalsRow = true,
                TableRange = $"A1:AE{bodyCells.Count + 1}",
                TotalCells =
                [
                    new TotalCell { Field = BusinessTexts.DataAnalysis_CommitteeName, Label = BusinessTexts.DataAnalysis_Total },
                    new TotalCell { Field = BusinessTexts.DataAnalysis_LastName, Function = TotalCellFunctions.Count }
                ]
            }
        };

        var exportStream = await _documentService.CreateExcel(spreadsheet, SpreadsheetOptions.Default);

        return (GenerateFileName(dataAnalysisDate, BusinessTexts.DataAnalysis_Membership), exportStream);
    }

    private async Task<IList<IList<Cell>>> GetMembershipData(DateOnly dataAnalysisDate)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var committees = await _committeeRepository.GetCommitteesForExport(dataAnalysisDate, departmentId, officeId, committeeId);

        var memberships = committees
            .SelectMany(x => x.Memberships)
            .Where(x => x is { IsActive: true, IsDeleted: false });

        var bodyCells = memberships
            .OrderBy(x => x.Committee!.GetDescription())
            .Select(membership => new List<Cell>
            {
                new() { Text = membership.Committee!.GetDescription() },
                // Person
                HyperlinkCell(membership.Person!.Surname, $"persons/{membership.PersonId}"),
                new() { Text = membership.Person!.GivenName },
                new() { Text = membership.Person!.CorrespondenceAddress?.City },
                new() { Text = membership.Person!.CorrespondenceAddress?.Canton?.GetText() },
                NumberCell(membership.Person!.BirthYear),
                NumberCell(membership.Person!.Age),
                new() { Text = membership.Person!.Occupation },
                new() { Text = membership.Person!.Language?.GetText() },
                new() { Text = membership.Person!.Gender?.GetText() },
                new() { Text = membership.Person!.FederalDuty ? BusinessTexts.Common_Yes : BusinessTexts.Common_No },
                new()
                {
                    Text = membership.Person!.Office is not null && membership.Person!.Office!.IsCentralFederalAdministration ? BusinessTexts.DataAnalysis_FederalDuty_Central :
                        membership.Person!.Office is not null && !membership.Person!.Office!.IsCentralFederalAdministration ? BusinessTexts.DataAnalysis_FederalDuty_Decentralized : string.Empty
                },
                new() { Text = membership.Person!.FederalAssembly ? BusinessTexts.Common_Yes : BusinessTexts.Common_No },
                // Mitgliedschaft
                new() { Text = membership.Function?.GetText() },
                DateCell(membership.BeginDate),
                DateCell(membership.EndDate),
                new() { Text = membership.ElectionType?.GetText() },
                new() { Text = membership.ElectionOffice?.GetText() },
                NumberCell(membership.MaximumEmploymentLevel.GetValueOrDefault()),
                NumberCell(MembershipTermCalculator.CalculateCurrentTermInYears([membership]), "#,0"),
                new() { Text = membership.MembershipAddition?.GetText() },
                // Gremium
                new() { Text = membership.Committee!.CommitteeType?.GetText() },
                new() { Text = membership.Committee!.Department?.GetText() },
                new() { Text = membership.Committee!.Office?.GetText() },
                new() { Text = membership.Committee!.CommitteeLevel?.GetText() },
                new() { Text = membership.Committee!.ExtraParliamentaryCommission ? BusinessTexts.Common_Yes : BusinessTexts.Common_No },
                new() { Text = membership.Committee!.SupervisionDuty.GetValueOrDefault() ? BusinessTexts.Common_Yes : BusinessTexts.Common_No },
                new() { Text = membership.Committee!.TermOfOffice?.GetText() },
                DateCell(membership.Committee!.BeginDate),
                DateCell(membership.Committee!.EndDate),
                NumberCell(membership.Committee!.Memberships.Count)
            } as IList<Cell>).ToList();

        return bodyCells;
    }

    public async Task<(string fileName, Stream content)> GenerateMembershipInterestExport(DateOnly dataAnalysisDate)
    {
        _logger.LogInformation("Generate membership export with interests for date {DataAnalysisDate}", dataAnalysisDate);

        string[] headers =
        [
            BusinessTexts.DataAnalysis_CommitteeName,
            // Person
            BusinessTexts.DataAnalysis_LastName,
            BusinessTexts.DataAnalysis_FirstName,
            BusinessTexts.DataAnalysis_BirthYear,
            BusinessTexts.DataAnalysis_InterestText,
            BusinessTexts.DataAnalysis_LegalForm,
            BusinessTexts.DataAnalysis_InterestLegalForm,
            BusinessTexts.DataAnalysis_InterestOrganization,
            BusinessTexts.DataAnalysis_InterestFunction,
            // Gremium
            BusinessTexts.DataAnalysis_CommitteeType,
            BusinessTexts.DataAnalysis_Department,
            BusinessTexts.DataAnalysis_Office,
            BusinessTexts.DataAnalysis_Level,
            BusinessTexts.DataAnalysis_ExtraParliamentaryCommission,
            BusinessTexts.DataAnalysis_SupervisionDuty,
            BusinessTexts.DataAnalysis_TermOfOffice,
            BusinessTexts.DataAnalysis_BeginDate,
            BusinessTexts.DataAnalysis_EndDate,
        ];

        var spreadsheet = new Spreadsheet
        {
            HeaderCells = headers.Select(header => new Cell { Text = header, Format = CellFormat.Bold, BackgroundColor = "#6495ED", ForegroundColor = "#FFFFFF" }).ToList(),
            BodyCells = await GetMembershipWithInterestData(dataAnalysisDate)
        };

        var exportStream = await _documentService.CreateExcel(spreadsheet, SpreadsheetOptions.Default);

        return (GenerateFileName(dataAnalysisDate, BusinessTexts.DataAnalysis_MembershipInterests), exportStream);
    }

    public async Task<(string fileName, Stream content)> GenerateContactPointExport(DateOnly dataAnalysisDate, Guid contactPointTypeId)
    {
        var backgroundColor = "#52c5ce";
        var foregroundColor = "#FFFFFF";
        var contactPointType = "DataProtectionOfficer";
        var fileName = BusinessTexts.DataAnalysis_ContactPointDataProtectionOfficer;

        if (contactPointTypeId == ContactPointType.SecretariatGuid)
        {
            backgroundColor = "#31777d";
            foregroundColor = "#FFFFFF";
            contactPointType = "Secretariat";
            fileName = BusinessTexts.DataAnalysis_ContactPointSecretariat;
        }

        _logger.LogInformation("Generate contact point export ({ContactPointType}) for date {DataAnalysisDate}", dataAnalysisDate, contactPointType);

        string[] headers =
        [
            BusinessTexts.DataAnalysis_CommitteeName,
            // Kontaktstellen
            BusinessTexts.DataAnalysis_CompanyName,
            BusinessTexts.DataAnalysis_CompanyPhone,
            BusinessTexts.DataAnalysis_TitleSurnameGivenName,
            BusinessTexts.DataAnalysis_PersonalPhoneMobile,
            BusinessTexts.DataAnalysis_EmailAddresses,
            // Gremium
            BusinessTexts.DataAnalysis_CommitteeType,
            BusinessTexts.DataAnalysis_Department,
            BusinessTexts.DataAnalysis_Office,
            BusinessTexts.DataAnalysis_Level,
            BusinessTexts.DataAnalysis_ExtraParliamentaryCommission,
            BusinessTexts.DataAnalysis_SupervisionDuty,
            BusinessTexts.DataAnalysis_TermOfOffice,
        ];

        var spreadsheet = new Spreadsheet
        {
            HeaderCells = headers.Select(header => new Cell { Text = header, Format = CellFormat.Bold, BackgroundColor = backgroundColor, ForegroundColor = foregroundColor }).ToList(),
            BodyCells = await GetContactPointData(dataAnalysisDate, contactPointTypeId)
        };

        var exportStream = await _documentService.CreateExcel(spreadsheet, SpreadsheetOptions.Default);

        return (GenerateFileName(dataAnalysisDate, fileName), exportStream);
    }

    private async Task<IList<IList<Cell>>> GetContactPointData(DateOnly dataAnalysisDate, Guid contactPointTypeId)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var committees = await _committeeRepository.GetCommitteesWithContactPointsForExport(dataAnalysisDate, departmentId, officeId, committeeId);

        // only show personal data, when these are released (ReleasePersonData)! Customer wants several fields concatenated, for space reason.
        var contactPointList = committees
            .SelectMany(committee => committee.ContactPoints.Where(cp => cp.ContactPointTypeId == contactPointTypeId && cp.BeginDate <= dataAnalysisDate && (cp.EndDate is null || cp.EndDate >= dataAnalysisDate)).DefaultIfEmpty(),
                (committee, contactPoint) => new
                {
                    CommitteeName = committee.GetDescription(),
                    CompanyName = contactPoint is not null ? string.Join(", ", new[] { contactPoint.CompanyName, contactPoint.Section }.Where(s => !string.IsNullOrWhiteSpace(s))) : string.Empty,
                    Phone = contactPoint is not null ? contactPoint.Phone : string.Empty,
                    TitleName = contactPoint is not null && contactPoint.ReleasePersonData ? string.Join(" ", new[] { contactPoint.Title, contactPoint.Surname, contactPoint.GivenName }.Where(s => !string.IsNullOrWhiteSpace(s))) : string.Empty,
                    PersonalPhoneMobile = contactPoint is not null && contactPoint.ReleasePersonData ? string.Join(" / ", new[] { contactPoint.PersonalPhone, contactPoint.PersonalMobile }.Where(s => !string.IsNullOrWhiteSpace(s))) : string.Empty,
                    Emails = contactPoint is not null && contactPoint.ReleasePersonData ? string.Join(";", new[] { contactPoint.Email, contactPoint.PersonalEmail }.Where(s => !string.IsNullOrWhiteSpace(s))) : !string.IsNullOrWhiteSpace(contactPoint?.Email) ? contactPoint.Email : string.Empty,
                    CommitteeType = committee.CommitteeType!.GetText(),
                    Department = committee.Department!.GetDescription(),
                    Office = committee.Office!.GetDescription(),
                    Level = committee.CommitteeLevel!.GetText(),
                    ExtraParliamentaryCommission = committee.ExtraParliamentaryCommission ? BusinessTexts.Common_Yes : BusinessTexts.Common_No,
                    SuperVisionDuty = committee.SupervisionDuty == true ? BusinessTexts.Common_Yes : BusinessTexts.Common_No,
                    TermOfOffice = committee.TermOfOffice!.GetDescription(),
                }
            ).ToList();

        var bodyCells = contactPointList
            .OrderBy(x => x.CommitteeName)
            .ThenBy(x => x.CompanyName)
            .ThenBy(x => x.TitleName)
            .Select(contactPoint => new List<Cell>
            {
                new() { Text = contactPoint.CommitteeName },
                new() { Text = contactPoint.CompanyName },
                new() { Text = contactPoint.Phone },
                new() { Text = contactPoint.TitleName },
                new() { Text = contactPoint.PersonalPhoneMobile },
                new() { Text = contactPoint.Emails },
                new() { Text = contactPoint.CommitteeType },
                new() { Text = contactPoint.Department },
                new() { Text = contactPoint.Office },
                new() { Text = contactPoint.Level },
                new() { Text = contactPoint.ExtraParliamentaryCommission },
                new() { Text = contactPoint.SuperVisionDuty },
                new() { Text = contactPoint.TermOfOffice }
            } as IList<Cell>).ToList();

        return bodyCells;
    }

    private async Task<IList<IList<Cell>>> GetMembershipWithInterestData(DateOnly dataAnalysisDate)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var committees = await _committeeRepository.GetCommitteesWithInterestsForExport(dataAnalysisDate, departmentId, officeId, committeeId);

        var membershipList = committees
            .SelectMany(committee => committee.Memberships, (committee, membership) => new { committee, membership })
            .SelectMany(
                cm => cm.membership.Person!.Interests.Where(i => (i.BeginDate == null || i.BeginDate < dataAnalysisDate) && (i.EndDate == null || i.EndDate >= dataAnalysisDate)).DefaultIfEmpty(),
                (cm, interest) => new
                {
                    CommitteName = cm.committee.GetDescription(),
                    PersonId = cm.membership.Person!.Id,
                    PersonSurname = cm.membership.Person.Surname,
                    PersonGivenName = cm.membership.Person.GivenName,
                    PersonBirthYear = cm.membership.Person.BirthYear,
                    InterestText = interest is not null ? interest.Text : string.Empty,
                    LegalForm = interest?.LegalForm != null ? interest.LegalForm.GetText() : string.Empty,
                    InterestLegalForm = interest?.InterestLegalForm != null ? interest.InterestLegalForm.GetText() : string.Empty,
                    InterestOrganisation = interest?.InterestCommittee != null ? interest.InterestCommittee.GetText() : string.Empty,
                    InterestFunction = interest?.InterestFunction != null ? interest.InterestFunction.GetText() : string.Empty,
                    CommitteeType = cm.committee.CommitteeType!.GetText(),
                    Department = cm.committee.Department!.GetDescription(),
                    Office = cm.committee.Office!.GetDescription(),
                    Level = cm.committee.CommitteeLevel!.GetText(),
                    ExtraParliamentaryCommission = cm.committee.ExtraParliamentaryCommission ? BusinessTexts.Common_Yes : BusinessTexts.Common_No,
                    SuperVisionDuty = cm.committee.SupervisionDuty == true ? BusinessTexts.Common_Yes : BusinessTexts.Common_No,
                    TermOfOffice = cm.committee.TermOfOffice!.GetDescription(),
                    cm.committee.BeginDate,
                    cm.committee.EndDate,
                    cm.membership.IsActive
                }
            )
            .Where(m => m.IsActive)
            .ToList();

        var bodyCells = membershipList
            .OrderBy(x => x.CommitteName)
            .ThenBy(x => x.PersonSurname)
            .ThenBy(x => x.PersonGivenName)
            .Select(membership => new List<Cell>
            {
                new() { Text = membership.CommitteName },
                HyperlinkCell(membership.PersonSurname, $"persons/{membership.PersonId}"),
                new() { Text = membership.PersonGivenName },
                NumberCell(membership.PersonBirthYear),
                new() { Text = membership.InterestText },
                new() { Text = membership.LegalForm },
                new() { Text = membership.InterestLegalForm },
                new() { Text = membership.InterestOrganisation },
                new() { Text = membership.InterestFunction },
                new() { Text = membership.CommitteeType },
                new() { Text = membership.Department },
                new() { Text = membership.Office },
                new() { Text = membership.Level },
                new() { Text = membership.ExtraParliamentaryCommission },
                new() { Text = membership.SuperVisionDuty },
                new() { Text = membership.TermOfOffice },
                DateCell(membership.BeginDate),
                DateCell(membership.EndDate),
            } as IList<Cell>).ToList();

        return bodyCells;
    }

    public async Task<(string fileName, Stream content)> GeneratePersonExport(DateOnly dataAnalysisDate)
    {
        _logger.LogInformation("Generate person export for date {DataAnalysisDate}", dataAnalysisDate);

        string[] headers =
        [
            BusinessTexts.DataAnalysis_LastName,
            BusinessTexts.DataAnalysis_FirstName,
            BusinessTexts.DataAnalysis_City,
            BusinessTexts.DataAnalysis_BirthYear,
            BusinessTexts.DataAnalysis_Age,
            BusinessTexts.DataAnalysis_Language,
            BusinessTexts.DataAnalysis_Gender,
            BusinessTexts.DataAnalysis_Occupation,
            BusinessTexts.DataAnalysis_ActiveMembershipCount,
            BusinessTexts.DataAnalysis_EmploymentLevel,
            BusinessTexts.DataAnalysis_Committee
        ];

        var bodyCells = await GetPersonData(dataAnalysisDate);
        var spreadsheet = new Spreadsheet
        {
            HeaderCells = headers.Select(header => new Cell { Text = header, Format = CellFormat.Bold, BackgroundColor = "#FFCB00C6", ForegroundColor = "#FFFFFF" }).ToList(),
            BodyCells = bodyCells,
            TableOptions = new TableOptions
            {
                CreateTable = true,
                ShowTotalsRow = true,
                TableRange = $"A1:K{bodyCells.Count + 1}",
                TotalCells =
                [
                    new TotalCell { Field = BusinessTexts.DataAnalysis_LastName, Label = BusinessTexts.DataAnalysis_Total },
                    new TotalCell { Field = BusinessTexts.DataAnalysis_FirstName, Function = TotalCellFunctions.Count }
                ]
            }
        };

        var exportStream = await _documentService.CreateExcel(spreadsheet, SpreadsheetOptions.Default);

        return (GenerateFileName(dataAnalysisDate, BusinessTexts.DataAnalysis_Person), exportStream);
    }

    private async Task<IList<IList<Cell>>> GetPersonData(DateOnly dataAnalysisDate)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var persons = await _personRepository.GetPersonsForExport(dataAnalysisDate, departmentId, officeId, committeeId);

        var bodyCells = persons
            .Select(person => new List<Cell>
            {
                HyperlinkCell(person.Surname, $"persons/{person.Id}"),
                new() { Text = person.GivenName },
                new() { Text = person.CorrespondenceAddress?.City },
                NumberCell(person.BirthYear),
                NumberCell(person.Age),
                new() { Text = person.Language?.GetText() },
                new() { Text = person.Gender?.GetText() },
                new() { Text = person.Occupation },
                NumberCell(person.ActiveMembershipCount),
                NumberCell(person.TotalEmploymentLevel),
                new()
                {
                    Format = CellFormat.Wrap,
                    Text = string.Join("\n", person.ActiveCommittees.Select(x => x.GetDescription()))
                }
            } as IList<Cell>).ToList();

        return bodyCells;
    }

    public async Task<(string fileName, Stream content)> GenerateRegionExport(DateOnly dataAnalysisDate)
    {
        _logger.LogInformation("Generate region export for date {DataAnalysisDate}", dataAnalysisDate);

        var departments = (await _masterDataRepository.GetDepartments()).ToArray();
        string[] headers = [BusinessTexts.DataAnalysis_Canton, .. departments.OrderBy(x => x.Sort).Select(x => x.GetText()), BusinessTexts.DataAnalysis_Total];

        var spreadsheet = new Spreadsheet
        {
            HeaderCells = headers.Select(header => new Cell { Text = header, Format = CellFormat.Bold, BackgroundColor = "#FF71C8CB", ForegroundColor = "#FFFFFF" }).ToList(),
            BodyCells = await GetRegionData(dataAnalysisDate, departments)
        };

        var exportStream = await _documentService.CreateExcel(spreadsheet, SpreadsheetOptions.Default);

        return (GenerateFileName(dataAnalysisDate, BusinessTexts.DataAnalysis_Region), exportStream);
    }

    private async Task<IList<IList<Cell>>> GetRegionData(DateOnly dataAnalysisDate, Department[] departments)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var bodyCells = new List<IList<Cell>>();

        var cantons = await _masterDataRepository.GetCantons();
        var committees = await _committeeRepository.GetCommitteesForRegionExport(dataAnalysisDate, departmentId, officeId, committeeId);

        foreach (var canton in cantons.OrderBy(x => x.Sort).ThenBy(x => x.GetText()))
        {
            var sum = 0;
            var cantonRow = new List<Cell> { new() { Text = canton.GetText() } };

            foreach (var department in departments)
            {
                var cantonSum = committees
                    .Where(x => x.DepartmentId == department.Id)
                    .SelectMany(x => x.Memberships)
                    .Where(x => x is { IsActive: true, IsDeleted: false })
                    .Count(x => x.Person!.CorrespondenceAddress?.CantonId == canton.Id);
                cantonRow.Add(NumberCell(cantonSum));
                sum += cantonSum;
            }

            cantonRow.Add(NumberCell(sum));

            bodyCells.Add(cantonRow);
        }

        var totalRow = new List<Cell>
        {
            new() { Text = BusinessTexts.DataAnalysis_Total },
        };

        for (var i = 0; i < departments.Length; i++)
        {
            var total = bodyCells.Sum(x => x[i + 1].Text is not null ? int.Parse(x[i + 1].Text!) : 0);
            totalRow.Add(NumberCell(total));
        }

        bodyCells.Add(totalRow);

        return bodyCells;
    }

    public async Task<(string fileName, Stream content)> GenerateAgeExport(DateOnly dataAnalysisDate)
    {
        _logger.LogInformation("Generate age export for date {DataAnalysisDate}", dataAnalysisDate);

        string[] headers =
        [
            BusinessTexts.DataAnalysis_Age,
            BusinessTexts.DataAnalysis_ActiveMemberCount,
            BusinessTexts.DataAnalysis_FemalePresidentCount,
            BusinessTexts.DataAnalysis_FemalePresidentPercentage,
            BusinessTexts.DataAnalysis_MalePresidentCount,
            BusinessTexts.DataAnalysis_MalePresidentPercentage,
            BusinessTexts.DataAnalysis_WomanCount,
            BusinessTexts.DataAnalysis_WomanPercentage,
            BusinessTexts.DataAnalysis_ManCount,
            BusinessTexts.DataAnalysis_ManPercentage,
            BusinessTexts.DataAnalysis_ItalianCount,
            BusinessTexts.DataAnalysis_ItalianPercentage,
            BusinessTexts.DataAnalysis_GermanCount,
            BusinessTexts.DataAnalysis_GermanPercentage,
            BusinessTexts.DataAnalysis_FrenchCount,
            BusinessTexts.DataAnalysis_FrenchPercentage,
            BusinessTexts.DataAnalysis_RomanshCount,
            BusinessTexts.DataAnalysis_RomanshPercentage,
            BusinessTexts.DataAnalysis_FederalDutyCount,
            BusinessTexts.DataAnalysis_FederalDutyPercentage,
            BusinessTexts.DataAnalysis_NotFederalDutyCount,
            BusinessTexts.DataAnalysis_NotFederalDutyPercentage,
            BusinessTexts.DataAnalysis_CentralFederalAdministrationDutyCount,
            BusinessTexts.DataAnalysis_CentralFederalAdministrationDutyPercentage,
            BusinessTexts.DataAnalysis_DecentralizedFederalAdministrationDutyCount,
            BusinessTexts.DataAnalysis_DecentralizedFederalAdministrationDutyPercentage,
            BusinessTexts.DataAnalysis_FederalAssemblyCount,
            BusinessTexts.DataAnalysis_FederalAssemblyPercentage,
            BusinessTexts.DataAnalysis_NotFederalAssemblyCount,
            BusinessTexts.DataAnalysis_NotFederalAssemblyPercentage
        ];

        var bodyCells = await GetAgeData(dataAnalysisDate);
        var spreadsheet = new Spreadsheet
        {
            HeaderCells = headers.Select(header => new Cell { Text = header, Format = CellFormat.Bold, BackgroundColor = "#FF99ACB0", ForegroundColor = "#FFFFFF" }).ToList(),
            BodyCells = bodyCells,
            TableOptions = new TableOptions
            {
                CreateTable = true,
                ShowTotalsRow = true,
                TableRange = $"A1:AD{bodyCells.Count + 1}",
                TotalCells =
                [
                    new TotalCell { Field = BusinessTexts.DataAnalysis_Age, Label = BusinessTexts.DataAnalysis_Total },
                    TotalSumCell(BusinessTexts.DataAnalysis_ActiveMemberCount),
                    TotalSumCell(BusinessTexts.DataAnalysis_FemalePresidentCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_FemalePresidentPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_MalePresidentCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_MalePresidentPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_WomanCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_WomanPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_ManCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_ManPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_ItalianCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_ItalianPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_GermanCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_GermanPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_FrenchCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_FrenchPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_RomanshCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_RomanshPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_FederalDutyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_FederalDutyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_NotFederalDutyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_NotFederalDutyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_CentralFederalAdministrationDutyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_CentralFederalAdministrationDutyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_DecentralizedFederalAdministrationDutyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_DecentralizedFederalAdministrationDutyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_FederalAssemblyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_FederalAssemblyPercentage),
                    TotalSumCell(BusinessTexts.DataAnalysis_NotFederalAssemblyCount),
                    TotalAveragePercentCell(BusinessTexts.DataAnalysis_NotFederalAssemblyPercentage)
                ]
            }
        };

        var exportStream = await _documentService.CreateExcel(spreadsheet, SpreadsheetOptions.Default);

        return (GenerateFileName(dataAnalysisDate, BusinessTexts.DataAnalysis_Age), exportStream);
    }

    private async Task<IList<IList<Cell>>> GetAgeData(DateOnly dataAnalysisDate)
    {
        var (departmentId, officeId, committeeId) = await _eiamAssignmentService.GetPermittedIds();

        var committees = await _committeeRepository.GetCommitteesForExport(dataAnalysisDate, departmentId, officeId, committeeId);

        var memberships = committees
            .SelectMany(x => x.Memberships)
            .Where(x => x is { IsActive: true, IsDeleted: false });

        var bodyCells = memberships.GroupBy(x => x.Person!.Age)
            .OrderBy(x => x.Key)
            .Select(membershipsByAge => new List<Cell>
            {
                NumberCell(membershipsByAge.Key),
                NumberCell(membershipsByAge.Count()), // Total aktive Mitglieder
                NumberCell(membershipsByAge.Count(x => x.Person!.IsFemale && x.Function!.IsPresident)), // # Präsidentinnen
                PercentageCell(Divide(membershipsByAge.Count(x => x.Person!.IsFemale && x.Function!.IsPresident), membershipsByAge.Count(x => x.Function!.IsPresident))), // % Präsidentinnen
                NumberCell(membershipsByAge.Count(x => x.Person!.IsMale && x.Function!.IsPresident)), // # Präsidenten
                PercentageCell(Divide(membershipsByAge.Count(x => x.Person!.IsMale && x.Function!.IsPresident), membershipsByAge.Count(x => x.Function!.IsPresident))), // % Präsidenten
                NumberCell(membershipsByAge.Count(x => x.Person!.IsFemale)), // # Frauen
                PercentageCell(Divide(membershipsByAge.Count(x => x.Person!.IsFemale), membershipsByAge.Count())), // % Frauen
                NumberCell(membershipsByAge.Count(x => x.Person!.IsMale)), // # Männer
                PercentageCell(Divide(membershipsByAge.Count(x => x.Person!.IsMale), membershipsByAge.Count())), // % Männer
                NumberCell(membershipsByAge.Count(x => x.Person!.IsItalian)), // # Italienisch
                PercentageCell(Divide(membershipsByAge.Count(x => x.Person!.IsItalian), membershipsByAge.Count())), // % Italienisch
                NumberCell(membershipsByAge.Count(x => x.Person!.IsGerman)), // # Deutsch
                PercentageCell(Divide(membershipsByAge.Count(x => x.Person!.IsGerman), membershipsByAge.Count())), // % Deutsch
                NumberCell(membershipsByAge.Count(x => x.Person!.IsFrench)), // # Französisch
                PercentageCell(Divide(membershipsByAge.Count(x => x.Person!.IsFrench), membershipsByAge.Count())), // % Französisch
                NumberCell(membershipsByAge.Count(x => x.Person!.IsRomansh)), // # Rätoromanisch
                PercentageCell(Divide(membershipsByAge.Count(x => x.Person!.IsRomansh), membershipsByAge.Count())), // % Rätoromanisch
                NumberCell(membershipsByAge.Count(x => x.Person!.FederalDuty)), // # Im Bundesdienst
                PercentageCell(Divide(membershipsByAge.Count(x => x.Person!.FederalDuty), membershipsByAge.Count())), // % Im Bundesdienst
                NumberCell(membershipsByAge.Count(x => !x.Person!.FederalDuty)), // # Nicht im Bundesdienst
                PercentageCell(Divide(membershipsByAge.Count(x => !x.Person!.FederalDuty), membershipsByAge.Count())), // % Nicht im Bundesdienst
                NumberCell(membershipsByAge.Count(x => x.Person!.Office is { IsCentralFederalAdministration: true })), // # Im zentralen Bundesdienst
                PercentageCell(Divide(membershipsByAge.Count(x => x.Person!.Office is { IsCentralFederalAdministration: true }), membershipsByAge.Count())), // % Im zentralen Bundesdienst
                NumberCell(membershipsByAge.Count(x => x.Person!.Office is { IsCentralFederalAdministration: false })), // # Im dezentralen Bundesdienst
                PercentageCell(Divide(membershipsByAge.Count(x => x.Person!.Office is { IsCentralFederalAdministration: false }), membershipsByAge.Count())), // % Im dezentralen Bundesdienst
                NumberCell(membershipsByAge.Count(x => x.Person!.FederalAssembly)), // # Mitglieder der Bundesversammlung
                PercentageCell(Divide(membershipsByAge.Count(x => x.Person!.FederalAssembly), membershipsByAge.Count())), // % Mitglieder der Bundesversammlung
                NumberCell(membershipsByAge.Count(x => !x.Person!.FederalAssembly)), // # Nicht Mitglieder der Bundesversammlung
                PercentageCell(Divide(membershipsByAge.Count(x => !x.Person!.FederalAssembly), membershipsByAge.Count()))
            } as IList<Cell>)
            .ToList();

        return bodyCells;
    }

    private static Cell NumberCell(decimal value, string format = "0")
    {
        return new Cell
        {
            Text = value.ToString(CultureInfo.InvariantCulture),
            FormatType = CellFormatTypes.Number,
            Format = format
        };
    }

    private static Cell DateCell(DateOnly? value)
    {
        return new Cell
        {
            Text = value is not null ? value.Value.ToString("O") : string.Empty,
            FormatType = CellFormatTypes.Date,
            Format = "dd.MM.yyyy"
        };
    }

    private static Cell PercentageCell(decimal value)
    {
        return new Cell
        {
            Text = value.ToString(CultureInfo.InvariantCulture),
            FormatType = CellFormatTypes.Percentage,
            Format = "0.0%"
        };
    }

    private Cell HyperlinkCell(string text, string relativeUrl)
    {
        return new Cell
        {
            Text = text,
            Hyperlink = $"{_configuration["FrontendUrl"]}/{relativeUrl}"
        };
    }

    private static TotalCell TotalSumCell(string field)
    {
        return new TotalCell
        {
            Field = field,
            Function = TotalCellFunctions.Sum
        };
    }

    private static TotalCell TotalAveragePercentCell(string field)
    {
        return new TotalCell
        {
            Field = field,
            Function = TotalCellFunctions.Average,
            Format = "0.00%"
        };
    }

    private static decimal Divide(decimal x, decimal y)
    {
        return y is 0 ? 0 : x / y;
    }

    private static string GenerateFileName(DateOnly dataAnalysisDate, string exportType)
    {
        return $"{dataAnalysisDate:yyyyMMdd}_{exportType}.xlsx";
    }
}
