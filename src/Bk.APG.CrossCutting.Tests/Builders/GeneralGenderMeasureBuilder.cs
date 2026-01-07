using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class GeneralGenderMeasureBuilder
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

    public GeneralGenderMeasureBuilder()
    {
        _id = _faker.Random.Guid();
        _departmentId = Guid.Parse("ACC1370E-9A87-413F-89EB-EA1FE7B3A724");
        _department = new DepartmentBuilder().Build();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _description = _faker.Random.String();
    }

    public GeneralGenderMeasureBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public GeneralGenderMeasureBuilder WithDepartmentId(Guid departmentId)
    {
        _departmentId = departmentId;
        return this;
    }

    public GeneralGenderMeasureBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public GeneralGenderMeasure Build()
    {
        return new GeneralGenderMeasure
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
