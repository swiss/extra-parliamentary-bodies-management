using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class MembershipCandidatesControllerTests
{
    private readonly IMembershipCandidateService _membershipCandidateService = Substitute.For<IMembershipCandidateService>();

    private MembershipCandidatesController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new MembershipCandidatesController(_membershipCandidateService);
    }

    [TearDown]
    public void TearDown()
    {
        _membershipCandidateService.ClearSubstitute();
    }

    [Test]
    public async Task UpdateCandidate_ShouldCallServiceAndReturnNoContent()
    {
        var membershipCandidateId = Guid.NewGuid();
        var partialUpdate = new MembershipCandidatePartialUpdateDto
        {
            FunctionId = Guid.NewGuid()
        };

        var result = await _controller.PartialUpdateMembershipCandidate(membershipCandidateId, partialUpdate) as NoContentResult;

        await _membershipCandidateService.Received(1).PartialUpdateMembershipCandidate(membershipCandidateId, partialUpdate);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(204));
    }
}
