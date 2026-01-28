import {DatePipe} from '@angular/common';
import {ChangeDetectionStrategy, Component, effect, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
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
import {MAT_TOOLTIP_DEFAULT_OPTIONS, MatTooltip, MatTooltipDefaultOptions} from '@angular/material/tooltip';
import {Router} from '@angular/router';
import {PagingParameters} from '@api/PagingParameters';
import {WorklistFilterParameters} from '@api/WorklistFilterParameters';
import {WorklistTask} from '@api/WorklistTask';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObButtonDirective} from '@oblique/oblique';
import {MasterDataService} from '@shared/master-data.service';
import {distinctUntilChanged, merge, startWith, Subject, switchMap} from 'rxjs';
import {WorklistFilterComponent} from './worklist-filter/worklist-filter.component';
import {WorklistService} from './worklist.service';

@Component({
    selector: 'apg-worklist',
    imports: [
        WorklistFilterComponent,
        TranslatePipe,
        MatTable,
        MatSort,
        MatCell,
        MatCellDef,
        MatColumnDef,
        MatHeaderCell,
        MatSortHeader,
        MatHeaderRow,
        MatHeaderRowDef,
        MatRow,
        MatRowDef,
        MatHeaderCellDef,
        MatNoDataRow,
        DatePipe,
        MatPaginator,
        MatTooltip,
        MatIcon,
        MatIconButton,
        ObButtonDirective,
    ],
    templateUrl: './worklist.component.html',
    styleUrl: './worklist.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    providers: [
        {provide: MAT_TOOLTIP_DEFAULT_OPTIONS, useValue: {showDelay: 300, position: 'before', disableTooltipInteractivity: true} as MatTooltipDefaultOptions},
    ],
})
export class WorklistComponent {
    readonly displayedColumns = signal<(keyof WorklistTask | 'actions')[]>([
        'worklistTaskType',
        'assignedBy',
        'assignedTo',
        'department',
        'office',
        'section',
        'worklistTaskState',
        'created',
        'dueDate',
        'actions',
    ]);

    tasksTable: MatTableDataSource<WorklistTask> = new MatTableDataSource<WorklistTask>();
    totalCount = signal(0);
    filterValue: WorklistFilterParameters = {};
    pagingParams: PagingParameters = {
        pageIndex: 0,
        pageSize: 25,
    };
    readonly pageSizeOptions = [25, 50, 100];
    currentSort: Sort = {
        active: 'dueDate',
        direction: 'asc',
    };

    readonly reload$ = new Subject<void>();

    private readonly data = signal<WorklistTask[]>([]);

    constructor(
        readonly translateService: TranslateService,
        readonly worklistService: WorklistService,
        private readonly router: Router,
        readonly masterDataService: MasterDataService
    ) {
        merge(
            this.reload$,
            this.translateService.onLangChange.pipe(
                startWith({lang: this.translateService.currentLang}),
                distinctUntilChanged((prev, curr) => prev.lang === curr.lang)
            )
        )
            .pipe(
                switchMap(() =>
                    this.worklistService.getWorklistTasks(
                        this.pagingParams,
                        {
                            sort: this.currentSort.active,
                            direction: this.currentSort.direction,
                        },
                        this.filterValue
                    )
                ),
                takeUntilDestroyed()
            )
            .subscribe(result => {
                this.data.set(result.items);
                this.totalCount.set(result.total);
            });

        effect(() => (this.tasksTable.data = this.data()));
    }

    onPageChange(pageEvent: PageEvent): void {
        this.pagingParams = {...pageEvent};
        this.reload$.next();
    }

    onSort(sort: Sort) {
        this.currentSort = {...sort};
        this.reload$.next();
    }

    onFilter(searchQuery: WorklistFilterParameters) {
        this.pagingParams.pageIndex = 0;
        this.filterValue = searchQuery;
        this.reload$.next();
    }

    navigateToDetails(task: WorklistTask) {
        void this.router.navigate(['worklist', task.id]);
    }

    navigateToUrl(url: string): void {
        void this.router.navigateByUrl(url);
    }
}
