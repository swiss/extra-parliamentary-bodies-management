using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class CommitteeTypeControllerTests
{
    private readonly ICommitteeTypeService _committeeTypeService = Substitute.For<ICommitteeTypeService>();

    private CommitteeTypeController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new CommitteeTypeController(_committeeTypeService);
    }

    [TearDown]
    public void TearDown()
    {
        _committeeTypeService.ClearSubstitute();
    }

    [Test]
    public async Task GetAll_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var committeeTypes = new Faker<CommitteeTypeListDto>().Generate(10);

        _committeeTypeService
            .GetCommitteeTypeList()
            .Returns(committeeTypes);

        var response = await _controller.GetAll();

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(committeeTypes));
        });
    }

    [Test]
    public async Task GetByIdForUpdate_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var committeeTypeUpdateDto = new Faker<CommitteeTypeUpdateDto>().Generate();

        _committeeTypeService.GetCommitteeTypeForUpdate(Arg.Any<Guid>()).Returns(committeeTypeUpdateDto);

        var result = await _controller.GetByIdForUpdate(Guid.NewGuid()) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(committeeTypeUpdateDto));
        });
    }

    [Test]
    public async Task Update_WhenCalledWithValidData_ShouldCallServiceAndReturnResult()
    {
        var updateDto = new Faker<CommitteeTypeUpdateDto>().Generate();
        _committeeTypeService.UpdateCommitteeType(updateDto.Id, updateDto).Returns(updateDto);

        var result = await _controller.Update(updateDto.Id, updateDto) as OkObjectResult;

        await _committeeTypeService.Received(1).UpdateCommitteeType(updateDto.Id, updateDto);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(updateDto));
        });
    }
}
