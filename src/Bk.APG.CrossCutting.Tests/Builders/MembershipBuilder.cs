using Bk.APG.Business.Models;
using Bogus;
using Person = Bk.APG.Business.Models.Person;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class MembershipBuilder
{
    private readonly Faker _faker = new();
    private Guid _id;
    private int _ogdId;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private Guid _personId;
    private Person? _person;
    private Guid _committeeId;
    private Committee? _committee;
    private int? _maximumEmploymentLevel;
    private DateOnly _beginDate;
    private DateOnly _endDate;
    private Guid _electionTypeId;
    private ElectionType? _electionType;
    private Guid _functionId;
    private Function? _function;
    private Guid _electionOfficeId;
    private ElectionOffice? _electionOffice;
    private Guid? _membershipAdditionId;
    private MembershipAddition? _membershipAddition;
    private string? _justificationLongerDuty;
    private string? _justificationShorterDuty;
    private string? _justificationMemberInFederalDuty;
    private string? _justificationMemberInFederalAssembly;
    private string? _requirementsProfile;
    private readonly string? _remarks;
    private readonly string? _remarksStatus;
    private bool _inCorrelationWithFederalDuty;
    private bool _isDeleted;
    private readonly int _oldId;

    public MembershipBuilder()
    {
        _id = _faker.Random.Guid();
        _ogdId = _faker.Random.Int();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _personId = _faker.Random.Guid();
        _committeeId = _faker.Random.Guid();
        _maximumEmploymentLevel = _faker.Random.Int().OrNull(_faker);
        _beginDate = _faker.Date.PastDateOnly();
        _endDate = _faker.Date.FutureDateOnly();
        _electionTypeId = _faker.Random.Guid();
        _electionType = new ElectionTypeBuilder().WithId(_electionTypeId).Build();
        _functionId = _faker.Random.Guid();
        _function = new FunctionBuilder().WithId(_functionId).Build();
        _electionOffice = new ElectionOfficeBuilder().WithId(_electionOfficeId).Build();
        _electionOfficeId = _faker.Random.Guid();
        _membershipAddition = new MembershipAdditionBuilder().WithId(_electionOfficeId).Build();
        _membershipAdditionId = _faker.Random.Guid();
        _justificationLongerDuty = _faker.Random.String().OrNull(_faker);
        _justificationShorterDuty = _faker.Random.String().OrNull(_faker);
        _justificationMemberInFederalDuty = _faker.Random.String().OrNull(_faker);
        _justificationMemberInFederalAssembly = _faker.Random.String().OrNull(_faker);
        _requirementsProfile = _faker.Random.String().OrNull(_faker);
        _remarks = _faker.Random.String().OrNull(_faker);
        _remarksStatus = _faker.Random.String().OrNull(_faker);
        _inCorrelationWithFederalDuty = false;
        _isDeleted = false;
        _oldId = _faker.Random.Int(1);
    }

    public MembershipBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public MembershipBuilder WithOgdId(int id)
    {
        _ogdId = id;
        return this;
    }

    public MembershipBuilder WithPerson(Person person)
    {
        _person = person;
        _personId = person.Id;
        return this;
    }

    public MembershipBuilder WithPersonId(Guid personId)
    {
        _personId = personId;
        return this;
    }

    public MembershipBuilder WithFunction(Function function)
    {
        _function = function;
        _functionId = function.Id;
        return this;
    }

    public MembershipBuilder WithFunctionId(Guid functionId)
    {
        _functionId = functionId;
        return this;
    }

    public MembershipBuilder WithBeginDate(DateOnly beginDate)
    {
        _beginDate = beginDate;
        return this;
    }

    public MembershipBuilder WithEndDate(DateOnly endDate)
    {
        _endDate = endDate;
        return this;
    }

    public MembershipBuilder WithMaximumEmploymentLevel(int maximumEmploymentLevel)
    {
        _maximumEmploymentLevel = maximumEmploymentLevel;
        return this;
    }

    public MembershipBuilder WithJustificationLongerDuty(string justificationLongerDuty)
    {
        _justificationLongerDuty = justificationLongerDuty;
        return this;
    }

    public MembershipBuilder WithJustificationShorterDuty(string justificationShorterDuty)
    {
        _justificationShorterDuty = justificationShorterDuty;
        return this;
    }

    public MembershipBuilder WithJustificationMemberInFederalDuty(string justificationMemberInFederalDuty)
    {
        _justificationMemberInFederalDuty = justificationMemberInFederalDuty;
        return this;
    }

    public MembershipBuilder WithJustificationMemberInFederalAssembly(string justificationMemberInFederalAssembly)
    {
        _justificationMemberInFederalAssembly = justificationMemberInFederalAssembly;
        return this;
    }

    public MembershipBuilder WithRequirementsProfile(string requirementsProfile)
    {
        _requirementsProfile = requirementsProfile;
        return this;
    }

    public MembershipBuilder WithIsActive(bool isActive)
    {
        _beginDate = isActive ? DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) : DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        _endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        return this;
    }

    public MembershipBuilder WithFemalePresident()
    {
        _function = new FunctionBuilder()
            .WithUri(Function.PresidentUri)
            .Build();
        _person = new PersonBuilder()
            .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
            .Build();
        return this;
    }

    public MembershipBuilder WithMalePresident()
    {
        _function = new FunctionBuilder()
            .WithUri(Function.PresidentUri)
            .Build();
        _person = new PersonBuilder()
            .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
            .Build();
        return this;
    }

    public MembershipBuilder WithFemaleMember()
    {
        _person = new PersonBuilder()
            .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
            .Build();
        return this;
    }

    public MembershipBuilder WithMaleMember()
    {
        _person = new PersonBuilder()
            .WithGender(new GenderBuilder().WithUri(Gender.Male).Build())
            .Build();
        return this;
    }

    public MembershipBuilder WithEmploymentLevel(int level)
    {
        _maximumEmploymentLevel = level;
        return this;
    }

    public MembershipBuilder WithCommitteeId(Guid id)
    {
        _committeeId = id;
        return this;
    }

    public MembershipBuilder WithCommittee(Committee committee)
    {
        _committee = committee;
        _committeeId = committee.Id;
        return this;
    }

    public MembershipBuilder WithMembershipAddition(MembershipAddition membershipAddition)
    {
        _membershipAddition = membershipAddition;
        _membershipAdditionId = membershipAddition.Id;
        return this;
    }

    public MembershipBuilder WithElectionOffice(ElectionOffice electionOffice)
    {
        _electionOffice = electionOffice;
        _electionOfficeId = electionOffice.Id;
        return this;
    }

    public MembershipBuilder WithIsDeleted(bool isDeleted)
    {
        _isDeleted = isDeleted;
        return this;
    }

    public MembershipBuilder WithInCorrelationWithFederalDuty(bool inCorrelationWithFederalDuty)
    {
        _inCorrelationWithFederalDuty = inCorrelationWithFederalDuty;
        return this;
    }

    public MembershipBuilder WithElectionType(ElectionType electionType)
    {
        _electionType = electionType;
        _electionTypeId = electionType.Id;
        return this;
    }

    public Membership Build()
    {
        return new Membership
        {
            Id = _id,
            OgdId = _ogdId,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            PersonId = _personId,
            Person = _person,
            CommitteeId = _committeeId,
            Committee = _committee,
            MaximumEmploymentLevel = _maximumEmploymentLevel,
            BeginDate = _beginDate,
            EndDate = _endDate,
            ElectionTypeId = _electionTypeId,
            ElectionType = _electionType,
            FunctionId = _functionId,
            Function = _function,
            ElectionOfficeId = _electionOfficeId,
            ElectionOffice = _electionOffice,
            MembershipAdditionId = _membershipAdditionId,
            MembershipAddition = _membershipAddition,
            JustificationLongerDuty = _justificationLongerDuty,
            JustificationShorterDuty = _justificationShorterDuty,
            JustificationMemberInFederalDuty = _justificationMemberInFederalDuty,
            JustificationMemberInFederalAssembly = _justificationMemberInFederalAssembly,
            RequirementsProfile = _requirementsProfile,
            Remarks = _remarks,
            RemarksStatus = _remarksStatus,
            IsDeleted = _isDeleted,
            InCorrelationWithFederalDuty = _inCorrelationWithFederalDuty,
            OldId = _oldId
        };
    }
}
