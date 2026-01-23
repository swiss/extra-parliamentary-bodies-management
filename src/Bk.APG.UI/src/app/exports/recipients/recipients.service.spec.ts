import {HttpClient} from '@angular/common/http';
import {TestBed} from '@angular/core/testing';
import {RecipientsService} from './recipients.service';

describe('RecipientsService', () => {
    let service: RecipientsService;
    let httpClientMock: jest.Mocked<HttpClient>;

    beforeEach(() => {
        httpClientMock = {
            get: jest.fn(),
        } as unknown as jest.Mocked<HttpClient>;

        TestBed.configureTestingModule({
            providers: [RecipientsService, {provide: HttpClient, useValue: httpClientMock}],
        });

        service = TestBed.inject(RecipientsService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
