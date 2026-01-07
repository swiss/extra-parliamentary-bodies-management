using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class AddressControllerTests
{
    private readonly IAddressService _addressService = Substitute.For<IAddressService>();

    private AddressController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new AddressController(_addressService);
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
        var value = (responseObject.Value as IEnumerable<AddressDto>)!.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(value, Is.Not.Null);
            Assert.That(value.First().Id, Is.EqualTo(result.Id));
        });

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
