using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class TermOfOfficeDateBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private string _uri;
    private DateOnly _beginDate;
    private DateOnly? _endDate;
    private readonly bool _isDeleted;
    private string _textDe;
    private readonly string _textFr;
    private readonly string _textIt;
    private readonly string _textRm;
    private string _descriptionDe;
    private readonly string _descriptionFr;
    private readonly string _descriptionIt;
    private readonly string _descriptionRm;
    private readonly int _sort;
    private readonly int _oldId;
    private bool _isGeneralElection;

    public TermOfOfficeDateBuilder()
    {
        _id = _faker.Random.Guid();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _uri = _faker.Internet.Url();
        _beginDate = _faker.Date.PastDateOnly();
        _endDate = _faker.Date.FutureDateOnly();
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
        _isGeneralElection = false;
    }

    public TermOfOfficeDateBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TermOfOfficeDateBuilder WithUri(string uri)
    {
        _uri = uri;
        return this;
    }

    public TermOfOfficeDateBuilder WithGermanText(string text)
    {
        _textDe = text;
        return this;
    }

    public TermOfOfficeDateBuilder WithGermanDescription(string description)
    {
        _descriptionDe = description;
        return this;
    }

    public TermOfOfficeDateBuilder WithBeginDate(DateOnly beginDate)
    {
        _beginDate = beginDate;
        return this;
    }

    public TermOfOfficeDateBuilder WithEndDate(DateOnly? endDate)
    {
        _endDate = endDate;
        return this;
    }

    public TermOfOfficeDateBuilder WithIsGeneralElection(bool isGeneralElection)
    {
        _isGeneralElection = isGeneralElection;
        return this;
    }

    public TermOfOfficeDate Build()
    {
        return new TermOfOfficeDate
        {
            Id = _id,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            Uri = _uri,
            BeginDate = _beginDate,
            EndDate = _endDate,
            IsDeleted = _isDeleted,
            TextDe = _textDe,
            TextFr = _textFr,
            TextIt = _textIt,
            TextRm = _textRm,
            DescriptionDe = _descriptionDe,
            DescriptionFr = _descriptionFr,
            DescriptionIt = _descriptionIt,
            DescriptionRm = _descriptionRm,
            IsGeneralElection = _isGeneralElection,
            Sort = _sort,
            OldId = _oldId
        };
    }
}
