import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {MembershipCandidateTermCalculation} from '@api/MembershipCandidateTermCalculation';
import {MembershipCandidateUpdate} from '@api/MembershipCandidateUpdate';
import {toDateOnlyString} from '@shared/DateAdapter';
import {Observable, Subject} from 'rxjs';

@Injectable({providedIn: 'root'})
export class MembershipCandidateService {
    reload$ = new Subject<void>();

    constructor(private readonly http: HttpClient) {}

    getMembershipCandidateForUpdate(id: string): Observable<MembershipCandidateUpdate> {
        return this.http.get<MembershipCandidateUpdate>(`/api/general-election/membership-candidates/${id}/update`);
    }

    updateMembershipCandidate(membershipCandidate: MembershipCandidateUpdate): Observable<void> {
        return this.http.put<void>(`/api/general-election/membership-candidates/${membershipCandidate.id}`, membershipCandidate);
    }

    calculateMembershipCandidateTerm(id: string, beginDate: Date, endDate: Date): Observable<MembershipCandidateTermCalculation> {
        return this.http.post<MembershipCandidateTermCalculation>(`/api/general-election/membership-candidates/${id}/calculate-term`, {
            beginDate: toDateOnlyString(beginDate),
            endDate: toDateOnlyString(endDate),
        });
    }
}
