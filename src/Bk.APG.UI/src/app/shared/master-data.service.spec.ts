import {HttpClient, HttpParams} from '@angular/common/http';
import {TestBed, waitForAsync} from '@angular/core/testing';
import {TranslateService} from '@ngx-translate/core';
import {of} from 'rxjs';
import {MasterDataService} from './master-data.service';

describe('MasterDataService', () => {
    let service: MasterDataService;

    const httpClientMock = {
        get: jest.fn(() => of([])),
    } as unknown as jest.Mocked<HttpClient>;

    const translateServiceMock = {
        onLangChange: of({lang: 'en'}),
        getCurrentLang: jest.fn(() => 'en'),
    } as unknown as jest.Mocked<TranslateService>;

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                {provide: HttpClient, useValue: httpClientMock},
                {provide: TranslateService, useValue: translateServiceMock},
            ],
        });
        service = TestBed.inject(MasterDataService);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get masterdata on language change', waitForAsync(() => {
        const httpGetSpy = jest.spyOn(httpClientMock, 'get');

        translateServiceMock.onLangChange.subscribe(() => {
            expect(httpGetSpy).toHaveBeenCalledWith('/api/masterData');
        });
    }));

    it('should make http call when getOfficeByName is called', waitForAsync(async () => {
        const mockResponse = {
            items: [
                {id: 1, description: 'Office 1'},
                {id: 2, description: 'Office 3'},
            ],
        };
        httpClientMock.get.mockReturnValue(of(mockResponse));

        service.getOfficeByName('myId').subscribe(response => {
            expect(response).toBeTruthy();
        });

        const expectedParams = new HttpParams().set('officeName', 'myId');

        expect(httpClientMock.get).toHaveBeenCalledWith(
            '/api/masterData/offices/search',
            expect.objectContaining({
                params: expectedParams,
            })
        );
    }));
});
