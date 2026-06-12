import {HttpClient, HttpParams} from '@angular/common/http';
import {PagingParameters} from '@api/PagingParameters';
import {PersonFilterParameters} from '@api/PersonFilterParameters';
import {SortParameter} from '@api/SortParameter';
import {firstValueFrom, of} from 'rxjs';
import {PersonsService} from './persons.service';

describe('PersonsService', () => {
    let service: PersonsService;

    const httpClientMock = {
        get: jest.fn(() => of(undefined)),
        post: jest.fn(() => of(undefined)),
        put: jest.fn(() => of(undefined)),
        delete: jest.fn(() => of(undefined)),
    } as unknown as jest.Mocked<HttpClient>;

    beforeEach(() => {
        service = new PersonsService(httpClientMock);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get persons list', async () => {
        const paging: PagingParameters = {
            pageIndex: 0,
            pageSize: 50,
        };

        const sort: SortParameter = {
            sort: 'surname',
            direction: 'asc',
        };
        const mockResponse = {
            total: 100,
            items: [
                {id: 1, name: 'John Doe', surname: 'Doe'},
                {id: 2, name: 'Jane Smith', surname: 'Smith'},
            ],
        };
        httpClientMock.get.mockReturnValue(of(mockResponse));

        const response = await firstValueFrom(service.getPersonList(paging, {}, sort));

        expect(response).toBeTruthy();
        expect(response.total).toEqual(100);
        expect(response.items).toHaveLength(2);

        const expectedParams = new HttpParams()
            .set('pageIndex', paging.pageIndex)
            .set('pageSize', paging.pageSize)
            .set('sort', sort.sort)
            .set('direction', sort.direction);

        expect(httpClientMock.get).toHaveBeenCalledWith(
            '/api/persons/list',
            expect.objectContaining({
                params: expectedParams,
            })
        );
    });

    it('should get persons detail', () => {
        service.getPersonDetails('myId').subscribe(response => {
            expect(response).toBeTruthy();
        });

        expect(httpClientMock.get).toHaveBeenCalledWith(`/api/persons/myId`);
    });

    it('should get similar persons', () => {
        service.getSimilarPersons('surname', 'givenName', 1999, 5).subscribe(response => {
            expect(response).toBeTruthy();
        });

        const expectedParams = new HttpParams()
            .set('surname', 'surname')
            .append('givenName', 'givenName')
            .append('birthYear', 1999)
            .append('birthYearRange', 5);

        expect(httpClientMock.get).toHaveBeenCalledWith(
            '/api/persons/getSimilarPersons',
            expect.objectContaining({
                params: expectedParams,
            })
        );
    });

    it('should add filter params for valid filter-parameter', () => {
        const filterParams = {
            freeText: 'test',
            hasActiveMembership: [true],
            cantons: ['canton1'],
            languages: ['lang1'],
        } as PersonFilterParameters;

        const params = service.appendFilter(new HttpParams(), filterParams);

        expect(params.get('freeText')).toEqual(filterParams.freeText);
        expect(params.get('hasActiveMembership')).toEqual('true');
        expect(params.get('cantonIds')).toEqual('canton1');
        expect(params.get('languageIds')).toEqual('lang1');
    });

    it.each([[undefined], [null]])("should not add any paging params when filter parameter is '%s'", value => {
        const params = service.appendFilter(new HttpParams(), value);

        expect(params.keys().length).toEqual(0);
    });

    it('should create person', () => {
        service.createPerson({
            surname: 'surname',
            givenName: 'givenname',
            birthYear: 2000,
            noInterest: false,
            noEmployment: false,
            languageId: 'lang',
            correspondenceLanguageId: 'corrLang',
            genderId: 'gender',
            legislaturePeriodIds: [],
            hasActiveMembership: false,
        });

        expect(httpClientMock.post).toHaveBeenCalledWith('/api/persons', {
            languageId: 'lang',
            correspondenceLanguageId: 'corrLang',
            genderId: 'gender',
            surname: 'surname',
            givenName: 'givenname',
            birthYear: 2000,
            noInterest: false,
            noEmployment: false,
            legislaturePeriodIds: [],
            hasActiveMembership: false,
        });
    });

    it('should update person', () => {
        service.updatePerson({
            id: '1',
            languageId: 'lang',
            correspondenceLanguageId: 'corrLang',
            genderId: 'gender',
            surname: 'surname',
            givenName: 'givenname',
            birthYear: 2000,
            federalAssembly: true,
            federalDuty: true,
            noInterest: false,
            noEmployment: false,
            legislaturePeriodIds: [],
            isMissingJustificationFederalAssembly: false,
            rowVersion: 123,
            hasActiveMembership: false,
            maskAddress: false,
            canDelete: false,
        });

        expect(httpClientMock.put).toHaveBeenCalledWith('/api/persons/1', {
            id: '1',
            languageId: 'lang',
            correspondenceLanguageId: 'corrLang',
            genderId: 'gender',
            surname: 'surname',
            givenName: 'givenname',
            birthYear: 2000,
            federalAssembly: true,
            federalDuty: true,
            noInterest: false,
            noEmployment: false,
            legislaturePeriodIds: [],
            isMissingJustificationFederalAssembly: false,
            rowVersion: 123,
            hasActiveMembership: false,
            maskAddress: false,
            canDelete: false,
        });
    });

    it('should get person memberships', () => {
        service.getPersonMemberships('myId').subscribe(response => {
            expect(response).toBeTruthy();
        });

        expect(httpClientMock.get).toHaveBeenCalledWith(`/api/persons/myId/memberships`);
    });

    it('should get persons by name', () => {
        service.getPersonsByName('test').subscribe(response => {
            expect(response).toBeTruthy();
        });

        const expectedParams = new HttpParams().set('name', 'test');

        expect(httpClientMock.get).toHaveBeenCalledWith(
            '/api/persons/getByName',
            expect.objectContaining({
                params: expectedParams,
            })
        );
    });

    describe('deletePerson', () => {
        it('should delete person', async () => {
            await firstValueFrom(service.deletePerson('myId'));
            expect(httpClientMock.delete).toHaveBeenCalledWith('/api/persons/myId');
        });
    });
});
