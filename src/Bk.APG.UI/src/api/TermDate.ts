export interface TermDate {
    id: string;
    text: string;
    description: string;
    beginDate: Date;
    endDate?: Date;
    isGeneralElection?: boolean;
}
