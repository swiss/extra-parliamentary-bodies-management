export interface MembershipCandidateDetail {
    id: string;
    personId?: string;
    surname: string;
    givenName: string;
    gender: string;
    language: string;
    birthYear: number;
    function: string;
    functionId: string;
    beginDate: Date;
    endDate?: Date;
    electionType: string;
    electionTypeId: string;
    membershipAddition: string;
    remarks?: string;
    remarksStatus?: string;
    isSelected: boolean;
    estimatedTermOfOffice: number;
}
