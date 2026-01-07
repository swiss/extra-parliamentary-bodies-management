import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {MembershipDetails} from '@api/MembershipDetails';
import {MembershipUpdate} from '@api/MembershipUpdate';
import {Observable, Subject} from 'rxjs';

@Injectable({providedIn: 'root'})
export class MembershipsService {
    reload$ = new Subject<void>();

    constructor(private readonly http: HttpClient) {}

    getMembershipForUpdate(id: string): Observable<MembershipUpdate> {
        return this.http.get<MembershipUpdate>(`/api/memberships/${id}/update`);
    }

    updateMembership(membership: MembershipUpdate): Observable<MembershipDetails> {
        return this.http.put<MembershipDetails>(`/api/memberships/${membership.id}`, membership);
    }

    deleteMembership(membershipId: string) {
        return this.http.delete(`/api/memberships/${membershipId}`);
    }
}
