import {provideHttpClient} from '@angular/common/http';
import {provideHttpClientTesting} from '@angular/common/http/testing';
import {NO_ERRORS_SCHEMA, signal, WritableSignal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ControlEvent, FormGroup} from '@angular/forms';
import {ActivatedRoute, ActivatedRouteSnapshot, Router} from '@angular/router';
import {ContactPointUpdate} from '@api/ContactPointUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService} from '@oblique/oblique';
import {MockComponents, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, Subject, throwError} from 'rxjs';
import {AuthService} from '../../../../auth/auth.service';
import {CommitteesService} from '../../../../committees/committees.service';
import {CommitteeDetailsService} from '../../committee-details.service';
import {ContactPointFormComponent} from '../contact-point-form/contact-point-form.component';
import {ContactPointsService} from '../contact-points.service';
import {ContactPointEditComponent} from './contact-point-edit.component';

describe('ContactPointEditComponent', () => {
    let component: ContactPointEditComponent;
    let fixture: ComponentFixture<ContactPointEditComponent>;
    let contactPointsServiceMock: Partial<ContactPointsService>;
    let activatedRouteMock: Partial<ActivatedRoute>;
    let routerMock: Partial<Router>;
    let notificationServiceMock: Partial<ObNotificationService>;
    let committeeDetailsServiceMock: Partial<CommitteeDetailsService>;
    let committeesServiceMock: Partial<CommitteesService>;
    let reloadSubject: Subject<void>;
    let eventsSubject: Subject<ControlEvent>;
    let form: WritableSignal<FormGroup>;

    const isObserverSubject = new BehaviorSubject(false);
    const authServiceMock = {
        isObserver$: isObserverSubject.asObservable(),
    };

    beforeEach(async () => {
        reloadSubject = new Subject<void>();
        eventsSubject = new Subject<ControlEvent>();

        committeeDetailsServiceMock = {
            isDataTabDisabled: signal(false),
            isMembersTabDisabled: signal(false),
            isContactsTabDisabled: signal(false),
            isJustificationsTabDisabled: signal(false),
            isDecisionsTabDisabled: signal(false),
        };

        contactPointsServiceMock = {
            getContactPointForUpdate: jest.fn(),
            updateContactPoint: jest.fn(),
            reload$: reloadSubject,
        };

        notificationServiceMock = {
            warning: jest.fn(),
            success: jest.fn(),
            error: jest.fn(),
        };

        committeesServiceMock = {
            getCommitteeDetails: jest.fn(),
        };

        activatedRouteMock = {
            snapshot: {
                params: {id: '123'},
                url: ['route', 'copy'],
            } as unknown as ActivatedRouteSnapshot,
        };
        routerMock = {
            navigate: jest.fn().mockResolvedValue(true),
        };

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), ContactPointEditComponent, MockComponents(ContactPointFormComponent)],
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

        (contactPointsServiceMock.getContactPointForUpdate as jest.Mock).mockReturnValue(of({id: '123', companyName: 'Test AG'}));
        (committeesServiceMock.getCommitteeDetails as jest.Mock).mockReturnValue(of({id: '123', description: 'Gremium für alles'}));

        fixture = TestBed.createComponent(ContactPointEditComponent);
        component = fixture.componentInstance;

        // @ts-ignore
        form = signal({
            pristine: true,
            events: eventsSubject.asObservable(),
            valid: true,
            reset: jest.fn(),
            markAsTouched: jest.fn(),
        });
        component.form = form;

        component.contactPointToUpdate = signal({} as unknown as ContactPointUpdate);

        fixture.detectChanges();
        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('save', () => {
        it('should mark form as touched if invalid', () => {
            form.update(form => ({...form, valid: false}) as unknown as FormGroup);

            component.save();

            expect(component.form().markAsTouched).toHaveBeenCalled();
            expect(contactPointsServiceMock.updateContactPoint).not.toHaveBeenCalled();
        });

        it('should call updateContactPoint and handle success', async () => {
            (contactPointsServiceMock.updateContactPoint as jest.Mock).mockReturnValue(of({}));
            const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
            const contactPointBeforeSave = {
                id: '1',
                contactPointTypeUri: 'my.uri.com',
                beginDate: new Date(2025, 1, 1),
                endDate: new Date(2025, 12, 31),
                companyName: 'Test Amt1',
                section: 'Sektion A1',
                languageId: 'lang1',
                genderId: 'gender1',
                surname: 'surname1',
                givenName: 'givenname1',
                street: 'Strasse1',
                poBox: '1',
                phone: 'phone1',
                email: 'email1',
                personalPhone: 'persPhon1',
                personalMobile: 'persMobile1',
                personalEmail: 'persEmail@mail.com1',
                zip: '5601',
                city: 'Teststadt1',
                title: 'title1',
                releasePersonData: false,
                isCopy: false,
                rowVersion: 666,
            } as ContactPointUpdate;

            component.contactPointToUpdate.set(contactPointBeforeSave);
            component.unmodifiedContactPoint = {
                id: '1',
                contactPointTypeUri: 'my.uri2.com',
                beginDate: new Date(2025, 0, 1),
                endDate: new Date(2025, 0, 1),
                companyName: 'Test Amt',
                section: 'Sektion A',
                languageId: 'lang',
                genderId: 'gender',
                surname: 'surname',
                givenName: 'givenname',
                street: 'Strasse',
                poBox: '',
                phone: 'phone',
                email: 'email',
                personalPhone: 'persPhon',
                personalMobile: 'persMobile',
                personalEmail: 'persEmail@mail.com',
                zip: '5600',
                city: 'Teststadt',
                title: 'title',
                releasePersonData: false,
                isCopy: false,
                rowVersion: 666,
            } as ContactPointUpdate;

            component.save();

            expect(contactPointsServiceMock.updateContactPoint).toHaveBeenCalled();
            expect(reloadNextSpy).toHaveBeenCalled();
            expect(routerMock.navigate).toHaveBeenCalledWith(['committees'], {queryParams: {tab: 'contacts'}, replaceUrl: true});

            await fixture.whenStable();

            expect(notificationServiceMock.success).toHaveBeenCalledWith('contactPoint.details.data.success');
            expect(component.unmodifiedContactPoint).toEqual(contactPointBeforeSave);
        });

        it('should handle updateContactPoint error', () => {
            (contactPointsServiceMock.updateContactPoint as jest.Mock).mockReturnValue(throwError(() => new Error('Update failed')));

            component.save();

            expect(contactPointsServiceMock.updateContactPoint).toHaveBeenCalled();
            expect(notificationServiceMock.error).toHaveBeenCalledWith('contactPoint.details.data.error');
        });
    });

    describe('reset', () => {
        it('should reset form', () => {
            component.reset();

            expect(component.form().reset).toHaveBeenCalled();
        });
    });

    describe('back', () => {
        it('should navigate back from form', () => {
            component.back();

            expect(routerMock.navigate).toHaveBeenCalledWith(['committees'], {queryParams: {tab: 'contacts'}, replaceUrl: true});
        });
    });
});
