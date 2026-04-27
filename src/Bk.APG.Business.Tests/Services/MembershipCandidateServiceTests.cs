using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.CrossCutting.Tests.Builders;
using Bogus;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class MembershipCandidateServiceTests
{
    private MembershipCandidateService _service;
    private readonly IMembershipCandidateRepository _membershipCandidateRepository = Substitute.For<IMembershipCandidateRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository = Substitute.For<IGeneralElectionCommitteeRepository>();
    private readonly IWorklistTaskRepository _worklistTaskRepository = Substitute.For<IWorklistTaskRepository>();
    private readonly IEiamAssignmentRepository _eiamAssignmentRepository = Substitute.For<IEiamAssignmentRepository>();
    private readonly IPersonService _personService = Substitute.For<IPersonService>();

    [SetUp]
    public void SetUp()
    {
        _service = new MembershipCandidateService(
            _membershipCandidateRepository,
            _authorizationService,
            _generalElectionCommitteeRepository,
            _worklistTaskRepository,
            _eiamAssignmentRepository,
            _personService,
            NullLogger<MembershipCandidateService>.Instance);

        _worklistTaskRepository.GetAllByPersonId(Arg.Any<Guid>()).Returns([]);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(Arg.Any<Guid>()).Returns([]);
        _worklistTaskRepository.GetByWorklistTaskTypeId(Arg.Any<Guid>()).Returns([]);
    }

    [TearDown]
    public void TearDown()
    {
        _membershipCandidateRepository.ClearSubstitute();
        _authorizationService.ClearSubstitute();
        _generalElectionCommitteeRepository.ClearSubstitute();
        _worklistTaskRepository.ClearSubstitute();
        _eiamAssignmentRepository.ClearSubstitute();
    }

    [Test]
    public async Task PartialUpdateMembershipCandidate_ShouldUpdateMembershipCandidate()
    {
        var membershipCandidateId = Guid.NewGuid();
        var existingMembershipCandidate = new MembershipCandidateBuilder().Build();
        _membershipCandidateRepository.GetByIdForUpdate(membershipCandidateId).Returns(existingMembershipCandidate);
        var partialUpdate = new MembershipCandidatePartialUpdateDto
        {
            FunctionId = Guid.NewGuid(),
            Remarks = "Updated remarks",
            RemarksStatus = "Updated status",
        };

        await _service.PartialUpdateMembershipCandidate(membershipCandidateId, partialUpdate);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(existingMembershipCandidate.FunctionId, Is.EqualTo(partialUpdate.FunctionId));
            Assert.That(existingMembershipCandidate.Remarks, Is.EqualTo(partialUpdate.Remarks));
            Assert.That(existingMembershipCandidate.RemarksStatus, Is.EqualTo(partialUpdate.RemarksStatus));
        }
    }

    [Test]
    public async Task UpdateMembershipCandidate_ShouldUpdateMembershipCandidate()
    {
        var membershipCandidateId = Guid.NewGuid();
        var existingMembershipCandidate = new MembershipCandidateBuilder().Build();
        _membershipCandidateRepository.GetByIdForUpdate(membershipCandidateId).Returns(existingMembershipCandidate);
        var updateDto = new MembershipCandidateUpdateDto
        {
            GivenName = "UpdatedGivenName",
            Surname = "UpdatedSurname",
            BirthYear = 1985,
            GenderId = Guid.NewGuid(),
            LanguageId = Guid.NewGuid(),
            ElectionTypeId = Guid.NewGuid(),
            ElectionOfficeId = Guid.NewGuid(),
            MaximumEmploymentLevel = 75,
            MembershipAdditionId = Guid.NewGuid(),
            Id = membershipCandidateId,
            PersonId = null,
            BeginDate = default,
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            FunctionId = Guid.NewGuid(),
            RowVersion = 0
        };

        await _service.UpdateMembershipCandidate(membershipCandidateId, updateDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(existingMembershipCandidate.GivenName, Is.EqualTo(updateDto.GivenName));
            Assert.That(existingMembershipCandidate.Surname, Is.EqualTo(updateDto.Surname));
            Assert.That(existingMembershipCandidate.BirthYear, Is.EqualTo(updateDto.BirthYear));
            Assert.That(existingMembershipCandidate.GenderId, Is.EqualTo(updateDto.GenderId));
            Assert.That(existingMembershipCandidate.LanguageId, Is.EqualTo(updateDto.LanguageId));
            Assert.That(existingMembershipCandidate.ElectionTypeId, Is.Not.EqualTo(updateDto.ElectionTypeId));
            Assert.That(existingMembershipCandidate.ElectionOfficeId, Is.EqualTo(updateDto.ElectionOfficeId));
            Assert.That(existingMembershipCandidate.FunctionId, Is.EqualTo(updateDto.FunctionId));
            Assert.That(existingMembershipCandidate.MaximumEmploymentLevel, Is.EqualTo(updateDto.MaximumEmploymentLevel));
            Assert.That(existingMembershipCandidate.MembershipAdditionId, Is.EqualTo(updateDto.MembershipAdditionId));
            Assert.That(existingMembershipCandidate.EndDate, Is.EqualTo(updateDto.EndDate));
            Assert.That(existingMembershipCandidate.InCorrelationWithFederalDuty, Is.EqualTo(updateDto.InCorrelationWithFederalDuty));
        }
    }

    [Test]
    public async Task DeleteMembershipCandidate_ShouldDeleteMembershipCandidate()
    {
        var membershipCandidateId = Guid.NewGuid();
        var existingMembershipCandidate = new MembershipCandidateBuilder()
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .Build();
        _membershipCandidateRepository.GetByIdForUpdate(membershipCandidateId).Returns(existingMembershipCandidate);

        await _service.DeleteMembershipCandidate(membershipCandidateId);

        await _membershipCandidateRepository.Received(1).Delete(existingMembershipCandidate);
    }

    [Test]
    public void DeleteMembershipCandidate_WithPersonRelationAndNotNewElectionType_ShouldThrowBusinessException()
    {
        var membershipCandidateId = Guid.NewGuid();
        var existingMembershipCandidate = new MembershipCandidateBuilder()
            .WithPersonId(Guid.NewGuid())
            .WithElectionTypeId(Guid.NewGuid())
            .Build();
        _membershipCandidateRepository.GetByIdForUpdate(membershipCandidateId).Returns(existingMembershipCandidate);

        var ex = Assert.ThrowsAsync<BusinessValidationException>(async () => await _service.DeleteMembershipCandidate(membershipCandidateId));
        Assert.That(ex.Message, Is.EqualTo("Only membership candidates without a linked person or with election type 'New Election' can be deleted."));
    }

    [Test]
    public async Task ValidateCandidateList_WithExistingAndNewPerson_ShouldCompleteCandidateListAndTasks()
    {
        var committeeId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).Build(),
            new MembershipCandidateBuilder().WithId(candidateIds[1]).WithPerson(new PersonBuilder().WithSurname("Clark").WithGivenName("Jim").WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build()).WithCorrespondenceLanguage(new LanguageBuilder().Build()).WithBirthYear(1963).Build()).Build(),
            new MembershipCandidateBuilder().WithId(Guid.NewGuid()).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithCandidateListStateId(CandidateListState.Draft)
            .WithVacanciesGeneralElection(null)
            .Build();

        foreach (var candidate in membershipCandidates)
        {
            generalElectionCommittee.MembershipCandidates.Add(candidate);
        }

        var worklistTasks = new List<WorklistTask>
        {
            new WorklistTaskBuilder().WithWorklistTaskType(new WorklistTaskTypeBuilder().WithId(WorklistTaskType.CandidateListCreate).Build()).Build(),
            new WorklistTaskBuilder().WithWorklistTaskType(new WorklistTaskTypeBuilder().WithId(WorklistTaskType.CandidateListForward).Build()).Build(),
            new WorklistTaskBuilder().WithWorklistTaskType(new WorklistTaskTypeBuilder().WithId(WorklistTaskType.CandidateListApprove).Build()).Build()
        };
        _personService.GetDuplicatePersonForGeneralElection(membershipCandidates[0]).Returns(
            new CandidateListDuplicateCheckResultDto()
            {
                DuplicateReason = DuplicateReason.NoDuplicateFound
            });
        _personService.CreatePersonInGeneralElection(membershipCandidates[0]).Returns(new PersonBuilder()
            .WithGivenName("Peter")
            .WithSurname("Tester")
            .WithBirthYear(1965)
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build());
        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(worklistTasks);
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        var validationResult = await _service.ValidateCandidateList(committeeId, candidateIds, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(generalElectionCommittee.CandidateListStateId, Is.EqualTo(CandidateListState.Validated));
            Assert.That(worklistTasks.All(t => t.WorklistTaskStateId == WorklistTaskState.Completed), Is.True);
            Assert.That(membershipCandidates[0].IsSelected, Is.True);
            Assert.That(membershipCandidates[1].IsSelected, Is.True);
            Assert.That(membershipCandidates[2].IsSelected, Is.False);
            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Errors.ToList(), Has.Count.EqualTo(0));
            Assert.That(validationResult.DuplicateCheckResults.ToList(), Has.Count.EqualTo(0));
            Assert.That(validationResult.CreatedPersons.ToList(), Has.Count.EqualTo(1));
            Assert.That(validationResult.ExistingPersons.ToList(), Has.Count.EqualTo(1));
        }

        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task ValidateCandidateList_WithDuplicateWarning_ShouldNotCompleteCandidateListAndTasks()
    {
        var committeeId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).Build(),
            new MembershipCandidateBuilder().WithId(candidateIds[1]).WithPerson(new PersonBuilder().WithSurname("Clark").WithGivenName("Jim").WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build()).WithCorrespondenceLanguage(new LanguageBuilder().Build()).WithBirthYear(1963).Build()).Build(),
            new MembershipCandidateBuilder().WithId(Guid.NewGuid()).Build()
        };
        var worklistTasks = new List<WorklistTask>
        {
            new WorklistTaskBuilder().Build(),
            new WorklistTaskBuilder().Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithMembershipCandidates(membershipCandidates)
            .WithVacanciesGeneralElection(null)
            .Build();
        _personService.GetDuplicatePersonForGeneralElection(membershipCandidates[0]).Returns(
            new CandidateListDuplicateCheckResultDto()
            {
                DuplicateReason = DuplicateReason.FullMatch
            });
        _personService.CreatePersonInGeneralElection(membershipCandidates[0]).Returns(new PersonBuilder()
            .WithGivenName("Peter")
            .WithSurname("Tester")
            .WithBirthYear(1965)
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build());
        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(worklistTasks);

        var validationResult = await _service.ValidateCandidateList(committeeId, candidateIds, false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(worklistTasks.All(t => t.WorklistTaskStateId == WorklistTaskState.Completed), Is.False);
            Assert.That(membershipCandidates[0].IsSelected, Is.True);
            Assert.That(membershipCandidates[1].IsSelected, Is.True);
            Assert.That(membershipCandidates[2].IsSelected, Is.False);
            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Errors.ToList(), Has.Count.EqualTo(0));
            Assert.That(validationResult.DuplicateCheckResults.ToList(), Has.Count.EqualTo(1));
            Assert.That(validationResult.CreatedPersons.ToList(), Has.Count.EqualTo(0));
            Assert.That(validationResult.ExistingPersons.ToList(), Has.Count.EqualTo(1));
        }

        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task ValidateCandidateList_WithoutDuplicateWarning_ShouldCompleteCandidateListAndTasks()
    {
        var committeeId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).WithPerson(new PersonBuilder().WithSurname("Clark").WithGivenName("Jim").WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build()).WithCorrespondenceLanguage(new LanguageBuilder().Build()).WithBirthYear(1963).Build()).Build(),
            new MembershipCandidateBuilder().WithId(candidateIds[1]).WithPerson(new PersonBuilder().WithSurname("Clark").WithGivenName("Jim").WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build()).WithCorrespondenceLanguage(new LanguageBuilder().Build()).WithBirthYear(1963).Build()).Build(),
            new MembershipCandidateBuilder().WithId(Guid.NewGuid()).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithMembershipCandidates(membershipCandidates)
            .WithVacanciesGeneralElection(null)
            .Build();
        var worklistTasks = new List<WorklistTask>
        {
            new WorklistTaskBuilder().WithWorklistTaskType(new WorklistTaskTypeBuilder().WithId(WorklistTaskType.CandidateListCreate).Build()).Build(),
            new WorklistTaskBuilder().WithWorklistTaskType(new WorklistTaskTypeBuilder().WithId(WorklistTaskType.CandidateListForward).Build()).Build(),
            new WorklistTaskBuilder().WithWorklistTaskType(new WorklistTaskTypeBuilder().WithId(WorklistTaskType.CandidateListApprove).Build()).Build()
        };
        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(worklistTasks);
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        var validationResult = await _service.ValidateCandidateList(committeeId, candidateIds, false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(worklistTasks.All(t => t.WorklistTaskStateId == WorklistTaskState.Completed), Is.True);
            Assert.That(membershipCandidates[0].IsSelected, Is.True);
            Assert.That(membershipCandidates[1].IsSelected, Is.True);
            Assert.That(membershipCandidates[2].IsSelected, Is.False);
            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Errors.ToList(), Has.Count.EqualTo(0));
            Assert.That(validationResult.DuplicateCheckResults.ToList(), Has.Count.EqualTo(0));
            Assert.That(validationResult.CreatedPersons.ToList(), Has.Count.EqualTo(0));
            Assert.That(validationResult.ExistingPersons.ToList(), Has.Count.EqualTo(2));
        }

        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task SaveCandidateList_ShouldUpdateCandidateSelection()
    {
        var committeeId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).Build(),
            new MembershipCandidateBuilder().WithId(Guid.NewGuid()).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMembershipCandidates(membershipCandidates)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);

        await _service.SaveCandidateList(committeeId, candidateIds);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(membershipCandidates[0].IsSelected, Is.True);
            Assert.That(membershipCandidates[1].IsSelected, Is.False);
        }

        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task ForwardCandidateList_ToDepartment_ShouldActivateDepartmentTaskAndCompleteOfficeTask()
    {
        var committeeId = Guid.NewGuid();
        var forwardToId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMembershipCandidates(membershipCandidates)
            .Build();
        var departmentTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.CandidateListApprove)
            .Build();
        var officeTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.CandidateListForward)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();
        var secretariatTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate)
            .Build();
        var forwardTo = new EiamAssignmentBuilder()
            .WithRole(Role.Department)
            .Build();
        var forwardDto = new CandidateListForwardDto
        {
            ForwardToId = forwardToId,
            CandidateIds = candidateIds,
            Description = "Test description"
        };

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _eiamAssignmentRepository.GetById(forwardToId).Returns(forwardTo);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(new List<WorklistTask> { departmentTask, officeTask, secretariatTask });

        await _service.ForwardCandidateList(committeeId, forwardDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(departmentTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Active));
            Assert.That(departmentTask.Description, Is.EqualTo(forwardDto.Description));
            Assert.That(officeTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
            Assert.That(membershipCandidates[0].IsSelected, Is.True);
        }

        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
        await _worklistTaskRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task ForwardCandidateList_ToOffice_ShouldActivateOfficeTaskAndCompleteSecretariatTask()
    {
        var committeeId = Guid.NewGuid();
        var forwardToId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMembershipCandidates(membershipCandidates)
            .Build();
        var departmentTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.CandidateListApprove)
            .Build();
        var officeTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.CandidateListForward)
            .Build();
        var secretariatTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();
        var forwardTo = new EiamAssignmentBuilder()
            .WithRole(Role.Office)
            .Build();
        var forwardDto = new CandidateListForwardDto
        {
            ForwardToId = forwardToId,
            CandidateIds = candidateIds,
            Description = "Test description"
        };

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _eiamAssignmentRepository.GetById(forwardToId).Returns(forwardTo);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(new List<WorklistTask> { departmentTask, officeTask, secretariatTask });

        await _service.ForwardCandidateList(committeeId, forwardDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(officeTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Active));
            Assert.That(officeTask.Description, Is.EqualTo(forwardDto.Description));
            Assert.That(secretariatTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
            Assert.That(membershipCandidates[0].IsSelected, Is.True);
        }

        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
        await _worklistTaskRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task ForwardCandidateList_ToSecretariat_ShouldActivateSecretariatTaskAndInactivateOfficeTask()
    {
        var committeeId = Guid.NewGuid();
        var forwardToId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMembershipCandidates(membershipCandidates)
            .Build();
        var departmentTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.CandidateListApprove)
            .Build();
        var officeTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.CandidateListForward)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();
        var secretariatTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate)
            .Build();
        var forwardTo = new EiamAssignmentBuilder()
            .WithRole(Role.Secretariat)
            .Build();
        var forwardDto = new CandidateListForwardDto
        {
            ForwardToId = forwardToId,
            CandidateIds = candidateIds,
            Description = "Test description"
        };

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _eiamAssignmentRepository.GetById(forwardToId).Returns(forwardTo);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(new List<WorklistTask> { departmentTask, officeTask, secretariatTask });

        await _service.ForwardCandidateList(committeeId, forwardDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(secretariatTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Active));
            Assert.That(secretariatTask.Description, Is.EqualTo(forwardDto.Description));
            Assert.That(officeTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Inactive));
            Assert.That(membershipCandidates[0].IsSelected, Is.True);
        }

        await _generalElectionCommitteeRepository.Received(1).CommitChanges();
        await _worklistTaskRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task FinalizeReadyForProposal_WithPassedValidation_ShouldCompleteAdminTaskAndReleaseGeneralElection()
    {
        var committeeId = Guid.NewGuid();
        var secretariatAssignment = new EiamAssignmentBuilder().WithRole(Role.Secretariat).Build();
        var departmentAssignment = new EiamAssignmentBuilder().WithRole(Role.Department).Build();
        var adminAssignment = new EiamAssignmentBuilder().WithRole(Role.Admin).WithId(EiamAssignment.AdminId).Build();

        var committeeType = new CommitteeTypeBuilder()
            .WithId(CommitteeType.ManagementCommitteeGuid)
            .Build();
        var committee = new CommitteeBuilder()
            .WithCommitteeType(committeeType)
            .Build();
        committee.EiamAssignmentId = secretariatAssignment.Id;
        committee.ContactPoints.Add(new ContactPointBuilder()
            .WithCommittee(committee)
            .WithEndDate(null)
            .WithContactPointType(new ContactPointTypeBuilder().WithId(ContactPointType.SecretariatGuid).Build())
            .Build());

        var department = new DepartmentBuilder().Build();
        department.EiamAssignmentId = departmentAssignment.Id;

        var committeeForUpdate = new GeneralElectionCommitteeBuilder()
            .WithCommitteeId(committeeId)
            .WithCommittee(committee)
            .WithDepartment(department)
            .WithCandidateListStateId(CandidateListState.Validated)
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithVacanciesGeneralElection(null)
            .Build();

        var secretariatTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
            .WithWorklistTaskStateId(WorklistTaskState.Inactive)
            .WithAssignedTo(secretariatAssignment)
            .Build();
        var departmentTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
            .WithWorklistTaskStateId(WorklistTaskState.Inactive)
            .WithAssignedTo(departmentAssignment)
            .Build();
        var adminTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithAssignedTo(adminAssignment)
            .Build();

        _authorizationService.GetCurrentEiamAssignment().Returns(adminAssignment);
        _authorizationService.GetCurrentUserName().Returns("admin.user");
        _generalElectionCommitteeRepository.GetByCommitteeId(committeeId).Returns(committeeForUpdate);
        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(committeeForUpdate, committeeForUpdate);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(committeeForUpdate.Id).Returns([secretariatTask, departmentTask, adminTask]);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(committeeId).Returns([secretariatTask, departmentTask, adminTask]);

        var result = await _service.FinalizeReadyForProposal(committeeId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.AllValidationsPassed, Is.True);
            Assert.That(adminTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
            Assert.That(committeeForUpdate.ReleaseGeneralElection, Is.True);
            Assert.That(committeeForUpdate.IsFederalCouncilProposalDirty, Is.False);
        }

        await _generalElectionCommitteeRepository.Received(2).CommitChanges();
        await _worklistTaskRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task FinalizeReadyForProposal_WithFailedValidation_ShouldReturnValidationResultWithoutFinalizing()
    {
        var committeeId = Guid.NewGuid();
        var committeeForUpdate = new GeneralElectionCommitteeBuilder()
            .WithCommitteeId(committeeId)
            .WithCandidateListStateId(CandidateListState.Validated)
            .WithMinimalMember(1)
            .WithMaximalMember(10)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(committeeId).Returns(committeeForUpdate);
        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(committeeForUpdate);

        var result = await _service.FinalizeReadyForProposal(committeeId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.AllValidationsPassed, Is.False);
            Assert.That(committeeForUpdate.ReleaseGeneralElection, Is.False);
        }

        await _generalElectionCommitteeRepository.DidNotReceive().CommitChanges();
        await _worklistTaskRepository.DidNotReceive().CommitChanges();
    }

    [Test]
    public async Task GetMembers_WithCompletedCandidateList_ShouldReturnActiveMemberships()
    {
        var committeeId = Guid.NewGuid();
        var selectedCandidateId = Guid.NewGuid();
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder()
                .WithId(selectedCandidateId)
                .WithIsSelected(true)
                .Build(),
            new MembershipCandidateBuilder()
                .WithIsSelected(false)
                .Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMembershipCandidates(membershipCandidates)
            .WithCandidateListStateId(CandidateListState.Validated)
            .Build();
        _generalElectionCommitteeRepository.GetByCommitteeId(committeeId).Returns(generalElectionCommittee);

        var result = await _service.GetMembers(committeeId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ActiveMemberships.Count(), Is.EqualTo(1));
            Assert.That(result.InactiveMemberships, Is.Empty);
        }
    }

    [Test]
    public async Task GetMembershipCandidateForUpdate_ShouldReturnMembershipCandidateUpdateDto()
    {
        var candidateId = Guid.NewGuid();
        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .Build();

        _membershipCandidateRepository.GetByIdForUpdate(candidateId).Returns(membershipCandidate);

        var result = await _service.GetMembershipCandidateForUpdate(candidateId);

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task CreateMembershipCandidate_ShouldCreateAndReturnMembershipCandidate()
    {
        var committeeId = Guid.NewGuid();
        var createdCandidateId = Guid.NewGuid();
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .Build();
        var createDto = new MembershipCandidateCreateDto
        {
            CommitteeId = committeeId,
            PersonId = Guid.NewGuid(),
            Surname = "TestSurname",
            GivenName = "TestGivenName",
            BirthYear = 1990,
            GenderId = Guid.NewGuid(),
            LanguageId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
            Remarks = "Test remarks",
            RemarksStatus = "Test status"
        };
        var createdCandidate = new MembershipCandidateBuilder()
            .WithId(createdCandidateId)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(committeeId).Returns(generalElectionCommittee);
        _membershipCandidateRepository.Create(Arg.Any<MembershipCandidate>()).Returns(createdCandidate);
        _membershipCandidateRepository.GetById(createdCandidateId).Returns(createdCandidate);
        _authorizationService.GetCurrentUserName().Returns("testuser");

        var result = await _service.CreateMembershipCandidate(createDto);

        Assert.That(result, Is.Not.Null);
        await _membershipCandidateRepository.Received(1).Create(Arg.Any<MembershipCandidate>());
    }

    [Test]
    public void ValidateCandidateCount_WhenCandidateCountBelowMinimum_AddsMinimumError()
    {
        var generalElectionCommittee = new Faker<GeneralElectionCommittee>().Generate();
        generalElectionCommittee.MinimalMembers = 5;
        generalElectionCommittee.MaximalMembers = 10;
        generalElectionCommittee.VacanciesGeneralElection = null;

        var validationResult = new CandidateListValidationResultDto();

        MembershipCandidateService.ValidateCandidateCount(3, generalElectionCommittee, validationResult);

        Assert.That(validationResult.Errors, Has.Count.EqualTo(1));
        Assert.That(validationResult.Errors.First(), Does.Contain("5"));
    }

    [Test]
    public void ValidateCandidateCount_WhenCandidateCountExceedsMaximum_AddsMaximumError()
    {
        var generalElectionCommittee = new Faker<GeneralElectionCommittee>().Generate();
        generalElectionCommittee.MinimalMembers = 5;
        generalElectionCommittee.MaximalMembers = 10;
        generalElectionCommittee.VacanciesGeneralElection = null;
        var validationResult = new CandidateListValidationResultDto();

        MembershipCandidateService.ValidateCandidateCount(12, generalElectionCommittee, validationResult);

        Assert.That(validationResult.Errors, Has.Count.EqualTo(1));
        Assert.That(validationResult.Errors.First(), Does.Contain("10"));
    }

    [Test]
    public void ValidateCandidateCount_WhenCandidateCountWithinRange_NoErrors()
    {
        var generalElectionCommittee = new Faker<GeneralElectionCommittee>().Generate();
        generalElectionCommittee.MinimalMembers = 5;
        generalElectionCommittee.MaximalMembers = 10;
        generalElectionCommittee.VacanciesGeneralElection = null;
        var validationResult = new CandidateListValidationResultDto();

        MembershipCandidateService.ValidateCandidateCount(7, generalElectionCommittee, validationResult);
        Assert.That(validationResult.Errors, Is.Empty);
    }

    [Test]
    public void ValidateCandidateCount_WhenMaximalMembersIsNull_OnlyValidatesMinimum()
    {
        var generalElectionCommittee = new Faker<GeneralElectionCommittee>().Generate();
        generalElectionCommittee.MinimalMembers = 5;
        generalElectionCommittee.MaximalMembers = null;
        generalElectionCommittee.VacanciesGeneralElection = null;
        var validationResult = new CandidateListValidationResultDto();

        MembershipCandidateService.ValidateCandidateCount(100, generalElectionCommittee, validationResult);

        Assert.That(validationResult.Errors, Is.Empty);
    }

    [Test]
    public void ValidateCandidateCount_WhenCandidateAndVacanciesCountWithinRange_NoErrors()
    {
        var generalElectionCommittee = new Faker<GeneralElectionCommittee>().Generate();
        generalElectionCommittee.MinimalMembers = 7;
        generalElectionCommittee.MaximalMembers = 10;
        generalElectionCommittee.VacanciesGeneralElection = 2;
        var validationResult = new CandidateListValidationResultDto();

        MembershipCandidateService.ValidateCandidateCount(7, generalElectionCommittee, validationResult);
        Assert.That(validationResult.Errors, Is.Empty);
    }

    [Test]
    public void ValidateCandidateCount_WhenCandidateCountWithVacanciesExceedsMaximum_AddsMaximumError()
    {
        var generalElectionCommittee = new Faker<GeneralElectionCommittee>().Generate();
        generalElectionCommittee.MinimalMembers = 5;
        generalElectionCommittee.MaximalMembers = 10;
        generalElectionCommittee.VacanciesGeneralElection = 5;
        var validationResult = new CandidateListValidationResultDto();

        MembershipCandidateService.ValidateCandidateCount(8, generalElectionCommittee, validationResult);

        Assert.That(validationResult.Errors, Has.Count.EqualTo(1));
        Assert.That(validationResult.Errors.First(), Does.Contain("10"));
    }

    [Test]
    public void ValidateCandidateCount_WhenCandidateCountWithVacanciesUnderMinimumMembers_AddsMinimumError()
    {
        var generalElectionCommittee = new Faker<GeneralElectionCommittee>().Generate();
        generalElectionCommittee.MinimalMembers = 10;
        generalElectionCommittee.MaximalMembers = 12;
        generalElectionCommittee.VacanciesGeneralElection = 3;
        var validationResult = new CandidateListValidationResultDto();

        MembershipCandidateService.ValidateCandidateCount(5, generalElectionCommittee, validationResult);

        Assert.That(validationResult.Errors, Has.Count.EqualTo(1));
        Assert.That(validationResult.Errors.First(), Does.Contain("10"));
    }

    [Test]
    public async Task GetSimilarMembershipCandidates_WithEmptyCandidateList_ShouldReturnNull()
    {
        var committeeId = Guid.NewGuid();

        var candidateToCheck = new MembershipCandidateCreateDto
        {
            CommitteeId = committeeId,
            Surname = "Regazzoni",
            GivenName = "Clay",
            BirthYear = 1939,
            GenderId = Gender.MaleGuid,
            LanguageId = Guid.Parse(Language.ItalianId),
            FunctionId = Guid.NewGuid(),
        };

        _generalElectionCommitteeRepository.GetByCommitteeId(committeeId).Returns(new GeneralElectionCommitteeBuilder().Build());

        var result = await _service.GetDuplicateMembershipCandidateForList(candidateToCheck.CommitteeId, candidateToCheck.Surname, candidateToCheck.GivenName, candidateToCheck.BirthYear, candidateToCheck.GenderId, candidateToCheck.LanguageId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetDuplicateMembershipCandidates_ShouldReturnSimilarCandidate()
    {
        var committeeId = Guid.NewGuid();

        var candidateToCheck = new MembershipCandidateCreateDto
        {
            CommitteeId = committeeId,
            Surname = "Regazzoni",
            GivenName = "Clay",
            BirthYear = 1939,
            GenderId = Gender.MaleGuid,
            LanguageId = Guid.Parse(Language.ItalianId),
            FunctionId = Guid.NewGuid(),
        };

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithSurname("Regazzoni")
            .WithGivenName("Clay")
            .WithBirthYear(1939)
            .WithGenderId(Gender.MaleGuid)
            .WithLanguageId(Guid.Parse(Language.ItalianId))
            .Build();
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder().WithMembershipCandidate(membershipCandidate).Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(committeeId).Returns(generalElectionCommittee);

        var result = await _service.GetDuplicateMembershipCandidateForList(candidateToCheck.CommitteeId, candidateToCheck.Surname, candidateToCheck.GivenName, candidateToCheck.BirthYear, candidateToCheck.GenderId, candidateToCheck.LanguageId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(membershipCandidate.Id));
    }

    [TestCase("Ragazzoni", "Clay", 1939, "d45a5c83-ddf8-4e78-9fd9-0cf2a8914d15", Language.ItalianId)]
    [TestCase("Regazzoni", "Clayy", 1939, "d45a5c83-ddf8-4e78-9fd9-0cf2a8914d15", Language.ItalianId)]
    [TestCase("Regazzoni", "Clay", 1940, "d45a5c83-ddf8-4e78-9fd9-0cf2a8914d15", Language.ItalianId)]
    [TestCase("Regazzoni", "Clay", 1939, "aa36da2a-b1d5-4b1e-a659-3f488dbc4d1e", Language.ItalianId)]
    public async Task GetDuplicateMembershipCandidates_ShouldNotReturnDuplicateCandidate(string surname, string givenName, int birthYear, string genderIdAsString, string languageIdAsString)
    {
        var committeeId = Guid.NewGuid();

        var candidateToCheck = new MembershipCandidateCreateDto
        {
            CommitteeId = committeeId,
            Surname = surname,
            GivenName = givenName,
            BirthYear = birthYear,
            GenderId = Guid.Parse(genderIdAsString),
            LanguageId = Guid.Parse(languageIdAsString),
            FunctionId = Guid.NewGuid(),
        };

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithSurname("Regazzoni")
            .WithGivenName("Clay")
            .WithBirthYear(1939)
            .WithGenderId(Gender.MaleGuid)
            .WithLanguageId(Guid.Parse(Language.ItalianId))
            .Build();
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder().WithMembershipCandidate(membershipCandidate).Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(committeeId).Returns(generalElectionCommittee);

        var result = await _service.GetDuplicateMembershipCandidateForList(candidateToCheck.CommitteeId, candidateToCheck.Surname, candidateToCheck.GivenName, candidateToCheck.BirthYear, candidateToCheck.GenderId, candidateToCheck.LanguageId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CompleteCandidateList_WithSuccessfulDuplicateCheck_UpdatesCandidateListState()
    {
        var committeeId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithMembershipCandidates(membershipCandidates)
            .WithVacanciesGeneralElection(null)
            .Build();
        var worklistTasks = new List<WorklistTask>
        {
            new WorklistTaskBuilder().WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate).Build()
        };

        _personService.GetDuplicatePersonForGeneralElection(membershipCandidates[0]).Returns(
            new CandidateListDuplicateCheckResultDto { DuplicateReason = DuplicateReason.NoDuplicateFound });
        _personService.CreatePersonInGeneralElection(membershipCandidates[0]).Returns(new PersonBuilder()
            .WithGivenName("Test")
            .WithSurname("User")
            .WithBirthYear(1980)
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build());
        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(worklistTasks);
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        await _service.ValidateCandidateList(committeeId, candidateIds, false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(generalElectionCommittee.CandidateListStateId, Is.EqualTo(CandidateListState.Validated));
            Assert.That(generalElectionCommittee.IsValidated, Is.True);
        }
    }

    [Test]
    public async Task CreateReadyForFederalCouncilProposalTasks_ForBigDepartment_CreatesFourTasks()
    {
        var committeeId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).WithPerson(new PersonBuilder().WithSurname("Test").WithGivenName("User").WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build()).WithCorrespondenceLanguage(new LanguageBuilder().Build()).WithBirthYear(1980).Build()).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithDepartment(new DepartmentBuilder().WithIsBigDepartment(true).Build())
            .WithOffice(new OfficeBuilder().Build())
            .WithOfficeReadyForProposalDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)))
            .WithMembershipCandidates(membershipCandidates)
            .WithVacanciesGeneralElection(null)
            .Build();
        var worklistTasks = new List<WorklistTask>
        {
            new WorklistTaskBuilder().WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate).Build()
        };

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(worklistTasks);
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        await _service.ValidateCandidateList(committeeId, candidateIds, true);

        await _worklistTaskRepository.Received(1).CreateRange(Arg.Is<List<WorklistTask>>(tasks => tasks.Count == 4));
    }

    [Test]
    public async Task CreateReadyForFederalCouncilProposalTasks_ForSmallDepartment_CreatesThreeTasks()
    {
        var committeeId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).WithPerson(new PersonBuilder().WithSurname("Test").WithGivenName("User").WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build()).WithCorrespondenceLanguage(new LanguageBuilder().Build()).WithBirthYear(1980).Build()).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithDepartment(new DepartmentBuilder().WithIsBigDepartment(false).Build())
            .WithMembershipCandidates(membershipCandidates)
            .WithVacanciesGeneralElection(null)
            .Build();
        var worklistTasks = new List<WorklistTask>
        {
            new WorklistTaskBuilder().WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate).Build()
        };

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(worklistTasks);
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        await _service.ValidateCandidateList(committeeId, candidateIds, true);

        await _worklistTaskRepository.Received(1).CreateRange(Arg.Is<List<WorklistTask>>(tasks => tasks.Count == 3));
    }

    [Test]
    public async Task ValidateCandidateList_WithMissingJustifications_CreatesJustificationTask()
    {
        var committeeId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).WithPerson(new PersonBuilder().WithSurname("Test").WithGivenName("User").WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build()).WithCorrespondenceLanguage(new LanguageBuilder().Build()).WithBirthYear(1980).Build()).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithSecretariatReadyForProposalDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            .WithCommitteeType(new CommitteeTypeBuilder().WithFemaleAndMaleThreshold(40, 40).WithLanguagesThreshold(1, 1, 1, 1).Build())
            .WithIsValidated(true)
            .WithMembershipCandidates(membershipCandidates)
            .WithVacanciesGeneralElection(null)
            .Build();
        var worklistTasks = new List<WorklistTask>
        {
            new WorklistTaskBuilder().WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate).Build()
        };

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(worklistTasks);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(Arg.Any<Guid>()).Returns(new List<WorklistTask>());
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        var validationResult = await _service.ValidateCandidateList(committeeId, candidateIds, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(validationResult.AreJustificationsMissing, Is.True);
            Assert.That(validationResult.IsValid, Is.True);
        }

        await _worklistTaskRepository.Received(1).Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingJustifications));
    }

    [Test]
    public async Task ValidateCandidateList_WithMissingJustificationsAndExistingInactiveTask_ActivatesTask()
    {
        var committeeId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).WithPerson(new PersonBuilder().WithSurname("Test").WithGivenName("User").WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build()).WithCorrespondenceLanguage(new LanguageBuilder().Build()).WithBirthYear(1980).Build()).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithCommitteeType(new CommitteeTypeBuilder().WithFemaleAndMaleThreshold(40, 40).WithLanguagesThreshold(1, 1, 1, 1).Build())
            .WithIsValidated(true)
            .WithMembershipCandidates(membershipCandidates)
            .WithVacanciesGeneralElection(null)
            .Build();
        var existingJustificationTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingJustifications)
            .WithWorklistTaskStateId(WorklistTaskState.Completed)
            .Build();
        var worklistTasks = new List<WorklistTask>
        {
            new WorklistTaskBuilder().WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate).Build()
        };

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(worklistTasks);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(Arg.Any<Guid>()).Returns(new List<WorklistTask> { existingJustificationTask });
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);
        var validationResult = await _service.ValidateCandidateList(committeeId, candidateIds, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(validationResult.AreJustificationsMissing, Is.True);
            Assert.That(existingJustificationTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Active));
        }

        await _worklistTaskRepository.DidNotReceive().Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingJustifications));
    }

    [Test]
    public async Task ValidateCandidateList_WithoutMissingJustifications_DoesNotCreateTask()
    {
        var committeeId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).WithPerson(new PersonBuilder().WithSurname("Test").WithGivenName("User").WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build()).WithCorrespondenceLanguage(new LanguageBuilder().Build()).WithBirthYear(1980).Build()).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithJustificationGenders("test")
            .WithMeasuresGenders("test")
            .WithJustificationLanguages("test")
            .WithMeasuresLanguages("test")
            .WithCommitteeType(new CommitteeTypeBuilder().WithFemaleAndMaleThreshold(40, 40).Build())
            .WithMembershipCandidates(membershipCandidates)
            .WithVacanciesGeneralElection(null)
            .Build();
        var worklistTasks = new List<WorklistTask>
        {
            new WorklistTaskBuilder().WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate).Build()
        };

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(worklistTasks);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(Arg.Any<Guid>()).Returns(new List<WorklistTask>());
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        var validationResult = await _service.ValidateCandidateList(committeeId, candidateIds, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(validationResult.AreJustificationsMissing, Is.False);
            Assert.That(validationResult.IsValid, Is.True);
        }

        await _worklistTaskRepository.DidNotReceive().Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingJustifications));
    }

    [Test]
    public async Task ValidateCandidateList_WithoutMissingJustificationsAndExistingActiveTask_CompletesTask()
    {
        var committeeId = Guid.NewGuid();
        var candidateIds = new List<Guid> { Guid.NewGuid() };
        var membershipCandidates = new List<MembershipCandidate>
        {
            new MembershipCandidateBuilder().WithId(candidateIds[0]).WithPerson(new PersonBuilder().WithSurname("Test").WithGivenName("User").WithGender(new GenderBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build()).WithCorrespondenceLanguage(new LanguageBuilder().Build()).WithBirthYear(1980).Build()).Build()
        };
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithJustificationGenders("test")
            .WithMeasuresGenders("test")
            .WithJustificationLanguages("test")
            .WithMeasuresLanguages("test")
            .WithCommitteeType(new CommitteeTypeBuilder().WithFemaleAndMaleThreshold(40, 40).Build())
            .WithMembershipCandidates(membershipCandidates)
            .WithVacanciesGeneralElection(null)
            .Build();
        var existingJustificationTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingJustifications)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();
        var worklistTasks = new List<WorklistTask>
        {
            new WorklistTaskBuilder().WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate).Build()
        };

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(worklistTasks);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(Arg.Any<Guid>()).Returns(new List<WorklistTask> { existingJustificationTask });
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        var validationResult = await _service.ValidateCandidateList(committeeId, candidateIds, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(validationResult.AreJustificationsMissing, Is.False);
            Assert.That(existingJustificationTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
        }
    }

    [Test]
    public async Task ValidateCandidateList_WithMissingContactPoints_CreatesContactPointTasks()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var candidateIds = new List<Guid> { candidateId };

        var person = new PersonBuilder()
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .Build();

        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithCandidateListStateId(CandidateListState.Validated)
            .WithCommittee(committee)
            .WithMembershipCandidates(new List<MembershipCandidate> { membershipCandidate })
            .WithVacanciesGeneralElection(null)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(new List<WorklistTask>());
        _worklistTaskRepository.GetAllByPersonId(person.Id).Returns([]);

        var validationResult = await _service.ValidateCandidateList(committeeId, candidateIds, true);

        Assert.That(validationResult.AreContactPointsMissing, Is.True);
        await _worklistTaskRepository.Received(1).Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingSecretariat));
        await _worklistTaskRepository.Received(1).Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingDataProtectionOfficer));
    }

    [Test]
    public async Task ValidateCandidateList_WithContactPointsPresent_CompletesContactPointTasks()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var candidateIds = new List<Guid> { candidateId };

        var person = new PersonBuilder()
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .Build();

        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .WithContactPoint(new ContactPointBuilder()
                .WithContactPointType(new ContactPointTypeBuilder().WithId(ContactPointType.SecretariatGuid).Build())
                .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
                .Build())
            .WithContactPoint(new ContactPointBuilder()
                .WithContactPointType(new ContactPointTypeBuilder().WithId(ContactPointType.DataProtectionOfficerGuid).Build())
                .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
                .Build())
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithCandidateListStateId(CandidateListState.Validated)
            .WithCommittee(committee)
            .WithMembershipCandidates(new List<MembershipCandidate> { membershipCandidate })
            .WithVacanciesGeneralElection(null)
            .Build();

        var missingSecretariatTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingSecretariat)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();

        var missingDataProtectionOfficerTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingDataProtectionOfficer)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();

        var secretariatAssignment = new EiamAssignmentBuilder().WithRole(Role.Secretariat).Build();
        var departmentAssignment = new EiamAssignmentBuilder().WithRole(Role.Department).Build();
        var adminAssignment = new EiamAssignmentBuilder().WithRole(Role.Admin).WithId(EiamAssignment.AdminId).Build();
        var readyForProposalSecretariatTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
            .WithWorklistTaskStateId(WorklistTaskState.Inactive)
            .WithAssignedTo(secretariatAssignment)
            .Build();
        var readyForProposalDepartmentTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
            .WithWorklistTaskStateId(WorklistTaskState.Inactive)
            .WithAssignedTo(departmentAssignment)
            .Build();
        var readyForProposalAdminTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
            .WithWorklistTaskStateId(WorklistTaskState.Inactive)
            .WithAssignedTo(adminAssignment)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns([
            missingSecretariatTask,
            missingDataProtectionOfficerTask,
            readyForProposalSecretariatTask,
            readyForProposalDepartmentTask,
            readyForProposalAdminTask
        ]);
        _worklistTaskRepository.GetAllByPersonId(person.Id).Returns([]);

        var validationResult = await _service.ValidateCandidateList(committeeId, candidateIds, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(validationResult.AreContactPointsMissing, Is.False);
            Assert.That(missingSecretariatTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
            Assert.That(missingDataProtectionOfficerTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
        }
    }

    [Test]
    public async Task ValidateCandidateList_SelectedNewCandidate_ShouldCreateInterestsAndBaseDataTasks()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var candidateIds = new List<Guid> { candidateId };

        var person = new PersonBuilder()
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithCommitteeType(new CommitteeTypeBuilder().WithId(CommitteeType.AuthoritiesCommissionGuid).Build())
            .WithMembershipCandidates(new List<MembershipCandidate> { membershipCandidate })
            .WithSecretariatReadyForProposalDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            .WithVacanciesGeneralElection(null)
            .Build();
        membershipCandidate.GeneralElectionCommittee = generalElectionCommittee;

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByPersonId(person.Id).Returns([]);
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);
        _personService.GetDuplicatePersonForGeneralElection(membershipCandidate).Returns(new CandidateListDuplicateCheckResultDto { DuplicateReason = DuplicateReason.NoDuplicateFound });
        _personService.CreatePersonInGeneralElection(membershipCandidate).Returns(person);

        await _service.ValidateCandidateList(committeeId, candidateIds, true);

        await _worklistTaskRepository.Received(1).Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonInterests));
        await _worklistTaskRepository.Received(1).Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonBaseData));
    }

    [Test]
    public async Task ValidateCandidateList_SelectedCandidateNeedsInterests_ShouldCreateInterestsTask()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var candidateIds = new List<Guid> { candidateId };

        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .Build();
        var membership = new Membership
        {
            BeginDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Committee = committee,
            Function = new FunctionBuilder().Build(),
            InCorrelationWithFederalDuty = false,
            IsDeleted = false,
            Created = DateTime.UtcNow,
            CreatedBy = "System",
            Modified = DateTime.UtcNow,
            ModifiedBy = "System"
        };
        var person = new PersonBuilder()
            .WithNoInterest(false)
            .WithFederalDuty(true)
            .WithOffice(Guid.NewGuid())
            .WithMemberships(new List<Membership> { membership })
            .WithInterests(new List<Interest>())
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithMembershipCandidates(new List<MembershipCandidate> { membershipCandidate })
            .WithSecretariatReadyForProposalDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            .WithVacanciesGeneralElection(null)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByPersonId(person.Id).Returns([]);
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        await _service.ValidateCandidateList(committeeId, candidateIds, true);

        await _worklistTaskRepository.Received(1).Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonInterests));
        await _worklistTaskRepository.DidNotReceive().Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonBaseData));
    }

    [Test]
    public async Task ValidateCandidateList_SelectedCandidateNeedsBaseData_ShouldCreateBaseDataTask()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var candidateIds = new List<Guid> { candidateId };

        var person = new PersonBuilder()
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .WithOfficeAddress(new Address { Email = "invalid-email", Created = DateTime.UtcNow, CreatedBy = "System", Modified = DateTime.UtcNow, ModifiedBy = "System" })
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithMembershipCandidates(new List<MembershipCandidate> { membershipCandidate })
            .WithSecretariatReadyForProposalDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            .WithVacanciesGeneralElection(null)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByPersonId(person.Id).Returns([]);
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        await _service.ValidateCandidateList(committeeId, candidateIds, true);

        await _worklistTaskRepository.Received(1).Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonBaseData));
        await _worklistTaskRepository.DidNotReceive().Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonInterests));
    }

    [Test]
    public async Task ValidateCandidateList_SelectedCandidateMissingInterests_ShouldIncludePersonInMissingInterests()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var candidateIds = new List<Guid> { candidateId };

        var committeeType = new CommitteeTypeBuilder()
            .WithId(CommitteeType.ManagementCommitteeGuid)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithCommitteeType(committeeType)
            .WithSecretariatReadyForProposalDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            .WithVacanciesGeneralElection(null)
            .Build();

        var person = new PersonBuilder()
            .WithSurname("Test")
            .WithGivenName("User")
            .WithEmployer("Employer")
            .WithOccupations(new List<Occupation> { new OccupationBuilder().Build() })
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .WithGeneralElectionCommittee(generalElectionCommittee)
            .Build();

        generalElectionCommittee.MembershipCandidates.Add(membershipCandidate);

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByPersonId(person.Id).Returns([]);
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        var validationResult = await _service.ValidateCandidateList(committeeId, candidateIds, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(validationResult.PersonsWithMissingInterests, Has.Count.EqualTo(1));
            Assert.That(validationResult.PersonsWithMissingBaseData, Has.Count.EqualTo(0));
            var missingPerson = validationResult.PersonsWithMissingInterests.Single();
            Assert.That(missingPerson.Id, Is.EqualTo(person.Id));
            Assert.That(missingPerson.GivenName, Is.EqualTo(person.GivenName));
            Assert.That(missingPerson.Surname, Is.EqualTo(person.Surname));
        }
    }

    [Test]
    public async Task ValidateCandidateList_SelectedCandidateMissingBaseData_ShouldIncludePersonInMissingBaseData()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var candidateIds = new List<Guid> { candidateId };

        var committeeType = new CommitteeTypeBuilder()
            .WithId(CommitteeType.ManagementCommitteeGuid)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithCommitteeType(committeeType)
            .WithSecretariatReadyForProposalDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            .WithVacanciesGeneralElection(null)
            .Build();

        var person = new PersonBuilder()
            .WithNoInterest(true)
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .WithGeneralElectionCommittee(generalElectionCommittee)
            .Build();

        generalElectionCommittee.MembershipCandidates.Add(membershipCandidate);

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByPersonId(person.Id).Returns([]);
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        var validationResult = await _service.ValidateCandidateList(committeeId, candidateIds, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(validationResult.PersonsWithMissingInterests, Has.Count.EqualTo(0));
            Assert.That(validationResult.PersonsWithMissingBaseData, Has.Count.EqualTo(1));
            var missingPerson = validationResult.PersonsWithMissingBaseData.Single();
            Assert.That(missingPerson.Id, Is.EqualTo(person.Id));
            Assert.That(missingPerson.GivenName, Is.EqualTo(person.GivenName));
            Assert.That(missingPerson.Surname, Is.EqualTo(person.Surname));
        }
    }

    [Test]
    public async Task ValidateCandidateList_CandidateMissingPersonId_ShouldSkipPersonTaskChecks()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var candidateIds = new List<Guid> { candidateId };

        var person = new PersonBuilder()
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .Build();
        membershipCandidate.PersonId = null;

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithCandidateListStateId(CandidateListState.Validated)
            .WithMembershipCandidates(new List<MembershipCandidate> { membershipCandidate })
            .WithVacanciesGeneralElection(null)
            .Build();

        membershipCandidate.GeneralElectionCommittee = generalElectionCommittee;
        membershipCandidate.GeneralElectionCommitteeId = generalElectionCommittee.Id;

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns([]);

        var validationResult = await _service.ValidateCandidateList(committeeId, candidateIds, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(validationResult.PersonsWithMissingInterests, Has.Count.EqualTo(0));
            Assert.That(validationResult.PersonsWithMissingBaseData, Has.Count.EqualTo(0));
        }

        await _worklistTaskRepository.Received(1).GetAllByPersonId(person.Id);
        await _worklistTaskRepository.DidNotReceive().Create(Arg.Is<WorklistTask>(t =>
            t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonInterests ||
            t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonBaseData));
    }

    [Test]
    public async Task ValidateCandidateList_NotSelectedCandidateWithActiveTasks_ShouldCompleteTasks()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();

        var person = new PersonBuilder()
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();
        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .WithIsSelected(true)
            .Build();

        var task = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionPersonInterests)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithMembershipCandidates(new List<MembershipCandidate> { membershipCandidate })
            .WithVacanciesGeneralElection(null)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByPersonId(person.Id).Returns([task]);
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);

        await _service.ValidateCandidateList(committeeId, [], true);

        Assert.That(task.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
    }

    [Test]
    public async Task ValidateCandidateList_SelectedCandidateWithInactiveTasks_ShouldActivateTasks()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var candidateIds = new List<Guid> { candidateId };

        var person = new PersonBuilder()
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .Build();

        var interestsTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionPersonInterests)
            .WithWorklistTaskStateId(WorklistTaskState.Completed)
            .Build();

        var baseDataTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionPersonBaseData)
            .WithWorklistTaskStateId(WorklistTaskState.Completed)
            .Build();

        var candidateListTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.CandidateListCreate)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithMembershipCandidates(new List<MembershipCandidate> { membershipCandidate })
            .WithSecretariatReadyForProposalDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            .WithVacanciesGeneralElection(null)
            .WithCommitteeType(new CommitteeTypeBuilder().WithId(CommitteeType.AuthoritiesCommissionGuid)
                .WithFemaleAndMaleThreshold(0, 0).WithLanguagesThreshold(null, null, null, null).WithLanguagesPercentageThreshold(null, null, null, null).Build())
            .Build();
        membershipCandidate.GeneralElectionCommittee = generalElectionCommittee;

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByPersonId(person.Id).Returns([interestsTask, baseDataTask]);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns(new List<WorklistTask> { candidateListTask });
        _worklistTaskRepository.GetByWorklistTaskTypeId(WorklistTaskType.GeneralElectionStart).Returns([new WorklistTaskBuilder().Build()]);
        _personService.GetDuplicatePersonForGeneralElection(membershipCandidate).Returns(new CandidateListDuplicateCheckResultDto { DuplicateReason = DuplicateReason.NoDuplicateFound });
        _personService.CreatePersonInGeneralElection(membershipCandidate).Returns(person);

        await _service.ValidateCandidateList(committeeId, candidateIds, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(interestsTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Active));
            Assert.That(baseDataTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Active));
        }

        await _worklistTaskRepository.DidNotReceive().Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonInterests));
        await _worklistTaskRepository.DidNotReceive().Create(Arg.Is<WorklistTask>(t => t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonBaseData));
    }

    [Test]
    public async Task ValidateCandidateList_SelectedCandidateMissingMaximumEmploymentLevel_ShouldCreateMembershipValidationTask()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();

        var committeeType = new CommitteeTypeBuilder().WithId(Guid.NewGuid()).Build();
        var committee = new CommitteeBuilder()
            .WithId(Guid.NewGuid())
            .WithCommitteeType(committeeType)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithCommittee(committee)
            .WithCommitteeType(committeeType)
            .WithCommitteeId(committee.Id)
            .WithCommitteeTypeId(committeeType.Id)
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithMarketOrientated(true)
            .WithSecretariatReadyForProposalDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            .WithCandidateListStateId(CandidateListState.Validated)
            .WithVacanciesGeneralElection(null)
            .Build();

        var person = new PersonBuilder()
            .WithGivenName("Max")
            .WithSurname("Muster")
            .WithNoInterest(true)
            .WithNoEmployment(true)
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .WithIsSelected(true)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .WithGeneralElectionCommittee(generalElectionCommittee)
            .Build();

        membershipCandidate.MaximumEmploymentLevel = null;
        generalElectionCommittee.MembershipCandidates.Add(membershipCandidate);

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns([]);

        var validationResult = await _service.ValidateCandidateList(committeeId, new List<Guid> { candidateId }, true);

        await _worklistTaskRepository.Received(1).Create(Arg.Is<WorklistTask>(t =>
            t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMembershipValidation &&
            t.MembershipCandidateId == candidateId));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(validationResult.PersonsWithMembershipValidationIssues, Has.Count.EqualTo(1));
            var missingPerson = validationResult.PersonsWithMembershipValidationIssues.Single();
            Assert.That(missingPerson.Id, Is.EqualTo(person.Id));
            Assert.That(missingPerson.GivenName, Is.EqualTo(person.GivenName));
            Assert.That(missingPerson.Surname, Is.EqualTo(person.Surname));
        }
    }

    [Test]
    public async Task ValidateCandidateList_SelectedCandidateMissingInterests_ShouldSkipMembershipValidationTasks()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();

        var committeeType = new CommitteeTypeBuilder().WithId(CommitteeType.ManagementCommitteeGuid).Build();
        var committee = new CommitteeBuilder()
            .WithId(Guid.NewGuid())
            .WithCommitteeType(committeeType)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithCommittee(committee)
            .WithCommitteeType(committeeType)
            .WithCommitteeId(committee.Id)
            .WithCommitteeTypeId(committeeType.Id)
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithMarketOrientated(true)
            .WithSecretariatReadyForProposalDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            .WithCandidateListStateId(CandidateListState.Validated)
            .WithVacanciesGeneralElection(null)
            .Build();

        var person = new PersonBuilder()
            .WithNoEmployment(true)
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .WithIsSelected(true)
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .WithGeneralElectionCommittee(generalElectionCommittee)
            .Build();

        membershipCandidate.MaximumEmploymentLevel = null;
        generalElectionCommittee.MembershipCandidates.Add(membershipCandidate);

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByPersonId(person.Id).Returns([]);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns([]);

        var validationResult = await _service.ValidateCandidateList(committeeId, new List<Guid> { candidateId }, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(validationResult.PersonsWithMissingInterests, Has.Count.EqualTo(1));
            Assert.That(validationResult.PersonsWithMembershipValidationIssues, Has.Count.EqualTo(0));
        }

        await _worklistTaskRepository.DidNotReceive().Create(Arg.Is<WorklistTask>(t =>
            t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMembershipValidation));
    }

    [Test]
    public async Task ValidateCandidateList_SelectedCandidateWithMembershipValidationIssue_ShouldActivateTask()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();

        var committeeType = new CommitteeTypeBuilder().WithId(CommitteeType.NonExtraParliamentaryCommitteeGuid).Build();
        var committee = new CommitteeBuilder()
            .WithId(Guid.NewGuid())
            .WithCommitteeType(committeeType)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithCommittee(committee)
            .WithCommitteeType(committeeType)
            .WithCommitteeId(committee.Id)
            .WithCommitteeTypeId(committeeType.Id)
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithMarketOrientated(true)
            .WithSecretariatReadyForProposalDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            .WithCandidateListStateId(CandidateListState.Validated)
            .WithVacanciesGeneralElection(null)
            .Build();

        var person = new PersonBuilder()
            .WithNoInterest(true)
            .WithNoEmployment(true)
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .WithIsSelected(true)
            .WithElectionTypeId(ElectionType.ReElectionGuid)
            .WithGeneralElectionCommittee(generalElectionCommittee)
            .Build();

        membershipCandidate.MaximumEmploymentLevel = null;
        generalElectionCommittee.MembershipCandidates.Add(membershipCandidate);

        var membershipTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMembershipValidation)
            .WithWorklistTaskStateId(WorklistTaskState.Completed)
            .WithMembershipCandidateId(candidateId)
            .WithGeneralElectionCommittee(generalElectionCommittee)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns([membershipTask]);

        var validationResult = await _service.ValidateCandidateList(committeeId, new List<Guid> { candidateId }, true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(membershipTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Active));
            Assert.That(validationResult.PersonsWithMembershipValidationIssues, Has.Count.EqualTo(1));
        }

        await _worklistTaskRepository.DidNotReceive().Create(Arg.Is<WorklistTask>(t =>
            t.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMembershipValidation));
    }

    [Test]
    public async Task ValidateCandidateList_SelectedCandidateWithoutMembershipValidationIssue_ShouldCompleteMembershipTask()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();

        var committeeType = new CommitteeTypeBuilder().WithId(CommitteeType.NonExtraParliamentaryCommitteeGuid).Build();
        var committee = new CommitteeBuilder()
            .WithId(Guid.NewGuid())
            .WithCommitteeType(committeeType)
            .Build();
        var termOfOffice = new TermOfOfficeBuilder().WithId(Guid.NewGuid()).Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithCommittee(committee)
            .WithCommitteeType(committeeType)
            .WithCommitteeId(committee.Id)
            .WithCommitteeTypeId(committeeType.Id)
            .WithTermOfOffice(termOfOffice)
            .WithTermOfOfficeId(termOfOffice.Id)
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithMarketOrientated(false)
            .WithSecretariatReadyForProposalDueDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            .WithCandidateListStateId(CandidateListState.Validated)
            .WithVacanciesGeneralElection(null)
            .Build();

        var person = new PersonBuilder()
            .WithNoInterest(true)
            .WithNoEmployment(true)
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .WithIsSelected(true)
            .WithElectionTypeId(ElectionType.ReElectionGuid)
            .WithGeneralElectionCommittee(generalElectionCommittee)
            .Build();

        membershipCandidate.MaximumEmploymentLevel = 60;
        generalElectionCommittee.MembershipCandidates.Add(membershipCandidate);

        var membershipTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMembershipValidation)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithMembershipCandidateId(candidateId)
            .WithGeneralElectionCommittee(generalElectionCommittee)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns([membershipTask]);

        await _service.ValidateCandidateList(committeeId, new List<Guid> { candidateId }, true);

        Assert.That(membershipTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
    }

    [Test]
    public async Task ValidateCandidateList_NotSelectedCandidateWithMembershipValidationTask_ShouldCompleteTask()
    {
        var committeeId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();

        var person = new PersonBuilder()
            .WithNoInterest(true)
            .WithNoEmployment(true)
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var membershipCandidate = new MembershipCandidateBuilder()
            .WithId(candidateId)
            .WithPerson(person)
            .WithIsSelected(true)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithMinimalMember(0)
            .WithMaximalMember(10)
            .WithMembershipCandidates(new List<MembershipCandidate> { membershipCandidate })
            .WithCandidateListStateId(CandidateListState.Validated)
            .WithVacanciesGeneralElection(null)
            .Build();
        membershipCandidate.GeneralElectionCommittee = generalElectionCommittee;

        var membershipTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMembershipValidation)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .WithMembershipCandidateId(candidateId)
            .WithGeneralElectionCommittee(generalElectionCommittee)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeIdForUpdate(committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommittee.Id).Returns([membershipTask]);

        await _service.ValidateCandidateList(committeeId, [], true);

        Assert.That(membershipTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
    }
}
