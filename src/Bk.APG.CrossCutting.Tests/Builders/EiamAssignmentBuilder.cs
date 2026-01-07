using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class EiamAssignmentBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private string _externalId;
    private Role _role;
    private Committee? _committee;
    private Guid _committeeId;
    private Department? _department;
    private readonly List<EiamAssignment> _children;
    private readonly Guid? _departmentId;

    public EiamAssignmentBuilder()
    {
        _id = _faker.Random.Guid();
        _externalId = _faker.Random.String();
        _role = _faker.PickRandom<Role>();
        _committee = null;
        _committeeId = _faker.Random.Guid();
        _department = null;
        _children = [];
        _departmentId = _faker.Random.Guid();
    }

    public EiamAssignmentBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public EiamAssignmentBuilder WithExternalId(string externalId)
    {
        _externalId = externalId;
        return this;
    }

    public EiamAssignmentBuilder WithRole(Role role)
    {
        _role = role;
        return this;
    }

    public EiamAssignmentBuilder WithCommittee(Committee committee)
    {
        _committee = committee;
        _committeeId = committee.Id;
        return this;
    }

    public EiamAssignmentBuilder WithDepartment(Department department)
    {
        _department = department;
        return this;
    }

    public EiamAssignmentBuilder WithChildren(IList<EiamAssignment> eiamAssignments)
    {
        _children.AddRange(eiamAssignments);
        return this;
    }

    public EiamAssignment Build()
    {
        return new EiamAssignment
        {
            Id = _id,
            ExternalId = _externalId,
            Role = _role,
            Committee = _committee,
            CommitteeId = _committeeId,
            Department = _department,
            Children = _children,
            DepartmentId = _departmentId
        };
    }
}
