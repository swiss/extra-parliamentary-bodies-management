import {HttpClient} from '@angular/common/http';
import {firstValueFrom, of} from 'rxjs';
import {CommitteeTypeService} from './committee-type.service';

describe('CommitteeTypeService', () => {
    let service: CommitteeTypeService;

    const httpClientMock = {
        get: jest.fn(() => of()),
        put: jest.fn(() => of()),
    } as unknown as jest.Mocked<HttpClient>;

    beforeEach(() => {
        service = new CommitteeTypeService(httpClientMock);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get committee type list', async () => {
        const mockResponse = {
            id: '1',
            text: 'Description 1',
        };
        httpClientMock.get.mockReturnValue(of(mockResponse));

        const response = await firstValueFrom(service.getCommitteeTypeList());
        expect(response).toBeTruthy();

        expect(httpClientMock.get).toHaveBeenCalledWith('/api/committee-types/list');
    });

    it('should get committee type detail', async () => {
        const mockResponse = {
            id: '1',
            text: 'Dext 1',
        };
        httpClientMock.get.mockReturnValue(of(mockResponse));

        const response = await service.getCommitteeTypeForUpdate('1');
        expect(response).toBeTruthy();

        expect(httpClientMock.get).toHaveBeenCalledWith('/api/committee-types/1/update');
    });

    it('should update committee type', () => {
        service.updateCommitteeType({
            id: '1',
            text: 'text',
            femaleThreshold: 15,
            maleThreshold: 15,
            germanMinimalThreshold: undefined,
            frenchMinimalThreshold: undefined,
            italianMinimalThreshold: undefined,
            romanshMinimalThreshold: undefined,
            germanThresholdPercentage: 25,
            frenchThresholdPercentage: 25,
            italianThresholdPercentage: 25,
            romanshThresholdPercentage: 10,
        });

        expect(httpClientMock.put).toHaveBeenCalledWith('/api/committee-types/1', {
            id: '1',
            text: 'text',
            femaleThreshold: 15,
            maleThreshold: 15,
            germanMinimalThreshold: undefined,
            frenchMinimalThreshold: undefined,
            italianMinimalThreshold: undefined,
            romanshMinimalThreshold: undefined,
            germanThresholdPercentage: 25,
            frenchThresholdPercentage: 25,
            italianThresholdPercentage: 25,
            romanshThresholdPercentage: 10,
        });
    });
});
