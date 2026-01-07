import {HttpClient, HttpHeaders, HttpResponse} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {RequestsAndReportsFilterParameters} from '@api/RequestsAndReportsFilterParameters';
import {toDateOnlyString} from '@shared/DateAdapter';
import {Observable} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class RequestsAndReportsService {
    private static readonly WORD_ACCEPT_HEADER = 'application/vnd.openxmlformats-officedocument.wordprocessingml.document';

    constructor(private readonly http: HttpClient) {}

    generateReport(filterParameters: RequestsAndReportsFilterParameters): Observable<HttpResponse<Blob>> {
        const headers = new HttpHeaders({
            'Content-Type': 'application/json',
            Accept: RequestsAndReportsService.WORD_ACCEPT_HEADER,
        });

        const body = {
            documentType: filterParameters?.documentType,
            analysisDate1: filterParameters?.analysisDate1 ? toDateOnlyString(filterParameters.analysisDate1) : null,
            analysisDate2: filterParameters?.analysisDate2 ? toDateOnlyString(filterParameters.analysisDate2) : null,
            departmentIds: filterParameters?.departments || [],
            officeIds: filterParameters?.offices || [],
            committeeTypeIds: filterParameters?.committeeTypes || [],
            committeeIds: filterParameters?.committees || [],
            isGeneralElection: filterParameters.isGeneralElection,
            committeesWithActiveMembership: filterParameters.committeesWithActiveMembership,
            releasedCommittees: filterParameters.releasedCommittees,
        };

        return this.http.post('/api/Report/download', body, {
            headers,
            observe: 'response' as const,
            responseType: 'blob' as const,
        });
    }
}
