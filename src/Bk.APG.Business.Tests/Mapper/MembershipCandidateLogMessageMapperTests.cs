using Bk.APG.Business.Mapper;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class MembershipCandidateLogMessageMapperTests
{
    [Test]
    public void ToMembershipCandidateLogMessage_ShouldMapCorrectly()
    {
        var committeeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var logMessage = "Person ABC wurde nicht übernommen, maximale Amtszeit überschritten!";
        var currentUser = "Franz Tester";

        var mappedLogMessage = MembershipCandidateLogMessageMapper.ToMembershipCandidateLogMessage(committeeId, personId, logMessage, currentUser);

        Assert.That(mappedLogMessage, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(mappedLogMessage.GeneralElectionCommitteeId, Is.EqualTo(committeeId));
            Assert.That(mappedLogMessage.PersonId, Is.EqualTo(personId));
            Assert.That(mappedLogMessage.LogMessage, Is.EqualTo(logMessage));
            Assert.That(mappedLogMessage.CreatedBy, Is.EqualTo(currentUser));
        });
    }
}
