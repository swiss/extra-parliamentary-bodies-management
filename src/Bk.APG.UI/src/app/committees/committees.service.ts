import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {CommitteeCreate} from '@api/CommitteeCreate';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {CommitteeFilterParameters} from '@api/CommitteeFilterParameters';
import {CommitteeJustificationUpdate} from '@api/CommitteeJustificationUpdate';
import {CommitteeList} from '@api/CommitteeList';
import {CommitteeMembershipValidationRequest} from '@api/CommitteeMembershipValidationRequest';
import {CommitteeMembershipValidationResult} from '@api/CommitteeMembershipValidationResult';
import {CommitteeUpdate} from '@api/CommitteeUpdate';
import {MembershipCreate} from '@api/MembershipCreate';
import {MembershipDetails} from '@api/MembershipDetails';
import {MembershipList} from '@api/MembershipList';
import {PagedResult} from '@api/PagedResult';
import {PagingParameters} from '@api/PagingParameters';
import {RequestsAndReportsFilterParameters} from '@api/RequestsAndReportsFilterParameters';
import {SortParameter} from '@api/SortParameter';
import {toDateOnlyString} from '@shared/DateAdapter';
import {append, appendMany, appendPaging, appendSort} from '@shared/http-params-util';
import {BehaviorSubject, map, Observable} from 'rxjs';

@Injectable({providedIn: 'root'})
export class CommitteesService {
    reload$ = new BehaviorSubject<void>(undefined);

    constructor(private readonly http: HttpClient) {}

    getCommitteeList(paging: PagingParameters, filter: CommitteeFilterParameters, sort?: SortParameter): Observable<PagedResult<CommitteeList>> {
        let params = new HttpParams();

        params = appendPaging(params, paging);
        params = this.appendFilter(params, filter);
        params = appendSort(params, sort);

        return this.http.get<PagedResult<CommitteeList>>('/api/committees/list', {params});
    }

    appendFilter = (params: HttpParams, filterParameter?: CommitteeFilterParameters | null): HttpParams => {
        if (filterParameter?.freeText) {
            params = append(params, 'freeText', filterParameter.freeText);
        }
        params = appendMany(params, 'levelIds', filterParameter?.levels);
        params = appendMany(params, 'departmentIds', filterParameter?.departments);
        params = appendMany(params, 'officeIds', filterParameter?.offices);
        params = appendMany(params, 'committeeTypeIds', filterParameter?.committeeTypes);
        params = appendMany(params, 'termIds', filterParameter?.terms);
        params = appendMany(params, 'isActive', filterParameter?.isActive);
        params = appendMany(params, 'isMarketOrientated', filterParameter?.isMarketOrientated);
        params = appendMany(params, 'hasSupervisionDuty', filterParameter?.hasSupervisionDuty);

        return params;
    };

    getCommitteeListForExport(filter: RequestsAndReportsFilterParameters): Observable<CommitteeList[]> {
        let params = new HttpParams();

        params = this.appendExportFilter(params, filter);

        return this.http.get<CommitteeList[]>('/api/committees/listExport', {params});
    }

    appendExportFilter = (params: HttpParams, filterParameter?: RequestsAndReportsFilterParameters | null): HttpParams => {
        params = appendMany(params, 'departmentIds', filterParameter?.departments);
        params = appendMany(params, 'officeIds', filterParameter?.offices);
        params = appendMany(params, 'committeeTypeIds', filterParameter?.committeeTypes);
        params = append(params, 'documentType', filterParameter?.documentType);
        params = append(params, 'analysisDate1', filterParameter?.analysisDate1 ? toDateOnlyString(filterParameter.analysisDate1) : null);
        params = append(params, 'analysisDate2', filterParameter?.analysisDate2 ? toDateOnlyString(filterParameter.analysisDate2) : null);
        return params;
    };

    getCommitteeDetails = (id: string) => this.http.get<CommitteeDetails>(`/api/committees/${id}`);

    getCommitteeForCreate(): Observable<CommitteeCreate> {
        return this.http.get<CommitteeCreate>(`/api/committees/create`);
    }

    getCommitteeForUpdate = (id: string) =>
        this.http.get<CommitteeUpdate>(`/api/committees/${id}/update`).pipe(
            map(committee => ({
                ...committee,
                beginDate: new Date(committee.beginDate),
                endDate: committee.endDate ? new Date(committee.endDate) : undefined,
            }))
        );

    updateCommittee = (committee: CommitteeUpdate) => this.http.put<CommitteeDetails>(`/api/committees/${committee.id}`, committee);

    createCommittee = (committee: CommitteeCreate) => this.http.post<CommitteeDetails>('/api/committees', committee);

    getCommitteeMembers = (id: string) => this.http.get<MembershipList>(`/api/committees/${id}/members`);

    createMember = (membership: MembershipCreate) => this.http.post<MembershipDetails>('/api/committees/member', membership);

    getCommitteesByDescription(desc: string) {
        const params = new HttpParams().set('desc', desc);
        return this.http.get<CommitteeDetails[]>(`/api/committees/getByDescription`, {params});
    }

    getCommitteeJustificationForUpdate = (id: string) => this.http.get<CommitteeJustificationUpdate>(`/api/committees/${id}/justifications`);

    updateCommitteeJustification = (committeeJustification: CommitteeJustificationUpdate) =>
        this.http.put<CommitteeJustificationUpdate>(`/api/committees/${committeeJustification.id}/justifications`, committeeJustification);

    validateMembership(id: string, validationRequest: CommitteeMembershipValidationRequest) {
        let params = new HttpParams();

        if (validationRequest?.committeeid) {
            params = append(params, 'committeeid', validationRequest.committeeid);
        }

        if (validationRequest?.personId) {
            params = append(params, 'personId', validationRequest.personId);
        }

        if (validationRequest?.currentMembershipId) {
            params = append(params, 'currentMembershipId', validationRequest.currentMembershipId);
        }

        if (validationRequest?.beginDate) {
            params = append(params, 'beginDate', toDateOnlyString(validationRequest.beginDate));
        }

        if (validationRequest?.endDate) {
            params = append(params, 'endDate', toDateOnlyString(validationRequest.endDate));
        }

        if (validationRequest?.inCorrelationWithFederalDuty) {
            params = append(params, 'inCorrelationWithFederalDuty', validationRequest?.inCorrelationWithFederalDuty);
        }

        if (validationRequest?.isUpdateMode) {
            params = append(params, 'isUpdateMode', validationRequest?.isUpdateMode);
        }

        return this.http.get<CommitteeMembershipValidationResult>(`/api/committees/${id}/checkMemberships`, {params});
    }
}
