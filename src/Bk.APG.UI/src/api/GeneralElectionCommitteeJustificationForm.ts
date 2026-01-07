import {FormControl} from '@angular/forms';

export interface GeneralElectionCommitteeJustificationForm {
    selectionProcedure: FormControl<string | null>;
    justificationMembers: FormControl<string | null>;
    justificationGenders: FormControl<string | null>;
    measuresGenders: FormControl<string | null>;
    justificationLanguages: FormControl<string | null>;
    measuresLanguages: FormControl<string | null>;
}
