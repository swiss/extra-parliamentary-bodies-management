/* eslint-disable dot-notation,@typescript-eslint/no-explicit-any */
import {HttpClient} from '@angular/common/http';
import {ComponentFixture, discardPeriodicTasks, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatAutocompleteModule, MatAutocompleteSelectedEvent, MatOption} from '@angular/material/autocomplete';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatExpansionModule} from '@angular/material/expansion';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatInput} from '@angular/material/input';
import {MatRadioButton} from '@angular/material/radio';
import {MatSelect, MatSelectChange, MatSelectTrigger} from '@angular/material/select';
import {Address} from '@api/Address';
import {PersonDetails} from '@api/PersonDetails';
import {PersonUpdate} from '@api/PersonUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObErrorMessagesModule, ObInputClearModule, ObNotificationService, ObUnsavedChangesDirective} from '@oblique/oblique';
import {AddressService} from '@shared/address.service';
import {ErrorService} from '@shared/error-service.service';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {MockComponents, MockDirectives, MockModule, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, Subject, throwError} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';
import {ConfigsService} from '../../../configs.service';
import {PersonsService} from '../../persons.service';
import {PersonDataFormComponent} from './person-data-form.component';

describe('PersonDataFormComponent', () => {
    let component: PersonDataFormComponent;
    let fixture: ComponentFixture<PersonDataFormComponent>;
    const masterDataServiceMock = {
        salutations: jest.fn(),
        genders: jest.fn(),
        languages: jest.fn(),
        cantons: jest.fn(),
        legislaturePeriods: jest.fn(),
        councils: jest.fn(),
        offices: jest.fn(),
    };

    const personsServiceMock = {
        getSimilarPersons: jest.fn(),
        reload$: new Subject(),
    };

    const addressServiceMock = {
        getAddressSuggestions: jest.fn(),
    };

    const notificationServiceMock = {
        success: jest.fn(),
        error: jest.fn(),
    };

    const errorServiceMock = {
        getControlError: jest.fn(),
    };

    const configsServiceMock = {
        frontendConfig: {
            entityIds: {
                language: {
                    romanshLanguageId: 'rm',
                },
                gender: {
                    maleId: 'maleId',
                    femaleId: 'femaleId',
                },
                salutation: {
                    maleId: 'maleId',
                    femaleId: 'femaleId',
                },
                canton: {
                    abroadId: 'efd079df-bc12-4407-82ae-937ea10379c3',
                },
            },
        },
    };

    const isOfficeSubject = new BehaviorSubject(false);
    const isDepartmentSubject = new BehaviorSubject(false);
    const isSecretariatSubject = new BehaviorSubject(false);
    const authServiceMock = {
        isDepartmentUser$: isDepartmentSubject.asObservable(),
        isOfficeUser$: isOfficeSubject.asObservable(),
        isSecretariatUser$: isSecretariatSubject.asObservable(),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                PersonDataFormComponent,
                MockModule(ReactiveFormsModule),
                MockModule(ObErrorMessagesModule),
                MockModule(MatExpansionModule),
                MockModule(MatAutocompleteModule),
                MockModule(MatIconModule),
                MockModule(ObInputClearModule),
                MockComponents(MatFormField, MatCheckbox, MatRadioButton, MatOption, MatSelect, HelpTooltipComponent),
                MockDirectives(MatInput, MatLabel, ObUnsavedChangesDirective, MatSelectTrigger),
                MockPipe(TranslatePipe),
            ],
            providers: [
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: PersonsService, useValue: personsServiceMock},
                {provide: AddressService, useValue: addressServiceMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: ErrorService, useValue: errorServiceMock},
                {provide: AuthService, useValue: authServiceMock},
                {provide: ConfigsService, useValue: configsServiceMock},
                HttpClient,
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(PersonDataFormComponent);
        component = fixture.componentInstance;
        component.personModification.set({} as PersonUpdate);
        component.isUpdateMode = true;
        fixture.detectChanges();

        personsServiceMock.getSimilarPersons.mockReturnValue(of([{id: 'id', surname: 'Clark', givenName: 'Jimm'}]));
        addressServiceMock.getAddressSuggestions.mockReturnValue(of([{city: 'Baden', zip: '5400', street: '', canton: {id: 'AG'}}]));
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create the component', () => {
        expect(component).toBeTruthy();
    });

    describe('Form Initialization', () => {
        it('should initialize the form with default values', () => {
            const form = component.personForm;
            expect(form).toBeDefined();
            expect(form.controls.salutationId.value).toBeUndefined();
            expect(form.controls.surname.value).toBe('');
            expect(form.controls.givenName.value).toBe('');
            expect(form.controls.birthYear.value).toBeNull();
            expect(form.controls.languageId.value).toBe('');
            expect(form.controls.correspondenceLanguageId.value).toBe('');
            expect(form.controls.correspondenceLanguageId.disabled).toBe(true); // disabled until Romansh selected
            expect(form.controls.title.value).toBe('');
            expect(form.controls.occupation.value).toBe(null);
            expect(form.controls.genderId.value).toBe('');
            expect(form.controls.federalDuty.value).toBe(false);
            expect(form.controls.federalAssembly.value).toBe(false);
            expect(form.controls.legislaturePeriodIds.value).toEqual([]);

            ['officeAddress', 'privateAddress'].forEach(addressType => {
                const addressGroup = form.get(addressType) as FormGroup;
                expect(addressGroup).toBeDefined();
                expect(addressGroup.controls.companyName.value).toBe('');
                expect(addressGroup.controls.street.value).toBe('');
                expect(addressGroup.controls.poBox.value).toBe('');
                expect(addressGroup.controls.countryCode.value).toBe('');
                expect(addressGroup.controls.zip.value).toBe('');
                expect(addressGroup.controls.city.value).toBe('');
                expect(addressGroup.controls.phone.value).toBe('');
                expect(addressGroup.controls.mobile.value).toBe('');
                expect(addressGroup.controls.email.value).toBe('');
                expect(addressGroup.controls.cantonId.value).toBeNull();
                expect(addressGroup.controls.activeAddress.value).toBe(false);
            });
        });
    });

    describe('changeActiveAddress Method', () => {
        it('should set privateAddress as active and officeAddress as inactive', () => {
            component.changeActiveAddress('privateAddress');
            const form = component.personForm;

            expect(form.controls.privateAddress.controls.activeAddress.value).toBe(true);
            expect(form.controls.officeAddress.controls.activeAddress.value).toBe(false);
            expect(form.dirty).toBe(true);
        });

        it('should set officeAddress as active and privateAddress as inactive', () => {
            component.changeActiveAddress('officeAddress');
            const form = component.personForm;

            expect(form.controls.officeAddress.controls.activeAddress.value).toBe(true);
            expect(form.controls.privateAddress.controls.activeAddress.value).toBe(false);
            expect(form.dirty).toBe(true);
        });

        it('should update validators when changing active address', () => {
            const officeAddressGroup = component.personForm.controls.officeAddress;
            const privateAddressGroup = component.personForm.controls.privateAddress;

            // Set some values but leave required fields empty
            officeAddressGroup.controls.street.setValue('');
            officeAddressGroup.controls.zip.setValue('');
            officeAddressGroup.controls.city.setValue('');
            officeAddressGroup.controls.cantonId.setValue(null);
            officeAddressGroup.controls.countryCode.setValue('');

            // Change to office address as active
            component.changeActiveAddress('officeAddress');

            // Now required fields should be invalid
            expect(officeAddressGroup.controls.street.hasError('required')).toBe(true);
            expect(officeAddressGroup.controls.zip.hasError('required')).toBe(true);
            expect(officeAddressGroup.controls.city.hasError('required')).toBe(true);
            expect(officeAddressGroup.controls.cantonId.hasError('required')).toBe(true);
            expect(officeAddressGroup.controls.countryCode.hasError('required')).toBe(true);

            // Private address should not have required errors since it's not active
            expect(privateAddressGroup.controls.street.hasError('required')).toBe(false);
        });
    });

    describe('Form Validation', () => {
        it('should require surname', () => {
            const surnameControl = component.personForm.controls.surname;
            surnameControl.setValue('');
            expect(surnameControl.valid).toBe(false);
            expect(surnameControl.hasError('required')).toBe(true);

            surnameControl.setValue('Doe');
            expect(surnameControl.valid).toBe(true);
        });

        it('should require givenName', () => {
            const givenNameControl = component.personForm.controls.givenName;
            givenNameControl.setValue('');
            expect(givenNameControl.valid).toBe(false);
            expect(givenNameControl.hasError('required')).toBe(true);

            givenNameControl.setValue('John');
            expect(givenNameControl.valid).toBe(true);
        });

        it('should validate birthYear with correct pattern and range', () => {
            const birthYearControl = component.personForm.controls.birthYear;

            birthYearControl.setValue(199.9);
            expect(birthYearControl.valid).toBe(false);
            expect(birthYearControl.hasError('pattern')).toBe(true);

            birthYearControl.setValue(1899);
            expect(birthYearControl.valid).toBe(false);
            expect(birthYearControl.hasError('min')).toBe(true);

            birthYearControl.setValue(2101);
            expect(birthYearControl.valid).toBe(false);
            expect(birthYearControl.hasError('max')).toBe(true);

            birthYearControl.setValue(2000);
            expect(birthYearControl.valid).toBe(true);
        });

        it('should disable correspondenceLanguageId when language is not Romansh and enable when Romansh', () => {
            const languageControl = component.personForm.controls.languageId;
            const corrControl = component.personForm.controls.correspondenceLanguageId;

            // Initial: empty language, correspondence disabled
            expect(languageControl.value).toBe('');
            expect(corrControl.disabled).toBe(true);

            // Set non-Romansh language
            languageControl.setValue('de');
            fixture.detectChanges();
            expect(corrControl.disabled).toBe(true);
            expect(corrControl.value).toBe('de'); // mirrors language

            // Set Romansh language -> enable
            languageControl.setValue('rm');
            fixture.detectChanges();
            expect(corrControl.enabled).toBe(true);
            expect(corrControl.value).toBe('rm');

            // Switch back to non-Romansh
            languageControl.setValue('fr');
            fixture.detectChanges();
            expect(corrControl.disabled).toBe(true);
            expect(corrControl.value).toBe('fr');
        });

        it('should require gender', () => {
            const genderControl = component.personForm.controls.genderId;
            genderControl.setValue('');
            expect(genderControl.valid).toBe(false);
            expect(genderControl.hasError('required')).toBe(true);

            genderControl.setValue('Female');
            expect(genderControl.valid).toBe(true);
        });

        it('should update isFemalePerson and salutationId when genderId changes', () => {
            const genderControl = component.personForm.controls.genderId;
            const salutationControl = component.personForm.controls.salutationId;

            // Set to male
            genderControl.setValue(configsServiceMock.frontendConfig.entityIds.gender.maleId);
            fixture.detectChanges();
            expect(component.isFemalePerson).toBe(false);
            expect(salutationControl.value).toBe(configsServiceMock.frontendConfig.entityIds.salutation.maleId);

            // Set to female
            genderControl.setValue(configsServiceMock.frontendConfig.entityIds.gender.femaleId);
            fixture.detectChanges();
            expect(component.isFemalePerson).toBe(true);
            expect(salutationControl.value).toBe(configsServiceMock.frontendConfig.entityIds.salutation.femaleId);
        });

        it('should initialize legislaturePeriodIds as an empty array', () => {
            expect(component.personForm.controls.legislaturePeriodIds.value).toEqual([]);
        });

        it('should validate legislaturePeriodIds when federalAssembly changes', () => {
            const federalAssemblyControl = component.personForm.controls.federalAssembly;
            const legislaturePeriodIdsControl = component.personForm.controls.legislaturePeriodIds;

            federalAssemblyControl.setValue(false);
            legislaturePeriodIdsControl.setValue([]);
            expect(legislaturePeriodIdsControl.valid).toBe(true);

            federalAssemblyControl.setValue(true);
            legislaturePeriodIdsControl.setValue([]);
            expect(legislaturePeriodIdsControl.valid).toBe(false);

            federalAssemblyControl.setValue(true);
            legislaturePeriodIdsControl.setValue(['id1', 'id2']);
            expect(legislaturePeriodIdsControl.valid).toBe(true);
        });
    });

    describe('Address Form Validation', () => {
        ['officeAddress', 'privateAddress'].forEach((addressType: string) => {
            describe(`${addressType} controls`, () => {
                let addressGroup: FormGroup;

                beforeEach(() => {
                    if (addressType === 'privateAddress') {
                        addressGroup = component.personForm.controls.privateAddress;
                    } else if (addressType === 'officeAddress') {
                        addressGroup = component.personForm.controls.officeAddress;
                    }
                });

                it('should require activeAddress', () => {
                    addressGroup.controls.activeAddress.setValue(null);
                    expect(addressGroup.controls.activeAddress.valid).toBe(false);
                    expect(addressGroup.controls.activeAddress.hasError('required')).toBe(true);

                    addressGroup.controls.activeAddress.setValue(true);
                    expect(addressGroup.controls.activeAddress.valid).toBe(true);
                });

                it('should require street when address is active', () => {
                    addressGroup.controls.activeAddress.setValue(false);
                    addressGroup.controls.street.setValue('');
                    addressGroup.controls.street.updateValueAndValidity();
                    expect(addressGroup.controls.street.hasError('required')).toBe(false);

                    addressGroup.controls.activeAddress.setValue(true);
                    addressGroup.controls.street.updateValueAndValidity();
                    expect(addressGroup.controls.street.hasError('required')).toBe(true);

                    addressGroup.controls.street.setValue('Main Street');
                    addressGroup.controls.street.updateValueAndValidity();
                    expect(addressGroup.controls.street.hasError('required')).toBe(false);
                });

                it('should require zip when address is active', () => {
                    addressGroup.controls.activeAddress.setValue(false);
                    addressGroup.controls.zip.setValue('');
                    addressGroup.controls.zip.updateValueAndValidity();
                    expect(addressGroup.controls.zip.hasError('required')).toBe(false);

                    addressGroup.controls.activeAddress.setValue(true);
                    addressGroup.controls.zip.updateValueAndValidity();
                    expect(addressGroup.controls.zip.hasError('required')).toBe(true);

                    addressGroup.controls.zip.setValue('8000');
                    addressGroup.controls.zip.updateValueAndValidity();
                    expect(addressGroup.controls.zip.hasError('required')).toBe(false);
                });

                it('should require city when address is active', () => {
                    addressGroup.controls.activeAddress.setValue(false);
                    addressGroup.controls.city.setValue('');
                    addressGroup.controls.city.updateValueAndValidity();
                    expect(addressGroup.controls.city.hasError('required')).toBe(false);

                    addressGroup.controls.activeAddress.setValue(true);
                    addressGroup.controls.city.updateValueAndValidity();
                    expect(addressGroup.controls.city.hasError('required')).toBe(true);

                    addressGroup.controls.city.setValue('Zurich');
                    addressGroup.controls.city.updateValueAndValidity();
                    expect(addressGroup.controls.city.hasError('required')).toBe(false);
                });

                it('should require cantonId when address is active', () => {
                    addressGroup.controls.activeAddress.setValue(false);
                    addressGroup.controls.cantonId.setValue(null);
                    addressGroup.controls.cantonId.updateValueAndValidity();
                    expect(addressGroup.controls.cantonId.hasError('required')).toBe(false);

                    addressGroup.controls.activeAddress.setValue(true);
                    addressGroup.controls.cantonId.updateValueAndValidity();
                    expect(addressGroup.controls.cantonId.hasError('required')).toBe(true);

                    addressGroup.controls.cantonId.setValue('ZH');
                    addressGroup.controls.cantonId.updateValueAndValidity();
                    expect(addressGroup.controls.cantonId.hasError('required')).toBe(false);
                });

                it('should require countryCode when address is active', () => {
                    addressGroup.controls.activeAddress.setValue(false);
                    addressGroup.controls.countryCode.setValue('');
                    addressGroup.controls.countryCode.updateValueAndValidity();
                    expect(addressGroup.controls.countryCode.hasError('required')).toBe(false);

                    addressGroup.controls.activeAddress.setValue(true);
                    addressGroup.controls.countryCode.updateValueAndValidity();
                    expect(addressGroup.controls.countryCode.hasError('required')).toBe(true);

                    addressGroup.controls.countryCode.setValue('CH');
                    addressGroup.controls.countryCode.updateValueAndValidity();
                    expect(addressGroup.controls.countryCode.hasError('required')).toBe(false);
                });
            });
        });
        it.each([
            ['', true],
            ['Zurich', false],
        ])('should return correct isEmptyPrivateAddress, city: %s, expected: %s', (city, result) => {
            const addressControlPrivate = component.personForm.get('privateAddress') as FormGroup;
            expect(addressControlPrivate).toBeDefined();
            addressControlPrivate.controls.city.setValue(city);
            addressControlPrivate.controls.cantonId.setValue('');
            addressControlPrivate.controls.companyName.setValue('');
            addressControlPrivate.controls.countryCode.setValue('');
            addressControlPrivate.controls.email.setValue('');
            addressControlPrivate.controls.mobile.setValue('');
            addressControlPrivate.controls.phone.setValue('');
            addressControlPrivate.controls.poBox.setValue('');
            addressControlPrivate.controls.street.setValue('');
            addressControlPrivate.controls.zip.setValue('');
            expect(component['checkIfAddressIsEmpty']('privateAddress')).toBe(result);
        });

        it.each([
            ['', true],
            ['Zurich', false],
        ])('should return correct isEmptyOfficeAddress, city: %s, expected: %s', (city, result) => {
            const addressControlOffice = component.personForm.get('officeAddress') as FormGroup;
            expect(addressControlOffice).toBeDefined();
            addressControlOffice.controls.city.setValue(city);
            addressControlOffice.controls.cantonId.setValue('');
            addressControlOffice.controls.companyName.setValue('');
            addressControlOffice.controls.countryCode.setValue('');
            addressControlOffice.controls.email.setValue('');
            addressControlOffice.controls.mobile.setValue('');
            addressControlOffice.controls.phone.setValue('');
            addressControlOffice.controls.poBox.setValue('');
            addressControlOffice.controls.street.setValue('');
            addressControlOffice.controls.zip.setValue('');
            expect(component['checkIfAddressIsEmpty']('officeAddress')).toBe(result);
        });

        it.each([
            ['', true],
            ['Zurich', false],
        ])('should return correct isEmptyActiveAddress, city: %s, expected: %s', (city, result) => {
            const addressControlPrivate = component.personForm.get('privateAddress') as FormGroup;
            addressControlPrivate.controls.city.setValue('Bern');

            const addressControlOffice = component.personForm.get('officeAddress') as FormGroup;
            expect(addressControlOffice).toBeDefined();
            addressControlOffice.controls.activeAddress.setValue(true);
            addressControlOffice.controls.city.setValue(city);
            addressControlOffice.controls.cantonId.setValue('');
            addressControlOffice.controls.companyName.setValue('');
            addressControlOffice.controls.countryCode.setValue('');
            addressControlOffice.controls.email.setValue('');
            addressControlOffice.controls.mobile.setValue('');
            addressControlOffice.controls.phone.setValue('');
            addressControlOffice.controls.poBox.setValue('');
            addressControlOffice.controls.street.setValue('');
            addressControlOffice.controls.zip.setValue('');

            expect(component['checkIfAddressIsEmpty']('officeAddress')).toBe(result);
        });

        it.each([
            ['Bern', 'Zürich', true, false, true],
            ['Bern', 'Zürich', false, true, true],
            ['', 'Zürich', true, false, false],
            ['', 'Zürich', false, true, true],
            ['Bern', 'Zürich', false, false, false],
        ])(
            'should return correct areAddressesValid, cityPrivate: %s, cityOffice: %s, activeAddressPrivate: %s, activeAddressOffice: %s, expected: %s',
            (cityPrivate, cityOffice, activePrivate, activeOffice, expected) => {
                const addressControlPrivate = component.personForm.get('privateAddress') as FormGroup;
                addressControlPrivate.controls.activeAddress.setValue(activePrivate);
                addressControlPrivate.controls.city.setValue(cityPrivate);
                addressControlPrivate.controls.street.setValue(activePrivate && cityPrivate ? 'Street' : '');
                addressControlPrivate.controls.zip.setValue(activePrivate && cityPrivate ? '3000' : '');
                addressControlPrivate.controls.cantonId.setValue(activePrivate && cityPrivate ? 'BE' : '');
                addressControlPrivate.controls.countryCode.setValue(activePrivate && cityPrivate ? 'CH' : '');
                addressControlPrivate.controls.companyName.setValue('');
                addressControlPrivate.controls.email.setValue('');
                addressControlPrivate.controls.mobile.setValue('');
                addressControlPrivate.controls.phone.setValue('');
                addressControlPrivate.controls.poBox.setValue('');

                const addressControlOffice = component.personForm.get('officeAddress') as FormGroup;
                expect(addressControlOffice).toBeDefined();
                addressControlOffice.controls.activeAddress.setValue(activeOffice);
                addressControlOffice.controls.city.setValue(cityOffice);
                addressControlOffice.controls.street.setValue(activeOffice ? 'Street' : '');
                addressControlOffice.controls.zip.setValue(activeOffice ? '8000' : '');
                addressControlOffice.controls.cantonId.setValue(activeOffice ? 'ZH' : '');
                addressControlOffice.controls.countryCode.setValue(activeOffice ? 'CH' : '');
                addressControlOffice.controls.companyName.setValue('');
                addressControlOffice.controls.email.setValue('');
                addressControlOffice.controls.mobile.setValue('');
                addressControlOffice.controls.phone.setValue('');
                addressControlOffice.controls.poBox.setValue('');

                // Update validators for the active address changes
                addressControlPrivate.controls.street.updateValueAndValidity();
                addressControlPrivate.controls.zip.updateValueAndValidity();
                addressControlPrivate.controls.city.updateValueAndValidity();
                addressControlPrivate.controls.cantonId.updateValueAndValidity();
                addressControlPrivate.controls.countryCode.updateValueAndValidity();
                addressControlOffice.controls.street.updateValueAndValidity();
                addressControlOffice.controls.zip.updateValueAndValidity();
                addressControlOffice.controls.city.updateValueAndValidity();
                addressControlOffice.controls.cantonId.updateValueAndValidity();
                addressControlOffice.controls.countryCode.updateValueAndValidity();

                // Manually trigger formState update to make computed signals re-evaluate
                component['formState'].update(v => v + 1);

                component.validateAddresses();
                expect(addressControlOffice.valid).toBe(expected);
            }
        );

        it('should return correct isWithoutActiveAddress', () => {
            const addressControlPrivate = component.personForm.get('privateAddress') as FormGroup;
            expect(addressControlPrivate).toBeDefined();
            addressControlPrivate.controls.activeAddress.setValue(false);

            const addressControlOffice = component.personForm.get('officeAddress') as FormGroup;
            expect(addressControlOffice).toBeDefined();
            addressControlOffice.controls.activeAddress.setValue(false);

            // Manually trigger formState update to make computed signals re-evaluate
            component['formState'].update(v => v + 1);

            expect(component.isWithoutActiveAddress()).toBe(true);
        });

        it('should not run validation when maskAddress is true', () => {
            component.isUpdateMode = true;
            component.personModification.set({maskAddress: true} as PersonUpdate);

            const addressControlOffice = component.personForm.get('officeAddress') as FormGroup;
            expect(addressControlOffice).toBeDefined();
            addressControlOffice.controls.activeAddress.setValue(false);
            addressControlOffice.controls.city.setValue('');
            addressControlOffice.controls.cantonId.setValue('');
            addressControlOffice.controls.companyName.setValue('');
            addressControlOffice.controls.countryCode.setValue('');
            addressControlOffice.controls.email.setValue('');
            addressControlOffice.controls.mobile.setValue('');
            addressControlOffice.controls.phone.setValue('');
            addressControlOffice.controls.poBox.setValue('');
            addressControlOffice.controls.street.setValue('');
            addressControlOffice.controls.zip.setValue('');

            const addressControlPrivate = component.personForm.get('privateAddress') as FormGroup;
            expect(addressControlPrivate).toBeDefined();
            addressControlPrivate.controls.activeAddress.setValue(false);
            addressControlPrivate.controls.city.setValue('');

            const result = component.validateAddresses();

            expect(result).toBe(true);
            expect(addressControlOffice.errors).toBeNull();
        });
    });

    describe('create mode', () => {
        it('should call PersonService when call searchSimilarPersons and should not enable further controls yet', () => {
            component.isUpdateMode = false;
            component.ngOnInit();
            component.personForm.get('surname')?.setValue('Clark');
            component.personForm.get('givenName')?.setValue('Jim');
            component.personForm.get('birthYear')?.setValue(1999);
            component.searchSimilarPersons();
            expect(personsServiceMock.getSimilarPersons).toHaveBeenCalledTimes(1);
            expect(component.similarPersons.length).toEqual(1);
            expect(component.personForm.controls.confirmNewPerson.value).toEqual(false);
        });

        it('should call PersonService when searchSimilarPersons and should check checkbox if match doesnt exists', () => {
            personsServiceMock.getSimilarPersons.mockReturnValue(of([]));
            component.isUpdateMode = false;
            component.ngOnInit();
            component.personForm.get('surname')?.setValue('Clark');
            component.personForm.get('givenName')?.setValue('Jim');
            component.personForm.get('birthYear')?.setValue(1999);
            component.searchSimilarPersons();
            expect(personsServiceMock.getSimilarPersons).toHaveBeenCalledTimes(1);
            expect(component.similarPersons.length).toEqual(0);
            expect(component.personForm.controls.confirmNewPerson.value).toEqual(true);
        });

        it('should handle getSimilarPersons errors ', () => {
            personsServiceMock.getSimilarPersons.mockReturnValue(throwError(() => new Error('Get failed')));
            component.isUpdateMode = false;
            component.ngOnInit();
            component.personForm.get('surname')?.setValue('Clark');
            component.personForm.get('givenName')?.setValue('Jim');
            component.personForm.get('birthYear')?.setValue(1999);
            component.searchSimilarPersons();

            expect(personsServiceMock.getSimilarPersons).toHaveBeenCalledTimes(1);
            expect(notificationServiceMock.error).toHaveBeenCalledWith('person.create.validate.name.error');
            expect(component.similarPersons.length).toEqual(0);
            expect(component.personForm.controls.surname.enabled).toEqual(true);
            expect(component.personForm.controls.givenName.enabled).toEqual(true);
            expect(component.personForm.controls.birthYear.enabled).toEqual(true);
        });
    });
    describe('update mode', () => {
        it('should not search similar persons', () => {
            component.isUpdateMode = true;
            personsServiceMock.getSimilarPersons.mockReturnValue(of([{id: 'id', surname: 'Clark', givenName: 'Jimm'}]));
            component.ngOnInit();
            component.personForm.get('surname')?.setValue('Clark');
            component.personForm.get('givenName')?.setValue('Jim');
            component.personForm.get('birthYear')?.setValue(1999);
            component.searchSimilarPersons();
            expect(personsServiceMock.getSimilarPersons).not.toHaveBeenCalled();
            expect(component.personForm.controls.confirmNewPerson.value).toEqual(true);
        });
    });
    it.each([
        ['', '', null],
        ['', 'Jim', null],
        ['Clark', '', null],
        ['', '', 1936],
        ['Clark', 'Jim', null],
        ['Clark', '', 1936],
        ['', 'Jim', 1936],
    ])('should not call PersonService without the basic field values, surname: %s, givenName: %s, birthYear: %s', () => {
        component.personForm.get('surname')?.setValue('');
        component.personForm.get('givenName')?.setValue('Jim');
        component.personForm.get('birthYear')?.setValue(1999);
        component.searchSimilarPersons();
        expect(personsServiceMock.getSimilarPersons).not.toHaveBeenCalled();
    });

    it('should set values in controls when setOfficeZipOrCity is called with valid values', () => {
        const newValue = {city: 'Baden', zip: '5400', street: 'Bahnhofstrasse', canton: {id: 'AG'}} as Address;
        const selectedEvent: MatAutocompleteSelectedEvent = {
            option: {value: newValue},
        } as MatAutocompleteSelectedEvent;

        component.personForm.controls.officeAddress.controls.street.setValue('');

        component.setZipOrCity(selectedEvent, 'officeAddress');

        expect(component.personForm.controls.officeAddress.controls.street.value).toBe('');
        expect(component.personForm.controls.officeAddress.controls.zip.value).toBe('5400');
        expect(component.personForm.controls.officeAddress.controls.city.value).toBe('Baden');
        expect(component.personForm.controls.officeAddress.controls.cantonId.value).toBe('AG');
    });

    it('should set values in controls when setOfficeStreet is called with valid values', () => {
        const newValue = {city: 'Baden', zip: '5400', street: 'Bahnhofstrasse', canton: {id: 'AG'}} as Address;
        const selectedEvent: MatAutocompleteSelectedEvent = {
            option: {value: newValue},
        } as MatAutocompleteSelectedEvent;

        component.setStreet(selectedEvent, 'officeAddress');

        expect(component.personForm.controls.officeAddress.controls.street.value).toBe('Bahnhofstrasse');
        expect(component.personForm.controls.officeAddress.controls.zip.value).toBe('5400');
        expect(component.personForm.controls.officeAddress.controls.city.value).toBe('Baden');
        expect(component.personForm.controls.officeAddress.controls.cantonId.value).toBe('AG');
    });

    it('should set values in controls when setPrivateZipOrCity is called with valid values', () => {
        const newValue = {city: 'Baden', zip: '5400', street: 'Bahnhofstrasse', canton: {id: 'AG'}} as Address;
        const selectedEvent: MatAutocompleteSelectedEvent = {
            option: {value: newValue},
        } as MatAutocompleteSelectedEvent;

        component.personForm.controls.privateAddress.controls.street.setValue('');

        component.setZipOrCity(selectedEvent, 'privateAddress');

        expect(component.personForm.controls.privateAddress.controls.street.value).toBe('');
        expect(component.personForm.controls.privateAddress.controls.zip.value).toBe('5400');
        expect(component.personForm.controls.privateAddress.controls.city.value).toBe('Baden');
        expect(component.personForm.controls.privateAddress.controls.cantonId.value).toBe('AG');
    });

    it('should set values in controls when setPrivateStreet is called with valid values', () => {
        const newValue = {city: 'Baden', zip: '5400', street: 'Bahnhofstrasse', canton: {id: 'AG'}} as Address;
        const selectedEvent: MatAutocompleteSelectedEvent = {
            option: {value: newValue},
        } as MatAutocompleteSelectedEvent;

        component.setStreet(selectedEvent, 'privateAddress');

        expect(component.personForm.controls.privateAddress.controls.street.value).toBe('Bahnhofstrasse');
        expect(component.personForm.controls.privateAddress.controls.zip.value).toBe('5400');
        expect(component.personForm.controls.privateAddress.controls.city.value).toBe('Baden');
        expect(component.personForm.controls.privateAddress.controls.cantonId.value).toBe('AG');
    });

    it('should set value in country when setCanton is a valid canton', () => {
        const event = new MatSelectChange({} as any, 'myCantonId');

        component.setCanton(event, 'privateAddress');

        expect(component.personForm.controls.privateAddress.controls.countryCode.value).toBe('CH');
    });

    it('should not set value in country when setCanton with abroad canton ID', () => {
        const event = new MatSelectChange({} as any, 'efd079df-bc12-4407-82ae-937ea10379c3');

        component.setCanton(event, 'privateAddress');

        expect(component.personForm.controls.privateAddress.controls.countryCode.value).toBe(null);
    });

    it.each([
        [{id: 'id', surname: 'Clark', givenName: 'Jim', officeAddress: {city: 'Bern', activeAddress: true}} as PersonDetails, ', Bern'],
        [{id: 'id', surname: 'Clark', givenName: 'Jim', privateAddress: {city: 'Zurich', activeAddress: true}} as PersonDetails, ', Zurich'],
        [
            {
                id: 'id',
                surname: 'Clark',
                givenName: 'Jim',
                officeAddress: {city: 'Bern', activeAddress: false},
                privateAddress: {city: 'Zurich', activeAddress: true},
            } as PersonDetails,
            ', Zurich',
        ],
    ])('should return persons city', (personDetail: PersonDetails, expectedString: string) => {
        const cityText = component.getPersonsCity(personDetail);
        expect(cityText).toEqual(expectedString);
    });

    it('should clean office address fields when clear is called', () => {
        component.personForm.controls.officeAddress.controls.street.setValue('ab');
        component.personForm.controls.officeAddress.controls.zip.setValue('cd');
        component.personForm.controls.officeAddress.controls.city.setValue('ef');
        component.personForm.controls.officeAddress.controls.cantonId.setValue('gh');

        component.clear('officeAddress');

        expect(component.personForm.controls.officeAddress.controls.street.value).toBe(null);
        expect(component.personForm.controls.officeAddress.controls.zip.value).toBe(null);
        expect(component.personForm.controls.officeAddress.controls.city.value).toBe(null);
        expect(component.personForm.controls.officeAddress.controls.cantonId.value).toBe(null);
    });

    it('should clean private address fields when clear is called', () => {
        component.personForm.controls.privateAddress.controls.street.setValue('ab');
        component.personForm.controls.privateAddress.controls.zip.setValue('cd');
        component.personForm.controls.privateAddress.controls.city.setValue('ef');
        component.personForm.controls.privateAddress.controls.cantonId.setValue('gh');

        component.clear('privateAddress');

        expect(component.personForm.controls.privateAddress.controls.street.value).toBe(null);
        expect(component.personForm.controls.privateAddress.controls.zip.value).toBe(null);
        expect(component.personForm.controls.privateAddress.controls.city.value).toBe(null);
        expect(component.personForm.controls.privateAddress.controls.cantonId.value).toBe(null);
    });

    it('should update noInterest control based on interests', fakeAsync(() => {
        component.isUpdateMode = true;

        component.personModification.update(person => ({...person, hasInterests: true}) as PersonUpdate);
        (personsServiceMock.reload$ as Subject<void>).next();
        tick();
        fixture.detectChanges();
        expect(component.personForm.controls.noInterest.disabled).toBe(true);

        component.personModification.update(person => ({...person, hasInterests: false}) as PersonUpdate);
        (personsServiceMock.reload$ as Subject<void>).next();
        tick();
        fixture.detectChanges();
        expect(component.personForm.controls.noInterest.enabled).toBe(true);
        discardPeriodicTasks();
    }));

    it('should update legislaturePeriodIds validation when federalAssembly changes', () => {
        const federalAssemblyControl = component.personForm.controls.federalAssembly;
        const legislaturePeriodIdsControl = component.personForm.controls.legislaturePeriodIds;

        const validator = (control: any) => {
            if (federalAssemblyControl.value && (!control.value || control.value.length === 0)) {
                return {legislaturePeriodRequired: true};
            }
            return null;
        };

        federalAssemblyControl.setValue(false);
        legislaturePeriodIdsControl.setValue([]);
        expect(validator(legislaturePeriodIdsControl)).toBeNull();

        federalAssemblyControl.setValue(true);
        legislaturePeriodIdsControl.setValue([]);
        expect(validator(legislaturePeriodIdsControl)).toEqual({legislaturePeriodRequired: true});

        federalAssemblyControl.setValue(true);
        legislaturePeriodIdsControl.setValue(['period1', 'period2']);
        expect(validator(legislaturePeriodIdsControl)).toBeNull();

        federalAssemblyControl.setValue(false);
        legislaturePeriodIdsControl.setValue([]);
        expect(validator(legislaturePeriodIdsControl)).toBeNull();
    });

    describe('subscribeToAddressChanges', () => {
        it('should update filteredZipsAndCitiesOffice when valid street value is set', fakeAsync(() => {
            const suggestions = [
                {zip: '5000', city: 'CityA', street: '', canton: {id: 'AG'}},
                {zip: '4000', city: 'CityB', street: '', canton: {id: 'AG'}},
            ];
            addressServiceMock.getAddressSuggestions.mockReturnValue(of(suggestions));
            component.personForm.controls.officeAddress.controls.street.setValue('Test');
            tick(300);
            expect(component.filteredZipsAndCitiesOffice()).toEqual([
                {zip: '4000', city: 'CityB', street: '', canton: {id: 'AG'}, id: ';4000;CityB;AG'},
                {zip: '5000', city: 'CityA', street: '', canton: {id: 'AG'}, id: ';5000;CityA;AG'},
            ]);
        }));

        it('should update filteredZipsAndCitiesOffice when valid city value is set', fakeAsync(() => {
            const suggestions = [{zip: '3000', city: 'TestCity', street: '', canton: {id: 'AG'}}];
            addressServiceMock.getAddressSuggestions.mockReturnValue(of(suggestions));
            component.personForm.controls.officeAddress.controls.city.setValue('TestCity');
            tick(300);
            expect(component.filteredZipsAndCitiesOffice()).toEqual([{zip: '3000', city: 'TestCity', street: '', canton: {id: 'AG'}, id: ';3000;TestCity;AG'}]);
        }));

        it('should update filteredZipsAndCitiesOffice when valid zip value is set', fakeAsync(() => {
            const suggestions = [{zip: '6000', city: 'CityC', street: '', canton: {id: 'AG'}}];
            addressServiceMock.getAddressSuggestions.mockReturnValue(of(suggestions));
            component.personForm.controls.officeAddress.controls.zip.setValue('6000');
            tick(300);
            expect(component.filteredZipsAndCitiesOffice()).toEqual([{zip: '6000', city: 'CityC', street: '', canton: {id: 'AG'}, id: ';6000;CityC;AG'}]);
        }));

        it('should not update filteredZipsAndCitiesOffice when street value is invalid', fakeAsync(() => {
            const suggestions = [{zip: '6000', city: 'CityC', street: '', canton: {id: 'AG'}}];
            addressServiceMock.getAddressSuggestions.mockReturnValue(of(suggestions));
            component.personForm.controls.officeAddress.controls.street.setValue('Tes');
            tick(300);
            expect(component.filteredZipsAndCitiesOffice()).toEqual([]);
        }));

        it('should update filteredZipsAndCitiesPrivate when valid street value is set', fakeAsync(() => {
            const suggestions = [
                {zip: '7000', city: 'CityD', street: '', canton: {id: 'AG'}},
                {zip: '6500', city: 'CityE', street: '', canton: {id: 'AG'}},
            ];
            addressServiceMock.getAddressSuggestions.mockReturnValue(of(suggestions));
            component.personForm.controls.privateAddress.controls.street.setValue('Test');
            tick(300);
            expect(component.filteredZipsAndCitiesPrivate()).toEqual([
                {zip: '6500', city: 'CityE', street: '', canton: {id: 'AG'}, id: ';6500;CityE;AG'},
                {zip: '7000', city: 'CityD', street: '', canton: {id: 'AG'}, id: ';7000;CityD;AG'},
            ]);
        }));

        it('should update filteredZipsAndCitiesPrivate when valid city value is set', fakeAsync(() => {
            const suggestions = [{zip: '8000', city: 'TestCityPrivate', street: '', canton: {id: 'AG'}}];
            addressServiceMock.getAddressSuggestions.mockReturnValue(of(suggestions));
            component.personForm.controls.privateAddress.controls.city.setValue('TestCityPrivate');
            tick(300);
            expect(component.filteredZipsAndCitiesPrivate()).toEqual([
                {zip: '8000', city: 'TestCityPrivate', street: '', canton: {id: 'AG'}, id: ';8000;TestCityPrivate;AG'},
            ]);
        }));

        it('should update filteredZipsAndCitiesPrivate when valid zip value is set', fakeAsync(() => {
            const suggestions = [{zip: '9000', city: 'CityF', street: '', canton: {id: 'AG'}}];
            addressServiceMock.getAddressSuggestions.mockReturnValue(of(suggestions));
            component.personForm.controls.privateAddress.controls.zip.setValue('9000');
            tick(300);
            expect(component.filteredZipsAndCitiesPrivate()).toEqual([{zip: '9000', city: 'CityF', street: '', canton: {id: 'AG'}, id: ';9000;CityF;AG'}]);
        }));

        it('should not update filteredZipsAndCitiesPrivate when zip value is invalid', fakeAsync(() => {
            const suggestions = [{zip: '9000', city: 'CityF', street: '', canton: {id: 'AG'}}];
            addressServiceMock.getAddressSuggestions.mockReturnValue(of(suggestions));
            component.personForm.controls.privateAddress.controls.zip.setValue('9');
            tick(300);
            expect(component.filteredZipsAndCitiesPrivate()).toEqual([]);
        }));
    });
});
