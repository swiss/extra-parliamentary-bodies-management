import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatButtonModule} from '@angular/material/button';
import {MatDatepicker, MatDatepickerModule, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatFormField, MatSelectModule} from '@angular/material/select';
import {Office} from '@api/Office';
import {LangChangeEvent, TranslateModule, TranslateService} from '@ngx-translate/core';
import {ObButtonModule, ObInputClearDirective} from '@oblique/oblique';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {SearchStorageService} from '@shared/services/search-storage.service';
import {MockComponents, MockDirective, MockModule, MockService} from 'ng-mocks';
import {of, Subject} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {WorklistFilterComponent} from './worklist-filter.component';

describe('WorklistFilterComponent', () => {
    let component: WorklistFilterComponent;
    let fixture: ComponentFixture<WorklistFilterComponent>;

    const masterDataServiceMock = {
        permittedDepartments: jest.fn(),
        permittedOffices: signal([
            {id: 'off1', text: 'Office 1', description: 'Office 1'},
            {id: 'off2', text: 'Office 2', description: 'Office 2'},
        ] as Office[]),
        committeeTypes: jest.fn(),
        worklistTaskTypes: signal([
            {id: 'id1', text: 'type1', description: 'desc1', canBeCreatedManually: true},
            {id: 'id2', text: 'type2', description: 'desc2', canBeCreatedManually: false},
        ]),
        worklistTaskStates: signal([
            {id: 'id1', text: 'state1', description: 'desc1'},
            {id: 'id2', text: 'state2', description: 'desc2'},
        ]),
    } as unknown as Partial<MasterDataService>;

    const langChangeSubject = new Subject<LangChangeEvent>();

    const translateServiceMock = {
        currentLang: 'en',
        onLangChange: langChangeSubject,
        get: jest.fn(),
    };

    const searchStorageServiceMock = {
        getParams: jest.fn(),
        setParams: jest.fn(),
        removeParams: jest.fn(),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                WorklistFilterComponent,
                MockModule(MatDatepickerModule),
                MockModule(TranslateModule),
                MockModule(MatFormFieldModule),
                MockModule(MatSelectModule),
                MockModule(MatButtonModule),
                MockModule(ObButtonModule),
                MockModule(MatIconModule),
                MockDirective(ObInputClearDirective),
                MockComponents(MatFormField, MatDatepicker, MatDatepickerToggle, HelpTooltipComponent),
            ],
            providers: [
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: AuthService, useValue: MockService(AuthService, {isAdmin$: of(true), isDepartmentUser$: of(false)})},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: SearchStorageService, useValue: searchStorageServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(WorklistFilterComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
