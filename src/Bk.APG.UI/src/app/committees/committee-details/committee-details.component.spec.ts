import {NO_ERRORS_SCHEMA} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ActivatedRoute, Params, Router} from '@angular/router';
import {ObNotificationService} from '@oblique/oblique';
import {BehaviorSubject} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {CommitteeDetailsComponent} from './committee-details.component';

describe('CommitteeDetailsComponent', () => {
    let component: CommitteeDetailsComponent;
    let fixture: ComponentFixture<CommitteeDetailsComponent>;
    const mockRouter = {navigate: jest.fn()} as unknown as Router;
    let mockActivatedRoute: Partial<ActivatedRoute>;
    let queryParamsSubject: BehaviorSubject<Params>;

    const notificationServiceMock = {
        warning: jest.fn(),
    };

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
            imports: [CommitteeDetailsComponent],
            providers: [
                {provide: Router, useValue: mockRouter},
                {provide: ActivatedRoute, useValue: mockActivatedRoute},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: AuthService, useValue: authServiceMock},
            ],
            schemas: [NO_ERRORS_SCHEMA],
        })
            .overrideTemplate(CommitteeDetailsComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(CommitteeDetailsComponent);
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
        component.selectedIndex.set(1);

        fixture.detectChanges();

        expect(mockRouter.navigate).toHaveBeenCalledWith([], {
            replaceUrl: true,
            queryParams: {tab: 'data'},
        });
    });

    it('should handle absence of tab in queryParams gracefully', () => {
        queryParamsSubject.next({});

        expect(component.selectedIndex()).toBe(0);
    });

    it('should not throw an error if queryParams.tab is undefined', () => {
        queryParamsSubject.next({tab: undefined});

        expect(component.selectedIndex()).toBe(0);
    });

    it('should call router.navigate on initialization with initial selectedIndex', () => {
        expect(mockRouter.navigate).toHaveBeenCalledWith([], {
            replaceUrl: true,
            queryParams: {tab: 'overview'},
        });
    });

    describe('onTabGroupClick', () => {
        it('should display a warning if the clicked element has "mat-mdc-tab-disabled" class', () => {
            const event = new MouseEvent('click');
            const target = document.createElement('div');
            target.classList.add('mat-mdc-tab-disabled');
            Object.defineProperty(event, 'target', {value: target});

            component.onTabGroupClick(event);

            expect(notificationServiceMock.warning).toHaveBeenCalledWith({message: 'common.saveReminder', timeout: 7000});
        });

        it('should not display a warning if the clicked element does not have "mat-mdc-tab-disabled" class', () => {
            const event = new MouseEvent('click');
            const target = document.createElement('div');
            Object.defineProperty(event, 'target', {value: target});

            component.onTabGroupClick(event);

            expect(notificationServiceMock.warning).not.toHaveBeenCalled();
        });
    });
});
