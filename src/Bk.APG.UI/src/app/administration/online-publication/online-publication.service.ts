import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class OnlinePublicationService {
    constructor(private readonly http: HttpClient) {}

    setOgdExportSetting(enableOnlinePublication: boolean) {
        return this.http.put('/api/apg-general-settings', enableOnlinePublication);
    }

    getOgdExportSetting(): Observable<boolean> {
        return this.http.get<boolean>('/api/apg-general-settings');
    }

    triggerPublication(): Observable<void> {
        return this.http.post<void>('/api/apg-general-settings/ogd-export', null);
    }
}
