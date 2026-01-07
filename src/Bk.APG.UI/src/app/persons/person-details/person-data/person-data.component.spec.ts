/* eslint-disable dot-notation */
import {signal, WritableSignal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {AbstractControl, ControlEvent, FormGroup, PristineChangeEvent} from '@angular/forms';
import {MatDialog} from '@angular/material/dialog';
import {ActivatedRoute, ActivatedRouteSnapshot, convertToParamMap, ParamMap, Router} from '@angular/router';
import {PersonUpdate} from '@api/PersonUpdate';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObAlertModule, ObNotificationService} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {MockComponents, MockModule, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, Subject, throwError} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';
import {PersonsService} from '../../persons.service';
import {PersonDataFormComponent} from '../../shared/person-data-form/person-data-form.component';
import {PersonDetailsService} from '../person-details.service';
import {PersonDataComponent} from './person-data.component';

describe('PersonDataComponent', () => {
    let component: PersonDataComponent;
    let fixture: ComponentFixture<PersonDataComponent>;
    let personsServiceMock: Partial<PersonsService>;
    let activatedRouteMock: Partial<ActivatedRoute>;
    let routerMock: Partial<Router>;
    let notificationServiceMock: Partial<ObNotificationService>;
    let personDetailsServiceMock: Partial<PersonDetailsService>;
    let entityAuditLogServiceMock: Partial<EntityAuditLogService>;
    let reloadSubject: Subject<void>;
    let eventsSubject: Subject<ControlEvent>;
    let reloadEntityAuditLogSubject: BehaviorSubject<void>;
    let form: WritableSignal<FormGroup>;
    let paramMapSubject: BehaviorSubject<ParamMap>;
    let dialogAfterClosedSubject: Subject<boolean>;
    let dialogMock: Partial<MatDialog>;
    let isObserverSubject: BehaviorSubject<boolean>;
    let isAdminSubject: BehaviorSubject<boolean>;

    beforeEach(async () => {
        reloadSubject = new Subject<void>();
        eventsSubject = new Subject<ControlEvent>();
        personsServiceMock = {
            getPersonForUpdate: jest.fn(() => of()),
            updatePerson: jest.fn(),
            deletePerson: jest.fn(() => of(undefined)),
            reload$: reloadSubject,
        };
        paramMapSubject = new BehaviorSubject<ParamMap>(convertToParamMap({id: '123'}));
        activatedRouteMock = {
            paramMap: paramMapSubject.asObservable(),
            snapshot: {
                params: {id: '123'},
            } as unknown as ActivatedRouteSnapshot,
        };
        routerMock = {
            navigate: jest.fn().mockResolvedValue(true),
        };
        notificationServiceMock = {
            success: jest.fn(),
            error: jest.fn(),
            warning: jest.fn(),
        };
        personDetailsServiceMock = {
            isDataFormDirty: signal(false),
        };
        isObserverSubject = new BehaviorSubject(false);
        isAdminSubject = new BehaviorSubject(false);
        const authServiceMock = {
            isObserver$: isObserverSubject.asObservable(),
            isAdmin$: isAdminSubject.asObservable(),
        };
        const translateServiceMock = {
            currentLang: 'en',
            onLangChange: new Subject(),
            instant: jest.fn((key: string) => key),
        };
        reloadEntityAuditLogSubject = new BehaviorSubject<void>(undefined);
        entityAuditLogServiceMock = {
            reload$: reloadEntityAuditLogSubject,
        };
        dialogAfterClosedSubject = new Subject<boolean>();
        dialogMock = {
            open: jest.fn().mockReturnValue({
                afterClosed: () => dialogAfterClosedSubject.asObservable(),
            }),
        };

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), MockModule(ObAlertModule), MockComponents(PersonDataFormComponent), PersonDataComponent],
            providers: [
                {provide: PersonsService, useValue: personsServiceMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: Router, useValue: routerMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: PersonDetailsService, useValue: personDetailsServiceMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: AuthService, useValue: authServiceMock},
                {provide: EntityAuditLogService, useValue: entityAuditLogServiceMock},
                {provide: MatDialog, useValue: dialogMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(PersonDataComponent);
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
        component.personUpdate = signal({} as unknown as PersonUpdate);

        (personsServiceMock.getPersonForUpdate as jest.Mock).mockReturnValue(of({id: '123', surname: 'John Doe', canDelete: false}));
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
            expect(personsServiceMock.updatePerson).not.toHaveBeenCalled();
        });

        it('should call updatePerson and handle success', async () => {
            const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
            const reloadEntityAuditLogNextSpy = jest.spyOn(reloadEntityAuditLogSubject, 'next');
            const personBeforeSave = {id: '123', surname: 'John Doe', canDelete: false} as PersonUpdate;

            component.personUpdate.set(personBeforeSave);
            (personsServiceMock.updatePerson as jest.Mock).mockReturnValue(of(personBeforeSave));
            component['unmodifiedPerson'] = {id: '999', surname: 'Old Name'} as PersonUpdate;

            component.save();

            expect(personsServiceMock.updatePerson).toHaveBeenCalled();
            expect(reloadNextSpy).toHaveBeenCalled();
            expect(reloadEntityAuditLogNextSpy).toHaveBeenCalled();
            expect(routerMock.navigate).toHaveBeenCalledWith([]);

            await fixture.whenStable();

            expect(notificationServiceMock.success).toHaveBeenCalledWith('person.details.data.success');
            expect(component['unmodifiedPerson']).toEqual(personBeforeSave);
        });

        it('should show warning when isMissingJustificationFederalAssembly is true', async () => {
            const personWithMissingJustification = {
                id: '123',
                surname: 'John Doe',
                isMissingJustificationFederalAssembly: true,
                canDelete: true,
            } as PersonUpdate;

            component.personUpdate.set(personWithMissingJustification);
            (personsServiceMock.updatePerson as jest.Mock).mockReturnValue(of(personWithMissingJustification));

            component.save();

            await fixture.whenStable();

            expect(notificationServiceMock.success).toHaveBeenCalledWith('person.details.data.success');
        });

        it('should handle updatePerson error', () => {
            (personsServiceMock.updatePerson as jest.Mock).mockReturnValue(throwError(() => new Error('Update failed')));

            component.save();

            expect(personsServiceMock.updatePerson).toHaveBeenCalled();
            expect(notificationServiceMock.error).toHaveBeenCalledWith('person.details.data.error');
        });
    });

    describe('reset', () => {
        it('should reset form and update personModification', () => {
            const unmodifiedPerson = {id: '123', surname: 'Original Name'} as PersonUpdate;
            component['unmodifiedPerson'] = unmodifiedPerson;
            // @ts-ignore
            component.formComponent = jest.fn().mockReturnValue({
                personModification: {
                    set: jest.fn(),
                },
            });

            component.reset();

            expect(component.form().reset).toHaveBeenCalledWith(unmodifiedPerson, {emitEvent: false});
            expect(component.formComponent().personModification.set).toHaveBeenCalledWith(unmodifiedPerson);
        });
    });

    describe('deletePerson', () => {
        it('should delete person after confirmation and warn about memberships for admin', async () => {
            const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
            const personToDelete = {id: '123', surname: 'John Doe', canDelete: true} as PersonUpdate;
            component.personUpdate = signal(personToDelete);
            isAdminSubject.next(true);

            component.deletePerson();
            dialogAfterClosedSubject.next(true);

            await fixture.whenStable();

            expect(dialogMock.open).toHaveBeenCalledWith(ConfirmDialogComponent, {
                width: '600px',
                data: {
                    title: 'person.delete.title',
                    message: 'person.delete.text person.delete.membershipWarning',
                },
            });
            expect(personsServiceMock.deletePerson).toHaveBeenCalledWith(personToDelete.id);
            expect(reloadNextSpy).toHaveBeenCalled();
            expect(routerMock.navigate).toHaveBeenCalledWith(['persons']);
            expect(notificationServiceMock.success).toHaveBeenCalledWith('person.delete.success');
        });

        it('should not delete person when confirmation is dismissed', async () => {
            component.personUpdate = signal({id: '123', surname: 'John Doe', canDelete: true} as PersonUpdate);

            component.deletePerson();
            dialogAfterClosedSubject.next(false);

            await fixture.whenStable();

            expect(personsServiceMock.deletePerson).not.toHaveBeenCalled();
            expect(routerMock.navigate).not.toHaveBeenCalledWith(['persons']);
        });
    });

    describe('ngOnInit', () => {
        it('should update isDataFormDirty on PristineChangeEvent emission', () => {
            eventsSubject.next(new PristineChangeEvent(false, {} as unknown as AbstractControl));
            expect(personDetailsServiceMock.isDataFormDirty!()).toEqual(true);

            eventsSubject.next(new PristineChangeEvent(true, {} as unknown as AbstractControl));
            expect(personDetailsServiceMock.isDataFormDirty!()).toEqual(false);
        });
    });
});
