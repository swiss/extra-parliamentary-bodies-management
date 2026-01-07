import {provideHttpClient} from '@angular/common/http';
import {provideHttpClientTesting} from '@angular/common/http/testing';
import {NO_ERRORS_SCHEMA, signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {FormControl, FormGroup} from '@angular/forms';
import {ActivatedRoute, ActivatedRouteSnapshot, Router} from '@angular/router';
import {ContactPointCreate} from '@api/ContactPointCreate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService} from '@oblique/oblique';
import {MockComponents, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, Subject, throwError} from 'rxjs';
import {AuthService} from '../../../../auth/auth.service';
import {CommitteesService} from '../../../../committees/committees.service';
import {CommitteeDetailsService} from '../../committee-details.service';
import {ContactPointFormComponent} from '../contact-point-form/contact-point-form.component';
import {ContactPointsService} from '../contact-points.service';
import {ContactPointCreateComponent} from './contact-point-create.component';

describe('ContactPointCreateComponent', () => {
    let component: ContactPointCreateComponent;
    let fixture: ComponentFixture<ContactPointCreateComponent>;
    let contactPointsServiceMock: Partial<ContactPointsService>;
    let activatedRouteMock: Partial<ActivatedRoute>;
    let routerMock: Partial<Router>;
    let notificationServiceMock: Partial<ObNotificationService>;
    let committeeDetailsServiceMock: Partial<CommitteeDetailsService>;
    let committeesServiceMock: Partial<CommitteesService>;
    let reloadSubject: Subject<void>;

    const isObserverSubject = new BehaviorSubject(false);
    const authServiceMock = {
        isObserver$: isObserverSubject.asObservable(),
    };

    beforeEach(async () => {
        reloadSubject = new Subject<void>();

        committeeDetailsServiceMock = {
            isDataTabDisabled: signal(false),
            isMembersTabDisabled: signal(false),
            isContactsTabDisabled: signal(false),
            isJustificationsTabDisabled: signal(false),
            isDecisionsTabDisabled: signal(false),
        };

        contactPointsServiceMock = {
            getContactPointForCreate: jest.fn(),
            createContactPoint: jest.fn(),
            reload$: reloadSubject,
        };

        committeesServiceMock = {
            getCommitteeDetails: jest.fn(),
        };

        notificationServiceMock = {
            success: jest.fn(),
            error: jest.fn(),
        };

        activatedRouteMock = {
            snapshot: {
                params: {id: '123'},
            } as unknown as ActivatedRouteSnapshot,
        };
        routerMock = {
            navigate: jest.fn().mockResolvedValue(true),
        };

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), ContactPointCreateComponent, MockComponents(ContactPointFormComponent)],
            providers: [
                {provide: ContactPointsService, useValue: contactPointsServiceMock},
                {provide: CommitteeDetailsService, useValue: committeeDetailsServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: CommitteesService, useValue: committeesServiceMock},
                {provide: AuthService, useValue: authServiceMock},
                provideHttpClient(),
                provideHttpClientTesting(),
            ],
            schemas: [NO_ERRORS_SCHEMA],
        }).compileComponents();

        (contactPointsServiceMock.getContactPointForCreate as jest.Mock).mockReturnValue(of({id: '123', companyName: 'Test AG'}));
        (committeesServiceMock.getCommitteeDetails as jest.Mock).mockReturnValue(of({id: '123', description: 'Gremium für alles'}));

        fixture = TestBed.createComponent(ContactPointCreateComponent);
        component = fixture.componentInstance;
        const mockFormComponent = {
            contactPointForm: new FormGroup({
                companyName: new FormControl(''),
            }),
        } as unknown as ContactPointFormComponent;

        component.formComponent = signal(mockFormComponent);

        component.contactPointToCreate = signal({} as unknown as ContactPointCreate);

        fixture.detectChanges();
        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should mark companyName as touched', () => {
        fixture.detectChanges();
        const companyName = component.form().controls.companyName;
        expect(companyName.touched).toBe(true);
    });

    it('should mark form as touched', () => {
        const companyName = component.form().controls.companyName;
        companyName.markAsDirty();
        companyName.setValue('test');
        fixture.detectChanges();
        expect(component.form().touched).toBe(true);
    });

    describe('save', () => {
        it('should mark form as touched if invalid', () => {
            // @ts-ignore
            component.form = signal({
                valid: false,
                markAllAsTouched: jest.fn(),
                reset: jest.fn(),
            });

            component.save();

            expect(component.form().markAllAsTouched).toHaveBeenCalled();
            expect(contactPointsServiceMock.createContactPoint).not.toHaveBeenCalled();
        });

        it('save should call createCommittee and handle success', async () => {
            // @ts-ignore
            component.form = signal({
                valid: true,
                markAllAsTouched: jest.fn(),
                reset: jest.fn(),
            });
            (contactPointsServiceMock.createContactPoint as jest.Mock).mockReturnValue(of({id: '1'}));

            const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
            component.save();

            expect(contactPointsServiceMock.createContactPoint).toHaveBeenCalled();
            expect(reloadNextSpy).toHaveBeenCalled();
            expect(routerMock.navigate).toHaveBeenCalledWith(['committees', '123'], {queryParams: {tab: 'contacts'}, replaceUrl: true});
            await fixture.whenStable();
            expect(notificationServiceMock.success).toHaveBeenCalledWith('contactPoint.details.data.success');
        });

        it('should handle createContactPoint error', () => {
            (contactPointsServiceMock.createContactPoint as jest.Mock).mockReturnValue(throwError(() => new Error('Create failed')));

            component.save();

            expect(contactPointsServiceMock.createContactPoint).toHaveBeenCalled();
            expect(notificationServiceMock.error).toHaveBeenCalledWith('contactPoint.details.data.error');
        });
    });

    describe('close', () => {
        it('should navigate at close', () => {
            component.close();

            expect(routerMock.navigate).toHaveBeenCalledWith(['committees', '123'], {queryParams: {tab: 'contacts'}, replaceUrl: true});
        });
    });
});
