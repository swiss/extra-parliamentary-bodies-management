using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Configuration;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class AppointmentDecisionServiceTests
{
    private readonly IAppointmentDecisionRepository _appointmentDecisionRepository = Substitute.For<IAppointmentDecisionRepository>();
    private readonly IDocumentService _documentService = Substitute.For<IDocumentService>();
    private readonly ILogger<AppointmentDecisionService> _logger = NullLogger<AppointmentDecisionService>.Instance;
    private readonly IOptions<AppointmentDecisionOptions> _appointmentDecisionOptions = Substitute.For<IOptions<AppointmentDecisionOptions>>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IDocumentStorageRepository _documentStorageRepository = Substitute.For<IDocumentStorageRepository>();

    private readonly AppointmentDecisionOptions _appointmentDecisionOptionValues = new()
    {
        ExebrcLink = "https://intranet.exebrc-r.bk.admin.ch/affair/",
    };

    private AppointmentDecisionService _service = null!;
    private readonly Guid _committeeId = Guid.Parse("9df96395-bd65-4235-a4fa-fb87689df11f");

    [SetUp]
    public void SetUp()
    {
        _appointmentDecisionOptions.Value.Returns(_appointmentDecisionOptionValues);

        _service = new AppointmentDecisionService(_appointmentDecisionRepository, _documentService, _logger, _appointmentDecisionOptions, _authorizationService, _documentStorageRepository);
        _authorizationService.IsAdmin.Returns(true);

        var appointmentDecision1 = new AppointmentDecisionBuilder()
            .WithId(Guid.Parse("8697753c-f57a-4805-b1e9-bb1e546f4101"))
            .WithCommitteeId(_committeeId)
            .WithCommittee(new CommitteeBuilder().WithOldId(111000).Build())
            .Build();

        var appointmentDecision2 = new AppointmentDecisionBuilder()
            .WithId(Guid.Parse("9C6F1A13-4535-404C-ABDD-EEB854B5494A"))
            .WithCommitteeId(_committeeId)
            .WithCommittee(new CommitteeBuilder().WithOldId(111000).Build())
            .Build();

        var appointmentDecisionList = new List<AppointmentDecision> { appointmentDecision1, appointmentDecision2 };

        _appointmentDecisionRepository.GetAppointmentDecisionsByCommitteeId(_committeeId).Returns(appointmentDecisionList);
        _appointmentDecisionRepository.GetAppointmentDecisionById(appointmentDecision1.Id).Returns(appointmentDecision1);
    }

    [TearDown]
    public void TearDown()
    {
        _appointmentDecisionRepository.ClearSubstitute();
        _documentStorageRepository.ClearSubstitute();
        _documentService.ClearSubstitute();
        _authorizationService.ClearSubstitute();
    }

    [Test]
    public async Task GetById_WhenCalled_ShouldCallRepository()
    {
        var appointmentDecision = await _service.GetById(new Guid("8697753c-f57a-4805-b1e9-bb1e546f4101"));

        await _appointmentDecisionRepository.Received(1).GetAppointmentDecisionById(Arg.Is(appointmentDecision.Id));

        Assert.That(appointmentDecision, Is.Not.Null);
        Assert.That(appointmentDecision.Id, Is.EqualTo(new Guid("8697753c-f57a-4805-b1e9-bb1e546f4101")));
    }

    [Test]
    public async Task GetAppointmentDecisionListByCommitteeId_WhenCalled_ShouldCallRepository()
    {
        var appointmentDecisions = (await _service.GetAppointmentDecisionListByCommitteeId(_committeeId)).ToArray();

        await _appointmentDecisionRepository.Received(1).GetAppointmentDecisionsByCommitteeId(Arg.Is(_committeeId));

        Assert.That(appointmentDecisions, Is.Not.Null);
        Assert.That(appointmentDecisions, Has.Length.EqualTo(2));
    }

    [Test]
    public void CreateAppointmentDecision_WithoutPermission_ShouldThrowAuthorizationException()
    {
        var stream = new MemoryStream();
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", "fileName");
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(false);
        _authorizationService.IsSecretariat.Returns(false);

        var createDto = new AppointmentDecisionCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            AppointmentDecisionTypeId = AppointmentDecisionType.Institution,
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents = [new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName", IsOriginal = false, File = file, LanguageId = new Guid(Language.GermanId) }]
        };

        Assert.That(async () => await _service.CreateAppointmentDecision(createDto), Throws.Exception.InstanceOf<AuthorizationException>());
    }

    [Test]
    public void CreateAppointmentDecision_ForInstitutionWithoutOriginalDocument_ShouldThrowBusinessValidationException()
    {
        using var stream = new MemoryStream();
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", "fileName");

        var createDto = new AppointmentDecisionCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            AppointmentDecisionTypeId = AppointmentDecisionType.Institution,
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents = [new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName", IsOriginal = false, File = file, LanguageId = new Guid(Language.GermanId) }]
        };

        Assert.That(async () => await _service.CreateAppointmentDecision(createDto), Throws.Exception.InstanceOf<BusinessValidationException>());
    }

    [Test]
    public void CreateAppointmentDecision_ForInstitutionWithMoreOriginalDocument_ShouldThrowBusinessValidationException()
    {
        var createDto = new AppointmentDecisionCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            AppointmentDecisionTypeId = AppointmentDecisionType.Institution,
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents =
            [
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName1", IsOriginal = true, File = null!, LanguageId = new Guid(Language.GermanId) },
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName2", IsOriginal = true, File = null!, LanguageId = new Guid(Language.FrenchId) }
            ]
        };

        Assert.That(async () => await _service.CreateAppointmentDecision(createDto), Throws.Exception.InstanceOf<BusinessValidationException>());
    }

    [Test]
    public void CreateAppointmentDecision_ForInstitutionWithDuplicateLanguage_ShouldThrowBusinessValidationException()
    {
        var createDto = new AppointmentDecisionCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            AppointmentDecisionTypeId = AppointmentDecisionType.Institution,
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents =
            [
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName1", IsOriginal = true, File = null!, LanguageId = new Guid(Language.GermanId) },
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName2", IsOriginal = true, File = null!, LanguageId = new Guid(Language.FrenchId) }
            ]
        };

        Assert.That(async () => await _service.CreateAppointmentDecision(createDto), Throws.Exception.InstanceOf<BusinessValidationException>());
    }

    [Test]
    public void CreateAppointmentDecision_WithEmptyTextForDecision_ShouldThrowBusinessValidationException()
    {
        var createDto = new AppointmentDecisionCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            AppointmentDecisionTypeId = AppointmentDecisionType.DecisionFederalCouncil,
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "",
            Link = "Link",
            Documents =
            [
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName1", IsOriginal = true, File = null!, LanguageId = new Guid(Language.GermanId) },
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName2", IsOriginal = true, File = null!, LanguageId = new Guid(Language.FrenchId) }
            ]
        };

        Assert.That(async () => await _service.CreateAppointmentDecision(createDto), Throws.Exception.InstanceOf<BusinessValidationException>());
    }

    [Test]
    public async Task CreateAppointmentDecision_WithErrorDuringUpload_ShouldRemoveUploadedFiles()
    {
        var stream = new MemoryStream("Hello world!"u8.ToArray());
        IFormFile file1 = new FormFile(stream, 0, stream.Length, "id_from_form1", "fileName1");

        _documentService.UploadDocument(Arg.Any<byte[]>()).Returns(_ => "id", x => throw new TimeoutException());

        var createDto = new AppointmentDecisionCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            AppointmentDecisionTypeId = Guid.NewGuid(),
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents =
            [
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName1", IsOriginal = true, File = file1, LanguageId = new Guid(Language.GermanId) },
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName2", IsOriginal = false, File = file1, LanguageId = new Guid(Language.FrenchId) }
            ]
        };

        _appointmentDecisionRepository.GetAppointmentDecisionById(Arg.Any<Guid>()).Returns(new AppointmentDecisionBuilder().Build());

        Assert.That(async () => await _service.CreateAppointmentDecision(createDto), Throws.Exception.InstanceOf<AppointmentDecisionCreateException>());

        await _documentService.Received(1).RemoveDocument(Arg.Any<string>());
    }

    [TestCase(true, false, false, false)]
    [TestCase(false, true, false, false)]
    [TestCase(false, false, true, false)]
    [TestCase(false, false, false, true)]
    public async Task CreateAppointmentDecision_ShouldCallRepository(bool isAdmin, bool isDepartment, bool isOffice, bool isSecretariat)
    {
        var stream = new MemoryStream("Hello world!"u8.ToArray());
        IFormFile file1 = new FormFile(stream, 0, stream.Length, "id_from_form1", "fileName1");
        _authorizationService.IsAdmin.Returns(isAdmin);
        _authorizationService.IsDepartment.Returns(isDepartment);
        _authorizationService.IsOffice.Returns(isOffice);
        _authorizationService.IsSecretariat.Returns(isSecretariat);

        var appointmentDecision = new AppointmentDecisionCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            AppointmentDecisionTypeId = Guid.NewGuid(),
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents = [new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName1", IsOriginal = true, File = file1, LanguageId = new Guid(Language.GermanId) }]
        };

        _appointmentDecisionRepository.GetAppointmentDecisionById(Arg.Any<Guid>()).Returns(new AppointmentDecisionBuilder().Build());

        await _service.CreateAppointmentDecision(appointmentDecision);

        _authorizationService.Received(1).GetCurrentUserName();

        await _appointmentDecisionRepository.Received(1).Create(Arg.Is<AppointmentDecision>(x =>
            x.CommitteeId == appointmentDecision.CommitteeId &&
            x.FileReferenceGerman != null &&
            x.AppointmentDecisionDate == appointmentDecision.AppointmentDecisionDate &&
            x.AppointmentDecisionTypeId == appointmentDecision.AppointmentDecisionTypeId &&
            x.AppointmentDecisionLinkTypeId == appointmentDecision.AppointmentDecisionLinkTypeId &&
            x.Text == appointmentDecision.Text &&
            x.Link == appointmentDecision.Link));
    }

    [Test]
    public void UpdateAppointmentDecision_WithoutPermission_ShouldThrowAuthorizationException()
    {
        var id = Guid.NewGuid();
        using var stream = new MemoryStream();
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", "fileName");
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(false);
        _authorizationService.IsSecretariat.Returns(false);

        var updateDto = new AppointmentDecisionUpdateDto
        {
            Id = id,
            AppointmentDecisionTypeId = AppointmentDecisionType.Institution,
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents = [new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName", IsOriginal = false, File = file, LanguageId = new Guid(Language.GermanId) }]
        };

        Assert.That(async () => await _service.UpdateAppointmentDecision(id, updateDto), Throws.Exception.InstanceOf<AuthorizationException>());
    }

    [Test]
    public void UpdateAppointmentDecision_ForInstitutionWithoutOriginalDocument_ShouldThrowBusinessValidationException()
    {
        var id = Guid.NewGuid();
        using var stream = new MemoryStream();
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", "fileName");

        var updateDto = new AppointmentDecisionUpdateDto
        {
            Id = id,
            AppointmentDecisionTypeId = AppointmentDecisionType.Institution,
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents = [new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName", IsOriginal = false, File = file, LanguageId = new Guid(Language.GermanId) }]
        };

        Assert.That(async () => await _service.UpdateAppointmentDecision(id, updateDto), Throws.Exception.InstanceOf<BusinessValidationException>());
    }

    [Test]
    public void UpdateAppointmentDecision_ForInstitutionWithMoreOriginalDocument_ShouldThrowBusinessValidationException()
    {
        var id = Guid.NewGuid();
        var updateDto = new AppointmentDecisionUpdateDto
        {
            Id = id,
            AppointmentDecisionTypeId = AppointmentDecisionType.Institution,
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents =
            [
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName1", IsOriginal = true, File = null!, LanguageId = new Guid(Language.GermanId) },
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName2", IsOriginal = true, File = null!, LanguageId = new Guid(Language.FrenchId) }
            ]
        };

        Assert.That(async () => await _service.UpdateAppointmentDecision(id, updateDto), Throws.Exception.InstanceOf<BusinessValidationException>());
    }

    [Test]
    public void UpdateAppointmentDecision_ForInstitutionWithDuplicateLanguage_ShouldThrowBusinessValidationException()
    {
        var id = Guid.NewGuid();

        var updateDto = new AppointmentDecisionUpdateDto
        {
            Id = id,
            AppointmentDecisionTypeId = AppointmentDecisionType.Institution,
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents =
            [
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName1", IsOriginal = true, File = null!, LanguageId = new Guid(Language.GermanId) },
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName2", IsOriginal = true, File = null!, LanguageId = new Guid(Language.FrenchId) }
            ]
        };

        Assert.That(async () => await _service.UpdateAppointmentDecision(id, updateDto), Throws.Exception.InstanceOf<BusinessValidationException>());
    }

    [Test]
    public void UpdateAppointmentDecision_WithEmptyTextForDecision_ShouldThrowBusinessValidationException()
    {
        var id = Guid.NewGuid();
        var createDto = new AppointmentDecisionUpdateDto
        {
            Id = id,
            AppointmentDecisionTypeId = AppointmentDecisionType.DecisionFederalCouncil,
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "",
            Link = "Link",
            Documents =
            [
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName1", IsOriginal = true, File = null!, LanguageId = new Guid(Language.GermanId) },
                new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName2", IsOriginal = true, File = null!, LanguageId = new Guid(Language.FrenchId) }
            ]
        };

        Assert.That(async () => await _service.UpdateAppointmentDecision(id, createDto), Throws.Exception.InstanceOf<BusinessValidationException>());
    }

    [Test]
    public async Task UpdateAppointmentDecision_WithErrorDuringUpload_ShouldRemoveUploadedFiles()
    {
        var id = Guid.NewGuid();
        var stream = new MemoryStream("Hello world!"u8.ToArray());
        IFormFile file1 = new FormFile(stream, 0, stream.Length, "id_from_form1", "fileName1");

        _documentService.UploadDocument(Arg.Any<byte[]>()).Returns(_ => "id", x => throw new TimeoutException());

        var createDto = new AppointmentDecisionUpdateDto
        {
            Id = id,
            AppointmentDecisionTypeId = Guid.NewGuid(),
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents =
            [
                new DocumentStorageModificationDto { Id = null, DisplayName = "displayName1", IsOriginal = true, File = file1, LanguageId = new Guid(Language.GermanId) },
                new DocumentStorageModificationDto { Id = null, DisplayName = "displayName2", IsOriginal = true, File = file1, LanguageId = new Guid(Language.FrenchId) }
            ]
        };
        _documentStorageRepository.GetByIdForUpdate(Arg.Any<Guid>()).Returns(new DocumentStorageBuilder().Build());
        _appointmentDecisionRepository.GetAppointmentDecisionById(Arg.Any<Guid>()).Returns(new AppointmentDecisionBuilder().Build());
        _appointmentDecisionRepository.GetAppointmentDecisionByIdForUpdate(Arg.Any<Guid>()).Returns(new AppointmentDecisionBuilder().Build());

        Assert.That(async () => await _service.UpdateAppointmentDecision(id, createDto), Throws.Exception.InstanceOf<AppointmentDecisionUpdateException>());

        await _documentService.Received(1).RemoveDocument(Arg.Any<string>());
    }

    [TestCase(true, false, false, false)]
    [TestCase(false, true, false, false)]
    [TestCase(false, false, true, false)]
    [TestCase(false, false, false, true)]
    public async Task UpdateAppointmentDecision_ShouldCallRepository(bool isAdmin, bool isDepartment, bool isOffice, bool isSecretariat)
    {
        var id = Guid.NewGuid();
        var stream = new MemoryStream("Hello world!"u8.ToArray());
        IFormFile file1 = new FormFile(stream, 0, stream.Length, "id_from_form1", "fileName1");
        _authorizationService.IsAdmin.Returns(isAdmin);
        _authorizationService.IsDepartment.Returns(isDepartment);
        _authorizationService.IsOffice.Returns(isOffice);
        _authorizationService.IsSecretariat.Returns(isSecretariat);

        var appointmentDecisionToUpdate = new AppointmentDecisionBuilder().Build();

        var appointmentDecisionDto = new AppointmentDecisionUpdateDto
        {
            Id = id,
            AppointmentDecisionTypeId = Guid.NewGuid(),
            AppointmentDecisionLinkTypeId = Guid.NewGuid(),
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Text = "Text",
            Link = "Link",
            Documents = [new DocumentStorageModificationDto { Id = Guid.NewGuid(), DisplayName = "displayName1", IsOriginal = true, File = file1, LanguageId = new Guid(Language.GermanId) }]
        };
        _documentStorageRepository.GetByIdForUpdate(Arg.Any<Guid>()).Returns(new DocumentStorageBuilder().Build());
        _appointmentDecisionRepository.GetAppointmentDecisionById(Arg.Any<Guid>()).Returns(new AppointmentDecisionBuilder().Build());
        _appointmentDecisionRepository.GetAppointmentDecisionByIdForUpdate(Arg.Any<Guid>()).Returns(appointmentDecisionToUpdate);

        _ = await _service.UpdateAppointmentDecision(id, appointmentDecisionDto);

        Assert.Multiple(() =>
        {
            Assert.That(appointmentDecisionToUpdate.AppointmentDecisionDate, Is.EqualTo(appointmentDecisionDto.AppointmentDecisionDate));
            Assert.That(appointmentDecisionToUpdate.AppointmentDecisionTypeId, Is.EqualTo(appointmentDecisionDto.AppointmentDecisionTypeId));
            Assert.That(appointmentDecisionToUpdate.AppointmentDecisionLinkTypeId, Is.EqualTo(appointmentDecisionDto.AppointmentDecisionLinkTypeId));
            Assert.That(appointmentDecisionToUpdate.Text, Is.EqualTo(appointmentDecisionDto.Text));
            Assert.That(appointmentDecisionToUpdate.Link, Is.EqualTo(appointmentDecisionDto.Link));
            Assert.That(appointmentDecisionToUpdate.FileReferenceGerman, Is.Not.Null);
            Assert.That(appointmentDecisionToUpdate.FileReferenceGerman!.DocumentName, Is.EqualTo("displayName1"));
        });
    }

    [Test]
    public void DeleteAppointmentDecision_WithoutPermission_ShouldThrowAuthorizationException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(false);
        _authorizationService.IsSecretariat.Returns(false);
        var id = Guid.NewGuid();

        var appointmentDecisionToUpdate = new AppointmentDecisionBuilder().WithId(id).Build();

        appointmentDecisionToUpdate.FileReferenceGerman = new DocumentStorageBuilder().Build();
        appointmentDecisionToUpdate.FileReferenceFrench = new DocumentStorageBuilder().Build();
        appointmentDecisionToUpdate.FileReferenceItalian = new DocumentStorageBuilder().Build();
        appointmentDecisionToUpdate.FileReferenceRomansh = new DocumentStorageBuilder().Build();

        _documentStorageRepository.GetByIdForUpdate(Arg.Any<Guid>()).Returns(new DocumentStorageBuilder().Build());
        _appointmentDecisionRepository.GetAppointmentDecisionByIdForUpdate(Arg.Any<Guid>()).Returns(appointmentDecisionToUpdate);

        Assert.That(async () => await _service.DeleteAppointmentDecision(id), Throws.Exception.InstanceOf<AuthorizationException>());
    }

    [TestCase(true, false, false, false)]
    [TestCase(false, true, false, false)]
    [TestCase(false, false, true, false)]
    [TestCase(false, false, false, true)]
    public async Task DeleteAppointmentDecision_ShouldCallRepository(bool isAdmin, bool isDepartment, bool isOffice, bool isSecretariat)
    {
        _authorizationService.IsAdmin.Returns(isAdmin);
        _authorizationService.IsDepartment.Returns(isDepartment);
        _authorizationService.IsOffice.Returns(isOffice);
        _authorizationService.IsSecretariat.Returns(isSecretariat);

        var id = Guid.NewGuid();

        var appointmentDecisionToUpdate = new AppointmentDecisionBuilder().WithId(id).Build();

        appointmentDecisionToUpdate.FileReferenceGerman = new DocumentStorageBuilder().Build();
        appointmentDecisionToUpdate.FileReferenceFrench = new DocumentStorageBuilder().Build();
        appointmentDecisionToUpdate.FileReferenceItalian = new DocumentStorageBuilder().Build();
        appointmentDecisionToUpdate.FileReferenceRomansh = new DocumentStorageBuilder().Build();

        _documentStorageRepository.GetByIdForUpdate(Arg.Any<Guid>()).Returns(new DocumentStorageBuilder().Build());
        _appointmentDecisionRepository.GetAppointmentDecisionByIdForUpdate(Arg.Any<Guid>()).Returns(appointmentDecisionToUpdate);

        await _service.DeleteAppointmentDecision(id);

        await _documentService.Received(4).RemoveDocument(Arg.Any<string>());
        _documentStorageRepository.Received(4).Delete(Arg.Any<DocumentStorage>());
        _appointmentDecisionRepository.Received(1).Delete(Arg.Is(appointmentDecisionToUpdate));
    }

    [Test]
    public void GetEmpty_ShouldCreateEmptyObject()
    {
        var emptyDto = _service.GetEmpty();

        Assert.That(emptyDto.AppointmentDecisionDate, Is.EqualTo(DateOnly.FromDateTime(DateTime.UtcNow)));
    }
}
