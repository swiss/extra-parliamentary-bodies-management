import {HttpClient, HttpHeaders} from '@angular/common/http';
import {TestBed} from '@angular/core/testing';
import {RequestsAndReportsFilterParameters} from '@api/RequestsAndReportsFilterParameters';
import {of} from 'rxjs';
import {ReportType} from '../ReportType';
import {RequestsAndReportsService} from './requests-and-reports.service';

describe('RequestsAndReportsService', () => {
    let service: RequestsAndReportsService;
    let httpClientMock: jest.Mocked<HttpClient>;

    beforeEach(() => {
        httpClientMock = {
            post: jest.fn(),
        } as unknown as jest.Mocked<HttpClient>;

        TestBed.configureTestingModule({providers: [RequestsAndReportsService, {provide: HttpClient, useValue: httpClientMock}]});
        service = TestBed.inject(RequestsAndReportsService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should call HttpClient.post with correct URL, headers, and options', () => {
        const filterParams = {
            analysisDate1: new Date('2024-01-01T00:00:00.000Z'),
            analysisDate2: new Date('2024-01-02T00:00:00.000Z'),
            departments: ['dep1', 'dep2'],
            offices: ['off1', 'off2'],
            committeeTypes: ['type1', 'type2'],
            committees: ['comm1', 'comm2'],
            documentType: ReportType.AppendixFederalCouncil,
            isGeneralElection: true,
            releasedCommittees: true,
        } as RequestsAndReportsFilterParameters;
        const expectedUrl = '/api/reports/download';

        const response = {body: new Blob()} as unknown;
        httpClientMock.post.mockReturnValue(of(response));

        service.generateReport(filterParams).subscribe(res => {
            expect(res).toBe(response);
        });
        const body = {
            analysisDate1: '2024-01-01',
            analysisDate2: '2024-01-02',
            committeeTypeIds: ['type1', 'type2'],
            committeeIds: ['comm1', 'comm2'],
            departmentIds: ['dep1', 'dep2'],
            documentType: ReportType.AppendixFederalCouncil,
            isGeneralElection: true,
            releasedCommittees: true,
            officeIds: ['off1', 'off2'],
        };
        expect(httpClientMock.post).toHaveBeenCalledWith(
            expectedUrl,
            body,
            expect.objectContaining({
                observe: 'response',
                responseType: 'blob',
            })
        );
    });

    it('should set Accept header to Word MIME type', () => {
        const date = new Date('2024-01-01T00:00:00.000Z');
        const filterParams = {analysisDate1: date} as RequestsAndReportsFilterParameters;
        httpClientMock.post.mockReturnValue(of({} as unknown));

        service.generateReport(filterParams).subscribe();

        const callArgs = httpClientMock.post.mock.calls[0];
        const options = callArgs[2];
        const acceptHeader = (options!.headers as HttpHeaders).get('Accept');
        expect(acceptHeader).toBe('application/vnd.openxmlformats-officedocument.wordprocessingml.document');
    });

    it('should set responseType to blob and observe to response', () => {
        const date = new Date('2024-01-01T00:00:00.000Z');
        const filterParams = {analysisDate1: date} as RequestsAndReportsFilterParameters;
        httpClientMock.post.mockReturnValue(of({} as unknown));

        service.generateReport(filterParams).subscribe();

        const callArgs = httpClientMock.post.mock.calls[0];
        const options = callArgs[2];
        expect(options!.responseType).toBe('blob');
        expect(options!.observe).toBe('response');
    });
});
