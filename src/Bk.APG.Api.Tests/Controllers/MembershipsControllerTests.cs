using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class MembershipsControllerTests
{
    private readonly IMembershipService _membershipService = Substitute.For<IMembershipService>();

    private MembershipsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new MembershipsController(_membershipService);
    }

    [TearDown]
    public void TearDown()
    {
        _membershipService.ClearSubstitute();
    }

    [Test]
    public async Task CreateMembership_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var createDto = new Faker<MembershipCreateDto>().Generate();
        var listDto = new Faker<MembershipDetailDto>().Generate();
        _membershipService.CreateMembership(createDto).Returns(listDto);

        var result = await _controller.CreateMember(createDto) as OkObjectResult;

        await _membershipService.Received(1).CreateMembership(createDto);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(listDto));
        });
    }

    [Test]
    public async Task GetByIdForUpdate_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var response = await _controller.GetByIdForUpdate(Guid.NewGuid());

        await _membershipService.ReceivedWithAnyArgs().GetMembershipForUpdate(Arg.Any<Guid>());

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
        });
    }

    [Test]
    public async Task Update_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var guid = Guid.NewGuid();
        var dto = new MembershipUpdateDto()
        {
            Id = guid,
            CommitteeId = new Guid(),
            BeginDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 2, 1),
            FunctionId = Guid.Empty,
            ElectionTypeId = Guid.Empty,
            ElectionOfficeId = Guid.Empty,
            InCorrelationWithFederalDuty = true,
            JustificationLongerDuty = "JustificationLongerDuty",
            JustificationShorterDuty = "JustificationLongerDuty",
            JustificationMemberInFederalDuty = "JustificationMemberInFederalDuty",
            JustificationMemberInFederalAssembly = "JustificationMemberInFederalAssembly",
            RequirementsProfile = "RequirementsProfile",
            OldMembershipAddition = "OldMembershipAddition",
            MaximumEmploymentLevel = 50,
            PersonId = Guid.NewGuid(),
            RowVersion = 666
        };
        var response = await _controller.Update(guid, dto);

        await _membershipService.ReceivedWithAnyArgs().UpdateMembership(Arg.Is(guid), Arg.Is(dto));

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
        });
    }

    [Test]
    public async Task DeleteMembership_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var id = Guid.NewGuid();

        var result = await _controller.DeleteMembership(id) as OkResult;

        await _membershipService.Received(1).DeleteMembership(id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
        });
    }
}
