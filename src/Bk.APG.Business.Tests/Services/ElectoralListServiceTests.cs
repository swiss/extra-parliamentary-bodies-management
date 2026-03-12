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
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository = Substitute.For<IGeneralElectionCommitteeRepository>();

    private ElectoralListService _electoralListService = null!;

    private ReportFilterParametersDto _filterDto = null!;

    private readonly Guid _zeroGuid = Guid.Empty;

    [SetUp]
    public void SetUp()
    {
        _electoralListService = new ElectoralListService(_documentService, _eiamAssignmentService, _committeeRepository, _generalElectionCommitteeRepository, NullLogger<ElectoralListService>.Instance);

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

        _generalElectionCommitteeRepository.GetByFilterForReport(_filterDto, _zeroGuid, _zeroGuid, _zeroGuid).Returns([committeeDetail]);
        _documentService.CreateWordFromTemplate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<string>()).Returns(documentStream);

        var (fileName, content) = await _electoralListService.GenerateDocument(_filterDto, listType);

        await _generalElectionCommitteeRepository.Received(1).GetByFilterForReport(_filterDto, _zeroGuid, _zeroGuid, _zeroGuid);

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
        Assert.That(fileName, Does.Contain(evaluationDate.ToString("yyyyMMdd")));
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

        _generalElectionCommitteeRepository.GetByFilterForReport(_filterDto, _zeroGuid, _zeroGuid, _zeroGuid).Returns([committeeDetail]);
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

        _generalElectionCommitteeRepository.GetByFilterForReport(_filterDto, _zeroGuid, _zeroGuid, _zeroGuid).Returns([committeeDetail]);
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

        _generalElectionCommitteeRepository.GetByFilterForReport(_filterDto, _zeroGuid, _zeroGuid, _zeroGuid).Returns([committeeDetail]);
        using var documentStream = new MemoryStream();
        _documentService.CreateWordFromTemplate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<string>()).Returns(documentStream);

        ElectoralListDto? capturedData = null!;
        await _documentService.CreateWordFromTemplate(Arg.Any<string>(), Arg.Do<object>(o => capturedData = o as ElectoralListDto), Arg.Any<string>());

        await _electoralListService.GenerateDocument(_filterDto, "ElectoralList_FederalCouncil");

        var member = capturedData.Departments.First().Committees.First().Functions.First().Members.First();
        Assert.That(member.OtherMemberships, Is.EqualTo($"[ {secondCommittee.GetDescription()} | {secondCommittee.CommitteeType!.GetText()} | {secondCommittee.Memberships.First().Function!.GetText()} ]"));
    }

    private static GeneralElectionCommittee CreateGeneralElectionCommittee(Guid? personId = null, Guid? termOfOfficeId = null)
    {
        personId ??= Guid.NewGuid();
        termOfOfficeId ??= TermOfOffice.Period4YearsInGeneralElectionGuid;

        var person = new PersonBuilder()
            .WithId(personId.Value)
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithPerson(person)
            .Build();

        var committee = new GeneralElectionCommitteeBuilder()
            .WithTermOfOfficeId(termOfOfficeId.Value)
            .WithCommitteeLevelId(CommitteeLevel.FederalCouncilGuid)
            .WithMembershipCandidate(membershipCandidate)
            .Build();

        return committee;
    }

    private static Committee CreateCommittee(Guid? personId = null, Guid? termOfOfficeId = null)
    {
        personId ??= Guid.NewGuid();
        termOfOfficeId ??= TermOfOffice.Period4YearsInGeneralElectionGuid;

        var person = new PersonBuilder()
            .WithId(personId.Value)
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .Build();

        var membership = new MembershipBuilder()
            .WithPerson(person)
            .Build();

        var committee = new CommitteeBuilder()
            .WithTermOfOfficeId(termOfOfficeId.Value)
            .WithCommitteeLevelId(CommitteeLevel.FederalCouncilGuid)
            .WithMembership(membership)
            .Build();

        membership.Committee = committee;

        return committee;
    }
}
