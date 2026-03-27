using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class FormLetterSenderService : IFormLetterSenderService
{
    private readonly IFormLetterSenderRepository _formLetterSenderRepository;
    private readonly IDocumentStorageRepository _documentStorageRepository;
    private readonly IDocumentService _documentService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<FormLetterSenderService> _logger;

    public FormLetterSenderService(IFormLetterSenderRepository formLetterSenderRepository, IDocumentStorageRepository documentStorageRepository, IDocumentService documentService, IAuthorizationService authorizationService, ILogger<FormLetterSenderService> logger)
    {
        _formLetterSenderRepository = formLetterSenderRepository;
        _documentStorageRepository = documentStorageRepository;
        _documentService = documentService;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<IEnumerable<FormLetterSenderListDto>> GetFormLetterSenderList()
    {
        var formLetterSenderList = await _formLetterSenderRepository.GetAll();

        if (_authorizationService.IsDepartment)
        {
            var department = await _authorizationService.GetDepartment();
            if (department is not null)
            {
                formLetterSenderList = formLetterSenderList.Where(x => x.DepartmentId == department.Id);
            }
        }

        return formLetterSenderList.Select(FormLetterSenderMapper.ToFormLetterSenderListDto);
    }

    public async Task<FormLetterSenderCreateDto> GetEmpty()
    {
        var departmentId = _authorizationService.IsDepartment
            ? (await _authorizationService.GetDepartment())!.Id
            : Guid.Empty;

        return new FormLetterSenderCreateDto
        {
            Description = null!,
            Surname = null!,
            GivenName = null!,
            StreetGerman = null!,
            StreetFrench = null!,
            StreetItalian = null!,
            StreetRomansh = null!,
            Zip = null!,
            CityGerman = null!,
            CityFrench = null!,
            CityItalian = null!,
            CityRomansh = null!,
            DepartmentId = departmentId
        };
    }

    public async Task<FormLetterSenderUpdateDto> CreateFormLetterSender(FormLetterSenderCreateDto formLetterSenderCreateDto)
    {
        ArgumentNullException.ThrowIfNull(formLetterSenderCreateDto);

        var currentUserName = _authorizationService.GetCurrentUserName();

        var formLetterSender = FormLetterSenderMapper.FromFormLetterSenderCreateDto(formLetterSenderCreateDto, currentUserName);

        try
        {
            if (formLetterSenderCreateDto.Signature is not null)
            {
                var documentStorageId = await UploadSignature(formLetterSenderCreateDto.Signature);

                formLetterSender.SignatureFileReference = new DocumentStorage
                {
                    DocumentName = formLetterSenderCreateDto.Signature.FileName,
                    DocumentStorageId = documentStorageId,
                    Created = DateTime.UtcNow,
                    CreatedBy = currentUserName,
                    Modified = DateTime.UtcNow,
                    ModifiedBy = currentUserName
                };
            }

            var createdFormLetterSender = await _formLetterSenderRepository.Create(formLetterSender);

            _logger.LogInformation("Form letter sender {FormLetterSenderId} created", createdFormLetterSender.Id);

            return FormLetterSenderMapper.ToFormLetterSenderUpdateDto(createdFormLetterSender, _authorizationService.IsAdmin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating form letter sender. Cleaning up any created signature.");

            if (formLetterSender.SignatureFileReferenceId is not null)
            {
                await _documentService.RemoveDocument(formLetterSender.SignatureFileReferenceId.Value.ToString());
            }

            throw;
        }
    }

    public async Task<FormLetterSenderUpdateDto> GetFormLetterSenderForUpdate(Guid id)
    {
        var formLetterSender = await _formLetterSenderRepository.GetByIdForUpdate(id);
        return FormLetterSenderMapper.ToFormLetterSenderUpdateDto(formLetterSender, _authorizationService.IsAdmin);
    }

    public async Task<FormLetterSenderUpdateDto> UpdateFormLetterSender(Guid id, FormLetterSenderUpdateDto formLetterSenderUpdateDto)
    {
        ArgumentNullException.ThrowIfNull(formLetterSenderUpdateDto);

        var formLetterSender = await _formLetterSenderRepository.GetByIdForUpdate(id);

        if (_authorizationService.IsDepartment)
        {
            var department = await _authorizationService.GetDepartment();
            if (department?.Id != formLetterSender.DepartmentId)
            {
                throw new AuthorizationException("User is not authorized to update this form letter sender");
            }
        }

        var currentUserName = _authorizationService.GetCurrentUserName();

        formLetterSender.Description = formLetterSenderUpdateDto.Description;
        formLetterSender.SenderFunctionId = formLetterSenderUpdateDto.SenderFunctionId;
        formLetterSender.Surname = formLetterSenderUpdateDto.Surname;
        formLetterSender.GivenName = formLetterSenderUpdateDto.GivenName;
        formLetterSender.DepartmentId = formLetterSenderUpdateDto.DepartmentId;
        formLetterSender.OfficeId = formLetterSenderUpdateDto.OfficeId;
        formLetterSender.StreetGerman = formLetterSenderUpdateDto.StreetGerman;
        formLetterSender.StreetFrench = formLetterSenderUpdateDto.StreetFrench;
        formLetterSender.StreetItalian = formLetterSenderUpdateDto.StreetItalian;
        formLetterSender.StreetRomansh = formLetterSenderUpdateDto.StreetRomansh;
        formLetterSender.Zip = formLetterSenderUpdateDto.Zip;
        formLetterSender.CityGerman = formLetterSenderUpdateDto.CityGerman;
        formLetterSender.CityFrench = formLetterSenderUpdateDto.CityFrench;
        formLetterSender.CityItalian = formLetterSenderUpdateDto.CityItalian;
        formLetterSender.CityRomansh = formLetterSenderUpdateDto.CityRomansh;
        formLetterSender.Email = formLetterSenderUpdateDto.Email;
        formLetterSender.Phone = formLetterSenderUpdateDto.Phone;
        formLetterSender.Website = formLetterSenderUpdateDto.Website;

        formLetterSender.Modified = DateTime.UtcNow;
        formLetterSender.ModifiedBy = currentUserName;

        if (string.IsNullOrWhiteSpace(formLetterSenderUpdateDto.SignatureFileName) && formLetterSender.SignatureFileReferenceId is not null)
        {
            // Signature has been removed
            _logger.LogInformation("Removing signature file reference {SignatureFileReferenceId} for form letter sender {FormLetterSenderId}", formLetterSender.SignatureFileReferenceId, formLetterSender.Id);
            await DeleteSignature(formLetterSender.SignatureFileReferenceId.Value);
            formLetterSender.SignatureFileReference = null;
        }
        else if (formLetterSenderUpdateDto.Signature is not null)
        {
            // Signature has been updated or newly added
            // Remove old signature if it exists
            if (formLetterSender.SignatureFileReferenceId is not null)
            {
                _logger.LogInformation("Removing old signature file reference {SignatureFileReferenceId} for form letter sender {FormLetterSenderId}", formLetterSender.SignatureFileReferenceId, formLetterSender.Id);
                await DeleteSignature(formLetterSender.SignatureFileReferenceId.Value);
            }

            // Upload new signature
            var documentStorageId = await UploadSignature(formLetterSenderUpdateDto.Signature);

            formLetterSender.SignatureFileReference = new DocumentStorage
            {
                DocumentName = formLetterSenderUpdateDto.Signature.FileName,
                DocumentStorageId = documentStorageId,
                Created = DateTime.UtcNow,
                CreatedBy = currentUserName,
                Modified = DateTime.UtcNow,
                ModifiedBy = currentUserName
            };
        }

        await _formLetterSenderRepository.CommitChanges();

        _logger.LogInformation("Updated form letter sender {FormLetterSenderId}", formLetterSender.Id);

        return FormLetterSenderMapper.ToFormLetterSenderUpdateDto(formLetterSender, _authorizationService.IsAdmin);
    }

    public async Task DeleteFormLetterSender(Guid id)
    {
        var formLetterSender = await _formLetterSenderRepository.GetByIdForUpdate(id);

        if (_authorizationService.IsDepartment)
        {
            var department = await _authorizationService.GetDepartment();
            if (department?.Id != formLetterSender.DepartmentId)
            {
                throw new AuthorizationException("User is not authorized to delete this form letter sender");
            }
        }

        if (formLetterSender.SignatureFileReferenceId is not null)
        {
            _logger.LogInformation("Removing signature file reference {SignatureFileReferenceId} for form letter sender {FormLetterSenderId} before deleting form letter sender", formLetterSender.SignatureFileReferenceId, formLetterSender.Id);
            await DeleteSignature(formLetterSender.SignatureFileReferenceId.Value);
        }

        _formLetterSenderRepository.Delete(formLetterSender);

        await _formLetterSenderRepository.CommitChanges();

        _logger.LogInformation("Deleted form letter sender {FormLetterSenderId}", formLetterSender.Id);

    }

    private async Task<string> UploadSignature(IFormFile signature)
    {
        await using var fileStream = signature.OpenReadStream();
        using var stream = new MemoryStream();
        await fileStream.CopyToAsync(stream);

        return await _documentService.UploadDocument(stream.ToArray());
    }

    private async Task DeleteSignature(Guid fileId)
    {
        var documentStorageEntry = await _documentStorageRepository.GetByIdForUpdate(fileId);
        await _documentService.RemoveDocument(documentStorageEntry.DocumentStorageId);
        _documentStorageRepository.Delete(documentStorageEntry);
    }
}
