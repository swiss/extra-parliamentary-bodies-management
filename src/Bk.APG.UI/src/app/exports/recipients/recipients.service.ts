import {HttpClient, HttpResponse, HttpHeaders} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {RecipientsFilterParameters} from '@api/RecipientsFilterParameters';
import {Observable} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class RecipientsService {
    private static readonly WORD_ACCEPT_HEADER = 'application/vnd.openxmlformats-officedocument.wordprocessingml.document';

    constructor(private readonly http: HttpClient) {}

    generateReport(filterParameters: RecipientsFilterParameters): Observable<HttpResponse<Blob>> {
        const headers = new HttpHeaders({
            'Content-Type': 'application/json',
        });

        const body = {
            departmentIds: filterParameters?.departments || [],
            officeIds: filterParameters?.offices || [],
            committeeTypeIds: filterParameters?.committeeTypes || [],
            committeeIds: filterParameters?.committees || [],
            correspondenceLanguageIds: filterParameters.correspondenceLanguages || [],
            electionTypeIds: filterParameters.electionTypes || [],
            formLetterSenderId: filterParameters.formLetterSender || null,
            exportType: filterParameters.exportType || null,
            exportFileType: filterParameters.exportFileType || null,
        };

        return this.http.post('/api/Report/downloadFormLetter', body, {
            headers,
            observe: 'response' as const,
            responseType: 'blob' as const,
        });
    }
}
