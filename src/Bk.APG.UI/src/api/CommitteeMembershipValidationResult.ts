export interface CommitteeMembershipValidationResult {
    committeeid: string;
    personId: string;
    hasErrors: boolean;
    tooManyMembers: boolean;
    isAlreadyActiveMember: boolean;
    maximumDurationExceeded: boolean;
    currentTermOfOffice: number;
    estimatedTermOfOffice: number;
    isFederalAssemblyAndAuthoritiesCommission: boolean;
}
