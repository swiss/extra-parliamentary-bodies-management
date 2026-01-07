using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class ContactPointBuilder
{
    private readonly Faker _faker = new();
    private Guid _id;
    private int _ogdId;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private Guid _committeeId;
    private Committee _committee;
    private Guid _contactPointTypeId;
    private ContactPointType? _contactPointType;
    private string _companyName;
    private string _section;
    private DateOnly _beginDate;
    private DateOnly? _endDate;
    private readonly string _street;
    private readonly string _poBox;
    private readonly string _zip;
    private readonly string _city;
    private string _phone;
    private string _email;
    private string _surname;
    private string _givenname;
    private string _title;
    private Language? _language;
    private Guid _languageId;
    private Gender? _gender;
    private Guid _genderId;
    private string _personalPhone;
    private string _personalMobile;
    private string _personalEmail;
    private bool _releasePersonData;
    private readonly int _oldId;

    public ContactPointBuilder()
    {
        _id = _faker.Random.Guid();
        _ogdId = _faker.Random.Int();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _committeeId = _faker.Random.Guid();
        _contactPointTypeId = _faker.Random.Guid();
        _companyName = _faker.Random.String();
        _section = _faker.Random.String();
        _beginDate = _faker.Date.PastDateOnly();
        _endDate = _faker.Date.FutureDateOnly();
        _street = _faker.Address.StreetAddress();
        _poBox = _faker.Random.String();
        _zip = _faker.Address.ZipCode();
        _city = _faker.Address.City();
        _phone = _faker.Phone.PhoneNumber();
        _email = _faker.Person.Email;
        _surname = _faker.Lorem.Word();
        _givenname = _faker.Lorem.Word();
        _title = _faker.Lorem.Word();
        _languageId = _faker.Random.Guid();
        _genderId = _faker.Random.Guid();
        _personalPhone = _faker.Phone.PhoneNumber();
        _personalMobile = _faker.Phone.PhoneNumber();
        _personalEmail = _faker.Person.Email;
        _releasePersonData = _faker.Random.Bool();
        _oldId = _faker.Random.Int(1);
        _committee = new CommitteeBuilder().Build();
    }

    public ContactPointBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ContactPointBuilder WithOgdId(int id)
    {
        _ogdId = id;
        return this;
    }

    public ContactPointBuilder WithCommittee(Committee committee)
    {
        _committee = committee;
        _committeeId = committee.Id;
        return this;
    }

    public ContactPointBuilder WithCommitteeId(Guid committeeId)
    {
        _committeeId = committeeId;
        return this;
    }

    public ContactPointBuilder WithBeginDate(DateOnly beginDate)
    {
        _beginDate = beginDate;
        return this;
    }

    public ContactPointBuilder WithEndDate(DateOnly? endDate)
    {
        _endDate = endDate;
        return this;
    }

    public ContactPointBuilder WithContactPointType(ContactPointType contactPointType)
    {
        _contactPointType = contactPointType;
        _contactPointTypeId = contactPointType.Id;
        return this;
    }

    public ContactPointBuilder WithLanguage(Language language)
    {
        _language = language;
        _languageId = language.Id;
        return this;
    }

    public ContactPointBuilder WithLanguageId(Guid languageId)
    {
        _languageId = languageId;
        return this;
    }

    public ContactPointBuilder WithGender(Gender gender)
    {
        _gender = gender;
        _genderId = gender.Id;
        return this;
    }

    public ContactPointBuilder WithGenderId(Guid genderId)
    {
        _genderId = genderId;
        return this;
    }

    public ContactPointBuilder WithCompanyName(string companyName)
    {
        _companyName = companyName;
        return this;
    }

    public ContactPointBuilder WithSection(string section)
    {
        _section = section;
        return this;
    }

    public ContactPointBuilder WithSurnameAndGivenName(string surname, string givenname)
    {
        _surname = surname;
        _givenname = givenname;
        return this;
    }

    public ContactPointBuilder WithPhone(string phone)
    {
        _phone = phone;
        return this;
    }

    public ContactPointBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public ContactPointBuilder WithPersonalPhone(string phone)
    {
        _personalPhone = phone;
        return this;
    }

    public ContactPointBuilder WithPersonalMobile(string mobile)
    {
        _personalMobile = mobile;
        return this;
    }

    public ContactPointBuilder WithPersonalEmail(string email)
    {
        _personalEmail = email;
        return this;
    }

    public ContactPointBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ContactPointBuilder WithReleasePersonData(bool releasePersonData)
    {
        _releasePersonData = releasePersonData;
        return this;
    }

    public ContactPoint Build()
    {
        return new ContactPoint
        {
            Id = _id,
            OgdId = _ogdId,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            CommitteeId = _committeeId,
            Committee = _committee,
            ContactPointTypeId = _contactPointTypeId,
            ContactPointType = _contactPointType,
            CompanyName = _companyName,
            Section = _section,
            BeginDate = _beginDate,
            EndDate = _endDate,
            Street = _street,
            PoBox = _poBox,
            Zip = _zip,
            City = _city,
            Phone = _phone,
            Email = _email,
            Surname = _surname,
            GivenName = _givenname,
            Title = _title,
            Language = _language,
            LanguageId = _languageId,
            Gender = _gender,
            GenderId = _genderId,
            PersonalPhone = _personalPhone,
            PersonalMobile = _personalMobile,
            PersonalEmail = _personalEmail,
            ReleasePersonData = _releasePersonData,
            OldId = _oldId
        };
    }
}
