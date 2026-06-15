import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {InterestUpdate} from '@api/InterestUpdate';
import {UidResult} from '@api/UidResult';
import {ObHttpApiInterceptorEvents} from '@oblique/oblique';
import {Observable} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class PersonInterestsService {
    constructor(
        private readonly http: HttpClient,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {}

    getInterestsByPersonId(personId: string): Observable<InterestUpdate[]> {
        return this.http.get<InterestUpdate[]>(`/api/persons/${personId}/interests`);
    }

    saveInterestForPerson(personId: string, interests: InterestUpdate[]): Observable<InterestUpdate[]> {
        return this.http.put<InterestUpdate[]>(`/api/persons/${personId}/interests`, interests);
    }

    getUidOrganizations(searchText: string): Observable<UidResult[]> {
        let params = new HttpParams();
        params = params.append('organizationName', searchText);

        this.httpApiInterceptorEvents.deactivateSpinnerOnNextAPICalls(1);
        return this.http.get<UidResult[]>('/api/uid/search', {params});
    }
}
