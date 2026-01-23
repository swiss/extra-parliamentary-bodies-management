using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class OfficeBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private Department? _department;
    private Guid _departmentId;
    private string _uri;
    private readonly bool _isDeleted;
    private string _textDe;
    private string _textFr;
    private string _textIt;
    private readonly string _textRm;
    private string _descriptionDe;
    private string _descriptionFr;
    private string _descriptionIt;
    private readonly string _descriptionRm;
    private readonly int _sort;
    private readonly int _oldId;
    private bool _isCentralFederalAdministration;
    private bool _isGeneralSecretariat;
    private readonly Guid _eiamAssignmentId;

    public OfficeBuilder()
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
        _departmentId = _faker.Random.Guid();
        _department = new DepartmentBuilder().WithId(_departmentId).Build();
        _isCentralFederalAdministration = false;
        _isGeneralSecretariat = false;
        _eiamAssignmentId = _faker.Random.Guid();
    }

    public OfficeBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public OfficeBuilder WithUri(string uri)
    {
        _uri = uri;
        return this;
    }

    public OfficeBuilder WithGermanText(string text)
    {
        _textDe = text;
        return this;
    }

    public OfficeBuilder WithGermanDescription(string description)
    {
        _descriptionDe = description;
        return this;
    }

    public OfficeBuilder WithFrenchText(string text)
    {
        _textFr = text;
        return this;
    }

    public OfficeBuilder WithFrenchDescription(string description)
    {
        _descriptionFr = description;
        return this;
    }

    public OfficeBuilder WithItalianText(string text)
    {
        _textIt = text;
        return this;
    }

    public OfficeBuilder WithItalianDescription(string description)
    {
        _descriptionIt = description;
        return this;
    }

    public OfficeBuilder WithIsCentralFederalAdministration(bool isCentralFederalAdministration)
    {
        _isCentralFederalAdministration = isCentralFederalAdministration;
        return this;
    }

    public OfficeBuilder WithIsGeneralSecretariat(bool isGeneralSecretariat)
    {
        _isGeneralSecretariat = isGeneralSecretariat;
        return this;
    }

    public OfficeBuilder WithDepartment(Department department)
    {
        _department = department;
        _departmentId = department.Id;
        return this;
    }

    public OfficeBuilder WithDepartmentId(Guid departmentId)
    {
        _departmentId = departmentId;
        return this;
    }

    public Office Build()
    {
        return new Office
        {
            Id = _id,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            Department = _department,
            DepartmentId = _departmentId,
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
            IsCentralFederalAdministration = _isCentralFederalAdministration,
            IsGeneralSecretariat = _isGeneralSecretariat,
            EiamAssignmentId = _eiamAssignmentId
        };
    }
}
