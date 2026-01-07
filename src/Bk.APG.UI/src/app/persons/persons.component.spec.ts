import {EventEmitter} from '@angular/core';
import {PageEvent} from '@angular/material/paginator';
import {Sort} from '@angular/material/sort';
import {PersonFilterParameters} from '@api/PersonFilterParameters';
import {PersonList} from '@api/PersonList';
import {LangChangeEvent} from '@ngx-translate/core';
import {of} from 'rxjs';
import {PersonsComponent} from './persons.component';

describe('PersonsComponent', () => {
    let component: PersonsComponent;

    const languageChangeEmitter = new EventEmitter<LangChangeEvent>();
    const translateServiceMock = {
        onLangChange: languageChangeEmitter,
        stream: jest.fn(),
    };

    const personsServiceMock = {
        getPersonList: jest.fn(),
    };

    const routeMock = {};

    const routerMock = {
        navigate: jest.fn().mockReturnValue({then: jest.fn()}),
    };

    beforeEach(async () => {
        component = new PersonsComponent(translateServiceMock as never, personsServiceMock as never, routeMock as never, routerMock as never);

        personsServiceMock.getPersonList.mockReturnValue(of({}));
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should load persons on language change', () => {
        component.ngOnInit();
        personsServiceMock.getPersonList.mockClear();

        translateServiceMock.onLangChange.emit({} as LangChangeEvent);

        expect(personsServiceMock.getPersonList).toHaveBeenCalledTimes(1);
    });

    it('should load persons on reload', () => {
        component.ngOnInit();
        personsServiceMock.getPersonList.mockClear();

        component.reload$.next();

        expect(personsServiceMock.getPersonList).toHaveBeenCalledTimes(1);
    });

    it('should set paging params on change', () => {
        const pagingEvent = {pageSize: 500, pageIndex: 24} as PageEvent;

        component.onPageChange(pagingEvent);

        expect(component.pagingParams.pageSize).toEqual(pagingEvent.pageSize);
        expect(component.pagingParams.pageIndex).toEqual(pagingEvent.pageIndex);
    });

    it('should reload on paging change', () => {
        const pagingEvent = {pageSize: 500, pageIndex: 24} as PageEvent;

        component.ngOnInit();
        personsServiceMock.getPersonList.mockClear();

        component.onPageChange(pagingEvent);

        expect(personsServiceMock.getPersonList).toHaveBeenCalledTimes(1);
    });

    it('should set isFiltered on non-empty filter', () => {
        component.ngOnInit();
        const filter = {
            freeText: 'test',
            hasActiveMembership: [true],
            cantons: ['canton1'],
            languages: ['lang1'],
        } as PersonFilterParameters;

        component.onFilter(filter);
        expect(component.searchSubject.value).toEqual(filter);
        expect(component.isFiltered()).toBe(true);
    });

    it('should set isFiltered on empty filter', () => {
        component.ngOnInit();
        const filter = {
            freeText: '',
            hasActiveMembership: [],
            cantons: [],
            languages: [],
        } as PersonFilterParameters;

        component.onFilter(filter);
        expect(component.searchSubject.value).toEqual(filter);
        expect(component.isFiltered()).toBe(false);
    });

    it('should set sort params on change', () => {
        const sort = {active: 'test_key', direction: 'desc'} as Sort;

        component.onSort(sort);

        expect(component.currentSort.active).toEqual(sort.active);
        expect(component.currentSort.direction).toEqual(sort.direction);
    });

    it('should reload on sort change', () => {
        const sort = {active: 'test_key', direction: 'desc'} as Sort;

        component.ngOnInit();
        personsServiceMock.getPersonList.mockClear();

        component.onSort(sort);

        expect(personsServiceMock.getPersonList).toHaveBeenCalledTimes(1);
    });

    it('should set filter params on change', () => {
        const filter = {
            freeText: 'test',
            hasActiveMembership: [true],
            cantons: ['canton1'],
            languages: ['lang1'],
        } as PersonFilterParameters;

        component.onFilter(filter);

        expect(component.searchSubject.value).toEqual(filter);
    });

    it('should navigate', () => {
        const personList: PersonList = {
            id: 'a',
            surname: 'surname',
            givenName: 'givenname',
            hasActiveMembership: true,
            birthYear: 1955,
            language: 'DE',
        };

        component.openPerson(personList);

        expect(routerMock.navigate).toHaveBeenCalledTimes(1);
    });
});
