export interface CommitteeUpdate {
    id: string;
    committeeNumber: number;
    isActive: boolean;
    descriptionGerman: string;
    descriptionFrench: string;
    descriptionItalian: string;
    descriptionRomansh: string;
    levelId: string;
    departmentId: string;
    officeId: string;
    committeeTypeId: string;
    federalLawEstablishment: boolean;
    supervisionDuty: boolean;
    marketOrientated: boolean;
    legalFormId?: string;
    oldLegalForm?: string; // TODO: entfernen, wenn neues APG abgenommen ist
    legalBase?: string;
    beginDate: Date;
    endDate?: Date;
    termOfOfficeId: string;
    minimalMembers?: number;
    maximalMembers?: number;
    additionalAuthorityMembers: boolean;
    linkAuthorityWebsite: string;
    federalInstitution?: boolean;
    linkHomepageGerman?: string;
    linkHomepageFrench?: string;
    linkHomepageItalian?: string;
    linkHomepageRomansh?: string;
    canEditAll: boolean;
    canEditDepartment: boolean;
    canEditLegalbase: boolean;
    membersCount: number;
    vacanciesInGeneralElection?: number;
    membershipAdditionsInGeneralElection?: string[];
    rowVersion: number;
}
