import {provideHttpClient} from '@angular/common/http';
import {provideHttpClientTesting} from '@angular/common/http/testing';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ActivatedRoute, Params, Router} from '@angular/router';
import {BehaviorSubject} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';
import {GeneralElectionCommitteeDetailsComponent} from './ge-committee-details.component';

describe('GeCommitteeDetailsComponent', () => {
    let component: GeneralElectionCommitteeDetailsComponent;
    let fixture: ComponentFixture<GeneralElectionCommitteeDetailsComponent>;
    const mockRouter = {navigate: jest.fn()} as unknown as Router;
    let mockActivatedRoute: Partial<ActivatedRoute>;
    let queryParamsSubject: BehaviorSubject<Params>;

    const isObserverSubject = new BehaviorSubject(false);
    const authServiceMock = {
        isObserver$: isObserverSubject.asObservable(),
    };

    beforeEach(async () => {
        queryParamsSubject = new BehaviorSubject<Params>({tab: 'overview'});

        mockActivatedRoute = {
            queryParams: queryParamsSubject.asObservable(),
        };

        await TestBed.configureTestingModule({
            imports: [GeneralElectionCommitteeDetailsComponent],
            providers: [
                {provide: Router, useValue: mockRouter},
                {provide: ActivatedRoute, useValue: mockActivatedRoute},
                {provide: AuthService, useValue: authServiceMock},
                provideHttpClient(),
                provideHttpClientTesting(),
            ],
            schemas: [NO_ERRORS_SCHEMA],
        })
            .overrideTemplate(GeneralElectionCommitteeDetailsComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(GeneralElectionCommitteeDetailsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => jest.clearAllMocks());

    it('should create the component', () => {
        expect(component).toBeTruthy();
    });

    it('should initialize selectedIndex to 0', () => {
        expect(component.selectedIndex()).toBe(0);
    });

    it('should update selectedIndex when queryParams change', () => {
        queryParamsSubject.next({tab: 'data'});

        expect(component.selectedIndex()).toBe(1);
    });

    it('should navigate with updated queryParams when selectedIndex changes', () => {
        component.selectedIndex.set(2);

        fixture.detectChanges();

        expect(mockRouter.navigate).toHaveBeenCalledWith([], {
            replaceUrl: true,
            queryParams: {tab: 'members'},
        });
    });

    it('should default to overview tab when tab parameter is not recognized', () => {
        queryParamsSubject.next({tab: 'unknown'});

        expect(component.selectedIndex()).toBe(0);
    });
});
