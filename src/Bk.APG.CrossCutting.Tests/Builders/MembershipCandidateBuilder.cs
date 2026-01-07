using Bk.APG.Business.Models;
using Bogus;
using Person = Bk.APG.Business.Models.Person;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class MembershipCandidateBuilder
{
    private readonly Faker _faker = new();
    private Guid _id;
    private Guid _generalElectionCommitteeId;
    private GeneralElectionCommittee? _generalElectionCommittee;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private Guid _membershipId;
    private Membership? _membership;
    private string _surname;
    private string _givenName;
    private int _birthYear;
    private Guid _languageId;
    private Language? _language;
    private Guid _genderId;
    private Gender? _gender;
    private Guid _personId;
    private Person? _person;
    private readonly int? _maximumEmploymentLevel;
    private DateOnly _beginDate;
    private DateOnly _endDate;
    private Guid _electionTypeId;
    private readonly ElectionType? _electionType;
    private Guid _functionId;
    private Function? _function;
    private readonly Guid _electionOfficeId;
    private readonly ElectionOffice? _electionOffice;
    private readonly Guid _membershipAdditionId;
    private readonly MembershipAddition? _membershipAddition;
    private readonly string? _justificationLongerDuty;
    private readonly string? _justificationShorterDuty;
    private readonly string? _justificationMemberInFederalDuty;
    private readonly string? _justificationMemberInFederalAssembly;
    private readonly string? _requirementsProfile;
    private readonly string? _remarks;
    private readonly string? _remarksStatus;
    private readonly bool _inCorrelationWithFederalDuty;
    private bool _isDeleted;
    private bool _isSelected;

    public MembershipCandidateBuilder()
    {
        _id = _faker.Random.Guid();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _generalElectionCommittee = new GeneralElectionCommitteeBuilder().Build();
        _generalElectionCommitteeId = _generalElectionCommittee.Id;
        _membership = new MembershipBuilder().WithId(_membershipId).Build();
        _membershipId = _faker.Random.Guid();
        _surname = _faker.Person.LastName;
        _givenName = _faker.Person.FirstName;
        _birthYear = _faker.Person.DateOfBirth.Year;
        _languageId = _faker.Random.Guid();
        _genderId = _faker.Random.Guid();
        _gender = new GenderBuilder().WithId(_genderId).Build();
        _personId = _faker.Random.Guid();
        _maximumEmploymentLevel = _faker.Random.Int().OrNull(_faker);
        _beginDate = _faker.Date.PastDateOnly();
        _endDate = _faker.Date.FutureDateOnly();
        _electionTypeId = _faker.Random.Guid();
        _electionType = new ElectionTypeBuilder().WithId(_electionTypeId).Build();
        _functionId = _faker.Random.Guid();
        _function = new FunctionBuilder().WithId(_functionId).Build();
        _languageId = _faker.Random.Guid();
        _language = new LanguageBuilder().WithId(_languageId).Build();
        _electionOffice = new ElectionOfficeBuilder().WithId(_electionOfficeId).Build();
        _electionOfficeId = _faker.Random.Guid();
        _membershipAddition = new MembershipAdditionBuilder().WithId(_membershipAdditionId).Build();
        _membershipAdditionId = _faker.Random.Guid();
        _justificationLongerDuty = _faker.Random.String().OrNull(_faker);
        _justificationShorterDuty = _faker.Random.String().OrNull(_faker);
        _justificationMemberInFederalDuty = _faker.Random.String().OrNull(_faker);
        _justificationMemberInFederalAssembly = _faker.Random.String().OrNull(_faker);
        _requirementsProfile = _faker.Random.String().OrNull(_faker);
        _remarks = _faker.Random.String().OrNull(_faker);
        _remarksStatus = _faker.Random.String().OrNull(_faker);
        _isDeleted = false;
        _isSelected = false;
        _inCorrelationWithFederalDuty = false;
    }

    public MembershipCandidateBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public MembershipCandidateBuilder WithMembership(Membership membership)
    {
        _membership = membership;
        _membershipId = membership.Id;
        return this;
    }

    public MembershipCandidateBuilder WithLanguageId(Guid id)
    {
        _languageId = id;
        return this;
    }

    public MembershipCandidateBuilder WithLanguage(Language language)
    {
        _language = language;
        _languageId = language.Id;
        return this;
    }

    public MembershipCandidateBuilder WithGenderId(Guid genderId)
    {
        _genderId = genderId;
        return this;
    }

    public MembershipCandidateBuilder WithGender(Gender gender)
    {
        _gender = gender;
        _genderId = gender.Id;
        return this;
    }

    public MembershipCandidateBuilder WithPersonId(Guid personId)
    {
        _personId = personId;
        return this;
    }

    public MembershipCandidateBuilder WithPerson(Person person)
    {
        _person = person;
        _personId = person.Id;
        return this;
    }

    public MembershipCandidateBuilder WithSurname(string surname)
    {
        _surname = surname;
        return this;
    }

    public MembershipCandidateBuilder WithGivenName(string givenName)
    {
        _givenName = givenName;
        return this;
    }

    public MembershipCandidateBuilder WithBirthYear(int year)
    {
        _birthYear = year;
        return this;
    }

    public MembershipCandidateBuilder WithFunction(Function function)
    {
        _function = function;
        _functionId = function.Id;
        return this;
    }

    public MembershipCandidateBuilder WithFunctionId(Guid functionId)
    {
        _functionId = functionId;
        return this;
    }

    public MembershipCandidateBuilder WithBeginDate(DateOnly beginDate)
    {
        _beginDate = beginDate;
        return this;
    }

    public MembershipCandidateBuilder WithEndDate(DateOnly endDate)
    {
        _endDate = endDate;
        return this;
    }

    public MembershipCandidateBuilder WithIsActive(bool isActive)
    {
        _beginDate = isActive ? DateOnly.FromDateTime(DateTime.Now.AddDays(-1)) : DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        _endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
        return this;
    }

    public MembershipCandidateBuilder WithIsSelected(bool isSelected)
    {
        _isSelected = isSelected;
        return this;
    }

    public MembershipCandidateBuilder WithIsDeleted(bool isDeleted)
    {
        _isDeleted = isDeleted;
        return this;
    }

    public MembershipCandidateBuilder WithElectionTypeId(Guid electionTypeId)
    {
        _electionTypeId = electionTypeId;
        return this;
    }

    public MembershipCandidateBuilder WithGeneralElectionCommittee(GeneralElectionCommittee committee)
    {
        _generalElectionCommittee = committee;
        _generalElectionCommitteeId = committee.Id;
        return this;
    }

    public MembershipCandidate Build()
    {
        return new MembershipCandidate
        {
            Id = _id,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            GeneralElectionCommitteeId = _generalElectionCommitteeId,
            GeneralElectionCommittee = _generalElectionCommittee,
            MembershipId = _membershipId,
            Membership = _membership,
            Surname = _surname,
            GivenName = _givenName,
            BirthYear = _birthYear,
            LanguageId = _languageId,
            Language = _language,
            GenderId = _genderId,
            Gender = _gender,
            PersonId = _personId,
            Person = _person,
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
            IsSelected = _isSelected
        };
    }
}
