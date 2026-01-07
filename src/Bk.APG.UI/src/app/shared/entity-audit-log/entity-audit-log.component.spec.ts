import {ComponentFixture, TestBed} from '@angular/core/testing';
import {EntityAuditLog} from '@api/EntityAuditLog';
import {TranslatePipe} from '@ngx-translate/core';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {MockPipe} from 'ng-mocks';
import {of, Subject} from 'rxjs';
import {EntityAuditLogComponent} from './entity-audit-log.component';

describe('EntityAuditLogComponent', () => {
    type EntityAuditLogServiceStub = {
        getEntityAuditLogs: jest.Mock;
        reload$: Subject<void>;
    };
    let entityAuditLogServiceMock: EntityAuditLogServiceStub;

    let component: EntityAuditLogComponent;
    let fixture: ComponentFixture<EntityAuditLogComponent>;

    const mockAuditLogs: EntityAuditLog[] = [
        {
            auditUser: 'user1',
            auditDate: new Date('2025-11-01'),
            entityType: 'committee',
            auditDataBefore: [],
            auditDataAfter: [{columnName: 'name', data: 'Test'}],
            auditAction: 'Create',
        },
        {
            auditUser: 'user2',
            auditDate: new Date('2025-11-02'),
            entityType: 'committee',
            auditDataBefore: [{columnName: 'name', data: 'Test'}],
            auditDataAfter: [{columnName: 'name', data: 'Updated'}],
            auditAction: 'Update',
        },
    ];

    beforeEach(async () => {
        const reload$ = new Subject<void>();
        entityAuditLogServiceMock = {
            reload$,
            getEntityAuditLogs: jest.fn().mockReturnValue(
                of({
                    items: mockAuditLogs,
                    total: 2,
                })
            ),
        };

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), EntityAuditLogComponent],
            providers: [{provide: EntityAuditLogService, useValue: entityAuditLogServiceMock}],
        }).compileComponents();

        fixture = TestBed.createComponent(EntityAuditLogComponent);
        fixture.componentRef.setInput('entityId', '123');
        fixture.componentRef.setInput('entityType', 'committee');
        fixture.componentRef.setInput('relatedEntityIds', ['456']);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should load audit logs on initialization', () => {
        expect(entityAuditLogServiceMock.getEntityAuditLogs).toHaveBeenCalledWith(
            '123',
            'committee',
            ['456'],
            {pageIndex: 0, pageSize: 5},
            {sort: 'auditDate', direction: 'desc'}
        );
        expect(component.dataSource.data).toEqual(mockAuditLogs);
        expect(component.totalCount()).toBe(2);
    });

    it('should update audit logs when entity ID changes', () => {
        entityAuditLogServiceMock.getEntityAuditLogs.mockClear();

        fixture.componentRef.setInput('entityId', '789');
        fixture.detectChanges();

        expect(entityAuditLogServiceMock.getEntityAuditLogs).toHaveBeenCalledWith(
            '789',
            'committee',
            ['456'],
            {pageIndex: 0, pageSize: 5},
            {sort: 'auditDate', direction: 'desc'}
        );
    });

    it('should update paging parameters on page change', () => {
        entityAuditLogServiceMock.getEntityAuditLogs.mockClear();

        component.onPageChange({pageIndex: 1, pageSize: 15, length: 100});
        fixture.detectChanges();

        expect(component.pagingParams()).toEqual({pageIndex: 1, pageSize: 15, length: 100});
        expect(entityAuditLogServiceMock.getEntityAuditLogs).toHaveBeenCalledWith(
            '123',
            'committee',
            ['456'],
            {pageIndex: 1, pageSize: 15, length: 100},
            {sort: 'auditDate', direction: 'desc'}
        );
    });

    it('should update sort parameters and reset page index on sort change', () => {
        entityAuditLogServiceMock.getEntityAuditLogs.mockClear();

        component.onSort({active: 'auditUser', direction: 'asc'});
        fixture.detectChanges();

        expect(component.pagingParams().pageIndex).toBe(0);
        expect(component.currentSort()).toEqual({active: 'auditUser', direction: 'asc'});
        expect(entityAuditLogServiceMock.getEntityAuditLogs).toHaveBeenCalledWith('123', 'committee', ['456'], expect.objectContaining({pageIndex: 0}), {
            sort: 'auditUser',
            direction: 'asc',
        });
    });

    it('should reload data when paging parameters change', () => {
        entityAuditLogServiceMock.getEntityAuditLogs.mockClear();

        component.pagingParams.set({pageIndex: 2, pageSize: 15});
        fixture.detectChanges();

        expect(entityAuditLogServiceMock.getEntityAuditLogs).toHaveBeenCalledWith(
            '123',
            'committee',
            ['456'],
            {pageIndex: 2, pageSize: 15},
            {sort: 'auditDate', direction: 'desc'}
        );
    });

    it('should reload data when sort changes', () => {
        entityAuditLogServiceMock.getEntityAuditLogs.mockClear();

        component.currentSort.set({active: 'auditAction', direction: 'asc'});
        fixture.detectChanges();

        expect(entityAuditLogServiceMock.getEntityAuditLogs).toHaveBeenCalledWith(
            '123',
            'committee',
            ['456'],
            {pageIndex: 0, pageSize: 5},
            {sort: 'auditAction', direction: 'asc'}
        );
    });

    it('should not call service when entity ID is empty', () => {
        entityAuditLogServiceMock.getEntityAuditLogs.mockClear();

        fixture.componentRef.setInput('entityId', '');
        fixture.detectChanges();

        expect(entityAuditLogServiceMock.getEntityAuditLogs).not.toHaveBeenCalled();
    });

    it('should not call service when related entity IDs are undefined', () => {
        entityAuditLogServiceMock.getEntityAuditLogs.mockClear();

        fixture.componentRef.setInput('relatedEntityIds', undefined);
        fixture.detectChanges();

        expect(entityAuditLogServiceMock.getEntityAuditLogs).not.toHaveBeenCalled();
    });

    it('should reload data when refresh$ emits', () => {
        entityAuditLogServiceMock.getEntityAuditLogs.mockClear();

        entityAuditLogServiceMock.reload$.next();
        fixture.detectChanges();

        expect(entityAuditLogServiceMock.getEntityAuditLogs).toHaveBeenCalledWith(
            '123',
            'committee',
            ['456'],
            {pageIndex: 0, pageSize: 5},
            {sort: 'auditDate', direction: 'desc'}
        );
    });
});
