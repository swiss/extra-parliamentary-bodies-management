import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatTableModule} from '@angular/material/table';
import {MatTooltipModule} from '@angular/material/tooltip';
import {ActivatedRoute, ActivatedRouteSnapshot, convertToParamMap, ParamMap, Router} from '@angular/router';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {LangChangeEvent, TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObNotificationService} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MockComponents, MockModule, MockPipe, MockService} from 'ng-mocks';
import {BehaviorSubject, of, Subject} from 'rxjs';
import {AuthService} from '../../../../auth/auth.service';
import {CommitteeDetailsService} from '../../committee-details.service';
import {AppointmentDecisionService} from '../appointment-decision.service';
import {AppointmentDecisionListComponent} from './appointment-decision-list.component';

describe('AppointmentDecisionListComponent', () => {
    let component: AppointmentDecisionListComponent;
    let fixture: ComponentFixture<AppointmentDecisionListComponent>;
    let appointmentDecisionServiceMock: Partial<AppointmentDecisionService>;
    let committeeDetailsServiceMock: Partial<CommitteeDetailsService>;
    let reloadSubject: Subject<void>;
    let paramMapSubject: BehaviorSubject<ParamMap>;
    let routeMock: Partial<ActivatedRoute>;
    let routerMock: Partial<Router>;
    let onLangChangeSubject: Subject<LangChangeEvent>;
    let notificationServiceMock: Partial<ObNotificationService>;

    beforeEach(async () => {
        reloadSubject = new Subject<void>();

        onLangChangeSubject = new Subject();
        const translateServiceMock = {
            onLangChange: onLangChangeSubject.asObservable(),
            currentLang: 'de',
            instant: jest.fn(),
        };

        appointmentDecisionServiceMock = {
            getAppointmentDecisionList: jest.fn().mockReturnValue(of({id: '123'})),
            reload$: reloadSubject,
            downloadFile: jest.fn().mockReturnValue(of()),
        };

        notificationServiceMock = {
            success: jest.fn(),
            error: jest.fn(),
        };

        committeeDetailsServiceMock = {
            committeeDetails: signal<CommitteeDetails>({canEdit: true} as CommitteeDetails),
        };

        paramMapSubject = new BehaviorSubject<ParamMap>(convertToParamMap({id: '123'}));
        routeMock = {
            paramMap: paramMapSubject.asObservable(),
            snapshot: {
                paramMap: convertToParamMap({id: '123'}),
            } as unknown as ActivatedRouteSnapshot,
        };

        routerMock = {
            navigate: jest.fn().mockReturnValue({then: jest.fn()}),
        };

        await TestBed.configureTestingModule({
            imports: [
                MockPipe(TranslatePipe),
                MockModule(MatTableModule),
                MockModule(MatIconModule),
                MockModule(MatTooltipModule),
                MockModule(MatButtonModule),
                MockComponents(ConfirmDialogComponent, HelpTooltipComponent),
                AppointmentDecisionListComponent,
            ],
            providers: [
                {provide: AppointmentDecisionService, useValue: appointmentDecisionServiceMock},
                {provide: ActivatedRoute, useValue: routeMock},
                {provide: Router, useValue: routerMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: CommitteeDetailsService, useValue: committeeDetailsServiceMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {
                    provide: AuthService,
                    useValue: MockService(AuthService, {
                        isAdmin$: of(true),
                        isDepartmentUser$: of(false),
                        isOfficeUser$: of(false),
                        isSecretariatUser$: of(false),
                        isObserver$: of(false),
                    }),
                },
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(AppointmentDecisionListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
        await fixture.whenStable();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should set canEdit', () => {
        expect(component.canEdit()).toBe(true);
    });

    it('should initialize committeeId from route params', () => {
        expect(component.committeeId).toBe('123');
    });

    it('should call getAppointmentDecisionList on init', () => {
        expect(appointmentDecisionServiceMock.getAppointmentDecisionList).toHaveBeenCalledWith('123');
    });

    it('should update committeeId when route param changes', () => {
        routeMock.snapshot!.paramMap.get = jest.fn().mockReturnValue('456');
        paramMapSubject.next({
            get: jest.fn().mockReturnValue('456'),
        } as unknown as ParamMap);

        expect(component.committeeId).toBe('456');
        expect(appointmentDecisionServiceMock.getAppointmentDecisionList).toHaveBeenCalledWith('456');
    });

    it('should react to language changes', () => {
        onLangChangeSubject.next({lang: 'fr'} as unknown as LangChangeEvent);

        expect(appointmentDecisionServiceMock.getAppointmentDecisionList).toHaveBeenCalledTimes(2);
        expect(appointmentDecisionServiceMock.getAppointmentDecisionList).toHaveBeenCalledWith('123');
    });

    it('should download a document', () => {
        const documentId = 'DocId';
        const fileName = 'Myfile.pdf';

        component.downloadDocument(documentId, fileName);

        expect(appointmentDecisionServiceMock.downloadFile).toHaveBeenCalledTimes(1);
    });
});
