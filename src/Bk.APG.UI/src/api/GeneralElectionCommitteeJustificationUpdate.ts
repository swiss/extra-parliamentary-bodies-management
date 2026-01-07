export interface GeneralElectionCommitteeJustificationUpdate {
    id: string;
    justificationMembers?: string;
    isJustificationGendersRequired?: boolean;
    justificationGenders?: string;
    measuresGenders?: string;
    generalMeasuresGenders?: string;
    isJustificationLanguagesRequired?: boolean;
    justificationLanguages?: string;
    measuresLanguages?: string;
    generalMeasuresLanguages?: string;
    selectionProcedure?: string;
    currentMemberCount?: number;
    currentGenderQuota?: string;
    currentLanguageQuota?: string;
    rowVersion: number;
}
