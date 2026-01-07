import {FormControl} from '@angular/forms';

export interface WorklistFilterForm {
    committee: FormControl<string | null>;
    departments: FormControl<string[] | null>;
    offices: FormControl<string[] | null>;
    worklistTaskTypes: FormControl<string[] | null>;
    worklistTaskStates: FormControl<string[] | null>;
    assignedBy: FormControl<string | null>;
    assignedTo: FormControl<string | null>;
    createdFrom: FormControl<Date | null>;
    createdTo: FormControl<Date | null>;
    dueDateFrom: FormControl<Date | null>;
    dueDateTo: FormControl<Date | null>;
}
