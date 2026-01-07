import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {ActivatedRoute, ActivatedRouteSnapshot, Router} from '@angular/router';
import {AppointmentDecisionUpdate} from '@api/AppointmentDecisionUpdate';
import {TranslateModule, TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {MockComponent, MockModule, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, throwError} from 'rxjs';
import {CommitteesService} from '../../../../committees/committees.service';
import {ConfigsService} from '../../../../configs.service';
import {AppointmentDecisionService} from '../appointment-decision.service';
import {AppointmentDecisionDataFormComponent} from '../shared/appointment-decision-data-form/appointment-decision-data-form.component';
import {AppointmentDecisionEditComponent} from './appointment-decision-edit.component';

describe('AppointmentDecisionEditComponent', () => {
    const appointmentDecisionUpdate = {
        id: '1',
        appointmentDecisionDate: new Date(2025, 1, 1),
        appointmentDecisionTypeId: '2',
        appointmentDecisionLinkTypeId: '3',
        text: 'text',
        link: 'link',
        documents: [
            {
                id: '11',
                displayName: 'dName',
                isOriginal: true,
                languageId: 'DE',
                file: {} as File,
            },
        ],
    };

    let component: AppointmentDecisionEditComponent;
    let fixture: ComponentFixture<AppointmentDecisionEditComponent>;
    const reloadSubject: BehaviorSubject<void> = new BehaviorSubject<void>(undefined);

    const errorServiceMock = {
        getControlError: jest.fn(),
    };
    const routerMock = {navigate: jest.fn().mockResolvedValue(true)};

    const appointmentDecisionServiceMock: Partial<AppointmentDecisionService> = {
        getAppointmentDecisionForUpdate: jest.fn().mockReturnValue(of(appointmentDecisionUpdate)),
        updateAppointmentDecision: jest.fn().mockReturnValue(of({})),
    };

    const activatedRouteMock = {
        parent: {
            snapshot: {params: {id: '111'}},
        },
        snapshot: {
            params: {id: '222'},
        } as unknown as ActivatedRouteSnapshot,
    };

    const configsServiceMock = {
        frontendConfig: {
            entityIds: {
                appointmentDecisionType: {
                    decisionFederalCouncilId: 'decisionFederalCouncilId',
                    institutionId: 'institutionId',
                    reportId: 'reportId',
                    otherId: 'otherId',
                },
                appointmentDecisionLinkType: {
                    exeLinkTypeId: 'exeLinkTypeId',
                    standardLinkTypeId: 'standardLinkTypeId',
                },
                language: {
                    germanLanguageId: 'germanLanguageId',
                    frenchLanguageId: 'frenchLanguageId',
                    italianLanguageId: 'italianLanguageId',
                    romanshLanguageId: 'romanshLanguageId',
                },
            },
        },
    };

    const committeesServiceMock: Partial<CommitteesService> = {
        createCommittee: jest.fn(),
        reload$: reloadSubject,
    };

    const notificationServiceMock = {
        success: jest.fn(),
        error: jest.fn().mockReturnValue(of()),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                AppointmentDecisionEditComponent,
                MockModule(ReactiveFormsModule),
                MockModule(MatFormFieldModule),
                MockPipe(TranslatePipe),
                MockModule(TranslateModule),
                MockComponent(AppointmentDecisionDataFormComponent),
            ],
            providers: [
                {provide: ErrorService, useValue: errorServiceMock},
                {provide: AppointmentDecisionService, useValue: appointmentDecisionServiceMock},
                {provide: CommitteesService, useValue: committeesServiceMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: ConfigsService, useValue: configsServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(AppointmentDecisionEditComponent);
        component = fixture.componentInstance;

        // @ts-ignore
        component.form = () => ({
            pristine: true,
        });
        // @ts-ignore
        component.documentForm = () => ({pristine: true, reset: jest.fn()});
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should close and return to committee', () => {
        component.back();
        expect(routerMock.navigate).toHaveBeenCalledWith(['committees', '111'], {queryParams: {tab: 'decisions'}, replaceUrl: true});
    });

    it('should update appointment decision and handle success', async () => {
        const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
        // @ts-ignore
        component.form = signal({
            valid: true,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });

        component.save();
        await fixture.whenStable();
        expect(appointmentDecisionServiceMock.updateAppointmentDecision).toHaveBeenCalled();
        expect(reloadNextSpy).toHaveBeenCalled();
        expect(routerMock.navigate).toHaveBeenCalledWith(['committees', '111'], {queryParams: {tab: 'decisions'}, replaceUrl: true});
        expect(notificationServiceMock.success).toHaveBeenCalledWith('appointmentDecision.save.success');
    });

    it('should handle error at save and show message', async () => {
        const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
        (appointmentDecisionServiceMock.updateAppointmentDecision as jest.Mock).mockReturnValue(throwError(() => new Error('Create failed')));
        // @ts-ignore
        component.form = signal({
            valid: true,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });

        component.save();
        await fixture.whenStable();
        expect(reloadNextSpy).not.toHaveBeenCalled();
        expect(routerMock.navigate).not.toHaveBeenCalled();
        expect(appointmentDecisionServiceMock.updateAppointmentDecision).toHaveBeenCalled();
        expect(notificationServiceMock.error).toHaveBeenCalledWith('appointmentDecision.save.error');
    });

    it('should return correct value for isOriginalSelected when selected', () => {
        component.appointmentDecisionToUpdate = signal<AppointmentDecisionUpdate>({
            id: 'id',
            appointmentDecisionDate: new Date(2022, 2, 2),
            appointmentDecisionTypeId: 'typeId',
            text: 'text',
            documents: [{id: 'docId', displayName: 'dname', isOriginal: true, languageId: 'lang', file: {} as File}],
        });
        expect(component.isOriginalSelected()).toBe(true);
    });

    it('should return correct value for isOriginalSelected when not selected', () => {
        component.appointmentDecisionToUpdate = signal<AppointmentDecisionUpdate>({
            id: 'id',
            appointmentDecisionDate: new Date(2022, 2, 2),
            appointmentDecisionTypeId: 'typeId',
            text: 'text',
            documents: [{id: 'docId', displayName: 'dname', isOriginal: false, languageId: 'lang', file: {} as File}],
        });
        expect(component.isOriginalSelected()).toBe(false);
    });

    it.each([
        ['institutionId', true],
        ['decisionFederalCouncilId', false],
    ])('should return correct value for isInstitution', (typeId, expected) => {
        component.appointmentDecisionToUpdate = signal<AppointmentDecisionUpdate>({
            id: 'id',
            appointmentDecisionDate: new Date(2022, 2, 2),
            appointmentDecisionTypeId: typeId,
            text: 'text',
            documents: [{id: 'docId', displayName: 'dname', isOriginal: false, languageId: 'lang', file: {} as File}],
        });
        expect(component.isInstitution()).toBe(expected);
    });

    it.each([
        ['reportId', true],
        ['decisionFederalCouncilId', false],
        ['institutionId', false],
    ])('should return correct value for isReport', (typeId, expected) => {
        component.appointmentDecisionToUpdate = signal<AppointmentDecisionUpdate>({
            id: 'id',
            appointmentDecisionDate: new Date(2022, 2, 2),
            appointmentDecisionTypeId: typeId,
            text: 'text',
            documents: [{id: 'docId', displayName: 'dname', isOriginal: false, languageId: 'lang', file: {} as File}],
        });
        expect(component.isReport()).toBe(expected);
    });
});
