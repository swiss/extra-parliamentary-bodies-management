using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class PersonsControllerTests
{
    private readonly IPersonService _personService = Substitute.For<IPersonService>();
    private readonly IMembershipService _membershipService = Substitute.For<IMembershipService>();
    private readonly ISalutationGeneratorService _salutationGeneratorService = Substitute.For<ISalutationGeneratorService>();
    private readonly IInterestService _interestService = Substitute.For<IInterestService>();

    private PersonsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new PersonsController(_personService, _membershipService, _salutationGeneratorService, _interestService);
    }

    [TearDown]
    public void TearDown()
    {
        _personService.ClearSubstitute();
        _membershipService.ClearSubstitute();
        _salutationGeneratorService.ClearSubstitute();
        _interestService.ClearSubstitute();
    }

    [Test]
    public async Task GetAll_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var persons = new PagedResultDto<PersonListDto>
        {
            Index = 0,
            Total = 100,
            Items = new Faker<PersonListDto>().Generate(10)
        };
        _personService
            .GetPersonList(Arg.Any<PagingParametersDto>(), Arg.Any<PersonFilterParametersDto>(), Arg.Any<string?>(), Arg.Any<SortDirection?>())
            .Returns(persons);

        var response = await _controller.GetAll(new PagingParametersDto(), new PersonFilterParametersDto(), new SortParametersDto());

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(persons));
        }
    }

    [Test]
    public async Task GetById_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var personDetail = new PersonDetailDto
        {
            Id = Guid.NewGuid(),
            GivenName = "Peter",
            Surname = "Tester",
            BirthYear = 1965,
            Language = "de",
            LanguageId = Guid.NewGuid(),
            CorrespondenceLanguage = "de",
            Gender = "männlich",
            GenderId = Guid.NewGuid(),
            MaskAddress = false
        };
        _personService.GetPersonDetail(Arg.Any<Guid>()).Returns(personDetail);

        var response = await _controller.GetById(Guid.NewGuid());

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(personDetail));
        }
    }

    [Test]
    public async Task GetByIdForUpdate_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var response = await _controller.GetByIdForUpdate(Guid.NewGuid());

        await _personService.ReceivedWithAnyArgs().GetPersonForUpdate(Arg.Any<Guid>());

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.That(responseObject.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task Update_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var guid = Guid.NewGuid();
        var dto = new PersonUpdateDto
        {
            BirthYear = 2000,
            CorrespondenceLanguageId = Guid.NewGuid(),
            GenderId = Guid.NewGuid(),
            GivenName = "givenName",
            Surname = "surname",
            Id = Guid.NewGuid(),
            LanguageId = Guid.NewGuid(),
            MaskAddress = false,
            RowVersion = 666
        };
        var response = await _controller.Update(guid, dto);

        await _personService.ReceivedWithAnyArgs().UpdatePerson(Arg.Is(guid), Arg.Is(dto));

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.That(responseObject.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task Delete_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var id = Guid.NewGuid();

        var response = await _controller.Delete(id);

        await _personService.Received().DeletePerson(id);

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.That(responseObject.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task Create_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var dto = new PersonCreateDto()
        {
            BirthYear = 2000,
            CorrespondenceLanguageId = Guid.NewGuid(),
            GenderId = Guid.NewGuid(),
            GivenName = "givenName",
            Surname = "surname",
            LanguageId = Guid.NewGuid(),
        };
        var response = await _controller.Create(dto);

        await _personService.ReceivedWithAnyArgs().CreatePerson(Arg.Is(dto));

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.That(responseObject.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public void GetEmpty_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var response = _controller.GetEmpty();

        _personService.ReceivedWithAnyArgs().GetEmpty();

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.That(responseObject.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetSimilarPersons_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var response = await _controller.GetSimilarPersons("surname", "givenName", 1985, 1);

        await _personService.Received().GetSimilarPersons("surname", "givenName", 1985, 1);

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.That(responseObject.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetMemberships_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var personId = Guid.NewGuid();
        var response = await _controller.GetMemberships(personId);

        await _membershipService.Received().GetAllByPersonId(personId);

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.That(responseObject.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetByName_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var response = await _controller.GetByName("clark");

        await _personService.Received().GetByName("clark");

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.That(responseObject.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GenerateSalutation_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var genderId = Guid.NewGuid();
        var correspondenceLanguageId = Guid.NewGuid();
        const string surname = "Kent";
        const string title = "Dr.";
        _salutationGeneratorService.CreateSalutationTextForPerson(genderId, correspondenceLanguageId, surname, title).Returns("Foo Bar");

        var response = await _controller.GenerateSalutation(genderId, correspondenceLanguageId, surname, title);

        await _salutationGeneratorService.Received(1).CreateSalutationTextForPerson(genderId, correspondenceLanguageId, surname, title);

        Assert.That(response, Is.Not.Null);
        var okResult = response as OkObjectResult;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo("Foo Bar"));
    }

    [Test]
    public async Task GetInterests_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var personId = Guid.NewGuid();

        var interests = new Faker<InterestUpdateDto>().Generate(10);

        _interestService
            .GetInterestsForUpdateByPersonId(personId)
            .Returns(interests);

        var response = await _controller.GetInterests(personId);

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
    public async Task UpdateInterests_WhenCalled_ShouldCallServiceAndReturnResult()
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
