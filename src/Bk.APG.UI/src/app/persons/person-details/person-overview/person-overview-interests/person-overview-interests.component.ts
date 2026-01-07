import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {
    MatTableDataSource,
    MatTable,
    MatColumnDef,
    MatHeaderCellDef,
    MatHeaderCell,
    MatCellDef,
    MatCell,
    MatHeaderRowDef,
    MatHeaderRow,
    MatRowDef,
    MatRow,
    MatNoDataRow,
} from '@angular/material/table';
import {InterestDetails} from '@api/InterestDetails';
import {TranslatePipe} from '@ngx-translate/core';

export type InterestColumns = 'text' | 'legalForm' | 'committee' | 'function';

@Component({
    selector: 'apg-person-overview-interests',
    templateUrl: './person-overview-interests.component.html',
    styleUrl: './person-overview-interests.component.scss',
    imports: [
        MatTable,
        MatColumnDef,
        MatHeaderCellDef,
        MatHeaderCell,
        MatCellDef,
        MatCell,
        MatHeaderRowDef,
        MatHeaderRow,
        MatRowDef,
        MatRow,
        MatNoDataRow,
        TranslatePipe,
    ],
})
export class PersonOverviewInterestsComponent implements OnChanges {
    @Input() interests!: InterestDetails[];

    readonly displayedColumns: InterestColumns[] = ['text', 'legalForm', 'committee', 'function'];
    dataSource = new MatTableDataSource<InterestDetails>();

    ngOnChanges(changes: SimpleChanges): void {
        /* eslint-disable dot-notation */
        if (changes?.['interests']) {
            this.dataSource.data = this.interests;
        }
    }
}
