import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {PagedResult} from '@api/PagedResult';
import {PagingParameters} from '@api/PagingParameters';
import {PersonCreate} from '@api/PersonCreate';
import {PersonDetails} from '@api/PersonDetails';
import {PersonFilterParameters} from '@api/PersonFilterParameters';
import {PersonList} from '@api/PersonList';
import {PersonMembership} from '@api/PersonMembership';
import {PersonUpdate} from '@api/PersonUpdate';
import {SortParameter} from '@api/SortParameter';
import {append, appendMany, appendPaging, appendSort} from '@shared/http-params-util';
import {Observable, Subject} from 'rxjs';

@Injectable({providedIn: 'root'})
export class PersonsService {
    reload$ = new Subject<void>();

    constructor(private readonly http: HttpClient) {}

    getPersonList(paging: PagingParameters, filter: PersonFilterParameters, sort?: SortParameter): Observable<PagedResult<PersonList>> {
        let params = new HttpParams();

        params = appendPaging(params, paging);
        params = this.appendFilter(params, filter);
        params = appendSort(params, sort);

        return this.http.get<PagedResult<PersonList>>('/api/persons/list', {params});
    }

    getPersonDetails(id: string): Observable<PersonDetails> {
        return this.http.get<PersonDetails>(`/api/persons/${id}`);
    }

    getPersonForUpdate(id: string): Observable<PersonUpdate> {
        return this.http.get<PersonUpdate>(`/api/persons/${id}/update`);
    }

    getPersonForCreate(): Observable<PersonCreate> {
        return this.http.get<PersonCreate>(`/api/persons/create`);
    }

    appendFilter = (params: HttpParams, filterParameter?: PersonFilterParameters | null): HttpParams => {
        if (filterParameter?.freeText) {
            params = append(params, 'freeText', filterParameter.freeText);
        }
        params = appendMany(params, 'cantonIds', filterParameter?.cantons);
        params = appendMany(params, 'languageIds', filterParameter?.languages);
        params = appendMany(params, 'hasActiveMembership', filterParameter?.hasActiveMembership);

        return params;
    };

    updatePerson(person: PersonUpdate): Observable<PersonUpdate> {
        return this.http.put<PersonUpdate>(`/api/persons/${person.id}`, person);
    }

    createPerson(person: PersonCreate): Observable<PersonUpdate> {
        return this.http.post<PersonUpdate>('/api/persons', person);
    }

    deletePerson(id: string): Observable<void> {
        return this.http.delete<void>(`/api/persons/${id}`);
    }

    getSimilarPersons(surname: string, givenName: string, birthYear: number, birthYearRange: number): Observable<PersonDetails[]> {
        let params = new HttpParams().set('surname', surname);
        params = params.append('givenName', givenName);
        params = params.append('birthYear', birthYear);
        params = params.append('birthYearRange', birthYearRange);

        return this.http.get<PersonDetails[]>('/api/persons/getSimilarPersons', {params});
    }

    getPersonMemberships(id: string) {
        return this.http.get<PersonMembership[]>(`/api/persons/${id}/memberships`);
    }

    getPersonsByName(name: string) {
        const params = new HttpParams().set('name', name);
        return this.http.get<PersonDetails[]>(`/api/persons/getByName`, {params});
    }

    generateSalutation(genderId: string, correspondenceLanguageId: string, surname: string, title?: string) {
        let params = new HttpParams();
        params = params.append('genderId', genderId);
        params = params.append('correspondenceLanguageId', correspondenceLanguageId);
        params = params.append('surname', surname);
        if (title) {
            params = params.append('title', title);
        }

        return this.http.get(`/api/persons/salutation`, {params, responseType: 'text'});
    }
}
