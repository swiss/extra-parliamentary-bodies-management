using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class ApgGeneralSettingsBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private bool _isOgdExportActivated;

    public ApgGeneralSettingsBuilder()
    {
        _id = _faker.Random.Guid();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _isOgdExportActivated = false;
    }

    public ApgGeneralSettingsBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ApgGeneralSettingsBuilder WithIsOgdExportActivated(bool enabled)
    {
        _isOgdExportActivated = enabled;
        return this;
    }

    public ApgGeneralSettings Build()
    {
        return new ApgGeneralSettings
        {
            Id = _id,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            IsOgdExportActivated = _isOgdExportActivated
        };
    }
}
