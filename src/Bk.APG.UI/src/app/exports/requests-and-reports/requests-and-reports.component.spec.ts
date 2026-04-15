import {EventEmitter, signal, DOCUMENT} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatDatepicker, MatDatepickerModule, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatInputModule} from '@angular/material/input';
import {MatFormField, MatSelectModule} from '@angular/material/select';
import {ActivatedRoute, ActivatedRouteSnapshot} from '@angular/router';
import {CommitteeList} from '@api/CommitteeList';
import {Office} from '@api/Office';
import {LangChangeEvent, TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObHttpApiInterceptorEvents, ObNotificationService, WINDOW} from '@oblique/oblique';
import {today} from '@shared/date-util';
import * as fileUtil from '@shared/file-util';
import {MasterDataService} from '@shared/master-data.service';
import {MockComponents, MockModule, MockPipe, MockService} from 'ng-mocks';
import {BehaviorSubject, of, throwError} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {CommitteesService} from '../../committees/committees.service';
import {ReportType} from '../ReportType';
import {RequestsAndReportsComponent} from './requests-and-reports.component';
import {RequestsAndReportsService} from './requests-and-reports.service';

describe('RequestsAndReportsComponent', () => {
    let component: RequestsAndReportsComponent;
    let fixture: ComponentFixture<RequestsAndReportsComponent>;
    const reloadSubject: BehaviorSubject<void> = new BehaviorSubject<void>(undefined);

    const committeeList: CommitteeList[] = [{id: 'id1', description: 'desc1'} as CommitteeList, {id: 'id2', description: 'desc2'} as CommitteeList];
    const committeesServiceMock = {
        reload$: reloadSubject,
        getCommitteeListForExport: jest.fn().mockReturnValue(of(committeeList)),
    };

    const interceptorEventsMock: jest.Mocked<ObHttpApiInterceptorEvents> = {
        deactivateSpinnerOnNextAPICalls: jest.fn(),
        deactivateNotificationOnNextAPICalls: jest.fn(),
    } as unknown as jest.Mocked<ObHttpApiInterceptorEvents>;

    const notificationServiceMock: jest.Mocked<ObNotificationService> = {
        info: jest.fn(),
        success: jest.fn(),
        error: jest.fn(),
    } as unknown as jest.Mocked<ObNotificationService>;

    const requestsAndReportsServiceMock: jest.Mocked<RequestsAndReportsService> = {
        generateReport: jest.fn().mockReturnValue(of(new Blob())),
    } as unknown as jest.Mocked<RequestsAndReportsService>;

    const masterDataServiceMock = {
        permittedDepartments: jest.fn(),
        permittedOffices: signal([
            {id: 'off1', text: 'Office 1', description: 'Office 1'},
            {id: 'off2', text: 'Office 2', description: 'Office 2'},
        ] as Office[]),
        committeeTypes: jest.fn(),
        termDates: signal([
            {id: 'id1', text: 'type1', description: 'desc1', beginDate: new Date(2024, 0, 1), endDate: new Date(2025, 11, 31)},
            {id: 'id2', text: 'type2', description: 'desc2', beginDate: new Date(2024, 0, 1), endDate: new Date(2025, 11, 31)},
        ]),
    } as unknown as Partial<MasterDataService>;

    const languageChangeEmitter = new EventEmitter<LangChangeEvent>();
    const translateServiceMock = {
        onLangChange: languageChangeEmitter,
        getCurrentLang: jest.fn(() => 'de'),
        instant: jest.fn(),
    };

    const activatedRouteMock: Partial<ActivatedRoute> = {
        snapshot: {
            data: {isGeneralElection: false},
        } as unknown as ActivatedRouteSnapshot,
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                RequestsAndReportsComponent,
                MockModule(MatDatepickerModule),
                MockModule(MatSelectModule),
                MockModule(MatInputModule),
                MockModule(ReactiveFormsModule),
                MockComponents(MatFormField, MatDatepicker, MatDatepickerToggle),
                MockPipe(TranslatePipe),
            ],
            providers: [
                {provide: WINDOW, useValue: window},
                {provide: DOCUMENT, useValue: document},
                {provide: RequestsAndReportsService, useValue: requestsAndReportsServiceMock},
                {provide: ObHttpApiInterceptorEvents, useValue: interceptorEventsMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: CommitteesService, useValue: committeesServiceMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
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

        fixture = TestBed.createComponent(RequestsAndReportsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should load isGeneralElection correctly', () => {
        expect(component.isGeneralElection).toBe(false);
    });

    it.each([ReportType.AppendixFederalCouncilCheck, ReportType.Vacancies])('should update date fields properly for document Type "%s"', dt => {
        component.form.controls.documentType.setValue(dt);
        expect(component.form.controls.analysisDate1.value).toEqual(today());
    });

    it.each([ReportType.CompareListGeneralElection])('should update "date 1" field and enable "date 2" field properly for document Type "%s"', dt => {
        component.analysisDateDefaultValue = new Date(2023, 2, 2);
        component.form.controls.documentType.setValue(dt);
        expect(component.form.controls.analysisDate1.value).toEqual(component.analysisDateDefaultValue);
        expect(component.form.controls.analysisDate2.value).toEqual(today());
        expect(component.form.controls.analysisDate2.disabled).toEqual(false);
    });

    it.each([ReportType.ParliamentaryReport, ReportType.Vacancies, ReportType.AppendixFederalCouncilCheck])(
        'should clear and disable "date 2" field for document Type "%s"',
        dt => {
            component.form.controls.documentType.setValue(dt);
            expect(component.form.controls.analysisDate2.disabled).toEqual(true);
            expect(component.form.controls.analysisDate2.value).toEqual(null);
        }
    );

    it('should load masterData correctly', () => {
        expect(component.departmentOffices().length).toBe(2);
        expect(component.termDates().length).toBe(2);
    });

    it('should load at start with the correct count of entries', () => {
        fixture.detectChanges();
        expect(committeesServiceMock.getCommitteeListForExport).toHaveBeenCalledTimes(5);
    });

    it('should call dataAnalysisService.generateExport with correct parameters', () => {
        component.form.controls.documentType.setValue(ReportType.ParliamentaryReport);
        const analysisDate = new Date(2024, 0, 1);
        component.form.controls.analysisDate1.setValue(analysisDate);
        component.form.controls.analysisDate2.setValue(analysisDate);
        component.form.controls.committeeTypes.setValue(['committeeTypeId']);
        component.form.controls.departments.setValue(['depId']);
        component.form.controls.offices.setValue(['offId']);
        component.selectedItems = new Set<CommitteeList>([
            {
                id: 'committeeId',
                commiteeId: 2,
                description: 'desc',
                level: 'level',
                department: 'department',
                office: 'office',
                committeeType: 'committeeType',
                term: 'term',
                isActive: true,
                isMarketOrientated: 'true',
                hasSupervisionDuty: false,
            },
        ]);

        requestsAndReportsServiceMock.generateReport = jest.fn().mockReturnValue(of(new Blob()));

        component.generateReport();

        expect(requestsAndReportsServiceMock.generateReport).toHaveBeenCalledWith({
            analysisDate1: analysisDate,
            analysisDate2: analysisDate,
            departments: ['depId'],
            documentType: ReportType.ParliamentaryReport,
            offices: ['offId'],
            committeeTypes: ['committeeTypeId'],
            committees: ['committeeId'],
            committeesWithActiveMembership: true,
            releasedCommittees: true,
            isGeneralElection: false,
        });
    });

    it('should show info notification when export starts', () => {
        component.generateReport();

        expect(notificationServiceMock.info).toHaveBeenCalledWith(
            expect.objectContaining({
                title: 'requestsAndReports.download.title',
                message: 'requestsAndReports.download.message',
                timeout: 10000,
            })
        );
    });

    it('should show success notification and update successful exports on success', () => {
        component.form.controls.documentType.setValue(ReportType.ParliamentaryReport);

        component.generateReport();

        expect(notificationServiceMock.success).toHaveBeenCalledWith(
            expect.objectContaining({
                message: 'requestsAndReports.download.success',
            })
        );
    });

    it('should call downloadFileFromHttpResponse on success', () => {
        const downloadSpy = jest.spyOn(fileUtil, 'downloadFileFromHttpResponse').mockImplementation(() => {});

        component.generateReport();

        expect(downloadSpy).toHaveBeenCalled();
        downloadSpy.mockRestore();
    });

    it('should show error notification on error', () => {
        component.form.controls.documentType.setValue(ReportType.ParliamentaryReport);

        requestsAndReportsServiceMock.generateReport = jest.fn().mockReturnValue(throwError(() => new Error('fail')));

        component.generateReport();

        expect(notificationServiceMock.error).toHaveBeenCalledWith(
            expect.objectContaining({
                message: 'requestsAndReports.download.error',
            })
        );
    });

    it('should not reload on language change with the same language', () => {
        committeesServiceMock.getCommitteeListForExport.mockClear();

        translateServiceMock.onLangChange.emit({lang: 'de'} as LangChangeEvent);

        expect(committeesServiceMock.getCommitteeListForExport).not.toHaveBeenCalled();
    });

    it('should reload committees on language change', () => {
        committeesServiceMock.getCommitteeListForExport.mockClear();

        translateServiceMock.onLangChange.emit({lang: 'fr'} as LangChangeEvent);

        expect(committeesServiceMock.getCommitteeListForExport).toHaveBeenCalledTimes(1);
    });

    it('should reload committees on filter change', () => {
        committeesServiceMock.getCommitteeListForExport.mockClear();

        component.form.controls.departments.setValue(['EDI']);

        expect(committeesServiceMock.getCommitteeListForExport).toHaveBeenCalledTimes(1);
    });
});
