using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class ContactPointMapper
{
    public static ContactPointDetailDto ToContactPointDetailDto(ContactPoint contactPoint)
    {
        return new ContactPointDetailDto
        {
            Id = contactPoint.Id,
            ContactPointType = contactPoint.ContactPointType?.GetText() ?? string.Empty,
            CompanyName = contactPoint.CompanyName,
            Section = contactPoint.Section,
            BeginDate = contactPoint.BeginDate,
            EndDate = contactPoint.EndDate,
            Street = contactPoint.Street,
            PoBox = contactPoint.PoBox,
            Zip = contactPoint.Zip,
            City = contactPoint.City,
            Phone = contactPoint.Phone,
            Email = contactPoint.Email,
            Surname = contactPoint.Surname,
            GivenName = contactPoint.GivenName,
            Title = contactPoint.Title,
            Language = contactPoint.Language?.GetText() ?? string.Empty,
            Gender = contactPoint.Gender?.GetText(),
            PersonalPhone = contactPoint.PersonalPhone,
            PersonalMobile = contactPoint.PersonalMobile,
            PersonalEmail = contactPoint.PersonalEmail,
            ReleasePersonData = contactPoint.ReleasePersonData,
        };
    }

    public static ContactPointCreateDto FromContactPointUpdateToCreateDto(ContactPointUpdateDto contactPointUpdate)
    {
        return new ContactPointCreateDto
        {
            CommitteeId = contactPointUpdate.CommitteeId,
            ContactPointTypeId = contactPointUpdate.ContactPointTypeId,
            ContactPointTypeUri = contactPointUpdate.ContactPointTypeUri,
            CompanyName = contactPointUpdate.CompanyName,
            Section = contactPointUpdate.Section,
            BeginDate = contactPointUpdate.BeginDate,
            EndDate = contactPointUpdate.EndDate,
            Street = contactPointUpdate.Street,
            PoBox = contactPointUpdate.PoBox,
            Zip = contactPointUpdate.Zip,
            City = contactPointUpdate.City,
            Phone = contactPointUpdate.Phone,
            Email = contactPointUpdate.Email,
            Surname = contactPointUpdate.Surname,
            GivenName = contactPointUpdate.GivenName,
            PersonalPhone = contactPointUpdate.PersonalPhone,
            PersonalMobile = contactPointUpdate.PersonalMobile,
            PersonalEmail = contactPointUpdate.PersonalEmail,
            Title = contactPointUpdate.Title,
            LanguageId = contactPointUpdate.LanguageId,
            GenderId = contactPointUpdate.GenderId,
            CommitteeBeginDate = contactPointUpdate.CommitteeBeginDate,
        };
    }

    public static ContactPoint FromContactPointCreateDto(ContactPointCreateDto contactPointCreate)
    {
        return new ContactPoint
        {
            CommitteeId = contactPointCreate.CommitteeId,
            ContactPointTypeId = contactPointCreate.ContactPointTypeId,
            CompanyName = contactPointCreate.CompanyName,
            Section = contactPointCreate.Section,
            BeginDate = contactPointCreate.BeginDate,
            EndDate = contactPointCreate.EndDate,
            Street = contactPointCreate.Street,
            PoBox = contactPointCreate.PoBox,
            Zip = contactPointCreate.Zip,
            City = contactPointCreate.City,
            Phone = contactPointCreate.Phone,
            Email = contactPointCreate.Email,
            Surname = contactPointCreate.Surname,
            GivenName = contactPointCreate.GivenName,
            Title = contactPointCreate.Title,
            LanguageId = contactPointCreate.LanguageId,
            GenderId = contactPointCreate.GenderId,
            PersonalPhone = contactPointCreate.PersonalPhone,
            PersonalMobile = contactPointCreate.PersonalMobile,
            PersonalEmail = contactPointCreate.PersonalEmail,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            ModifiedBy = "SYSTEM"
        };
    }

    public static ContactPointUpdateDto ToContactPointUpdateDto(ContactPoint contactPoint)
    {
        return new ContactPointUpdateDto
        {
            Id = contactPoint.Id,
            CommitteeId = contactPoint.CommitteeId,
            ContactPointTypeUri = contactPoint.ContactPointType!.Uri,
            ContactPointTypeId = contactPoint.ContactPointTypeId,
            CompanyName = contactPoint.CompanyName,
            Section = contactPoint.Section,
            BeginDate = contactPoint.BeginDate,
            EndDate = contactPoint.EndDate,
            Street = contactPoint.Street,
            PoBox = contactPoint.PoBox,
            Zip = contactPoint.Zip!,
            City = contactPoint.City!,
            Phone = contactPoint.Phone,
            Email = contactPoint.Email,
            Surname = contactPoint.Surname,
            GivenName = contactPoint.GivenName,
            Title = contactPoint.Title,
            LanguageId = contactPoint.LanguageId,
            GenderId = contactPoint.GenderId,
            PersonalPhone = contactPoint.PersonalPhone,
            PersonalMobile = contactPoint.PersonalMobile,
            PersonalEmail = contactPoint.PersonalEmail,
            ReleasePersonData = contactPoint.ReleasePersonData,
            CommitteeBeginDate = contactPoint.Committee!.BeginDate,
            RowVersion = contactPoint.RowVersion
        };
    }

    public static ContactPoint FromContactPointUpdateDto(ContactPointUpdateDto contactPointUpdateDto)
    {
        return new ContactPoint
        {
            Id = contactPointUpdateDto.Id,
            CommitteeId = contactPointUpdateDto.CommitteeId,
            ContactPointTypeId = contactPointUpdateDto.ContactPointTypeId,
            CompanyName = contactPointUpdateDto.CompanyName,
            Section = contactPointUpdateDto.Section,
            BeginDate = contactPointUpdateDto.BeginDate,
            EndDate = contactPointUpdateDto.EndDate,
            Street = contactPointUpdateDto.Street,
            PoBox = contactPointUpdateDto.PoBox,
            Zip = contactPointUpdateDto.Zip,
            City = contactPointUpdateDto.City,
            Phone = contactPointUpdateDto.Phone,
            Email = contactPointUpdateDto.Email,
            Surname = contactPointUpdateDto.Surname,
            GivenName = contactPointUpdateDto.GivenName,
            Title = contactPointUpdateDto.Title,
            LanguageId = contactPointUpdateDto.LanguageId,
            GenderId = contactPointUpdateDto.GenderId,
            PersonalPhone = contactPointUpdateDto.PersonalPhone,
            PersonalMobile = contactPointUpdateDto.PersonalMobile,
            PersonalEmail = contactPointUpdateDto.PersonalEmail,
            ReleasePersonData = contactPointUpdateDto.ReleasePersonData,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            ModifiedBy = "SYSTEM"
        };
    }

    public static ContactPointCreateDto ToContactPointCreateDto(ContactPoint contactPoint)
    {
        return new ContactPointCreateDto
        {
            CommitteeId = contactPoint.CommitteeId,
            ContactPointTypeUri = contactPoint.ContactPointType!.Uri,
            ContactPointTypeId = contactPoint.ContactPointTypeId,
            CompanyName = contactPoint.CompanyName,
            Section = contactPoint.Section,
            BeginDate = contactPoint.BeginDate,
            EndDate = contactPoint.EndDate,
            Street = contactPoint.Street,
            PoBox = contactPoint.PoBox,
            Zip = contactPoint.Zip!,
            City = contactPoint.City!,
            Phone = contactPoint.Phone,
            Email = contactPoint.Email,
            Surname = contactPoint.Surname,
            GivenName = contactPoint.GivenName,
            Title = contactPoint.Title,
            LanguageId = contactPoint.LanguageId,
            GenderId = contactPoint.GenderId,
            PersonalPhone = contactPoint.PersonalPhone,
            PersonalMobile = contactPoint.PersonalMobile,
            PersonalEmail = contactPoint.PersonalEmail,
            ReleasePersonData = contactPoint.ReleasePersonData,
        };
    }

    public static ContactPointListDto ToContactPointListDto(ContactPoint contactPoint)
    {
        return new ContactPointListDto
        {
            Id = contactPoint.Id,
            ContactPointType = contactPoint.ContactPointType!.GetText(),
            CompanyName = contactPoint.CompanyName,
            Section = contactPoint.Section,
            BeginDate = contactPoint.BeginDate,
            EndDate = contactPoint.EndDate,
            ZipCity = contactPoint.Zip + " " + contactPoint.City,
            PersonName = contactPoint.Surname + " " + contactPoint.GivenName,
            Phone = string.IsNullOrWhiteSpace(contactPoint.Phone) ? contactPoint.PersonalPhone : contactPoint.Phone,
            Mobile = string.IsNullOrWhiteSpace(contactPoint.PersonalMobile) ? string.Empty : contactPoint.PersonalMobile,
            Email = string.IsNullOrWhiteSpace(contactPoint.Email) ? contactPoint.PersonalEmail : contactPoint.Email,
        };
    }
}
