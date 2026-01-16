using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class InterestBuilder
{
    private readonly Faker _faker = new();
    private Guid _id;
    private int _ogdId;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private Guid _personId;
    private Business.Models.Person? _person;
    private string? _text;
    private string? _interestText;
    private InterestCommittee? _interestCommittee;
    private Guid _interestCommitteeId;
    private InterestFunction? _interestFunction;
    private Guid _interestFunctionId;
    private InterestLegalForm? _interestLegalForm;
    private Guid _interestLegalFormId;
    private LegalForm? _legalForm;
    private Guid _legalFormId;
    private DateOnly? _beginDate;
    private DateOnly? _endDate;
    private string _uidOrganisationId;
    private readonly bool _isDeleted;
    private readonly uint _rowVersion;
    private readonly int _oldId;
    private bool? _verifiedSuccessfully;

    public InterestBuilder()
    {
        _id = _faker.Random.Guid();
        _ogdId = _faker.Random.Int();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _personId = _faker.Random.Guid();
        _text = _faker.Lorem.Text().OrNull(_faker);
        _interestText = _faker.Lorem.Text().OrNull(_faker);
        _beginDate = _faker.Date.PastDateOnly();
        _endDate = _faker.Date.FutureDateOnly();
        _uidOrganisationId = _faker.Random.String();
        _isDeleted = false;
        _rowVersion = _faker.Random.UInt();
        _oldId = _faker.Random.Int(1);
        _verifiedSuccessfully = false;
    }

    public InterestBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public InterestBuilder WithOgdId(int ogdId)
    {
        _ogdId = ogdId;
        return this;
    }

    public InterestBuilder WithPerson(Business.Models.Person person)
    {
        _person = person;
        _personId = person.Id;
        return this;
    }

    public InterestBuilder WithPersonId(Guid personId)
    {
        _personId = personId;
        return this;
    }

    public InterestBuilder WithText(string? text)
    {
        _text = text;
        return this;
    }

    public InterestBuilder WithInterestText(string? interestText)
    {
        _interestText = interestText;
        return this;
    }

    public InterestBuilder WithInterestCommittee(InterestCommittee interestCommittee)
    {
        _interestCommittee = interestCommittee;
        _interestCommitteeId = interestCommittee.Id;
        return this;
    }

    public InterestBuilder WithInterestCommitteeId(Guid interestCommitteeId)
    {
        _interestCommitteeId = interestCommitteeId;
        return this;
    }

    public InterestBuilder WithInterestLegalForm(InterestLegalForm interestLegalForm)
    {
        _interestLegalForm = interestLegalForm;
        return this;
    }

    public InterestBuilder WithInterestLegalFormId(Guid interestLegalFormId)
    {
        _interestLegalFormId = interestLegalFormId;
        return this;
    }

    public InterestBuilder WithInterestFunction(InterestFunction interestFunction)
    {
        _interestFunction = interestFunction;
        _interestFunctionId = interestFunction.Id;
        return this;
    }

    public InterestBuilder WithInterestFunctionId(Guid interestFunctionId)
    {
        _interestFunctionId = interestFunctionId;
        return this;
    }

    public InterestBuilder WithLegalForm(LegalForm legalForm)
    {
        _legalForm = legalForm;
        return this;
    }

    public InterestBuilder WithLegalFormId(Guid legalFormId)
    {
        _legalFormId = legalFormId;
        return this;
    }

    public InterestBuilder WithVerification(bool? verified)
    {
        _verifiedSuccessfully = verified;
        return this;
    }

    public InterestBuilder WithBeginDate(DateOnly? beginDate)
    {
        _beginDate = beginDate;
        return this;
    }

    public InterestBuilder WithEndDate(DateOnly? endDate)
    {
        _endDate = endDate;
        return this;
    }

    public InterestBuilder WithUidOrganisationId(string uidOrganisationId)
    {
        _uidOrganisationId = uidOrganisationId;
        return this;
    }

    public Interest Build()
    {
        return new Interest
        {
            Id = _id,
            OgdId = _ogdId,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            PersonId = _personId,
            Person = _person,
            Text = _text,
            InterestText = _interestText,
            InterestCommittee = _interestCommittee,
            InterestLegalForm = _interestLegalForm,
            InterestFunction = _interestFunction,
            LegalForm = _legalForm,
            InterestCommitteeId = _interestCommitteeId,
            InterestLegalFormId = _interestLegalFormId,
            InterestFunctionId = _interestFunctionId,
            LegalFormId = _legalFormId,
            BeginDate = _beginDate,
            EndDate = _endDate,
            UidOrganisationId = _uidOrganisationId,
            IsDeleted = _isDeleted,
            RowVersion = _rowVersion,
            OldId = _oldId,
            VerifiedSuccessfully = _verifiedSuccessfully
        };
    }
}
