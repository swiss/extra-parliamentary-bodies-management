using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class CandidateListStateBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private readonly string _uri;
    private readonly bool _isDeleted;
    private readonly string _textDe;
    private readonly string _textFr;
    private readonly string _textIt;
    private readonly string _textRm;
    private readonly string _descriptionDe;
    private readonly string _descriptionFr;
    private readonly string _descriptionIt;
    private readonly string _descriptionRm;
    private readonly int _sort;
    private readonly int _oldId;

    public CandidateListStateBuilder()
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

    public CandidateListStateBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public CandidateListState Build()
    {
        return new CandidateListState
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
