import {FormControl} from '@angular/forms';

export interface PersonFilterForm {
    freeText: FormControl<string | null>;
    hasActiveMembership: FormControl<boolean[] | null>;
    cantons: FormControl<string[] | null>;
    languages: FormControl<string[] | null>;
}
