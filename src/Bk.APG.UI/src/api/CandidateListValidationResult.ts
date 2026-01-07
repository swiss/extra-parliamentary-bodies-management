import {CandidateListDuplicateCheckResult} from './CandidateListDuplicateCheckResult';
import {PersonDetails} from './PersonDetails';

export interface CandidateListValidationResult {
    isValid: boolean;
    errors: string[];
    areJustificationsMissing: boolean;
    duplicateCheckResults: CandidateListDuplicateCheckResult[];
    createdPersons: PersonDetails[];
    existingPersons: PersonDetails[];
}
