export interface CommitteeJustificationUpdate {
    id: string;
    justificationMembers?: string;
    justificationGenders?: string;
    measuresGenders?: string;
    generalMeasuresGenders?: string;
    justificationLanguages?: string;
    measuresLanguages?: string;
    generalMeasuresLanguages?: string;
    currentMemberCount?: number;
    currentGenderQuota?: string;
    currentLanguageQuota?: string;
    rowVersion: number;
}
