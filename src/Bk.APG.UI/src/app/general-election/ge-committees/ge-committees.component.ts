import {DatePipe} from '@angular/common';
import {Component, signal} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatSort, MatSortHeader, Sort} from '@angular/material/sort';
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
import {ActivatedRoute, Router} from '@angular/router';
import {GeneralElectionCommitteeFilterParameters} from '@api/GeneralElectionCommitteeFilterParameters';
import {GeneralElectionCommitteeList} from '@api/GeneralElectionCommitteeList';
import {PagingParameters} from '@api/PagingParameters';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {combineLatest, distinctUntilChanged, startWith, Subject, switchMap} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {GeneralElectionCommitteesFilterComponent} from './ge-committees-filter/ge-committees-filter.component';
import {GeneralElectionCommitteesService} from './ge-committees.service';

export type CommitteesColumns =
    | 'description'
    | 'department'
    | 'office'
    | 'committeeType'
    | 'term'
    | 'isMarketOrientated'
    | 'hasSupervisionDuty'
    | 'status'
    | 'vacancies'
    | 'statusProposal'
    | 'modified';
@Component({
    selector: 'apg-ge-committees',
    templateUrl: './ge-committees.component.html',
    styleUrl: './ge-committees.component.scss',
    providers: [GeneralElectionCommitteesService],
    imports: [
        GeneralElectionCommitteesFilterComponent,
        MatTable,
        MatSort,
        MatColumnDef,
        MatHeaderCellDef,
        MatHeaderCell,
        MatSortHeader,
        MatCellDef,
        MatCell,
        MatHeaderRowDef,
        MatHeaderRow,
        MatRowDef,
        MatRow,
        MatNoDataRow,
        MatPaginator,
        TranslatePipe,
        DatePipe,
    ],
})
export class GeneralElectionCommitteesComponent {
    readonly displayedColumns: CommitteesColumns[] = [
        'description',
        'department',
        'office',
        'committeeType',
        'isMarketOrientated',
        'hasSupervisionDuty',
        'status',
        'vacancies',
        'statusProposal',
        'modified',
    ];
    dataSource = new MatTableDataSource<GeneralElectionCommitteeList>();
    filterValue: GeneralElectionCommitteeFilterParameters = {};
    readonly totalCount = signal(0);
    readonly reload$ = new Subject<void>();
    readonly pageSizeOptions = [50, 100, 150, 300, 500];

    pagingParams: PagingParameters = {
        pageIndex: 0,
        pageSize: 50,
    };
    currentSort: Sort = {
        active: 'description',
        direction: 'asc',
    };

    protected isAdmin = toSignal(this.authService.isAdmin$);
    protected isDepartmentUser = toSignal(this.authService.isDepartmentUser$);

    constructor(
        private readonly translateService: TranslateService,
        private readonly generalElectionCommitteesService: GeneralElectionCommitteesService,
        private readonly authService: AuthService,
        private readonly route: ActivatedRoute,
        private readonly router: Router
    ) {
        if (this.isAdmin()) {
            this.displayedColumns = [
                'description',
                'department',
                'office',
                'committeeType',
                'isMarketOrientated',
                'hasSupervisionDuty',
                'status',
                'vacancies',
                'statusProposal',
                'modified',
            ];
        } else if (this.isDepartmentUser()) {
            this.displayedColumns = [
                'description',
                'office',
                'committeeType',
                'isMarketOrientated',
                'hasSupervisionDuty',
                'status',
                'vacancies',
                'statusProposal',
                'modified',
            ];
        } else {
            this.displayedColumns = [
                'description',
                'committeeType',
                'isMarketOrientated',
                'hasSupervisionDuty',
                'status',
                'vacancies',
                'statusProposal',
                'modified',
            ];
        }

        combineLatest([
            this.reload$.pipe(startWith(null)),
            this.translateService.onLangChange.pipe(
                startWith({lang: this.translateService.getCurrentLang()}),
                distinctUntilChanged((prev, curr) => prev.lang === curr.lang)
            ),
        ])
            .pipe(
                switchMap(() =>
                    this.generalElectionCommitteesService.getGeneralElectionCommitteeList(this.pagingParams, this.filterValue, {
                        sort: this.currentSort.active,
                        direction: this.currentSort.direction,
                    })
                ),
                takeUntilDestroyed()
            )
            .subscribe(result => {
                this.dataSource.data = result.items;
                this.totalCount.set(result.total);
            });
    }

    onPageChange(pageEvent: PageEvent): void {
        this.pagingParams = {...pageEvent};
        this.reload$.next();
    }

    onSort(sort: Sort) {
        this.pagingParams.pageIndex = 0;
        this.currentSort = {...sort};
        this.reload$.next();
    }

    onFilter(searchQuery: GeneralElectionCommitteeFilterParameters) {
        this.pagingParams.pageIndex = 0;
        this.filterValue = searchQuery;
        this.reload$.next();
    }

    openGeneralElectionCommittee(committeeId: string) {
        void this.router.navigate([committeeId], {relativeTo: this.route}).then();
    }
}
