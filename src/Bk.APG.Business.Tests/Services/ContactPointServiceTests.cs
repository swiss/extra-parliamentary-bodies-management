using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class ContactPointServiceTests
{
    private readonly IContactPointRepository _contactPointRepository = Substitute.For<IContactPointRepository>();
    private readonly ICommitteeRepository _committeeRepository = Substitute.For<ICommitteeRepository>();
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository = Substitute.For<IGeneralElectionCommitteeRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IMasterDataService _masterDataService = Substitute.For<IMasterDataService>();
    private readonly IWorklistTaskRepository _worklistTaskRepository = Substitute.For<IWorklistTaskRepository>();
    private readonly ILogger<ContactPointService> _logger = NullLogger<ContactPointService>.Instance;

    private ContactPointService _service = null!;
    private readonly Guid _committeeId = Guid.Parse("6D0744AD-A600-4D39-AAD7-3D48DAD095BE");
    private readonly Guid _languageId = Guid.Parse("C7EDE877-CC5F-4D6C-804D-98FD2FDE6009");
    private readonly Guid _genderId = Guid.Parse("13223736-08F7-49DB-846A-6C5AAF39A7E4");
    private readonly Guid _contactPointTypeId = Guid.Parse("1AA3A8E1-CECE-4499-970F-7D0318F7C9D8");
    private readonly Guid _guid = Guid.Parse("6B911AD7-9312-49E9-A0FA-B4401E042FB7");
    private ContactPoint _contactPointToUpdate;
    private ContactPoint _contactPoint1;
    private ContactPoint _contactPoint2;
    private List<ContactPoint> _contactPointList;
    private Committee _committee;

    [SetUp]
    public void SetUp()
    {
        _service = new ContactPointService(
            _contactPointRepository,
            _committeeRepository,
            _generalElectionCommitteeRepository,
            _authorizationService,
            _masterDataService,
            _worklistTaskRepository,
            _logger);

        _committee = new CommitteeBuilder()
            .WithId(_committeeId)
            .WithBeginDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-100)))
            .Build();

        _contactPoint1 = new ContactPointBuilder()
            .WithId(Guid.Parse("8697753c-f57a-4805-b1e9-bb1e546f4101"))
            .WithBeginDate(new DateOnly(2025, 1, 1))
            .WithEndDate(new DateOnly(2027, 12, 31))
            .WithCommittee(new CommitteeBuilder().WithId(_committeeId).WithGermanDescription("Test Gremium").Build())
            .WithContactPointType(new ContactPointTypeBuilder().WithId(_contactPointTypeId).Build())
            .WithGenderId(Guid.Parse("9061213B-0E20-4B44-AA1C-4860372E15DA"))
            .WithCompanyName("Test Firma")
            .WithSurnameAndGivenName("Berger", "Rüdiger")
            .Build();

        _contactPoint2 = new ContactPointBuilder()
            .WithId(Guid.Parse("9C6F1A13-4535-404C-ABDD-EEB854B5494A"))
            .WithCommittee(new CommitteeBuilder().WithId(_committeeId).WithGermanDescription("Test Gremium 2").Build())
            .WithContactPointType(new ContactPointTypeBuilder().WithId(_contactPointTypeId).Build())
            .WithGenderId(Guid.Parse("9061213B-0E20-4B44-AA1C-4860372E15DA"))
            .WithCompanyName("Test Firma 2")
            .WithSurnameAndGivenName("Tanner", "René")
            .Build();

        _contactPointToUpdate = new ContactPointBuilder()
            .WithId(_guid)
            .WithCommittee(_committee)
            .Build();

        _contactPointList = [_contactPoint1, _contactPoint2];

        _contactPointRepository.GetContactPointsByCommitteeId(_committeeId).Returns(_contactPointList);
        _committeeRepository.GetById(Arg.Any<Guid>()).Returns(_committee);
        _authorizationService.HasAccessToCommittee(Arg.Any<Committee>()).Returns(true);

        _masterDataService.GetContactPointGuidFromContactPointUri(Arg.Any<string>()).Returns(_contactPointTypeId);
        _generalElectionCommitteeRepository.GetByCommitteeId(Arg.Any<Guid>())
            .Returns(_ => Task.FromException<GeneralElectionCommittee>(new EntityNotFoundException("Not found")));
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(Arg.Any<Guid>()).Returns([]);
    }

    [TearDown]
    public void TearDown()
    {
        _contactPointRepository.ClearSubstitute();
        _authorizationService.ClearSubstitute();
        _masterDataService.ClearSubstitute();
        _generalElectionCommitteeRepository.ClearSubstitute();
        _worklistTaskRepository.ClearSubstitute();
    }

    [Test]
    public async Task GetContactPointDetailsByCommitteeId_WhenCalled_ShouldCallRepository()
    {
        var contactPoints = (await _service.GetContactPointListByCommitteeId(_committeeId)).ToList();

        await _contactPointRepository.Received(1).GetContactPointsByCommitteeId(Arg.Is(_committeeId));

        Assert.That(contactPoints, Is.Not.Null);
        Assert.That(contactPoints, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetEmpty_WhenCalled_ShouldReturnDto()
    {
        var emptyContactPoint = await _service.GetEmpty(_committeeId);

        Assert.That(emptyContactPoint, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(emptyContactPoint.ContactPointTypeId, Is.EqualTo(Guid.Empty));
            Assert.That(emptyContactPoint.ContactPointTypeUri, Is.EqualTo(string.Empty));
            Assert.That(emptyContactPoint.Zip, Is.EqualTo(string.Empty));
            Assert.That(emptyContactPoint.City, Is.EqualTo(string.Empty));
        });
    }

    [Test]
    public async Task UpdateContactPoint_WithValidData_ShouldUpdateContactPoint()
    {
        var updateDto = new ContactPointUpdateDto
        {
            Id = _contactPointToUpdate.Id,
            CommitteeId = _committeeId,
            ContactPointTypeId = _contactPointTypeId,
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
            GenderId = _genderId,
            LanguageId = _languageId,
            RowVersion = 666
        };

        _contactPointRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_contactPointToUpdate);

        await _service.Update(_contactPointToUpdate.Id, updateDto);

        _authorizationService.Received(1).GetCurrentUserName();
        await _masterDataService.Received(1).GetContactPointGuidFromContactPointUri(updateDto.ContactPointTypeUri);
        await _contactPointRepository.Received(1).GetByIdForUpdate(_contactPointToUpdate.Id, updateDto.RowVersion);
        await _contactPointRepository.Received(1).CommitChanges();

        Assert.That(_contactPointToUpdate, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(_contactPointToUpdate.CommitteeId, Is.EqualTo(updateDto.CommitteeId));
            Assert.That(_contactPointToUpdate.ContactPointTypeId, Is.EqualTo(updateDto.ContactPointTypeId));
            Assert.That(_contactPointToUpdate.CompanyName, Is.EqualTo(updateDto.CompanyName));
            Assert.That(_contactPointToUpdate.Section, Is.EqualTo(updateDto.Section));
            Assert.That(_contactPointToUpdate.BeginDate, Is.EqualTo(updateDto.BeginDate));
            Assert.That(_contactPointToUpdate.EndDate, Is.EqualTo(updateDto.EndDate));
            Assert.That(_contactPointToUpdate.Street, Is.EqualTo(updateDto.Street));
            Assert.That(_contactPointToUpdate.PoBox, Is.EqualTo(updateDto.PoBox));
            Assert.That(_contactPointToUpdate.Zip, Is.EqualTo(updateDto.Zip));
            Assert.That(_contactPointToUpdate.City, Is.EqualTo(updateDto.City));
            Assert.That(_contactPointToUpdate.Phone, Is.EqualTo(updateDto.Phone));
            Assert.That(_contactPointToUpdate.Email, Is.EqualTo(updateDto.Email));
            Assert.That(_contactPointToUpdate.Surname, Is.EqualTo(updateDto.Surname));
            Assert.That(_contactPointToUpdate.GivenName, Is.EqualTo(updateDto.GivenName));
            Assert.That(_contactPointToUpdate.Title, Is.EqualTo(updateDto.Title));
            Assert.That(_contactPointToUpdate.PersonalPhone, Is.EqualTo(updateDto.PersonalPhone));
            Assert.That(_contactPointToUpdate.PersonalMobile, Is.EqualTo(updateDto.PersonalMobile));
            Assert.That(_contactPointToUpdate.PersonalEmail, Is.EqualTo(updateDto.PersonalEmail));
            Assert.That(_contactPointToUpdate.ReleasePersonData, Is.EqualTo(updateDto.ReleasePersonData));
            Assert.That(_contactPointToUpdate.OldId, Is.EqualTo(updateDto.OldId));
            Assert.That(_contactPointToUpdate.LanguageId, Is.EqualTo(updateDto.LanguageId));
            Assert.That(_contactPointToUpdate.GenderId, Is.EqualTo(updateDto.GenderId));
        });
    }

    [Test]
    public async Task UpdateContactPoint_WhenContactPointsResolveTasks_ShouldCompleteTasks()
    {
        var generalElectionCommitteeId = Guid.NewGuid();
        var updateDto = new ContactPointUpdateDto
        {
            Id = _contactPointToUpdate.Id,
            CommitteeId = _committeeId,
            ContactPointTypeId = _contactPointTypeId,
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
            GenderId = _genderId,
            LanguageId = _languageId,
            RowVersion = 666
        };

        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .WithContactPoint(new ContactPointBuilder()
                .WithContactPointType(new ContactPointTypeBuilder().WithId(ContactPointType.SecretariatGuid).Build())
                .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
                .Build())
            .WithContactPoint(new ContactPointBuilder()
                .WithContactPointType(new ContactPointTypeBuilder().WithId(ContactPointType.DataProtectionOfficerGuid).Build())
                .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
                .Build())
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithId(generalElectionCommitteeId)
            .WithCommittee(committee)
            .Build();

        var missingSecretariatTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingSecretariat)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();

        var missingDataProtectionOfficerTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingDataProtectionOfficer)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(_committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommitteeId)
            .Returns([missingSecretariatTask, missingDataProtectionOfficerTask]);
        _contactPointRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_contactPointToUpdate);

        await _service.Update(_contactPointToUpdate.Id, updateDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(missingSecretariatTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
            Assert.That(missingDataProtectionOfficerTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
        }

        await _worklistTaskRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task UpdateContactPoint_WhenContactPointsStillMissing_ShouldNotCompleteTasks()
    {
        var generalElectionCommitteeId = Guid.NewGuid();
        var updateDto = new ContactPointUpdateDto
        {
            Id = _contactPointToUpdate.Id,
            CommitteeId = _committeeId,
            ContactPointTypeId = _contactPointTypeId,
            ContactPointTypeUri = "my.uri.admin.ch",
            CompanyName = "Test Amt Missing",
            BeginDate = new DateOnly(2020, 1, 1),
            EndDate = new DateOnly(2027, 12, 31),
            Zip = "3000",
            City = "Bern 12",
            GenderId = _genderId,
            LanguageId = _languageId,
            RowVersion = 666
        };

        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithId(generalElectionCommitteeId)
            .WithCommittee(committee)
            .Build();

        var missingSecretariatTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingSecretariat)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(_committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommitteeId)
            .Returns([missingSecretariatTask]);
        _contactPointRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_contactPointToUpdate);

        await _service.Update(_contactPointToUpdate.Id, updateDto);

        Assert.That(missingSecretariatTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Active));
        await _worklistTaskRepository.DidNotReceive().CommitChanges();
    }

    [Test]
    public async Task UpdateContactPointInCopyMode_WithValidData_ShouldCreateContactPoint()
    {
        var updateDto = new ContactPointUpdateDto
        {
            Id = _contactPointToUpdate.Id,
            CommitteeId = _committeeId,
            ContactPointTypeId = _contactPointTypeId,
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
            GenderId = _genderId,
            LanguageId = _languageId,
            IsCopy = true,
            RowVersion = 666
        };

        _contactPointRepository.GetById(Arg.Any<Guid>()).Returns(_contactPointToUpdate);

        await _service.Update(_contactPointToUpdate.Id, updateDto);

        _authorizationService.Received(1).GetCurrentUserName();
        await _contactPointRepository.Received(1).Create(Arg.Any<ContactPoint>());
        await _contactPointRepository.Received(0).CommitChanges();

        Assert.That(_contactPointToUpdate, Is.Not.Null);
    }

    [TestCase("2025-04-01", "2025-12-31")]
    [TestCase("2023-04-01", "2026-12-31")]
    [TestCase("2025-04-01", null)]
    [TestCase("2023-04-01", "2028-12-31")]
    public void UpdateContactPointInCopyMode_WithDuplicateNameAndDate_ShouldThrow(string beginDate, string? endDate)
    {
        // The daterange of the existing record is 01.01.2025 - 31.12.2027. All dates touching this range, are invalid!

        var updateDto = GetContactPointUpdateDtoForDuplicateCheck();

        var begin = DateOnly.Parse(beginDate, CultureInfo.InvariantCulture);
        updateDto.BeginDate = begin;

        if (endDate is not null)
        {
            var end = DateOnly.Parse(endDate, CultureInfo.InvariantCulture);
            updateDto.EndDate = end;
        }
        else
        {
            updateDto.EndDate = null;
        }

        _contactPointRepository.GetById(Arg.Any<Guid>()).Returns(_contactPointToUpdate);

        Assert.That(async () => await _service.Update(_contactPointToUpdate.Id, updateDto), Throws.Exception.InstanceOf<BusinessValidationException>());
    }

    [TestCase("2023-04-01", "2023-06-01")]
    [TestCase("2024-04-01", "2024-06-01")]
    [TestCase("2028-01-01", "2029-12-31")]
    [TestCase("2028-01-01", null)]
    public async Task UpdateContactPointInCopyMode_WithDuplicateNameAndValidDate_ShouldSucceed(string beginDate, string? endDate)
    {
        // The daterange of the existing record is 01.01.2025 - 31.12.2027. All dates not touching this range, are valid!
        var updateDto = GetContactPointUpdateDtoForDuplicateCheck();

        var begin = DateOnly.Parse(beginDate, CultureInfo.InvariantCulture);
        updateDto.BeginDate = begin;

        if (endDate is not null)
        {
            var end = DateOnly.Parse(endDate, CultureInfo.InvariantCulture);
            updateDto.EndDate = end;
        }
        else
        {
            updateDto.EndDate = null;
        }

        _contactPointRepository.GetById(Arg.Any<Guid>()).Returns(_contactPointToUpdate);

        await _service.Update(_contactPointToUpdate.Id, updateDto);

        _authorizationService.Received(1).GetCurrentUserName();
        await _contactPointRepository.Received(1).Create(Arg.Any<ContactPoint>());
        await _contactPointRepository.Received(0).CommitChanges();

        Assert.That(_contactPointToUpdate, Is.Not.Null);
    }

    [Test]
    public async Task UpdateContactPointInCopyMode_WithDifferentCompanyName_ShouldNotThrow()
    {
        var updateDto = GetContactPointUpdateDtoForDuplicateCheck();
        updateDto.CompanyName = "Different";

        _contactPointRepository.GetById(Arg.Any<Guid>()).Returns(_contactPointToUpdate);

        await _service.Update(_contactPointToUpdate.Id, updateDto);

        _authorizationService.Received(1).GetCurrentUserName();
        await _contactPointRepository.Received(1).Create(Arg.Any<ContactPoint>());
        await _contactPointRepository.Received(0).CommitChanges();

        Assert.That(_contactPointToUpdate, Is.Not.Null);
    }

    [Test]
    public async Task DeleteContactPoint_WithExistingId_ShouldRemoveContactPoint()
    {
        _contactPointRepository.GetByIdForUpdate(_guid).Returns(_contactPointToUpdate);

        await _service.Delete(_guid);

        await _authorizationService.Received(1).HasAccessToCommittee(_contactPointToUpdate.Committee!);
        await _contactPointRepository.Received(1).GetByIdForUpdate(_guid);
        _contactPointRepository.Received(1).Delete(_contactPointToUpdate);
        await _contactPointRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task DeleteContactPoint_WhenUserIsNotAuthorized_ShouldThrow()
    {
        _authorizationService.HasAccessToCommittee(_committee).Returns(false);
        _contactPointRepository.GetByIdForUpdate(_guid).Returns(_contactPointToUpdate);

        Assert.That(async () => await _service.Delete(_guid), Throws.Exception.InstanceOf<AuthorizationException>());
        _contactPointRepository.DidNotReceive().Delete(Arg.Any<ContactPoint>());
        await _contactPointRepository.Received(0).CommitChanges();
    }

    [Test]
    public async Task Create_WhenCalledWithValidData_ShouldCallRepository()
    {
        var createDto = new ContactPointCreateDto
        {
            CommitteeId = _committeeId,
            ContactPointTypeId = _contactPointTypeId,
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
            GenderId = _genderId,
            LanguageId = _languageId,
            IsCopy = false,
        };

        var contactPoint = new ContactPoint
        {
            CommitteeId = _committeeId,
            ContactPointTypeId = _contactPointTypeId,
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
            GenderId = _genderId,
            LanguageId = _languageId,
            Created = DateTime.UtcNow,
            CreatedBy = "tester",
            Modified = DateTime.UtcNow,
            ModifiedBy = "tester"
        };

        _contactPointRepository.GetById(Arg.Any<Guid>()).Returns(contactPoint);

        await _service.Create(createDto);

        await _masterDataService.Received(1).GetContactPointGuidFromContactPointUri(createDto.ContactPointTypeUri);

        _authorizationService.Received(1).GetCurrentUserName();
        await _contactPointRepository.Received(1).Create(
            Arg.Is<ContactPoint>(p => p.CompanyName == "Test Amt"
                && p.Surname == "Muster"
                && p.GivenName == "Rita"));

        await _contactPointRepository.Received(1).GetById(Arg.Any<Guid>());
    }

    [Test]
    public async Task CreateContactPoint_WhenContactPointsResolveTasks_ShouldCompleteTasks()
    {
        var generalElectionCommitteeId = Guid.NewGuid();
        var createDto = new ContactPointCreateDto
        {
            CommitteeId = _committeeId,
            ContactPointTypeId = _contactPointTypeId,
            ContactPointTypeUri = "my.uri.admin.ch",
            CompanyName = "New Company",
            BeginDate = new DateOnly(2020, 1, 1),
            EndDate = new DateOnly(2027, 12, 31),
            Zip = "3000",
            City = "Bern 12",
            GenderId = _genderId,
            LanguageId = _languageId,
            IsCopy = false,
        };

        var committee = new CommitteeBuilder()
            .WithCommitteeTypeId(CommitteeType.AuthoritiesCommissionGuid)
            .WithContactPoint(new ContactPointBuilder()
                .WithContactPointType(new ContactPointTypeBuilder().WithId(ContactPointType.SecretariatGuid).Build())
                .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
                .Build())
            .WithContactPoint(new ContactPointBuilder()
                .WithContactPointType(new ContactPointTypeBuilder().WithId(ContactPointType.DataProtectionOfficerGuid).Build())
                .WithEndDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
                .Build())
            .Build();

        var generalElectionCommittee = new GeneralElectionCommitteeBuilder()
            .WithId(generalElectionCommitteeId)
            .WithCommittee(committee)
            .Build();

        var missingSecretariatTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingSecretariat)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();

        var missingDataProtectionOfficerTask = new WorklistTaskBuilder()
            .WithWorklistTaskTypeId(WorklistTaskType.GeneralElectionMissingDataProtectionOfficer)
            .WithWorklistTaskStateId(WorklistTaskState.Active)
            .Build();

        _generalElectionCommitteeRepository.GetByCommitteeId(_committeeId).Returns(generalElectionCommittee);
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(generalElectionCommitteeId)
            .Returns([missingSecretariatTask, missingDataProtectionOfficerTask]);
        _contactPointRepository.GetById(Arg.Any<Guid>()).Returns(new ContactPointBuilder().WithCommittee(committee).Build());

        await _service.Create(createDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(missingSecretariatTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
            Assert.That(missingDataProtectionOfficerTask.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Completed));
        }

        await _worklistTaskRepository.Received(1).CommitChanges();
    }

    [Test]
    public void Create_WhenCalledWithDuplicateData_ShouldThrow()
    {
        var createDto = new ContactPointCreateDto
        {
            CommitteeId = _committeeId,
            ContactPointTypeId = _contactPointTypeId,
            ContactPointTypeUri = "my.uri.admin.ch",
            CompanyName = "Test Firma",
            Section = "Sektion Nr. 1",
            BeginDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2027, 12, 31),
            Street = "Teststrasse 2",
            PoBox = "Postfach",
            Zip = "3000",
            City = "Bern 12",
            Phone = "+41 77 444 33 22",
            Email = "test@testamt.ch",
            Surname = "Berger",
            GivenName = "Rüdiger",
            Title = "Datenschutzberater",
            PersonalPhone = "+41 78 777 66 55",
            PersonalMobile = "+41 78 999 88 77",
            PersonalEmail = "r.muster@test.ch",
            ReleasePersonData = false,
            OldId = 123123,
            GenderId = _genderId,
            LanguageId = _languageId,
            IsCopy = false,
        };

        Assert.That(async () => await _service.Create(createDto), Throws.Exception.InstanceOf<BusinessValidationException>());
    }

    private ContactPointUpdateDto GetContactPointUpdateDtoForDuplicateCheck()
    {
        return new ContactPointUpdateDto
        {
            Id = _contactPointToUpdate.Id,
            CommitteeId = _committeeId,
            ContactPointTypeId = _contactPointTypeId,
            ContactPointTypeUri = "my.uri.admin.ch",
            CompanyName = "Test Firma",
            Section = "Sektion Nr. 1",
            BeginDate = new DateOnly(2025, 4, 1),
            EndDate = new DateOnly(2025, 12, 31),
            Street = "Teststrasse 2",
            PoBox = "Postfach",
            Zip = "3000",
            City = "Bern 12",
            Phone = "+41 77 444 33 22",
            Email = "test@testamt.ch",
            Surname = "Berger",
            GivenName = "Rüdiger",
            Title = "Datenschutzberaterin",
            PersonalPhone = "+41 78 777 66 55",
            PersonalMobile = "+41 78 999 88 77",
            PersonalEmail = "r.muster@test.ch",
            ReleasePersonData = false,
            OldId = 123123,
            GenderId = _genderId,
            LanguageId = _languageId,
            IsCopy = true,
            RowVersion = 666
        };
    }
}
