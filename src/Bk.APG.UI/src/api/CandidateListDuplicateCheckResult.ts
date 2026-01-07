import {DuplicateReason} from './DuplicateReason';
import {MembershipCandidateDetail} from './MembershipCandidateDetail';
import {PersonDetails} from './PersonDetails';

export interface CandidateListDuplicateCheckResult {
    membershipCandidateToCheck: MembershipCandidateDetail;
    personFound: PersonDetails;
    duplicateReason: DuplicateReason;
}
