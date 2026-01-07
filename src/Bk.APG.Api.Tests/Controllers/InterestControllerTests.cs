using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class InterestControllerTests
{
    private readonly IInterestService _interestService = Substitute.For<IInterestService>();

    private InterestController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new InterestController(_interestService);
    }

    [TearDown]
    public void TearDown()
    {
        _interestService.ClearSubstitute();
    }

    [Test]
    public async Task GetAllByPersonId_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var personId = Guid.NewGuid();

        var interests = new Faker<InterestUpdateDto>().Generate(10);

        _interestService
            .GetInterestsForUpdateByPersonId(personId)
            .Returns(interests);

        var response = await _controller.GetAllByPersonId(personId);

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(interests));
        });
    }

    [Test]
    public async Task Update_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var guid = Guid.NewGuid();
        var updateDtos = new List<InterestUpdateDto>();
        var updateDto = new InterestUpdateDto
        {
            Id = guid,
            PersonId = Guid.NewGuid(),
            Text = "my old text",
            InterestText = "my new text",
            InterestCommitteeId = Guid.NewGuid(),
            InterestFunctionId = Guid.NewGuid(),
            InterestLegalFormId = Guid.NewGuid(),
            LegalFormId = Guid.NewGuid(),
            RowVersion = 666
        };

        updateDtos.Add(updateDto);

        var response = await _controller.UpdateInterests(guid, updateDtos.ToArray());

        await _interestService.ReceivedWithAnyArgs().UpdateInterests(Arg.Is(guid), Arg.Is(updateDtos.ToArray()));

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.That(responseObject.StatusCode, Is.EqualTo(200));
    }
}
