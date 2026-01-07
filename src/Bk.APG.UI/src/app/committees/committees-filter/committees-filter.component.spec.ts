import {signal} from '@angular/core';
import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {Router} from '@angular/router';
import {Office} from '@api/Office';
import {TranslateModule} from '@ngx-translate/core';
import {ObButtonModule, ObInputClearDirective} from '@oblique/oblique';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {SearchStorageService} from '@shared/services/search-storage.service';
import {MockComponents, MockDirective, MockModule} from 'ng-mocks';
import {BehaviorSubject} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {Role} from '../../auth/Role';
import {CommitteesFilterComponent} from './committees-filter.component';

describe('CommitteesFilterComponent', () => {
    let component: CommitteesFilterComponent;
    let fixture: ComponentFixture<CommitteesFilterComponent>;

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

    const roles: Role[] = [Role.Admin];
    const rolesSubject = new BehaviorSubject(roles);

    const authServiceMock = {
        roles$: rolesSubject.asObservable(),
    };

    const searchStorageServiceMock = {
        getParams: jest.fn(),
        setParams: jest.fn(),
        removeParams: jest.fn(),
    };

    beforeEach(async () => {
        component = await TestBed.configureTestingModule({
            imports: [
                CommitteesFilterComponent,
                ReactiveFormsModule,
                MockModule(TranslateModule),
                MockModule(MatFormFieldModule),
                MockModule(MatInputModule),
                MockModule(MatSelectModule),
                MockModule(MatButtonModule),
                MockModule(ObButtonModule),
                MockDirective(ObInputClearDirective),
                CommitteesFilterComponent,
                MockComponents(HelpTooltipComponent),
            ],
            providers: [
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: AuthService, useValue: authServiceMock},
                {provide: SearchStorageService, useValue: searchStorageServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(CommitteesFilterComponent);
        component = fixture.componentInstance;

        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should set canCreate', () => {
        expect(component.canCreate).toBe(true);
    });

    it('should filter', fakeAsync(() => {
        jest.spyOn(component.filter, 'emit');

        component.form.controls.levels.setValue(['foo']);
        tick();

        expect(component.filter.emit).toHaveBeenCalledTimes(1);
        expect(component.filter.emit).toHaveBeenCalledWith({
            freeText: null,
            levels: ['foo'],
            departments: null,
            offices: null,
            committeeTypes: null,
            terms: null,
            isActive: [true],
            isMarketOrientated: null,
            hasSupervisionDuty: null,
        });

        expect(searchStorageServiceMock.setParams).toHaveBeenCalledTimes(1);
        expect(searchStorageServiceMock.setParams).toHaveBeenCalledWith('committee-search', {
            freeText: null,
            levels: ['foo'],
            departments: null,
            offices: null,
            committeeTypes: null,
            terms: null,
            isActive: [true],
            isMarketOrientated: null,
            hasSupervisionDuty: null,
        });
    }));

    it('should setup filter form', () => {
        const filterForm = component.form;
        expect(filterForm.controls.freeText.value).toBeNull();
        expect(filterForm.controls.levels.value).toBeNull();
        expect(filterForm.controls.departments.value).toBeNull();
        expect(filterForm.controls.offices.value).toBeNull();
        expect(filterForm.controls.committeeTypes.value).toBeNull();
        expect(filterForm.controls.terms.value).toBeNull();
        expect(filterForm.controls.isActive.value).toStrictEqual([true]);
        expect(filterForm.controls.isMarketOrientated.value).toBeNull();
        expect(filterForm.controls.hasSupervisionDuty.value).toBeNull();
        expect(searchStorageServiceMock.getParams).toHaveBeenCalledTimes(1);
    });

    it('should apply filter after reset', fakeAsync(() => {
        jest.spyOn(component.filter, 'emit');

        component.reset();
        tick(500);

        expect(component.filter.emit).toHaveBeenCalledTimes(1);
        expect(component.form.controls.isActive).toBeTruthy();
        expect(searchStorageServiceMock.removeParams).toHaveBeenCalledTimes(1);
    }));

    it('should navigate to create committee', () => {
        component.create();

        expect(routerMock.navigate).toHaveBeenCalledWith(['committees/create']);
    });

    it('should refresh filter for field freeText', fakeAsync(() => {
        jest.spyOn(component.filter, 'emit');
        component.form.controls.freeText.setValue('freeText');

        tick(500);

        expect(component.filter.emit).toHaveBeenCalledTimes(1);
    }));

    it('should refresh filter for checkboxes', fakeAsync(() => {
        jest.spyOn(component.filter, 'emit');

        component.form.controls.isActive.setValue([true]);
        tick();

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
    });
});
