import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {EiamAssignment} from '@api/EiamAssignment';

@Injectable({
    providedIn: 'root',
})
export class EiamAssignmentService {
    constructor(private readonly http: HttpClient) {}

    getCurrentEiamAssignment() {
        return this.http.get<EiamAssignment>('/api/eiam-assignments/current');
    }

    getAvailableEiamAssignments() {
        return this.http.get<EiamAssignment[]>('/api/eiam-assignments/available');
    }
}
