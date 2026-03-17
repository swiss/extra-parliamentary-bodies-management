import {provideHttpClient} from '@angular/common/http';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatIconModule} from '@angular/material/icon';
import {PageEvent} from '@angular/material/paginator';
import {Sort} from '@angular/material/sort';
import {Router} from '@angular/router';
import {PagedResult} from '@api/PagedResult';
import {WorklistTask} from '@api/WorklistTask';
import {LangChangeEvent, TranslatePipe, TranslateService} from '@ngx-translate/core';
import {MockModule, MockPipe} from 'ng-mocks';
import {of, Subject} from 'rxjs';
import {WorklistComponent} from './worklist.component';
import {WorklistService} from './worklist.service';

describe('WorklistComponent', () => {
    let component: WorklistComponent;
    let fixture: ComponentFixture<WorklistComponent>;
    let worklistService: Partial<WorklistService>;

    const mockWorklistTasks: WorklistTask[] = [
        {
            id: '1',
            assignedTo: 'admin',
            dueDate: new Date('2025-08-05'),
            worklistTaskType: 'review',
            worklistTaskState: 'pending',
            assignedBy: 'user1',
            created: new Date('2025-08-01'),
        },
        {
            id: '2',
            assignedTo: 'user',
            dueDate: new Date('2025-08-06'),
            worklistTaskType: 'approval',
            worklistTaskState: 'active',
            assignedBy: 'user2',
            created: new Date('2025-08-02'),
        },
    ];

    const mockPagedResult: PagedResult<WorklistTask> = {
        index: 0,
        total: 2,
        items: mockWorklistTasks,
    };

    beforeEach(async () => {
        const langChangeSubject = new Subject<LangChangeEvent>();

        const translateServiceMock = {
            getCurrentLang: jest.fn(() => 'en'),
            onLangChange: langChangeSubject,
            get: jest.fn(),
        };

        const worklistServiceMock = {
            getWorklistTasks: jest.fn().mockReturnValue(of(mockPagedResult)),
        } as Partial<WorklistService>;

        const routerMock = {
            navigate: jest.fn(),
            navigateByUrl: jest.fn(),
        };

        await TestBed.configureTestingModule({
            imports: [WorklistComponent, MockPipe(TranslatePipe), MockModule(MatIconModule)],
            providers: [
                {provide: WorklistService, useValue: worklistServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: TranslateService, useValue: translateServiceMock},
                provideHttpClient(),
            ],
        })
            .overrideTemplate(WorklistComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(WorklistComponent);
        component = fixture.componentInstance;
        worklistService = TestBed.inject(WorklistService);

        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should load worklist tasks on initialization', () => {
        component.reload$.next();
        fixture.detectChanges();

        expect(worklistService.getWorklistTasks).toHaveBeenCalledWith({pageIndex: 0, pageSize: 25}, {direction: 'asc', sort: 'dueDate'}, {});
        expect(component.tasksTable.data).toEqual(mockWorklistTasks);
        expect(component.totalCount()).toBe(2);
    });

    it('should reload data when page changes', () => {
        const pageEvent: PageEvent = {
            pageIndex: 1,
            pageSize: 100,
            length: 200,
            previousPageIndex: 0,
        };

        component.onPageChange(pageEvent);

        expect(component.pagingParams).toEqual({
            pageIndex: 1,
            pageSize: 100,
            length: 200,
            previousPageIndex: 0,
        });

        expect(worklistService.getWorklistTasks).toHaveBeenCalledWith(
            {pageIndex: 1, pageSize: 100, length: 200, previousPageIndex: 0},
            {direction: 'asc', sort: 'dueDate'},
            {}
        );
    });

    it('should reload data when sort changes', () => {
        const sortEvent: Sort = {
            active: 'worklistTaskType',
            direction: 'desc',
        };

        component.onSort(sortEvent);

        expect(component.currentSort).toEqual({
            active: 'worklistTaskType',
            direction: 'desc',
        });

        expect(worklistService.getWorklistTasks).toHaveBeenCalledWith({pageIndex: 0, pageSize: 25}, {direction: 'desc', sort: 'worklistTaskType'}, {});
    });

    describe('navigateToUrl', () => {
        it('should navigate to the provided URL', () => {
            const router = TestBed.inject(Router);
            const navigateByUrlSpy = jest.spyOn(router, 'navigateByUrl');

            component.navigateToUrl('/test-url');

            expect(navigateByUrlSpy).toHaveBeenCalledWith('/test-url');
        });
    });
});
