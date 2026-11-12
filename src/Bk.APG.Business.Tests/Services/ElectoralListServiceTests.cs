using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.Common.Resources;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class ElectoralListServiceTests
{
    private readonly Swiss.FCh.DocumentService.Client.IDocumentService _documentService = Substitute.For<Swiss.FCh.DocumentService.Client.IDocumentService>();
    private readonly IEiamAssignmentService _eiamAssignmentService = Substitute.For<IEiamAssignmentService>();
    private readonly ICommitteeRepository _committeeRepository = Substitute.For<ICommitteeRepository>();
    private readonly IMasterDataRepository _masterDataRepository = Substitute.For<IMasterDataRepository>();
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository = Substitute.For<IGeneralElectionCommitteeRepository>();

    private ElectoralListService _electoralListService = null!;

    private ReportFilterParametersDto _filterDto = null!;

    private readonly Guid _zeroGuid = Guid.Empty;

    [SetUp]
    public void SetUp()
    {
        _electoralListService = new ElectoralListService(_documentService, _eiamAssignmentService, _committeeRepository, _masterDataRepository, _generalElectionCommitteeRepository, NullLogger<ElectoralListService>.Instance);

        _filterDto = new ReportFilterParametersDto { DocumentType = ReportType.ElectoralListFC, AnalysisDate1 = DateOnly.FromDateTime(DateTime.Today), CommitteesWithActiveMembership = false, ReleasedCommittees = false };
    }

    [TearDown]
    public void TearDown()
    {
        _documentService.ClearSubstitute();
        _committeeRepository.ClearSubstitute();
    }

    [Test]
    public async Task GenerateDocument_WithValidData_ShouldReturnFileNameAndContent()
    {
        var evaluationDate = DateTime.Today;
        const string listType = "ElectoralList_FederalCouncil";
        var committeeDetail = CreateGeneralElectionCommittee();
        using var documentStream = new MemoryStream();

        _generalElectionCommitteeRepository.GetByFilterForReport(_zeroGuid, _zeroGuid, _zeroGuid, _filterDto).Returns([committeeDetail]);
        _documentService.CreateWordFromTemplate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<string>()).Returns(documentStream);

        var (fileName, content) = await _electoralListService.GenerateDocument(_filterDto, listType);

        await _generalElectionCommitteeRepository.Received(1).GetByFilterForReport(_zeroGuid, _zeroGuid, _zeroGuid, _filterDto);

        await _documentService.Received(1).CreateWordFromTemplate(
            $"Templates/{listType}.docx",
            Arg.Any<object>(),
            "electoralList"
        );

        Assert.Multiple(() =>
        {
            Assert.That(fileName, Is.Not.Null);
            Assert.That(content, Is.EqualTo(documentStream));
        });
        Assert.That(fileName, Does.Contain(evaluationDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture)));
        Assert.That(fileName, Does.Contain(BusinessTexts.ElectoralList_ElectoralList));
        Assert.That(fileName, Does.Contain(BusinessTexts.ResourceManager.GetString(listType)));
        Assert.That(fileName, Does.EndWith(".docx"));
    }

    [Test]
    public async Task GenerateDocument_WithPersonHavingInterests_ShouldFormatInterestsCorrectly()
    {
        var committeeDetail = CreateGeneralElectionCommittee();
        var person = committeeDetail.MembershipCandidates.First().Person!;
        person.NoInterest = false;
        var interest = new InterestBuilder()
            .WithLegalForm(new LegalFormBuilder().Build())
            .WithInterestCommittee(new InterestCommitteeBuilder().Build())
            .Build();
        person.Interests = [interest];
        using var documentStream = new MemoryStream();

        _generalElectionCommitteeRepository.GetByFilterForReport(_zeroGuid, _zeroGuid, _zeroGuid, _filterDto).Returns([committeeDetail]);
        _documentService.CreateWordFromTemplate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<string>()).Returns(documentStream);

        ElectoralListDto? capturedData = null!;
        await _documentService.CreateWordFromTemplate(Arg.Any<string>(), Arg.Do<object>(o => capturedData = o as ElectoralListDto), Arg.Any<string>());

        await _electoralListService.GenerateDocument(_filterDto, "ElectoralList_FederalCouncil");

        var member = capturedData.Departments.First().Committees.First().Functions.First().Members.First();
        var interests = member.Interests;
        Assert.That(interests, Is.EqualTo($"[ {interest.Text} | {interest.LegalForm!.GetText()} | {interest.InterestCommittee!.GetText()} | - ]"));
    }

    [Test]
    public async Task GenerateDocument_WithPersonHavingNoInterests_ShouldReturnNoText()
    {
        var committeeDetail = CreateGeneralElectionCommittee();
        var person = committeeDetail.MembershipCandidates.First().Person!;
        person.NoInterest = true;
        person.Interests = new List<Interest>();
        using var documentStream = new MemoryStream();

        _generalElectionCommitteeRepository.GetByFilterForReport(_zeroGuid, _zeroGuid, _zeroGuid, _filterDto).Returns([committeeDetail]);
        _documentService.CreateWordFromTemplate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<string>()).Returns(documentStream);

        ElectoralListDto? capturedData = null!;
        await _documentService.CreateWordFromTemplate(Arg.Any<string>(), Arg.Do<object>(o => capturedData = o as ElectoralListDto), Arg.Any<string>());

        await _electoralListService.GenerateDocument(_filterDto, "ElectoralList_FederalCouncil");

        var member = capturedData.Departments.First().Committees.First().Functions.First().Members.First();
        Assert.That(member.Interests, Is.EqualTo(BusinessTexts.Common_None));
    }

    [Test]
    public async Task GenerateDocument_WithOtherMemberships_ShouldIncludeOtherMemberships()
    {
        var committeeDetail = CreateGeneralElectionCommittee();

        var personId = committeeDetail.MembershipCandidates.First().PersonId;
        var secondCommittee = CreateCommittee(personId, Guid.NewGuid());

        _committeeRepository.GetAll().Returns(new List<Committee> { secondCommittee });

        _generalElectionCommitteeRepository.GetByFilterForReport(_zeroGuid, _zeroGuid, _zeroGuid, _filterDto).Returns([committeeDetail]);
        using var documentStream = new MemoryStream();
        _documentService.CreateWordFromTemplate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<string>()).Returns(documentStream);

        ElectoralListDto? capturedData = null!;
        await _documentService.CreateWordFromTemplate(Arg.Any<string>(), Arg.Do<object>(o => capturedData = o as ElectoralListDto), Arg.Any<string>());

        await _electoralListService.GenerateDocument(_filterDto, "ElectoralList_FederalCouncil");

        var member = capturedData.Departments.First().Committees.First().Functions.First().Members.First();
        Assert.That(member.OtherMemberships, Is.EqualTo($"[ {secondCommittee.GetDescription()} | {secondCommittee.CommitteeType!.GetText()} | {secondCommittee.Memberships.First().Function!.GetText()} ]"));
    }

    [Test]
    public async Task GenerateDocument_WithOtherGeneralElectionMembership_ShouldIncludeOtherGeneralElectionCommittee()
    {
        var personId = Guid.NewGuid();
        var function = new FunctionBuilder().WithGermanText("Mitglied").Build();
        var currentCommittee = CreateGeneralElectionCommittee(personId, function: function);
        var otherCommittee = CreateGeneralElectionCommittee(personId, function: function);

        var members = await GenerateAndGetMembers(currentCommittee, otherCommittee);

        var expectedForCurrentCommittee = FormatOtherMembership(otherCommittee, function);
        var expectedForOtherCommittee = FormatOtherMembership(currentCommittee, function);
        Assert.That(members.Select(member => member.OtherMemberships), Is.EquivalentTo(new[] { expectedForCurrentCommittee, expectedForOtherCommittee }));
    }

    [Test]
    public async Task GenerateDocument_WithMultipleCandidatesInOtherGeneralElectionCommittee_ShouldListCommitteeOnlyOnce()
    {
        var personId = Guid.NewGuid();
        var functionA = new FunctionBuilder().WithGermanText("A").Build();
        var functionB = new FunctionBuilder().WithGermanText("B").Build();
        var currentCommittee = CreateGeneralElectionCommittee(personId, function: functionA);
        var otherCommittee = CreateGeneralElectionCommittee(personId, function: functionA);
        otherCommittee.MembershipCandidates.Add(new MembershipCandidateBuilder()
            .WithPerson(CreatePerson(personId))
            .WithFunction(functionB)
            .WithIsSelected(true)
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
            .Build());

        var members = await GenerateAndGetMembers(currentCommittee, otherCommittee);

        var memberOfCurrentCommittee = members.Single(member => member.OtherMemberships!.Contains(otherCommittee.GetDescription(), StringComparison.Ordinal));
        Assert.That(CountOccurrences(memberOfCurrentCommittee.OtherMemberships!, otherCommittee.GetDescription()), Is.EqualTo(1));
    }

    [Test]
    public async Task GenerateDocument_WithInactiveGeneralElectionMembership_ShouldExcludeIt()
    {
        var personId = Guid.NewGuid();
        var currentCommittee = CreateGeneralElectionCommittee(personId);
        var inactiveCommittee = CreateGeneralElectionCommittee(
            personId,
            beginDate: DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            endDate: DateOnly.FromDateTime(DateTime.Today.AddDays(-5)));

        var members = await GenerateAndGetMembers(currentCommittee, inactiveCommittee);

        Assert.That(members.Single().OtherMemberships, Is.Null);
    }

    [Test]
    public async Task GenerateDocument_WithGeneralElectionMembershipWithoutPerson_ShouldExcludeIt()
    {
        var personId = Guid.NewGuid();
        var currentCommittee = CreateGeneralElectionCommittee(personId);
        var otherCommittee = CreateGeneralElectionCommittee(personId);
        var personlessCandidate = otherCommittee.MembershipCandidates.First();
        personlessCandidate.Person = null;
        personlessCandidate.PersonId = null;

        var members = await GenerateAndGetMembers(currentCommittee, otherCommittee);

        Assert.That(members.Single().OtherMemberships, Is.Null);
    }

    [Test]
    public async Task GenerateDocument_WithNonGeneralElectionAndGeneralElectionMemberships_ShouldCombineThem()
    {
        var personId = Guid.NewGuid();
        var function = new FunctionBuilder().WithGermanText("Mitglied").Build();
        var currentCommittee = CreateGeneralElectionCommittee(personId, function: function);
        var otherGeneralElectionCommittee = CreateGeneralElectionCommittee(personId, function: function);
        var nonGeneralElectionCommittee = CreateCommittee(personId, Guid.NewGuid());
        _committeeRepository.GetAll().Returns(new List<Committee> { nonGeneralElectionCommittee });

        var members = await GenerateAndGetMembers(currentCommittee, otherGeneralElectionCommittee);

        var nonGeneralElectionText = FormatOtherMembership(nonGeneralElectionCommittee, nonGeneralElectionCommittee.Memberships.First().Function!);
        var otherGeneralElectionText = FormatOtherMembership(otherGeneralElectionCommittee, function);
        var expected = string.Join($"{Environment.NewLine}", new[] { nonGeneralElectionText, otherGeneralElectionText }.OrderBy(text => text));
        Assert.That(members.Select(member => member.OtherMemberships), Does.Contain(expected));
    }

    [Test]
    public async Task GenerateDocument_WithSelfOrganizedGeneralElectionCommittee_ShouldUseMemberFunction()
    {
        var personId = Guid.NewGuid();
        var memberFunction = new FunctionBuilder().WithId(Function.MemberGuid).WithGermanText("Mitglied").Build();
        var currentCommittee = CreateGeneralElectionCommittee(personId);
        var selfOrganizedCommittee = CreateGeneralElectionCommittee(personId, selfOrganized: true);
        _masterDataRepository.GetById<Function>(Function.MemberGuid).Returns(memberFunction);

        var members = await GenerateAndGetMembers(currentCommittee, selfOrganizedCommittee);

        var memberOfCurrentCommittee = members.Single(member => member.OtherMemberships!.Contains(selfOrganizedCommittee.GetDescription(), StringComparison.Ordinal));
        Assert.That(memberOfCurrentCommittee.OtherMemberships, Does.Contain(memberFunction.GetText()));
    }

    private async Task<List<ElectoralListMemberDto>> GenerateAndGetMembers(params GeneralElectionCommittee[] generalElectionCommittees)
    {
        _generalElectionCommitteeRepository.GetByFilterForReport(_zeroGuid, _zeroGuid, _zeroGuid, _filterDto).Returns(generalElectionCommittees);
        using var documentStream = new MemoryStream();
        _documentService.CreateWordFromTemplate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<string>()).Returns(documentStream);

        ElectoralListDto? capturedData = null!;
        await _documentService.CreateWordFromTemplate(Arg.Any<string>(), Arg.Do<object>(captured => capturedData = captured as ElectoralListDto), Arg.Any<string>());

        await _electoralListService.GenerateDocument(_filterDto, "ElectoralList_FederalCouncil");

        return capturedData.Departments
            .SelectMany(department => department.Committees)
            .SelectMany(committee => committee.Functions)
            .SelectMany(function => function.Members)
            .ToList();
    }

    private static string FormatOtherMembership(GeneralElectionCommittee committee, Function function)
    {
        return $"[ {committee.GetDescription()} | {committee.CommitteeType!.GetText()} | {function.GetText()} ]";
    }

    private static string FormatOtherMembership(Committee committee, Function function)
    {
        return $"[ {committee.GetDescription()} | {committee.CommitteeType!.GetText()} | {function.GetText()} ]";
    }

    private static int CountOccurrences(string text, string value)
    {
        return text.Split(value).Length - 1;
    }

    private static GeneralElectionCommittee CreateGeneralElectionCommittee(
        Guid? personId = null,
        Guid? termOfOfficeId = null,
        Function? function = null,
        bool isSelected = true,
        DateOnly? beginDate = null,
        DateOnly? endDate = null,
        bool selfOrganized = false)
    {
        personId ??= Guid.NewGuid();
        termOfOfficeId ??= TermOfOffice.Period4YearsInGeneralElectionGuid;

        var membershipCandidateBuilder = new MembershipCandidateBuilder()
            .WithPerson(CreatePerson(personId.Value))
            .WithIsSelected(isSelected)
            .WithBeginDate(beginDate ?? DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
            .WithEndDate(endDate ?? DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        if (function is not null)
        {
            membershipCandidateBuilder.WithFunction(function);
        }

        var description = $"Kommission-{Guid.NewGuid():N}";
        var committee = new GeneralElectionCommitteeBuilder()
            .WithGermanDescription(description)
            .WithFrenchDescription(description)
            .WithItalianDescription(description)
            .WithRomanschDescription(description)
            .WithTermOfOfficeId(termOfOfficeId.Value)
            .WithCommitteeLevelId(CommitteeLevel.FederalCouncilGuid)
            .WithMembershipCandidate(membershipCandidateBuilder.Build())
            .Build();

        committee.SelfOrganized = selfOrganized;

        return committee;
    }

    private static Committee CreateCommittee(Guid? personId = null, Guid? termOfOfficeId = null)
    {
        personId ??= Guid.NewGuid();
        termOfOfficeId ??= TermOfOffice.Period4YearsInGeneralElectionGuid;

        var membership = new MembershipBuilder()
            .WithPerson(CreatePerson(personId.Value))
            .Build();

        var description = $"Kommission-{Guid.NewGuid():N}";
        var committee = new CommitteeBuilder()
            .WithGermanDescription(description)
            .WithFrenchDescription(description)
            .WithItalianDescription(description)
            .WithTermOfOfficeId(termOfOfficeId.Value)
            .WithCommitteeLevelId(CommitteeLevel.FederalCouncilGuid)
            .WithMembership(membership)
            .Build();

        membership.Committee = committee;
        membership.CommitteeId = committee.Id;

        return committee;
    }

    private static Person CreatePerson(Guid personId)
    {
        return new PersonBuilder()
            .WithId(personId)
            .WithLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .Build();
    }
}
