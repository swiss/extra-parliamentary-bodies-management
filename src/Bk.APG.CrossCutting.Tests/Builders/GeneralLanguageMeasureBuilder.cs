using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class GeneralLanguageMeasureBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private Guid _departmentId;
    private readonly Department _department;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private string _description;

    public GeneralLanguageMeasureBuilder()
    {
        _id = _faker.Random.Guid();
        _departmentId = Guid.Parse("ABE93902-C1E9-4140-8457-D681BF31B809");
        _department = new DepartmentBuilder().Build();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _description = _faker.Random.String();
    }

    public GeneralLanguageMeasureBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public GeneralLanguageMeasureBuilder WithDepartmentId(Guid departmentId)
    {
        _departmentId = departmentId;
        return this;
    }

    public GeneralLanguageMeasureBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public GeneralLanguageMeasure Build()
    {
        return new GeneralLanguageMeasure
        {
            Id = _id,
            DepartmentId = _departmentId,
            Department = _department,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            Description = _description
        };
    }
}
