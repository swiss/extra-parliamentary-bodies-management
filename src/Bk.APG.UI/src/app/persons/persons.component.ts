import {NgClass} from '@angular/common';
import {Component, OnDestroy, OnInit, signal} from '@angular/core';
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
import {PagingParameters} from '@api/PagingParameters';
import {PersonFilterParameters} from '@api/PersonFilterParameters';
import {PersonList} from '@api/PersonList';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {BehaviorSubject, merge, Subject, switchMap, takeUntil} from 'rxjs';
import {PersonsFilterComponent} from './persons-filter/persons-filter.component';
import {PersonsService} from './persons.service';

export type PersonsColumns = 'surname' | 'givenName' | 'hasActiveMembership' | 'birthYear' | 'canton' | 'city' | 'language';

@Component({
    selector: 'apg-persons',
    templateUrl: './persons.component.html',
    styleUrl: './persons.component.scss',
    providers: [PersonsService],
    imports: [
        PersonsFilterComponent,
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
export class PersonsComponent implements OnInit, OnDestroy {
    readonly displayedColumns: PersonsColumns[] = ['surname', 'givenName', 'hasActiveMembership', 'birthYear', 'canton', 'city', 'language'];
    dataSource = new MatTableDataSource<PersonList>();
    searchSubject = new BehaviorSubject<PersonFilterParameters>({});
    readonly totalCount = signal(0);
    readonly reload$ = new Subject<void>();
    readonly pageSizeOptions = [50, 100, 150, 300, 500];
    pagingParams: PagingParameters = {
        pageIndex: 0,
        pageSize: 50,
    };
    currentSort: Sort = {
        active: 'surname',
        direction: 'asc',
    };
    readonly isFiltered = signal(false);

    private readonly destroyed$ = new Subject<void>();

    constructor(
        private readonly translateService: TranslateService,
        private readonly personsService: PersonsService,
        private readonly route: ActivatedRoute,
        private readonly router: Router
    ) {}

    ngOnInit(): void {
        merge(this.reload$, this.translateService.onLangChange, this.searchSubject.pipe())
            .pipe(
                takeUntil(this.destroyed$),
                switchMap(() =>
                    this.personsService.getPersonList(this.pagingParams, this.searchSubject.value, {
                        sort: this.currentSort.active,
                        direction: this.currentSort.direction,
                    })
                )
            )
            .subscribe(result => {
                this.dataSource.data = result.items;
                this.isFiltered.set(
                    (this.searchSubject.value.cantons?.length ?? 0) > 0 ||
                        (this.searchSubject.value.freeText?.length ?? 0) > 0 ||
                        (this.searchSubject.value.hasActiveMembership?.length ?? 0) > 0 ||
                        (this.searchSubject.value.languages?.length ?? 0) > 0
                );
                this.totalCount.set(result.total);
            });
    }

    ngOnDestroy(): void {
        this.destroyed$.next();
        this.destroyed$.complete();
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

    onFilter(searchQuery: PersonFilterParameters) {
        this.pagingParams.pageIndex = 0;
        this.searchSubject.next(searchQuery);
    }

    openPerson(item: PersonList): void {
        void this.router.navigate([item.id], {relativeTo: this.route}).then();
    }
}
