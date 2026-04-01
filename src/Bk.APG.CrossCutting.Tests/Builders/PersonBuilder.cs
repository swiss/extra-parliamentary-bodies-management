using Bk.APG.Business.Models;
using Bogus;
using Person = Bk.APG.Business.Models.Person;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class PersonBuilder
{
    private readonly Faker _faker = new();
    private Guid _id;
    private int _ogdId;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private string _surname;
    private string _givenName;
    private int _birthYear;
    private string? _occupation;
    private string? _employer;
    private bool _noEmployment;
    private bool _federalDuty;
    private bool _federalAssembly;
    private string? _title;
    private Guid? _salutationId;
    private Guid _languageId;
    private Language? _language;
    private Language? _correspondenceLanguage;
    private Guid _correspondenceLanguageId;
    private Guid _genderId;
    private Gender? _gender;
    private Address? _officeAddress;
    private Guid? _officeAddressId;
    private Address? _privateAddress;
    private Guid? _privateAddressId;
    private Address? _correspondenceAddress;
    private Guid _correspondenceAddressId;
    private readonly int _oldId;
    private readonly string? _remarksPersonData;
    private readonly string? _remarksPersonDataAdmin;
    private bool _noInterest;
    private readonly List<LegislaturePeriod> _legislaturePeriods;
    private readonly List<Membership> _memberships;
    private readonly List<Occupation> _occupations;
    private Office? _office;
    private Guid? _officeId;
    private readonly List<Interest> _interests;

    public PersonBuilder()
    {
        _id = _faker.Random.Guid();
        _ogdId = _faker.Random.Int();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _surname = _faker.Person.LastName;
        _givenName = _faker.Person.FirstName;
        _birthYear = _faker.Person.DateOfBirth.Year;
        _occupation = _faker.Random.String().OrNull(_faker);
        _employer = _faker.Random.String().OrNull(_faker);
        _noEmployment = false;
        _federalDuty = false;
        _federalAssembly = false;
        _title = _faker.Random.String().OrNull(_faker);
        _salutationId = _faker.Random.Guid();
        _languageId = _faker.Random.Guid();
        _correspondenceLanguageId = _faker.Random.Guid();
        _genderId = _faker.Random.Guid();
        _officeAddressId = _faker.Random.Guid();
        _privateAddressId = _faker.Random.Guid();
        _correspondenceAddressId = _faker.Random.Guid();
        _oldId = _faker.Random.Int(1);
        _remarksPersonData = _faker.Random.String().OrNull(_faker);
        _remarksPersonDataAdmin = _faker.Random.String().OrNull(_faker);
        _noInterest = false;
        _legislaturePeriods = [];
        _memberships = [];
        _office = null;
        _officeId = null;
        _interests = [];
        _occupations = [];
    }

    public PersonBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public PersonBuilder WithOgdId(int id)
    {
        _ogdId = id;
        return this;
    }

    public PersonBuilder WithSalutationId(Guid? id)
    {
        _salutationId = id;
        return this;
    }

    public PersonBuilder WithLanguageId(Guid id)
    {
        _languageId = id;
        return this;
    }

    public PersonBuilder WithLanguage(Language language)
    {
        ArgumentNullException.ThrowIfNull(language);

        _language = language;
        _languageId = language.Id;
        return this;
    }

    public PersonBuilder WithCorrespondenceLanguage(Language language)
    {
        ArgumentNullException.ThrowIfNull(language);

        _correspondenceLanguage = language;
        _correspondenceLanguageId = language.Id;
        return this;
    }

    public PersonBuilder WithCorrespondenceLanguageId(Guid id)
    {
        _correspondenceLanguageId = id;
        return this;
    }

    public PersonBuilder WithGenderId(Guid genderId)
    {
        _genderId = genderId;
        return this;
    }

    public PersonBuilder WithGender(Gender gender)
    {
        ArgumentNullException.ThrowIfNull(gender);

        _gender = gender;
        _genderId = gender.Id;
        return this;
    }

    public PersonBuilder WithOfficeAddressId(Guid? id)
    {
        _officeAddressId = id;
        return this;
    }

    public PersonBuilder WithPrivateAddressId(Guid? id)
    {
        _privateAddressId = id;
        return this;
    }

    public PersonBuilder WithCorrespondenceAddressId(Guid id)
    {
        _correspondenceAddressId = id;
        return this;
    }

    public PersonBuilder WithOfficeAddress(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        _officeAddress = address;
        _officeAddressId = address.Id;
        return this;
    }

    public PersonBuilder WithPrivateAddress(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        _privateAddress = address;
        _privateAddressId = address.Id;
        return this;
    }

    public PersonBuilder WithCorrespondenceAddress(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        _correspondenceAddress = address;
        _correspondenceAddressId = address.Id;
        return this;
    }

    public PersonBuilder WithoutCorrespondenceAddress()
    {
        _correspondenceAddress = null;
        _correspondenceAddressId = Guid.Empty;
        return this;
    }

    public PersonBuilder WithSurname(string surname)
    {
        _surname = surname;
        return this;
    }

    public PersonBuilder WithGivenName(string givenName)
    {
        _givenName = givenName;
        return this;
    }

    public PersonBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public PersonBuilder WithBirthYear(int year)
    {
        _birthYear = year;
        return this;
    }

    public PersonBuilder WithLegislaturePeriods(IList<LegislaturePeriod> legislaturePeriods)
    {
        _legislaturePeriods.AddRange(legislaturePeriods);
        return this;
    }

    public PersonBuilder WithOccupations(IList<Occupation> occupations)
    {
        _occupations.AddRange(occupations);
        return this;
    }

    public PersonBuilder WithFederalDuty(bool federalDuty)
    {
        _federalDuty = federalDuty;
        return this;
    }

    public PersonBuilder WithFederalAssembly(bool federalAssembly)
    {
        _federalAssembly = federalAssembly;
        return this;
    }

    public PersonBuilder WithNoEmployment(bool noEmployment)
    {
        _noEmployment = noEmployment;
        return this;
    }

    public PersonBuilder WithNoInterest(bool noInterest)
    {
        _noInterest = noInterest;
        return this;
    }

    public PersonBuilder WithMemberships(IList<Membership> memberships)
    {
        _memberships.AddRange(memberships);
        return this;
    }

    public PersonBuilder WithOffice(Office office)
    {
        ArgumentNullException.ThrowIfNull(office);

        _office = office;
        _officeId = office.Id;
        return this;
    }

    public PersonBuilder WithOffice(Guid id)
    {
        _officeId = id;
        return this;
    }

    public PersonBuilder WithOccupation(string occupation)
    {
        _occupation = occupation;
        return this;
    }

    public PersonBuilder WithEmployer(string employer)
    {
        _employer = employer;
        return this;
    }

    public PersonBuilder WithInterests(IList<Interest> interests)
    {
        _interests.AddRange(interests);
        return this;
    }

    public Person Build()
    {
        return new Person
        {
            Id = _id,
            OgdId = _ogdId,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            Surname = _surname,
            GivenName = _givenName,
            BirthYear = _birthYear,
            Occupation = _occupation,
            Employer = _employer,
            NoEmployment = _noEmployment,
            FederalDuty = _federalDuty,
            FederalAssembly = _federalAssembly,
            Title = _title,
            SalutationId = _salutationId,
            LanguageId = _languageId,
            Language = _language,
            CorrespondenceLanguage = _correspondenceLanguage,
            CorrespondenceLanguageId = _correspondenceLanguageId,
            GenderId = _genderId,
            Gender = _gender,
            OfficeAddress = _officeAddress,
            OfficeAddressId = _officeAddressId,
            PrivateAddress = _privateAddress,
            PrivateAddressId = _privateAddressId,
            CorrespondenceAddress = _correspondenceAddress,
            CorrespondenceAddressId = _correspondenceAddressId,
            OldId = _oldId,
            RemarksPersonData = _remarksPersonData,
            RemarksPersonDataAdmin = _remarksPersonDataAdmin,
            NoInterest = _noInterest,
            LegislaturePeriods = _legislaturePeriods,
            Memberships = _memberships,
            Office = _office,
            OfficeId = _officeId,
            Interests = _interests,
            Occupations = _occupations,
        };
    }
}
