using Bk.APG.Business.Models;

namespace Bk.APG.Business.Extensions;

public static class EiamAssignmentExtensions
{
    public static IEnumerable<Guid> GetSearchableIds(this EiamAssignment eiamAssignment)
    {
        var searchableIds = new List<Guid> { eiamAssignment.Id };

        switch (eiamAssignment.Role)
        {
            case Role.Department:
                searchableIds.AddRange(eiamAssignment.Children.Select(officeAssignment => officeAssignment.Id));
                searchableIds.AddRange(eiamAssignment.Children.SelectMany(committeeAssignment => committeeAssignment.Children.Select(cc => cc.Id)));
                break;
            case Role.Office:
                searchableIds.AddRange(eiamAssignment.Children.Select(committeeAssignment => committeeAssignment.Id));
                break;
            case Role.Secretariat:
            case Role.Admin:
            case Role.Observer:
            default:
                break;
        }

        return searchableIds;
    }

    public static IEnumerable<EiamAssignment> GetAssignableIds(this EiamAssignment eiamAssignment)
    {
        var assignableIds = new List<EiamAssignment>();

        if (eiamAssignment.Parent is not null)
        {
            assignableIds.Add(eiamAssignment.Parent!);
        }

        assignableIds.AddRange(eiamAssignment.Children);

        return assignableIds;
    }

    public static IEnumerable<EiamAssignment> GetAssignmentsForCandidateListForward(this EiamAssignment eiamAssignment, Guid committeeId)
    {
        var assignableIds = new List<EiamAssignment>();
        switch (eiamAssignment.Role)
        {
            case Role.Department:
                assignableIds.Add(eiamAssignment.Department!.IsBigDepartment
                    ? eiamAssignment.Children.First(officeAssignment => officeAssignment.Children.Any(secretariatAssignment => secretariatAssignment.CommitteeId == committeeId))
                    : eiamAssignment.Children
                        .SelectMany(officeAssignment => officeAssignment.Children)
                        .First(secretariatAssignment => secretariatAssignment.CommitteeId == committeeId));
                break;
            case Role.Office:
                assignableIds.Add(eiamAssignment.Parent!);
                assignableIds.Add(eiamAssignment.Children.First(secretariatAssignment => secretariatAssignment.CommitteeId == committeeId));
                break;
            case Role.Secretariat:
                assignableIds.Add(eiamAssignment.Committee!.Department!.IsBigDepartment ? eiamAssignment.Parent! : eiamAssignment.Parent!.Parent!);
                break;
            case Role.Admin:
            case Role.Observer:
            default:
                throw new ArgumentOutOfRangeException($"Role {eiamAssignment.Role} not supported");
        }

        return assignableIds;
    }
}
