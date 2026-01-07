using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Configuration;
using Bk.APG.CrossCutting.Exception;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bk.APG.Business.Services;

public class AppointmentDecisionService : IAppointmentDecisionService
{
    private readonly IAppointmentDecisionRepository _appointmentDecisionRepository;
    private readonly IDocumentService _documentService;
    private readonly ILogger<AppointmentDecisionService> _logger;
    private readonly AppointmentDecisionOptions _appointmentDecisionOptions;
    private readonly IAuthorizationService _authorizationService;
    private readonly IDocumentStorageRepository _documentStorageRepository;

    public AppointmentDecisionService(
        IAppointmentDecisionRepository appointmentDecisionRepository,
        IDocumentService documentService,
        ILogger<AppointmentDecisionService> logger,
        IOptions<AppointmentDecisionOptions> appointmentDecisionOptions,
        IAuthorizationService authorizationService,
        IDocumentStorageRepository documentStorageRepository)
    {
        _appointmentDecisionRepository = appointmentDecisionRepository;
        _documentService = documentService;
        _logger = logger;
        _appointmentDecisionOptions = appointmentDecisionOptions.Value;
        _authorizationService = authorizationService;
        _documentStorageRepository = documentStorageRepository;
    }

    public async Task<AppointmentDecisionListDto> GetById(Guid appointmentDecisionId)
    {
        var appointmentDecision = await _appointmentDecisionRepository.GetAppointmentDecisionById(appointmentDecisionId);

        return AppointmentDecisionMapper.ToAppointmentDecisionListDto(appointmentDecision, _appointmentDecisionOptions.ExebrcLink);
    }

    public async Task<AppointmentDecisionUpdateDto> GetByIdForUpdate(Guid appointmentDecisionId)
    {
        var appointmentDecision = await _appointmentDecisionRepository.GetAppointmentDecisionByIdForUpdate(appointmentDecisionId);
        var currentUserName = _authorizationService.GetCurrentUserName();
        return AppointmentDecisionMapper.ToAppointmentDecisionUpdateDto(appointmentDecision, currentUserName);
    }

    public async Task<IEnumerable<AppointmentDecisionListDto>> GetAppointmentDecisionListByCommitteeId(Guid committeeId)
    {
        var appointmentDecisions = await _appointmentDecisionRepository.GetAppointmentDecisionsByCommitteeId(committeeId);

        var exeBrcLink = _appointmentDecisionOptions.ExebrcLink;

        var appointmentDecisionList = appointmentDecisions.Select(ad => AppointmentDecisionMapper.ToAppointmentDecisionListDto(ad, exeBrcLink)).ToList().OrderByDescending(ad => ad.AppointmentDecisionDate);

        return appointmentDecisionList;
    }

    public async Task<AppointmentDecisionListDto> CreateAppointmentDecision(AppointmentDecisionCreateDto createDto)
    {
        CheckAuthorization();

        ValidateAppointmentDecisionModificationDto(createDto);
        var userName = _authorizationService.GetCurrentUserName();
        var uploadedDocumentIds = new List<string>();

        var appointmentDecision = AppointmentDecisionMapper.FromAppointmentDecisionCreateDto(createDto, userName);

        try
        {
            if (createDto.Documents is not null)
            {
                foreach (var documentDto in createDto.Documents)
                {
                    await using var fileStream = documentDto!.File!.OpenReadStream();
                    using var stream = new MemoryStream();

                    await fileStream.CopyToAsync(stream);

                    var documentId = await _documentService.UploadDocument(stream.ToArray());

                    uploadedDocumentIds.Add(documentId);

                    var utcNow = DateTime.UtcNow;

                    var documentStorage = DocumentStorageMapper.FromModificationDto(documentDto, documentId, userName);

                    SetDocumentReferenceWithLanguage(documentDto.LanguageId, documentDto.IsOriginal, appointmentDecision, documentStorage);
                }
            }

            await _appointmentDecisionRepository.Create(appointmentDecision);

            _logger.LogInformation("Appointment decision {AppointmentDecisionId} created", appointmentDecision.Id);
        }
        catch (Exception ex)
        {
            foreach (var guid in uploadedDocumentIds)
            {
                await _documentService.RemoveDocument(guid);
            }

            _logger.LogError(ex, "Error while creating appointment decision");

            throw new AppointmentDecisionCreateException($"Error while creating appointment decision, removed uploaded documentDto items with storage Id: {string.Join(',', uploadedDocumentIds.Select(y => y.ToString()))}");
        }

        return await GetById(appointmentDecision.Id);
    }

