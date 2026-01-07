import {provideHttpClient} from '@angular/common/http';
import {provideHttpClientTesting} from '@angular/common/http/testing';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {UntypedFormControl, UntypedFormGroup} from '@angular/forms';
import {MatAutocompleteModule, MatAutocompleteSelectedEvent, MatOption} from '@angular/material/autocomplete';
import {MatDatepickerModule, MatDatepicker, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatIcon} from '@angular/material/icon';
import {MatFormField, MatSelect} from '@angular/material/select';
import {TranslatePipe} from '@ngx-translate/core';
import {ErrorService} from '@shared/error-service.service';
import {MasterDataService} from '@shared/master-data.service';
import {MockComponents, MockModule, MockPipe} from 'ng-mocks';
import {of} from 'rxjs';
import {PersonInterestsService} from '../person-interests.service';
import {InterestsEditFormDetailComponent} from './interests-edit-form-detail.component';

describe('InterestsEditFormDetailComponent', () => {
    let component: InterestsEditFormDetailComponent;
    let fixture: ComponentFixture<InterestsEditFormDetailComponent>;

    const personInterestsServiceMock = {
        getUidOrganizations: jest.fn(),
    };

    const errorServiceMock = {
        getControlError: jest.fn(),
    };

    const masterDataServiceMock = {
        interestCommittees: jest.fn(),
        interestFunctions: jest.fn(),
        interestLegalForms: jest.fn(),
        legalForms: jest.fn(),
    } as unknown as Partial<MasterDataService>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                MockPipe(TranslatePipe),
                InterestsEditFormDetailComponent,
                MockModule(MatDatepickerModule),
                MockModule(MatAutocompleteModule),
                MockComponents(MatFormField, MatIcon, MatDatepicker, MatDatepickerToggle, MatOption, MatSelect),
            ],
            providers: [
                {provide: PersonInterestsService, useValue: personInterestsServiceMock},
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: ErrorService, useValue: errorServiceMock},
                provideHttpClient(),
                provideHttpClientTesting(),
            ],
            schemas: [NO_ERRORS_SCHEMA],
        }).compileComponents();

        fixture = TestBed.createComponent(InterestsEditFormDetailComponent);
        component = fixture.componentInstance;
        component.formGroup = new UntypedFormGroup({
            id: new UntypedFormControl(null),
            personId: new UntypedFormControl(''),
            text: new UntypedFormControl(''),
            interestText: new UntypedFormControl(''),
            interestCommitteeId: new UntypedFormControl(null),
            interestFunctionId: new UntypedFormControl(null),
            interestLegalFormId: new UntypedFormControl(null),
            beginDate: new UntypedFormControl(null),
            endDate: new UntypedFormControl(null),
            legalFormId: new UntypedFormControl(''),
            uidOrganisationId: new UntypedFormControl(''),
            rowVersion: new UntypedFormControl(0),
            isInactiveRow: new UntypedFormControl(null),
            isUidRow: new UntypedFormControl(null),
        });
    });

    describe('ngAfterViewInit', () => {
        it('should not call getUidOrganizations if input text is shorter than min length', fakeAsync(() => {
            personInterestsServiceMock.getUidOrganizations.mockClear();

            component.formGroup.controls.text.setValue('ab');
            fixture.detectChanges();
            tick(500);

            expect(personInterestsServiceMock.getUidOrganizations).not.toHaveBeenCalled();
        }));

        it('should call getUidOrganizations and set cleanOrganizations if input text meets min length', fakeAsync(() => {
            const dummyResponse = [
                {
                    organizationName: 'Org1',
                    uidOrganisationId: '123',
                    city: 'City1',
                    zip: '0001',
                    legalFormText: 'LF1',
                    legalFormId: 'L1',
                },
            ];
            personInterestsServiceMock.getUidOrganizations.mockReturnValue(of(dummyResponse));

            fixture.detectChanges();
            component.formGroup.controls.interestText.setValue('Org');
            tick(500);

            expect(personInterestsServiceMock.getUidOrganizations).toHaveBeenCalledWith('Org');
            expect(component.cleanOrganizations()).toEqual([
                {
                    organizationName: 'Org1',
                    uidOrganisationId: '123',
                    city: 'City1',
                    zip: '0001',
                    legalFormText: 'LF1',
                    id: '123;Org1;L1',
                },
            ]);
        }));
    });

    describe('setOrganization', () => {
        it('should update formGroup controls based on selected organization', () => {
            const selectedKey = '456;TestOrg;T1';
            const fakeEvent = {
                option: {value: selectedKey},
            } as MatAutocompleteSelectedEvent;

            component.setOrganization(fakeEvent);
            expect(component.formGroup.controls.uidOrganisationId.value).toBe('456');
            expect(component.formGroup.controls.interestText.value).toBe('TestOrg');
            expect(component.formGroup.controls.legalFormId.value).toBe('T1');
        });
    });

    describe('remove', () => {
        it('should emit when remove is called', () => {
            jest.spyOn(component.interestRemoved, 'emit');
            component.remove();
            expect(component.interestRemoved.emit).toHaveBeenCalledTimes(1);
        });
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should mark interestText as touched', () => {
        fixture.detectChanges();
        const control = component.formGroup.controls.interestText;
        expect(control.touched).toBe(true);
    });
});
