import {CandidateListDuplicateCheckResult} from './CandidateListDuplicateCheckResult';
import {PersonDetails} from './PersonDetails';
import {PersonMinimal} from './PersonMinimal';

export interface CandidateListValidationResult {
    isValid: boolean;
    errors: string[];
    areJustificationsMissing: boolean;
    areContactPointsMissing: boolean;
    isReadyForProposalActivated: boolean;
    duplicateCheckResults: CandidateListDuplicateCheckResult[];
    createdPersons: PersonDetails[];
    existingPersons: PersonDetails[];
    isAdditionalMembershipValidationRequired: boolean;
    allValidationsPassed: boolean;
    personsWithMissingInterests: PersonMinimal[];
    personsWithMissingBaseData: PersonMinimal[];
    personsWithMembershipValidationIssues: PersonMinimal[];
}
