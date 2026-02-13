using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute.ExceptionExtensions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class FormLetterSenderServiceTests
{
    private readonly IFormLetterSenderRepository _formLetterSenderRepository = Substitute.For<IFormLetterSenderRepository>();
    private readonly IDocumentStorageRepository _documentStorageRepository = Substitute.For<IDocumentStorageRepository>();
    private readonly IDocumentService _documentService = Substitute.For<IDocumentService>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly ILogger<FormLetterSenderService> _logger = NullLogger<FormLetterSenderService>.Instance;

    private FormLetterSenderService _service = null!;
    private const string CurrentUserName = "testuser";

    [SetUp]
    public void SetUp()
    {
        _service = new FormLetterSenderService(
            _formLetterSenderRepository,
            _documentStorageRepository,
            _documentService,
            _authorizationService,
            _logger);

        _authorizationService.GetCurrentUserName().Returns(CurrentUserName);
    }

    [TearDown]
    public void TearDown()
    {
        _formLetterSenderRepository.ClearSubstitute();
        _documentStorageRepository.ClearSubstitute();
        _documentService.ClearSubstitute();
        _authorizationService.ClearSubstitute();
    }

    [Test]
    public async Task CreateFormLetterSender_WithoutSignature_ReturnsCreatedFormLetterSender()
    {
        var createDto = CreateFormLetterSenderCreateDto();
        var expectedId = Guid.NewGuid();
        var createdFormLetterSender = new FormLetterSender
        {
            Id = expectedId,
            Description = createDto.Description,
            Surname = createDto.Surname,
            GivenName = createDto.GivenName,
            SenderFunctionId = createDto.SenderFunctionId,
            DepartmentId = createDto.DepartmentId,
            OfficeId = createDto.OfficeId,
            StreetGerman = createDto.StreetGerman,
            StreetFrench = createDto.StreetFrench,
            StreetItalian = createDto.StreetItalian,
            StreetRomansh = createDto.StreetRomansh,
            Zip = createDto.Zip,
            CityGerman = createDto.CityGerman,
            CityFrench = createDto.CityFrench,
            CityItalian = createDto.CityItalian,
            CityRomansh = createDto.CityRomansh,
            Phone = createDto.Phone,
            Email = createDto.Email,
            Website = createDto.Website,
            Created = DateTime.UtcNow,
            CreatedBy = CurrentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = CurrentUserName
        };

        _formLetterSenderRepository.Create(Arg.Any<FormLetterSender>()).Returns(createdFormLetterSender);

        var result = await _service.CreateFormLetterSender(createDto);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(expectedId));
            Assert.That(result.Description, Is.EqualTo(createDto.Description));
            Assert.That(result.Surname, Is.EqualTo(createDto.Surname));
            Assert.That(result.GivenName, Is.EqualTo(createDto.GivenName));
        }
    }

    [Test]
    public async Task CreateFormLetterSender_WithSignature_UploadsSignatureAndReturnsCreatedFormLetterSender()
    {
        var createDto = CreateFormLetterSenderCreateDto();
        var signature = CreateFormFile("signature.png", "image/png");
        createDto.Signature = signature;

        var documentStorageId = Guid.NewGuid();
        var expectedId = Guid.NewGuid();

        _documentService.UploadDocument(Arg.Any<byte[]>()).Returns(documentStorageId.ToString());

        var createdFormLetterSender = new FormLetterSender
        {
            Id = expectedId,
            Description = createDto.Description,
            Surname = createDto.Surname,
            GivenName = createDto.GivenName,
            SenderFunctionId = createDto.SenderFunctionId,
            DepartmentId = createDto.DepartmentId,
            OfficeId = createDto.OfficeId,
            StreetGerman = createDto.StreetGerman,
            StreetFrench = createDto.StreetFrench,
            StreetItalian = createDto.StreetItalian,
            StreetRomansh = createDto.StreetRomansh,
            Zip = createDto.Zip,
            CityGerman = createDto.CityGerman,
            CityFrench = createDto.CityFrench,
            CityItalian = createDto.CityItalian,
            CityRomansh = createDto.CityRomansh,
            Phone = createDto.Phone,
            Email = createDto.Email,
            Website = createDto.Website,
            SignatureFileReference = new DocumentStorage
            {
                DocumentName = signature.FileName,
                DocumentStorageId = documentStorageId.ToString(),
                Created = DateTime.UtcNow,
                CreatedBy = CurrentUserName,
                Modified = DateTime.UtcNow,
                ModifiedBy = CurrentUserName
            },
            Created = DateTime.UtcNow,
            CreatedBy = CurrentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = CurrentUserName
        };

        _formLetterSenderRepository.Create(Arg.Any<FormLetterSender>()).Returns(createdFormLetterSender);

        var result = await _service.CreateFormLetterSender(createDto);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(expectedId));
            Assert.That(result.SignatureFileName, Is.EqualTo(signature.FileName));
        }

        await _documentService.Received(1).UploadDocument(Arg.Any<byte[]>());
        await _formLetterSenderRepository.Received(1).Create(Arg.Any<FormLetterSender>());
    }

    [Test]
    public void CreateFormLetterSender_WithSignatureUploadFailure_RollsBackAndThrowsException()
    {
        var createDto = CreateFormLetterSenderCreateDto();
        var signature = CreateFormFile("signature.png", "image/png");
        createDto.Signature = signature;

        var documentStorageId = Guid.NewGuid();
        _documentService.UploadDocument(Arg.Any<byte[]>()).Returns(documentStorageId.ToString());

        _formLetterSenderRepository.Create(Arg.Any<FormLetterSender>()).Throws(new InvalidOperationException("Database error"));

        Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateFormLetterSender(createDto));
    }

    [Test]
    public void CreateFormLetterSender_RepositoryThrows_CleanupSignatureAndThrows()
    {
        var createDto = CreateFormLetterSenderCreateDto();
        var signature = CreateFormFile("signature.png", "image/png");
        createDto.Signature = signature;

        var documentStorageId = Guid.NewGuid();
        _documentService.UploadDocument(Arg.Any<byte[]>()).Returns(documentStorageId.ToString());

        const string exceptionMessage = "Repository error";
        _formLetterSenderRepository.Create(Arg.Any<FormLetterSender>()).Throws(new InvalidOperationException(exceptionMessage));

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateFormLetterSender(createDto));
        Assert.That(ex.Message, Is.EqualTo(exceptionMessage));
    }

    [Test]
    public async Task CreateFormLetterSender_AllFieldsAreMappedCorrectly()
    {
        var createDto = new FormLetterSenderCreateDto
        {
            Description = "Test Description",
            Surname = "Doe",
            GivenName = "John",
            SenderFunctionId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Strasse 1",
            StreetFrench = "Rue 1",
            StreetItalian = "Via 1",
            StreetRomansh = "Via 1",
            Zip = "3011",
            CityGerman = "Bern",
            CityFrench = "Berne",
            CityItalian = "Berna",
            CityRomansh = "Berna",
            Phone = "031 123 4567",
            Email = "john.doe@example.com",
            Website = "https://example.com",
            Signature = null
        };

        var createdFormLetterSender = FormLetterSenderMapper.FromFormLetterSenderCreateDto(createDto, CurrentUserName);

        _formLetterSenderRepository.Create(Arg.Any<FormLetterSender>()).Returns(createdFormLetterSender);

        var result = await _service.CreateFormLetterSender(createDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Description, Is.EqualTo(createDto.Description));
            Assert.That(result.Surname, Is.EqualTo(createDto.Surname));
            Assert.That(result.GivenName, Is.EqualTo(createDto.GivenName));
            Assert.That(result.SenderFunctionId, Is.EqualTo(createDto.SenderFunctionId));
            Assert.That(result.DepartmentId, Is.EqualTo(createDto.DepartmentId));
            Assert.That(result.OfficeId, Is.EqualTo(createDto.OfficeId));
            Assert.That(result.StreetGerman, Is.EqualTo(createDto.StreetGerman));
            Assert.That(result.StreetFrench, Is.EqualTo(createDto.StreetFrench));
            Assert.That(result.StreetItalian, Is.EqualTo(createDto.StreetItalian));
            Assert.That(result.StreetRomansh, Is.EqualTo(createDto.StreetRomansh));
            Assert.That(result.Zip, Is.EqualTo(createDto.Zip));
            Assert.That(result.CityGerman, Is.EqualTo(createDto.CityGerman));
            Assert.That(result.CityFrench, Is.EqualTo(createDto.CityFrench));
            Assert.That(result.CityItalian, Is.EqualTo(createDto.CityItalian));
            Assert.That(result.CityRomansh, Is.EqualTo(createDto.CityRomansh));
            Assert.That(result.Phone, Is.EqualTo(createDto.Phone));
            Assert.That(result.Email, Is.EqualTo(createDto.Email));
            Assert.That(result.Website, Is.EqualTo(createDto.Website));
        }
    }

    [Test]
    public async Task UpdateFormLetterSender_UpdateAllFields_ReturnsUpdatedFormLetterSender()
    {
        var formLetterSenderId = Guid.NewGuid();
        var existingFormLetterSender = CreateExistingFormLetterSender(formLetterSenderId);

        var updateDto = new FormLetterSenderUpdateDto
        {
            Id = formLetterSenderId,
            Description = "Updated Description",
            Surname = "Smith",
            GivenName = "Jane",
            SenderFunctionId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Updated Strasse",
            StreetFrench = "Updated Rue",
            StreetItalian = "Updated Via",
            StreetRomansh = "Updated Via",
            Zip = "3012",
            CityGerman = "Zurich",
            CityFrench = "Zurich",
            CityItalian = "Zurigo",
            CityRomansh = "Turitg",
            Phone = "044 987 6543",
            Email = "jane.smith@example.com",
            Website = "https://example.org",
            SignatureFileName = null,
            Signature = null
        };

        _formLetterSenderRepository.GetByIdForUpdate(formLetterSenderId).Returns(existingFormLetterSender);

        var result = await _service.UpdateFormLetterSender(formLetterSenderId, updateDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Description, Is.EqualTo(updateDto.Description));
            Assert.That(result.Surname, Is.EqualTo(updateDto.Surname));
            Assert.That(result.GivenName, Is.EqualTo(updateDto.GivenName));
            Assert.That(result.StreetGerman, Is.EqualTo(updateDto.StreetGerman));
            Assert.That(result.Email, Is.EqualTo(updateDto.Email));
            Assert.That(result.Phone, Is.EqualTo(updateDto.Phone));
            Assert.That(result.Website, Is.EqualTo(updateDto.Website));
        }

        await _formLetterSenderRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task UpdateFormLetterSender_WithoutSignatureNoChange_ReturnsUpdatedFormLetterSender()
    {
        var formLetterSenderId = Guid.NewGuid();
        var existingFormLetterSender = CreateExistingFormLetterSender(formLetterSenderId);

        var updateDto = new FormLetterSenderUpdateDto
        {
            Id = formLetterSenderId,
            Description = "Updated Description",
            Surname = "Smith",
            GivenName = "Jane",
            SenderFunctionId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Updated Strasse",
            StreetFrench = "Updated Rue",
            StreetItalian = "Updated Via",
            StreetRomansh = "Updated Via",
            Zip = "3012",
            CityGerman = "Zurich",
            CityFrench = "Zurich",
            CityItalian = "Zurigo",
            CityRomansh = "Turitg",
            Phone = "044 987 6543",
            Email = "jane.smith@example.com",
            Website = "https://example.org",
            SignatureFileName = "existing-signature.png",
            Signature = null
        };

        _formLetterSenderRepository.GetByIdForUpdate(formLetterSenderId).Returns(existingFormLetterSender);

        var result = await _service.UpdateFormLetterSender(formLetterSenderId, updateDto);

        Assert.That(result, Is.Not.Null);

        await _formLetterSenderRepository.Received(1).CommitChanges();
        await _documentService.DidNotReceive().RemoveDocument(Arg.Any<string>());
    }

    [Test]
    public async Task UpdateFormLetterSender_RemoveSignature_DeletesOldSignatureFile()
    {
        var formLetterSenderId = Guid.NewGuid();
        var signatureFileId = Guid.NewGuid();
        var existingFormLetterSender = CreateExistingFormLetterSender(formLetterSenderId, signatureFileId);

        var updateDto = new FormLetterSenderUpdateDto
        {
            Id = formLetterSenderId,
            Description = "Updated Description",
            Surname = "Smith",
            GivenName = "Jane",
            SenderFunctionId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Strasse",
            StreetFrench = "Rue",
            StreetItalian = "Via",
            StreetRomansh = "Via",
            Zip = "3012",
            CityGerman = "Zurich",
            CityFrench = "Zurich",
            CityItalian = "Zurigo",
            CityRomansh = "Turitg",
            SignatureFileName = "",
            Signature = null
        };

        var documentStorageEntry = new DocumentStorage
        {
            Id = signatureFileId,
            DocumentStorageId = Guid.NewGuid().ToString(),
            DocumentName = "old-signature.png",
            Created = DateTime.UtcNow,
            CreatedBy = "admin",
            Modified = DateTime.UtcNow,
            ModifiedBy = "admin"
        };

        _formLetterSenderRepository.GetByIdForUpdate(formLetterSenderId).Returns(existingFormLetterSender);
        _documentStorageRepository.GetByIdForUpdate(signatureFileId).Returns(documentStorageEntry);

        var result = await _service.UpdateFormLetterSender(formLetterSenderId, updateDto);

        Assert.That(result.SignatureFileName, Is.Null.Or.Empty);

        await _documentStorageRepository.Received(1).GetByIdForUpdate(signatureFileId);
        await _documentService.Received(1).RemoveDocument(documentStorageEntry.DocumentStorageId);
        _documentStorageRepository.Received(1).Delete(documentStorageEntry);
        await _formLetterSenderRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task UpdateFormLetterSender_ReplaceSignature_DeletesOldAndUploadsNew()
    {
        var formLetterSenderId = Guid.NewGuid();
        var oldSignatureFileId = Guid.NewGuid();
        var existingFormLetterSender = CreateExistingFormLetterSender(formLetterSenderId, oldSignatureFileId);

        var newSignature = CreateFormFile("new-signature.png", "image/png");
        var newDocumentStorageId = Guid.NewGuid();

        var updateDto = new FormLetterSenderUpdateDto
        {
            Id = formLetterSenderId,
            Description = "Updated Description",
            Surname = "Smith",
            GivenName = "Jane",
            SenderFunctionId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Strasse",
            StreetFrench = "Rue",
            StreetItalian = "Via",
            StreetRomansh = "Via",
            Zip = "3012",
            CityGerman = "Zurich",
            CityFrench = "Zurich",
            CityItalian = "Zurigo",
            CityRomansh = "Turitg",
            SignatureFileName = "old-signature.png",
            Signature = newSignature
        };

        var oldDocumentStorageEntry = new DocumentStorage
        {
            Id = oldSignatureFileId,
            DocumentStorageId = Guid.NewGuid().ToString(),
            DocumentName = "old-signature.png",
            Created = DateTime.UtcNow,
            CreatedBy = "admin",
            Modified = DateTime.UtcNow,
            ModifiedBy = "admin"
        };

        _formLetterSenderRepository.GetByIdForUpdate(formLetterSenderId).Returns(existingFormLetterSender);
        _documentStorageRepository.GetByIdForUpdate(oldSignatureFileId).Returns(oldDocumentStorageEntry);
        _documentService.UploadDocument(Arg.Any<byte[]>()).Returns(newDocumentStorageId.ToString());

        var result = await _service.UpdateFormLetterSender(formLetterSenderId, updateDto);

        Assert.That(result.SignatureFileName, Is.EqualTo(newSignature.FileName));

        await _documentStorageRepository.Received(1).GetByIdForUpdate(oldSignatureFileId);
        await _documentService.Received(1).RemoveDocument(oldDocumentStorageEntry.DocumentStorageId);
        _documentStorageRepository.Received(1).Delete(oldDocumentStorageEntry);
        await _documentService.Received(1).UploadDocument(Arg.Any<byte[]>());
        await _formLetterSenderRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task UpdateFormLetterSender_AddSignatureToExistingFormLetter_UploadsNewSignature()
    {
        var formLetterSenderId = Guid.NewGuid();
        var existingFormLetterSender = CreateExistingFormLetterSender(formLetterSenderId); // No signature

        var newSignature = CreateFormFile("new-signature.png", "image/png");
        var newDocumentStorageId = Guid.NewGuid();

        var updateDto = new FormLetterSenderUpdateDto
        {
            Id = formLetterSenderId,
            Description = "Updated Description",
            Surname = "Smith",
            GivenName = "Jane",
            SenderFunctionId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Strasse",
            StreetFrench = "Rue",
            StreetItalian = "Via",
            StreetRomansh = "Via",
            Zip = "3012",
            CityGerman = "Zurich",
            CityFrench = "Zurich",
            CityItalian = "Zurigo",
            CityRomansh = "Turitg",
            SignatureFileName = null,
            Signature = newSignature
        };

        _formLetterSenderRepository.GetByIdForUpdate(formLetterSenderId).Returns(existingFormLetterSender);
        _documentService.UploadDocument(Arg.Any<byte[]>()).Returns(newDocumentStorageId.ToString());

        var result = await _service.UpdateFormLetterSender(formLetterSenderId, updateDto);

        Assert.That(result.SignatureFileName, Is.EqualTo(newSignature.FileName));

        await _documentService.Received(1).UploadDocument(Arg.Any<byte[]>());
        await _documentStorageRepository.DidNotReceive().GetByIdForUpdate(Arg.Any<Guid>());
        await _formLetterSenderRepository.Received(1).CommitChanges();
    }

    [Test]
    public async Task UpdateFormLetterSender_UpdatesModificationFields()
    {
        var formLetterSenderId = Guid.NewGuid();
        var existingFormLetterSender = CreateExistingFormLetterSender(formLetterSenderId);
        var originalModified = existingFormLetterSender.Modified;

        var updateDto = new FormLetterSenderUpdateDto
        {
            Id = formLetterSenderId,
            Description = "Updated Description",
            Surname = "Smith",
            GivenName = "Jane",
            SenderFunctionId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Strasse",
            StreetFrench = "Rue",
            StreetItalian = "Via",
            StreetRomansh = "Via",
            Zip = "3012",
            CityGerman = "Zurich",
            CityFrench = "Zurich",
            CityItalian = "Zurigo",
            CityRomansh = "Turitg",
            SignatureFileName = null,
            Signature = null
        };

        _formLetterSenderRepository.GetByIdForUpdate(formLetterSenderId).Returns(existingFormLetterSender);

        await _service.UpdateFormLetterSender(formLetterSenderId, updateDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(existingFormLetterSender.Modified, Is.GreaterThanOrEqualTo(originalModified));
            Assert.That(existingFormLetterSender.ModifiedBy, Is.EqualTo(CurrentUserName));
        }
    }

    [Test]
    public async Task GetFormLetterSenderList_WhenUserIsDepartment_ShouldReturnOnlyDepartmentSenders()
    {
        var userDepartmentId = Guid.NewGuid();
        var otherDepartmentId = Guid.NewGuid();

        var department = new DepartmentBuilder().WithId(userDepartmentId).Build();

        var senderInUserDepartment = new FormLetterSender
        {
            Id = Guid.NewGuid(),
            Department = new DepartmentBuilder().Build(),
            DepartmentId = userDepartmentId,
            Description = "User Department Sender",
            Surname = "Doe",
            GivenName = "John",
            SenderFunction = new FormLetterSenderFunctionBuilder().Build(),
            SenderFunctionId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Street",
            StreetFrench = "Rue",
            StreetItalian = "Via",
            StreetRomansh = "Via",
            Zip = "3011",
            CityGerman = "Bern",
            CityFrench = "Berne",
            CityItalian = "Berna",
            CityRomansh = "Berna",
            Created = DateTime.UtcNow,
            CreatedBy = "admin",
            Modified = DateTime.UtcNow,
            ModifiedBy = "admin"
        };

        var senderInOtherDepartment = new FormLetterSender
        {
            Id = Guid.NewGuid(),
            Department = new DepartmentBuilder().Build(),
            DepartmentId = otherDepartmentId,
            Description = "Other Department Sender",
            Surname = "Smith",
            GivenName = "Jane",
            SenderFunction = new FormLetterSenderFunctionBuilder().Build(),
            SenderFunctionId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Street",
            StreetFrench = "Rue",
            StreetItalian = "Via",
            StreetRomansh = "Via",
            Zip = "3012",
            CityGerman = "Zurich",
            CityFrench = "Zurich",
            CityItalian = "Zurigo",
            CityRomansh = "Turitg",
            Created = DateTime.UtcNow,
            CreatedBy = "admin",
            Modified = DateTime.UtcNow,
            ModifiedBy = "admin"
        };

        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns(department);
        _formLetterSenderRepository.GetAll().Returns([senderInUserDepartment, senderInOtherDepartment]);

        var result = (await _service.GetFormLetterSenderList()).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().Id, Is.EqualTo(senderInUserDepartment.Id));
        Assert.That(result.First().Description, Is.EqualTo("User Department Sender"));
    }

    [Test]
    public async Task GetFormLetterSenderList_WhenUserIsAdmin_ShouldReturnAllSenders()
    {
        var department1Id = Guid.NewGuid();
        var department2Id = Guid.NewGuid();

        var sender1 = new FormLetterSender
        {
            Id = Guid.NewGuid(),
            Department = new DepartmentBuilder().Build(),
            DepartmentId = department1Id,
            Description = "Sender 1",
            Surname = "Doe",
            GivenName = "John",
            SenderFunction = new FormLetterSenderFunctionBuilder().Build(),
            SenderFunctionId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Street",
            StreetFrench = "Rue",
            StreetItalian = "Via",
            StreetRomansh = "Via",
            Zip = "3011",
            CityGerman = "Bern",
            CityFrench = "Berne",
            CityItalian = "Berna",
            CityRomansh = "Berna",
            Created = DateTime.UtcNow,
            CreatedBy = "admin",
            Modified = DateTime.UtcNow,
            ModifiedBy = "admin"
        };

        var sender2 = new FormLetterSender
        {
            Id = Guid.NewGuid(),
            Department = new DepartmentBuilder().Build(),
            DepartmentId = department2Id,
            Description = "Sender 2",
            Surname = "Smith",
            GivenName = "Jane",
            SenderFunction = new FormLetterSenderFunctionBuilder().Build(),
            SenderFunctionId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Street",
            StreetFrench = "Rue",
            StreetItalian = "Via",
            StreetRomansh = "Via",
            Zip = "3012",
            CityGerman = "Zurich",
            CityFrench = "Zurich",
            CityItalian = "Zurigo",
            CityRomansh = "Turitg",
            Created = DateTime.UtcNow,
            CreatedBy = "admin",
            Modified = DateTime.UtcNow,
            ModifiedBy = "admin"
        };

        _authorizationService.IsDepartment.Returns(false);
        _formLetterSenderRepository.GetAll().Returns([sender1, sender2]);

        var result = (await _service.GetFormLetterSenderList()).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public void UpdateFormLetterSender_WhenDepartmentUserUpdatesOwnDepartment_ShouldSucceed()
    {
        var departmentId = Guid.NewGuid();
        var formLetterSenderId = Guid.NewGuid();
        var department = new DepartmentBuilder().WithId(departmentId).Build();

        var existingFormLetterSender = CreateExistingFormLetterSender(formLetterSenderId);
        existingFormLetterSender.DepartmentId = departmentId;

        var updateDto = new FormLetterSenderUpdateDto
        {
            Id = formLetterSenderId,
            Description = "Updated",
            Surname = "Updated",
            GivenName = "Updated",
            SenderFunctionId = Guid.NewGuid(),
            DepartmentId = departmentId,
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Street",
            StreetFrench = "Rue",
            StreetItalian = "Via",
            StreetRomansh = "Via",
            Zip = "3011",
            CityGerman = "Bern",
            CityFrench = "Berne",
            CityItalian = "Berna",
            CityRomansh = "Berna",
            SignatureFileName = null,
            Signature = null
        };

        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns(department);
        _formLetterSenderRepository.GetByIdForUpdate(formLetterSenderId).Returns(existingFormLetterSender);

        Assert.DoesNotThrowAsync(async () => await _service.UpdateFormLetterSender(formLetterSenderId, updateDto));
    }

    [Test]
    public void UpdateFormLetterSender_WhenDepartmentUserUpdatesDifferentDepartment_ShouldThrowAuthorizationException()
    {
        var userDepartmentId = Guid.NewGuid();
        var otherDepartmentId = Guid.NewGuid();
        var formLetterSenderId = Guid.NewGuid();
        var department = new DepartmentBuilder().WithId(userDepartmentId).Build();

        var existingFormLetterSender = CreateExistingFormLetterSender(formLetterSenderId);
        existingFormLetterSender.DepartmentId = otherDepartmentId;

        var updateDto = new FormLetterSenderUpdateDto
        {
            Id = formLetterSenderId,
            Description = "Updated",
            Surname = "Updated",
            GivenName = "Updated",
            SenderFunctionId = Guid.NewGuid(),
            DepartmentId = otherDepartmentId,
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Street",
            StreetFrench = "Rue",
            StreetItalian = "Via",
            StreetRomansh = "Via",
            Zip = "3011",
            CityGerman = "Bern",
            CityFrench = "Berne",
            CityItalian = "Berna",
            CityRomansh = "Berna",
            SignatureFileName = null,
            Signature = null
        };

        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns(department);
        _formLetterSenderRepository.GetByIdForUpdate(formLetterSenderId).Returns(existingFormLetterSender);

        var ex = Assert.ThrowsAsync<CrossCutting.Exception.AuthorizationException>(
            async () => await _service.UpdateFormLetterSender(formLetterSenderId, updateDto));

        Assert.That(ex!.Message, Does.Contain("not authorized to update"));
    }

    [Test]
    public void DeleteFormLetterSender_WhenDepartmentUserDeletesOwnDepartment_ShouldSucceed()
    {
        var departmentId = Guid.NewGuid();
        var formLetterSenderId = Guid.NewGuid();
        var department = new DepartmentBuilder().WithId(departmentId).Build();

        var existingFormLetterSender = CreateExistingFormLetterSender(formLetterSenderId);
        existingFormLetterSender.DepartmentId = departmentId;

        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns(department);
        _formLetterSenderRepository.GetByIdForUpdate(formLetterSenderId).Returns(existingFormLetterSender);

        Assert.DoesNotThrowAsync(async () => await _service.DeleteFormLetterSender(formLetterSenderId));
    }

    [Test]
    public void DeleteFormLetterSender_WhenDepartmentUserDeletesDifferentDepartment_ShouldThrowAuthorizationException()
    {
        var userDepartmentId = Guid.NewGuid();
        var otherDepartmentId = Guid.NewGuid();
        var formLetterSenderId = Guid.NewGuid();
        var department = new DepartmentBuilder().WithId(userDepartmentId).Build();

        var existingFormLetterSender = CreateExistingFormLetterSender(formLetterSenderId);
        existingFormLetterSender.DepartmentId = otherDepartmentId;

        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns(department);
        _formLetterSenderRepository.GetByIdForUpdate(formLetterSenderId).Returns(existingFormLetterSender);

        var ex = Assert.ThrowsAsync<CrossCutting.Exception.AuthorizationException>(
            async () => await _service.DeleteFormLetterSender(formLetterSenderId));

        Assert.That(ex!.Message, Does.Contain("not authorized to delete"));
    }

    private static FormLetterSenderCreateDto CreateFormLetterSenderCreateDto()
    {
        return new FormLetterSenderCreateDto
        {
            Description = "Test Description",
            Surname = "Doe",
            GivenName = "John",
            SenderFunctionId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Strasse 1",
            StreetFrench = "Rue 1",
            StreetItalian = "Via 1",
            StreetRomansh = "Via 1",
            Zip = "3011",
            CityGerman = "Bern",
            CityFrench = "Berne",
            CityItalian = "Berna",
            CityRomansh = "Berna",
            Phone = "031 123 4567",
            Email = "john.doe@example.com",
            Website = "https://example.com",
            Signature = null
        };
    }

    private static FormLetterSender CreateExistingFormLetterSender(Guid id, Guid? signatureFileId = null)
    {
        var formLetterSender = new FormLetterSender
        {
            Id = id,
            Description = "Original Description",
            Surname = "Doe",
            GivenName = "John",
            SenderFunctionId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            StreetGerman = "Strasse 1",
            StreetFrench = "Rue 1",
            StreetItalian = "Via 1",
            StreetRomansh = "Via 1",
            Zip = "3011",
            CityGerman = "Bern",
            CityFrench = "Berne",
            CityItalian = "Berna",
            CityRomansh = "Berna",
            Phone = "031 123 4567",
            Email = "john.doe@example.com",
            Website = "https://example.com",
            Created = DateTime.UtcNow.AddDays(-1),
            CreatedBy = "admin",
            Modified = DateTime.UtcNow,
            ModifiedBy = "admin",
            RowVersion = 1
        };

        if (signatureFileId.HasValue)
        {
            formLetterSender.SignatureFileReferenceId = signatureFileId.Value;
            formLetterSender.SignatureFileReference = new DocumentStorage
            {
                Id = signatureFileId.Value,
                DocumentName = "old-signature.png",
                DocumentStorageId = Guid.NewGuid().ToString(),
                Created = DateTime.UtcNow.AddDays(-1),
                CreatedBy = "admin",
                Modified = DateTime.UtcNow,
                ModifiedBy = "admin"
            };
        }

        return formLetterSender;
    }

    private static IFormFile CreateFormFile(string fileName, string contentType)
    {
        var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            writer.Write("Test file content");
            writer.Flush();
        }
        stream.Position = 0;

        var formFile = Substitute.For<IFormFile>();
        formFile.FileName.Returns(fileName);
        formFile.ContentType.Returns(contentType);
        formFile.Length.Returns(stream.Length);
        formFile.OpenReadStream().Returns(stream);

        return formFile;
    }
}
