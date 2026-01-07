using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class MembershipCandidateLogMessageBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private Guid _generalElectionCommitteeId;
    private GeneralElectionCommittee _generalElectionCommittee;
    private Guid _personId;
    private Business.Models.Person _person;
    private string _logMessage;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;

    public MembershipCandidateLogMessageBuilder()
    {
        _id = _faker.Random.Guid();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _generalElectionCommitteeId = _faker.Random.Guid();
        _generalElectionCommittee = new GeneralElectionCommitteeBuilder().WithId(_generalElectionCommitteeId).Build();
        _personId = _faker.Random.Guid();
        _person = new PersonBuilder().WithId(_personId).Build();
        _logMessage = _faker.Random.String();
    }

    public MembershipCandidateLogMessageBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public MembershipCandidateLogMessageBuilder WithLogMessage(string logMessage)
    {
        _logMessage = logMessage;
        return this;
    }

    public MembershipCandidateLogMessageBuilder WithPersonId(Guid personId)
    {
        _personId = personId;
        return this;
    }

    public MembershipCandidateLogMessageBuilder WithPerson(Business.Models.Person person)
    {
        _person = person;
        _personId = person.Id;
        return this;
    }

    public MembershipCandidateLogMessageBuilder WithGeneralElectionCommitteeId(Guid generalElectionCommitteeId)
    {
        _generalElectionCommitteeId = generalElectionCommitteeId;
        return this;
    }

    public MembershipCandidateLogMessageBuilder WithGeneralElectionCommittee(GeneralElectionCommittee generalElectionCommittee)
    {
        _generalElectionCommittee = generalElectionCommittee;
        _generalElectionCommitteeId = generalElectionCommittee.Id;
        return this;
    }

    public MembershipCandidateLogMessage Build()
    {
        return new MembershipCandidateLogMessage
        {
            Id = _id,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            GeneralElectionCommitteeId = _generalElectionCommitteeId,
            GeneralElectionCommittee = _generalElectionCommittee,
            PersonId = _personId,
            Person = _person,
            LogMessage = _logMessage
        };
    }
}
