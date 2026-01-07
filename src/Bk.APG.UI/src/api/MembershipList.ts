import {CommitteeMember} from '@api/CommitteeMember';
import {CommitteeQuotas} from '@api/CommitteeQuotas';

export interface MembershipList {
    committeeQuotas: CommitteeQuotas;
    activeMemberships: CommitteeMember[];
    inactiveMemberships: CommitteeMember[];
}
