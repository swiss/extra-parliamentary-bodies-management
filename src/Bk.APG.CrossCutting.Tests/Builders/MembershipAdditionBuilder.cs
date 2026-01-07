using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class MembershipAdditionBuilder
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
    private string _textRm;
    private string _descriptionDe;
    private string _descriptionFr;
    private string _descriptionIt;
    private string _descriptionRm;
    private readonly int _sort;
    private readonly int _oldId;

    public MembershipAdditionBuilder()
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
        _descriptionDe = _faker.Random.String();
        _descriptionFr = _faker.Random.String();
        _descriptionIt = _faker.Random.String();
        _descriptionRm = _faker.Random.String();
        _sort = _faker.Random.Int(0);
        _oldId = _faker.Random.Int(1);
    }

    public MembershipAdditionBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public MembershipAdditionBuilder WithUri(string uri)
    {
        _uri = uri;
        return this;
    }

    public MembershipAdditionBuilder WithGermanText(string text)
    {
        _textDe = text;
        return this;
    }

    public MembershipAdditionBuilder WithFrenchText(string text)
    {
        _textFr = text;
        return this;
    }

    public MembershipAdditionBuilder WithItalianText(string text)
    {
        _textIt = text;
        return this;
    }

    public MembershipAdditionBuilder WithRomanshText(string text)
    {
        _textRm = text;
        return this;
    }

    public MembershipAdditionBuilder WithGermanDescription(string description)
    {
        _descriptionDe = description;
        return this;
    }

    public MembershipAdditionBuilder WithFrenchDescription(string description)
    {
        _descriptionFr = description;
        return this;
    }

    public MembershipAdditionBuilder WithItalianDescription(string description)
    {
        _descriptionIt = description;
        return this;
    }

    public MembershipAdditionBuilder WithRomanshDescription(string description)
    {
        _descriptionRm = description;
        return this;
    }

    public MembershipAddition Build()
    {
        return new MembershipAddition
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
            DescriptionDe = _descriptionDe,
            DescriptionFr = _descriptionFr,
            DescriptionIt = _descriptionIt,
            DescriptionRm = _descriptionRm,
            Sort = _sort,
            OldId = _oldId
        };
    }
}
