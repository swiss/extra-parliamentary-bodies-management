export interface PersonMembership {
    id: string;
    committee: string;
    department: string;
    function: string;
    beginDate: Date;
    endDate: Date;
    electionType: string;
    isActive: boolean;
    needsAttention: boolean;
}
