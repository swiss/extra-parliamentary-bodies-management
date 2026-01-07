import {NgClass} from '@angular/common';
import {Component, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
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
import {CommitteeFilterParameters} from '@api/CommitteeFilterParameters';
import {CommitteeList} from '@api/CommitteeList';
import {PagingParameters} from '@api/PagingParameters';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {combineLatest, distinctUntilChanged, startWith, Subject, switchMap} from 'rxjs';
import {CommitteesFilterComponent} from './committees-filter/committees-filter.component';
import {CommitteesService} from './committees.service';

export type CommitteesColumns =
    | 'committeeId'
    | 'description'
    | 'level'
    | 'department'
    | 'office'
    | 'committeeType'
    | 'term'
    | 'isActive'
    | 'isMarketOrientated'
    | 'hasSupervisionDuty';

@Component({
    selector: 'apg-committees',
    templateUrl: './committees.component.html',
    styleUrl: './committees.component.scss',
    providers: [CommitteesService],
    imports: [
        CommitteesFilterComponent,
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
        NgClass,
        MatNoDataRow,
        MatPaginator,
        TranslatePipe,
    ],
})
export class CommitteesComponent {
    readonly displayedColumns: CommitteesColumns[] = [
        'committeeId',
        'description',
        'level',
        'department',
        'office',
        'committeeType',
        'term',
        'isActive',
        'isMarketOrientated',
        'hasSupervisionDuty',
    ];
    dataSource = new MatTableDataSource<CommitteeList>();
    filterValue: CommitteeFilterParameters = {};
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

    constructor(
        private readonly translateService: TranslateService,
        private readonly committeesService: CommitteesService,
        private readonly route: ActivatedRoute,
        private readonly router: Router
    ) {
        combineLatest([
            this.reload$,
            this.translateService.onLangChange.pipe(
                startWith({lang: this.translateService.currentLang}),
                distinctUntilChanged((prev, curr) => prev.lang === curr.lang)
            ),
        ])
            .pipe(
                switchMap(() =>
                    this.committeesService.getCommitteeList(this.pagingParams, this.filterValue, {
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

    onFilter(searchQuery: CommitteeFilterParameters) {
        this.pagingParams.pageIndex = 0;
        this.filterValue = searchQuery;
        this.reload$.next();
    }

    openCommittee(id: string) {
        void this.router.navigate([id], {relativeTo: this.route}).then();
    }
}
