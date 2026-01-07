export interface CommitteeMember {
    id: string;
    surname: string;
    givenName: string;
    gender: string;
    language: string;
    function: string;
    employmentLevel: string;
    beginDate: Date;
    endDate: Date;
    electionType: string;
    hasMembershipAddition: boolean;
    isActive: boolean;
    isFuture: boolean;
    needsAttention: boolean;
}
