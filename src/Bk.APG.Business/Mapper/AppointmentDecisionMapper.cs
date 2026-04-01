using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Common.Resources;

namespace Bk.APG.Business.Mapper;

public static class AppointmentDecisionMapper
{
    public static AppointmentDecisionListDto ToAppointmentDecisionListDto(AppointmentDecision appointmentDecision, string exeBrcLink)
    {
        ArgumentNullException.ThrowIfNull(appointmentDecision);

        return new AppointmentDecisionListDto
        {
            Id = appointmentDecision.Id,
            AppointmentDecisionDate = appointmentDecision.AppointmentDecisionDate,
            AppointmentDecisionType = appointmentDecision.AppointmentDecisionType?.GetText() ?? string.Empty,
            AppointmentDecisionLinkType = appointmentDecision.AppointmentDecisionLinkType?.GetText() ?? string.Empty,
            DocumentStorageId = appointmentDecision.OriginalDocument?.DocumentStorageId ?? string.Empty,
            Text = appointmentDecision.Text,
            LinkText = appointmentDecision.Link,
            Link = CreateLink(appointmentDecision, exeBrcLink),
            FileName = GetFileName(),
            Modified = appointmentDecision.Modified,
        };

        string GetFileName()
        {
            if (appointmentDecision.OriginalDocument is not null)
            {
                return appointmentDecision.OriginalDocument.DocumentName;
            }

            var fileName = appointmentDecision.FileReferenceGerman?.DocumentName ??
                           appointmentDecision.FileReferenceFrench?.DocumentName ??
                           appointmentDecision.FileReferenceItalian?.DocumentName ??
                           appointmentDecision.FileReferenceRomansh?.DocumentName;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            return appointmentDecision.FileReferenceCount > 1
                ? $"{fileName} ({BusinessTexts.AppointmentDecision_MultipleFiles})"
                : fileName;
        }
    }

    public static AppointmentDecision FromAppointmentDecisionCreateDto(AppointmentDecisionCreateDto dto, string currentUserName)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var utcNow = DateTime.UtcNow;

        return new AppointmentDecision
        {
            CommitteeId = dto.CommitteeId,
            AppointmentDecisionDate = dto.AppointmentDecisionDate,
            AppointmentDecisionTypeId = dto.AppointmentDecisionTypeId,
            AppointmentDecisionLinkTypeId = dto.AppointmentDecisionLinkTypeId,
            Text = dto.Text,
            Link = dto.Link,
            Created = utcNow,
            CreatedBy = currentUserName,
            Modified = utcNow,
            ModifiedBy = currentUserName,
        };
    }

    public static AppointmentDecisionUpdateDto ToAppointmentDecisionUpdateDto(AppointmentDecision appointmentDecision, string currentUserName)
    {
        ArgumentNullException.ThrowIfNull(appointmentDecision);

        var updateDto = new AppointmentDecisionUpdateDto
        {
            Id = appointmentDecision.Id,
            AppointmentDecisionDate = appointmentDecision.AppointmentDecisionDate,
            AppointmentDecisionTypeId = appointmentDecision.AppointmentDecisionTypeId,
            AppointmentDecisionLinkTypeId = appointmentDecision.AppointmentDecisionLinkTypeId,
            Text = appointmentDecision.Text,
            Link = appointmentDecision.Link,
            Documents = new List<DocumentStorageModificationDto>()
        };

        if (appointmentDecision.FileReferenceGerman is not null)
        {
            updateDto.Documents.Add(DocumentStorageMapper.ToUpdateDto(appointmentDecision.FileReferenceGerman, new Guid(Language.GermanId), appointmentDecision.FileReferenceGermanId == appointmentDecision.OriginalDocumentId));
        }

        if (appointmentDecision.FileReferenceFrench is not null)
        {
            updateDto.Documents.Add(DocumentStorageMapper.ToUpdateDto(appointmentDecision.FileReferenceFrench, new Guid(Language.FrenchId), appointmentDecision.FileReferenceFrenchId == appointmentDecision.OriginalDocumentId));
        }

        if (appointmentDecision.FileReferenceItalian is not null)
        {
            updateDto.Documents.Add(DocumentStorageMapper.ToUpdateDto(appointmentDecision.FileReferenceItalian, new Guid(Language.ItalianId), appointmentDecision.FileReferenceItalianId == appointmentDecision.OriginalDocumentId));
        }

        if (appointmentDecision.FileReferenceRomansh is not null)
        {
            updateDto.Documents.Add(DocumentStorageMapper.ToUpdateDto(appointmentDecision.FileReferenceRomansh, new Guid(Language.RomanshId), appointmentDecision.FileReferenceRomanshId == appointmentDecision.OriginalDocumentId));
        }
        return updateDto;
    }

    private static string CreateLink(AppointmentDecision appointmentDecision, string exeBrcLink)
    {
        if (appointmentDecision.AppointmentDecisionLinkTypeId == AppointmentDecisionLinkType.ExeBrcNumber)
        {
            return exeBrcLink + appointmentDecision.Link;
        }

        if (appointmentDecision.AppointmentDecisionLinkTypeId == AppointmentDecisionLinkType.LinkTypeStandard)
        {
            return appointmentDecision.Link ?? string.Empty;
        }

        return string.Empty;
    }
}
