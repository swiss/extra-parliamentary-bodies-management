using Bk.APG.Business.Models;
using Bk.APG.Business.Services;

namespace Bk.APG.Business.Extensions;

public static class EiamAssignmentExtensions
{
    public static IEnumerable<Guid> GetSearchableIds(this EiamAssignment eiamAssignment)
    {
        ArgumentNullException.ThrowIfNull(eiamAssignment);

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
        ArgumentNullException.ThrowIfNull(eiamAssignment);

        var assignableIds = new List<EiamAssignment>();

        if (eiamAssignment.Parent is not null)
        {
            assignableIds.Add(eiamAssignment.Parent!);
        }

        assignableIds.AddRange(eiamAssignment.Children);

        return assignableIds;
    }

    public static IEnumerable<EiamAssignment> GetAssignmentsForCandidateListForward(this EiamAssignment eiamAssignment, Guid committeeId, bool isAssignedToSecretariat)
    {
        ArgumentNullException.ThrowIfNull(eiamAssignment);

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

                if (!isAssignedToSecretariat)
                {
                    assignableIds.Add(eiamAssignment.Children.First(secretariatAssignment => secretariatAssignment.CommitteeId == committeeId));
                }
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

    public static IEnumerable<EiamAssignment> GetAssignmentsForReadyForProposalForward(this EiamAssignment eiamAssignment, Guid committeeId, IEnumerable<WorklistTask> worklistTasks)
    {
        ArgumentNullException.ThrowIfNull(eiamAssignment);

        var assignableIds = new List<EiamAssignment>();
        switch (eiamAssignment.Role)
        {
            case Role.Admin:
                if (WorklistTaskService.HasCompletedReadyForProposal(worklistTasks, Role.Department))
                {
                    assignableIds.Add(eiamAssignment.Children
                    .First(departmentAssignment => departmentAssignment.Children.Any(officeAssignment => officeAssignment.Children.Any(secretariatAssignment => secretariatAssignment.CommitteeId == committeeId))));
                }
                break;
            case Role.Department:
                assignableIds.Add(eiamAssignment.Parent!);
                if (eiamAssignment.Department!.IsBigDepartment)
                {
                    if (WorklistTaskService.HasCompletedReadyForProposal(worklistTasks, Role.Office))
                    {
                        assignableIds.Add(eiamAssignment.Children.First(officeAssignment => officeAssignment.Children.Any(secretariatAssignment => secretariatAssignment.CommitteeId == committeeId)));
                    }
                }
                else
                {
                    if (WorklistTaskService.HasCompletedReadyForProposal(worklistTasks, Role.Secretariat))
                    {
                        assignableIds.Add(eiamAssignment.Children
                            .SelectMany(officeAssignment => officeAssignment.Children)
                            .First(secretariatAssignment => secretariatAssignment.CommitteeId == committeeId));
                    }
                }
                break;
            case Role.Office:
                assignableIds.Add(eiamAssignment.Parent!);
                if (WorklistTaskService.HasCompletedReadyForProposal(worklistTasks, Role.Secretariat))
                {
                    assignableIds.Add(eiamAssignment.Children.First(secretariatAssignment => secretariatAssignment.CommitteeId == committeeId));
                }
                break;
            case Role.Secretariat:
                assignableIds.Add(eiamAssignment.Committee!.Department!.IsBigDepartment ? eiamAssignment.Parent! : eiamAssignment.Parent!.Parent!);
                break;
            case Role.Observer:
            default:
                throw new ArgumentOutOfRangeException($"Role {eiamAssignment.Role} not supported");
        }

        return assignableIds;
    }
}
