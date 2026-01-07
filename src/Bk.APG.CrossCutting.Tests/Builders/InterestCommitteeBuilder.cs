using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class InterestCommitteeBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private int _ogdId;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private string _uri;
    private readonly bool _isDeleted;
    private readonly string _textDe;
    private readonly string _textFr;
    private readonly string _textIt;
    private readonly string _textRm;
    private string _descriptionDe;
    private string _descriptionFr;
    private string _descriptionIt;
    private string _descriptionRm;
    private readonly int _sort;
    private readonly int _oldId;

    public InterestCommitteeBuilder()
    {
        _id = _faker.Random.Guid();
        _ogdId = _faker.Random.Int();
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
    }

    public InterestCommitteeBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public InterestCommitteeBuilder WithOgdId(int ogdId)
    {
        _ogdId = ogdId;
        return this;
    }

    public InterestCommitteeBuilder WithUri(string uri)
    {
        _uri = uri;
        return this;
    }

    public InterestCommitteeBuilder WithDescriptionDe(string descriptionDe)
    {
        _descriptionDe = descriptionDe;
        return this;
    }

    public InterestCommitteeBuilder WithDescriptionFr(string descriptionFr)
    {
        _descriptionFr = descriptionFr;
        return this;
    }

    public InterestCommitteeBuilder WithDescriptionIt(string descriptionIt)
    {
        _descriptionIt = descriptionIt;
        return this;
    }

    public InterestCommitteeBuilder WithDescriptionRm(string descriptionRm)
    {
        _descriptionRm = descriptionRm;
        return this;
    }

    public InterestCommittee Build()
    {
        return new InterestCommittee
        {
            Id = _id,
            OgdId = _ogdId,
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
