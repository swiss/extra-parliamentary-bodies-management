import {HttpParams} from '@angular/common/http';
import {PagingParameters} from '../../api/PagingParameters';
import {SortParameter} from '../../api/SortParameter';
import {append, appendMany, appendPaging, appendSort} from './http-params-util';

describe('HttpParamsUtil', () => {
    it.each([[undefined], [null]])("should not add any sort params when sort parameter is '%s'", value => {
        const params = appendSort(new HttpParams(), value);

        expect(params.keys().length).toEqual(0);
    });

    it('should add sort params for valid sort-parameter', () => {
        const sortParams: SortParameter = {sort: 'test_sort', direction: 'desc'};
        const params = appendSort(new HttpParams(), sortParams);

        expect(params.get('sort')).toEqual(sortParams.sort);
        expect(params.get('direction')).toEqual(sortParams.direction);
    });

    it.each([[undefined], [null]])("should not add any paging params when sort parameter is '%s'", value => {
        const params = appendPaging(new HttpParams(), value);

        expect(params.keys().length).toEqual(0);
    });

    it('should add paging params for valid paging-parameter', () => {
        const pagingParams: PagingParameters = {pageIndex: 5, pageSize: 125};
        const params = appendPaging(new HttpParams(), pagingParams);

        expect(params.get('pageIndex')).toEqual(`${pagingParams.pageIndex}`);
        expect(params.get('pageSize')).toEqual(`${pagingParams.pageSize}`);
    });

    describe('appendMany', () => {
        it.each([[undefined], [null], [[]]])("should not add when values is '%s'", values => {
            const params = appendMany(new HttpParams(), 'test_param_name', values);

            expect(params.keys().length).toEqual(0);
        });

        it.each([
            [[1, 2, '3'], 3],
            [['3', 'test_value'], 2],
            [[100], 1],
            [[true, false], 2],
        ])('should add values to params', (values, expected) => {
            const paramName = 'test_param_name';
            const params = appendMany(new HttpParams(), paramName, values);

            expect(params.getAll(paramName)!.length).toEqual(expected);
        });
    });

    describe('append', () => {
        it.each([[undefined], [null], [''], [false]])("should not add when values is '%s'", value => {
            const params = append(new HttpParams(), 'test_param_name', value);

            expect(params.keys().length).toEqual(0);
        });

        it.each([['test_value'], [0], [42], [true]])("should add when values is '%s'", value => {
            const paramName = 'test_param_name';
            const params = append(new HttpParams(), paramName, value);

            expect(params.keys().length).toEqual(1);
            expect(params.get(paramName)).toEqual(value.toString());
        });

        it('should add date as ISO string', () => {
            const date = new Date();
            const paramName = 'test_data_param_name';
            const params = append(new HttpParams(), paramName, date);

            expect(params.get(paramName)).toEqual(date.toISOString());
        });

        it("should 'false' if allowFalse is set", () => {
            const value = false;
            const paramName = 'test_false_param_name';
            const params = append(new HttpParams(), paramName, value, {allowFalse: true});

            expect(params.get(paramName)).toEqual(`${value}`);
        });
    });
});
