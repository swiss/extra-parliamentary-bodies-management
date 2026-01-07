import {FormControl} from '@angular/forms';

export interface CommitteeJustificationForm {
    justificationMembers: FormControl<string | null>;
    justificationGenders: FormControl<string | null>;
    measuresGenders: FormControl<string | null>;
    justificationLanguages: FormControl<string | null>;
    measuresLanguages: FormControl<string | null>;
}
