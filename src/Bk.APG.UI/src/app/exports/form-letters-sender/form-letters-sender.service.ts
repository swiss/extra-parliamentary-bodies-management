import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {FormLettersSenderCreate} from '@api/FormLettersSenderCreate';
import {FormLettersSenderList} from '@api/FormLettersSenderList';
import {FormLettersSenderUpdate} from '@api/FormLettersSenderUpdate';
import {Observable, Subject} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class FormLettersSenderService {
    reload$ = new Subject<void>();

    constructor(private readonly http: HttpClient) {}

    getFormLettersSenderList(): Observable<FormLettersSenderList[]> {
        return this.http.get<FormLettersSenderList[]>('/api/form-letter-senders/list');
    }

    getEmptyFormLettersSender(): Observable<FormLettersSenderCreate> {
        return this.http.get<FormLettersSenderCreate>('/api/form-letter-senders/empty');
    }

    getFormLettersSenderForUpdate(id: string): Observable<FormLettersSenderUpdate> {
        return this.http.get<FormLettersSenderUpdate>(`/api/form-letter-senders/${id}/update`);
    }

    createFormLettersSender(sender: FormLettersSenderCreate): Observable<FormLettersSenderUpdate> {
        const formData = this.getFormData(sender);
        return this.http.post<FormLettersSenderUpdate>('/api/form-letter-senders', formData);
    }

    updateFormLettersSender(sender: FormLettersSenderUpdate): Observable<FormLettersSenderUpdate> {
        const formData = this.getFormData(sender);
        return this.http.put<FormLettersSenderUpdate>(`/api/form-letter-senders/${sender.id}`, formData);
    }

    deleteFormLettersSender(id: string): Observable<void> {
        return this.http.delete<void>(`/api/form-letter-senders/${id}`);
    }

    private getFormData(sender: FormLettersSenderCreate | FormLettersSenderUpdate): FormData {
        const formData = new FormData();

        // Add all properties except signature (file)
        for (const key of Object.keys(sender)) {
            if (key !== 'signature') {
                const value = (sender as unknown as Record<string, unknown>)[key];
                if (value !== null && value !== undefined && value !== '') {
                    formData.append(key, String(value));
                }
            }
        }

        if (sender.signature) {
            formData.append('signature', sender.signature, sender.signature.name);
        }

        return formData;
    }
}
