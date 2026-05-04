using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
public class EiamAssignmentServiceTests
{
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IWorklistTaskRepository _worklistTaskRepository = Substitute.For<IWorklistTaskRepository>();
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository = Substitute.For<IGeneralElectionCommitteeRepository>();

    private EiamAssignmentService _service = null!;

    private readonly Guid _zeroGuid = Guid.Empty;

    [SetUp]
    public void SetUp()
    {
        _service = new EiamAssignmentService(_authorizationService, _worklistTaskRepository, _generalElectionCommitteeRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _authorizationService.ClearSubstitute();
    }

    [Test]
    public async Task GetAvailableAssignments_ShouldReturnAssignableAssignments()
    {
        var parentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "parent-external-id",
            Role = Role.Department
        };
        var childAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "child-external-id",
            Role = Role.Office,
            Parent = parentAssignment
        };
        var currentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "current-external-id",
            Role = Role.Office,
            Parent = parentAssignment,
            Children = new List<EiamAssignment> { childAssignment }
        };

        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);

        var result = await _service.GetAvailableAssignments();

        await _authorizationService.Received(1).GetCurrentEiamAssignment();

        var assignmentsList = result.ToList();
        Assert.That(assignmentsList, Is.Not.Null);
        Assert.That(assignmentsList, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(assignmentsList.Any(a => a.Id == parentAssignment.Id), Is.True);
            Assert.That(assignmentsList.Any(a => a.Id == childAssignment.Id), Is.True);
        });
    }

    [Test]
    public async Task GetAllForCandidateListForward_WithDepartmentRole_ShouldReturnOfficeOrSecretariat()
    {
        var committeeId = Guid.NewGuid();
        var secretariatAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "secretariat-external-id",
            Role = Role.Secretariat,
            CommitteeId = committeeId
        };
        var officeAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "office-external-id",
            Role = Role.Office,
            Children = new List<EiamAssignment> { secretariatAssignment }
        };

        var currentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "department-external-id",
            Role = Role.Department,
            Department = new DepartmentBuilder().WithIsBigDepartment(true).Build(),
            Children = new List<EiamAssignment> { officeAssignment }
        };

        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);

        var result = (await _service.GetAllForCandidateListForward(committeeId)).ToList();

        await _authorizationService.Received(1).GetCurrentEiamAssignment();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(officeAssignment.Id));
    }

    [Test]
    public async Task GetAllForCandidateListForward_WithDepartmentRole_AndSmallDepartment_ShouldReturnSecretariat()
    {
        var committeeId = Guid.NewGuid();
        var secretariatAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "secretariat-external-id",
            Role = Role.Secretariat,
            CommitteeId = committeeId
        };
        var officeAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "office-external-id",
            Role = Role.Office,
            Children = new List<EiamAssignment> { secretariatAssignment }
        };
        var currentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "department-external-id",
            Role = Role.Department,
            Department = new DepartmentBuilder().WithIsBigDepartment(false).Build(),
            Children = new List<EiamAssignment> { officeAssignment }
        };

        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);

        var result = (await _service.GetAllForCandidateListForward(committeeId)).ToList();

        await _authorizationService.Received(1).GetCurrentEiamAssignment();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(secretariatAssignment.Id));
    }

    [Test]
    public async Task GetAllForCandidateListForward_WithOfficeRole_ShouldReturnDepartmentAndSecretariat()
    {
        var committeeId = Guid.NewGuid();
        var departmentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "department-external-id",
            Role = Role.Department
        };
        var secretariatAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "secretariat-external-id",
            Role = Role.Secretariat,
            CommitteeId = committeeId
        };
        var currentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "office-external-id",
            Role = Role.Office,
            Parent = departmentAssignment,
            Children = new List<EiamAssignment> { secretariatAssignment }
        };

        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);

        var result = (await _service.GetAllForCandidateListForward(committeeId)).ToList();

        await _authorizationService.Received(1).GetCurrentEiamAssignment();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result.Any(a => a.Id == departmentAssignment.Id), Is.True);
            Assert.That(result.Any(a => a.Id == secretariatAssignment.Id), Is.True);
        });
    }

    [Test]
    public async Task GetAllForCandidateListForward_WithSecretariatRole_AndBigDepartment_ShouldReturnOffice()
    {
        var committeeId = Guid.NewGuid();
        var officeAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "office-external-id",
            Role = Role.Office
        };
        var currentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "secretariat-external-id",
            Role = Role.Secretariat,
            CommitteeId = committeeId,
            Parent = officeAssignment,
            Committee = new CommitteeBuilder()
                .WithDepartment(new DepartmentBuilder().WithIsBigDepartment(true).Build()).Build()
        };

        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);

        var result = (await _service.GetAllForCandidateListForward(committeeId)).ToList();

        await _authorizationService.Received(1).GetCurrentEiamAssignment();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(officeAssignment.Id));
    }

    [Test]
    public async Task GetAllForCandidateListForward_WithSecretariatRole_AndSmallDepartment_ShouldReturnDepartment()
    {
        var committeeId = Guid.NewGuid();
        var departmentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "department-external-id",
            Role = Role.Department
        };
        var officeAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "office-external-id",
            Role = Role.Office,
            Parent = departmentAssignment
        };
        var currentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "secretariat-external-id",
            Role = Role.Secretariat,
            CommitteeId = committeeId,
            Parent = officeAssignment,
            Committee = new CommitteeBuilder()
                .WithDepartment(new DepartmentBuilder().WithIsBigDepartment(false).Build()).Build()
        };

        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);

        var result = (await _service.GetAllForCandidateListForward(committeeId)).ToList();

        await _authorizationService.Received(1).GetCurrentEiamAssignment();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(departmentAssignment.Id));
    }

    [Test]
    public async Task GetAllForReadyForProposalForward_WithDepartmentRole_ShouldReturnParentAndSecretariat()
    {
        var committeeId = Guid.NewGuid();
        var parentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "parent-external-id",
            Role = Role.Admin
        };
        var secretariatAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "secretariat-external-id",
            Role = Role.Secretariat,
            CommitteeId = committeeId
        };
        var officeAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "office-external-id",
            Role = Role.Office,
            Children = new List<EiamAssignment> { secretariatAssignment }
        };
        var currentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "department-external-id",
            Role = Role.Department,
            Parent = parentAssignment,
            Department = new DepartmentBuilder().WithIsBigDepartment(false).Build(),
            Children = new List<EiamAssignment> { officeAssignment }
        };
        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);

        _worklistTaskRepository.GetAllByCommitteeId(committeeId).Returns([
            new WorklistTaskBuilder().WithWorklistTaskStateId(WorklistTaskState.Completed)
                .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
                .WithAssignedTo(new EiamAssignmentBuilder().WithRole(Role.Department).Build()).Build(),
            new WorklistTaskBuilder().WithWorklistTaskStateId(WorklistTaskState.Completed)
                .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
                .WithAssignedTo(new EiamAssignmentBuilder().WithRole(Role.Office).Build()).Build(),
            new WorklistTaskBuilder().WithWorklistTaskStateId(WorklistTaskState.Completed)
                .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
                .WithAssignedTo(new EiamAssignmentBuilder().WithRole(Role.Secretariat).Build()).Build()
            ]);

        var result = (await _service.GetAllForReadyForProposalForward(committeeId)).ToList();

        await _authorizationService.Received(1).GetCurrentEiamAssignment();

        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Any(a => a.Id == parentAssignment.Id), Is.True);
            Assert.That(result.Any(a => a.Id == secretariatAssignment.Id), Is.True);
        }
    }

    [Test]
    public async Task GetCurrentEiamAssignment_ShouldReturnCurrentAssignment()
    {
        var currentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "current-external-id",
            Role = Role.Department,
            DepartmentId = Guid.NewGuid()
        };

        _authorizationService.GetCurrentEiamAssignment().Returns(currentAssignment);

        var result = await _service.GetCurrentEiamAssignment();

        await _authorizationService.Received(1).GetCurrentEiamAssignment();

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(currentAssignment.Id));
            Assert.That(result.DepartmentId, Is.EqualTo(currentAssignment.DepartmentId));
        });
    }

    [Test]
    public async Task GetPermittedIds_ForAdminRole_ShouldReturnData()
    {
        _authorizationService.IsAdmin.Returns(true);
        _authorizationService.IsObserver.Returns(false);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(false);
        _authorizationService.IsSecretariat.Returns(false);

        var (departmentId, officeId, committeeId) = await _service.GetPermittedIds();

        Assert.That(departmentId, Is.EqualTo(_zeroGuid));
        Assert.That(officeId, Is.EqualTo(_zeroGuid));
        Assert.That(committeeId, Is.EqualTo(_zeroGuid));
    }

    [Test]
    public async Task GetPermittedIds_ForObserverRole_ShouldReturnData()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsObserver.Returns(true);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(false);
        _authorizationService.IsSecretariat.Returns(false);

        var (departmentId, officeId, committeeId) = await _service.GetPermittedIds();

        Assert.That(departmentId, Is.EqualTo(_zeroGuid));
        Assert.That(officeId, Is.EqualTo(_zeroGuid));
        Assert.That(committeeId, Is.EqualTo(_zeroGuid));
    }

    [Test]
    public async Task GetPermittedIds_ForDepartmentRole_ShouldReturnData()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsObserver.Returns(false);
        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.IsOffice.Returns(false);
        _authorizationService.IsSecretariat.Returns(false);

        var eiamId = Guid.Parse("93EB1745-69E1-4AA4-8102-421172F182D3");

        var eiamAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "1234",
            Role = Role.Department,
            DepartmentId = eiamId
        };

        _authorizationService.GetCurrentEiamAssignment().Returns(eiamAssignment);

        var (departmentId, officeId, committeeId) = await _service.GetPermittedIds();

        Assert.That(departmentId, Is.EqualTo(eiamId));
        Assert.That(officeId, Is.EqualTo(_zeroGuid));
        Assert.That(committeeId, Is.EqualTo(_zeroGuid));
    }

    [Test]
    public async Task GetPermittedIds_ForOfficeRole_ShouldReturnData()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsObserver.Returns(false);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(true);
        _authorizationService.IsSecretariat.Returns(false);

        var eiamId = Guid.Parse("6058701B-D38D-45E0-9C68-9C7495E286F0");

        var eiamAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "1234",
            Role = Role.Office,
            OfficeId = eiamId
        };

        _authorizationService.GetCurrentEiamAssignment().Returns(eiamAssignment);

        var (departmentId, officeId, committeeId) = await _service.GetPermittedIds();

        Assert.That(departmentId, Is.EqualTo(_zeroGuid));
        Assert.That(officeId, Is.EqualTo(eiamId));
        Assert.That(committeeId, Is.EqualTo(_zeroGuid));
    }

    [Test]
    public async Task GetPermittedIds_ForSecretariatRole_ShouldReturnData()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsObserver.Returns(false);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(false);
        _authorizationService.IsSecretariat.Returns(true);

        var eiamId = Guid.Parse("04C755DE-E771-4A4C-B91B-7CBB9D34E28B");

        var eiamAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "1234",
            Role = Role.Secretariat,
            CommitteeId = eiamId
        };

        _authorizationService.GetCurrentEiamAssignment().Returns(eiamAssignment);

        var (departmentId, officeId, committeeId) = await _service.GetPermittedIds();

        Assert.That(departmentId, Is.EqualTo(_zeroGuid));
        Assert.That(officeId, Is.EqualTo(_zeroGuid));
        Assert.That(committeeId, Is.EqualTo(eiamId));
    }
}
