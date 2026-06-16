using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class AddressesControllerTests
{
    private readonly IAddressService _addressService = Substitute.For<IAddressService>();

    private AddressesController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new AddressesController(_addressService);
    }

    [TearDown]
    public void TearDown()
    {
        _addressService.ClearSubstitute();
    }

    [Test]
    public async Task Search_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var dto = new AddressSearchDto();
        var result = new AddressDto { Id = Guid.NewGuid() };
        _addressService.Search(Arg.Any<AddressSearchDto>()).Returns([result]);

        var response = await _controller.Search(dto);
        Assert.That(response, Is.Not.Null);

        var responseObject = response as OkObjectResult;
        Assert.That(responseObject, Is.Not.Null);

        var responseObjectValue = (responseObject.Value as IEnumerable<AddressDto>)?.ToArray();
        Assert.That(responseObjectValue, Is.Not.Null);
        Assert.That(responseObjectValue, Is.Not.Empty);
        Assert.That(responseObjectValue.First().Id, Is.EqualTo(result.Id));

        await _addressService.Received(1).Search(Arg.Is(dto));
    }

    [Test]
    public async Task Verify_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var dto = new AddressSearchDto();
        var result = new AddressVerificationResultDto();
        _addressService.Verify(Arg.Any<AddressSearchDto>()).Returns(result);

        var response = await _controller.Verify(dto);

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.That(responseObject.Value, Is.InstanceOf<AddressVerificationResultDto>());

        await _addressService.Received(1).Verify(Arg.Is(dto));
    }
}
