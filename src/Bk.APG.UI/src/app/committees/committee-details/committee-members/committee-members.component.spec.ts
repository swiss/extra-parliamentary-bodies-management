/* eslint-disable @typescript-eslint/no-explicit-any */
import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatIconTestingModule} from '@angular/material/icon/testing';
import {PageEvent} from '@angular/material/paginator';
import {Sort} from '@angular/material/sort';
import {ActivatedRoute, Router} from '@angular/router';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {MembershipList} from '@api/MembershipList';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObGlobalEventsService} from '@oblique/oblique';
import {MembersQuotasComponent} from '@shared/members-quotas/members-quotas.component';
import {MockPipe, MockService} from 'ng-mocks';
import {of} from 'rxjs';
import {GeneralElectionService} from '../../../general-election/general-election.service';
import {CommitteesService} from '../../committees.service';
import {CommitteeDetailsService} from '../committee-details.service';
import {CommitteeMembersComponent} from './committee-members.component';

describe('CommitteeMembersComponent', () => {
    let component: CommitteeMembersComponent;
    let fixture: ComponentFixture<CommitteeMembersComponent>;
    let router: Router;
    let activatedRoute: ActivatedRoute;

    const routerMock = {navigate: jest.fn()};
    const activatedRouteMock = {snapshot: {params: {id: '123'}}};
    const translateServiceMock = {
        getCurrentLang: jest.fn(() => 'en'),
        onLangChange: of({lang: 'en'}),
        instant: jest.fn().mockReturnValue('Bis 10%'),
    };
    const committeesServiceMock = {
        getCommitteeMembers: jest.fn().mockReturnValue(
            of({
                committeeQuotas: {},
                activeMemberships: [
                    {id: '1', surname: 'Clark', givenName: 'Jim', employmentLevel: '10'},
                    {id: '2', surname: 'Schwarzenegger', givenName: 'Arnold', employmentLevel: '0'},
                ],
                inactiveMemberships: [],
            })
        ),
    };
    const committeeDetailsServiceMock = {
        committeeDetails: signal<CommitteeDetails>({canEdit: true} as CommitteeDetails),
    };

    const mockGeneralElectionService = {
        isGeneralElectionVisible: jest.fn(),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), CommitteeMembersComponent, MembersQuotasComponent, MatIconTestingModule],
            providers: [
                {provide: Router, useValue: routerMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: CommitteesService, useValue: committeesServiceMock},
                {provide: CommitteeDetailsService, useValue: committeeDetailsServiceMock},
                {provide: GeneralElectionService, useValue: mockGeneralElectionService},
                {provide: ObGlobalEventsService, useValue: MockService<ObGlobalEventsService>},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(CommitteeMembersComponent);
        component = fixture.componentInstance;
        router = TestBed.inject(Router);
        activatedRoute = TestBed.inject(ActivatedRoute);
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should set canEdit', () => {
        expect(component.canEdit()).toBe(true);
    });

    it('should set members employmentLevel correctly ', () => {
        expect(committeesServiceMock.getCommitteeMembers).toHaveBeenCalledTimes(1);
        expect(component.activeMembers().length).toBe(2);
        expect(component.activeMembers()[0].employmentLevel).toBe('10');
        expect(component.activeMembers()[1].employmentLevel).toBe('0');
        expect(component.inactiveMembers().length).toBe(0);
    });

    describe('editMember', () => {
        it('should navigate to the member edit page', () => {
            const member = {id: 'm1'} as any;
            component.editMember(member);
            expect(router.navigate).toHaveBeenCalledWith(['members', 'm1'], {relativeTo: activatedRoute});
        });
    });

    describe('createMember', () => {
        it('should navigate to the member creation page', () => {
            component.createMember();
            expect(router.navigate).toHaveBeenCalledWith(['members', 'create'], {relativeTo: activatedRoute});
        });
    });

    describe('sortData', () => {
        it('should update the provided sort signal with new sort value', () => {
            const newSort: Sort = {active: 'givenName', direction: 'asc'};
            const sortSignal = component.activeMembersSort;
            component.sortData(newSort, sortSignal);
            expect(sortSignal()).toEqual(newSort);
        });
    });

    describe('onPageChange', () => {
        it('should update the provided paging signal with new page event', () => {
            const newPageEvent: PageEvent = {pageIndex: 2, pageSize: 10, length: 100};
            const pagingSignal = component.activeMembersPaging;
            component.onPageChange(newPageEvent, pagingSignal);
            expect(pagingSignal()).toEqual({pageIndex: newPageEvent.pageIndex, pageSize: newPageEvent.pageSize});
        });
    });

    describe('prepareData', () => {
        it('should sort and slice the members array correctly', () => {
            const dummyMembers = [
                {id: '1', beginDate: '2020-01-01'} as any,
                {id: '2', beginDate: '2019-01-01'} as any,
                {id: '3', beginDate: '2021-01-01'} as any,
            ];
            const paging: {pageIndex: number; pageSize: number} = {pageIndex: 0, pageSize: 2};
            const sort: Sort = {active: 'beginDate', direction: 'asc'};

            const data = (component as any).prepareData(dummyMembers, paging, sort);

            expect(data.length).toBe(2);
            expect(data[0].id).toBe('2'); // 2019-01-01
            expect(data[1].id).toBe('1'); // 2020-01-01
        });

        it('should sort data case-insensitively', () => {
            const membersWithCase = [{id: '1', surname: 'b'} as any, {id: '2', surname: 'C'} as any, {id: '3', surname: 'A'} as any];
            const paging: {pageIndex: number; pageSize: number} = {pageIndex: 0, pageSize: 3};
            const sort: Sort = {active: 'surname', direction: 'asc'};

            const data = (component as any).prepareData(membersWithCase, paging, sort);

            expect(data[0].id).toBe('3');
            expect(data[1].id).toBe('1');
            expect(data[2].id).toBe('2');
        });
    });

    describe('compare', () => {
        const memberA = {beginDate: '2020-01-01'} as any;
        const memberB = {beginDate: '2021-01-01'} as any;

        it('should return -1 for ascending order when first member is less than second', () => {
            const result = (component as any).compare(memberA, memberB, {active: 'beginDate', direction: 'asc'});
            expect(result).toBe(-1);
        });

        it('should return 1 for descending order when first member is less than second', () => {
            const result = (component as any).compare(memberA, memberB, {active: 'beginDate', direction: 'desc'});
            expect(result).toBe(1);
        });

        it('should reverse the result for descending order', () => {
            const ascResult = (component as any).compare(memberA, memberB, {active: 'beginDate', direction: 'asc'});
            const descResult = (component as any).compare(memberA, memberB, {active: 'beginDate', direction: 'desc'});
            expect(descResult).toBe(-ascResult);
        });
    });

    describe('computed properties', () => {
        it('should compute active members correctly', () => {
            const membershipList = {
                committeeQuotas: {},
                activeMemberships: [{id: '1', isActive: true, needsAttention: false, beginDate: new Date('2023-01-01')} as any],
                inactiveMemberships: [
                    {id: '2', isActive: false, needsAttention: true, beginDate: new Date('2022-01-01')} as any,
                    {id: '3', isActive: false, needsAttention: false, beginDate: new Date('2021-01-01')} as any,
                ],
            };
            component.membershipList.set(membershipList as MembershipList);
            expect(component.activeMembers().length).toBe(1);
            expect(component.activeMembers()[0].id).toBe('1');
        });

        it('should compute inactive members correctly', () => {
            const membershipList = {
                activeMemberships: [{id: '1', isActive: true, needsAttention: false, beginDate: new Date('2023-01-01')} as any],
                inactiveMemberships: [
                    {id: '2', isActive: false, needsAttention: true, beginDate: new Date('2022-01-01')} as any,
                    {id: '3', isActive: false, needsAttention: false, beginDate: new Date('2021-01-01')} as any,
                ],
            };
            component.membershipList.set(membershipList as MembershipList);
            expect(component.inactiveMembers().length).toBe(2);
            expect(component.inactiveMembers()[0].id).toBe('2');
        });
    });
});