    public async Task<AppointmentDecisionListDto> UpdateAppointmentDecision(Guid id, AppointmentDecisionUpdateDto updateDto)
    {
        _logger.LogInformation("Update appointment decision {AppointmentDecisionId}", id);

        CheckAuthorization();

        ValidateAppointmentDecisionModificationDto(updateDto);

        var appointmentDecision = await _appointmentDecisionRepository.GetAppointmentDecisionByIdForUpdate(id);
        var currentUserName = _authorizationService.GetCurrentUserName();

        var uploadedDocumentIds = new List<string>();

        // update basic fields
        UpdateBasicData(appointmentDecision, updateDto);

        try
        {
            var documentIdsToRemove = EmptyFileReferences(appointmentDecision);

            if (updateDto.Documents is not null)
            {
                foreach (var documentDto in updateDto.Documents)
                {
                    DocumentStorage? documentStorage = null;

                    if (documentDto.Id is null)
                    {
                        // newly added document
                        await using var fileStream = documentDto!.File!.OpenReadStream();
                        using var stream = new MemoryStream();

                        await fileStream.CopyToAsync(stream);

                        var documentId = await _documentService.UploadDocument(stream.ToArray());

                        uploadedDocumentIds.Add(documentId);

                        documentStorage = DocumentStorageMapper.FromModificationDto(documentDto, documentId, currentUserName);
                        await _documentStorageRepository.Create(documentStorage);
                    }
                    else
                    {
                        // document update
                        documentStorage = await _documentStorageRepository.GetByIdForUpdate(documentDto.Id.Value);
                        documentStorage.DocumentName = documentDto.DisplayName;
                        documentIdsToRemove.Remove(documentStorage.Id);
                    }

                    SetDocumentReferenceWithLanguage(documentDto.LanguageId, documentDto.IsOriginal, appointmentDecision, documentStorage);
                }
            }

            // remove currently unused documents

            foreach (var documentId in documentIdsToRemove)
            {
                _logger.LogInformation("Removing document storage entry {DocumentId}' ...", documentId);

                await DeleteDocumentStorageEntry(documentId);

                _logger.LogInformation("Document storage entry (incl. file) {DocumentId}' removed", documentId);
            }

            await _appointmentDecisionRepository.CommitChanges();

            _logger.LogInformation("Appointment decision {AppointmentDecisionId} updated", appointmentDecision.Id);
        }
        catch (Exception ex)
        {
            foreach (var guid in uploadedDocumentIds)
            {
                await _documentService.RemoveDocument(guid);
            }

            _logger.LogError(ex, "Error while updating appointment decision");

            throw new AppointmentDecisionUpdateException($"Error while updating appointment decision, removed uploaded documentDto items with storage Id: {string.Join(',', uploadedDocumentIds.Select(y => y.ToString()))}");
        }

        return await GetById(appointmentDecision.Id);
    }

    public async Task DeleteAppointmentDecision(Guid id)
    {
        _logger.LogInformation("Delete appointment decision {AppointmentDecisionId}", id);

        CheckAuthorization();

        var appointmentDecision = await _appointmentDecisionRepository.GetAppointmentDecisionByIdForUpdate(id);

        try
        {
            if (appointmentDecision.FileReferenceGerman is not null)
            {
                _logger.LogInformation("Removing document storage entry DE {FileReferenceId}...", appointmentDecision.FileReferenceGermanId);

                await DeleteDocumentStorageEntry(appointmentDecision.FileReferenceGerman.Id);
            }

            if (appointmentDecision.FileReferenceFrench is not null)
            {
                _logger.LogInformation("Removing document storage entry FR {FileReferenceId}...", appointmentDecision.FileReferenceFrenchId);

                await DeleteDocumentStorageEntry(appointmentDecision.FileReferenceFrench.Id);
            }

            if (appointmentDecision.FileReferenceItalian is not null)
            {
                _logger.LogInformation("Removing document storage entry IT {FileReferenceId}...", appointmentDecision.FileReferenceItalianId);

                await DeleteDocumentStorageEntry(appointmentDecision.FileReferenceItalian.Id);
            }

            if (appointmentDecision.FileReferenceRomansh is not null)
            {
                _logger.LogInformation("Removing document storage entry RM {FileReferenceId}...", appointmentDecision.FileReferenceRomanshId);

                await DeleteDocumentStorageEntry(appointmentDecision.FileReferenceRomansh.Id);
            }

            _appointmentDecisionRepository.Delete(appointmentDecision);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting appointment decision");
        }

        await _appointmentDecisionRepository.CommitChanges();
    }

    public AppointmentDecisionCreateDto GetEmpty()
    {
        return new AppointmentDecisionCreateDto
        {
            AppointmentDecisionDate = DateOnly.FromDateTime(DateTime.Now)
        };
    }

