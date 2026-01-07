import {CommitteeMember} from '@api/CommitteeMember';
import {CommitteeQuotas} from '@api/CommitteeQuotas';

export interface GeneralElectionMembershipList {
    committeeQuotas: CommitteeQuotas;
    memberships: CommitteeMember[];
}
