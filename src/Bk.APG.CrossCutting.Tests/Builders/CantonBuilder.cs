using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class CantonBuilder
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
    private readonly string _textFr;
    private readonly string _textIt;
    private readonly string _textRm;
    private readonly string _descriptionDe;
    private readonly string _descriptionFr;
    private readonly string _descriptionIt;
    private readonly string _descriptionRm;
    private readonly int _sort;
    private readonly int _oldId;
    private readonly string? _region;

    public CantonBuilder()
    {
        _id = _faker.Random.Guid();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _uri = _faker.Internet.Url();
        _isDeleted = false;
        _textDe = _faker.Lorem.Text();
        _textFr = _faker.Lorem.Text();
        _textIt = _faker.Lorem.Text();
        _textRm = _faker.Lorem.Text();
        _descriptionDe = _faker.Lorem.Text();
        _descriptionFr = _faker.Lorem.Text();
        _descriptionIt = _faker.Lorem.Text();
        _descriptionRm = _faker.Lorem.Text();
        _sort = _faker.Random.Int(0);
        _oldId = _faker.Random.Int(1);
        _region = _faker.Lorem.Text().OrNull(_faker);
    }

    public CantonBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public CantonBuilder WithUri(string uri)
    {
        _uri = uri;
        return this;
    }

    public CantonBuilder WithTextDe(string textDe)
    {
        _textDe = textDe;
        return this;
    }

    public Canton Build()
    {
        return new Canton
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
            OldId = _oldId,
            Region = _region
        };
    }
}
