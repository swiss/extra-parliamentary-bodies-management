export interface CommitteeMembershipValidationRequest {
    committeeid: string;
    personId: string;
    currentMembershipId?: string;
    beginDate: Date;
    endDate: Date;
    inCorrelationWithFederalDuty: boolean;
    isUpdateMode: boolean;
}
