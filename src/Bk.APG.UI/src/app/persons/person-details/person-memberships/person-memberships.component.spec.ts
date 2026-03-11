import {EventEmitter, signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatIconTestingModule} from '@angular/material/icon/testing';
import {Sort} from '@angular/material/sort';
import {ActivatedRoute, ActivatedRouteSnapshot, convertToParamMap, ParamMap, Router} from '@angular/router';
import {PersonMembership} from '@api/PersonMembership';
import {LangChangeEvent, TranslatePipe, TranslateService} from '@ngx-translate/core';
import {MockPipe} from 'ng-mocks';
import {BehaviorSubject, of} from 'rxjs';
import {PersonsService} from '../../persons.service';
import {PersonDetailsService} from '../person-details.service';
import {PersonMembershipsComponent} from './person-memberships.component';

describe('PersonMembershipsComponent', () => {
    let component: PersonMembershipsComponent;
    let fixture: ComponentFixture<PersonMembershipsComponent>;
    let routerMock: Partial<Router>;
    let routeMock: Partial<ActivatedRoute>;

    beforeEach(async () => {
        routerMock = {
            navigate: jest.fn().mockResolvedValue(true),
        };
        const personsServiceMock = {
            getPersonMemberships: jest.fn().mockReturnValue(of([])),
        };
        const personDetailsServiceMock = {
            personDetails: signal({id: '123'}),
            personName: signal(''),
        };
        const translateServiceMock = {
            onLangChange: new EventEmitter<LangChangeEvent>(),
            getCurrentLang: jest.fn(() => 'en'),
        };

        const paramMapSubject = new BehaviorSubject<ParamMap>(convertToParamMap({id: '123'}));
        routeMock = {
            paramMap: paramMapSubject.asObservable(),
            snapshot: {
                paramMap: convertToParamMap({id: '123'}),
            } as unknown as ActivatedRouteSnapshot,
        };

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), PersonMembershipsComponent, MatIconTestingModule],
            providers: [
                {provide: Router, useValue: routerMock},
                {provide: PersonsService, useValue: personsServiceMock},
                {provide: PersonDetailsService, useValue: personDetailsServiceMock},
                {provide: ActivatedRoute, useValue: routeMock},
                {provide: TranslateService, useValue: translateServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(PersonMembershipsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('sortData', () => {
        it('should set the current sort', () => {
            const sort = {active: 'committee', direction: 'asc'} as Sort;
            const currentSort = component.currentActiveMembershipsSort;
            component.sortData(sort, currentSort);
            expect(currentSort()).toEqual(sort);
        });
    });

    describe('editMembership', () => {
        it('should navigate to the edit membership route', () => {
            const membership: PersonMembership = {id: '1', isActive: true} as PersonMembership;
            component.editMembership(membership);
            expect(routerMock.navigate).toHaveBeenCalledWith(['memberships', membership.id], {relativeTo: routeMock});
        });
    });

    describe('createMembership', () => {
        it('should navigate to the create membership route', () => {
            component.createMembership();
            expect(routerMock.navigate).toHaveBeenCalledWith(['memberships', 'create'], {relativeTo: routeMock});
        });
    });

    describe('computed properties', () => {
        it('should compute active memberships', () => {
            const memberships: PersonMembership[] = [
                {id: '1', isActive: true, needsAttention: false, beginDate: new Date('2023-01-01')} as PersonMembership,
                {id: '2', isActive: true, needsAttention: true, beginDate: new Date('2022-01-01')} as PersonMembership,
                {id: '3', isActive: false, needsAttention: false, beginDate: new Date('2021-01-01')} as PersonMembership,
            ];
            component.memberships.set(memberships);
            expect(component.activeMemberships().length).toBe(2);
            expect(component.activeMemberships()[0].id).toBe('1');
        });

        it('should compute inactive memberships', () => {
            const memberships: PersonMembership[] = [
                {id: '1', isActive: true, needsAttention: false, beginDate: new Date('2023-01-01')} as PersonMembership,
                {id: '2', isActive: false, needsAttention: true, beginDate: new Date('2022-01-01')} as PersonMembership,
                {id: '3', isActive: false, needsAttention: false, beginDate: new Date('2021-01-01')} as PersonMembership,
            ];
            component.memberships.set(memberships);
            expect(component.inactiveMemberships().length).toBe(2);
            expect(component.inactiveMemberships()[0].id).toBe('2');
        });
    });
});
