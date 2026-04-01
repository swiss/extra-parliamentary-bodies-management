using Bk.APG.Api.Controllers;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class OccupationControllerTests
{
    private readonly IOccupationService _occupationService = Substitute.For<IOccupationService>();

    private OccupationController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new OccupationController(_occupationService);
    }

    [TearDown]
    public void TearDown()
    {
        _occupationService.ClearSubstitute();
    }

    [Test]
    public async Task GetBySearchString_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var response = await _controller.GetBySearchString("clark");

        await _occupationService.Received().GetBySearchString("clark");

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
        });
    }
}
