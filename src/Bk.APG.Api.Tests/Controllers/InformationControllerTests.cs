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

    [TestCase("1.1.0_hash", "1.1.0 (hash)")]
    [TestCase("1.1.0_timestamp_hash", "1.1.0 (timestamp_hash)")]
    public void GetApplicationVersion_WhenEnvVariableIsDefined_ReturnsVersion(string envVariable, string expectedDisplayVersion)
    {
        Environment.SetEnvironmentVariable("APPLICATION_VERSION", envVariable);

        var result = _controller.GetApplicationVersion();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        var value = ((OkObjectResult)result).Value;

        Assert.That(value, Is.Not.Null);
        Assert.That(value, Is.InstanceOf<VersionDto>());
        Assert.That(((VersionDto)value).ApplicationVersion, Is.EqualTo(expectedDisplayVersion));
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
        Assert.That(((VersionDto)value).ApplicationVersion, Is.EqualTo(string.Empty));
    }
}
