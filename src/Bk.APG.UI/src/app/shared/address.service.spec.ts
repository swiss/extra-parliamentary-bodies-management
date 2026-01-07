import {HttpClient, HttpParams} from '@angular/common/http';
import {AddressSearchDto} from '@api/AddressSearchDto';
import {ObHttpApiInterceptorEvents} from '@oblique/oblique';
import {of} from 'rxjs';
import {AddressService} from './address.service';

describe('AddressService', () => {
    let service: AddressService;

    const httpClientMock = {
        get: jest.fn(() => of()),
    } as unknown as jest.Mocked<HttpClient>;

    const httpApiInterceptorEventsMock = {
        deactivateSpinnerOnNextAPICalls: jest.fn(),
    } as unknown as jest.Mocked<ObHttpApiInterceptorEvents>;

    const searchDto: AddressSearchDto = {
        street: 'Bahnhofstrasse',
        zip: '5400',
        city: 'Baden',
    };

    beforeEach(() => {
        service = new AddressService(httpClientMock, httpApiInterceptorEventsMock);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get address suggestions', () => {
        let response;

        service.getAddressSuggestions(searchDto).subscribe(value => (response = value));

        const expectedParams = new HttpParams().set('street', 'Bahnhofstrasse').set('zip', '5400').set('city', 'Baden');

        expect(httpApiInterceptorEventsMock.deactivateSpinnerOnNextAPICalls).toHaveBeenCalledWith(1);
        expect(httpClientMock.get).toHaveBeenCalledWith(
            '/api/addresses/search',
            expect.objectContaining({
                params: expectedParams,
            })
        );
        expect(response).toBeUndefined();
    });
});
