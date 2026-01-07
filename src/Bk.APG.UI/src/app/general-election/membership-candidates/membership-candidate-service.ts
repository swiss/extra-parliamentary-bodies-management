import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {MembershipCandidateUpdate} from '@api/MembershipCandidateUpdate';
import {Observable, Subject} from 'rxjs';

@Injectable({providedIn: 'root'})
export class MembershipCandidateService {
    reload$ = new Subject<void>();

    constructor(private readonly http: HttpClient) {}

    getMembershipCandidateForUpdate(id: string): Observable<MembershipCandidateUpdate> {
        return this.http.get<MembershipCandidateUpdate>(`/api/general-election/membership-candidate/${id}/update`);
    }

    updateMembershipCandidate(membershipCandidate: MembershipCandidateUpdate): Observable<void> {
        return this.http.put<void>(`/api/general-election/membership-candidate/${membershipCandidate.id}`, membershipCandidate);
    }
}
