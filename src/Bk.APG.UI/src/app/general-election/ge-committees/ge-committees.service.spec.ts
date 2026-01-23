/* eslint-disable @typescript-eslint/no-explicit-any */
import {HttpParams, provideHttpClient} from '@angular/common/http';
import {HttpTestingController, provideHttpClientTesting} from '@angular/common/http/testing';
import {TestBed} from '@angular/core/testing';
import {GeneralElectionCommitteeFilterParameters} from '@api/GeneralElectionCommitteeFilterParameters';
import {GeneralElectionCommitteeList} from '@api/GeneralElectionCommitteeList';
import {PagedResult} from '@api/PagedResult';
import {PagingParameters} from '@api/PagingParameters';
import {RecipientsFilterParameters} from '@api/RecipientsFilterParameters';
import {SortParameter} from '@api/SortParameter';
import {Subject} from 'rxjs';
import {GeneralElectionCommitteesService} from './ge-committees.service';

describe('GeneralElectionCommitteesService', () => {
    let service: GeneralElectionCommitteesService;
    let httpMock: HttpTestingController;
    let routerEventsSubject: Subject<any>;

    beforeEach(() => {
        routerEventsSubject = new Subject();

        TestBed.configureTestingModule({
            providers: [GeneralElectionCommitteesService, provideHttpClient(), provideHttpClientTesting()],
        });

        httpMock = TestBed.inject(HttpTestingController);
        service = TestBed.inject(GeneralElectionCommitteesService);
    });

    afterEach(() => {
        httpMock?.verify();
        routerEventsSubject?.complete();
    });

    it('should get general election committee list', () => {
        const mockPagedResult: PagedResult<GeneralElectionCommitteeList> = {
            index: 0,
            total: 2,
            items: [
                {
                    id: '1',
                    committeeId: '2',
                    committeeType: 'type',
                    description: 'desc',
                    department: 'department',
                    office: 'office',
                    isMarketOrientated: 'true',
                    hasSupervisionDuty: false,
                    status: 'status',
                    vacanciesGeneralElection: 1,
                    statusProposal: 'statusP',
                    modified: new Date(2025, 1, 1),
                    modifiedBy: 'test',
                } as GeneralElectionCommitteeList,
                {
                    id: '2',
                    committeeType: 'type',
                    description: 'desc',
                    department: 'department',
                    office: 'office',
                    isMarketOrientated: 'true',
                    hasSupervisionDuty: false,
                    status: 'status',
                    vacanciesGeneralElection: 1,
                    statusProposal: 'statusP',
                    modified: new Date(2025, 1, 1),
                    modifiedBy: 'test',
                } as GeneralElectionCommitteeList,
            ],
        };

        const paging: PagingParameters = {
            pageIndex: 0,
            pageSize: 50,
        };

        const sort: SortParameter = {
            sort: 'description',
            direction: 'asc',
        };

        service.getGeneralElectionCommitteeList(paging, {} as GeneralElectionCommitteeFilterParameters, sort).subscribe(result => {
            expect(result).toEqual(mockPagedResult);
        });

        const req = httpMock.expectOne(
            request =>
                request.url === '/api/general-election/committees/list' &&
                request.params.get('pageIndex') === paging.pageIndex.toString() &&
                request.params.get('pageSize') === paging.pageSize.toString() &&
                request.params.get('sort') === sort.sort &&
                request.params.get('direction') === sort.direction
        );

        expect(req.request.method).toBe('GET');
        req.flush(mockPagedResult);
    });

    it('should add filter params for valid filter-parameter', () => {
        const filterParams: GeneralElectionCommitteeFilterParameters = {
            freeText: 'test',
            departments: ['department1'],
            offices: ['office1'],
            committeeTypes: ['committeeType1'],
            isMarketOrientated: [true],
            hasSupervisionDuty: [true],
            status: 'status',
            vacancies: '1',
            statusProposal: 'statusP',
        };

        const params = service.appendFilter(new HttpParams(), filterParams);

        expect(params.get('freeText')).toEqual(filterParams.freeText);
        expect(params.get('departmentIds')).toEqual(filterParams.departments![0]);
        expect(params.get('officeIds')).toEqual(filterParams.offices![0]);
        expect(params.get('committeeTypeIds')).toEqual(filterParams.committeeTypes![0]);
        expect(params.get('isMarketOrientated')).toEqual(filterParams.isMarketOrientated![0].toString());
        expect(params.get('hasSupervisionDuty')).toEqual(filterParams.hasSupervisionDuty![0].toString());
        expect(params.get('status')).toEqual(filterParams.status);
        expect(params.get('vacancies')).toEqual(filterParams.vacancies);
        expect(params.get('statusProposal')).toEqual(filterParams.statusProposal);
    });

    it.each([[undefined], [null]])("should not add any paging params when filter parameter is '%s'", value => {
        const params = service.appendFilter(new HttpParams(), value);

        expect(params.keys().length).toEqual(0);
    });

    it('should get general election committee recipient export list', () => {
        const committeeList: GeneralElectionCommitteeList[] = [
            {
                id: '1',
                committeeId: '2',
                committeeType: 'type',
                description: 'desc',
                department: 'department',
                office: 'office',
                isMarketOrientated: 'true',
                hasSupervisionDuty: false,
                status: 'status',
                vacanciesGeneralElection: 1,
                statusProposal: 'statusP',
                modified: new Date(2025, 1, 1),
                modifiedBy: 'test',
            } as GeneralElectionCommitteeList,
            {
                id: '2',
                committeeType: 'type',
                description: 'desc',
                department: 'department',
                office: 'office',
                isMarketOrientated: 'true',
                hasSupervisionDuty: false,
                status: 'status',
                vacanciesGeneralElection: 1,
                statusProposal: 'statusP',
                modified: new Date(2025, 1, 1),
                modifiedBy: 'test',
            } as GeneralElectionCommitteeList,
        ];

        service.getGeneralElectionCommitteeListForRecipientExport({} as RecipientsFilterParameters).subscribe(result => {
            expect(result).toEqual(committeeList);
        });

        const req = httpMock.expectOne(request => request.url === '/api/general-election/committees/recipient');

        expect(req.request.method).toBe('GET');
        req.flush(committeeList);
    });
});
