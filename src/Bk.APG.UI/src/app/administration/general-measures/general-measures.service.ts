import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {GeneralMeasure} from '@api/GeneralMeasure';
import {GeneralMeasureUpdate} from '@api/GeneralMeasureUpdate';

@Injectable({
    providedIn: 'root',
})
export class GeneralMeasuresService {
    constructor(private readonly http: HttpClient) {}

    getGeneralMeasures() {
        return this.http.get<GeneralMeasure[]>('/api/general-measures');
    }

    saveGeneralMeasure(generalMeasureUpdate: GeneralMeasureUpdate) {
        return this.http.put('/api/general-measures', generalMeasureUpdate);
    }

    validate(departmentId: string) {
        return this.http.post<void>(`/api/general-measures/${departmentId}/validate`, {});
    }

    forward(departmentId: string, message: string, forwardToAdmin: boolean) {
        return this.http.post<void>(`/api/general-measures/${departmentId}/forward`, {message, forwardToAdmin});
    }
}
