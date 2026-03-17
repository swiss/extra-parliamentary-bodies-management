import {Component} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {
    MatCell,
    MatCellDef,
    MatColumnDef,
    MatHeaderCell,
    MatHeaderCellDef,
    MatHeaderRow,
    MatHeaderRowDef,
    MatNoDataRow,
    MatRow,
    MatRowDef,
    MatTable,
    MatTableDataSource,
} from '@angular/material/table';
import {Router} from '@angular/router';
import {CommitteeTypeList} from '@api/CommitteeTypeList';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {distinctUntilChanged, map, startWith, Subject, switchMap} from 'rxjs';
import {CommitteeTypeService} from '../committee-type.service';

export type CommitteeTypeColumns =
    | 'text'
    | 'femaleThreshold'
    | 'maleThreshold'
    | 'germanMinimalThreshold'
    | 'frenchMinimalThreshold'
    | 'italianMinimalThreshold'
    | 'romanshMinimalThreshold'
    | 'germanThresholdPercentage'
    | 'frenchThresholdPercentage'
    | 'italianThresholdPercentage'
    | 'romanshThresholdPercentage';

@Component({
    selector: 'apg-committee-type-list',
    templateUrl: './committee-type-list.component.html',
    styleUrl: './committee-type-list.component.scss',
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
export class CommitteeTypeListComponent {
    readonly displayedColumns: CommitteeTypeColumns[] = [
        'text',
        'femaleThreshold',
        'maleThreshold',
        'germanMinimalThreshold',
        'frenchMinimalThreshold',
        'italianMinimalThreshold',
        'romanshMinimalThreshold',
        'germanThresholdPercentage',
        'frenchThresholdPercentage',
        'italianThresholdPercentage',
        'romanshThresholdPercentage',
    ];

    dataSource = new MatTableDataSource<CommitteeTypeList>();

    private readonly destroyed$ = new Subject<void>();
    private readonly refresh = new Subject<void>();

    constructor(
        protected readonly committeeTypeService: CommitteeTypeService,
        private readonly translateService: TranslateService,
        private readonly router: Router
    ) {
        this.translateService.onLangChange
            .pipe(
                startWith({lang: this.translateService.getCurrentLang()}),
                map(lang => lang.lang),
                distinctUntilChanged()
            )
            .pipe(
                switchMap(() => this.committeeTypeService.getCommitteeTypeList()),
                takeUntilDestroyed()
            )
            .subscribe(committeeTypes => {
                this.dataSource.data = committeeTypes;
            });
    }

    editCommitteeType(id: string) {
        void this.router.navigate(['administration/committeeTypes', id]);
    }
}
