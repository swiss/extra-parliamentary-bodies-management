import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {OpenDataStackDashboard} from '@api/OpenDataStackDashboard';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {ConfigsService} from '../../configs.service';

interface OpenDataStackDashboardResponse {
    count: number;
    result: [
        {
            id: string;
            dashboard_title: string;
            status: string;
        },
    ];
}

@Injectable({
    providedIn: 'root',
})
export class OpenDataStackService {
    constructor(
        private readonly http: HttpClient,
        private readonly configService: ConfigsService
    ) {}

    exchangeToken(): Observable<string> {
        return this.http.post('/api/open-data-stack/token', {}, {responseType: 'text'});
    }

    getDashboards(): Observable<OpenDataStackDashboard[]> {
        return this.http
            .get<OpenDataStackDashboardResponse>(`${this.configService.frontendConfig.openDataStack.baseUrl}/api/v1/dashboard/?q=(page_size:100)`, {
                withCredentials: true,
            })
            .pipe(
                map(response =>
                    response.result.map(dashboard => ({
                        id: dashboard.id,
                        title: dashboard.dashboard_title,
                        status: dashboard.status,
                        embedRedirect: `/superset/dashboard/${dashboard.id}/?standalone=2`,
                    }))
                )
            );
    }
}
