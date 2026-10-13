using Bk.APG.Business.Extensions;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Tests.Extensions;

[TestFixture]
internal class ElectionTypeExtensionsTests
{
    private static readonly Guid[] AllElectionTypeIds =
    [
        ElectionType.NewElectionGuid,
        ElectionType.ReElectionGuid,
        ElectionType.RetirementGuid,
        ElectionType.MaximumMembershipDurationGuid,
        ElectionType.CommitteeDissolutionWithdrawalGuid,
        ElectionType.MembershipEndedBecauseOfDeathGuid,
        ElectionType.OtherRetirementReasonGuid,
        ElectionType.PermanentGuid
    ];

    [Test]
    public void GetElectionTypesForPresent_WithAllElectionTypes_ReturnsOnlyRetirementTypes()
    {
        var result = AllElectionTypeIds.GetElectionTypesForPresent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result, Does.Contain(ElectionType.RetirementGuid));
            Assert.That(result, Does.Contain(ElectionType.MaximumMembershipDurationGuid));
            Assert.That(result, Does.Contain(ElectionType.CommitteeDissolutionWithdrawalGuid));
            Assert.That(result, Does.Contain(ElectionType.OtherRetirementReasonGuid));
            Assert.That(result, Does.Not.Contain(ElectionType.NewElectionGuid));
            Assert.That(result, Does.Not.Contain(ElectionType.ReElectionGuid));
            Assert.That(result, Does.Not.Contain(ElectionType.MembershipEndedBecauseOfDeathGuid));
            Assert.That(result, Does.Not.Contain(ElectionType.PermanentGuid));
        });
    }

    [Test]
    public void GetElectionTypesForFuture_WithAllElectionTypes_ReturnsOnlyNewAndReElection()
    {
        var result = AllElectionTypeIds.GetElectionTypesForFuture();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result, Does.Contain(ElectionType.NewElectionGuid));
            Assert.That(result, Does.Contain(ElectionType.ReElectionGuid));
            Assert.That(result, Does.Not.Contain(ElectionType.RetirementGuid));
            Assert.That(result, Does.Not.Contain(ElectionType.MaximumMembershipDurationGuid));
            Assert.That(result, Does.Not.Contain(ElectionType.CommitteeDissolutionWithdrawalGuid));
            Assert.That(result, Does.Not.Contain(ElectionType.OtherRetirementReasonGuid));
            Assert.That(result, Does.Not.Contain(ElectionType.MembershipEndedBecauseOfDeathGuid));
            Assert.That(result, Does.Not.Contain(ElectionType.PermanentGuid));
        });
    }

    [Test]
    public void GetElectionTypesForPresent_WithFilteredInput_ReturnsOnlyIntersectingTypes()
    {
        var input = new List<Guid> { ElectionType.RetirementGuid, ElectionType.NewElectionGuid };

        var result = input.GetElectionTypesForPresent();

        Assert.That(result, Is.EqualTo(new List<Guid> { ElectionType.RetirementGuid }));
    }

    [Test]
    public void GetElectionTypesForFuture_WithFilteredInput_ReturnsOnlyIntersectingTypes()
    {
        var input = new List<Guid> { ElectionType.RetirementGuid, ElectionType.ReElectionGuid };

        var result = input.GetElectionTypesForFuture();

        Assert.That(result, Is.EqualTo(new List<Guid> { ElectionType.ReElectionGuid }));
    }

    [Test]
    public void GetElectionTypesForPresent_WithEmptyInput_ReturnsEmptyList()
    {
        var result = Array.Empty<Guid>().GetElectionTypesForPresent();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetElectionTypesForFuture_WithEmptyInput_ReturnsEmptyList()
    {
        var result = Array.Empty<Guid>().GetElectionTypesForFuture();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetElectionTypesForPresent_DoesNotMutateInputList()
    {
        var input = new List<Guid>(AllElectionTypeIds);

        _ = input.GetElectionTypesForPresent();

        Assert.That(input, Is.EqualTo(AllElectionTypeIds));
    }

    [Test]
    public void GetElectionTypesForFuture_DoesNotMutateInputList()
    {
        var input = new List<Guid>(AllElectionTypeIds);

        _ = input.GetElectionTypesForFuture();

        Assert.That(input, Is.EqualTo(AllElectionTypeIds));
    }

    [Test]
    public void GetElectionTypesForPresent_WithNullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IEnumerable<Guid>)null!).GetElectionTypesForPresent());
    }

    [Test]
    public void GetElectionTypesForFuture_WithNullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IEnumerable<Guid>)null!).GetElectionTypesForFuture());
    }
}
