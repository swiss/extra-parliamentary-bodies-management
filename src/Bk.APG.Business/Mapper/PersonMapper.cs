using System.Data;
using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;
using Swiss.FCh.Cube.Dimension.Model;
using Swiss.FCh.Cube.RawData.Model;

namespace Bk.APG.Business.Mapper;

public static class PersonMapper
{
    public static PersonListDto ToPersonListDto(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);
        ArgumentNullException.ThrowIfNull(person.Language);
        ArgumentNullException.ThrowIfNull(person.Memberships);

        return new PersonListDto
        {
            Id = person.Id,
            Surname = person.Surname,
            GivenName = person.GivenName,
            HasActiveMembership = person.Memberships.Any(m => m.IsActive),
            BirthYear = person.BirthYear,
            Canton = person.CorrespondenceAddress?.Canton?.GetText(),
            City = person.CorrespondenceAddress?.City,
            Language = person.Language.GetText(),
            NeedsAttention = person.NeedsAttention
        };
    }

    public static PersonMinimalDto ToPersonMinimalDto(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        return new PersonMinimalDto
        {
            Id = person.Id,
            GivenName = person.GivenName,
            Surname = person.Surname
        };
    }

    public static PersonFilterParameters? ToPersonFilterParameters(PersonFilterParametersDto? dto)
    {
        if (dto is null)
        {
            return null;
        }

        return new PersonFilterParameters
        {
            FreeText = dto.FreeText,
            CantonIds = dto.CantonIds,
            LanguageIds = dto.LanguageIds,
            HasActiveMembership = dto.HasActiveMembership,
        };
    }

    public static PersonDetailDto ToPersonDetailDto(Person person, bool maskAddress)
    {
        ArgumentNullException.ThrowIfNull(person);

        ArgumentNullException.ThrowIfNull(person.Language);
        ArgumentNullException.ThrowIfNull(person.CorrespondenceLanguage);
        ArgumentNullException.ThrowIfNull(person.Gender);

        var today = DateOnly.FromDateTime(DateTime.Today);

        var occupations = string.Join(" / ", person.Gender.Uri == Gender.Female
            ? person.Occupations.Select(o => o.GetFemaleText(CultureInfo.CurrentUICulture)).Order()
            : person.Occupations.Select(o => o.GetText(CultureInfo.CurrentUICulture)).Order()
        );

        var personDetail = new PersonDetailDto
        {
            Id = person.Id,
            Salutation = person.Salutation?.GetText(),
            SalutationText = person.SalutationText,
            Surname = person.Surname,
            GivenName = person.GivenName,
            BirthYear = person.BirthYear,
            MaskAddress = maskAddress,
            PrivateAddress = person.PrivateAddress is not null && person.CorrespondenceAddress is not null
                ? AddressMapper.ToAddressDetailDto(person.PrivateAddress!, person.CorrespondenceAddressId!.Value, maskAddress)
                : null,
            OfficeAddress = person.OfficeAddress is not null && person.CorrespondenceAddress is not null
                ? AddressMapper.ToAddressDetailDto(person.OfficeAddress!, person.CorrespondenceAddressId!.Value, maskAddress)
                : null,
            Language = person.Language.GetDescription(),
            LanguageId = person.LanguageId,
            CorrespondenceLanguage = person.CorrespondenceLanguage.GetDescription(),
            Title = person.Title,
            Occupation = person.Occupation,
            Occupations = occupations,
            Employer = person.Employer,
            NoEmployment = person.NoEmployment,
            NoInterest = person.NoInterest,
            Gender = person.Gender.GetText(),
            GenderId = person.Gender.Id,
            FederalDuty = person.FederalDuty,
            FederalAssembly = person.FederalAssembly,
            Office = person.Office is not null ? person.Office?.GetDescription() + " (" + person.Office?.GetText() + ")" : null,
            Council = person.Council?.GetText(),
            LegislaturePeriods = string.Join("/", person.LegislaturePeriods.Select(a => a.GetText())),
            Memberships = person.Memberships.Select(m =>
                new MembershipDetailDto
                {
                    Committee = m.Committee!.GetDescription(),
                    BeginDate = m.BeginDate,
                    EndDate = m.EndDate,
                    Function = m.Function!.GetText(),
                    Id = m.Id,
                    IsActive = m.IsActive,
                    IsFuture = m.IsFuture,
                    NeedsAttention = m.NeedsAttention
                }).ToList(),
            Interests = person.Interests
                .Where(i => (i.BeginDate == null || i.BeginDate <= today) && (i.EndDate == null || i.EndDate >= today))
                .Select(i => new InterestListDto
                {
                    Id = i.Id,
                    Text = !string.IsNullOrWhiteSpace(i.InterestText)
                        ? i.InterestText
                        : !string.IsNullOrWhiteSpace(i.Text)
                            ? i.Text
                            : "-",
                    Committee = i.InterestCommittee!.GetText(),
                    Function = i.InterestFunction!.GetText(),
                    LegalForm = i.LegalForm is not null ? i.LegalForm.GetText() : string.Empty
                })
                .ToList()
        };

        personDetail.Memberships = personDetail.Memberships
            .OrderByDescending(m => m.IsActive || m.NeedsAttention)
            .ThenByDescending(m => m.BeginDate)
            .ToList();

        personDetail.Interests = personDetail.Interests
            .OrderBy(i => i.Text)
            .ToList();

        personDetail.NeedsAttentionLongerDuty = person.NeedsAttentionLongerDuty;
        personDetail.NeedsAttentionShorterDuty = person.NeedsAttentionShorterDuty;
        personDetail.NeedsAttentionFederalDuty = person.NeedsAttentionFederalDuty;
        personDetail.NeedsAttentionFederalAssemblyAdministrationCommission = person.NeedsAttentionFederalAssemblyAdministrationCommission;
        personDetail.NeedsAttentionFederalAssemblyAuthoritiesCommission = person.NeedsAttentionFederalAssemblyAuthoritiesCommission;
        personDetail.NeedsAttentionBasicData = person.NeedsAttentionBasicData;
        personDetail.NeedsAttentionOccupation = person.NeedsAttentionOccupation;
        personDetail.NeedsAttentionInterests = person.NeedsAttentionInterests;
        personDetail.NeedsAttentionMembershipExpired = person.NeedsAttentionMembershipExpired;

        return personDetail;
    }

    public static PersonUpdateDto ToPersonUpdateDto(Person person, bool maskAddress, bool canDelete)
    {
        ArgumentNullException.ThrowIfNull(person);

        var isMissingJustificationFederalAssembly =
            person.Memberships.Any(m => m.JustificationMemberInFederalAssemblyNeeded && string.IsNullOrWhiteSpace(m.JustificationMemberInFederalDuty) &&
                                        person.LegislaturePeriods.Any(lp => (m.BeginDate >= lp.StartDate && m.EndDate <= lp.EndDate) ||
                                                                            (m.EndDate >= lp.StartDate && m.EndDate <= lp.EndDate) ||
                                                                            (m.BeginDate <= lp.StartDate && m.EndDate >= lp.EndDate)));

        var hasActiveMembership =
            person.Memberships.Any(m => m.IsActive);

        var personUpdateDto = new PersonUpdateDto
        {
            Id = person.Id,
            Surname = person.Surname,
            GivenName = person.GivenName,
            BirthYear = person.BirthYear,
            MaskAddress = maskAddress,
            PrivateAddress = person.PrivateAddress is not null && person.CorrespondenceAddress is not null && !maskAddress
                ? AddressMapper.ToAddressUpdateDto(person.PrivateAddress!, person.CorrespondenceAddressId!.Value)
                : null,
            OfficeAddress = person.OfficeAddress is not null && person.CorrespondenceAddress is not null && !maskAddress
                ? AddressMapper.ToAddressUpdateDto(person.OfficeAddress!, person.CorrespondenceAddressId!.Value)
                : null,
            LanguageId = person.LanguageId,
            CorrespondenceLanguageId = person.CorrespondenceLanguageId,
            Title = person.Title,
            Occupation = person.Occupation,
            Employer = person.Employer,
            NoEmployment = person.NoEmployment,
            NoInterest = person.NoInterest,
            GenderId = person.GenderId,
            FederalDuty = person.FederalDuty,
            FederalAssembly = person.FederalAssembly,
            SalutationId = person.SalutationId,
            SalutationText = person.SalutationText,
            OfficeId = person.OfficeId,
            CouncilId = person.CouncilId,
            HasInterests = person.Interests.Count != 0,
            LegislaturePeriodIds = person.LegislaturePeriods.Select(lp => lp.Id).ToList(),
            IsMissingJustificationFederalAssembly = isMissingJustificationFederalAssembly,
            HasActiveMembership = hasActiveMembership,
            Occupations = person.Occupations.Select(o => MasterDataMapper.MapToOccupationDto(o, CultureInfo.CurrentUICulture)).ToList(),
            NeedsAttentionOccupation = person.NeedsAttentionOccupation,
            RowVersion = person.RowVersion,
            CanDelete = canDelete
        };

        return personUpdateDto;
    }

    public static Person FromPersonCreateDto(PersonCreateDto personCreateDto)
    {
        ArgumentNullException.ThrowIfNull(personCreateDto);
        ArgumentNullException.ThrowIfNull(personCreateDto.Surname);
        ArgumentNullException.ThrowIfNull(personCreateDto.GivenName);
        ArgumentNullException.ThrowIfNull(personCreateDto.BirthYear);
        ArgumentNullException.ThrowIfNull(personCreateDto.LanguageId);
        ArgumentNullException.ThrowIfNull(personCreateDto.GenderId);
        ArgumentNullException.ThrowIfNull(personCreateDto.CorrespondenceLanguageId);

        var privateAddress = personCreateDto.PrivateAddress is not null ? AddressMapper.FromAddressUpdateDto(personCreateDto.PrivateAddress!) : null;
        var officeAddress = personCreateDto.OfficeAddress is not null ? AddressMapper.FromAddressUpdateDto(personCreateDto.OfficeAddress!) : null;

        var person = new Person
        {
            Surname = personCreateDto.Surname,
            GivenName = personCreateDto.GivenName,
            BirthYear = personCreateDto.BirthYear,
            PrivateAddress = privateAddress,
            PrivateAddressId = privateAddress?.Id,
            OfficeAddress = officeAddress,
            OfficeAddressId = officeAddress?.Id,
            CorrespondenceAddress = (personCreateDto.PrivateAddress is not null && personCreateDto.PrivateAddress.ActiveAddress ? privateAddress : officeAddress)!,
            CorrespondenceAddressId = (personCreateDto.PrivateAddress is not null && personCreateDto.PrivateAddress.ActiveAddress ? privateAddress : officeAddress)!.Id,
            LanguageId = personCreateDto.LanguageId,
            CorrespondenceLanguageId = personCreateDto.CorrespondenceLanguageId,
            Title = personCreateDto.Title,
            Occupation = personCreateDto.Occupation,
            Employer = personCreateDto.Employer,
            NoEmployment = personCreateDto.NoEmployment,
            NoInterest = personCreateDto.NoInterest,
            GenderId = personCreateDto.GenderId,
            FederalDuty = personCreateDto.FederalDuty,
            FederalAssembly = personCreateDto.FederalAssembly,
            SalutationId = personCreateDto.SalutationId,
            SalutationText = personCreateDto.SalutationText,
            OfficeId = personCreateDto.OfficeId,
            CouncilId = personCreateDto.CouncilId,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            ModifiedBy = "SYSTEM",
        };

        return person;
    }

    public static Person FromMembershipCandidate(MembershipCandidate membershipCandidate)
    {
        ArgumentNullException.ThrowIfNull(membershipCandidate);

        ArgumentNullException.ThrowIfNull(membershipCandidate.Surname);
        ArgumentNullException.ThrowIfNull(membershipCandidate.GivenName);
        ArgumentNullException.ThrowIfNull(membershipCandidate.BirthYear);
        ArgumentNullException.ThrowIfNull(membershipCandidate.LanguageId);
        ArgumentNullException.ThrowIfNull(membershipCandidate.GenderId);

        var person = new Person
        {
            Surname = membershipCandidate.Surname,
            GivenName = membershipCandidate.GivenName,
            BirthYear = membershipCandidate.BirthYear,
            LanguageId = membershipCandidate.LanguageId,
            CorrespondenceLanguageId = membershipCandidate.LanguageId,
            NoEmployment = false,
            NoInterest = false,
            GenderId = membershipCandidate.GenderId,
            FederalDuty = false,
            FederalAssembly = false,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            ModifiedBy = "SYSTEM",
        };

        return person;
    }

    public static DimensionItem ToDimensionItem(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        var dimensionItem =
            new DimensionItem(
                person.OgdId,
                new Literal($"{person.Surname}, {person.GivenName} ({person.BirthYear})", OgdExportConstants.LanguageDe),
                [
                    new AdditionalLiteralProperty(OgdExportConstants.SchemaGivenName, new Literal(person.GivenName)),
                    new AdditionalLiteralProperty(OgdExportConstants.SchemaFamilyName, new Literal(person.Surname)),
                    new AdditionalLiteralProperty(OgdExportConstants.SchemaBirthDate, new Literal(person.BirthYear.ToString(CultureInfo.InvariantCulture)))
                ]);

        dimensionItem.AdditionalUriProperties.Add(new AdditionalUriProperty(OgdExportConstants.SchemaGender, OgdExportConstants.CreateUriLinkForRegisterLdAdminCh(person.IsFemale ? Gender.Female : Gender.Male)));

        if (!string.IsNullOrWhiteSpace(person.Title))
        {
            dimensionItem.AdditionalLiteralProperties.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaHonorificPrefix, new Literal(person.Title, OgdExportConstants.LanguageDe)));
        }

        foreach (var occupation in person.Occupations)
        {
            dimensionItem.AdditionalUriProperties.Add(new AdditionalUriProperty(OgdExportConstants.PersonHasOccupation, $"occupation:{occupation.OgdId}"));

            if (person.IsFemale)
            {
                if (!string.IsNullOrWhiteSpace(occupation.TextFemaleDe))
                {
                    dimensionItem.AdditionalLiteralProperties.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaOccupation, new Literal(occupation.TextFemaleDe, OgdExportConstants.LanguageDe)));
                }
                if (!string.IsNullOrWhiteSpace(occupation.TextFemaleFr))
                {
                    dimensionItem.AdditionalLiteralProperties.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaOccupation, new Literal(occupation.TextFemaleFr, OgdExportConstants.LanguageFr)));
                }
                if (!string.IsNullOrWhiteSpace(occupation.TextFemaleIt))
                {
                    dimensionItem.AdditionalLiteralProperties.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaOccupation, new Literal(occupation.TextFemaleIt, OgdExportConstants.LanguageIt)));
                }
                if (!string.IsNullOrWhiteSpace(occupation.TextFemaleRm))
                {
                    dimensionItem.AdditionalLiteralProperties.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaOccupation, new Literal(occupation.TextFemaleRm, OgdExportConstants.LanguageRm)));
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(occupation.TextDe))
                {
                    dimensionItem.AdditionalLiteralProperties.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaOccupation, new Literal(occupation.TextDe, OgdExportConstants.LanguageDe)));
                }
                if (!string.IsNullOrWhiteSpace(occupation.TextFr))
                {
                    dimensionItem.AdditionalLiteralProperties.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaOccupation, new Literal(occupation.TextFr, OgdExportConstants.LanguageFr)));
                }
                if (!string.IsNullOrWhiteSpace(occupation.TextIt))
                {
                    dimensionItem.AdditionalLiteralProperties.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaOccupation, new Literal(occupation.TextIt, OgdExportConstants.LanguageIt)));
                }
                if (!string.IsNullOrWhiteSpace(occupation.TextRm))
                {
                    dimensionItem.AdditionalLiteralProperties.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaOccupation, new Literal(occupation.TextRm, OgdExportConstants.LanguageRm)));
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(person.Employer))
        {
            dimensionItem.AdditionalLiteralProperties.Add(new AdditionalLiteralProperty(OgdExportConstants.SchemaWorksFor, new Literal(person.Employer)));
        }

        if (person is { OfficeId: not null, Office: not null, FederalDuty: true })
        {
            dimensionItem.AdditionalUriProperties.Add(new AdditionalUriProperty(OgdExportConstants.PersonHasOffice, OgdExportConstants.CreateUriLinkForLdAdminCh(person.Office.Uri)));

            if (person.Office.Department != null)
            {
                dimensionItem.AdditionalUriProperties.Add(new AdditionalUriProperty(OgdExportConstants.PersonHasDepartment, OgdExportConstants.CreateUriLinkForLdAdminCh(person.Office.Department.Uri)));
            }
        }
        return dimensionItem;
    }

    public static ObservationDataRow ToObservation(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        var dataRow = new ObservationDataRow
        {
            KeyUri = $"{OgdExportConstants.NamespacePerson}:{person.OgdId}"
        };

        dataRow.KeyDimensionLinks.Add(new KeyDimensionLink
        {
            Predicate = $"{OgdExportConstants.NamespacePerson}:hasPerson", Uri = $"{OgdExportConstants.NamespacePerson}:{person.OgdId}", ShapePropertyMetadata = new ShapePropertyMetadata
            {
                NameDe = "Mitglied",
                NameFr = "Membre",
                NameIt = "Membro",
                NameEn = "Member",
                Type = OgdExportConstants.CubeKeyDimension,
                NodeKind = OgdExportConstants.ShaclNodeKindIri,
                ScaleType = OgdExportConstants.QudtNominalScale,
                MinCount = 1,
                MaxCount = 1
            }
        });

        return dataRow;
    }
}
