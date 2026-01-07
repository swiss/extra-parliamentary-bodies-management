import {HttpClient, HttpHeaders, HttpResponse} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ExportType} from '@api/DataAnalysis';
import {toDateOnlyString} from '@shared/DateAdapter';
import {Observable} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class DataAnalysisService {
    private static readonly EXCEL_ACCEPT_HEADER = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';

    constructor(private readonly http: HttpClient) {}

    generateExport(exportType: ExportType, date: Date): Observable<HttpResponse<Blob>> {
        const headers = new HttpHeaders().set('Accept', DataAnalysisService.EXCEL_ACCEPT_HEADER);

        return this.http.get<Blob>(`/api/data-analysis/${exportType}/${toDateOnlyString(date)}`, {
            headers,
            observe: 'response',
            responseType: 'blob' as 'json',
        });
    }
}
