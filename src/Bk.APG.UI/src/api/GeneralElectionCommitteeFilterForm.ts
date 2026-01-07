import {FormControl} from '@angular/forms';

export interface GeneralElectionCommitteeFilterForm {
    freeText: FormControl<string | null>;
    departments: FormControl<string[] | null>;
    offices: FormControl<string[] | null>;
    committeeTypes: FormControl<string[] | null>;
    isMarketOrientated: FormControl<boolean[] | null>;
    hasSupervisionDuty: FormControl<boolean[] | null>;
    status: FormControl<string | null>;
    vacancies: FormControl<string | null>;
    statusProposal: FormControl<string | null>;
}
