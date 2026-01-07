import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Address} from '@api/Address';
import {AddressSearchDto} from '@api/AddressSearchDto';
import {ObHttpApiInterceptorEvents} from '@oblique/oblique';
import {append} from '@shared/http-params-util';
import {Observable} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class AddressService {
    constructor(
        private readonly http: HttpClient,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {}

    getAddressSuggestions(addressSearch: AddressSearchDto): Observable<Address[]> {
        let params = new HttpParams();
        params = this.appendParams(params, addressSearch);

        this.httpApiInterceptorEvents.deactivateSpinnerOnNextAPICalls(1);
        return this.http.get<Address[]>('/api/addresses/search', {params});
    }

    private readonly appendParams = (params: HttpParams, searchParams?: AddressSearchDto | null): HttpParams => {
        if (searchParams?.street) {
            params = append(params, 'street', searchParams.street);
        }
        if (searchParams?.zip) {
            params = append(params, 'zip', searchParams.zip);
        }
        if (searchParams?.city) {
            params = append(params, 'city', searchParams.city);
        }

        return params;
    };
}
