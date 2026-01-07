using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class PersonServiceTests
{
    private readonly IPersonRepository _personRepository = Substitute.For<IPersonRepository>();
    private readonly IMasterDataRepository _masterDataRepository = Substitute.For<IMasterDataRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly ISalutationGeneratorService _salutationGeneratorService = Substitute.For<ISalutationGeneratorService>();
    private readonly ILogger<PersonService> _logger = NullLogger<PersonService>.Instance;
    private readonly IGeneralElectionService _generalElectionService = Substitute.For<IGeneralElectionService>();
    private readonly ITermOfOfficeDateService _termOfOfficeDateService = Substitute.For<ITermOfOfficeDateService>();
    private readonly IMembershipRepository _membershipRepository = Substitute.For<IMembershipRepository>();
    private readonly IMembershipCandidateRepository _membershipCandidateRepository = Substitute.For<IMembershipCandidateRepository>();
    private readonly IInterestRepository _interestRepository = Substitute.For<IInterestRepository>();
    private readonly IMembershipCandidateLogMessageRepository _membershipCandidateLogMessageRepository = Substitute.For<IMembershipCandidateLogMessageRepository>();
    private readonly IWorklistTaskRepository _worklistTaskRepository = Substitute.For<IWorklistTaskRepository>();

    private PersonService _service = null!;
    private Person _personToCreate;
    private Person _personToUpdate;
    private Person _personToUpdateWithAddresses;
    private List<Membership> _membershipList;
    private Membership _membership;
    private Guid _genderId;
    private Guid _languageId;

    [SetUp]
    public void SetUp()
    {
        _service = new PersonService(
            _authorizationService,
            _generalElectionService,
            _termOfOfficeDateService,
            _salutationGeneratorService,
            _masterDataRepository,
            _personRepository,
            _membershipRepository,
            _membershipCandidateRepository,
            _interestRepository,
            _membershipCandidateLogMessageRepository,
            _worklistTaskRepository,
            _logger);

        _genderId = Guid.Parse("e4f12a39-8b2d-4c7b-b49c-5e9f4d7d1a67");
        _languageId = Guid.Parse(Language.GermanId);

        var language = new LanguageBuilder()
            .WithId(_languageId)
            .Build();

        var gender = new GenderBuilder()
            .WithId(_genderId)
            .Build();

        var personId = Guid.NewGuid();
        _membershipList = new List<Membership>();

        _membership = new MembershipBuilder().WithIsActive(true).Build();
        _membershipList.Add(_membership);

        _personToCreate = new PersonBuilder()
            .WithId(personId)
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithCorrespondenceLanguage(language)
            .WithLanguage(language)
            .WithGender(gender)
            .WithLegislaturePeriods([new LegislaturePeriodBuilder().Build(), new LegislaturePeriodBuilder().Build()])
            .Build();

        _personToUpdate = new PersonBuilder()
            .WithId(personId)
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithCorrespondenceLanguage(language)
            .WithLanguage(language)
            .WithGender(gender)
            .WithLegislaturePeriods([new LegislaturePeriodBuilder().Build(), new LegislaturePeriodBuilder().Build()])
            .Build();

        _personToUpdateWithAddresses = new PersonBuilder()
            .WithId(Guid.NewGuid())
            .WithOfficeAddress(new AddressBuilder().Build())
            .WithPrivateAddress(new AddressBuilder().Build())
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithCorrespondenceLanguage(language)
            .WithLanguage(language)
            .WithGender(gender)
            .WithMemberships(_membershipList)
            .Build();

        _personRepository.GetByIdForUpdate(_personToUpdate.Id).Returns(_personToUpdate);
        _personRepository.GetById(_personToUpdate.Id).Returns(_personToUpdate);

        _personRepository.GetByIdForUpdate(_personToUpdateWithAddresses.Id).Returns(_personToUpdateWithAddresses);
        _personRepository.GetById(_personToUpdateWithAddresses.Id).Returns(_personToUpdateWithAddresses);

        _authorizationService.GetCurrentUserName().Returns("currentUser");
        _membershipCandidateRepository.GetByPersonIdForUpdate(Arg.Any<Guid>()).Returns(Array.Empty<MembershipCandidate>());
    }

    [TearDown]
    public void TearDown()
    {
        _personRepository.ClearSubstitute();
        _authorizationService.ClearSubstitute();
        _masterDataRepository.ClearSubstitute();
        _membershipRepository.ClearSubstitute();
        _membershipCandidateRepository.ClearSubstitute();
        _interestRepository.ClearSubstitute();
        _membershipCandidateLogMessageRepository.ClearSubstitute();
        _worklistTaskRepository.ClearSubstitute();
    }

    [Test]
    public async Task GetPersonList_ShouldCallService()
    {
        var pagingDto = new PagingParametersDto { PageIndex = 42, PageSize = 2 };
        var filtersDto = new PersonFilterParametersDto()
        {
            FreeText = "test"
        };
        const string sortKey = "givenname";
        const SortDirection sortDirection = SortDirection.Asc;
        var resultFromRepository = new PagedResult<Person>
        {
            Total = 276,
            Index = 42,
            Items =
            [
                new PersonBuilder()
                    .WithCorrespondenceAddress(new AddressBuilder().Build())
                    .WithLanguage(new LanguageBuilder().Build())
                    .Build(),
                new PersonBuilder()
                    .WithCorrespondenceAddress(new AddressBuilder().Build())
                    .WithLanguage(new LanguageBuilder().Build())
                    .Build()
            ]
        };
        _personRepository
            .GetAll(Arg.Any<PagingParameters>(), Arg.Any<PersonFilterParameters?>(), Arg.Any<string?>(), Arg.Any<SortDirection?>())
            .Returns(resultFromRepository);

        var persons = await _service.GetPersonList(pagingDto, filtersDto, sortKey, sortDirection);

        await _personRepository.Received(1).GetAll(
            Arg.Is<PagingParameters>(p => p.PageIndex == pagingDto.PageIndex && p.PageSize == pagingDto.PageSize),
            Arg.Any<PersonFilterParameters?>(),
            Arg.Is(sortKey),
            Arg.Is(sortDirection));

        Assert.That(persons, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(persons.Index, Is.EqualTo(resultFromRepository.Index));
            Assert.That(persons.Total, Is.EqualTo(resultFromRepository.Total));
            Assert.That(persons.Items.Count(), Is.EqualTo(resultFromRepository.Items.Count()));
        });
    }

    [Test]
    public async Task GetPersonList_WithEmptyFilter_ShouldCallServiceAndReturnEmptyList()
    {
        var pagingDto = new PagingParametersDto { PageIndex = 42, PageSize = 2 };
        var filtersDto = new PersonFilterParametersDto();
        const string sortKey = "givenname";
        const SortDirection sortDirection = SortDirection.Asc;
        var resultFromRepository = new PagedResult<Person>
        {
            Total = 276,
            Index = 42,
            Items =
            [
                new PersonBuilder()
                    .WithCorrespondenceAddress(new AddressBuilder().Build())
                    .WithLanguage(new LanguageBuilder().Build())
                    .Build(),
                new PersonBuilder()
                    .WithCorrespondenceAddress(new AddressBuilder().Build())
                    .WithLanguage(new LanguageBuilder().Build())
                    .Build()
            ]
        };
        _personRepository
            .GetAll(Arg.Any<PagingParameters>(), Arg.Any<PersonFilterParameters?>(), Arg.Any<string?>(), Arg.Any<SortDirection?>())
            .Returns(resultFromRepository);

        var persons = await _service.GetPersonList(pagingDto, filtersDto, sortKey, sortDirection);

        await _personRepository.DidNotReceiveWithAnyArgs().GetAll(
            Arg.Any<PagingParameters>(),
            Arg.Any<PersonFilterParameters?>(),
            Arg.Any<string?>(),
            Arg.Any<SortDirection>());

        Assert.That(persons, Is.Not.Null);
        Assert.That(persons.Items, Is.Empty);
        Assert.That(persons.Total, Is.EqualTo(0));
    }

    [Test]
    public async Task GetPersonDetail_ShouldReturnData()
    {
        var personId = Guid.NewGuid();

        var person = new PersonBuilder()
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        _personRepository
            .GetById(Arg.Any<Guid>())
            .Returns(person);

        var personDetail = await _service.GetPersonDetail(personId);

        Assert.That(personDetail, Is.Not.Null);
        Assert.That(personDetail.Id, Is.EqualTo(person.Id));
    }

    [Test]
    public async Task GetPersonForUpdate_ShouldReturnData()
    {
        var personId = Guid.NewGuid();

        var person = new PersonBuilder()
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        _personRepository
            .GetByIdForUpdate(Arg.Any<Guid>())
            .Returns(person);

        var personDetail = await _service.GetPersonForUpdate(personId);

        Assert.That(personDetail, Is.Not.Null);
        Assert.That(personDetail.Id, Is.EqualTo(person.Id));
    }

    [Test]
    public async Task UpdatePerson_WithValidData_ShouldUpdatePersonProperties()
    {
        var updateDto = new PersonUpdateDto
        {
            Id = _personToUpdate.Id,
            Surname = "surname",
            BirthYear = 2000,
            CorrespondenceLanguageId = _personToUpdate.CorrespondenceLanguageId,
            GenderId = _personToUpdate.GenderId,
            GivenName = "givenName",
            LanguageId = _personToUpdate.LanguageId,
            SalutationId = _personToUpdate.SalutationId,
            MaskAddress = false,
            PrivateAddress = new AddressUpdateDto(),
            FederalAssembly = true,
            LegislaturePeriodIds = _personToUpdate.LegislaturePeriods.Select(x => x.Id).ToList(),
            RowVersion = 666
        };

        _personRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_personToUpdate);
        _masterDataRepository.GetLegislaturePeriodsByIds(Arg.Any<List<Guid>>()).Returns(_personToUpdate.LegislaturePeriods);

        await _service.UpdatePerson(_personToUpdate.Id, updateDto);

        _authorizationService.Received(1).GetCurrentUserName();
        await _personRepository.Received(1).GetByIdForUpdate(_personToUpdate.Id, updateDto.RowVersion);
        await _personRepository.Received(1).CommitChanges();

        Assert.That(_personToUpdate, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(_personToUpdate.Surname, Is.EqualTo(updateDto.Surname));
            Assert.That(_personToUpdate.GivenName, Is.EqualTo(updateDto.GivenName));
            Assert.That(_personToUpdate.BirthYear, Is.EqualTo(updateDto.BirthYear));
            Assert.That(_personToUpdate.CorrespondenceLanguageId, Is.EqualTo(updateDto.CorrespondenceLanguageId));
            Assert.That(_personToUpdate.LanguageId, Is.EqualTo(updateDto.LanguageId));
            Assert.That(_personToUpdate.GenderId, Is.EqualTo(updateDto.GenderId));
            Assert.That(_personToUpdate.SalutationId, Is.EqualTo(updateDto.SalutationId));
            Assert.That(_personToUpdate.FederalAssembly, Is.EqualTo(updateDto.FederalAssembly));
            Assert.That(_personToUpdate.LegislaturePeriods, Has.Count.EqualTo(updateDto.LegislaturePeriodIds.Count));
            Assert.That(_personToUpdate.LegislaturePeriods.Select(x => x.Id).ToList(), Is.EquivalentTo(updateDto.LegislaturePeriodIds));
        });
    }

    [Test]
    public async Task UpdatePerson_WhenCalledWithValidDataAndNewAddresses_ShouldCallRepositoryAndCreateAddresses()
    {
        _authorizationService.IsAdmin.Returns(true);
        var updateDto = new PersonUpdateDto
        {
            Id = _personToUpdate.Id,
            Surname = "surname",
            BirthYear = 2000,
            CorrespondenceLanguageId = _personToUpdate.CorrespondenceLanguageId,
            GenderId = _personToUpdate.GenderId,
            GivenName = "givenName",
            LanguageId = _personToUpdate.LanguageId,
            SalutationId = _personToUpdate.SalutationId,
            MaskAddress = false,
            OfficeAddress = new AddressUpdateDto
            {
                Id = null,
                ActiveAddress = true,
                City = "Basel"
            },
            PrivateAddress = new AddressUpdateDto
            {
                Id = null,
                City = "Zürich"
            },
            RowVersion = 666
        };

        _personRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_personToUpdate);

        await _service.UpdatePerson(_personToUpdate.Id, updateDto);

        _authorizationService.Received(1).GetCurrentUserName();
        await _personRepository.Received(1).GetByIdForUpdate(_personToUpdate.Id, updateDto.RowVersion);
        await _personRepository.Received(1).CommitChanges();

        Assert.That(_personToUpdate, Is.Not.Null);
        Assert.That(_personToUpdate.PrivateAddress, Is.Not.Null);
        Assert.That(_personToUpdate.OfficeAddress, Is.Not.Null);
        Assert.That(_personToUpdate.CorrespondenceAddress, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(_personToUpdate.Modified, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(_personToUpdate.ModifiedBy, Is.EqualTo("currentUser"));
            Assert.That(_personToUpdate.OfficeAddress.City, Is.EqualTo("Basel"));
            Assert.That(_personToUpdate.OfficeAddress.Created, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(_personToUpdate.OfficeAddress.CreatedBy, Is.EqualTo("currentUser"));
            Assert.That(_personToUpdate.OfficeAddress.Modified, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(_personToUpdate.OfficeAddress.ModifiedBy, Is.EqualTo("currentUser"));
            Assert.That(_personToUpdate.PrivateAddress.City, Is.EqualTo("Zürich"));
            Assert.That(_personToUpdate.PrivateAddress.Created, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(_personToUpdate.PrivateAddress.CreatedBy, Is.EqualTo("currentUser"));
            Assert.That(_personToUpdate.PrivateAddress.Modified, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(_personToUpdate.PrivateAddress.ModifiedBy, Is.EqualTo("currentUser"));
            Assert.That(_personToUpdate.CorrespondenceAddress.City, Is.EqualTo("Basel"));
        });
    }

    [Test]
    public async Task UpdatePerson_WhenCalledWithValidDataAndExistingAddresses_ShouldCallRepositoryAndUpdateAddresses()
    {
        var updateDto = new PersonUpdateDto
        {
            Id = _personToUpdateWithAddresses.Id,
            Surname = "surname",
            BirthYear = 2000,
            CorrespondenceLanguageId = _personToUpdate.CorrespondenceLanguageId,
            GenderId = _personToUpdate.GenderId,
            GivenName = "givenName",
            LanguageId = _personToUpdate.LanguageId,
            SalutationId = _personToUpdate.SalutationId,
            MaskAddress = false,
            OfficeAddress = new AddressUpdateDto
            {
                Id = null,
                ActiveAddress = true,
                City = "Basel"
            },
            PrivateAddress = new AddressUpdateDto
            {
                Id = null,
                City = "Zürich"
            },
            RowVersion = 666
        };

        _personRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_personToUpdateWithAddresses);

        await _service.UpdatePerson(_personToUpdateWithAddresses.Id, updateDto);

        _authorizationService.Received(1).GetCurrentUserName();
        await _personRepository.Received(1).GetByIdForUpdate(_personToUpdateWithAddresses.Id, updateDto.RowVersion);
        await _personRepository.Received(1).CommitChanges();

        Assert.That(_personToUpdateWithAddresses, Is.Not.Null);
        Assert.That(_personToUpdateWithAddresses.PrivateAddress, Is.Not.Null);
        Assert.That(_personToUpdateWithAddresses.OfficeAddress, Is.Not.Null);
        Assert.That(_personToUpdateWithAddresses.CorrespondenceAddress, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(_personToUpdateWithAddresses.Modified, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(_personToUpdateWithAddresses.ModifiedBy, Is.EqualTo("currentUser"));
            Assert.That(_personToUpdateWithAddresses.OfficeAddress.City, Is.EqualTo("Basel"));
            Assert.That(_personToUpdateWithAddresses.OfficeAddress.Modified, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(_personToUpdateWithAddresses.OfficeAddress.ModifiedBy, Is.EqualTo("currentUser"));
            Assert.That(_personToUpdateWithAddresses.PrivateAddress.City, Is.EqualTo("Zürich"));
            Assert.That(_personToUpdateWithAddresses.PrivateAddress.Modified, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(_personToUpdateWithAddresses.PrivateAddress.ModifiedBy, Is.EqualTo("currentUser"));
            Assert.That(_personToUpdateWithAddresses.CorrespondenceAddress.City, Is.EqualTo("Basel"));
        });
    }

    [Test]
    public async Task UpdatePerson_WithMaskedAddress_ShouldNotUpdateAddresses()
    {
        var updateDto = new PersonUpdateDto
        {
            Id = _personToUpdateWithAddresses.Id,
            Surname = "surname",
            BirthYear = 2000,
            CorrespondenceLanguageId = _personToUpdate.CorrespondenceLanguageId,
            GenderId = _personToUpdate.GenderId,
            GivenName = "givenName",
            LanguageId = _personToUpdate.LanguageId,
            SalutationId = _personToUpdate.SalutationId,
            MaskAddress = false,
            OfficeAddress = new AddressUpdateDto
            {
                Id = null,
                ActiveAddress = true,
                City = "Basel"
            },
            PrivateAddress = new AddressUpdateDto
            {
                Id = null,
                City = "Zürich"
            },
            RowVersion = 666
        };
        _personToUpdateWithAddresses.Memberships.Clear();
        _personToUpdateWithAddresses.Memberships.Add(new MembershipBuilder().WithEndDate(DateOnly.FromDateTime(DateTime.Today.AddMonths(-1).AddDays(-1))).Build());
        _personRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_personToUpdateWithAddresses);

        await _service.UpdatePerson(updateDto.Id, updateDto);

        Assert.Multiple(() =>
        {
            Assert.That(_personToUpdateWithAddresses.OfficeAddress!.City, Is.Not.EqualTo(updateDto.OfficeAddress.City));
            Assert.That(_personToUpdateWithAddresses.PrivateAddress!.City, Is.Not.EqualTo(updateDto.PrivateAddress.City));
            Assert.That(_personToUpdateWithAddresses.CorrespondenceAddress!.City, Is.Not.EqualTo(updateDto.OfficeAddress.City));
        });
    }

    [Test]
    public async Task UpdatePerson_WhenCalledWithActiveMembershipAndRoleAdmin_ShouldCallUpdateLanguageAndGender()
    {
        var genderGuid = Guid.NewGuid();
        var languageGuid = Guid.NewGuid();

        var updateDto = new PersonUpdateDto
        {
            Id = _personToUpdateWithAddresses.Id,
            Surname = "surname",
            BirthYear = 2000,
            CorrespondenceLanguageId = _personToUpdate.CorrespondenceLanguageId,
            GenderId = genderGuid,
            GivenName = "givenName",
            LanguageId = languageGuid,
            SalutationId = _personToUpdate.SalutationId,
            MaskAddress = false,
            OfficeAddress = new AddressUpdateDto
            {
                Id = null,
                ActiveAddress = true,
                City = "Basel"
            },
            PrivateAddress = new AddressUpdateDto
            {
                Id = null,
                City = "Zürich"
            },
            RowVersion = 666
        };

        _personRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_personToUpdateWithAddresses);

        _personToUpdateWithAddresses.GenderId = genderGuid;
        _personToUpdateWithAddresses.LanguageId = languageGuid;

        var updatedDto = await _service.UpdatePerson(_personToUpdateWithAddresses.Id, updateDto);

        _authorizationService.Received(1).GetCurrentUserName();
        await _personRepository.Received(1).GetByIdForUpdate(_personToUpdateWithAddresses.Id, updateDto.RowVersion);
        await _personRepository.Received(1).CommitChanges();

        Assert.That(_personToUpdateWithAddresses, Is.Not.Null);
        Assert.That(_personToUpdateWithAddresses.PrivateAddress, Is.Not.Null);
        Assert.That(_personToUpdateWithAddresses.OfficeAddress, Is.Not.Null);
        Assert.That(_personToUpdateWithAddresses.CorrespondenceAddress, Is.Not.Null);
        Assert.That(_personToUpdateWithAddresses.CorrespondenceAddress, Is.Not.Null);
        Assert.That(updatedDto.GenderId, Is.EqualTo(genderGuid));
        Assert.That(updatedDto.LanguageId, Is.EqualTo(languageGuid));
    }

    [Test]
    public void UpdatePerson_WhenCalledWithActiveMembershipAndRoleDepartment_ShouldThrow()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var genderGuid = Guid.NewGuid();
        var languageGuid = Guid.NewGuid();

        var updateDto = new PersonUpdateDto
        {
            Id = _personToUpdateWithAddresses.Id,
            Surname = "surname",
            BirthYear = 2000,
            CorrespondenceLanguageId = _personToUpdate.CorrespondenceLanguageId,
            GenderId = genderGuid,
            GivenName = "givenName",
            LanguageId = _personToUpdate.LanguageId,
            SalutationId = languageGuid,
            MaskAddress = false,
            OfficeAddress = new AddressUpdateDto
            {
                Id = null,
                ActiveAddress = true,
                City = "Basel"
            },
            PrivateAddress = new AddressUpdateDto
            {
                Id = null,
                City = "Zürich"
            },
            RowVersion = 666
        };

        _personRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_personToUpdateWithAddresses);

        Assert.That(async () => await _service.UpdatePerson(_personToUpdateWithAddresses.Id, updateDto), Throws.Exception.InstanceOf<AuthorizationException>());
    }

    [Test]
    public async Task CreatePerson_WhenCalledWithValidData_ShouldCallRepository()
    {
        var createDto = new PersonCreateDto
        {
            Surname = "Hugentobler",
            BirthYear = 2000,
            CorrespondenceLanguageId = _personToCreate.CorrespondenceLanguageId,
            GenderId = _personToCreate.GenderId,
            GivenName = "Martin",
            LanguageId = _personToCreate.LanguageId,
            SalutationId = _personToCreate.SalutationId,
            Title = "Botschafter",
            LegislaturePeriodIds = _personToCreate.LegislaturePeriods.Select(x => x.Id).ToList(),
            OfficeAddress = new AddressUpdateDto
            {
                Id = null
            }
        };
        var officeAddress = new Address
        {
            Created = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            Modified = DateTime.UtcNow,
            ModifiedBy = "SYSTEM",
            CantonId = Guid.NewGuid(),
            City = "city_office",
            CompanyName = "company_name_office",
            Email = "email_office",
            Mobile = "mobile_office",
            Phone = "phone_office",
            PoBox = "pobox_office",
            Street = "street_office",
            Zip = "zip_office",
            CountryCode = "country_code_office"
        };

        var person = new Person
        {
            BirthYear = 2001,
            Created = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            GivenName = "Martin",
            Surname = "Hugentobler",
            Modified = DateTime.UtcNow,
            ModifiedBy = "SYSTEM",
            Gender = new GenderBuilder().Build(),
            GenderId = _personToCreate.GenderId,
            Language = new LanguageBuilder().Build(),
            LanguageId = Guid.NewGuid(),
            CorrespondenceLanguage = new LanguageBuilder().Build(),
            CorrespondenceLanguageId = Guid.Parse(Language.GermanId),
            Occupation = "occupation",
            FederalAssembly = true,
            FederalDuty = true,
            OfficeAddressId = Guid.NewGuid(),
            PrivateAddressId = Guid.NewGuid(),
            SalutationId = Guid.NewGuid(),
            Title = "Botschafter",
            OfficeAddress = officeAddress,
            CorrespondenceAddress = officeAddress,
            CorrespondenceAddressId = Guid.NewGuid(),
        };

        var salutation = new SalutationBuilder().WithGender(_personToCreate.GenderId).WithDescriptionDe("Sehr geehrter Herr").Build();
        var salutations = new List<Salutation> { salutation };

        _personRepository.Create(Arg.Any<Person>()).Returns(person);
        _personRepository.GetById(Arg.Any<Guid>()).Returns(person);
        _masterDataRepository.GetLegislaturePeriodsByIds(Arg.Any<List<Guid>>()).Returns(_personToCreate.LegislaturePeriods);
        _masterDataRepository.GetSalutations().Returns(salutations);

        var newPerson = await _service.CreatePerson(createDto);

        _authorizationService.Received(1).GetCurrentUserName();
        await _personRepository.Received(1).Create(
            Arg.Is<Person>(p => p.Created >= DateTime.UtcNow.AddSeconds(-1)
                                && p.CreatedBy == "currentUser"
                                && p.Modified >= DateTime.UtcNow.AddSeconds(-1)
                                && p.ModifiedBy == "currentUser"
                                && p.PrivateAddress == null
                                && p.CorrespondenceAddress != null
                                && p.OfficeAddress != null && p.OfficeAddress.Created >= DateTime.UtcNow.AddSeconds(-1) && p.OfficeAddress.CreatedBy == "currentUser"
                                && p.OfficeAddress.Modified >= DateTime.UtcNow.AddSeconds(-1) && p.OfficeAddress.ModifiedBy == "currentUser"
                                && p.LegislaturePeriods.Count == 2));
        await _personRepository.Received(1).GetById(Arg.Any<Guid>());

        await _salutationGeneratorService.Received(1).CreateSalutationTextForPerson(person.GenderId, person.CorrespondenceLanguageId, person.Surname, person.Title);
    }

    [Test]
    public async Task CreatePersonInGeneralElection_WhenCalledWithValidData_ShouldCreatePerson()
    {
        var membershipCandidate = new MembershipCandidateBuilder().WithId(new Guid()).WithSurname("Clark").WithGivenName("Jim").WithGender(new GenderBuilder().WithId(Gender.MaleGuid).Build())
             .WithLanguage(new LanguageBuilder().WithId(Guid.Parse(Language.GermanId)).Build()).WithBirthYear(1963).Build();

        var salutation = "Sehr geehrter Herr";

        var createdPerson = new PersonBuilder().WithSurname("Clark").WithGivenName("Jim").WithGender(new GenderBuilder().WithId(Gender.MaleGuid).Build())
             .WithLanguage(new LanguageBuilder().WithId(Guid.Parse(Language.GermanId)).Build()).WithCorrespondenceLanguage(new LanguageBuilder().WithId(Guid.Parse(Language.GermanId)).Build()).WithBirthYear(1963).Build();
        _personRepository.Create(Arg.Any<Person>()).Returns(createdPerson);
        _personRepository.GetById(Arg.Any<Guid>()).Returns(createdPerson);
        _masterDataRepository.GetLegislaturePeriodsByIds(Arg.Any<List<Guid>>()).Returns(_personToCreate.LegislaturePeriods);
        _salutationGeneratorService.CreateSalutationTextForPerson(createdPerson.GenderId, createdPerson.CorrespondenceLanguageId, createdPerson.Surname, createdPerson.Title).Returns(salutation);

        var newPerson = await _service.CreatePersonInGeneralElection(membershipCandidate);

        _authorizationService.Received(1).GetCurrentUserName();
        await _personRepository.Received(1).Create(
            Arg.Is<Person>(p => p.Created >= DateTime.UtcNow.AddSeconds(-1)
                                && p.CreatedBy == "currentUser"
                                && p.Modified >= DateTime.UtcNow.AddSeconds(-1)
                                && p.ModifiedBy == "currentUser"
                                && p.Surname == "Clark"
                                && p.GivenName == "Jim"
                                && p.BirthYear == 1963
                                && p.GenderId == Gender.MaleGuid
                                && p.LanguageId == Guid.Parse(Language.GermanId)
                                && p.CorrespondenceLanguageId == Guid.Parse(Language.GermanId)
                                && p.PrivateAddress == null
                                && p.CorrespondenceAddress == null
                                && p.OfficeAddress == null));
        await _personRepository.Received(1).GetById(Arg.Any<Guid>());
    }

    [Test]
    public async Task GetPersonsByNameAndYear_ShouldReturnData()
    {
        List<Person> persons =
        [
            new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder().Build())
                .WithCorrespondenceLanguage(new LanguageBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .WithGender(new GenderBuilder().Build())
                .WithSurname("schwarzenegger")
                .WithGivenName("arnold")
                .WithBirthYear(1947)
                .Build(),
            new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder().Build())
                .WithCorrespondenceLanguage(new LanguageBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .WithGender(new GenderBuilder().Build())
                .WithSurname("lopez")
                .WithGivenName("jennifer")
                .WithBirthYear(1969)
                .Build()
        ];

        _personRepository.GetPersonsByBirthYear(Arg.Any<int>(), Arg.Any<int>()).Returns(persons);

        var personDetails = (await _service.GetSimilarPersons("schwaarzenegger", "arnóld", 1947, 5))!.ToList();

        Assert.That(personDetails, Is.Not.Null);
        Assert.That(personDetails.First().Id, Is.EqualTo(persons.First().Id));
        await _personRepository.Received(1).GetPersonsByBirthYear(Arg.Is(1947), 5);
        await _personRepository.Received(1).GetPersonsByBirthYear(Arg.Is(1974), 0);
    }

    [Test]
    public async Task GetPersonsByNameAndYear_WithSameLastDigits_ShouldReturnDataWithoutSecondCall()
    {
        List<Person> persons = [];

        _personRepository.GetPersonsByBirthYear(Arg.Any<int>(), Arg.Any<int>()).Returns(persons);

        _ = (await _service.GetSimilarPersons("Clark", "Jim", 1966, 5))!.ToList();

        await _personRepository.Received(1).GetPersonsByBirthYear(Arg.Is(1966), 5);
    }

    [TestCase("schwaarzenegger", "arnóld", 1947, 5, 1)]
    [TestCase("schwaarzenegger", "arnólda", 1947, 5, 0)]
    [TestCase("swaarzenegger", "arnold", 1947, 5, 0)]
    [TestCase("lopez", "jennifer", 1969, 5, 2)]
    [TestCase("lópez", "jennifer", 1969, 5, 2)]
    [TestCase("lópez", "jennifer'", 1969, 5, 2)]
    [TestCase("lópez", "jennifer'", 1970, 1, 2)]
    [TestCase("lópez", "jennifer'", 1971, 1, 1)]
    [TestCase("lópez", "jennifer'", 1972, 1, 0)]
    [TestCase("lópez", "jennifers", 1969, 5, 0)]
    [TestCase("lopeza", "jennifer", 1969, 5, 0)]
    [TestCase("Clárk", "Jimm", 1937, 5, 1)]
    [TestCase("Clark", "Jimm'", 1937, 5, 1)]
    [TestCase("clark", "jim", 1937, 5, 1)]
    [TestCase("clark", "jim", 1946, 9, 0)]
    [TestCase("clark", "jimmy", 1936, 5, 0)]
    [TestCase("clarky", "jim", 1936, 5, 0)]
    [TestCase("clark", "jim", 1936, 0, 2)]
    [TestCase("jim", "clark", 1936, 0, 2)]
    [TestCase("jimm", "clark", 1936, 0, 2)]
    [TestCase("jim", "clark ", 1936, 0, 2)]
    public async Task GetPersonsByNameAndYear_ShouldFindCorrectSimilarity(string surname, string givenName, int birthYear, int birthYearRange, int expectedCount)
    {
        List<Person> persons =
        [
            new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder().Build())
                .WithCorrespondenceLanguage(new LanguageBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .WithGender(new GenderBuilder().Build())
                .WithSurname("Schwarzenegger")
                .WithGivenName("Arnold")
                .WithBirthYear(1947)
                .Build(),
            new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder().Build())
                .WithCorrespondenceLanguage(new LanguageBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .WithGender(new GenderBuilder().Build())
                .WithSurname("Lopez")
                .WithGivenName("Jennifer")
                .WithBirthYear(1969)
                .Build(),
            new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder().Build())
                .WithCorrespondenceLanguage(new LanguageBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .WithGender(new GenderBuilder().Build())
                .WithSurname("Loppez")
                .WithGivenName("Iennifer")
                .WithBirthYear(1970)
                .Build(),
            new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder().Build())
                .WithCorrespondenceLanguage(new LanguageBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .WithGender(new GenderBuilder().Build())
                .WithSurname("Clark")
                .WithGivenName("Jim")
                .WithBirthYear(1936)
                .Build(),
            new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder().Build())
                .WithCorrespondenceLanguage(new LanguageBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .WithGender(new GenderBuilder().Build())
                .WithSurname("Clark")
                .WithGivenName("Jim")
                .WithBirthYear(1963)
                .Build()
        ];

        var personsFromRepositoryRange = persons.Where(y => Math.Abs(y.BirthYear - birthYear) <= birthYearRange);
        var personsFromRepositoryDigitChangeAllowed = persons.Where(y => Math.Abs(y.BirthYear - birthYear) <= birthYearRange
                                                                         || (y.BirthYear.ToString().Substring(0, 2) == birthYear.ToString().Substring(0, 2) && y.BirthYear.ToString()[3] == birthYear.ToString()[2] && y.BirthYear.ToString()[2] == birthYear.ToString()[3]));

        _personRepository.GetPersonsByBirthYear(Arg.Any<int>(), Arg.Any<int>())
            .Returns(personsFromRepositoryRange, personsFromRepositoryDigitChangeAllowed);

        var personDetails = (await _service.GetSimilarPersons(surname, givenName, birthYear, birthYearRange))!.ToList();
        var birthYearWithLastDigitsExchanged = birthYear.ToString().Substring(0, 2) + birthYear.ToString()[3] + birthYear.ToString()[2];

        Assert.That(personDetails, Is.Not.Null);
        Assert.That(personDetails, Has.Count.EqualTo(expectedCount));
        await _personRepository.Received(1).GetPersonsByBirthYear(Arg.Is(birthYear), birthYearRange);
        await _personRepository.Received(1).GetPersonsByBirthYear(Arg.Is(Convert.ToInt32(birthYearWithLastDigitsExchanged)), 0);
    }

    [Test]
    public async Task GetByName_ShouldReturnPersonList()
    {
        List<Person> persons =
        [
            new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder().Build())
                .WithCorrespondenceLanguage(new LanguageBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .WithGender(new GenderBuilder().Build())
                .WithSurname("Schwarzenegger")
                .WithGivenName("Arnold")
                .WithBirthYear(1947)
                .Build(),
            new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder().Build())
                .WithCorrespondenceLanguage(new LanguageBuilder().Build())
                .WithLanguage(new LanguageBuilder().Build())
                .WithGender(new GenderBuilder().Build())
                .WithSurname("Lopez")
                .WithGivenName("Jennifer")
                .WithBirthYear(1969)
                .Build()
        ];

        _personRepository.GetByName(Arg.Any<string>()).Returns(persons);

        var personDetails = (await _service.GetByName("e"))?.ToList();

        Assert.That(personDetails, Is.Not.Null);
        Assert.That(personDetails, Has.Count.EqualTo(2));
    }

    [Test]
    public void ShouldMaskAddress_IsAdmin_ReturnsFalse()
    {
        _authorizationService.IsAdmin.Returns(true);

        var actual = _service.ShouldMaskAddress(new PersonBuilder().Build());

        Assert.That(actual, Is.False);
    }

    [Test]
    public void ShouldMaskAddress_ForPersonWithoutMemberships_ReturnsFalse()
    {
        var person = new PersonBuilder()
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithMemberships([])
            .Build();

        var actual = _service.ShouldMaskAddress(person);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void ShouldMaskAddress_ForPersonWithInactiveMemberships_ReturnsTrue()
    {
        var person = new PersonBuilder()
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithMemberships([new MembershipBuilder().WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddMonths(-1).AddDays(-1))).Build()])
            .Build();

        var actual = _service.ShouldMaskAddress(person);

        Assert.That(actual, Is.True);
    }

    [Test]
    public void ShouldMaskAddress_ForPersonWithActiveMemberships_ReturnsFalse()
    {
        var person = new PersonBuilder().WithMemberships([new MembershipBuilder().WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddMonths(-1).AddDays(1))).Build()]).Build();

        var actual = _service.ShouldMaskAddress(person);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void ShouldMaskAddress_ForPersonWithoutCorrespondenceAddress_ReturnsFalse()
    {
        var person = new PersonBuilder().WithoutCorrespondenceAddress().Build();

        var actual = _service.ShouldMaskAddress(person);

        Assert.That(actual, Is.False);
    }

    [Test]
    public async Task DeletePerson_AsDepartmentWithoutMemberships_ShouldDeleteDependencies()
    {
        var personId = Guid.NewGuid();
        var person = new PersonBuilder()
            .WithId(personId)
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithMemberships([])
            .Build();
        var interests = new List<Interest> { new InterestBuilder().Build() };
        var worklistTasks = new List<WorklistTask> { new WorklistTaskBuilder().Build() };
        var logMessages = new List<MembershipCandidateLogMessage> { new MembershipCandidateLogMessageBuilder().Build() };

        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);
        _personRepository.GetByIdForUpdate(personId).Returns(person);
        _membershipCandidateLogMessageRepository.GetByPersonId(personId).Returns(logMessages);
        _interestRepository.GetAllByPersonIdForUpdate(personId).Returns(interests);
        _worklistTaskRepository.GetByPersonOrMemberships(personId, Arg.Any<IEnumerable<Guid>>(), Arg.Any<IEnumerable<Guid>>()).Returns(worklistTasks);

        await _service.DeletePerson(personId);

        _membershipCandidateRepository.Received().DeleteRange(Arg.Is<IEnumerable<MembershipCandidate>>(c => !c.Any()));
        _membershipRepository.Received().DeleteRange(Arg.Is<IEnumerable<Membership>>(c => !c.Any()));
        _worklistTaskRepository.Received().DeleteRange(worklistTasks);
        _membershipCandidateLogMessageRepository.Received().DeleteRange(logMessages);
        _interestRepository.Received().DeleteRange(Arg.Is<IEnumerable<Interest>>(i => i.SequenceEqual(interests)));
        _personRepository.Received().Delete(person);
        await _personRepository.Received().CommitChanges();
    }

    [Test]
    public void DeletePerson_WithMembershipAsDepartment_ShouldThrowAuthorizationException()
    {
        var personId = Guid.NewGuid();
        var membership = new MembershipBuilder().Build();
        var person = new PersonBuilder()
            .WithId(personId)
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithMemberships([membership])
            .Build();

        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);
        _personRepository.GetByIdForUpdate(personId).Returns(person);

        Assert.That(async () => await _service.DeletePerson(personId), Throws.Exception.InstanceOf<AuthorizationException>());
    }

    [Test]
    public async Task DeletePerson_AsAdminWithMembershipCandidates_ShouldDeleteAllRelatedEntities()
    {
        var personId = Guid.NewGuid();
        var membership = new MembershipBuilder().Build();
        var person = new PersonBuilder()
            .WithId(personId)
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithMemberships([membership])
            .Build();
        var membershipCandidate = new MembershipCandidateBuilder().WithPerson(person).Build();
        var interests = new List<Interest> { new InterestBuilder().Build() };
        var worklistTasks = new List<WorklistTask> { new WorklistTaskBuilder().Build() };
        var logMessages = new List<MembershipCandidateLogMessage> { new MembershipCandidateLogMessageBuilder().Build() };

        _authorizationService.IsAdmin.Returns(true);
        _personRepository.GetByIdForUpdate(personId).Returns(person);
        _membershipCandidateRepository.GetByPersonIdForUpdate(personId).Returns(new List<MembershipCandidate> { membershipCandidate });
        _membershipCandidateLogMessageRepository.GetByPersonId(personId).Returns(logMessages);
        _interestRepository.GetAllByPersonIdForUpdate(personId).Returns(interests);
        _worklistTaskRepository.GetByPersonOrMemberships(personId, Arg.Any<IEnumerable<Guid>>(), Arg.Any<IEnumerable<Guid>>()).Returns(worklistTasks);

        await _service.DeletePerson(personId);

        _membershipCandidateRepository.Received().DeleteRange(Arg.Is<IEnumerable<MembershipCandidate>>(c => c.Single() == membershipCandidate));
        _membershipRepository.Received().DeleteRange(Arg.Is<IEnumerable<Membership>>(c => c.Single() == membership));
        _worklistTaskRepository.Received().DeleteRange(worklistTasks);
        _membershipCandidateLogMessageRepository.Received().DeleteRange(logMessages);
        _interestRepository.Received().DeleteRange(Arg.Is<IEnumerable<Interest>>(i => i.Single() == interests[0]));
        _personRepository.Received().Delete(person);
        await _personRepository.Received().CommitChanges();
    }

    [TestCase("Clark", "jim", 1936, Gender.MaleId, Language.GermanId, DuplicateReason.FullMatch)]
    [TestCase("CLARK", "JIM", 1936, Gender.MaleId, Language.GermanId, DuplicateReason.FullMatch)]
    [TestCase("clark", "jim", 1936, Gender.MaleId, Language.GermanId, DuplicateReason.FullMatch)]
    [TestCase("Clark", "Jim", 1936, Gender.MaleId, Language.GermanId, DuplicateReason.FullMatch)]
    [TestCase("Clark", "jimm", 1936, Gender.MaleId, Language.GermanId, DuplicateReason.GivenNameMismatch)]
    [TestCase("Clark", "Jim", 1940, Gender.MaleId, Language.GermanId, DuplicateReason.InBirthYearRange)]
    [TestCase("Clark", "Jim", 1963, Gender.MaleId, Language.GermanId, DuplicateReason.InBirthYearRange)]
    [TestCase("Clark", "Jim", 1950, Gender.MaleId, Language.GermanId, DuplicateReason.NoDuplicateFound)]
    [TestCase("Clark", "Jim", 1936, Gender.MaleId, Language.FrenchId, DuplicateReason.LanguageMismatch)]
    [TestCase("lark", "Jim", 1936, Gender.MaleId, Language.GermanId, DuplicateReason.NoDuplicateFound)]
    [TestCase("Clark", "Jxm", 1936, Gender.MaleId, Language.GermanId, DuplicateReason.NoDuplicateFound)]
    [TestCase("Clark", "Jim", 1936, Gender.FemaleId, Language.GermanId, DuplicateReason.NoDuplicateFound)]
    public async Task GetDuplicatePersonForGeneralElection_ShouldFindCorrectSimilarity(string surname, string givenName, int birthYear, string genderId, string languageId, DuplicateReason duplicateKind)
    {
        var membershipCandidate = new MembershipCandidateBuilder().WithSurname(surname).WithGivenName(givenName).WithBirthYear(birthYear).WithGenderId(Guid.Parse(genderId)).WithLanguageId(Guid.Parse(languageId)).Build();
        List<Person> persons =
        [
            new PersonBuilder()
                .WithCorrespondenceAddress(new AddressBuilder().Build())
                .WithCorrespondenceLanguage(new LanguageBuilder().Build())
                .WithLanguage(new LanguageBuilder().WithId(Guid.Parse(Language.GermanId)).Build())
                .WithGender(new GenderBuilder().WithId(Guid.Parse(Gender.MaleId)).Build())
                .WithSurname("Clark")
                .WithGivenName("Jim")
                .WithBirthYear(1936)
                .Build()
        ];
        _personRepository.GetPersonsByBirthYear(Arg.Any<int>(), Arg.Any<int>()).Returns(persons);

        var personDetails = await _service.GetDuplicatePersonForGeneralElection(membershipCandidate);

        Assert.That(personDetails, Is.Not.Null);
        Assert.That(personDetails.DuplicateReason, Is.EqualTo(duplicateKind));
    }
}
