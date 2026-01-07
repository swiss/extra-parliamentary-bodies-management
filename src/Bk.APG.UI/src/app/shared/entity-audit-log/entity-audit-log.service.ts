import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {EntityAuditLog} from '@api/EntityAuditLog';
import {PagedResult} from '@api/PagedResult';
import {PagingParameters} from '@api/PagingParameters';
import {SortParameter} from '@api/SortParameter';
import {append, appendMany, appendPaging, appendSort} from '@shared/http-params-util';
import {Subject} from 'rxjs';

export type EntityType = 'person' | 'committee' | 'generalElectionCommittee';
@Injectable({providedIn: 'root'})
export class EntityAuditLogService {
    readonly reload$ = new Subject<void>();

    constructor(private readonly http: HttpClient) {}

    getEntityAuditLogs(entityId: string, entityType: EntityType, relatedEntityIds: string[], paging: PagingParameters, sort?: SortParameter) {
        let params = new HttpParams();

        params = append(params, 'entityType', entityType);
        params = appendPaging(params, paging);
        params = appendSort(params, sort);
        params = appendMany(params, 'relatedEntityIds', relatedEntityIds);

        return this.http.get<PagedResult<EntityAuditLog>>(`/api/entityAuditLog/${entityId}`, {params});
    }
}
