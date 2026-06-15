import {HttpClient, HttpHeaders} from '@angular/common/http';
import {TestBed} from '@angular/core/testing';
import {ExportType} from '@api/DataAnalysis';
import {of} from 'rxjs';
import {DataAnalysisService} from './data-analysis.service';

describe('DataAnalysisService', () => {
    let service: DataAnalysisService;
    let httpClientMock: jest.Mocked<HttpClient>;

    beforeEach(() => {
        httpClientMock = {
            get: jest.fn(),
        } as unknown as jest.Mocked<HttpClient>;

        TestBed.configureTestingModule({
            providers: [DataAnalysisService, {provide: HttpClient, useValue: httpClientMock}],
        });

        service = TestBed.inject(DataAnalysisService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should call HttpClient.get with correct URL, headers, and options', () => {
        const exportType: ExportType = 'committee-types';
        const date = new Date('2024-01-01T00:00:00.000Z');
        const expectedUrl = `/api/data-analysis/${exportType}/2024-01-01`;
        const expectedHeaders = new HttpHeaders().set('Accept', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
        const expectedOptions = {
            headers: expectedHeaders,
            observe: 'response' as const,
            responseType: 'blob' as const,
        };

        const response = {body: new Blob()} as unknown;
        httpClientMock.get.mockReturnValue(of(response));

        service.generateExport(exportType, date).subscribe(res => {
            expect(res).toBe(response);
        });

        expect(httpClientMock.get).toHaveBeenCalledWith(expectedUrl, expectedOptions);
    });

    it('should set Accept header to Excel MIME type', () => {
        const exportType: ExportType = 'persons';
        const date = new Date();
        httpClientMock.get.mockReturnValue(of({} as unknown));

        service.generateExport(exportType, date).subscribe();

        const callArgs = httpClientMock.get.mock.calls[0];
        const options = callArgs[1];
        const acceptHeader = (options!.headers as HttpHeaders).get('Accept');
        expect(acceptHeader).toBe('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
    });

    it('should set responseType to blob and observe to response', () => {
        const exportType: ExportType = 'memberships';
        const date = new Date();
        httpClientMock.get.mockReturnValue(of({} as unknown));

        service.generateExport(exportType, date).subscribe();

        const callArgs = httpClientMock.get.mock.calls[0];
        const options = callArgs[1];
        expect(options!.responseType).toBe('blob');
        expect(options!.observe).toBe('response');
    });
});
