import {NgClass, DatePipe} from '@angular/common';
import {Component, input} from '@angular/core';
import {
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
import {MembershipDetails} from '@api/MembershipDetails';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
    selector: 'apg-person-overview-memberships',
    templateUrl: './person-overview-memberships.component.html',
    styleUrl: './person-overview-memberships.component.scss',
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
        NgClass,
        MatNoDataRow,
        DatePipe,
        TranslatePipe,
    ],
})
export class PersonOverviewMembershipsComponent {
    memberships = input.required<MembershipDetails[]>();

    readonly displayedColumns: (keyof MembershipDetails)[] = ['committee', 'function', 'beginDate', 'endDate'];
}