    private static void UpdateBasicData(AppointmentDecision appointmentDecision, AppointmentDecisionUpdateDto updateDto)
    {
        appointmentDecision.AppointmentDecisionTypeId = updateDto.AppointmentDecisionTypeId;
        appointmentDecision.AppointmentDecisionLinkTypeId = updateDto.AppointmentDecisionLinkTypeId;
        appointmentDecision.Text = updateDto.Text;
        appointmentDecision.Link = updateDto.Link;
        appointmentDecision.AppointmentDecisionDate = updateDto.AppointmentDecisionDate;
    }

    private static List<Guid> EmptyFileReferences(AppointmentDecision appointmentDecision)
    {
        var documentIdsToRemove = new List<Guid>();
        if (appointmentDecision.FileReferenceGerman is not null)
        {
            documentIdsToRemove.Add(appointmentDecision.FileReferenceGerman.Id);
        }

        if (appointmentDecision.FileReferenceFrench is not null)
        {
            documentIdsToRemove.Add(appointmentDecision.FileReferenceFrench.Id);
        }

        if (appointmentDecision.FileReferenceItalian is not null)
        {
            documentIdsToRemove.Add(appointmentDecision.FileReferenceItalian.Id);
        }

        if (appointmentDecision.FileReferenceRomansh is not null)
        {
            documentIdsToRemove.Add(appointmentDecision.FileReferenceRomansh.Id);
        }

        appointmentDecision.FileReferenceGerman = null;
        appointmentDecision.FileReferenceFrench = null;
        appointmentDecision.FileReferenceItalian = null;
        appointmentDecision.FileReferenceRomansh = null;
        appointmentDecision.OriginalDocument = null;

        return documentIdsToRemove;
    }

    private static void SetDocumentReferenceWithLanguage(Guid languageId, bool isOriginal, AppointmentDecision appointmentDecision, DocumentStorage documentStorage)
    {
        switch (languageId.ToString().ToLower())
        {
            case Language.GermanId:
                appointmentDecision.FileReferenceGerman = documentStorage;
                break;
            case Language.FrenchId:
                appointmentDecision.FileReferenceFrench = documentStorage;
                break;
            case Language.ItalianId:
                appointmentDecision.FileReferenceItalian = documentStorage;
                break;
            case Language.RomanshId:
                appointmentDecision.FileReferenceRomansh = documentStorage;
                break;
            default:
                break;
        }

        if (isOriginal)
        {
            appointmentDecision.OriginalDocument = documentStorage;
        }
    }

    private static void ValidateAppointmentDecisionModificationDto(AppointmentDecisionModificationDto appointmentDecisionModificationDto)
    {
        if (appointmentDecisionModificationDto.AppointmentDecisionTypeId == AppointmentDecisionType.DecisionFederalCouncil && string.IsNullOrWhiteSpace(appointmentDecisionModificationDto.Text))
        {
            throw new BusinessValidationException("Text is mandatory for decision type 'BR-Beschluss'");
        }

        if (appointmentDecisionModificationDto.Documents is not null && appointmentDecisionModificationDto.AppointmentDecisionTypeId == AppointmentDecisionType.Institution)
        {
            if (!appointmentDecisionModificationDto.Documents.Any(y => y.IsOriginal))
            {
                throw new BusinessValidationException("Original documentDto missing");
            }

            if (appointmentDecisionModificationDto.Documents.Count(y => y.IsOriginal) > 1)
            {
                throw new BusinessValidationException("Multiple original documents");
            }

            if (appointmentDecisionModificationDto.Documents.GroupBy(y => y.LanguageId).Any(y => y.Count() > 1))
            {
                throw new BusinessValidationException("Multiple documents with the same language");
            }
        }

        if (appointmentDecisionModificationDto.Documents is not null)
        {
            if (appointmentDecisionModificationDto.Documents.Any(d => d.Id is null && d.File is null))
            {
                throw new BusinessValidationException("Missing File stream for a new documentDto");
            }
        }
    }

    private async Task DeleteDocumentStorageEntry(Guid documentStorageEntryId)
    {
        var documentStorageEntry = await _documentStorageRepository.GetByIdForUpdate(documentStorageEntryId);

        await _documentService.RemoveDocument(documentStorageEntry.DocumentStorageId);

        _documentStorageRepository.Delete(documentStorageEntry);
    }

    private void CheckAuthorization()
    {
        if (_authorizationService is { IsAdmin: false, IsDepartment: false, IsOffice: false, IsSecretariat: false })
        {
            throw new AuthorizationException("User is not allowed to create / edit / delete appointment decisions");
        }
    }
}
