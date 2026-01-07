import {HttpClient} from '@angular/common/http';
import {TestBed} from '@angular/core/testing';
import {of} from 'rxjs';
import {EiamAssignmentService} from './eiam-assignment.service';

describe('EiamAssignmentService', () => {
    let service: EiamAssignmentService;

    const httpClientMock = {
        get: jest.fn(() => of([])),
    } as unknown as jest.Mocked<HttpClient>;

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [{provide: HttpClient, useValue: httpClientMock}],
        });
        service = TestBed.inject(EiamAssignmentService);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get available eiam assignments', () => {
        const mockAssignments = [
            {id: 1, text: 'Assignment 1'},
            {id: 2, text: 'Assignment 2'},
        ];
        httpClientMock.get.mockReturnValue(of(mockAssignments));

        service.getAvailableEiamAssignments().subscribe(assignments => {
            expect(assignments).toEqual(mockAssignments);
        });

        expect(httpClientMock.get).toHaveBeenCalledWith('/api/eiam-assignments/available');
    });
});
