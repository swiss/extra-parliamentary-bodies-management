using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class FormLetterSenderMapper
{
    public static FormLetterSenderListDto ToFormLetterSenderListDto(FormLetterSender formLetterSender)
    {
        return new FormLetterSenderListDto
        {
            Id = formLetterSender.Id,
            Description = formLetterSender.Description,
            Surname = formLetterSender.Surname,
            GivenName = formLetterSender.GivenName,
            SenderFunction = formLetterSender.SenderFunction!.GetText(),
            Department = formLetterSender.Department!.GetText()
        };
    }

    public static FormLetterSender FromFormLetterSenderCreateDto(FormLetterSenderCreateDto formLetterSenderCreateDto, string currentUserName)
    {
        return new FormLetterSender
        {
            Description = formLetterSenderCreateDto.Description,
            Surname = formLetterSenderCreateDto.Surname,
            GivenName = formLetterSenderCreateDto.GivenName,
            SenderFunctionId = formLetterSenderCreateDto.SenderFunctionId,
            DepartmentId = formLetterSenderCreateDto.DepartmentId,
            OfficeId = formLetterSenderCreateDto.OfficeId,
            StreetGerman = formLetterSenderCreateDto.StreetGerman,
            StreetFrench = formLetterSenderCreateDto.StreetFrench,
            StreetItalian = formLetterSenderCreateDto.StreetItalian,
            StreetRomansh = formLetterSenderCreateDto.StreetRomansh,
            Zip = formLetterSenderCreateDto.Zip,
            CityGerman = formLetterSenderCreateDto.CityGerman,
            CityFrench = formLetterSenderCreateDto.CityFrench,
            CityItalian = formLetterSenderCreateDto.CityItalian,
            CityRomansh = formLetterSenderCreateDto.CityRomansh,
            Phone = formLetterSenderCreateDto.Phone,
            Email = formLetterSenderCreateDto.Email,
            Website = formLetterSenderCreateDto.Website,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName
        };
    }

    public static FormLetterSenderUpdateDto ToFormLetterSenderUpdateDto(FormLetterSender formLetterSender, bool canEditDepartment)
    {
        return new FormLetterSenderUpdateDto
        {
            Id = formLetterSender.Id,
            Description = formLetterSender.Description,
            SenderFunctionId = formLetterSender.SenderFunctionId,
            Surname = formLetterSender.Surname,
            GivenName = formLetterSender.GivenName,
            DepartmentId = formLetterSender.DepartmentId,
            OfficeId = formLetterSender.OfficeId,
            StreetGerman = formLetterSender.StreetGerman,
            StreetFrench = formLetterSender.StreetFrench,
            StreetItalian = formLetterSender.StreetItalian,
            StreetRomansh = formLetterSender.StreetRomansh,
            Zip = formLetterSender.Zip,
            CityGerman = formLetterSender.CityGerman,
            CityFrench = formLetterSender.CityFrench,
            CityItalian = formLetterSender.CityItalian,
            CityRomansh = formLetterSender.CityRomansh,
            Email = formLetterSender.Email,
            Phone = formLetterSender.Phone,
            Website = formLetterSender.Website,
            SignatureFileName = formLetterSender.SignatureFileReference?.DocumentName,
            CanEditDepartment = canEditDepartment
        };
    }
}
