import {MembershipAddition} from './MembershipAddition';

export interface MembershipCreate {
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
    membershipAddition?: MembershipAddition;
    membershipAdditionId?: string;
    justificationLongerDuty?: string;
    justificationShorterDuty?: string;
    justificationMemberInFederalDuty?: string;
    justificationMemberInFederalAssembly?: string;
    requirementsProfile?: string;
    remarks?: string;
    remarksStatus?: string;
    inCorrelationWithFederalDuty: boolean;
}
