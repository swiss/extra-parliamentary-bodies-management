import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {FormBuilder, ReactiveFormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatIcon} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {Router} from '@angular/router';
import {TranslateModule} from '@ngx-translate/core';
import {ObButtonModule} from '@oblique/oblique';
import {MasterDataService} from '@shared/master-data.service';
import {SearchStorageService} from '@shared/services/search-storage.service';
import {MockComponent, MockModule} from 'ng-mocks';
import {BehaviorSubject} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {Role} from '../../auth/Role';
import {PersonsFilterComponent} from './persons-filter.component';

describe('PersonFilterComponent', () => {
    let component: PersonsFilterComponent;
    const formBuilder: FormBuilder = new FormBuilder();
    let fixture: ComponentFixture<PersonsFilterComponent>;

    const masterDataServiceMock = {
        languages: jest.fn(),
        cantons: jest.fn(),
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
                PersonsFilterComponent,
                ReactiveFormsModule,
                MockModule(TranslateModule),
                MockModule(MatFormFieldModule),
                MockModule(MatInputModule),
                MockModule(MatSelectModule),
                MockModule(MatButtonModule),
                MockModule(ObButtonModule),
                MockComponent(MatIcon),
                PersonsFilterComponent,
            ],
            providers: [
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: AuthService, useValue: authServiceMock},
                {provide: SearchStorageService, useValue: searchStorageServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(PersonsFilterComponent);
        component = fixture.componentInstance;

        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should be able to create person', () => {
        expect(component.canCreatePerson()).toBe(true);
    });

    it('should filter', () => {
        jest.spyOn(component.filter, 'emit');

        component.form = formBuilder.group({
            freeText: formBuilder.control<string | null>('test'),
            hasActiveMembership: formBuilder.control<boolean[] | null>([true]),
            cantons: formBuilder.control<string[] | null>(['canton1']),
            languages: formBuilder.control<string[] | null>(['lang1']),
        });
        component.applyFilter();

        expect(component.filter.emit).toHaveBeenCalledTimes(1);
        expect(component.filter.emit).toHaveBeenCalledWith({
            freeText: 'test',
            hasActiveMembership: [true],
            cantons: ['canton1'],
            languages: ['lang1'],
        });

        expect(searchStorageServiceMock.setParams).toHaveBeenCalledTimes(1);
        expect(searchStorageServiceMock.setParams).toHaveBeenCalledWith('person-search', {
            freeText: 'test',
            hasActiveMembership: [true],
            cantons: ['canton1'],
            languages: ['lang1'],
        });
    });

    it('should setup filter form', () => {
        const filterForm = component.setupPersonFilterForm();
        expect(filterForm.controls.freeText.value).toBeNull();
        expect(filterForm.controls.languages.value).toBeNull();
        expect(filterForm.controls.cantons.value).toBeNull();
        expect(filterForm.controls.hasActiveMembership.value).toBeNull();
        expect(searchStorageServiceMock.getParams).toHaveBeenCalledTimes(2);
    });

    it('should apply filter after reset', () => {
        jest.spyOn(component.filter, 'emit');

        component.reset();

        expect(component.filter.emit).toHaveBeenCalledTimes(3);
        expect(searchStorageServiceMock.removeParams).toHaveBeenCalledTimes(1);
    });

    it('should navigate to create person', () => {
        component.create();

        expect(routerMock.navigate).toHaveBeenCalledWith(['persons/create']);
    });

    it('should refresh filter', fakeAsync(() => {
        jest.spyOn(component.filter, 'emit');
        component.form.controls.freeText.setValue('freeText');
        tick(500);

        expect(component.filter.emit).toHaveBeenCalledTimes(1);
    }));

    it('should refresh filter for checkboxes', fakeAsync(() => {
        jest.spyOn(component.filter, 'emit');
        component.form.controls.hasActiveMembership.setValue([true]);
        component.form.controls.cantons.setValue(['TI']);
        component.form.controls.languages.setValue(['DE']);

        expect(component.filter.emit).toHaveBeenCalledTimes(3);
    }));
});
