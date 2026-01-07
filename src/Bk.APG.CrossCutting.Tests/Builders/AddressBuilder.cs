using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class AddressBuilder
{
    private readonly Faker _faker = new();
    private Guid _id;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private readonly string? _companyName;
    private readonly string? _street;
    private readonly string? _poBox;
    private readonly string? _countryCode;
    private readonly string? _zip;
    private string? _city;
    private Guid? _cantonId;
    private Canton? _canton;
    private string? _phone;
    private string? _mobile;
    private string? _email;
    private readonly int _oldId;

    public AddressBuilder()
    {
        _id = _faker.Random.Guid();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _companyName = _faker.Company.CompanyName().OrNull(_faker);
        _street = _faker.Address.StreetName().OrNull(_faker);
        _poBox = _faker.Random.String().OrNull(_faker);
        _countryCode = _faker.Address.CountryCode().OrNull(_faker);
        _zip = _faker.Address.ZipCode().OrNull(_faker);
        _city = _faker.Address.City().OrNull(_faker);
        _cantonId = _faker.Random.Guid();
        _phone = _faker.Phone.PhoneNumber().OrNull(_faker);
        _mobile = _faker.Phone.PhoneNumber().OrNull(_faker);
        _email = _faker.Person.Email.OrNull(_faker);
        _oldId = _faker.Random.Int(1);
    }

    public AddressBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public AddressBuilder WithCantonId(Guid? cantonId)
    {
        _cantonId = cantonId;
        return this;
    }

    public AddressBuilder WithCity(string? city)
    {
        _city = city;
        return this;
    }

    public AddressBuilder WithCanton(Canton? canton)
    {
        _cantonId = canton?.Id;
        _canton = canton;
        return this;
    }

    public AddressBuilder WithMobile(string? mobile)
    {
        _mobile = mobile;
        return this;
    }

    public AddressBuilder WithPhone(string? phone)
    {
        _phone = phone;
        return this;
    }

    public AddressBuilder WithEmail(string? email)
    {
        _email = email;
        return this;
    }

    public Address Build()
    {
        return new Address
        {
            Id = _id,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            CompanyName = _companyName,
            Street = _street,
            PoBox = _poBox,
            CountryCode = _countryCode,
            Zip = _zip,
            City = _city,
            CantonId = _cantonId,
            Canton = _canton,
            Phone = _phone,
            Mobile = _mobile,
            Email = _email,
            OldId = _oldId
        };
    }
}
