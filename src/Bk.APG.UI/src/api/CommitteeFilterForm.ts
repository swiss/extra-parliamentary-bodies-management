import {FormControl} from '@angular/forms';

export interface CommitteeFilterForm {
    freeText: FormControl<string | null>;
    levels: FormControl<string[] | null>;
    departments: FormControl<string[] | null>;
    offices: FormControl<string[] | null>;
    committeeTypes: FormControl<string[] | null>;
    terms: FormControl<string[] | null>;
    isActive: FormControl<boolean[] | null>;
    isMarketOrientated: FormControl<boolean[] | null>;
    hasSupervisionDuty: FormControl<boolean[] | null>;
}
