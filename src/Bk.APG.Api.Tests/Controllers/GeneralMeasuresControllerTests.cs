using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class GeneralMeasuresControllerTests
{
    private readonly IGeneralMeasureService _generalMeasureService = Substitute.For<IGeneralMeasureService>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

    private GeneralMeasuresController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new GeneralMeasuresController(_generalMeasureService, _authorizationService);
    }

    [TearDown]
    public void TearDown()
    {
        _generalMeasureService.ClearSubstitute();
        _authorizationService.ClearSubstitute();
    }

    [Test]
    public async Task GetGeneralMeasures_WhenCalled_ShouldReturnOkWithData()
    {
        var measures = new List<GeneralMeasureDto>();
        _generalMeasureService.GetGeneralMeasures().Returns(measures);

        var result = await _controller.GetGeneralMeasures();

        await _generalMeasureService.Received(1).GetGeneralMeasures();
        Assert.That(result, Is.Not.Null);
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(measures));
    }

    [Test]
    public async Task UpdateGeneralMeasure_WhenAdminUser_ShouldCallServiceAndReturnNoContent()
    {
        var departmentId = Guid.NewGuid();
        var updateDto = new GeneralMeasureUpdateDto
        {
            DepartmentId = departmentId,
            JustificationGenders = "Updated Gender",
            JustificationLanguages = "Updated Language"
        };

        _authorizationService.IsDepartment.Returns(false);

        var result = await _controller.UpdateGeneralMeasure(updateDto);

        await _generalMeasureService.Received(1).AddOrUpdateGeneralMeasure(updateDto);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        var noContentResult = result as NoContentResult;
        Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));
    }

    [Test]
    public async Task UpdateGeneralMeasure_WhenDepartmentUserUpdatesOwnDepartment_ShouldCallServiceAndReturnNoContent()
    {
        var departmentId = Guid.NewGuid();
        var department = new DepartmentBuilder().WithId(departmentId).Build();
        var updateDto = new GeneralMeasureUpdateDto
        {
            DepartmentId = departmentId,
            JustificationGenders = "Updated Gender",
            JustificationLanguages = "Updated Language"
        };

        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns(department);

        var result = await _controller.UpdateGeneralMeasure(updateDto);

        await _generalMeasureService.Received(1).AddOrUpdateGeneralMeasure(updateDto);
        await _authorizationService.Received(1).GetDepartment();
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        var noContentResult = result as NoContentResult;
        Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));
    }

    [Test]
    public async Task UpdateGeneralMeasure_WhenDepartmentUserUpdatesDifferentDepartment_ShouldReturnForbid()
    {
        var userDepartmentId = Guid.NewGuid();
        var otherDepartmentId = Guid.NewGuid();
        var department = new DepartmentBuilder().WithId(userDepartmentId).Build();
        var updateDto = new GeneralMeasureUpdateDto
        {
            DepartmentId = otherDepartmentId,
            JustificationGenders = "Updated Gender",
            JustificationLanguages = "Updated Language"
        };

        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns(department);

        var result = await _controller.UpdateGeneralMeasure(updateDto);

        await _generalMeasureService.DidNotReceive().AddOrUpdateGeneralMeasure(Arg.Any<GeneralMeasureUpdateDto>());
        await _authorizationService.Received(1).GetDepartment();
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }

    [Test]
    public async Task UpdateGeneralMeasure_WhenDepartmentUserAndDepartmentIsNull_ShouldReturnForbid()
    {
        var departmentId = Guid.NewGuid();
        var updateDto = new GeneralMeasureUpdateDto
        {
            DepartmentId = departmentId,
            JustificationGenders = "Updated Gender",
            JustificationLanguages = "Updated Language"
        };

        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns((Department?)null);

        var result = await _controller.UpdateGeneralMeasure(updateDto);

        await _generalMeasureService.DidNotReceive().AddOrUpdateGeneralMeasure(Arg.Any<GeneralMeasureUpdateDto>());
        await _authorizationService.Received(1).GetDepartment();
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }

    [Test]
    public async Task Forward_WhenDepartmentUserForOwnDepartment_ShouldCallServiceAndReturnNoContent()
    {
        var departmentId = Guid.NewGuid();
        var department = new DepartmentBuilder().WithId(departmentId).Build();
        var forwardDto = new GeneralMeasureForwardDto { Message = "Bitte prüfen.", ForwardToAdmin = true };

        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns(department);

        var result = await _controller.Forward(departmentId, forwardDto);

        await _generalMeasureService.Received(1).Forward(departmentId, forwardDto.Message, forwardDto.ForwardToAdmin);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Forward_WhenForwardToAdminAndNotDepartment_ShouldReturnForbid()
    {
        var departmentId = Guid.NewGuid();
        var forwardDto = new GeneralMeasureForwardDto { Message = "Bitte prüfen.", ForwardToAdmin = true };
        _authorizationService.IsDepartment.Returns(false);

        var result = await _controller.Forward(departmentId, forwardDto);

        await _generalMeasureService.DidNotReceive().Forward(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<bool>());
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }

    [Test]
    public async Task Forward_WhenForwardToAdminAndDepartmentUserForDifferentDepartment_ShouldReturnForbid()
    {
        var departmentId = Guid.NewGuid();
        var otherDepartmentId = Guid.NewGuid();
        var department = new DepartmentBuilder().WithId(otherDepartmentId).Build();
        var forwardDto = new GeneralMeasureForwardDto { Message = "Bitte prüfen.", ForwardToAdmin = true };

        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns(department);

        var result = await _controller.Forward(departmentId, forwardDto);

        await _generalMeasureService.DidNotReceive().Forward(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<bool>());
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }

    [Test]
    public async Task ValidateGeneralMeasure_WhenAdmin_ShouldCallServiceAndReturnNoContent()
    {
        var departmentId = Guid.NewGuid();
        _authorizationService.IsAdmin.Returns(true);

        var result = await _controller.ValidateGeneralMeasure(departmentId);

        await _generalMeasureService.Received(1).Validate(departmentId);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task ValidateGeneralMeasure_WhenNotAdmin_ShouldReturnForbid()
    {
        var departmentId = Guid.NewGuid();
        _authorizationService.IsAdmin.Returns(false);

        var result = await _controller.ValidateGeneralMeasure(departmentId);

        await _generalMeasureService.DidNotReceive().Validate(Arg.Any<Guid>());
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }

    [Test]
    public async Task Forward_WhenForwardToDepartmentAndAdmin_ShouldCallServiceAndReturnNoContent()
    {
        var departmentId = Guid.NewGuid();
        var forwardDto = new GeneralMeasureForwardDto { Message = "Bitte ergänzen.", ForwardToAdmin = false };
        _authorizationService.IsAdmin.Returns(true);

        var result = await _controller.Forward(departmentId, forwardDto);

        await _generalMeasureService.Received(1).Forward(departmentId, forwardDto.Message, forwardDto.ForwardToAdmin);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Forward_WhenForwardToDepartmentAndNotAdmin_ShouldReturnForbid()
    {
        var departmentId = Guid.NewGuid();
        var forwardDto = new GeneralMeasureForwardDto { Message = "Bitte ergänzen.", ForwardToAdmin = false };
        _authorizationService.IsAdmin.Returns(false);

        var result = await _controller.Forward(departmentId, forwardDto);

        await _generalMeasureService.DidNotReceive().Forward(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<bool>());
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }
}
