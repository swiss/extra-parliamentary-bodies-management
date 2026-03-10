using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class MembershipServiceTests
{
    private readonly IMembershipRepository _membershipRepository = Substitute.For<IMembershipRepository>();
    private readonly ICommitteeRepository _committeeRepository = Substitute.For<ICommitteeRepository>();
    private readonly IMasterDataRepository _masterDataRepository = Substitute.For<IMasterDataRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly ICultureService _cultureService = Substitute.For<ICultureService>();
    private readonly IGeneralElectionService _generalElectionService = Substitute.For<IGeneralElectionService>();
    private readonly IGeneralElectionCommitteeService _generalElectionCommitteeService = Substitute.For<IGeneralElectionCommitteeService>();
    private readonly ITermOfOfficeDateService _termOfOfficeDateService = Substitute.For<ITermOfOfficeDateService>();
    private MembershipService _service = null!;

    [SetUp]
    public void Setup()
    {
        _service = new MembershipService(_membershipRepository, _committeeRepository, _authorizationService, _cultureService, _generalElectionService,
            _generalElectionCommitteeService, _termOfOfficeDateService, _masterDataRepository, NullLogger<MembershipService>.Instance);
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
    }

    [TearDown]
    public void TearDown()
    {
        _generalElectionService.ClearSubstitute();
        _generalElectionCommitteeService.ClearSubstitute();
        _membershipRepository.ClearSubstitute();
        _committeeRepository.ClearSubstitute();
        _authorizationService.ClearSubstitute();
    }

    [Test]
    public async Task GetAllByPersonId_ShouldReturnData()
    {
        var personId = Guid.NewGuid();
        var memberships = new List<Membership>
        {
            new MembershipBuilder().WithCommittee(new CommitteeBuilder().Build()).Build(),
            new MembershipBuilder().WithCommittee(new CommitteeBuilder().Build()).Build(),
        };

        _membershipRepository.GetAllByPersonId(personId).Returns(memberships);

        var result = (await _service.GetAllByPersonId(personId)).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(memberships.Count));
    }

    [Test]
    public async Task GetAllByCommitteeId_ShouldReturnData()
    {
        var committeeId = Guid.NewGuid();

        var committee = new CommitteeBuilder().WithId(committeeId).Build();

        var membership1 = new MembershipBuilder()
            .WithCommittee(committee)
            .WithPerson(new PersonBuilder()
                .WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .Build())
            .Build();

        var membership2 = new MembershipBuilder()
            .WithCommittee(committee)
            .WithPerson(new PersonBuilder()
                .WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .Build())
            .Build();

        var membershipList = new List<Membership>
        {
            membership1,
            membership2
        };

        _membershipRepository.GetAllByCommitteeId(committeeId).Returns(membershipList);
        _committeeRepository.GetById(committeeId).Returns(committee);

        var result = (await _service.GetAllByCommitteeId(committeeId)).ActiveMemberships.ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(membershipList.Count));
    }

    [Test]
    public async Task GetAllActiveMembershipsForCommittee_ShouldReturnData()
    {
        var committeeId = Guid.NewGuid();

        var committee = new CommitteeBuilder().WithId(committeeId).Build();

        var membership1 = new MembershipBuilder()
            .WithCommittee(committee)
            .WithPerson(new PersonBuilder()
                .WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .Build())
            .Build();

        var membership2 = new MembershipBuilder()
            .WithCommittee(committee)
            .WithPerson(new PersonBuilder()
                .WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .Build())
            .Build();

        var membershipList = new List<Membership>
        {
            membership1,
            membership2
        };

        _membershipRepository.GetAllActiveMembershipsForCommittee(committeeId).Returns(membershipList);
        _committeeRepository.GetById(committeeId).Returns(committee);

        var result = (await _service.GetAllActiveByCommitteeId(committeeId)).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(membershipList.Count));
    }

    [Test]
    public async Task CreateMembership_ShouldCallRepository()
    {
        var committeeId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();

        var committee = new CommitteeBuilder()
            .WithId(committeeId)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .WithMaximalMember(5)
            .Build();

        var membership = new MembershipBuilder()
            .WithId(membershipId)
            .WithCommittee(committee)
            .WithFunction(new FunctionBuilder().Build())
            .Build();

        var createDto = new MembershipCreateDto
        {
            PersonId = Guid.NewGuid(),
            CommitteeId = committeeId,
            MaximumEmploymentLevel = 80,
            BeginDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            ElectionTypeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            MembershipAdditionId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            Remarks = "remarks",
            RemarksStatus = "remarksStatus",
        };

        _membershipRepository.GetById(Arg.Any<Guid>()).Returns(membership);

        _committeeRepository.GetById(committeeId).Returns(committee);

        _membershipRepository.Create(Arg.Any<Membership>()).Returns(membership);

        await _service.CreateMembership(createDto);

        _authorizationService.Received(1).GetCurrentUserName();
        await _membershipRepository.Received(1).Create(Arg.Is<Membership>(x => x.PersonId == createDto.PersonId));
    }

    [Test]
    public async Task CreateMembership_InGeneralElection_ShouldInvalidateCandidateList()
    {
        var committeeId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();

        var committee = new CommitteeBuilder()
            .WithId(committeeId)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .WithCommitteeLevelId(CommitteeLevel.FederalCouncilGuid)
            .WithTermOfOfficeId(TermOfOffice.Period4YearsInGeneralElectionGuid)
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithCandidateListStateId(CandidateListState.Completed).Build())
            .WithMaximalMember(5)
            .Build();

        var membership = new MembershipBuilder()
            .WithId(membershipId)
            .WithCommittee(committee)
            .WithFunction(new FunctionBuilder().Build())
            .Build();

        var createDto = new MembershipCreateDto
        {
            PersonId = Guid.NewGuid(),
            CommitteeId = committeeId,
            MaximumEmploymentLevel = 80,
            BeginDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            ElectionTypeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            MembershipAdditionId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            Remarks = "remarks",
            RemarksStatus = "remarksStatus",
        };

        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(true);

        _membershipRepository.GetById(Arg.Any<Guid>()).Returns(membership);

        _committeeRepository.GetById(committeeId).Returns(committee);

        _membershipRepository.Create(Arg.Any<Membership>()).Returns(membership);

        await _service.CreateMembership(createDto);

        await _generalElectionCommitteeService.Received(1).InvalidateMembershipCandidateList(committeeId);
    }

    [Test]
    public async Task CreateMembership_InGeneralElection_ShouldSetFederalCouncilProposalToDirty()
    {
        var committeeId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();

        var committee = new CommitteeBuilder()
            .WithId(committeeId)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .WithCommitteeLevelId(CommitteeLevel.FederalCouncilGuid)
            .WithTermOfOfficeId(TermOfOffice.Period4YearsInGeneralElectionGuid)
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithCandidateListStateId(CandidateListState.ReadyForFederalCouncilProposal).Build())
            .WithMaximalMember(5)
            .Build();

        var membership = new MembershipBuilder()
            .WithId(membershipId)
            .WithCommittee(committee)
            .WithFunction(new FunctionBuilder().Build())
            .Build();

        var createDto = new MembershipCreateDto
        {
            PersonId = Guid.NewGuid(),
            CommitteeId = committeeId,
            MaximumEmploymentLevel = 80,
            BeginDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            ElectionTypeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            MembershipAdditionId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            Remarks = "remarks",
            RemarksStatus = "remarksStatus",
        };

        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(true);

        _membershipRepository.GetById(Arg.Any<Guid>()).Returns(membership);

        _committeeRepository.GetById(committeeId).Returns(committee);

        _membershipRepository.Create(Arg.Any<Membership>()).Returns(membership);

        await _service.CreateMembership(createDto);

        await _generalElectionCommitteeService.Received(1).SetFederalCouncilProposalToDirty(committeeId);
    }

    [Test]
    public async Task CreateMembership_InGeneralElectionAndReadyForFederalCouncilProposal_ShouldNotInvalidateCandidateListButMirrorEntries()
    {
        var committeeId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();

        var committee = new CommitteeBuilder()
            .WithId(committeeId)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .WithCommitteeLevelId(CommitteeLevel.FederalCouncilGuid)
            .WithTermOfOfficeId(TermOfOffice.Period4YearsInGeneralElectionGuid)
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithCandidateListStateId(CandidateListState.ReadyForFederalCouncilProposal).Build())
            .WithMaximalMember(5)
            .Build();

        var membership = new MembershipBuilder()
            .WithId(membershipId)
            .WithCommittee(committee)
            .WithFunction(new FunctionBuilder().Build())
            .Build();

        var createDto = new MembershipCreateDto
        {
            PersonId = Guid.NewGuid(),
            CommitteeId = committeeId,
            MaximumEmploymentLevel = 80,
            BeginDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            ElectionTypeId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            MembershipAdditionId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            Remarks = "remarks",
            RemarksStatus = "remarksStatus",
        };

        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(true);

        _membershipRepository.GetById(Arg.Any<Guid>()).Returns(membership);

        _committeeRepository.GetById(committeeId).Returns(committee);

        _membershipRepository.Create(Arg.Any<Membership>()).Returns(membership);

        await _service.CreateMembership(createDto);

        await _generalElectionCommitteeService.DidNotReceiveWithAnyArgs().InvalidateMembershipCandidateList(committeeId);
        await _generalElectionService.Received(1).CreateNewMembershipCandidate(membership, true);
    }

    [Test]
    public async Task GetMembershipForUpdate_ShouldReturnData()
    {
        var membershipId = Guid.NewGuid();

        var membership = new MembershipBuilder()
            .WithBeginDate(new DateOnly(2024, 1, 1))
            .WithEndDate(new DateOnly(2024, 2, 1))
            .WithFunctionId(Guid.NewGuid())
            .WithMaximumEmploymentLevel(1)
            .WithJustificationShorterDuty("text")
            .Build();

        _membershipRepository
            .GetById(Arg.Any<Guid>())
            .Returns(membership);

        var membershipDetail = await _service.GetMembershipForUpdate(membershipId);

        Assert.That(membershipDetail, Is.Not.Null);
        Assert.That(membershipDetail.Id, Is.EqualTo(membership.Id));
    }

    [Test]
    public async Task DeleteMembership_InGeneralElection_ShouldInvalidateCandidateList()
    {
        _authorizationService.IsAdmin.Returns(true);
        _authorizationService.IsDepartment.Returns(false);

        var committeeId = Guid.NewGuid();
        var membershipToDeleteId = Guid.NewGuid();

        var committee = new CommitteeBuilder()
            .WithId(committeeId)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .WithCommitteeLevelId(CommitteeLevel.FederalCouncilGuid)
            .WithTermOfOfficeId(TermOfOffice.Period4YearsInGeneralElectionGuid)
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithCandidateListStateId(CandidateListState.Completed).Build())
            .WithMaximalMember(5)
            .Build();

        var membership = new MembershipBuilder()
            .WithId(membershipToDeleteId)
            .WithCommittee(committee)
            .WithCommitteeId(committee.Id)
            .WithFunction(new FunctionBuilder().Build())
            .Build();

        _membershipRepository.GetByIdForUpdate(membershipToDeleteId).Returns(membership);
        _membershipRepository.GetById(membershipToDeleteId).Returns(membership);

        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(true);

        _committeeRepository.GetById(committeeId).Returns(committee);

        await _service.DeleteMembership(membershipToDeleteId);

        await _generalElectionCommitteeService.Received(1).InvalidateMembershipCandidateList(committeeId);
    }

    [Test]
    public async Task DeleteMembership_InGeneralElection_SetFederalCouncilProposalToDirty()
    {
        _authorizationService.IsAdmin.Returns(true);
        _authorizationService.IsDepartment.Returns(false);

        var committeeId = Guid.NewGuid();
        var membershipToDeleteId = Guid.NewGuid();

        var committee = new CommitteeBuilder()
            .WithId(committeeId)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .WithCommitteeLevelId(CommitteeLevel.FederalCouncilGuid)
            .WithTermOfOfficeId(TermOfOffice.Period4YearsInGeneralElectionGuid)
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithCandidateListStateId(CandidateListState.ReadyForFederalCouncilProposal).Build())
            .WithMaximalMember(5)
            .Build();

        var membership = new MembershipBuilder()
            .WithId(membershipToDeleteId)
            .WithCommittee(committee)
            .WithCommitteeId(committee.Id)
            .WithFunction(new FunctionBuilder().Build())
            .Build();

        _membershipRepository.GetByIdForUpdate(membershipToDeleteId).Returns(membership);
        _membershipRepository.GetById(membershipToDeleteId).Returns(membership);

        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(true);

        _committeeRepository.GetById(committeeId).Returns(committee);

        await _service.DeleteMembership(membershipToDeleteId);

        await _generalElectionCommitteeService.Received(1).SetFederalCouncilProposalToDirty(committeeId);
    }

    [Test]
    public async Task UpdateMembership_InGeneralElection_ShouldInvalidateCandidateList()
    {
        _authorizationService.IsAdmin.Returns(true);
        _authorizationService.IsDepartment.Returns(false);

        var committeeId = Guid.NewGuid();
        var membershipToUpdateId = Guid.NewGuid();

        var committee = new CommitteeBuilder()
            .WithId(committeeId)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .WithCommitteeLevelId(CommitteeLevel.FederalCouncilGuid)
            .WithTermOfOfficeId(TermOfOffice.Period4YearsInGeneralElectionGuid)
            .WithMaximalMember(5)
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithCandidateListStateId(CandidateListState.Completed).Build())
            .Build();

        var membership = new MembershipBuilder()
            .WithId(membershipToUpdateId)
            .WithCommittee(committee)
            .WithFunction(new FunctionBuilder().Build())
            .Build();

        var updateDto = new MembershipUpdateDto
        {
            Id = membershipToUpdateId,
            BeginDate = new DateOnly(2024, 1, 1),            
            CommitteeId = committee.Id,
            EndDate = new DateOnly(2024, 2, 1),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            ElectionTypeId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            MembershipAdditionId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            InCorrelationWithFederalDuty = true,
            MaximumEmploymentLevel = 2,
            RowVersion = 666
        };

        _membershipRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(membership);
        _membershipRepository.GetById(updateDto.Id).Returns(membership);

        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(true);

        _committeeRepository.GetById(committeeId).Returns(committee);

        await _service.UpdateMembership(updateDto.Id, updateDto);

        await _generalElectionCommitteeService.Received(1).InvalidateMembershipCandidateList(committeeId);
    }

    [Test]
    public async Task UpdateMembership_InGeneralElection_ShouldSetFederalCouncilProposalToDirty()
    {
        _authorizationService.IsAdmin.Returns(true);
        _authorizationService.IsDepartment.Returns(false);

        var committeeId = Guid.NewGuid();
        var membershipToUpdateId = Guid.NewGuid();

        var committee = new CommitteeBuilder()
            .WithId(committeeId)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .WithCommitteeLevelId(CommitteeLevel.FederalCouncilGuid)
            .WithTermOfOfficeId(TermOfOffice.Period4YearsInGeneralElectionGuid)
            .WithMaximalMember(5)
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithCandidateListStateId(CandidateListState.ReadyForFederalCouncilProposal).Build())
            .Build();

        var membership = new MembershipBuilder()
            .WithId(membershipToUpdateId)
            .WithCommittee(committee)
            .WithFunction(new FunctionBuilder().Build())
            .Build();

        var updateDto = new MembershipUpdateDto
        {
            Id = membershipToUpdateId,
            BeginDate = new DateOnly(2024, 1, 1),
            CommitteeId = committee.Id,
            EndDate = new DateOnly(2024, 2, 1),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            ElectionTypeId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            MembershipAdditionId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            InCorrelationWithFederalDuty = true,
            MaximumEmploymentLevel = 2,
            RowVersion = 666
        };

        _membershipRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(membership);
        _membershipRepository.GetById(updateDto.Id).Returns(membership);

        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(true);

        _committeeRepository.GetById(committeeId).Returns(committee);

        await _service.UpdateMembership(updateDto.Id, updateDto);

        await _generalElectionCommitteeService.Received(1).SetFederalCouncilProposalToDirty(committeeId);
    }

    [Test]
    public async Task UpdateMembership_WithValidDataAndRoleAdmin_ShouldUpdateMembershipProperties()
    {
        _authorizationService.IsAdmin.Returns(true);
        _authorizationService.IsDepartment.Returns(false);

        var membershipToUpdateId = Guid.NewGuid();

        var committee = new CommitteeBuilder().Build();

        var membership = new MembershipBuilder()
            .WithCommitteeId(committee.Id)
            .WithId(membershipToUpdateId)
            .Build();

        var updateDto = new MembershipUpdateDto
        {
            Id = membershipToUpdateId,
            BeginDate = new DateOnly(2024, 1, 1),
            CommitteeId = committee.Id,
            EndDate = new DateOnly(2024, 2, 1),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            ElectionTypeId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            MembershipAdditionId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            InCorrelationWithFederalDuty = true,
            MaximumEmploymentLevel = 2,
            RowVersion = 666
        };
        _committeeRepository.GetById(committee.Id).Returns(committee);
        _membershipRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(membership);
        _membershipRepository.GetById(updateDto.Id).Returns(membership);

        await _service.UpdateMembership(updateDto.Id, updateDto);

        _authorizationService.Received(1).GetCurrentUserName();
        await _membershipRepository.Received(1).GetByIdForUpdate(membershipToUpdateId, updateDto.RowVersion);
        await _membershipRepository.Received(1).CommitChanges();

        Assert.That(membership, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(membership.BeginDate, Is.EqualTo(updateDto.BeginDate));
            Assert.That(membership.EndDate, Is.EqualTo(updateDto.EndDate));
            Assert.That(membership.FunctionId, Is.EqualTo(updateDto.FunctionId));
            Assert.That(membership.ElectionOfficeId, Is.EqualTo(updateDto.ElectionOfficeId));
            Assert.That(membership.ElectionTypeId, Is.EqualTo(updateDto.ElectionTypeId));
            Assert.That(membership.PersonId, Is.EqualTo(updateDto.PersonId));
            Assert.That(membership.MembershipAdditionId, Is.EqualTo(updateDto.MembershipAdditionId));
            Assert.That(membership.MaximumEmploymentLevel, Is.EqualTo(updateDto.MaximumEmploymentLevel));
            Assert.That(membership.JustificationLongerDuty, Is.EqualTo(updateDto.JustificationLongerDuty));
            Assert.That(membership.JustificationShorterDuty, Is.EqualTo(updateDto.JustificationShorterDuty));
            Assert.That(membership.JustificationMemberInFederalDuty, Is.EqualTo(updateDto.JustificationMemberInFederalDuty));
            Assert.That(membership.JustificationMemberInFederalAssembly, Is.EqualTo(updateDto.JustificationMemberInFederalAssembly));
            Assert.That(membership.RequirementsProfile, Is.EqualTo(updateDto.RequirementsProfile));
            Assert.That(membership.InCorrelationWithFederalDuty, Is.EqualTo(updateDto.InCorrelationWithFederalDuty));
        });
    }

    [Test]
    public void UpdateMembership_WithChangedBeginDateAndRoleDepartment_ShouldThrowException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var membershipToUpdateId = Guid.NewGuid();

        var membership = new MembershipBuilder()
            .WithId(membershipToUpdateId)
            .WithBeginDate(new DateOnly(2025, 2, 2))
            .WithCommittee(new CommitteeBuilder().Build())
            .Build();

        var committee = new CommitteeBuilder().Build();

        var updateDto = new MembershipUpdateDto
        {
            Id = membershipToUpdateId,
            BeginDate = new DateOnly(2024, 1, 1),
            CommitteeId = new Guid(),
            EndDate = new DateOnly(2024, 2, 1),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            ElectionTypeId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            MembershipAdditionId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            InCorrelationWithFederalDuty = true,
            MaximumEmploymentLevel = 2,
            RowVersion = 666
        };

        _membershipRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(membership);
        _membershipRepository.GetById(updateDto.Id).Returns(membership);

        Assert.That(async () => await _service.UpdateMembership(updateDto.Id, updateDto), Throws.Exception.InstanceOf<AuthorizationException>());
    }

    [Test]
    public void UpdateMembership_WithInactiveMembershipAndNotPermittedRole_ShouldThrowException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var membershipToUpdateId = Guid.NewGuid();

        var membership = new MembershipBuilder()
            .WithId(membershipToUpdateId)
            .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-100)))
            .Build();

        var committee = new CommitteeBuilder().Build();

        var updateDto = new MembershipUpdateDto
        {
            Id = membershipToUpdateId,
            BeginDate = new DateOnly(2024, 1, 1),
            CommitteeId = new Guid(),
            EndDate = new DateOnly(2024, 2, 1),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            ElectionTypeId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            MembershipAdditionId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            InCorrelationWithFederalDuty = true,
            MaximumEmploymentLevel = 2,
            RowVersion = 666
        };

        _membershipRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(membership);
        _membershipRepository.GetById(updateDto.Id).Returns(membership);

        Assert.That(async () => await _service.UpdateMembership(updateDto.Id, updateDto), Throws.Exception.InstanceOf<AuthorizationException>());
    }

    [Test]
    public void UpdateMembership_WithNonAuthorizedDepartment_ShouldThrowException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var membershipToUpdateId = Guid.NewGuid();
        var notPermittedDepartmentId = Guid.NewGuid();
        var permittedDepartmentId = Guid.NewGuid();

        var department = new DepartmentBuilder().WithId(permittedDepartmentId).Build();

        var membership = new MembershipBuilder()
            .WithId(membershipToUpdateId)
            .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-100)))
            .Build();

        var committee = new CommitteeBuilder().Build();

        var updateDto = new MembershipUpdateDto
        {
            Id = membershipToUpdateId,
            BeginDate = new DateOnly(2024, 1, 1),
            CommitteeId = new Guid(),
            EndDate = new DateOnly(2024, 2, 1),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            ElectionTypeId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            MembershipAdditionId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            InCorrelationWithFederalDuty = true,
            MaximumEmploymentLevel = 2,
            RowVersion = 666
        };

        _authorizationService.GetDepartment().Returns(department);
        _membershipRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(membership);
        _membershipRepository.GetById(updateDto.Id).Returns(membership);

        Assert.That(async () => await _service.UpdateMembership(updateDto.Id, updateDto), Throws.Exception.InstanceOf<AuthorizationException>());
    }

    [Test]
    public void UpdateMembership_WithNonAuthorizedSecretariat_ShouldThrowException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsSecretariat.Returns(true);

        var membershipToUpdateId = Guid.NewGuid();

        var membership = new MembershipBuilder()
            .WithId(membershipToUpdateId)
            .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-100)))
            .Build();

        var committee = new CommitteeBuilder().Build();

        var updateDto = new MembershipUpdateDto
        {
            Id = membershipToUpdateId,
            BeginDate = new DateOnly(2024, 1, 1),
            CommitteeId = new Guid(),
            EndDate = new DateOnly(2024, 2, 1),
            FunctionId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            ElectionTypeId = Guid.NewGuid(),
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationShorterDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            MembershipAdditionId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            InCorrelationWithFederalDuty = true,
            MaximumEmploymentLevel = 2,
            RowVersion = 666
        };

        _authorizationService.IsCommitteeAssigned(committee.Id).Returns(false);
        _membershipRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(membership);
        _membershipRepository.GetById(updateDto.Id).Returns(membership);

        Assert.That(async () => await _service.UpdateMembership(updateDto.Id, updateDto), Throws.Exception.InstanceOf<AuthorizationException>());
    }
}
