export interface InterestUpdate {
    id: string;
    personId: string;
    text?: string;
    interestText: string;
    interestLegalFormId?: string;
    interestCommitteeId: string;
    interestFunctionId?: string;
    legalFormId: string;
    uidOrganisationId: string;
    beginDate?: Date;
    endDate?: Date;
    rowVersion: number;
    isInactive: boolean;
    isUid: boolean;
}
