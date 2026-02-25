import {CandidateListDuplicateCheckResult} from './CandidateListDuplicateCheckResult';
import {PersonDetails} from './PersonDetails';
import {PersonMinimal} from './PersonMinimal';

export interface CandidateListValidationResult {
    isValid: boolean;
    errors: string[];
    areJustificationsMissing: boolean;
    areContactPointsMissing: boolean;
    duplicateCheckResults: CandidateListDuplicateCheckResult[];
    createdPersons: PersonDetails[];
    existingPersons: PersonDetails[];
    isAdditionalMembershipValidationRequired: boolean;
    personsWithMissingInterests: PersonMinimal[];
    personsWithMissingBaseData: PersonMinimal[];
    personsWithMembershipValidationIssues: PersonMinimal[];
}
