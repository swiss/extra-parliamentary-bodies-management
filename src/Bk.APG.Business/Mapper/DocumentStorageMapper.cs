using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class DocumentStorageMapper
{
    public static DocumentStorage FromModificationDto(DocumentStorageModificationDto documentStorageModificationDto, string documentStorageId, string userName)
    {
        ArgumentNullException.ThrowIfNull(documentStorageModificationDto);

        var utcNow = DateTime.UtcNow;

        return new DocumentStorage
        {
            Id = documentStorageModificationDto.Id ?? Guid.NewGuid(),
            DocumentName = documentStorageModificationDto.DisplayName,
            CreatedBy = userName,
            Created = utcNow,
            ModifiedBy = userName,
            Modified = utcNow,
            DocumentStorageId = documentStorageId,
        };
    }

    public static DocumentStorageModificationDto ToUpdateDto(DocumentStorage documentStorage, Guid languageId, bool isOriginal)
    {
        ArgumentNullException.ThrowIfNull(documentStorage);

        return new DocumentStorageModificationDto
        {
            Id = documentStorage.Id,
            DisplayName = documentStorage.DocumentName,
            LanguageId = languageId,
            IsOriginal = isOriginal
        };
    }
}
