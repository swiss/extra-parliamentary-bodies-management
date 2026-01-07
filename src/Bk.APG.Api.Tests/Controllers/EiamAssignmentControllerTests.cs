using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class EiamAssignmentControllerTests
{
    private readonly IEiamAssignmentService _eiamAssignmentService = Substitute.For<IEiamAssignmentService>();

    private EiamAssignmentController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new EiamAssignmentController(_eiamAssignmentService);
    }

    [TearDown]
    public void TearDown()
    {
        _eiamAssignmentService.ClearSubstitute();
    }

    [Test]
    public async Task GetAvailableEiamAssignments_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var expectedResult = new List<EiamAssignmentDto>
        {
            new() { Id = Guid.NewGuid(), Text = "text" },
            new() { Id = Guid.NewGuid(), Text = "text2" },
        };
        _eiamAssignmentService.GetAvailableAssignments().Returns(expectedResult);

        var response = await _controller.GetAvailableEiamAssignments();

        await _eiamAssignmentService.Received(1).GetAvailableAssignments();

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;
        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(expectedResult));
        });
    }
}
