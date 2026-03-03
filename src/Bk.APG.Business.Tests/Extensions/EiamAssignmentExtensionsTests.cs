using Bk.APG.Business.Extensions;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Extensions;

[TestFixture]
public class EiamAssignmentExtensionsTests
{
    [Test]
    public void GetSearchableIds_WithAdminRole_ReturnsOnlyAssignmentId()
    {
        var assignmentId = Guid.NewGuid();
        var eiamAssignment = new EiamAssignmentBuilder()
            .WithId(assignmentId)
            .Build();

        var result = eiamAssignment.GetSearchableIds().ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo(assignmentId));
    }

    [Test]
    public void GetAssignableIds_WithNoParentOrChildren_ReturnsEmptyCollection()
    {
        var eiamAssignment = new EiamAssignmentBuilder().Build();

        var result = eiamAssignment.GetAssignableIds().ToList();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetAssignableIds_WithParentAndChildren_ReturnsParentAndChildren()
    {
        var parent = new EiamAssignmentBuilder().Build();
        var child1 = new EiamAssignmentBuilder().Build();
        var child2 = new EiamAssignmentBuilder().Build();

        var eiamAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "test",
            Role = Role.Department,
            Parent = parent,
            Children = new List<EiamAssignment> { child1, child2 }
        };

        var result = eiamAssignment.GetAssignableIds().ToList();

        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result, Contains.Item(parent));
        Assert.That(result, Contains.Item(child1));
        Assert.That(result, Contains.Item(child2));
    }

    [Test]
    public void GetAssignmentsForCandidateListForward_WithDepartmentRole_ReturnsSecretariat()
    {
        var department = new DepartmentBuilder()
            .WithIsBigDepartment(false)
            .Build();

        var secretariatAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Secretariat)
            .WithCommittee(new CommitteeBuilder().WithDepartment(department).Build())
            .Build();

        var officeAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Office)
            .WithChildren([secretariatAssignment])
            .Build();

        var departmentAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Department)
            .WithDepartment(department)
            .WithChildren([officeAssignment])
            .Build();

        var result = departmentAssignment.GetAssignmentsForCandidateListForward(secretariatAssignment.CommitteeId!.Value).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo(secretariatAssignment));
    }

    [Test]
    public void GetAssignmentsForReadyForProposalForward_WithDepartmentRole_AndSmallDepartment_ReturnsParentAndSecretariat()
    {
        var department = new DepartmentBuilder()
            .WithIsBigDepartment(false)
            .Build();
        var parentAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Admin)
            .Build();

        var secretariatAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Secretariat)
            .WithCommittee(new CommitteeBuilder().WithDepartment(department).Build())
            .Build();

        var officeAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Office)
            .WithChildren([secretariatAssignment])
            .Build();

        var departmentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "department",
            Role = Role.Department,
            Parent = parentAssignment,
            Department = department,
            Children = new List<EiamAssignment> { officeAssignment }
        };

        var result = departmentAssignment.GetAssignmentsForReadyForProposalForward(secretariatAssignment.CommitteeId!.Value).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Contains.Item(parentAssignment));
            Assert.That(result, Contains.Item(secretariatAssignment));
        }
    }

    [Test]
    public void GetAssignmentsForReadyForProposalForward_WithDepartmentRole_AndBigDepartment_ReturnsParentAndOffice()
    {
        var department = new DepartmentBuilder()
            .WithIsBigDepartment(true)
            .Build();
        var parentAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Admin)
            .Build();

        var secretariatAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Secretariat)
            .WithCommittee(new CommitteeBuilder().WithDepartment(department).Build())
            .Build();

        var officeAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Office)
            .WithChildren([secretariatAssignment])
            .Build();

        var departmentAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "department",
            Role = Role.Department,
            Parent = parentAssignment,
            Department = department,
            Children = new List<EiamAssignment> { officeAssignment }
        };

        var result = departmentAssignment.GetAssignmentsForReadyForProposalForward(secretariatAssignment.CommitteeId!.Value).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Contains.Item(parentAssignment));
            Assert.That(result, Contains.Item(officeAssignment));
        }
    }

    [Test]
    public void GetAssignmentsForReadyForProposalForward_WithOfficeRole_ReturnsDepartmentAndSecretariat()
    {
        var committeeId = Guid.NewGuid();
        var departmentAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Department)
            .Build();
        var secretariatAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Secretariat)
            .WithCommittee(new CommitteeBuilder().WithId(committeeId).Build())
            .Build();
        var officeAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "office",
            Role = Role.Office,
            Parent = departmentAssignment,
            Children = new List<EiamAssignment> { secretariatAssignment }
        };

        var result = officeAssignment.GetAssignmentsForReadyForProposalForward(committeeId).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Contains.Item(departmentAssignment));
            Assert.That(result, Contains.Item(secretariatAssignment));
        }
    }

    [Test]
    public void GetAssignmentsForReadyForProposalForward_WithSecretariatRole_AndBigDepartment_ReturnsOffice()
    {
        var committeeId = Guid.NewGuid();
        var officeAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Office)
            .Build();
        var secretariatAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "secretariat",
            Role = Role.Secretariat,
            CommitteeId = committeeId,
            Parent = officeAssignment,
            Committee = new CommitteeBuilder()
                .WithId(committeeId)
                .WithDepartment(new DepartmentBuilder().WithIsBigDepartment(true).Build())
                .Build()
        };

        var result = secretariatAssignment.GetAssignmentsForReadyForProposalForward(committeeId).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo(officeAssignment));
    }

    [Test]
    public void GetAssignmentsForReadyForProposalForward_WithSecretariatRole_AndSmallDepartment_ReturnsDepartment()
    {
        var committeeId = Guid.NewGuid();
        var departmentAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Department)
            .Build();
        var officeAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "office",
            Role = Role.Office,
            Parent = departmentAssignment
        };
        var secretariatAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "secretariat",
            Role = Role.Secretariat,
            CommitteeId = committeeId,
            Parent = officeAssignment,
            Committee = new CommitteeBuilder()
                .WithId(committeeId)
                .WithDepartment(new DepartmentBuilder().WithIsBigDepartment(false).Build())
                .Build()
        };

        var result = secretariatAssignment.GetAssignmentsForReadyForProposalForward(committeeId).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo(departmentAssignment));
    }

    [Test]
    public void GetAssignmentsForReadyForProposalForward_WithAdminRole_ReturnsMatchingDepartment()
    {
        var committeeId = Guid.NewGuid();

        var nonMatchingSecretariat = new EiamAssignmentBuilder()
            .WithRole(Role.Secretariat)
            .WithCommittee(new CommitteeBuilder().WithId(Guid.NewGuid()).Build())
            .Build();
        var nonMatchingOffice = new EiamAssignmentBuilder()
            .WithRole(Role.Office)
            .WithChildren([nonMatchingSecretariat])
            .Build();
        var nonMatchingDepartment = new EiamAssignmentBuilder()
            .WithRole(Role.Department)
            .WithChildren([nonMatchingOffice])
            .Build();

        var matchingSecretariat = new EiamAssignmentBuilder()
            .WithRole(Role.Secretariat)
            .WithCommittee(new CommitteeBuilder().WithId(committeeId).Build())
            .Build();
        var matchingOffice = new EiamAssignmentBuilder()
            .WithRole(Role.Office)
            .WithChildren([matchingSecretariat])
            .Build();
        var matchingDepartment = new EiamAssignmentBuilder()
            .WithRole(Role.Department)
            .WithChildren([matchingOffice])
            .Build();

        var adminAssignment = new EiamAssignmentBuilder()
            .WithRole(Role.Admin)
            .WithChildren([nonMatchingDepartment, matchingDepartment])
            .Build();

        var result = adminAssignment.GetAssignmentsForReadyForProposalForward(committeeId).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo(matchingDepartment));
    }

    [Test]
    public void GetAssignmentsForReadyForProposalForward_WithObserverRole_ShouldThrow()
    {
        var assignment = new EiamAssignmentBuilder()
            .WithRole(Role.Observer)
            .Build();

        Assert.Throws<ArgumentOutOfRangeException>(() => assignment.GetAssignmentsForReadyForProposalForward(Guid.NewGuid()));
    }
}
