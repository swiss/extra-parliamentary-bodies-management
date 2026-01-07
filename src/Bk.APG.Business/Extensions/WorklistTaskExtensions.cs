using Bk.APG.Business.Models;

namespace Bk.APG.Business.Extensions;

public static class WorklistTaskExtensions
{
    public static bool GetCanBeForwarded(this WorklistTask worklistTask, string currentExternalId)
    {
        return worklistTask.AssignedTo!.ExternalId == currentExternalId
               && worklistTask.WorklistTaskStateId == WorklistTaskState.Active
               && worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralElectionDispatch;
    }

    public static string? GetNavigationUrl(this WorklistTask worklistTask)
    {
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
            navigationUrl = $"/general-election/committees/{worklistTask.CommitteeId}";
        }

        return navigationUrl;
    }
}
