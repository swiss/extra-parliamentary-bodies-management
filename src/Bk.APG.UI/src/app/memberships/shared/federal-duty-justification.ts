export const federalDutyJustificationTexts = {
    de: 'Die Amtszeitbeschränkung gilt nicht für Bundesangestellte, deren Mitgliedschaft für die Aufgabenerfüllung erforderlich ist oder in einem anderen Erlass zwingend vorgeschrieben wird (Art. 8i Abs. 3 RVOV).',
} as const;

const applicableCommitteeTypeIds = ['f2e2af70-d1d4-42b5-b23a-793cbc220064', '0a4b7f1d-d8bf-4932-bece-dd2a51cc2d59'];

export function isFederalDutyJustificationApplicable(committeeTypeId: string | undefined): boolean {
    return committeeTypeId !== undefined && applicableCommitteeTypeIds.includes(committeeTypeId);
}

export function getFederalDutyJustification(): string {
    return federalDutyJustificationTexts.de;
}

export function isFederalDutyJustification(text: string | undefined): boolean {
    return text !== undefined && Object.values(federalDutyJustificationTexts).includes(text as never);
}
