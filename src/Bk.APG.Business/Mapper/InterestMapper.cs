using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.RawData.Model;

namespace Bk.APG.Business.Mapper;

public static class InterestMapper
{
    public static InterestUpdateDto ToInterestUpdateDto(Interest interest)
    {
        return new InterestUpdateDto
        {
            Id = interest.Id,
            PersonId = interest.PersonId,
            Text = interest.Text,
            InterestText = interest.InterestText!,
            InterestCommitteeId = interest.InterestCommitteeId,
            InterestFunctionId = interest.InterestFunctionId,
            InterestLegalFormId = interest.InterestLegalFormId,
            LegalFormId = interest.LegalFormId,
            BeginDate = interest.BeginDate,
            EndDate = interest.EndDate,
            UidOrganisationId = interest.UidOrganisationId,
            RowVersion = interest.RowVersion,
            IsInactive = interest.EndDate < DateOnly.FromDateTime(DateTime.Now)
        };
    }

    public static Interest FromInterestUpdateDto(InterestUpdateDto interestUpdateDto)
    {
        var interest = new Interest
        {
            Id = interestUpdateDto.Id ?? Guid.NewGuid(),
            PersonId = interestUpdateDto.PersonId,
            Text = interestUpdateDto.Text,
            InterestText = interestUpdateDto.InterestText,
            InterestCommitteeId = interestUpdateDto.InterestCommitteeId,
            InterestFunctionId = interestUpdateDto.InterestFunctionId,
            InterestLegalFormId = interestUpdateDto.InterestLegalFormId,
            LegalFormId = interestUpdateDto.LegalFormId,
            BeginDate = interestUpdateDto.BeginDate,
            EndDate = interestUpdateDto.EndDate,
            UidOrganisationId = interestUpdateDto.UidOrganisationId,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            ModifiedBy = "SYSTEM",
            RowVersion = interestUpdateDto.RowVersion
        };

        return interest;
    }

    public static ObservationDataRow ToObservation(Interest interest)
    {
        ArgumentNullException.ThrowIfNull(interest, nameof(interest));
        ArgumentNullException.ThrowIfNull(interest.Person, nameof(interest.Person));

        const string ldUri = "https://ld.admin.ch/";

        var dataRow = new ObservationDataRow
        {
            KeyUri = $"vested-interest:{interest.OgdId}",
            ValidFrom = interest.BeginDate?.ToDateTime(TimeOnly.MinValue),
            ValidTo = interest.EndDate?.ToDateTime(TimeOnly.MinValue),
        };

        dataRow.KeyDimensionLinks.Add(new KeyDimensionLink { Predicate = "vested-interest:hasPerson", Uri = $"person:{interest.Person.OgdId}" });

        if (interest.InterestFunction is not null)
        {
            dataRow.KeyDimensionLinks.Add(new KeyDimensionLink { Predicate = "vested-interest:hasFunction", Uri = $"interest-function:{interest.InterestFunction.OgdId}" });
        }

        if (interest.InterestCommittee is not null)
        {
            dataRow.KeyDimensionLinks.Add(new KeyDimensionLink { Predicate = "vested-interest:hasCommittee", Uri = $"interest-committee:{interest.InterestCommittee.OgdId}" });
        }

        if (interest.LegalForm is not null && !string.IsNullOrWhiteSpace(interest.LegalForm.Uri))
        {
            if (interest.LegalForm.Uri.Contains(ldUri))
            {
                //if it is an ld.admin.ch URI, we can export it as a key dimension value, because the URI is known and the namespace is registered
                dataRow.KeyDimensionLinks.Add(new KeyDimensionLink { Predicate = OgdExportConstants.SchemaLegalName, Uri = $"ld:{interest.LegalForm.Uri.Replace(ldUri, "")}" });
            }
            else
            {
                //if we don't know the URI (not registered on the graph), the we export the URI as a literal
                dataRow.Values.Add(new DimensionValue { Predicate = OgdExportConstants.SchemaLegalName, Object = interest.LegalForm.Uri });
            }
        }

        if (!string.IsNullOrWhiteSpace(interest.Text) || !string.IsNullOrWhiteSpace(interest.InterestText))
        {
            dataRow.Values.Add(new DimensionValue { Predicate = OgdExportConstants.SchemaName, Object = interest.InterestText ?? interest.Text });
        }

        if (interest.UidOrganisationId is not null)
        {
            dataRow.Values.Add(new DimensionValue { Predicate = OgdExportConstants.SchemaIdentifier, Object = interest.UidOrganisationId });
        }

        return dataRow;
    }
}
