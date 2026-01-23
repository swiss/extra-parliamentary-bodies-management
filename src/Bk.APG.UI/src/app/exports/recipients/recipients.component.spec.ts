import {DOCUMENT, EventEmitter, signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatDatepicker, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule, MatFormField} from '@angular/material/select';
import {ActivatedRoute, ActivatedRouteSnapshot} from '@angular/router';
import {GeneralElectionCommitteeList} from '@api/GeneralElectionCommitteeList';
import {Office} from '@api/Office';
import {LangChangeEvent, TranslatePipe, TranslateService} from '@ngx-translate/core';
import {WINDOW, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {MasterDataService} from '@shared/master-data.service';
import {MockModule, MockComponents, MockPipe, MockService} from 'ng-mocks';
import {BehaviorSubject, of} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {GeneralElectionCommitteesService} from '../../general-election/ge-committees/ge-committees.service';
import {RecipientsComponent} from './recipients.component';
import {RecipientsService} from './recipients.service';

describe('RecipientsComponent', () => {
    let component: RecipientsComponent;
    let fixture: ComponentFixture<RecipientsComponent>;

    const reloadSubject: BehaviorSubject<void> = new BehaviorSubject<void>(undefined);

    const committeeList: GeneralElectionCommitteeList[] = [
        {id: 'id1', description: 'desc1'} as GeneralElectionCommitteeList,
        {id: 'id2', description: 'desc2'} as GeneralElectionCommitteeList,
    ];

    const generalElectionCommitteesServiceMock = {
        reload$: reloadSubject,
        getCommitteeListForExport: jest.fn().mockReturnValue(of(committeeList)),
    };

    const recipientsServiceMock: jest.Mocked<RecipientsService> = {
        generateReport: jest.fn().mockReturnValue(of(new Blob())),
    } as unknown as jest.Mocked<RecipientsService>;

    const activatedRouteMock: Partial<ActivatedRoute> = {
        snapshot: {
            data: {isGeneralElection: false},
        } as unknown as ActivatedRouteSnapshot,
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

    const masterDataServiceMock = {
        permittedDepartments: jest.fn(),
        permittedOffices: signal([
            {id: 'off1', text: 'Office 1', description: 'Office 1'},
            {id: 'off2', text: 'Office 2', description: 'Office 2'},
        ] as Office[]),
        committeeTypes: jest.fn(),
        languages: jest.fn(),
        electionTypes: jest.fn(),
    } as unknown as Partial<MasterDataService>;

    const languageChangeEmitter = new EventEmitter<LangChangeEvent>();
    const translateServiceMock = {
        onLangChange: languageChangeEmitter,
        currentLang: 'de',
        instant: jest.fn(),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                RecipientsComponent,
                MockModule(MatSelectModule),
                MockModule(MatInputModule),
                MockModule(ReactiveFormsModule),
                MockComponents(MatFormField, MatDatepicker, MatDatepickerToggle),
                MockPipe(TranslatePipe),
            ],
            providers: [
                {provide: WINDOW, useValue: window},
                {provide: DOCUMENT, useValue: document},
                {provide: RecipientsService, useValue: recipientsServiceMock},
                {provide: ObHttpApiInterceptorEvents, useValue: interceptorEventsMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: GeneralElectionCommitteesService, useValue: generalElectionCommitteesServiceMock},
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

        fixture = TestBed.createComponent(RecipientsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
