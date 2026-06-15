import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {CommitteeTypeList} from '@api/CommitteeTypeList';
import {CommitteeTypeUpdate} from '@api/CommitteeTypeUpdate';
import {Observable} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class CommitteeTypeService {
    constructor(private readonly http: HttpClient) {}

    getCommitteeTypeList(): Observable<CommitteeTypeList[]> {
        return this.http.get<CommitteeTypeList[]>('/api/committee-types/list');
    }

    getCommitteeTypeForUpdate(committeeTypeId: string): Observable<CommitteeTypeUpdate> {
        return this.http.get<CommitteeTypeUpdate>(`/api/committee-types/${committeeTypeId}/update`);
    }

    updateCommitteeType(committeeType: CommitteeTypeUpdate): Observable<CommitteeTypeUpdate> {
        return this.http.put<CommitteeTypeUpdate>(`/api/committee-types/${committeeType.id}`, committeeType);
    }
}
