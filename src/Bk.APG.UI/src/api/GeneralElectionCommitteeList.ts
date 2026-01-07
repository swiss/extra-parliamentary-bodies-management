export interface GeneralElectionCommitteeList {
    id: string;
    committeeId: string;
    description: string;
    department: string;
    office: string;
    committeeType: string;
    isMarketOrientated: string;
    hasSupervisionDuty: boolean;
    status: string;
    vacanciesGeneralElection: number;
    statusProposal: string;
    modified: Date;
    modifiedBy: string;
}
