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
        return this.http.get<CommitteeTypeList[]>('/api/committeeTypes/list');
    }

    getCommitteeTypeForUpdate(committeeTypeId: string): Observable<CommitteeTypeUpdate> {
        return this.http.get<CommitteeTypeUpdate>(`/api/committeeTypes/${committeeTypeId}/update`);
    }

    updateCommitteeType(committeeType: CommitteeTypeUpdate): Observable<CommitteeTypeUpdate> {
        return this.http.put<CommitteeTypeUpdate>(`/api/committeeTypes/${committeeType.id}`, committeeType);
    }
}
