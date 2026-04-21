import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class OpenDataStackService {
    constructor(private readonly http: HttpClient) {}

    exchangeToken(): Observable<string> {
        return this.http.post('/api/opendatastack/token', {}, {responseType: 'text'});
    }
}
