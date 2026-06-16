using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute.ExceptionExtensions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class AddressServiceTests
{
    private readonly ILogger<AddressService> _logger = NullLogger<AddressService>.Instance;
    private readonly IPostService _postService = Substitute.For<IPostService>();

    private AddressService _addressService = null!;

    [SetUp]
    public void SetUp()
    {
        _addressService = new AddressService(_logger, _postService);
    }

    [TearDown]
    public void TearDown()
    {
        _postService.ClearSubstitute();
    }

    [Test]
    public async Task Search_WhenCalled_ShouldCallService()
    {
        var dto = new AddressSearchDto();
        _postService.Search(Arg.Any<AddressSearchDto>()).Returns([]);

        var result = await _addressService.Search(dto);

        Assert.That(result, Is.Not.Null);

        await _postService.Received(1).Search(Arg.Is(dto));
    }

    [Test]
    public async Task Verify_WhenCalled_ShouldCallService()
    {
        var dto = new AddressSearchDto();
        _postService.Verify(Arg.Any<AddressSearchDto>()).Returns(new AddressVerificationResultDto());

        var result = await _addressService.Verify(dto);

        Assert.That(result, Is.Not.Null);

        await _postService.Received(1).Verify(Arg.Is(dto));
    }

    [Test]
    public async Task Verify_WithServiceThrowing_ShouldReturnInvalidResultDto()
    {
        var dto = new AddressSearchDto();
        _postService.Verify(Arg.Any<AddressSearchDto>()).ThrowsAsync(new TimeoutException());

        var result = await _addressService.Verify(dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Status, Is.EqualTo(AddressVerificationStatus.Invalid));

        await _postService.Received(1).Verify(Arg.Is(dto));
    }
}
