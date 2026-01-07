import {HttpClient, HttpParams} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {PagedResult} from '@api/PagedResult';
import {PagingParameters} from '@api/PagingParameters';
import {SortParameter} from '@api/SortParameter';
import {WorklistFilterParameters} from '@api/WorklistFilterParameters';
import {WorklistTask} from '@api/WorklistTask';
import {WorklistTaskCreate} from '@api/WorklistTaskCreate';
import {WorklistTaskForward} from '@api/WorklistTaskForward';
import {WorklistTaskUpdate} from '@api/WorklistTaskUpdate';
import {toDateOnlyString} from '@shared/DateAdapter';
import {append, appendMany, appendPaging, appendSort} from '@shared/http-params-util';

@Injectable()
export class WorklistService {
    readonly #http = inject(HttpClient);

    getWorklistTasks(paging: PagingParameters, sort: SortParameter, filter: WorklistFilterParameters) {
        let params = new HttpParams();
        params = appendPaging(params, paging);
        params = this.appendFilter(params, filter);
        params = appendSort(params, sort);
        return this.#http.get<PagedResult<WorklistTask>>('/api/worklist-tasks', {params});
    }

    create(worklistTask: WorklistTaskCreate) {
        return this.#http.post('/api/worklist-tasks', worklistTask);
    }

    getWorklistTaskForUpdate(id: string) {
        return this.#http.get<WorklistTaskUpdate>(`/api/worklist-tasks/${id}`);
    }

    update(id: string, worklistTask: WorklistTaskUpdate) {
        return this.#http.put(`/api/worklist-tasks/${id}`, worklistTask);
    }

    forward(id: string, worklistTaskForward: WorklistTaskForward) {
        return this.#http.post(`/api/worklist-tasks/${id}/forward`, worklistTaskForward);
    }

    appendFilter = (params: HttpParams, filterParameters?: WorklistFilterParameters | null): HttpParams => {
        params = append(params, 'committee', filterParameters?.committee);
        params = appendMany(params, 'departmentIds', filterParameters?.departments);
        params = appendMany(params, 'officeIds', filterParameters?.offices);
        params = appendMany(params, 'worklistTaskStateIds', filterParameters?.worklistTaskStates);
        params = appendMany(params, 'worklistTaskTypeIds', filterParameters?.worklistTaskTypes);
        params = append(params, 'assignedBy', filterParameters?.assignedBy);
        params = append(params, 'assignedTo', filterParameters?.assignedTo);
        params = append(params, 'createdFrom', filterParameters?.createdFrom ? toDateOnlyString(filterParameters?.createdFrom) : null);
        params = append(params, 'createdTo', filterParameters?.createdTo ? toDateOnlyString(filterParameters?.createdTo) : null);
        params = append(params, 'dueDateFrom', filterParameters?.dueDateFrom ? toDateOnlyString(filterParameters?.dueDateFrom) : null);
        params = append(params, 'dueDateTo', filterParameters?.dueDateTo ? toDateOnlyString(filterParameters?.dueDateTo) : null);

        return params;
    };
}
