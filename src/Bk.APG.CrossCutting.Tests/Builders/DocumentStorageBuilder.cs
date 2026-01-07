using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class DocumentStorageBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private string _documentName;
    private string _documentStorageId;

    public DocumentStorageBuilder()
    {
        _id = _faker.Random.Guid();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _documentName = _faker.Random.String();
        _documentStorageId = _faker.Random.String();
    }

    public DocumentStorageBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public DocumentStorageBuilder WithDisplayName(string documentName)
    {
        _documentName = documentName;
        return this;
    }

    public DocumentStorageBuilder WithDocumentStorageId(string documentStorageId)
    {
        _documentStorageId = documentStorageId;
        return this;
    }

    public DocumentStorage Build()
    {
        return new DocumentStorage
        {
            Id = _id,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            DocumentName = _documentName,
            DocumentStorageId = _documentStorageId
        };
    }
}
