import {ContactPointDetail} from './ContactPointDetail';

export interface CommitteeDetails {
    id: string;
    committeeNumber: number;
    description: string;
    descriptionDe: string;
    descriptionFr: string;
    descriptionIt: string;
    descriptionRm: string;
    committeeLevel: string;
    department: string;
    office: string;
    committeeType: string;
    committeeTypeId?: string;
    legalForm?: string;
    oldLegalForm?: string; // TODO: entfernen, wenn neues APG abgenommen ist
    legalBase?: string;
    federalLawEstablishment: boolean;
    marketOrientated: boolean;
    extraParliamentaryCommission: boolean;
    supervisionDuty: boolean;
    beginDate: Date;
    endDate?: Date;
    termOfOffice: string;
    termOfOfficeId: string;
    termOfOfficeEndDate: Date;
    membersCount: number;
    minimalMembers?: number;
    maximalMembers?: number;
    vacanciesGeneralElection?: number;
    additionalAuthorityMembers: boolean;
    linkAuthorityWebsite?: string;
    remarksBaseData?: string;
    remarksBaseDataAdmin?: string;
    isDeleted: boolean;
    contactPoints?: ContactPointDetail[];
    isActive: boolean;
    needsAttention: boolean;
    needsAttentionLongerDuty: boolean;
    needsAttentionShorterDuty: boolean;
    needsAttentionFederalDuty: boolean;
    needsAttentionFederalAssembly: boolean;
    needsAttentionNoMembers: boolean;
    needsAttentionAboveMaxMembers: boolean;
    needsAttentionDataProtectionOfficer: boolean;
    needsAttentionSecretariat: boolean;
    needsAttentionBasicData: boolean;
    needsAttentionMembershipExpired: boolean;
    needsAttentionMembershipInterestOrOccupation: boolean;
    canCreateMembership: boolean;
    period4YearsInGeneralElection: boolean;
    canEdit: boolean;
    vacanciesInCurrentTermOfOffice?: number;

    justificationMembers?: string;

    femaleThreshold?: number;
    femaleQuota?: number;
    maleThreshold?: number;
    maleQuota?: number;
    justificationGenders?: string;
    measuresGenders?: string;
    generalGenderMeasure?: string;

    germanThreshold?: number;
    germanQuota?: number;
    frenchThreshold?: number;
    frenchQuota?: number;
    italianThreshold?: number;
    italianQuota?: number;
    romanshThreshold?: number;
    romanshQuota?: number;
    justificationLanguages?: string;
    measuresLanguages?: string;
    generalLanguageMeasure?: string;
    federalInstitution?: boolean;
}
