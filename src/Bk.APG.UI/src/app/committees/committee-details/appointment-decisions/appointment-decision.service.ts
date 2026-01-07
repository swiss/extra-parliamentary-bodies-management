import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {AppointmentDecisionCreate} from '@api/AppointmentDecisionCreate';
import {AppointmentDecisionList} from '@api/AppointmentDecisionList';
import {AppointmentDecisionUpdate} from '@api/AppointmentDecisionUpdate';
import {toDateOnlyString} from '@shared/DateAdapter';
import {Subject, Observable} from 'rxjs';

/* eslint-disable @typescript-eslint/no-explicit-any */
@Injectable({
    providedIn: 'root',
})
export class AppointmentDecisionService {
    reload$ = new Subject<void>();

    constructor(private readonly http: HttpClient) {}

    getAppointmentDecisionList(committeeId: string): Observable<AppointmentDecisionList[]> {
        return this.http.get<AppointmentDecisionList[]>(`/api/appointmentDecisions/${committeeId}/list`);
    }

    getAppointmentDecisionForCreate(): Observable<AppointmentDecisionCreate> {
        return this.http.get<AppointmentDecisionCreate>(`/api/appointmentDecisions/create`);
    }

    getAppointmentDecisionForUpdate(appointmentDecisionId: string): Observable<AppointmentDecisionUpdate> {
        return this.http.get<AppointmentDecisionUpdate>(`/api/appointmentDecisions/${appointmentDecisionId}/update`);
    }

    createAppointmentDecision(appointmentDecision: AppointmentDecisionCreate): Observable<AppointmentDecisionList> {
        const formData = this.getFormData(appointmentDecision);
        return this.http.post<AppointmentDecisionList>('/api/appointmentDecisions', formData);
    }

    updateAppointmentDecision(id: string, appointmentDecision: AppointmentDecisionUpdate): Observable<AppointmentDecisionList> {
        const formData = this.getFormData(appointmentDecision);
        return this.http.put<AppointmentDecisionList>(`/api/appointmentDecisions/${id}`, formData);
    }

    deleteAppointmentDecision(appointmentDecisionId: string): Observable<AppointmentDecisionList[]> {
        return this.http.delete<AppointmentDecisionList[]>(`/api/appointmentDecisions/${appointmentDecisionId}`);
    }

    downloadFile(id: string): Observable<Blob> {
        const params = new HttpParams().set('id', id);

        return this.http.get(`/api/appointmentDecisions/document`, {params, responseType: 'blob'});
    }

    private getFormData(appointmentDecision: AppointmentDecisionCreate | AppointmentDecisionUpdate): FormData {
        const formData: FormData = new FormData();

        // add appointment decision properties
        for (const key in appointmentDecision) {
            if (key !== 'documents') {
                if (Object.prototype.hasOwnProperty.call(appointmentDecision, key) && (appointmentDecision as any)[key]) {
                    if (key.endsWith('Date')) {
                        formData.append(key, toDateOnlyString(new Date((appointmentDecision as any)[key])));
                    } else {
                        formData.append(key, (appointmentDecision as any)[key]);
                    }
                }
            }
        }
        // add document properties
        for (let i = 0; i < appointmentDecision.documents.length; i++) {
            const document = appointmentDecision.documents[i];
            for (const key in document) {
                if (key !== 'file') {
                    formData.append(`documents[${i}][${key}]`, (document as any)[key]);
                }
            }
            // add the document itself
            if (document.file instanceof Blob) {
                formData.append(`documents[${i}].file`, document.file, document.file.name);
            }
        }
        return formData;
    }
}
