import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {FormArray, FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {ActivatedRoute, ActivatedRouteSnapshot, Router} from '@angular/router';
import {TranslateModule, TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MockComponents, MockModule, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, throwError} from 'rxjs';
import {CommitteesService} from '../../../../committees/committees.service';
import {ConfigsService} from '../../../../configs.service';
import {AppointmentDecisionService} from '../appointment-decision.service';
import {AppointmentDecisionDataFormComponent} from '../shared/appointment-decision-data-form/appointment-decision-data-form.component';
import {AppointmentDecisionCreateComponent} from './appointment-decision-create.component';

describe('AppointmentDecisionCreateComponent', () => {
    let component: AppointmentDecisionCreateComponent;
    let fixture: ComponentFixture<AppointmentDecisionCreateComponent>;
    const reloadSubject: BehaviorSubject<void> = new BehaviorSubject<void>(undefined);

    const errorServiceMock = {
        getControlError: jest.fn(),
    };
    const routerMock = {navigate: jest.fn().mockResolvedValue(true)};

    const appointmentDecisionServiceMock: Partial<AppointmentDecisionService> = {
        getAppointmentDecisionForCreate: jest.fn().mockReturnValue(of({})),
        createAppointmentDecision: jest.fn().mockReturnValue(of({})),
    };

    const activatedRouteMock = {
        snapshot: {
            params: {id: '123'},
        } as unknown as ActivatedRouteSnapshot,
    };

    const configsServiceMock = {
        frontendConfig: {
            entityIds: {
                appointmentDecisionType: {
                    decisionFederalCouncilId: 'decisionFederalCouncilId',
                    institutionId: 'institutionId',
                    reportId: 'reportId',
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
                MockModule(ReactiveFormsModule),
                MockModule(MatFormFieldModule),
                MockPipe(TranslatePipe),
                MockModule(TranslateModule),
                MockComponents(AppointmentDecisionDataFormComponent, HelpTooltipComponent),
                AppointmentDecisionCreateComponent,
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

        fixture = TestBed.createComponent(AppointmentDecisionCreateComponent);
        component = fixture.componentInstance;

        const mockFormComponent = {
            appointmentDecisionForm: new FormGroup({
                appointmentDecisionTypeId: new FormControl(''),
                text: new FormControl(''),
                link: new FormControl(''),
                appointmentDecisionLinkTypeId: new FormControl(''),
            }),
            documentsForm: new FormArray([]),
            isTypeDecisionFederalCouncilSelected: () => false,
            isInstitutionSelected: () => false,
            isReportSelected: () => false,
            isOtherSelected: () => false,
        } as unknown as AppointmentDecisionDataFormComponent;

        component.formComponent = signal(mockFormComponent);

        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should close and return to committee', () => {
        component.close();
        expect(routerMock.navigate).toHaveBeenCalledWith(['committees', '123'], {queryParams: {tab: 'decisions'}, replaceUrl: true});
    });

    it('should save appointment decision and handle success', async () => {
        const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
        // @ts-ignore
        component.form = signal({
            valid: true,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });

        component.save();
        await fixture.whenStable();
        expect(appointmentDecisionServiceMock.createAppointmentDecision).toHaveBeenCalled();
        expect(reloadNextSpy).toHaveBeenCalled();
        expect(routerMock.navigate).toHaveBeenCalledWith(['committees', '123'], {queryParams: {tab: 'decisions'}, replaceUrl: true});
        expect(notificationServiceMock.success).toHaveBeenCalledWith('appointmentDecision.save.success');
    });

    it('should handle error at save and show message', async () => {
        const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
        (appointmentDecisionServiceMock.createAppointmentDecision as jest.Mock).mockReturnValue(throwError(() => new Error('Create failed')));
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
        expect(appointmentDecisionServiceMock.createAppointmentDecision).toHaveBeenCalled();
        expect(notificationServiceMock.error).toHaveBeenCalledWith('appointmentDecision.save.error');
    });

    it('should return correct value for isOriginalSelected when selected', () => {
        component.appointmentDecisionToCreate.set({
            committeeId: 'committeeId',
            appointmentDecisionDate: new Date(2022, 2, 2),
            appointmentDecisionTypeId: 'typeId',
            text: 'text',
            documents: [{displayName: 'dname', isOriginal: true, languageId: 'lang', file: {} as File}],
        });
        expect(component.isOriginalSelected()).toBe(true);
    });

    it('should return correct value for isOriginalSelected when not selected', () => {
        component.appointmentDecisionToCreate.set({
            committeeId: 'committeeId',
            appointmentDecisionDate: new Date(2022, 2, 2),
            appointmentDecisionTypeId: 'typeId',
            text: 'text',
            documents: [{displayName: 'dname', isOriginal: false, languageId: 'lang', file: {} as File}],
        });
        expect(component.isOriginalSelected()).toBe(false);
    });

    it.each([
        ['institutionId', true],
        ['decisionFederalCouncilId', false],
    ])('should return correct value for isInstitution', (typeId, expected) => {
        component.appointmentDecisionToCreate.set({
            committeeId: 'committeeId',
            appointmentDecisionDate: new Date(2022, 2, 2),
            appointmentDecisionTypeId: typeId,
            text: 'text',
            documents: [{displayName: 'dname', isOriginal: false, languageId: 'lang', file: {} as File}],
        });
        expect(component.isInstitution()).toBe(expected);
    });

    it.each([
        ['reportId', true],
        ['decisionFederalCouncilId', false],
        ['institutionId', false],
    ])('should return correct value for isReport', (typeId, expected) => {
        component.appointmentDecisionToCreate.set({
            committeeId: 'committeeId',
            appointmentDecisionDate: new Date(2022, 2, 2),
            appointmentDecisionTypeId: typeId,
            text: 'text',
            documents: [{displayName: 'dname', isOriginal: false, languageId: 'lang', file: {} as File}],
        });
        expect(component.isReport()).toBe(expected);
    });

    it('should mark appointmentDecisionTypeId as touched', () => {
        fixture.detectChanges();
        const control = component.form().controls.appointmentDecisionTypeId;
        expect(control.touched).toBe(true);
    });
});
