using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class InformationControllerTests
{
    private InformationController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new InformationController();
    }

    [Test]
    public void GetApplicationVersion_WhenEnvVariableIsDefined_ReturnsVersion()
    {
        Environment.SetEnvironmentVariable("APPLICATION_VERSION", "v1");

        var result = _controller.GetApplicationVersion();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        var value = ((OkObjectResult)result).Value;

        Assert.That(value, Is.Not.Null);
        Assert.That(value, Is.InstanceOf<VersionDto>());
        Assert.That(((VersionDto)value).ApplicationVersion, Is.EqualTo("v1"));
    }

    [Test]
    public void GetApplicationVersion_WhenEnvVariableIsNotSet_ReturnsEmptyVersion()
    {
        Environment.SetEnvironmentVariable("APPLICATION_VERSION", null);

        var result = _controller.GetApplicationVersion();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        var value = ((OkObjectResult)result).Value;

        Assert.That(value, Is.Not.Null);
        Assert.That(value, Is.InstanceOf<VersionDto>());
        Assert.That(((VersionDto)value).ApplicationVersion, Is.Null);
    }
}
