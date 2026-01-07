import {HttpClient} from '@angular/common/http';
import {TestBed} from '@angular/core/testing';
import {of} from 'rxjs';
import {OnlinePublicationService} from './online-publication.service';

describe('OnlinePublicationService', () => {
    let service: OnlinePublicationService;

    const httpClientMock = {
        get: jest.fn(() => of()),
        put: jest.fn(() => of()),
    } as unknown as jest.Mocked<HttpClient>;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = new OnlinePublicationService(httpClientMock);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get general settings value', async () => {
        httpClientMock.get.mockReturnValue(of(true));
        const response = await service.getOgdExportSetting();

        expect(response).toBeTruthy();

        expect(httpClientMock.get).toHaveBeenCalledWith('/api/apgGeneralSettings');
    });

    it('should update general settings value', () => {
        service.setOgdExportSetting(true);

        expect(httpClientMock.put).toHaveBeenCalledWith('/api/apgGeneralSettings', true);
    });
});
