import {HttpClient} from '@angular/common/http';
import {TestBed} from '@angular/core/testing';
import {RecipientsFilterParameters} from '@api/RecipientsFilterParameters';
import {of} from 'rxjs';
import {RecipientsService} from './recipients.service';

describe('RecipientsService', () => {
    let service: RecipientsService;
    let httpClientMock: jest.Mocked<HttpClient>;

    beforeEach(() => {
        httpClientMock = {
            get: jest.fn(),
            post: jest.fn(),
        } as unknown as jest.Mocked<HttpClient>;

        TestBed.configureTestingModule({
            providers: [RecipientsService, {provide: HttpClient, useValue: httpClientMock}],
        });

        service = TestBed.inject(RecipientsService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should call HttpClient.post with correct URL, headers, and options', () => {
        const filterParams = {
            departments: ['dep1', 'dep2'],
            offices: ['off1', 'off2'],
            committeeTypes: ['type1', 'type2'],
            committees: ['comm1', 'comm2'],
            correspondenceLanguages: ['lang1', 'lang2'],
            electionTypes: ['elect1', 'elect2'],
            formLetterSender: 'id',
        } as RecipientsFilterParameters;
        const expectedUrl = '/api/Report/downloadFormLetter';

        const response = {body: new Blob()} as unknown;
        httpClientMock.post.mockReturnValue(of(response));

        service.generateReport(filterParams).subscribe(res => {
            expect(res).toBe(response);
        });
        const body = {
            committeeTypeIds: ['type1', 'type2'],
            committeeIds: ['comm1', 'comm2'],
            departmentIds: ['dep1', 'dep2'],
            officeIds: ['off1', 'off2'],
            correspondenceLanguageIds: ['lang1', 'lang2'],
            electionTypeIds: ['elect1', 'elect2'],
            exportFileType: null,
            exportType: null,
            formLetterSenderId: 'id',
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
});
