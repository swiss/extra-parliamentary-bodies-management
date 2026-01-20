import {HttpClient, HttpHeaders, HttpResponse} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {CandidateListForward} from '@api/CandidateListForward';
import {CandidateListValidationResult} from '@api/CandidateListValidationResult';
import {EiamAssignment} from '@api/EiamAssignment';
import {MembershipCandidateCreate} from '@api/MembershipCandidateCreate';
import {MembershipCandidateDetail} from '@api/MembershipCandidateDetail';
import {MembershipCandidatePartialUpdate} from '@api/MembershipCandidatePartialUpdate';
import {Observable, Subject} from 'rxjs';

@Injectable({providedIn: 'root'})
export class GeneralElectionCommitteeCandidateListService {
    reload$ = new Subject<void>();

    private static readonly EXCEL_ACCEPT_HEADER = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';

    constructor(private readonly http: HttpClient) {}

    partialUpdateMembershipCandidate(membershipCandidateId: string, membershipCandidatePartialUpdate: MembershipCandidatePartialUpdate): Observable<void> {
        return this.http.patch<void>(`/api/general-election/membership-candidate/${membershipCandidateId}`, membershipCandidatePartialUpdate);
    }

    createMembershipCandidate(membershipCandidateCreate: MembershipCandidateCreate): Observable<MembershipCandidateDetail> {
        return this.http.post<MembershipCandidateDetail>(`/api/general-election/membership-candidate`, membershipCandidateCreate);
    }

    deleteMembershipCandidate(membershipCandidateId: string): Observable<void> {
        return this.http.delete<void>(`/api/general-election/membership-candidate/${membershipCandidateId}`);
    }

    getAssignmentsForCandidateListForward(committeeId: string) {
        return this.http.get<EiamAssignment[]>(`/api/general-election/committees/${committeeId}/candidate-list/forward`);
    }

    forwardCandidateList(committeeId: string, candidateListForward: CandidateListForward) {
        return this.http.post<void>(`/api/general-election/committees/${committeeId}/candidate-list/forward`, candidateListForward);
    }

    validateCandidateList(committeeId: string, selectedCandidateIds: string[], duplicateCheckConfirmed: boolean) {
        const body = {
            selectedCandidateIds,
            duplicateCheckConfirmed,
        };

        return this.http.post<CandidateListValidationResult>(`/api/general-election/committees/${committeeId}/candidate-list/validate`, body);
    }

    saveCandidateList(committeeId: string, selectedIds: string[]) {
        return this.http.post<void>(`/api/general-election/committees/${committeeId}/candidate-list/save`, [...selectedIds]);
    }

    getDuplicateMembershipCandidate(dto: MembershipCandidateCreate) {
        return this.http.post<MembershipCandidateDetail | null>('/api/general-election/committees/getDuplicateMembershipCandidate', dto);
    }

    generateExport(committeeId: string): Observable<HttpResponse<Blob>> {
        const headers = new HttpHeaders().set('Accept', GeneralElectionCommitteeCandidateListService.EXCEL_ACCEPT_HEADER);

        return this.http.get<Blob>(`/api/general-election/committees/${committeeId}/download`, {
            headers,
            observe: 'response',
            responseType: 'blob' as 'json',
        });
    }
}
