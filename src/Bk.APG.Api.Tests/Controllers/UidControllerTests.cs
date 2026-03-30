using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Infrastructure.Service.UID.Service;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class UidControllerTests
{
    private readonly IUidService _uidService = Substitute.For<IUidService>();

    private UidController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new UidController(_uidService);
    }

    [TearDown]
    public void TearDown()
    {
        _uidService.ClearSubstitute();
    }

    [Test]
    public async Task GetBySearchString_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var interests = new Faker<UidDto>().Generate(10);

        _uidService.Search("A").Returns(interests);

        var response = await _controller.Search("A");

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(interests));
        });
    }
}
