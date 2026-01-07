import {MembershipAddition} from './MembershipAddition';

export interface MembershipUpdate {
    id: string;
    personId: string;
    committeeId: string;
    maximumEmploymentLevel?: number;
    beginDate: Date;
    endDate?: Date;
    electionTypeId: string;
    functionName: string;
    functionId: string;
    electionOfficeName: string;
    electionOfficeId: string;
    membershipAdditionId?: string;
    membershipAddition?: MembershipAddition;
    justificationLongerDuty?: string;
    justificationShorterDuty?: string;
    justificationMemberInFederalDuty?: string;
    justificationMemberInFederalAssembly?: string;
    requirementsProfile?: string;
    remarks?: string;
    remarksStatus?: string;
    inCorrelationWithFederalDuty: boolean;
    rowVersion: number;
    canEdit: boolean;
    canEditBeginDate: boolean;
    canDelete: boolean;
}
