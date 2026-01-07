import {DatePipe} from '@angular/common';
import {Component, effect, input, signal} from '@angular/core';
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
import {EntityAuditLog} from '@api/EntityAuditLog';
import {PagingParameters} from '@api/PagingParameters';
import {TranslatePipe} from '@ngx-translate/core';
import {EntityAuditLogService, EntityType} from './entity-audit-log.service';

export type EntityAuditLogColumns = 'auditUser' | 'auditDate' | 'entityType' | 'auditDataBefore' | 'auditDataAfter' | 'auditAction';

@Component({
    selector: 'apg-entity-audit-log',
    imports: [
        MatTable,
        MatSort,
        MatSortHeader,
        MatColumnDef,
        MatCellDef,
        MatCell,
        MatHeaderCell,
        MatHeaderCellDef,
        MatHeaderRow,
        MatHeaderRowDef,
        MatRow,
        MatRowDef,
        MatNoDataRow,
        MatPaginator,
        DatePipe,
        TranslatePipe,
    ],
    templateUrl: './entity-audit-log.component.html',
    styleUrl: './entity-audit-log.component.scss',
})
export class EntityAuditLogComponent {
    entityId = input.required<string>();
    entityType = input.required<EntityType>();
    relatedEntityIds = input.required<string[]>();

    dataSource = new MatTableDataSource<EntityAuditLog>();

    readonly totalCount = signal(0);

    pagingParams = signal<PagingParameters>({
        pageIndex: 0,
        pageSize: 5,
    });
    currentSort = signal<Sort>({
        active: 'auditDate',
        direction: 'desc',
    });

    protected readonly pageSizeOptions = [5, 15, 30];
    protected readonly displayedColumns: EntityAuditLogColumns[] = ['auditUser', 'auditDate', 'entityType', 'auditDataBefore', 'auditDataAfter', 'auditAction'];

    private readonly refreshTick = signal(0);

    constructor(private readonly entityAuditLogService: EntityAuditLogService) {
        effect(() => {
            this.refreshTick(); // dependency to allow manual refresh

            const entityId = this.entityId();
            const entityType = this.entityType();
            const relatedEntityIds = this.relatedEntityIds();
            const pagingParams = this.pagingParams();
            const sort = this.currentSort();

            if (entityId && entityType && relatedEntityIds !== undefined) {
                this.entityAuditLogService
                    .getEntityAuditLogs(entityId, entityType, relatedEntityIds, pagingParams, {
                        sort: sort.active,
                        direction: sort.direction,
                    })
                    .subscribe(result => {
                        this.dataSource.data = result.items;
                        this.totalCount.set(result.total);
                    });
            }
        });

        this.entityAuditLogService.reload$.pipe(takeUntilDestroyed()).subscribe(() => {
            this.refreshTick.update(x => x + 1);
        });
    }

    onPageChange(pageEvent: PageEvent): void {
        this.pagingParams.set({...pageEvent});
    }

    onSort(sort: Sort) {
        this.pagingParams.set({...this.pagingParams(), pageIndex: 0});
        this.currentSort.set({...sort});
    }
}
