import {HttpClient, HttpParams} from '@angular/common/http';
import {CommitteeCreate} from '@api/CommitteeCreate';
import {CommitteeFilterParameters} from '@api/CommitteeFilterParameters';
import {CommitteeJustificationUpdate} from '@api/CommitteeJustificationUpdate';
import {CommitteeMembershipValidationRequest} from '@api/CommitteeMembershipValidationRequest';
import {CommitteeUpdate} from '@api/CommitteeUpdate';
import {PagingParameters} from '@api/PagingParameters';
import {RequestsAndReportsFilterParameters} from '@api/RequestsAndReportsFilterParameters';
import {SortParameter} from '@api/SortParameter';
import {firstValueFrom, of} from 'rxjs';
import {CommitteesService} from './committees.service';

describe('CommitteesService', () => {
    let service: CommitteesService;

    const httpClientMock = {
        get: jest.fn(() => of()),
        put: jest.fn(() => of()),
        post: jest.fn(() => of()),
    } as unknown as jest.Mocked<HttpClient>;

    beforeEach(() => {
        service = new CommitteesService(httpClientMock);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get committee list', async () => {
        const paging: PagingParameters = {
            pageIndex: 0,
            pageSize: 50,
        };

        const sort: SortParameter = {
            sort: 'description',
            direction: 'asc',
        };
        const mockResponse = {
            total: 100,
            items: [
                {id: '1', description: 'Foo'},
                {id: '2', description: 'Bar'},
            ],
        };
        httpClientMock.get.mockReturnValue(of(mockResponse));

        const response = await firstValueFrom(service.getCommitteeList(paging, {}, sort));

        expect(response).toBeTruthy();
        expect(response.total).toEqual(100);
        expect(response.items).toHaveLength(2);

        const expectedParams = new HttpParams()
            .set('pageIndex', paging.pageIndex)
            .set('pageSize', paging.pageSize)
            .set('sort', sort.sort)
            .set('direction', sort.direction);

        expect(httpClientMock.get).toHaveBeenCalledWith(
            '/api/committees/list',
            expect.objectContaining({
                params: expectedParams,
            })
        );
    });

    it('should load committee list for exports', async () => {
        const mockResponse = [
            {id: '1', description: 'Foo'},
            {id: '2', description: 'Bar'},
        ];
        httpClientMock.get.mockReturnValue(of(mockResponse));

        const filterParams: RequestsAndReportsFilterParameters = {
            departments: ['department1'],
            offices: ['office1'],
            committeeTypes: ['committeeType1'],
        };

        const response = await firstValueFrom(service.getCommitteeListForExport(filterParams));

        expect(response).toBeTruthy();
        expect(response).toHaveLength(2);

        expect(httpClientMock.get).toHaveBeenCalledWith('/api/committees/listExport', {
            params: {
                cloneFrom: {cloneFrom: null, encoder: {}, map: null, updates: null},
                encoder: {},
                map: null,
                updates: [
                    {op: 'a', param: 'departmentIds', value: 'department1'},
                    {op: 'a', param: 'officeIds', value: 'office1'},
                    {op: 'a', param: 'committeeTypeIds', value: 'committeeType1'},
                ],
            },
        });
    });

    it('should add filter params for valid filter-parameter', () => {
        const filterParams: CommitteeFilterParameters = {
            freeText: 'test',
            levels: ['level1'],
            departments: ['department1'],
            offices: ['office1'],
            committeeTypes: ['committeeType1'],
            terms: ['term1'],
            isActive: [true],
            isMarketOrientated: [true],
            hasSupervisionDuty: [true],
        };

        const params = service.appendFilter(new HttpParams(), filterParams);

        expect(params.get('freeText')).toEqual(filterParams.freeText);
        expect(params.get('levelIds')).toEqual(filterParams.levels![0]);
        expect(params.get('departmentIds')).toEqual(filterParams.departments![0]);
        expect(params.get('officeIds')).toEqual(filterParams.offices![0]);
        expect(params.get('committeeTypeIds')).toEqual(filterParams.committeeTypes![0]);
        expect(params.get('termIds')).toEqual(filterParams.terms![0]);
        expect(params.get('isActive')).toEqual(filterParams.isActive![0].toString());
        expect(params.get('isMarketOrientated')).toEqual(filterParams.isMarketOrientated![0].toString());
        expect(params.get('hasSupervisionDuty')).toEqual(filterParams.hasSupervisionDuty![0].toString());
    });

    it.each([[undefined], [null]])("should not add any paging params when filter parameter is '%s'", value => {
        const params = service.appendFilter(new HttpParams(), value);

        expect(params.keys().length).toEqual(0);
    });

    it('should get committee details', async () => {
        const mockResponse = {
            id: '1',
            description: 'Description 1',
        };
        httpClientMock.get.mockReturnValue(of(mockResponse));

        const response = await firstValueFrom(service.getCommitteeDetails('1'));

        expect(response).toBeTruthy();
        expect(response.id).toEqual('1');
        expect(response.description).toEqual('Description 1');

        expect(httpClientMock.get).toHaveBeenCalledWith('/api/committees/1');
    });

    it('should update committee', () => {
        const committeeUpdate = {id: '1'} as CommitteeUpdate;

        service.updateCommittee(committeeUpdate);

        expect(httpClientMock.put).toHaveBeenCalledWith('/api/committees/1', committeeUpdate);
    });

    it('should update committee justification', () => {
        const committeeJustificationUpdate = {id: '1'} as CommitteeJustificationUpdate;

        service.updateCommitteeJustification(committeeJustificationUpdate);

        expect(httpClientMock.put).toHaveBeenCalledWith('/api/committees/1/justifications', committeeJustificationUpdate);
    });

    it('should validate membership', () => {
        const validationRequest: CommitteeMembershipValidationRequest = {
            committeeId: '1',
            personId: '2',
            beginDate: new Date(2023, 0, 1),
            endDate: new Date(2024, 11, 31),
            inCorrelationWithFederalDuty: false,
            isUpdateMode: true,
        };

        service.validateMembership('1', validationRequest);

        const expectedParams = new HttpParams()
            .set('committeeId', '1')
            .set('personId', '2')
            .set('beginDate', '2023-01-01')
            .set('endDate', '2024-12-31')
            .set('isUpdateMode', true);

        expect(httpClientMock.get).toHaveBeenCalledWith(
            '/api/committees/1/check-memberships',
            expect.objectContaining({
                params: expectedParams,
            })
        );
    });

    it('should create committee', () => {
        const committeeCreate = {} as CommitteeCreate;

        service.createCommittee(committeeCreate);

        expect(httpClientMock.post).toHaveBeenCalledWith('/api/committees', committeeCreate);
    });

    it('should get committee members', async () => {
        const mockResponse = {
            activeMemberships: [
                {
                    id: '1',
                    surname: 'Surname',
                },
            ],
        };
        httpClientMock.get.mockReturnValue(of(mockResponse));

        const response = await firstValueFrom(service.getCommitteeMembers('1'));

        expect(response).toBeTruthy();
        expect(response.activeMemberships[0].id).toEqual('1');
        expect(response.activeMemberships[0].surname).toEqual('Surname');

        expect(httpClientMock.get).toHaveBeenCalledWith('/api/committees/1/members');
    });

    it('should get committees by description', () => {
        service.getCommitteesByDescription('test').subscribe(response => {
            expect(response).toBeTruthy();
        });

        const expectedParams = new HttpParams().set('desc', 'test');

        expect(httpClientMock.get).toHaveBeenCalledWith(
            '/api/committees/get-by-description',
            expect.objectContaining({
                params: expectedParams,
            })
        );
    });
});
