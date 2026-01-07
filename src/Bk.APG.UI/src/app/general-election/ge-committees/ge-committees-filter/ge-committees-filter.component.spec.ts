import {signal} from '@angular/core';
import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatIcon} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {Router} from '@angular/router';
import {EiamAssignment} from '@api/EiamAssignment';
import {Office} from '@api/Office';
import {TranslateModule} from '@ngx-translate/core';
import {ObButtonModule, ObInputClearDirective} from '@oblique/oblique';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {EiamAssignmentService} from '@shared/services/eiam-assignment.service';
import {SearchStorageService} from '@shared/services/search-storage.service';
import {MockComponent, MockComponents, MockDirective, MockModule} from 'ng-mocks';
import {BehaviorSubject, of} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';
import {GeneralElectionCommitteesFilterComponent} from './ge-committees-filter.component';

describe('GeCommitteesFilterComponent', () => {
    let component: GeneralElectionCommitteesFilterComponent;
    let fixture: ComponentFixture<GeneralElectionCommitteesFilterComponent>;

    const masterDataServiceMock = {
        levels: jest.fn(),
        departments: jest.fn(),
        offices: signal([] as Office[]),
        committeeTypes: jest.fn(),
        terms: jest.fn(),
    } as unknown as Partial<MasterDataService>;

    const routerMock = {
        navigate: jest.fn(),
    };

    const isDepartmentUserSubject = new BehaviorSubject(false);
    const authServiceMock = {
        isAdmin$: of(true),
        isDepartmentUser$: isDepartmentUserSubject.asObservable(),
    };

    const currentAssignmentSubject = new BehaviorSubject<EiamAssignment>({} as EiamAssignment);
    const eiamAssignmentServiceMock = {
        getCurrentEiamAssignment: jest.fn(() => currentAssignmentSubject.asObservable()),
    } as unknown as Partial<EiamAssignmentService>;

    const searchStorageServiceMock = {
        getParams: jest.fn(),
        setParams: jest.fn(),
        removeParams: jest.fn(),
    };

    beforeEach(async () => {
        component = await TestBed.configureTestingModule({
            imports: [
                GeneralElectionCommitteesFilterComponent,
                ReactiveFormsModule,
                MockModule(TranslateModule),
                MockModule(MatFormFieldModule),
                MockModule(MatInputModule),
                MockModule(MatSelectModule),
                MockModule(MatButtonModule),
                MockModule(ObButtonModule),
                MockComponent(MatIcon),
                MockDirective(ObInputClearDirective),
                MockComponents(HelpTooltipComponent),
            ],
            providers: [
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: AuthService, useValue: authServiceMock},
                {provide: EiamAssignmentService, useValue: eiamAssignmentServiceMock},
                {provide: SearchStorageService, useValue: searchStorageServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(GeneralElectionCommitteesFilterComponent);
        component = fixture.componentInstance;

        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should filter', fakeAsync(() => {
        jest.spyOn(component.filter, 'emit');

        component.form.controls.departments.setValue(['foo']);
        tick();

        expect(component.filter.emit).toHaveBeenCalledTimes(1);
        expect(component.filter.emit).toHaveBeenCalledWith({
            freeText: null,
            departments: ['foo'],
            offices: null,
            committeeTypes: null,
            isMarketOrientated: null,
            hasSupervisionDuty: null,
            status: null,
            vacancies: null,
            statusProposal: null,
        });

        expect(searchStorageServiceMock.setParams).toHaveBeenCalledTimes(1);
        expect(searchStorageServiceMock.setParams).toHaveBeenCalledWith('general-election-committee-search', {
            freeText: null,
            departments: ['foo'],
            offices: null,
            committeeTypes: null,
            isMarketOrientated: null,
            hasSupervisionDuty: null,
            status: null,
            vacancies: null,
            statusProposal: null,
        });
    }));

    it('should setup filter form', () => {
        const filterForm = component.form;
        expect(filterForm.controls.freeText.value).toBeNull();
        expect(filterForm.controls.departments.value).toBeNull();
        expect(filterForm.controls.offices.value).toBeNull();
        expect(filterForm.controls.committeeTypes.value).toBeNull();
        expect(filterForm.controls.isMarketOrientated.value).toBeNull();
        expect(filterForm.controls.hasSupervisionDuty.value).toBeNull();
        expect(searchStorageServiceMock.getParams).toHaveBeenCalledTimes(1);
    });

    it('should apply filter after reset', fakeAsync(() => {
        jest.spyOn(component.filter, 'emit');

        component.reset();
        tick(500);

        expect(component.filter.emit).toHaveBeenCalledTimes(1);
        expect(searchStorageServiceMock.removeParams).toHaveBeenCalledTimes(1);
    }));

    it('should refresh filter for field freeText', fakeAsync(() => {
        jest.spyOn(component.filter, 'emit');
        component.form.controls.freeText.setValue('freeText');

        tick(500);

        expect(component.filter.emit).toHaveBeenCalledTimes(1);
    }));

    describe('selecting departments', () => {
        beforeEach(() => {
            masterDataServiceMock.offices!.set([
                {id: '1', departmentId: '1'},
                {id: '2', departmentId: '1'},
                {id: '3', departmentId: '2'},
                {id: '4', departmentId: '3'},
            ] as Office[]);
        });

        it('should set department offices by selected department', () => {
            component.form.controls.departments.setValue(['1', '3']);

            expect(component.departmentOffices()).toEqual([
                {id: '1', departmentId: '1'},
                {id: '2', departmentId: '1'},
                {id: '4', departmentId: '3'},
            ]);
        });

        it('should return all offices when no departments selected', () => {
            component.form.controls.departments.setValue(null);

            expect(component.departmentOffices()).toEqual([
                {id: '1', departmentId: '1'},
                {id: '2', departmentId: '1'},
                {id: '3', departmentId: '2'},
                {id: '4', departmentId: '3'},
            ]);
        });

        it('should filter offices to user department for department users', async () => {
            masterDataServiceMock.offices!.set([
                {id: '1', departmentId: '1'},
                {id: '2', departmentId: '1'},
                {id: '3', departmentId: '2'},
                {id: '4', departmentId: '3'},
            ] as Office[]);

            isDepartmentUserSubject.next(true);
            currentAssignmentSubject.next({id: 'a', text: 'b', departmentId: '2'});

            expect(component.departmentOffices()).toEqual([{id: '3', departmentId: '2'}]);
        });
    });
});
