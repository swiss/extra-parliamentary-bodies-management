using Bk.APG.Business.Models;

namespace Bk.APG.Business.Extensions;

public static class WorklistTaskExtensions
{
    public static bool GetCanBeForwarded(this WorklistTask worklistTask, Guid currentEiamAssignmentId)
    {
        ArgumentNullException.ThrowIfNull(worklistTask);

        return worklistTask.AssignedTo!.Id == currentEiamAssignmentId
            && worklistTask.WorklistTaskStateId == WorklistTaskState.Active
            && worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralElectionDispatch;
    }

    public static string GetSection(this WorklistTask worklistTask)
    {
        ArgumentNullException.ThrowIfNull(worklistTask);

        var section = string.Empty;

        if (worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonBaseData ||
            worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonInterests ||
            worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMembershipValidation)
        {
            section += $"{worklistTask.Person!.GivenName} {worklistTask.Person!.Surname}; ";
        }

        return section + worklistTask.Committee?.GetDescription();
    }

    public static string? GetNavigationUrl(this WorklistTask worklistTask)
    {
        ArgumentNullException.ThrowIfNull(worklistTask);

        string? navigationUrl = null;

        if (worklistTask.WorklistTaskTypeId == WorklistTaskType.CandidateListCreate ||
            worklistTask.WorklistTaskTypeId == WorklistTaskType.CandidateListForward ||
            worklistTask.WorklistTaskTypeId == WorklistTaskType.CandidateListApprove)
        {
            navigationUrl = $"/general-election/committees/{worklistTask.CommitteeId}?tab=candidateList";
        }
        else if (worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingJustifications)
        {
            navigationUrl = $"/general-election/committees/{worklistTask.CommitteeId}?tab=justifications";
        }
        else if (worklistTask.WorklistTaskTypeId == WorklistTaskType.ReadyForFederalCouncilProposal)
        {
            navigationUrl = $"/general-election/committees/{worklistTask.CommitteeId}?tab=data";
        }
        else if (worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonInterests)
        {
            navigationUrl = $"/persons/{worklistTask.PersonId}?tab=interests";
        }
        else if (worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonBaseData)
        {
            navigationUrl = $"/persons/{worklistTask.PersonId}?tab=data";
        }
        else if (worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMembershipValidation)
        {
            navigationUrl = $"/general-election/committees/{worklistTask.CommitteeId}/membership-candidate/{worklistTask.MembershipCandidateId}";
        }
        else if (worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingSecretariat ||
                 worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralElectionMissingDataProtectionOfficer)
        {
            navigationUrl = $"/committees/{worklistTask.CommitteeId}?tab=contacts";
        }
        else if (worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralMeasureCheck ||
                 worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralMeasureValidate)
        {
            navigationUrl = "/administration/generalMeasures";
        }

        return navigationUrl;
    }
}
