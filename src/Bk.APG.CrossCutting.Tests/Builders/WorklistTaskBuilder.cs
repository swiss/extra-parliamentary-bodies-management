using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class WorklistTaskBuilder
{
    private readonly Faker _faker = new();
    private Guid _id;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private Guid? _parentTaskId;
    private WorklistTask? _parentTask;
    private Guid _assignedToId;
    private EiamAssignment _assignedTo;
    private readonly Guid _assignedById;
    private readonly EiamAssignment _assignedBy;
    private readonly DateOnly _dueDate;
    private Guid _worklistTaskTypeId;
    private WorklistTaskType? _worklistTaskType;
    private Guid _worklistTaskStateId;
    private readonly WorklistTaskState? _worklistTaskState;
    private Guid? _departmentId;
    private Department? _department;
    private Guid? _officeId;
    private Office? _office;
    private Guid? _committeeId;
    private readonly Guid? _membershipId;
    private readonly Guid? _personId;
    private Guid? _generalElectionCommitteeId;
    private GeneralElectionCommittee? _generalElectionCommittee;
    private readonly Guid? _membershipCandidateId;
    private readonly string _description;
    private readonly Guid? _termOfOfficeDateId;

    public WorklistTaskBuilder()
    {
        _description = _faker.Lorem.Sentence();
        _id = _faker.Random.Guid();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _parentTaskId = _faker.Random.Guid().OrNull(_faker);
        _assignedToId = _faker.Random.Guid();
        _assignedTo = new EiamAssignmentBuilder().WithId(_assignedToId).Build();
        _assignedById = _faker.Random.Guid();
        _assignedBy = new EiamAssignmentBuilder().WithId(_assignedById).Build();
        _dueDate = _faker.Date.FutureDateOnly();
        _worklistTaskTypeId = _faker.Random.Guid();
        _worklistTaskType = new WorklistTaskTypeBuilder().WithId(_worklistTaskTypeId).Build();
        _worklistTaskStateId = _faker.Random.Guid();
        _worklistTaskState = new WorklistTaskStateBuilder().WithId(_worklistTaskStateId).Build();
        _committeeId = _faker.Random.Guid().OrNull(_faker);
        _membershipId = _faker.Random.Guid().OrNull(_faker);
        _personId = _faker.Random.Guid().OrNull(_faker);
        _generalElectionCommitteeId = _faker.Random.Guid().OrNull(_faker);
        _membershipCandidateId = _faker.Random.Guid().OrNull(_faker);
        _termOfOfficeDateId = _faker.Random.Guid().OrNull(_faker);
        _departmentId = _faker.Random.Guid().OrNull(_faker);
        _officeId = _faker.Random.Guid().OrNull(_faker);
    }

    public WorklistTaskBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public WorklistTaskBuilder WithParentWorklistTask(WorklistTask parentTask)
    {
        _parentTask = parentTask;
        _parentTaskId = parentTask.Id;
        return this;
    }

    public WorklistTaskBuilder WithWorklistTaskTypeId(Guid worklistTaskTypeId)
    {
        _worklistTaskTypeId = worklistTaskTypeId;
        return this;
    }

    public WorklistTaskBuilder WithWorklistTaskType(WorklistTaskType worklistTaskType)
    {
        _worklistTaskType = worklistTaskType;
        _worklistTaskTypeId = worklistTaskType.Id;
        return this;
    }

    public WorklistTaskBuilder WithWorklistTaskStateId(Guid worklistTaskStateId)
    {
        _worklistTaskStateId = worklistTaskStateId;
        return this;
    }

    public WorklistTaskBuilder WithCommitteeId(Guid membershipId)
    {
        _committeeId = membershipId;
        return this;
    }

    public WorklistTaskBuilder WithGeneralElectionCommittee(GeneralElectionCommittee generalElectionCommittee)
    {
        _generalElectionCommittee = generalElectionCommittee;
        _generalElectionCommitteeId = generalElectionCommittee.Id;
        return this;
    }

    public WorklistTaskBuilder WithGeneralElectionCommitteeId(Guid generalElectionCommitteeId)
    {
        _generalElectionCommitteeId = generalElectionCommitteeId;
        return this;
    }

    public WorklistTaskBuilder WithAssignedTo(EiamAssignment assignedTo)
    {
        _assignedTo = assignedTo;
        _assignedToId = assignedTo.Id;
        return this;
    }

    public WorklistTaskBuilder WithDepartment(Department department)
    {
        _department = department;
        _departmentId = department.Id;
        return this;
    }

    public WorklistTaskBuilder WithOffice(Office office)
    {
        _office = office;
        _officeId = office.Id;
        return this;
    }

    public WorklistTask Build()
    {
        return new WorklistTask
        {
            Id = _id,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            AssignedToId = _assignedToId,
            AssignedTo = _assignedTo,
            AssignedById = _assignedById,
            AssignedBy = _assignedBy,
            ParentTask = _parentTask,
            ParentTaskId = _parentTaskId,
            DueDate = _dueDate,
            PersonId = _personId,
            Person = new PersonBuilder().Build(),
            CommitteeId = _committeeId,
            Committee = new CommitteeBuilder().Build(),
            MembershipId = _membershipId,
            Membership = new MembershipBuilder().Build(),
            WorklistTaskTypeId = _worklistTaskTypeId,
            WorklistTaskType = _worklistTaskType,
            WorklistTaskState = _worklistTaskState,
            WorklistTaskStateId = _worklistTaskStateId,
            GeneralElectionCommitteeId = _generalElectionCommitteeId,
            GeneralElectionCommittee = _generalElectionCommittee,
            MembershipCandidateId = _membershipCandidateId,
            MembershipCandidate = new MembershipCandidateBuilder().Build(),
            Description = _description,
            TermOfOfficeDate = new TermOfOfficeDateBuilder().Build(),
            TermOfOfficeDateId = _termOfOfficeDateId,
            DepartmentId = _departmentId,
            Department = _department,
            OfficeId = _officeId,
            Office = _office
        };
    }
}
