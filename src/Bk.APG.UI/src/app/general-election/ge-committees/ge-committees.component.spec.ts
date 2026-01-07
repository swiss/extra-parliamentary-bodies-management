/* eslint-disable @typescript-eslint/no-explicit-any */
import {provideHttpClient} from '@angular/common/http';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {Sort} from '@angular/material/sort';
import {ActivatedRoute, Router} from '@angular/router';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {MockPipe, MockService} from 'ng-mocks';
import {Subject, of} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {GeneralElectionCommitteesComponent} from './ge-committees.component';
import {GeneralElectionCommitteesService} from './ge-committees.service';

interface MockRouter extends Omit<Router, 'url'> {
    url: string;
}

describe('GeneralElectionCommitteesComponent', () => {
    let component: GeneralElectionCommitteesComponent;
    let fixture: ComponentFixture<GeneralElectionCommitteesComponent>;
    let routeMock: Partial<ActivatedRoute>;
    let routerMock: jest.Mocked<MockRouter>;

    beforeEach(async () => {
        const routerEventsSubject = new Subject();
        const translateServiceMock = {
            currentLang: 'en',
            onLangChange: new Subject(),
        };
        routeMock = {};
        routerMock = {
            url: '/some-path',
            navigate: jest.fn().mockResolvedValue(true),
            events: routerEventsSubject.asObservable(),
        } as any;

        const generalElectionCommitteesServiceMock = {
            getGeneralElectionCommitteeList: jest.fn(),
        };

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), GeneralElectionCommitteesComponent],
            providers: [
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: GeneralElectionCommitteesService, useValue: MockService(generalElectionCommitteesServiceMock)},
                {provide: AuthService, useValue: MockService(AuthService, {isAdmin$: of(true), isDepartmentUser$: of(false), isAuthenticated$: of(true)})},
                {provide: ActivatedRoute, useValue: routeMock},
                {provide: Router, useValue: routerMock},
                provideHttpClient(),
            ],
            schemas: [NO_ERRORS_SCHEMA],
        })
            .overrideTemplate(GeneralElectionCommitteesComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(GeneralElectionCommitteesComponent);
        component = fixture.componentInstance;
        component.reload$.next();
        fixture.detectChanges();
    });

    describe('onPageChange', () => {
        it('should update pagingParams and trigger reload$', () => {
            const reloadSpy = jest.spyOn(component.reload$, 'next');
            const pageEvent = {pageSize: 100, pageIndex: 2, length: 200};

            component.onPageChange(pageEvent);

            expect(component.pagingParams.pageSize).toBe(pageEvent.pageSize);
            expect(component.pagingParams.pageIndex).toBe(pageEvent.pageIndex);
            expect(reloadSpy).toHaveBeenCalled();
        });
    });

    describe('onSort', () => {
        it('should update currentSort and trigger reload$', () => {
            const reloadSpy = jest.spyOn(component.reload$, 'next');
            const sortEvent = {active: 'committeeId', direction: 'desc'};

            component.onSort(sortEvent as Sort);

            expect(component.currentSort.active).toBe(sortEvent.active);
            expect(component.currentSort.direction).toBe(sortEvent.direction);
            expect(reloadSpy).toHaveBeenCalled();
        });
    });

    describe('onFilter', () => {
        it('should update filterValue and trigger reload$', () => {
            const reloadSpy = jest.spyOn(component.reload$, 'next');
            const filterParams = {
                freeText: 'test',
                hasActiveMembership: [true],
                cantons: ['canton1'],
                languages: ['lang1'],
            };

            component.onFilter(filterParams);

            expect(component.filterValue).toEqual(filterParams);
            expect(reloadSpy).toHaveBeenCalled();
        });
    });

    describe('openCommittee', () => {
        it('should navigate using router', async () => {
            const committeeId = 'committee123';

            component.openGeneralElectionCommittee(committeeId);

            expect(routerMock.navigate).toHaveBeenCalledWith([committeeId], {
                relativeTo: routeMock,
            });
        });
    });
});
