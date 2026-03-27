using System.Globalization;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.Common.Resources;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Swiss.FCh.DocumentService.Client.Models;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class DataAnalysisServiceTests
{
    private Swiss.FCh.DocumentService.Client.IDocumentService _documentService;
    private IEiamAssignmentService _eiamAssignmentService;
    private ICommitteeRepository _committeeRepository;
    private IPersonRepository _personRepository;
    private IMasterDataRepository _masterDataRepository;
    private ILogger<DataAnalysisService> _logger;
    private IConfiguration _configuration;
    private DataAnalysisService _service;

    private readonly Guid _zeroGuid = Guid.Empty;

    [SetUp]
    public void Setup()
    {
        _documentService = Substitute.For<Swiss.FCh.DocumentService.Client.IDocumentService>();
        _eiamAssignmentService = Substitute.For<IEiamAssignmentService>();
        _committeeRepository = Substitute.For<ICommitteeRepository>();
        _personRepository = Substitute.For<IPersonRepository>();
        _masterDataRepository = Substitute.For<IMasterDataRepository>();
        _logger = NullLogger<DataAnalysisService>.Instance;
        _configuration = Substitute.For<IConfiguration>();
        _service = new DataAnalysisService(_documentService, _eiamAssignmentService, _committeeRepository, _personRepository, _masterDataRepository, _logger, _configuration);
    }

    [TearDown]
    public void TearDown()
    {
        _documentService.ClearSubstitute();
        _eiamAssignmentService.ClearSubstitute();
        _committeeRepository.ClearSubstitute();
        _personRepository.ClearSubstitute();
        _masterDataRepository.ClearSubstitute();
        _configuration.ClearSubstitute();
    }

    [Test]
    public async Task GenerateCommitteeTypeExport_ShouldCalculatePercentagesCorrectly()
    {
        var dataAnalysisDate = new DateOnly(2024, 1, 1);

        Committee[] committees =
        [
            new CommitteeBuilder()
                .WithMembership(new MembershipBuilder().WithIsActive(true).WithFemalePresident().Build())
                .WithMembership(new MembershipBuilder().WithIsActive(true).WithFemalePresident().Build())
                .WithMembership(new MembershipBuilder().WithIsActive(true).WithFemalePresident().Build())
                .WithMembership(new MembershipBuilder().WithIsActive(true).WithMalePresident().Build())
                .WithMembership(new MembershipBuilder().WithIsActive(true).WithFemaleMember().Build())
                .WithMembership(new MembershipBuilder().WithIsActive(true).WithMaleMember().Build())
                .Build()
        ];

        _committeeRepository
            .GetCommitteesForExport(dataAnalysisDate, _zeroGuid, _zeroGuid, _zeroGuid)
            .Returns(committees);

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _service.GenerateCommitteeTypeExport(dataAnalysisDate);

        Assert.That(capturedSpreadsheet, Is.Not.Null);
        var dataRow = capturedSpreadsheet.BodyCells[0];

        Assert.Multiple(() =>
        {
            Assert.That(dataRow[1].Text, Is.EqualTo("1")); // Active Committee Count
            Assert.That(dataRow[2].Text, Is.EqualTo("6")); // Active Member Count
            Assert.That(dataRow[3].Text, Is.EqualTo("3")); // President Count (Female)
            Assert.That(dataRow[4].Text, Is.EqualTo("0.75")); // President Percentage (Female)
            Assert.That(dataRow[5].Text, Is.EqualTo("1")); // President Count (Male)
            Assert.That(dataRow[6].Text, Is.EqualTo("0.25")); // President Percentage (Male)
            Assert.That(dataRow[7].Text, Is.EqualTo("4")); // Woman Count
            Assert.That(dataRow[8].Text, Is.EqualTo("0.6666666666666666666666666667")); // Woman Percentage
            Assert.That(dataRow[9].Text, Is.EqualTo("2")); // Man Count
            Assert.That(dataRow[10].Text, Is.EqualTo("0.3333333333333333333333333333")); // Man Percentage
        });
    }

    [Test]
    public async Task GenerateCommitteeTypeExport_ShouldHandleZeroDenominatorCorrectly()
    {
        var dataAnalysisDate = new DateOnly(2024, 1, 1);

        Committee[] committees = [
            new CommitteeBuilder()
                .Build()
            ];

        _committeeRepository
            .GetCommitteesForExport(dataAnalysisDate, _zeroGuid, _zeroGuid, _zeroGuid)
            .Returns(committees);

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _service.GenerateCommitteeTypeExport(dataAnalysisDate);

        Assert.That(capturedSpreadsheet, Is.Not.Null);
        var dataRow = capturedSpreadsheet.BodyCells[0];

        Assert.Multiple(() =>
        {
            Assert.That(dataRow[4].Text, Is.EqualTo("0")); // President Percentage
            Assert.That(dataRow[6].Text, Is.EqualTo("0")); // Woman Percentage
            Assert.That(dataRow[8].Text, Is.EqualTo("0")); // Man Percentage
            Assert.That(dataRow[10].Text, Is.EqualTo("0")); // Italian Percentage
            Assert.That(dataRow[12].Text, Is.EqualTo("0")); // German Percentage
            Assert.That(dataRow[14].Text, Is.EqualTo("0")); // French Percentage
            Assert.That(dataRow[16].Text, Is.EqualTo("0")); // Romansh Percentage
        });
    }

    [Test]
    public async Task GenerateCommitteeTypeExport_ShouldAggregateMultipleCommitteesCorrectly()
    {
        var dataAnalysisDate = new DateOnly(2024, 1, 1);

        var membership1 = new MembershipBuilder()
            .WithIsActive(true)
            .WithFunction(new FunctionBuilder().WithUri(Function.PresidentUri).Build())
            .WithPerson(new PersonBuilder()
                .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
                .WithLanguage(new LanguageBuilder().WithUri(Language.ItalianUri).Build())
                .WithFederalDuty(true)
                .WithFederalAssembly(true)
                .WithOffice(new OfficeBuilder().WithIsCentralFederalAdministration(true).Build())
                .Build())
            .Build();

        var membership2 = new MembershipBuilder()
            .WithIsActive(true)
            .WithPerson(new PersonBuilder()
                .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                .WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build())
                .WithFederalDuty(false)
                .WithFederalAssembly(false)
                .WithOffice(new OfficeBuilder().WithIsCentralFederalAdministration(false).Build())
                .Build())
            .Build();

        var membership3 = new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder()
                    .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
                    .WithLanguage(new LanguageBuilder().WithUri(Language.FrenchUri).Build())
                    .WithFederalDuty(true)
                    .WithFederalAssembly(true)
                    .Build())
                .WithFunction(new FunctionBuilder().WithUri(Function.PresidentUri).Build())
                .Build();

        var membership4 = new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder()
                    .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                    .WithLanguage(new LanguageBuilder().WithUri(Language.RomanshUri).Build())
                    .WithFederalDuty(false)
                    .WithFederalAssembly(false)
                    .Build())
                .Build();

        var committeeType = new CommitteeTypeBuilder().WithId(Guid.NewGuid()).Build();

        Committee[] committees =
        [
            new CommitteeBuilder()
                .WithCommitteeType(committeeType)
                .WithMembership(membership1)
                .WithMembership(membership2)
                .Build(),
            new CommitteeBuilder()
                .WithCommitteeType(committeeType)
                .WithMembership(membership3)
                .WithMembership(membership4)
                .Build(),
        ];

        _committeeRepository
            .GetCommitteesForExport(dataAnalysisDate, _zeroGuid, _zeroGuid, _zeroGuid)
            .Returns(committees);

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _service.GenerateCommitteeTypeExport(dataAnalysisDate);

        Assert.That(capturedSpreadsheet, Is.Not.Null);
        var dataRow = capturedSpreadsheet.BodyCells[0];

        Assert.Multiple(() =>
        {
            // Verify aggregated counts
            Assert.That(dataRow[1].Text, Is.EqualTo("2")); // Active Committee Count
            Assert.That(dataRow[2].Text, Is.EqualTo("4")); // Active Member Count
            Assert.That(dataRow[3].Text, Is.EqualTo("2")); // President Count (Female)
            Assert.That(dataRow[4].Text, Is.EqualTo("1")); // President Percentage (Female) - 2/2 = 100%
            Assert.That(dataRow[5].Text, Is.EqualTo("0")); // President Count (Male)
            Assert.That(dataRow[6].Text, Is.EqualTo("0")); // President Percentage (Male) - 0/2 = 0%
            Assert.That(dataRow[7].Text, Is.EqualTo("2")); // Woman Count
            Assert.That(dataRow[8].Text, Is.EqualTo("0.5")); // Woman Percentage - 2/4 = 50%
            Assert.That(dataRow[9].Text, Is.EqualTo("2")); // Man Count
            Assert.That(dataRow[10].Text, Is.EqualTo("0.5")); // Man Percentage - 2/4 = 50%

            // Verify language distribution
            Assert.That(dataRow[11].Text, Is.EqualTo("1")); // Italian Count
            Assert.That(dataRow[12].Text, Is.EqualTo("0.25")); // Italian Percentage - 1/4 = 25%
            Assert.That(dataRow[13].Text, Is.EqualTo("1")); // German Count
            Assert.That(dataRow[14].Text, Is.EqualTo("0.25")); // German Percentage - 1/4 = 25%
            Assert.That(dataRow[15].Text, Is.EqualTo("1")); // French Count
            Assert.That(dataRow[16].Text, Is.EqualTo("0.25")); // French Percentage - 1/4 = 25%
            Assert.That(dataRow[17].Text, Is.EqualTo("1")); // Romansh Count
            Assert.That(dataRow[18].Text, Is.EqualTo("0.25")); // Romansh Percentage - 1/4 = 25%

            // Verify federal duty and assembly distribution
            Assert.That(dataRow[19].Text, Is.EqualTo("2")); // Federal Duty Count
            Assert.That(dataRow[20].Text, Is.EqualTo("0.5")); // Federal Duty Percentage - 2/4 = 50%
            Assert.That(dataRow[21].Text, Is.EqualTo("2")); // Not Federal Duty Count
            Assert.That(dataRow[22].Text, Is.EqualTo("0.5")); // Not Federal Duty Percentage - 2/4 = 50%
            Assert.That(dataRow[23].Text, Is.EqualTo("1")); // Central Federal Duty Count
            Assert.That(dataRow[24].Text, Is.EqualTo("0.25")); // Central Federal Duty Percentage - 2/4 = 50%
            Assert.That(dataRow[25].Text, Is.EqualTo("1")); // Decentralized Federal Duty Count
            Assert.That(dataRow[26].Text, Is.EqualTo("0.25")); // Decentralized Federal Duty Percentage - 2/4 = 50%
            Assert.That(dataRow[27].Text, Is.EqualTo("2")); // Federal Assembly Count
            Assert.That(dataRow[28].Text, Is.EqualTo("0.5")); // Federal Assembly Percentage - 2/4 = 50%
            Assert.That(dataRow[29].Text, Is.EqualTo("2")); // Not Federal Assembly Count
            Assert.That(dataRow[30].Text, Is.EqualTo("0.5")); // Not Federal Assembly Percentage - 2/4 = 50%
        });
    }

    [Test]
    public async Task GenerateCommitteeExport_ShouldGenerateCommitteeCorrectly()
    {
        var dataAnalysisDate = new DateOnly(2024, 1, 1);
        var committee = new CommitteeBuilder()
            .WithBeginDate(new DateOnly(2020, 1, 1))
            .WithEndDate(new DateOnly(2025, 1, 1))
            .WithCommitteeType(new CommitteeTypeBuilder().WithId(CommitteeType.AdministrationCommissionGuid).Build())
            .WithSupervisionDuty(true)
            .Build();

        var membership1 = new MembershipBuilder()
            .WithIsActive(true)
            .WithFunction(new FunctionBuilder().WithUri(Function.PresidentUri).Build())
            .WithPerson(new PersonBuilder()
                .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
                .WithLanguage(new LanguageBuilder().WithUri(Language.ItalianUri).Build())
                .WithFederalDuty(true)
                .WithFederalAssembly(true)
                .WithOffice(new OfficeBuilder().WithIsCentralFederalAdministration(true).Build())
                .Build())
            .Build();

        var membership2 = new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder()
                    .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                    .WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build())
                    .WithOffice(new OfficeBuilder().WithIsCentralFederalAdministration(false).Build())
                    .WithFederalDuty(false)
                    .WithFederalAssembly(false)
                    .Build())
                .Build();

        committee.Memberships.Add(membership1);
        committee.Memberships.Add(membership2);

        _committeeRepository
            .GetCommitteesForExport(dataAnalysisDate, _zeroGuid, _zeroGuid, _zeroGuid)
            .Returns([committee]);

        _configuration["FrontendUrl"].Returns("FooBar");

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _service.GenerateCommitteeExport(dataAnalysisDate);

        Assert.That(capturedSpreadsheet, Is.Not.Null);

        var dataRow = capturedSpreadsheet.BodyCells[0];
        Assert.Multiple(() =>
        {
            Assert.That(dataRow[0].Text, Is.EqualTo(committee.GetDescription()));
            Assert.That(dataRow[0].Hyperlink, Is.EqualTo($"FooBar/committees/{committee.Id}"));
            Assert.That(dataRow[1].Text, Is.EqualTo(committee.CommitteeType!.GetText()));
            Assert.That(dataRow[2].Text, Is.EqualTo(committee.Department!.GetText()));
            Assert.That(dataRow[3].Text, Is.EqualTo(committee.Office!.GetText()));
            Assert.That(dataRow[4].Text, Is.EqualTo(committee.CommitteeLevel!.GetText()));
            Assert.That(dataRow[5].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // ExtraParliamentaryCommission
            Assert.That(dataRow[6].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // SupervisionDuty
            Assert.That(dataRow[7].Text, Is.EqualTo(committee.TermOfOffice!.GetText()));
            Assert.That(dataRow[8].Text, Is.EqualTo(committee.BeginDate.ToString("O", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[9].Text, Is.EqualTo(committee.EndDate?.ToString("O", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[10].Text, Is.EqualTo("2")); // Active Member Count
            Assert.That(dataRow[11].Text, Is.EqualTo("1")); // Female President Count
            Assert.That(dataRow[12].Text, Is.EqualTo("1")); // Female President Percentage (1/1)
            Assert.That(dataRow[13].Text, Is.EqualTo("0")); // Male President Count
            Assert.That(dataRow[14].Text, Is.EqualTo("0")); // Male President Percentage (1/1)
            Assert.That(dataRow[15].Text, Is.EqualTo("1")); // Woman Count
            Assert.That(dataRow[16].Text, Is.EqualTo("0.5")); // Woman Percentage (1/2)
            Assert.That(dataRow[17].Text, Is.EqualTo("1")); // Man Count
            Assert.That(dataRow[18].Text, Is.EqualTo("0.5")); // Man Percentage (1/2)
            Assert.That(dataRow[19].Text, Is.EqualTo("1")); // Italian Count
            Assert.That(dataRow[20].Text, Is.EqualTo("0.5")); // Italian Percentage
            Assert.That(dataRow[21].Text, Is.EqualTo("1")); // German Count
            Assert.That(dataRow[22].Text, Is.EqualTo("0.5")); // German Percentage
            Assert.That(dataRow[23].Text, Is.EqualTo("0")); // French Count
            Assert.That(dataRow[24].Text, Is.EqualTo("0")); // French Percentage
            Assert.That(dataRow[25].Text, Is.EqualTo("0")); // Romansh Count
            Assert.That(dataRow[26].Text, Is.EqualTo("0")); // Romansh Percentage
            Assert.That(dataRow[27].Text, Is.EqualTo("1")); // Federal Duty Count
            Assert.That(dataRow[28].Text, Is.EqualTo("0.5")); // Federal Duty Percentage (1/2)
            Assert.That(dataRow[29].Text, Is.EqualTo("1")); // Not Federal Duty Count
            Assert.That(dataRow[30].Text, Is.EqualTo("0.5")); // Not Federal Duty Percentage (1/2)
            Assert.That(dataRow[31].Text, Is.EqualTo("1")); // Central Federal Duty Count
            Assert.That(dataRow[32].Text, Is.EqualTo("0.5")); // Central Federal Duty Percentage - 2/4 = 50%
            Assert.That(dataRow[33].Text, Is.EqualTo("1")); // Decentralized Federal Duty Count
            Assert.That(dataRow[34].Text, Is.EqualTo("0.5")); // Decentralized Federal Duty Percentage - 2/4 = 50%
            Assert.That(dataRow[35].Text, Is.EqualTo("1")); // Federal Assembly Count
            Assert.That(dataRow[36].Text, Is.EqualTo("0.5")); // Federal Assembly Percentage (1/2)
            Assert.That(dataRow[37].Text, Is.EqualTo("1")); // Not Federal Assembly Count
            Assert.That(dataRow[38].Text, Is.EqualTo("0.5")); // Not Federal Assembly Percentage (1/2)
        });

        var tableOptions = capturedSpreadsheet.TableOptions;
        Assert.That(tableOptions, Is.Not.Null);
        Assert.That(tableOptions.TableRange, Is.EqualTo("A1:AM2"));
    }

    [Test]
    public async Task GenerateMembershipExport_ShouldExportMembershipCorrectly()
    {
        var dataAnalysisDate = new DateOnly(2024, 1, 1);
        var committee = new CommitteeBuilder()
            .WithBeginDate(new DateOnly(2020, 1, 1))
            .WithEndDate(new DateOnly(2025, 1, 1))
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .WithSupervisionDuty(true)
            .Build();
        var person = new PersonBuilder()
            .WithFederalDuty(true)
            .WithFederalAssembly(false)
            .WithCorrespondenceAddress(new AddressBuilder().WithCanton(new CantonBuilder().Build()).Build())
            .WithGender(new GenderBuilder().Build())
            .WithLanguage(new LanguageBuilder().Build())
            .WithBirthYear(2000)
            .WithOffice(new OfficeBuilder().WithIsCentralFederalAdministration(true).Build())
            .Build();
        var membership = new MembershipBuilder()
            .WithIsActive(true)
            .WithPerson(person)
            .WithPersonId(person.Id)
            .WithCommittee(committee)
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddYears(-4)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddYears(2)))
            .Build();
        committee.Memberships.Add(membership);

        _committeeRepository.GetCommitteesForExport(dataAnalysisDate, _zeroGuid, _zeroGuid, _zeroGuid).Returns([committee]);

        _configuration["FrontendUrl"].Returns("FooBar");

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _service.GenerateMembershipExport(dataAnalysisDate);

        Assert.That(capturedSpreadsheet, Is.Not.Null);

        var dataRow = capturedSpreadsheet.BodyCells[0];
        Assert.Multiple(() =>
        {
            Assert.That(dataRow[0].Text, Is.EqualTo(committee.GetDescription()));
            Assert.That(dataRow[1].Text, Is.EqualTo(membership.Person!.Surname));
            Assert.That(dataRow[1].Hyperlink, Is.EqualTo($"FooBar/persons/{membership.PersonId}"));
            Assert.That(dataRow[2].Text, Is.EqualTo(membership.Person!.GivenName));
            Assert.That(dataRow[3].Text, Is.EqualTo(membership.Person!.CorrespondenceAddress!.City));
            Assert.That(dataRow[4].Text, Is.EqualTo(membership.Person!.CorrespondenceAddress!.Canton!.GetText()));
            Assert.That(dataRow[5].Text, Is.EqualTo(membership.Person!.BirthYear.ToString(CultureInfo.InvariantCulture)));
            Assert.That(dataRow[6].Text, Is.EqualTo((DateTime.UtcNow.Year - membership.Person!.BirthYear).ToString(CultureInfo.InvariantCulture)));
            Assert.That(dataRow[7].Text, Is.EqualTo(membership.Person!.Occupation));
            Assert.That(dataRow[8].Text, Is.EqualTo(membership.Person!.Language!.GetText()));
            Assert.That(dataRow[9].Text, Is.EqualTo(membership.Person!.Gender!.GetText()));
            Assert.That(dataRow[10].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // Federal Duty
            Assert.That(dataRow[11].Text, Is.EqualTo(BusinessTexts.DataAnalysis_FederalDuty_Central)); // Central/Decentralized Duty
            Assert.That(dataRow[12].Text, Is.EqualTo(BusinessTexts.Common_No)); // Federal Assembly
            Assert.That(dataRow[13].Text, Is.EqualTo(membership.Function!.GetText()));
            Assert.That(dataRow[14].Text, Is.EqualTo(membership.BeginDate.ToString("O", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[15].Text, Is.EqualTo(membership.EndDate.ToString("O", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[16].Text, Is.EqualTo(membership.ElectionType!.GetText()));
            Assert.That(dataRow[17].Text, Is.EqualTo(membership.ElectionOffice!.GetText()));
            Assert.That(dataRow[18].Text, Is.EqualTo(membership.MaximumEmploymentLevel.GetValueOrDefault().ToString(CultureInfo.InvariantCulture)));
            Assert.That(dataRow[19].Text, Is.EqualTo("4"));
            Assert.That(dataRow[20].Text, Is.EqualTo(membership.MembershipAddition!.GetText()));
            Assert.That(dataRow[21].Text, Is.EqualTo(committee.CommitteeType!.GetText()));
            Assert.That(dataRow[22].Text, Is.EqualTo(committee.Department!.GetText()));
            Assert.That(dataRow[23].Text, Is.EqualTo(committee.Office!.GetText()));
            Assert.That(dataRow[24].Text, Is.EqualTo(committee.CommitteeLevel!.GetText()));
            Assert.That(dataRow[25].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // ExtraParliamentary Commission
            Assert.That(dataRow[26].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // Supervision Duty
            Assert.That(dataRow[27].Text, Is.EqualTo(committee.TermOfOffice!.GetText()));
            Assert.That(dataRow[28].Text, Is.EqualTo(committee.BeginDate.ToString("O", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[29].Text, Is.EqualTo(committee.EndDate?.ToString("O", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[30].Text, Is.EqualTo("1"));
        });

        var tableOptions = capturedSpreadsheet.TableOptions;
        Assert.That(tableOptions, Is.Not.Null);
        Assert.That(tableOptions.TableRange, Is.EqualTo("A1:AE2"));
    }

    [Test]
    public async Task GenerateMembershipInterestExport_WithMultipleInterests_ShouldExportDataCorrectly()
    {
        var dataAnalysisDate = new DateOnly(2024, 1, 1);

        var legalForm1 = new LegalFormBuilder().Build();
        var interestLegalForm1 = new InterestLegalFormBuilder().Build();
        var interestFunction1 = new InterestFunctionBuilder().Build();
        var interestCommittee1 = new InterestCommitteeBuilder().Build();

        var legalForm2 = new LegalFormBuilder().Build();
        var interestLegalForm2 = new InterestLegalFormBuilder().Build();
        var interestFunction2 = new InterestFunctionBuilder().Build();
        var interestCommittee2 = new InterestCommitteeBuilder().Build();

        var legalForm3 = new LegalFormBuilder().Build();
        var interestLegalForm3 = new InterestLegalFormBuilder().Build();
        var interestFunction3 = new InterestFunctionBuilder().Build();
        var interestCommittee3 = new InterestCommitteeBuilder().Build();

        var interest1 = new InterestBuilder().WithText("Interesse an Geld").WithInterestCommittee(interestCommittee1).WithInterestFunction(interestFunction1).WithInterestLegalForm(interestLegalForm1).WithLegalForm(legalForm1).WithBeginDate(null).WithEndDate(null).Build();
        var interest2 = new InterestBuilder().WithText("Interesse an Zaster (veraltet)").WithInterestCommittee(interestCommittee3).WithInterestFunction(interestFunction3).WithInterestLegalForm(interestLegalForm3).WithLegalForm(legalForm3).WithBeginDate(dataAnalysisDate.AddDays(-150)).WithEndDate(dataAnalysisDate.AddDays(-50)).Build();
        var interest3 = new InterestBuilder().WithText("Interesse an Macht").WithInterestCommittee(interestCommittee2).WithInterestFunction(interestFunction2).WithInterestLegalForm(interestLegalForm2).WithLegalForm(legalForm2).WithBeginDate(dataAnalysisDate.AddDays(-50)).WithEndDate(dataAnalysisDate.AddDays(50)).Build();

        var interests = new List<Interest>
        {
            interest1,
            interest2,
            interest3
        };

        var committee = new CommitteeBuilder()
            .WithBeginDate(new DateOnly(2020, 1, 1))
            .WithEndDate(new DateOnly(2025, 1, 1))
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .WithSupervisionDuty(true)
            .Build();
        var person = new PersonBuilder()
            .WithBirthYear(2000)
            .WithInterests(interests)
            .Build();
        var membership = new MembershipBuilder()
            .WithIsActive(true)
            .WithPerson(person)
            .Build();
        committee.Memberships.Add(membership);

        _committeeRepository.GetCommitteesWithInterestsForExport(dataAnalysisDate, _zeroGuid, _zeroGuid, _zeroGuid).Returns([committee]);

        _configuration["FrontendUrl"].Returns("FooBar");

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _service.GenerateMembershipInterestExport(dataAnalysisDate);

        Assert.That(capturedSpreadsheet, Is.Not.Null);

        var dataRow = capturedSpreadsheet.BodyCells[0];
        Assert.Multiple(() =>
        {
            Assert.That(dataRow[0].Text, Is.EqualTo(committee.GetDescription()));
            Assert.That(dataRow[1].Text, Is.EqualTo(membership.Person!.Surname));
            Assert.That(dataRow[1].Hyperlink, Is.EqualTo($"FooBar/persons/{membership.PersonId}"));
            Assert.That(dataRow[2].Text, Is.EqualTo(membership.Person!.GivenName));
            Assert.That(dataRow[3].Text, Is.EqualTo(membership.Person!.BirthYear.ToString(CultureInfo.InvariantCulture)));
            Assert.That(dataRow[4].Text, Is.EqualTo(membership.Person!.Interests.FirstOrDefault()?.Text));
            Assert.That(dataRow[5].Text, Is.EqualTo(membership.Person!.Interests.FirstOrDefault()?.LegalForm!.GetText()));
            Assert.That(dataRow[6].Text, Is.EqualTo(membership.Person!.Interests.FirstOrDefault()?.InterestLegalForm!.GetText()));
            Assert.That(dataRow[7].Text, Is.EqualTo(membership.Person!.Interests.FirstOrDefault()?.InterestCommittee!.GetText()));
            Assert.That(dataRow[8].Text, Is.EqualTo(membership.Person!.Interests.FirstOrDefault()?.InterestFunction!.GetText()));
            Assert.That(dataRow[9].Text, Is.EqualTo(committee.CommitteeType!.GetText()));
            Assert.That(dataRow[10].Text, Is.EqualTo(committee.Department!.GetDescription()));
            Assert.That(dataRow[11].Text, Is.EqualTo(committee.Office!.GetDescription()));
            Assert.That(dataRow[12].Text, Is.EqualTo(committee.CommitteeLevel!.GetText()));
            Assert.That(dataRow[13].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // ExtraParliamentary Commission
            Assert.That(dataRow[14].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // Supervision Duty
            Assert.That(dataRow[15].Text, Is.EqualTo(committee.TermOfOffice!.GetDescription()));
            Assert.That(dataRow[16].Text, Is.EqualTo(committee.BeginDate.ToString("O", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[17].Text, Is.EqualTo(committee.EndDate?.ToString("O", CultureInfo.InvariantCulture)));
        });

        dataRow = capturedSpreadsheet.BodyCells[1];
        Assert.Multiple(() =>
        {
            Assert.That(dataRow[0].Text, Is.EqualTo(committee.GetDescription()));
            Assert.That(dataRow[1].Text, Is.EqualTo(membership.Person!.Surname));
            Assert.That(dataRow[1].Hyperlink, Is.EqualTo($"FooBar/persons/{membership.PersonId}"));
            Assert.That(dataRow[2].Text, Is.EqualTo(membership.Person!.GivenName));
            Assert.That(dataRow[3].Text, Is.EqualTo(membership.Person!.BirthYear.ToString(CultureInfo.InvariantCulture)));
            Assert.That(dataRow[4].Text, Is.EqualTo(membership.Person!.Interests.LastOrDefault()?.Text));
            Assert.That(dataRow[5].Text, Is.EqualTo(membership.Person!.Interests.LastOrDefault()?.LegalForm!.GetText()));
            Assert.That(dataRow[6].Text, Is.EqualTo(membership.Person!.Interests.LastOrDefault()?.InterestLegalForm!.GetText()));
            Assert.That(dataRow[7].Text, Is.EqualTo(membership.Person!.Interests.LastOrDefault()?.InterestCommittee!.GetText()));
            Assert.That(dataRow[8].Text, Is.EqualTo(membership.Person!.Interests.LastOrDefault()?.InterestFunction!.GetText()));
            Assert.That(dataRow[9].Text, Is.EqualTo(committee.CommitteeType!.GetText()));
            Assert.That(dataRow[10].Text, Is.EqualTo(committee.Department!.GetDescription()));
            Assert.That(dataRow[11].Text, Is.EqualTo(committee.Office!.GetDescription()));
            Assert.That(dataRow[12].Text, Is.EqualTo(committee.CommitteeLevel!.GetText()));
            Assert.That(dataRow[13].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // ExtraParliamentary Commission
            Assert.That(dataRow[14].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // Supervision Duty
            Assert.That(dataRow[15].Text, Is.EqualTo(committee.TermOfOffice!.GetDescription()));
            Assert.That(dataRow[16].Text, Is.EqualTo(committee.BeginDate.ToString("O", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[17].Text, Is.EqualTo(committee.EndDate?.ToString("O", CultureInfo.InvariantCulture)));
        });
    }

    [Test]
    public async Task GenerateMembershipInterestExport_WithNoInterest_ShouldExportDataCorrectly()
    {
        var dataAnalysisDate = new DateOnly(2024, 1, 1);

        var interests = new List<Interest>();

        var committee = new CommitteeBuilder()
            .WithBeginDate(new DateOnly(2020, 1, 1))
            .WithEndDate(new DateOnly(2025, 1, 1))
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .WithSupervisionDuty(true)
            .Build();
        var person = new PersonBuilder()
            .WithBirthYear(2000)
            .WithInterests(interests)
            .Build();
        var membership = new MembershipBuilder()
            .WithIsActive(true)
            .WithPerson(person)
            .Build();
        committee.Memberships.Add(membership);

        _committeeRepository.GetCommitteesWithInterestsForExport(dataAnalysisDate, _zeroGuid, _zeroGuid, _zeroGuid).Returns([committee]);

        _configuration["FrontendUrl"].Returns("FooBar");

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _service.GenerateMembershipInterestExport(dataAnalysisDate);

        Assert.That(capturedSpreadsheet, Is.Not.Null);

        var dataRow = capturedSpreadsheet.BodyCells[0];
        Assert.Multiple(() =>
        {
            Assert.That(dataRow[0].Text, Is.EqualTo(committee.GetDescription()));
            Assert.That(dataRow[1].Text, Is.EqualTo(membership.Person!.Surname));
            Assert.That(dataRow[1].Hyperlink, Is.EqualTo($"FooBar/persons/{membership.PersonId}"));
            Assert.That(dataRow[2].Text, Is.EqualTo(membership.Person!.GivenName));
            Assert.That(dataRow[3].Text, Is.EqualTo(membership.Person!.BirthYear.ToString(CultureInfo.InvariantCulture)));
            Assert.That(dataRow[4].Text, Is.EqualTo(string.Empty));
            Assert.That(dataRow[5].Text, Is.EqualTo(string.Empty));
            Assert.That(dataRow[6].Text, Is.EqualTo(string.Empty));
            Assert.That(dataRow[7].Text, Is.EqualTo(string.Empty));
            Assert.That(dataRow[8].Text, Is.EqualTo(string.Empty));
            Assert.That(dataRow[9].Text, Is.EqualTo(committee.CommitteeType!.GetText()));
            Assert.That(dataRow[10].Text, Is.EqualTo(committee.Department!.GetDescription()));
            Assert.That(dataRow[11].Text, Is.EqualTo(committee.Office!.GetDescription()));
            Assert.That(dataRow[12].Text, Is.EqualTo(committee.CommitteeLevel!.GetText()));
            Assert.That(dataRow[13].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // ExtraParliamentary Commission
            Assert.That(dataRow[14].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // Supervision Duty
            Assert.That(dataRow[15].Text, Is.EqualTo(committee.TermOfOffice!.GetDescription()));
            Assert.That(dataRow[16].Text, Is.EqualTo(committee.BeginDate.ToString("O", CultureInfo.InvariantCulture)));
            Assert.That(dataRow[17].Text, Is.EqualTo(committee.EndDate?.ToString("O", CultureInfo.InvariantCulture)));
        });
    }

    [Test]
    public async Task GeneratePersonExport_ShouldExportPersonCorrectly()
    {
        var zeroGuid = Guid.Empty;

        var dataAnalysisDate = new DateOnly(2024, 1, 1);
        var person = new PersonBuilder()
            .WithGender(new GenderBuilder().Build())
            .WithLanguage(new LanguageBuilder().Build())
            .WithBirthYear(2000)
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithMemberships([new MembershipBuilder().WithIsActive(true).WithEmploymentLevel(30).WithCommittee(new CommitteeBuilder().WithGermanDescription("Gremium DE").Build()).Build()])
            .Build();
        _personRepository.GetPersonsForExport(dataAnalysisDate, zeroGuid, zeroGuid, zeroGuid).Returns([person]);

        _configuration["FrontendUrl"].Returns("FooBar");

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _service.GeneratePersonExport(dataAnalysisDate);

        Assert.That(capturedSpreadsheet, Is.Not.Null);

        var dataRow = capturedSpreadsheet.BodyCells[0];
        Assert.Multiple(() =>
        {
            Assert.That(dataRow[0].Text, Is.EqualTo(person.Surname));
            Assert.That(dataRow[0].Hyperlink, Is.EqualTo($"FooBar/persons/{person.Id}"));
            Assert.That(dataRow[1].Text, Is.EqualTo(person.GivenName));
            Assert.That(dataRow[2].Text, Is.EqualTo(person.CorrespondenceAddress!.City));
            Assert.That(dataRow[3].Text, Is.EqualTo(person.BirthYear.ToString(CultureInfo.InvariantCulture)));
            Assert.That(dataRow[4].Text, Is.EqualTo((DateTime.UtcNow.Year - person.BirthYear).ToString(CultureInfo.InvariantCulture)));
            Assert.That(dataRow[5].Text, Is.EqualTo(person.Language!.GetText()));
            Assert.That(dataRow[6].Text, Is.EqualTo(person.Gender!.GetText()));
            Assert.That(dataRow[7].Text, Is.EqualTo(person.Occupation));
            Assert.That(dataRow[8].Text, Is.EqualTo("1"));
            Assert.That(dataRow[9].Text, Is.EqualTo("30"));
            Assert.That(dataRow[10].Text, Is.EqualTo(person.Memberships.FirstOrDefault()!.Committee!.GetDescription()));
        });

        var tableOptions = capturedSpreadsheet.TableOptions;
        Assert.That(tableOptions, Is.Not.Null);
        Assert.That(tableOptions.TableRange, Is.EqualTo("A1:K2"));
    }

    [Test]
    public async Task GenerateContactPointExport_WithMultipleSecretariatsAndReleasedPersonalData_ShouldExportDataCorrectly()
    {
        var dataAnalysisDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));

        var contactPointTypeId = Guid.NewGuid();

        var secretariat = new ContactPointBuilder()
            .WithContactPointType(new ContactPointTypeBuilder().WithId(contactPointTypeId).Build())
            .WithEmail("test@test.ch")
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-360)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(360)))
            .WithCompanyName("Test AG")
            .WithSection("Sektion Funktionstests")
            .WithPhone("+41 44 666 77 88")
            .WithSurnameAndGivenName("Tester", "Hansjörg")
            .WithTitle("Cheftester")
            .WithPersonalEmail("juere.tester@test.ch")
            .WithPersonalMobile("+41 78 666 77 88")
            .WithPersonalPhone("+41 44 666 77 88")
            .WithReleasePersonData(true)
            .Build();

        var secretariat2 = new ContactPointBuilder()
            .WithContactPointType(new ContactPointTypeBuilder().WithId(contactPointTypeId).Build())
            .WithEmail("test@testerei.ch")
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-360)))
            .WithEndDate(null)
            .WithCompanyName("Testerei AG")
            .WithSection(string.Empty)
            .WithPhone("+41 44 666 77 88")
            .WithSurnameAndGivenName(string.Empty, string.Empty)
            .WithTitle(string.Empty)
            .WithPersonalEmail(string.Empty)
            .WithPersonalMobile(string.Empty)
            .WithPersonalPhone(string.Empty)
            .WithReleasePersonData(true)
            .Build();

        var committee = new CommitteeBuilder()
            .WithBeginDate(new DateOnly(2020, 1, 1))
            .WithEndDate(new DateOnly(2026, 1, 1))
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .WithSupervisionDuty(true)
            .Build();

        committee.ContactPoints.Add(secretariat);
        committee.ContactPoints.Add(secretariat2);

        _committeeRepository.GetCommitteesWithContactPointsForExport(dataAnalysisDate, _zeroGuid, _zeroGuid, _zeroGuid).Returns([committee]);

        _configuration["FrontendUrl"].Returns("FooBar");

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _service.GenerateContactPointExport(dataAnalysisDate, contactPointTypeId);

        Assert.That(capturedSpreadsheet, Is.Not.Null);

        var dataRow = capturedSpreadsheet.BodyCells[0];
        Assert.Multiple(() =>
        {
            Assert.That(dataRow[0].Text, Is.EqualTo(committee.GetDescription()));
            Assert.That(dataRow[1].Text, Is.EqualTo("Test AG, Sektion Funktionstests"));
            Assert.That(dataRow[2].Text, Is.EqualTo("+41 44 666 77 88"));
            Assert.That(dataRow[3].Text, Is.EqualTo("Cheftester Tester Hansjörg"));
            Assert.That(dataRow[4].Text, Is.EqualTo("+41 44 666 77 88 / +41 78 666 77 88"));
            Assert.That(dataRow[5].Text, Is.EqualTo("test@test.ch;juere.tester@test.ch"));
            Assert.That(dataRow[6].Text, Is.EqualTo(committee.CommitteeType!.GetText()));
            Assert.That(dataRow[7].Text, Is.EqualTo(committee.Department!.GetDescription()));
            Assert.That(dataRow[8].Text, Is.EqualTo(committee.Office!.GetDescription()));
            Assert.That(dataRow[9].Text, Is.EqualTo(committee.CommitteeLevel!.GetText()));
            Assert.That(dataRow[10].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // ExtraParliamentary Commission
            Assert.That(dataRow[11].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // Supervision Duty
            Assert.That(dataRow[12].Text, Is.EqualTo(committee.TermOfOffice!.GetDescription()));
        });

        dataRow = capturedSpreadsheet.BodyCells[1];
        Assert.Multiple(() =>
        {
            Assert.That(dataRow[0].Text, Is.EqualTo(committee.GetDescription()));
            Assert.That(dataRow[1].Text, Is.EqualTo("Testerei AG"));
            Assert.That(dataRow[2].Text, Is.EqualTo("+41 44 666 77 88"));
            Assert.That(dataRow[3].Text, Is.EqualTo(string.Empty));
            Assert.That(dataRow[4].Text, Is.EqualTo(string.Empty));
            Assert.That(dataRow[5].Text, Is.EqualTo("test@testerei.ch"));
            Assert.That(dataRow[6].Text, Is.EqualTo(committee.CommitteeType!.GetText()));
            Assert.That(dataRow[7].Text, Is.EqualTo(committee.Department!.GetDescription()));
            Assert.That(dataRow[8].Text, Is.EqualTo(committee.Office!.GetDescription()));
            Assert.That(dataRow[9].Text, Is.EqualTo(committee.CommitteeLevel!.GetText()));
            Assert.That(dataRow[10].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // ExtraParliamentary Commission
            Assert.That(dataRow[11].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // Supervision Duty
            Assert.That(dataRow[12].Text, Is.EqualTo(committee.TermOfOffice!.GetDescription()));
        });
    }

    [Test]
    public async Task GenerateContactPointExport_WithMultipleSecretariatsAndNotReleasedPersonalData_ShouldExportDataCorrectly()
    {
        var dataAnalysisDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));

        var contactPointTypeId = Guid.NewGuid();

        var secretariat = new ContactPointBuilder()
            .WithContactPointType(new ContactPointTypeBuilder().WithId(contactPointTypeId).Build())
            .WithEmail("test@test.ch")
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-360)))
            .WithEndDate(null)
            .WithCompanyName("Test AG")
            .WithSection("Sektion Funktionstests")
            .WithPhone("+41 44 666 77 88")
            .WithSurnameAndGivenName("Tester", "Hansjörg")
            .WithTitle("Cheftester")
            .WithPersonalEmail("juere.tester@test.ch")
            .WithPersonalMobile("+41 78 666 77 88")
            .WithPersonalPhone("+41 44 666 77 88")
            .WithReleasePersonData(false)
            .Build();

        var committee = new CommitteeBuilder()
            .WithBeginDate(new DateOnly(2020, 1, 1))
            .WithEndDate(new DateOnly(2026, 1, 1))
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .WithSupervisionDuty(true)
            .Build();

        committee.ContactPoints.Add(secretariat);

        _committeeRepository.GetCommitteesWithContactPointsForExport(dataAnalysisDate, _zeroGuid, _zeroGuid, _zeroGuid).Returns([committee]);

        _configuration["FrontendUrl"].Returns("FooBar");

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _service.GenerateContactPointExport(dataAnalysisDate, contactPointTypeId);

        Assert.That(capturedSpreadsheet, Is.Not.Null);

        var dataRow = capturedSpreadsheet.BodyCells[0];
        Assert.Multiple(() =>
        {
            Assert.That(dataRow[0].Text, Is.EqualTo(committee.GetDescription()));
            Assert.That(dataRow[1].Text, Is.EqualTo("Test AG, Sektion Funktionstests"));
            Assert.That(dataRow[2].Text, Is.EqualTo("+41 44 666 77 88"));
            Assert.That(dataRow[3].Text, Is.EqualTo(string.Empty));
            Assert.That(dataRow[4].Text, Is.EqualTo(string.Empty));
            Assert.That(dataRow[5].Text, Is.EqualTo("test@test.ch"));
            Assert.That(dataRow[6].Text, Is.EqualTo(committee.CommitteeType!.GetText()));
            Assert.That(dataRow[7].Text, Is.EqualTo(committee.Department!.GetDescription()));
            Assert.That(dataRow[8].Text, Is.EqualTo(committee.Office!.GetDescription()));
            Assert.That(dataRow[9].Text, Is.EqualTo(committee.CommitteeLevel!.GetText()));
            Assert.That(dataRow[10].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // ExtraParliamentary Commission
            Assert.That(dataRow[11].Text, Is.EqualTo(BusinessTexts.Common_Yes)); // Supervision Duty
            Assert.That(dataRow[12].Text, Is.EqualTo(committee.TermOfOffice!.GetDescription()));
        });
    }

    [Test]
    public async Task GenerateRegionExport_ShouldExportDataCorrectly()
    {
        var dataAnalysisDate = new DateOnly(2024, 1, 1);
        var departments = new[]
        {
            new DepartmentBuilder().Build(),
            new DepartmentBuilder().Build()
        };

        var cantons = new[]
        {
            new CantonBuilder().Build(),
            new CantonBuilder().Build()
        }.OrderBy(x => x.Sort).ToArray();

        var committees = new[]
        {
            new CommitteeBuilder()
                .WithDepartment(departments[0])
                .WithMembership(CreateActiveMembership(cantons[0]))
                .WithMembership(CreateActiveMembership(cantons[0]))
                .WithMembership(CreateInactiveMembership(cantons[0]))
                .Build(),

            new CommitteeBuilder()
                .WithDepartment(departments[1])
                .WithMembership(CreateActiveMembership(cantons[0]))
                .WithMembership(CreateActiveMembership(cantons[1]))
                .WithMembership(CreateActiveMembership(cantons[1]))
                .WithMembership(CreateActiveMembership(cantons[1]))
                .WithMembership(CreateDeletedMembership(cantons[1]))
                .Build()
        };

        _masterDataRepository.GetDepartments().Returns(departments);
        _masterDataRepository.GetCantons().Returns(cantons);
        _committeeRepository.GetCommitteesForRegionExport(dataAnalysisDate, _zeroGuid, _zeroGuid, _zeroGuid).Returns(committees);

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _service.GenerateRegionExport(dataAnalysisDate);

        Assert.That(capturedSpreadsheet, Is.Not.Null);
        Assert.That(capturedSpreadsheet!.BodyCells, Has.Count.EqualTo(3));

        var canton1Row = capturedSpreadsheet.BodyCells[0];
        Assert.Multiple(() =>
        {
            Assert.That(canton1Row[0].Text, Is.EqualTo(cantons[0].GetText()));
            Assert.That(canton1Row[1].Text, Is.EqualTo("2"));
            Assert.That(canton1Row[2].Text, Is.EqualTo("1"));
        });

        var canton2Row = capturedSpreadsheet.BodyCells[1];
        Assert.Multiple(() =>
        {
            Assert.That(canton2Row[0].Text, Is.EqualTo(cantons[1].GetText()));
            Assert.That(canton2Row[1].Text, Is.EqualTo("0"));
            Assert.That(canton2Row[2].Text, Is.EqualTo("3"));
        });

        var totalRow = capturedSpreadsheet.BodyCells[2];
        Assert.Multiple(() =>
        {
            Assert.That(totalRow[1].Text, Is.EqualTo("2"));
            Assert.That(totalRow[2].Text, Is.EqualTo("4"));
        });
    }

    private static Membership CreateActiveMembership(Canton canton)
    {
        return new MembershipBuilder()
            .WithIsActive(true)
            .WithIsDeleted(false)
            .WithPerson(new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder()
                    .WithCanton(canton)
                    .Build())
                .Build())
            .Build();
    }

    private static Membership CreateInactiveMembership(Canton canton)
    {
        return new MembershipBuilder()
            .WithIsActive(false)
            .WithIsDeleted(false)
            .WithPerson(new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder()
                    .WithCanton(canton)
                    .Build())
                .Build())
            .Build();
    }

    private static Membership CreateDeletedMembership(Canton canton)
    {
        return new MembershipBuilder()
            .WithIsActive(true)
            .WithIsDeleted(true)
            .WithPerson(new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder()
                    .WithCanton(canton)
                    .Build())
                .Build())
            .Build();
    }

    [Test]
    public async Task GenerateAgeExport_ShouldGenerateAgeExportCorrectly()
    {
        var dataAnalysisDate = new DateOnly(2024, 1, 1);
        var committee = new CommitteeBuilder()
            .WithBeginDate(new DateOnly(2020, 1, 1))
            .WithEndDate(new DateOnly(2025, 1, 1))
            .WithCommitteeType(new CommitteeTypeBuilder().WithId(CommitteeType.AdministrationCommissionGuid).Build())
            .WithSupervisionDuty(true)
            .Build();

        var membership1 = new MembershipBuilder()
            .WithIsActive(true)
            .WithFunction(new FunctionBuilder().WithUri(Function.PresidentUri).Build())
            .WithPerson(new PersonBuilder()
                .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
                .WithLanguage(new LanguageBuilder().WithUri(Language.ItalianUri).Build())
                .WithFederalDuty(true)
                .WithFederalAssembly(true)
                .WithOffice(new OfficeBuilder().WithIsCentralFederalAdministration(true).Build())
                .WithBirthYear(2000)
                .Build())
            .Build();

        var membership2 = new MembershipBuilder()
                .WithIsActive(true)
                .WithPerson(new PersonBuilder()
                    .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
                    .WithLanguage(new LanguageBuilder().WithUri(Language.GermanUri).Build())
                    .WithOffice(new OfficeBuilder().WithIsCentralFederalAdministration(false).Build())
                    .WithFederalDuty(false)
                    .WithFederalAssembly(false)
                    .WithBirthYear(2000)
                    .Build())
                .Build();

        committee.Memberships.Add(membership1);
        committee.Memberships.Add(membership2);

        _committeeRepository
            .GetCommitteesForExport(dataAnalysisDate, _zeroGuid, _zeroGuid, _zeroGuid)
            .Returns([committee]);

        Spreadsheet? capturedSpreadsheet = null;
        _documentService
            .CreateExcel(Arg.Do<Spreadsheet>(spreadsheet => capturedSpreadsheet = spreadsheet), Arg.Any<SpreadsheetOptions?>())
            .Returns(new MemoryStream());

        await _service.GenerateAgeExport(dataAnalysisDate);

        Assert.That(capturedSpreadsheet, Is.Not.Null);

        var dataRow = capturedSpreadsheet.BodyCells[0];
        Assert.Multiple(() =>
        {
            Assert.That(dataRow[0].Text, Is.EqualTo(committee.Memberships.First().Person!.Age.ToString(CultureInfo.InvariantCulture)));
            Assert.That(dataRow[1].Text, Is.EqualTo("2")); // Active Member Count
            Assert.That(dataRow[2].Text, Is.EqualTo("1")); // Female President Count
            Assert.That(dataRow[3].Text, Is.EqualTo("1")); // Female President Percentage (1/1)
            Assert.That(dataRow[4].Text, Is.EqualTo("0")); // Male President Count
            Assert.That(dataRow[5].Text, Is.EqualTo("0")); // Male President Percentage (1/1)
            Assert.That(dataRow[6].Text, Is.EqualTo("1")); // Woman Count
            Assert.That(dataRow[7].Text, Is.EqualTo("0.5")); // Woman Percentage (1/2)
            Assert.That(dataRow[8].Text, Is.EqualTo("1")); // Man Count
            Assert.That(dataRow[9].Text, Is.EqualTo("0.5")); // Man Percentage (1/2)
            Assert.That(dataRow[10].Text, Is.EqualTo("1")); // Italian Count
            Assert.That(dataRow[11].Text, Is.EqualTo("0.5")); // Italian Percentage
            Assert.That(dataRow[12].Text, Is.EqualTo("1")); // German Count
            Assert.That(dataRow[13].Text, Is.EqualTo("0.5")); // German Percentage
            Assert.That(dataRow[14].Text, Is.EqualTo("0")); // French Count
            Assert.That(dataRow[15].Text, Is.EqualTo("0")); // French Percentage
            Assert.That(dataRow[16].Text, Is.EqualTo("0")); // Romansh Count
            Assert.That(dataRow[17].Text, Is.EqualTo("0")); // Romansh Percentage
            Assert.That(dataRow[18].Text, Is.EqualTo("1")); // Federal Duty Count
            Assert.That(dataRow[19].Text, Is.EqualTo("0.5")); // Federal Duty Percentage (1/2)
            Assert.That(dataRow[20].Text, Is.EqualTo("1")); // Not Federal Duty Count
            Assert.That(dataRow[21].Text, Is.EqualTo("0.5")); // Not Federal Duty Percentage (1/2)
            Assert.That(dataRow[22].Text, Is.EqualTo("1")); // Central Federal Duty Count
            Assert.That(dataRow[23].Text, Is.EqualTo("0.5")); // Central Federal Duty Percentage - 2/4 = 50%
            Assert.That(dataRow[24].Text, Is.EqualTo("1")); // Decentralized Federal Duty Count
            Assert.That(dataRow[25].Text, Is.EqualTo("0.5")); // Decentralized Federal Duty Percentage - 2/4 = 50%
            Assert.That(dataRow[26].Text, Is.EqualTo("1")); // Federal Assembly Count
            Assert.That(dataRow[27].Text, Is.EqualTo("0.5")); // Federal Assembly Percentage (1/2)
            Assert.That(dataRow[28].Text, Is.EqualTo("1")); // Not Federal Assembly Count
            Assert.That(dataRow[29].Text, Is.EqualTo("0.5")); // Not Federal Assembly Percentage (1/2)
        });

        var tableOptions = capturedSpreadsheet.TableOptions;
        Assert.That(tableOptions, Is.Not.Null);
        Assert.That(tableOptions.TableRange, Is.EqualTo("A1:AD2"));
    }
}
