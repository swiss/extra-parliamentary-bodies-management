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
}
