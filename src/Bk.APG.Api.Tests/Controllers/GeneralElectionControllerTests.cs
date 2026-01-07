using Bk.APG.Api.Controllers;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class GeneralElectionControllerTests
{
    private readonly IGeneralElectionService _generalElectionService = Substitute.For<IGeneralElectionService>();

    private GeneralElectionController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new GeneralElectionController(_generalElectionService);
    }

    [TearDown]
    public void TearDown()
    {
        _generalElectionService.ClearSubstitute();
    }

    [Test]
    public async Task GetIsGeneralElectionToggleAvailable_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        const bool expectedResult = true;
        _generalElectionService.IsGeneralElectionToggleAvailable().Returns(expectedResult);

        var response = await _controller.GetIsGeneralElectionToggleAvailable();

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;
        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(expectedResult));
        });
        await _generalElectionService.Received(1).IsGeneralElectionToggleAvailable();
    }
}
