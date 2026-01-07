import {HttpClientTestingModule, HttpTestingController} from '@angular/common/http/testing';
import {TestBed} from '@angular/core/testing';
import {PagedResult} from '@api/PagedResult';
import {PagingParameters} from '@api/PagingParameters';
import {SortParameter} from '@api/SortParameter';
import {WorklistFilterParameters} from '@api/WorklistFilterParameters';
import {WorklistTask} from '@api/WorklistTask';
import {WorklistService} from './worklist.service';

describe('WorklistService', () => {
    let service: WorklistService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [WorklistService],
        });
        service = TestBed.inject(WorklistService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get worklist tasks with correct parameters', () => {
        const mockPagedResult: PagedResult<WorklistTask> = {
            index: 0,
            total: 2,
            items: [
                {
                    id: '1',
                    assignedTo: 'admin',
                    dueDate: new Date('2025-08-05'),
                    worklistTaskType: 'review',
                    worklistTaskState: 'pending',
                } as WorklistTask,
                {
                    id: '2',
                    assignedTo: 'user',
                    dueDate: new Date('2025-08-06'),
                    worklistTaskType: 'approval',
                    worklistTaskState: 'active',
                } as WorklistTask,
            ],
        };

        const paging: PagingParameters = {
            pageIndex: 0,
            pageSize: 50,
        };

        const sort: SortParameter = {
            sort: 'dueDate',
            direction: 'asc',
        };

        service.getWorklistTasks(paging, sort, {} as WorklistFilterParameters).subscribe(result => {
            expect(result).toEqual(mockPagedResult);
        });

        const req = httpMock.expectOne(
            request =>
                request.url === '/api/worklist-tasks' &&
                request.params.get('pageIndex') === paging.pageIndex.toString() &&
                request.params.get('pageSize') === paging.pageSize.toString() &&
                request.params.get('sort') === sort.sort &&
                request.params.get('direction') === sort.direction
        );

        expect(req.request.method).toBe('GET');
        req.flush(mockPagedResult);
    });
});
