import {FormControl} from '@angular/forms';
import {ReportType} from '../app/exports/ReportType';

export interface RequestsAndReportsFilterForm {
    documentType: FormControl<ReportType | null>;
    analysisDate1: FormControl<Date | null>;
    analysisDate2: FormControl<Date | null>;
    departments: FormControl<string[] | null>;
    offices: FormControl<string[] | null>;
    committeeTypes: FormControl<string[] | null>;
    committeesWithActiveMembership: FormControl<boolean>;
    releasedCommittees: FormControl<boolean>;
}
