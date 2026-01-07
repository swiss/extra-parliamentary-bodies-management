using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class OccupationBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private string _uri;
    private readonly bool _isDeleted;
    private string _textDe;
    private string _textFr;
    private string _textIt;
    private readonly string _textRm;
    private string _textFemaleDe;
    private string _textFemaleFr;
    private string _textFemaleIt;
    private readonly string _textFemaleRm;
    private string _descriptionDe;
    private string _descriptionFr;
    private string _descriptionIt;
    private readonly string _descriptionRm;
    private readonly int _sort;
    private readonly int _oldId;

    public OccupationBuilder()
    {
        _id = _faker.Random.Guid();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _uri = _faker.Internet.Url();
        _isDeleted = false;
        _textDe = _faker.Random.String();
        _textFr = _faker.Random.String();
        _textIt = _faker.Random.String();
        _textRm = _faker.Random.String();
        _textFemaleDe = _faker.Random.String();
        _textFemaleFr = _faker.Random.String();
        _textFemaleIt = _faker.Random.String();
        _textFemaleRm = _faker.Random.String();
        _descriptionDe = _faker.Random.String();
        _descriptionFr = _faker.Random.String();
        _descriptionIt = _faker.Random.String();
        _descriptionRm = _faker.Random.String();
        _sort = _faker.Random.Int(0);
        _oldId = _faker.Random.Int(1);
    }

    public OccupationBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public OccupationBuilder WithUri(string uri)
    {
        _uri = uri;
        return this;
    }

    public OccupationBuilder WithGermanText(string text)
    {
        _textDe = text;
        return this;
    }

    public OccupationBuilder WithGermanDescription(string description)
    {
        _descriptionDe = description;
        return this;
    }

    public OccupationBuilder WithFrenchText(string text)
    {
        _textFr = text;
        return this;
    }

    public OccupationBuilder WithFrenchDescription(string description)
    {
        _descriptionFr = description;
        return this;
    }

    public OccupationBuilder WithItalianText(string text)
    {
        _textIt = text;
        return this;
    }

    public OccupationBuilder WithItalianDescription(string description)
    {
        _descriptionIt = description;
        return this;
    }

    public OccupationBuilder WithGermanFemaleText(string text)
    {
        _textFemaleDe = text;
        return this;
    }

    public OccupationBuilder WithFrenchFemaleText(string text)
    {
        _textFemaleFr = text;
        return this;
    }

    public OccupationBuilder WithItalianFemaleText(string text)
    {
        _textFemaleIt = text;
        return this;
    }

    public Occupation Build()
    {
        return new Occupation
        {
            Id = _id,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            Uri = _uri,
            IsDeleted = _isDeleted,
            TextDe = _textDe,
            TextFr = _textFr,
            TextIt = _textIt,
            TextRm = _textRm,
            TextFemaleDe = _textFemaleDe,
            TextFemaleFr = _textFemaleFr,
            TextFemaleIt = _textFemaleIt,
            TextFemaleRm = _textFemaleRm,
            DescriptionDe = _descriptionDe,
            DescriptionFr = _descriptionFr,
            DescriptionIt = _descriptionIt,
            DescriptionRm = _descriptionRm,
            Sort = _sort,
            OldId = _oldId,
        };
    }
}
