import {provideHttpClient} from '@angular/common/http';
import {HttpTestingController, provideHttpClientTesting} from '@angular/common/http/testing';
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
            providers: [WorklistService, provideHttpClient(), provideHttpClientTesting()],
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

    it('should include filter parameters when filters are provided', () => {
        const mockPagedResult: PagedResult<WorklistTask> = {
            index: 0,
            total: 1,
            items: [],
        };
        const paging: PagingParameters = {
            pageIndex: 0,
            pageSize: 50,
        };
        const sort: SortParameter = {
            sort: 'dueDate',
            direction: 'asc',
        };
        const filters: WorklistFilterParameters = {
            assignedTo: 'admin',
            worklistTaskStates: ['pending'],
        };

        service.getWorklistTasks(paging, sort, filters).subscribe(result => {
            expect(result).toEqual(mockPagedResult);
        });

        const req = httpMock.expectOne(
            request =>
                request.url === '/api/worklist-tasks' &&
                request.params.get('pageIndex') === paging.pageIndex.toString() &&
                request.params.get('pageSize') === paging.pageSize.toString() &&
                request.params.get('sort') === sort.sort &&
                request.params.get('direction') === sort.direction &&
                request.params.get('assignedTo') === filters.assignedTo &&
                request.params.get('worklistTaskStateIds') === filters.worklistTaskStates![0]
        );

        expect(req.request.method).toBe('GET');
        req.flush(mockPagedResult);
    });

    it('should not include extra filter parameters when filter object is empty', () => {
        const mockPagedResult: PagedResult<WorklistTask> = {
            index: 0,
            total: 0,
            items: [],
        };
        const paging: PagingParameters = {
            pageIndex: 0,
            pageSize: 50,
        };
        const sort: SortParameter = {
            sort: 'dueDate',
            direction: 'asc',
        };

        service.getWorklistTasks(paging, sort, {}).subscribe(result => {
            expect(result).toEqual(mockPagedResult);
        });

        const req = httpMock.expectOne(
            request =>
                request.url === '/api/worklist-tasks' &&
                request.params.get('pageIndex') === paging.pageIndex.toString() &&
                request.params.get('pageSize') === paging.pageSize.toString() &&
                request.params.get('sort') === sort.sort &&
                request.params.get('direction') === sort.direction &&
                request.params.get('assignedTo') === null &&
                request.params.get('worklistTaskStateIds') === null
        );

        expect(req.request.method).toBe('GET');
        req.flush(mockPagedResult);
    });
});
