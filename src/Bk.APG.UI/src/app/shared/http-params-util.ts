import {HttpParams} from '@angular/common/http';
import {PagingParameters} from '../../api/PagingParameters';
import {SortParameter} from '../../api/SortParameter';

export interface AppendOptions {
    allowFalse: boolean;
}

export const appendMany = (params: HttpParams, name: string, values?: number[] | string[] | boolean[] | (string | number | boolean)[] | null): HttpParams => {
    if (!values?.length) {
        return params;
    }

    values.forEach(id => {
        params = params.append(name, id);
    });

    return params;
};

export const append = (
    params: HttpParams,
    name: string,
    value?: string | Date | boolean | number | null,
    appendOptions: AppendOptions = {allowFalse: false}
): HttpParams => {
    if (value === null || value === undefined || value === '' || (value === false && !appendOptions.allowFalse)) {
        return params;
    }

    if (value instanceof Date) {
        return params.set(name, value.toISOString());
    }

    return params.set(name, value);
};

export const appendSort = (params: HttpParams, sortParameter?: SortParameter | null): HttpParams => {
    if (!sortParameter?.sort) {
        return params;
    }

    params = append(params, 'sort', sortParameter.sort);
    params = append(params, 'direction', sortParameter.direction);

    return params;
};

export const appendPaging = (params: HttpParams, pagingParameter?: PagingParameters | null): HttpParams => {
    if (!pagingParameter) {
        return params;
    }

    params = append(params, 'pageIndex', pagingParameter.pageIndex);
    params = append(params, 'pageSize', pagingParameter.pageSize);

    return params;
};
