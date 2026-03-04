/* eslint-disable dot-notation */
/* eslint-disable @typescript-eslint/no-explicit-any */
import {signal, WritableSignal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {AbstractControl, ControlEvent, FormGroup, PristineChangeEvent} from '@angular/forms';
import {MatDialog, MatDialogRef} from '@angular/material/dialog';
import {ActivatedRoute, ActivatedRouteSnapshot, Router} from '@angular/router';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {CommitteeUpdate} from '@api/CommitteeUpdate';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObAlertModule, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {MockComponents, MockModule, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, Subject, throwError} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';
import {GeneralElectionService} from '../../../general-election/general-election.service';
import {CommitteesService} from '../../committees.service';
import {CommitteeDataFormComponent} from '../../shared/committee-data-form/committee-data-form.component';
import {CommitteeDetailsService} from '../committee-details.service';
import {CommitteeDataComponent} from './committee-data.component';

describe('CommitteeDataComponent', () => {
    let component: CommitteeDataComponent;
    let fixture: ComponentFixture<CommitteeDataComponent>;
    let committeesServiceMock: Partial<CommitteesService>;
    let activatedRouteMock: Partial<ActivatedRoute>;
    let routerMock: Partial<Router>;
    let notificationServiceMock: Partial<ObNotificationService>;
    let committeeDetailsServiceMock: Partial<CommitteeDetailsService>;
    let httpApiInterceptorEventsMock: Partial<ObHttpApiInterceptorEvents>;
    let entityAuditLogServiceMock: Partial<EntityAuditLogService>;
    let reloadSubject: BehaviorSubject<void>;
    let eventsSubject: Subject<ControlEvent>;
    let reloadEntityAuditLogSubject: BehaviorSubject<void>;
    let form: WritableSignal<FormGroup>;
    let matDialogMock: Partial<MatDialog>;
    let translateServiceMock: Partial<TranslateService>;
    let authServiceMock: Partial<AuthService>;
    let mockGeneralElectionService: Partial<GeneralElectionService>;

    beforeEach(async () => {
        reloadSubject = new BehaviorSubject<void>(undefined);
        eventsSubject = new Subject<ControlEvent>();
        reloadEntityAuditLogSubject = new BehaviorSubject<void>(undefined);

        committeesServiceMock = {
            getCommitteeForUpdate: jest.fn(() => of()),
            updateCommittee: jest.fn(),
            reload$: reloadSubject,
        };
        committeeDetailsServiceMock = {
            isDataFormDirty: signal(false),
            committeeDetails: signal<CommitteeDetails>({} as CommitteeDetails),
        };
        activatedRouteMock = {
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
        };
        httpApiInterceptorEventsMock = {
            deactivateNotificationOnNextAPICalls: jest.fn(),
        };
        entityAuditLogServiceMock = {
            reload$: reloadEntityAuditLogSubject,
        };

        matDialogMock = {
            open: jest.fn(),
        };

        translateServiceMock = {
            instant: jest.fn((key: string) => key),
        };

        authServiceMock = {
            isAdmin$: new BehaviorSubject(false),
        };

        mockGeneralElectionService = {
            isGeneralElectionVisible: jest.fn(),
        } as any;

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), MockComponents(CommitteeDataFormComponent), MockModule(ObAlertModule), CommitteeDataComponent],
            providers: [
                {provide: CommitteesService, useValue: committeesServiceMock},
                {provide: CommitteeDetailsService, useValue: committeeDetailsServiceMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: Router, useValue: routerMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: ObHttpApiInterceptorEvents, useValue: httpApiInterceptorEventsMock},
                {provide: EntityAuditLogService, useValue: entityAuditLogServiceMock},
                {provide: MatDialog, useValue: matDialogMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: AuthService, useValue: authServiceMock},
                {provide: GeneralElectionService, useValue: mockGeneralElectionService},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(CommitteeDataComponent);
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
        component.committeeUpdate = signal({} as unknown as CommitteeUpdate);

        (committeesServiceMock.getCommitteeForUpdate as jest.Mock).mockReturnValue(of({id: '123', committeeNumber: 1337, isActive: true}));
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
            expect(committeesServiceMock.updateCommittee).not.toHaveBeenCalled();
        });

        it('should call updateCommittee and handle success', async () => {
            (committeesServiceMock.updateCommittee as jest.Mock).mockReturnValue(of({}));
            const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
            const reloadEntityAuditLogNextSpy = jest.spyOn(reloadEntityAuditLogSubject, 'next');
            const committeeBeforeSave = {id: '123', committeeNumber: 1337, isActive: true} as CommitteeUpdate;

            component.committeeUpdate.set(committeeBeforeSave);
            component['unmodifiedCommittee'] = {id: '999', committeeNumber: 42, isActive: false} as CommitteeUpdate;

            component.save();

            expect(committeesServiceMock.updateCommittee).toHaveBeenCalled();
            expect(reloadNextSpy).toHaveBeenCalled();
            expect(reloadEntityAuditLogNextSpy).toHaveBeenCalled();
            expect(routerMock.navigate).toHaveBeenCalledWith([]);

            await fixture.whenStable();

            expect(httpApiInterceptorEventsMock.deactivateNotificationOnNextAPICalls).toHaveBeenCalled();
            expect(notificationServiceMock.success).toHaveBeenCalledWith('committee.details.data.success');
            expect(component['unmodifiedCommittee']).toEqual(committeeBeforeSave);
        });

        it('should handle updateCommittee error', () => {
            (committeesServiceMock.updateCommittee as jest.Mock).mockReturnValue(throwError(() => new Error('Update failed')));

            component.save();

            expect(httpApiInterceptorEventsMock.deactivateNotificationOnNextAPICalls).toHaveBeenCalled();
            expect(committeesServiceMock.updateCommittee).toHaveBeenCalled();
            expect(notificationServiceMock.error).toHaveBeenCalledWith('committee.details.data.error');
        });
    });

    describe('reset', () => {
        it('should reset form', () => {
            component.reset();

            expect(component.form().reset).toHaveBeenCalled();
        });
    });

    describe('ngOnInit', () => {
        it('should update isDataFormDirty on PristineChangeEvent emission', () => {
            eventsSubject.next(new PristineChangeEvent(false, {} as unknown as AbstractControl));
            expect(committeeDetailsServiceMock.isDataFormDirty!()).toEqual(true);

            eventsSubject.next(new PristineChangeEvent(true, {} as unknown as AbstractControl));
            expect(committeeDetailsServiceMock.isDataFormDirty!()).toEqual(false);
        });
    });

    describe('End date confirmation dialog', () => {
        let formComponentMock: any;
        let dialogRefMock: Partial<MatDialogRef<any>>;

        beforeEach(() => {
            dialogRefMock = {
                afterClosed: jest.fn().mockReturnValue(of(true)),
            };

            formComponentMock = {
                hasEndDateSet: jest.fn(() => false),
                markEndDateAsSaved: jest.fn(),
            };

            // Override the viewChild to return our mock
            Object.defineProperty(component, 'formComponent', {
                value: () => formComponentMock,
                writable: true,
            });

            (matDialogMock.open as jest.Mock).mockReturnValue(dialogRefMock);
        });

        it('should not show dialog when admin user', async () => {
            (authServiceMock.isAdmin$ as BehaviorSubject<boolean>).next(true);
            const committeeUpdate = {
                id: '123',
                endDate: new Date('2024-12-31'),
            } as CommitteeUpdate;
            component.committeeUpdate.set(committeeUpdate);
            component['unmodifiedCommittee'] = {id: '123'} as CommitteeUpdate;

            (committeesServiceMock.updateCommittee as jest.Mock).mockReturnValue(of({}));

            component.save();

            await fixture.whenStable();

            expect(matDialogMock.open).not.toHaveBeenCalled();
            expect(committeesServiceMock.updateCommittee).toHaveBeenCalled();
        });

        it('should not show dialog when end date has not changed', async () => {
            (authServiceMock.isAdmin$ as BehaviorSubject<boolean>).next(false);
            const endDate = new Date('2024-12-31');
            const committeeUpdate = {id: '123', endDate} as CommitteeUpdate;
            component.committeeUpdate.set(committeeUpdate);
            component['unmodifiedCommittee'] = {id: '123', endDate} as CommitteeUpdate;

            (committeesServiceMock.updateCommittee as jest.Mock).mockReturnValue(of({}));

            component.save();

            await fixture.whenStable();

            expect(matDialogMock.open).not.toHaveBeenCalled();
            expect(committeesServiceMock.updateCommittee).toHaveBeenCalled();
        });

        it('should show dialog when non-admin user sets new end date', async () => {
            (authServiceMock.isAdmin$ as BehaviorSubject<boolean>).next(false);
            const committeeUpdate = {
                id: '123',
                endDate: new Date('2024-12-31'),
            } as CommitteeUpdate;
            component.committeeUpdate.set(committeeUpdate);
            component['unmodifiedCommittee'] = {id: '123'} as CommitteeUpdate;

            (committeesServiceMock.updateCommittee as jest.Mock).mockReturnValue(of({}));

            component.save();

            await fixture.whenStable();

            expect(matDialogMock.open).toHaveBeenCalledWith(expect.anything(), {
                width: '600px',
                data: {
                    title: 'committee.endDate.confirmation.title',
                    message: 'committee.endDate.confirmation.message',
                },
            });
        });

        it('should call performSave when dialog is confirmed', async () => {
            (authServiceMock.isAdmin$ as BehaviorSubject<boolean>).next(false);
            const committeeUpdate = {
                id: '123',
                endDate: new Date('2024-12-31'),
            } as CommitteeUpdate;
            component.committeeUpdate.set(committeeUpdate);
            component['unmodifiedCommittee'] = {id: '123'} as CommitteeUpdate;

            (committeesServiceMock.updateCommittee as jest.Mock).mockReturnValue(of({}));

            component.save();

            await fixture.whenStable();

            expect(committeesServiceMock.updateCommittee).toHaveBeenCalled();
        });

        it('should not call performSave when dialog is cancelled', async () => {
            (authServiceMock.isAdmin$ as BehaviorSubject<boolean>).next(false);
            (dialogRefMock.afterClosed as jest.Mock).mockReturnValue(of(false));

            const committeeUpdate = {
                id: '123',
                endDate: new Date('2024-12-31'),
            } as CommitteeUpdate;
            component.committeeUpdate.set(committeeUpdate);
            component['unmodifiedCommittee'] = {id: '123'} as CommitteeUpdate;

            (committeesServiceMock.updateCommittee as jest.Mock).mockReturnValue(of({}));

            component.save();

            await fixture.whenStable();

            expect(committeesServiceMock.updateCommittee).not.toHaveBeenCalled();
        });

        it('should call markEndDateAsSaved after successful save', async () => {
            (authServiceMock.isAdmin$ as BehaviorSubject<boolean>).next(true);
            const committeeUpdate = {
                id: '123',
                endDate: new Date('2024-12-31'),
            } as CommitteeUpdate;
            component.committeeUpdate.set(committeeUpdate);
            component['unmodifiedCommittee'] = {id: '123'} as CommitteeUpdate;

            (committeesServiceMock.updateCommittee as jest.Mock).mockReturnValue(of({}));

            component.save();

            await fixture.whenStable();

            expect(formComponentMock.markEndDateAsSaved).toHaveBeenCalled();
        });
    });
});
