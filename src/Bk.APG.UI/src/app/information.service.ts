import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {VersionDto} from '@api/VersionDto';
import {first} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class InformationService {
    applicationVersion?: string;

    constructor(private readonly http: HttpClient) {}

    loadApplicationVersion(): void {
        this.http
            .get<VersionDto>('/api/information/version')
            .pipe(first())
            .subscribe(v => (this.applicationVersion = v.applicationVersion));
    }
}
