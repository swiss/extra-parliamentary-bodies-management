using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class ContactPointsControllerTests
{
    private readonly IContactPointService _contactPointService = Substitute.For<IContactPointService>();

    private ContactPointsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new ContactPointsController(_contactPointService);
    }

    [TearDown]
    public void TearDown()
    {
        _contactPointService.ClearSubstitute();
    }

    [Test]
    public async Task GetAllByCommitteeId_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var contactPointListDto = new ContactPointListDto
        {
            Id = Guid.NewGuid(),
            ContactPointType = string.Empty
        };

        var contactPoints = new List<ContactPointListDto>
        {
            contactPointListDto
        };

        _contactPointService
            .GetContactPointListByCommitteeId(Arg.Any<Guid>())
            .Returns(contactPoints);

        var response = await _controller.GetAllByCommitteeId(Guid.NewGuid());

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(contactPoints));
        });
    }

    [Test]
    public async Task GetEmpty_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var response = await _controller.GetEmpty(Guid.NewGuid());

        await _contactPointService.ReceivedWithAnyArgs().GetEmpty(Guid.NewGuid());

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
        });
    }

    [Test]
    public async Task Create_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var dto = new ContactPointCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            ContactPointTypeId = Guid.NewGuid(),
            ContactPointTypeUri = "my.uri.admin.ch",
            CompanyName = "Test Amt",
            Section = "Sektion Nr. 1",
            BeginDate = new DateOnly(2020, 1, 1),
            EndDate = new DateOnly(2027, 12, 31),
            Street = "Teststrasse 2",
            PoBox = "Postfach",
            Zip = "3000",
            City = "Bern 12",
            Phone = "+41 77 444 33 22",
            Email = "test@testamt.ch",
            Surname = "Muster",
            GivenName = "Rita",
            Title = "Datenschutzberaterin",
            PersonalPhone = "+41 78 777 66 55",
            PersonalMobile = "+41 78 999 88 77",
            PersonalEmail = "r.muster@test.ch",
            ReleasePersonData = false,
            OldId = 123123,
            GenderId = Guid.NewGuid(),
            LanguageId = Guid.NewGuid(),
            IsCopy = false,
        };
        var response = await _controller.Create(dto);

        await _contactPointService.ReceivedWithAnyArgs().Create(Arg.Is(dto));

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
        });
    }

    [Test]
    public async Task GetByIdForUpdate_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var contactPointUpdateDto = new ContactPointUpdateDto
        {
            Id = Guid.NewGuid(),
            CommitteeId = Guid.NewGuid(),
            ContactPointTypeUri = "www.uri.com",
            ContactPointTypeId = Guid.NewGuid(),
            Zip = "5600",
            City = "Lenzburg",
            RowVersion = 666
        };

        _contactPointService
            .GetByIdForUpdate(Arg.Any<Guid>())
            .Returns(contactPointUpdateDto);

        var response = await _controller.GetByIdForUpdate(Guid.NewGuid());

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(contactPointUpdateDto));
        });
    }

    [Test]
    public async Task Update_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var guid = Guid.NewGuid();
        var dto = new ContactPointUpdateDto()
        {
            Id = guid,
            CommitteeId = Guid.NewGuid(),
            ContactPointTypeId = Guid.NewGuid(),
            ContactPointTypeUri = "my.uri.admin.ch",
            CompanyName = "Test Amt",
            Section = "Sektion Nr. 1",
            BeginDate = new DateOnly(2020, 1, 1),
            EndDate = new DateOnly(2027, 12, 31),
            Street = "Teststrasse 2",
            PoBox = "Postfach",
            Zip = "3000",
            City = "Bern 12",
            Phone = "+41 77 444 33 22",
            Email = "test@testamt.ch",
            Surname = "Muster",
            GivenName = "Rita",
            Title = "Datenschutzberaterin",
            PersonalPhone = "+41 78 777 66 55",
            PersonalMobile = "+41 78 999 88 77",
            PersonalEmail = "r.muster@test.ch",
            ReleasePersonData = false,
            OldId = 123123,
            GenderId = Guid.NewGuid(),
            LanguageId = Guid.NewGuid(),
            RowVersion = 666
        };
        var response = await _controller.Update(guid, dto);

        await _contactPointService.ReceivedWithAnyArgs().Update(Arg.Is(guid), Arg.Is(dto));

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.That(responseObject.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task Delete_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var id = Guid.NewGuid();

        var response = await _controller.Delete(id);

        await _contactPointService.Received(1).Delete(Arg.Is(id));

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.That(responseObject.StatusCode, Is.EqualTo(200));
    }
}
