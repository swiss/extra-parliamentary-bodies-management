import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {GeneralElectionCommitteeFilterParameters} from '@api/GeneralElectionCommitteeFilterParameters';
import {GeneralElectionCommitteeJustificationUpdate} from '@api/GeneralElectionCommitteeJustificationUpdate';
import {GeneralElectionCommitteeList} from '@api/GeneralElectionCommitteeList';
import {MembershipList} from '@api/MembershipList';
import {PagedResult} from '@api/PagedResult';
import {PagingParameters} from '@api/PagingParameters';
import {RecipientsFilterParameters} from '@api/RecipientsFilterParameters';
import {SortParameter} from '@api/SortParameter';
import {append, appendMany, appendPaging, appendSort} from '@shared/http-params-util';
import {Observable} from 'rxjs';

@Injectable()
export class GeneralElectionCommitteesService {
    constructor(private readonly http: HttpClient) {}

    getGeneralElectionCommitteeList(
        paging: PagingParameters,
        filter: GeneralElectionCommitteeFilterParameters,
        sort?: SortParameter
    ): Observable<PagedResult<GeneralElectionCommitteeList>> {
        let params = new HttpParams();

        params = appendPaging(params, paging);
        params = this.appendFilter(params, filter);
        params = appendSort(params, sort);

        return this.http.get<PagedResult<GeneralElectionCommitteeList>>('/api/general-election/committees/list', {params});
    }

    appendFilter = (params: HttpParams, filterParameter?: GeneralElectionCommitteeFilterParameters | null): HttpParams => {
        if (filterParameter?.freeText) {
            params = append(params, 'freeText', filterParameter.freeText);
        }
        params = appendMany(params, 'departmentIds', filterParameter?.departments);
        params = appendMany(params, 'officeIds', filterParameter?.offices);
        params = appendMany(params, 'committeeTypeIds', filterParameter?.committeeTypes);
        params = appendMany(params, 'isMarketOrientated', filterParameter?.isMarketOrientated);
        params = appendMany(params, 'hasSupervisionDuty', filterParameter?.hasSupervisionDuty);
        params = appendMany(params, 'vacancies', filterParameter?.vacancies);
        params = appendMany(params, 'isNew', filterParameter?.isNew);
        params = appendMany(params, 'statusProposal', filterParameter?.statusProposal);

        return params;
    };

    getGeneralElectionCommitteeMembers = (id: string) => this.http.get<MembershipList>(`/api/general-election/committees/${id}/members`);

    getGeneralElectionCommitteeJustificationForUpdate = (id: string) =>
        this.http.get<GeneralElectionCommitteeJustificationUpdate>(`/api/general-election/committees/${id}/justifications`);

    updateGeneralElectionCommitteeJustification = (committeeJustification: GeneralElectionCommitteeJustificationUpdate) =>
        this.http.put<GeneralElectionCommitteeJustificationUpdate>(
            `/api/general-election/committees/${committeeJustification.id}/justifications`,
            committeeJustification
        );

    updateGeneralElectionCommitteeVacancies = (committeeId: string, vacancies: number) =>
        this.http.put<GeneralElectionCommitteeJustificationUpdate>(`/api/general-election/committees/${committeeId}/vacancies`, vacancies);

    getGeneralElectionCommitteeListForRecipientExport(filter: RecipientsFilterParameters): Observable<GeneralElectionCommitteeList[]> {
        let params = new HttpParams();

        params = this.appendExportFilter(params, filter);

        return this.http.get<GeneralElectionCommitteeList[]>('/api/general-election/committees/recipient', {params});
    }

    getUnfinishedGeneralElectionCommitteeList(): Observable<GeneralElectionCommitteeList[]> {
        return this.http.get<GeneralElectionCommitteeList[]>('/api/general-election/committees/checkCompletion');
    }

    appendExportFilter = (params: HttpParams, filterParameter?: RecipientsFilterParameters | null): HttpParams => {
        params = appendMany(params, 'departmentIds', filterParameter?.departments);
        params = appendMany(params, 'officeIds', filterParameter?.offices);
        params = appendMany(params, 'committeeTypeIds', filterParameter?.committeeTypes);
        params = appendMany(params, 'correspondenceLanguageIds', filterParameter?.correspondenceLanguages);
        params = appendMany(params, 'electionTypeIds', filterParameter?.electionTypes);

        return params;
    };
}
