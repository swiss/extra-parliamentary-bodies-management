import {FormControl} from '@angular/forms';

export interface RecipientsFilterForm {
    departments: FormControl<string[] | null>;
    offices: FormControl<string[] | null>;
    committeeTypes: FormControl<string[] | null>;
    correspondenceLanguages: FormControl<string[] | null>;
    electionTypes: FormControl<string[] | null>;
    formLetterSender: FormControl<string | null>;
}
