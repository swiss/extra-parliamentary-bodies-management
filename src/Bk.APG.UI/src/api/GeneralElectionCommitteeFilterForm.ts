import {FormControl} from '@angular/forms';

export interface GeneralElectionCommitteeFilterForm {
    freeText: FormControl<string | null>;
    departments: FormControl<string[] | null>;
    offices: FormControl<string[] | null>;
    committeeTypes: FormControl<string[] | null>;
    isMarketOrientated: FormControl<boolean[] | null>;
    hasSupervisionDuty: FormControl<boolean[] | null>;
    isNew: FormControl<boolean[] | null>;
    vacancies: FormControl<boolean[] | null>;
    statusProposal: FormControl<boolean[] | null>;
}
