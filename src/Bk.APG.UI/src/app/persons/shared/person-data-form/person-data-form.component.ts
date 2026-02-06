/* eslint-disable max-lines */
import {COMMA, ENTER} from '@angular/cdk/keycodes';
import {DatePipe, NgClass} from '@angular/common';
import {Component, computed, effect, ElementRef, Input, model, OnInit, signal, ViewChild} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatAutocomplete, MatAutocompleteSelectedEvent, MatAutocompleteTrigger} from '@angular/material/autocomplete';
import {MatCard, MatCardContent} from '@angular/material/card';
import {MatCheckbox, MatCheckboxChange} from '@angular/material/checkbox';
import {MatChipGrid, MatChipInput, MatChipRow} from '@angular/material/chips';
import {MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle} from '@angular/material/expansion';
import {MatIcon} from '@angular/material/icon';
import {MatError, MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatRadioButton} from '@angular/material/radio';
import {MatOption, MatSelect, MatSelectChange, MatSelectTrigger} from '@angular/material/select';
import {Address} from '@api/Address';
import {Occupation} from '@api/Occupation';
import {Office} from '@api/Office';
import {PersonCreate} from '@api/PersonCreate';
import {PersonDetails} from '@api/PersonDetails';
import {PersonUpdate} from '@api/PersonUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObAlertComponent, ObErrorMessagesDirective, ObInputClearDirective, ObNotificationService, ObUnsavedChangesDirective} from '@oblique/oblique';
import {AddressService} from '@shared/address.service';
import {ErrorService} from '@shared/error-service.service';
import {conditionalValidator} from '@shared/form-validators/conditional.validator';
import {TEL_PATTERN} from '@shared/form-validators/validation-patterns';
import {isEmptyId} from '@shared/id-util';
import {MasterDataService} from '@shared/master-data.service';
import {combineLatest, debounceTime, filter, map, merge, Subject, switchMap, takeUntil} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';
import {ConfigsService} from '../../../configs.service';
import {HelpTooltipComponent} from '../../../shared/help-tooltip/help-tooltip.component';
import {PersonsService} from '../../persons.service';

type AddressControlId = 'officeAddress' | 'privateAddress';

@Component({
    selector: 'apg-person-data-form',
    templateUrl: './person-data-form.component.html',
    styleUrl: './person-data-form.component.scss',
    imports: [
        ReactiveFormsModule,
        ObUnsavedChangesDirective,
        MatFormField,
        ObErrorMessagesDirective,
        MatLabel,
        MatInput,
        MatError,
        MatCard,
        MatCardContent,
        MatCheckbox,
        MatSelect,
        MatOption,
        MatSelectTrigger,
        MatExpansionPanel,
        MatExpansionPanelHeader,
        MatExpansionPanelTitle,
        MatAutocompleteTrigger,
        ObAlertComponent,
        ObInputClearDirective,
        MatAutocomplete,
        MatRadioButton,
        DatePipe,
        TranslatePipe,
        MatChipGrid,
        MatChipInput,
        MatChipRow,
        MatAutocomplete,
        MatIcon,
        FormsModule,
        NgClass,
        HelpTooltipComponent,
    ],
})
export class PersonDataFormComponent implements OnInit {
    @ViewChild('expansionPanelPrivateAddress')
    expansionPanelPrivateAddress!: MatExpansionPanel;
    @ViewChild('expansionPanelOfficeAddress')
    expansionPanelOfficeAddress!: MatExpansionPanel;

    @Input() isUpdateMode = false;
    personModification = model<PersonUpdate | PersonCreate>();
    privateAddressExists = computed(() => !!this.personModification()?.privateAddress);
    officeAddressExists = computed(() => !!this.personModification()?.officeAddress);

    areAddressesEmpty = computed(() => {
        if (this.maskAddress()) {
            return false;
        }

        this.formState(); // Track form state
        return this.checkIfAddressIsEmpty('officeAddress') && this.checkIfAddressIsEmpty('privateAddress');
    });
    isWithoutActiveAddress = computed(() => {
        if (this.maskAddress()) {
            return false;
        }

        this.formState(); // Track form state
        return !this.personForm.controls.privateAddress.controls.activeAddress.value && !this.personForm.controls.officeAddress.controls.activeAddress.value;
    });
    isActiveAddressEmpty = computed(() => {
        if (this.maskAddress()) {
            return false;
        }

        this.formState(); // Track form state
        return (
            (this.personForm.controls.privateAddress.controls.activeAddress.value && this.checkIfAddressIsEmpty('privateAddress')) ||
            (this.personForm.controls.officeAddress.controls.activeAddress.value && this.checkIfAddressIsEmpty('officeAddress'))
        );
    });
    hasAddressErrors = computed(() => !this.maskAddress() && (this.areAddressesEmpty() || this.isWithoutActiveAddress() || this.isActiveAddressEmpty()));
    occupationsDisabled = false;

    personForm = this.createForm();
    similarPersons: PersonDetails[] = [];
    officeId: string | undefined | null;
    councilId: string | undefined | null;
    employer: string | undefined | null;
    isFemalePerson: boolean = false;

    @ViewChild('chipInput') chipInput!: ElementRef<HTMLInputElement>;

    readonly separatorKeysCodes: number[] = [ENTER, COMMA];
    selectedOccupations: Occupation[] = [];
    filteredOccupationDb = signal<Occupation[]>([]);

    selectedLegislaturePeriodTexts = computed(() => {
        const legislaturePeriodIds = this.personModification()?.legislaturePeriodIds;
        const legislaturePeriods = this.masterDataService.legislaturePeriods();
        if (!!legislaturePeriodIds?.length || !!legislaturePeriods?.length) {
            return legislaturePeriods
                .filter(legislaturePeriod => legislaturePeriodIds?.includes(legislaturePeriod.id))
                .map(legislaturePeriod => legislaturePeriod.text)
                .join(', ');
        }
        return '';
    });

    filteredZipsAndCitiesOffice = signal<Address[]>([]);
    filteredZipsAndCitiesStreetOffice = computed<Address[]>(() => this.filteredZipsAndCitiesOffice().filter(address => address.street !== undefined));

    filteredZipsAndCitiesPrivate = signal<Address[]>([]);
    filteredZipsAndCitiesStreetPrivate = computed<Address[]>(() => this.filteredZipsAndCitiesPrivate().filter(address => address.street !== undefined));

    filteredOffices = signal<Office[]>([]);

    isDepartment = false;
    isOffice = false;
    isSecretariat = false;

    protected readonly maskAddress = computed(() => this.isUpdateMode && (this.personModification() as PersonUpdate)?.maskAddress);

    private static readonly RECOMMENDATION_STREET_MIN_LENGTH = 4;
    private static readonly RECOMMENDATION_ZIP_MIN_LENGTH = 2;
    private static readonly RECOMMENDATION_CITY_MIN_LENGTH = 2;

    // Signal to trigger reactivity on form changes
    private readonly formState = signal(0);

    private readonly unsubscribe = new Subject<void>();

    private readonly selectedGenderId = toSignal(this.personForm.controls.genderId.valueChanges);
    private readonly selectedSurname = toSignal(this.personForm.controls.surname.valueChanges);
    private readonly selectedTitle = toSignal(this.personForm.controls.title.valueChanges.pipe(debounceTime(1000)));
    private readonly selectedCorrespondenceLanguageId = toSignal(this.personForm.controls.correspondenceLanguageId.valueChanges);
    private lastSalutationParamsKey?: string;

    private readonly salutationParams = computed(() => ({
        surname: this.selectedSurname() || this.personModification()?.surname,
        title: this.selectedTitle() || this.personModification()?.title,
        correspondenceLanguageId: this.selectedCorrespondenceLanguageId() || this.personModification()?.correspondenceLanguageId,
        genderId: this.selectedGenderId() || this.personModification()?.genderId,
    }));

    constructor(
        protected readonly masterDataService: MasterDataService,
        protected readonly personService: PersonsService,
        protected readonly addressService: AddressService,
        protected readonly obNotificationService: ObNotificationService,
        protected readonly errorService: ErrorService,
        protected readonly authService: AuthService,
        protected readonly configsService: ConfigsService
    ) {
        this.authService.isDepartmentUser$.pipe(takeUntil(this.unsubscribe)).subscribe(isDepartment => {
            this.isDepartment = isDepartment;
        });

        this.authService.isOfficeUser$.pipe(takeUntil(this.unsubscribe)).subscribe(isOffice => {
            this.isOffice = isOffice;
        });

        this.authService.isSecretariatUser$.pipe(takeUntil(this.unsubscribe)).subscribe(isSecretariat => {
            this.isSecretariat = isSecretariat;
        });

        effect(() => {
            if (this.isUpdateMode && this.personModification()?.hasInterests) {
                this.personForm.controls.noInterest.disable({emitEvent: false});
            } else if (this.isUpdateMode) {
                this.personForm.controls.noInterest.enable({emitEvent: false});
            }
        });

        const effectRef = effect(() => {
            if (this.isUpdateMode && this.personModification()) {
                this.personForm.patchValue(this.personModification()!, {emitEvent: false});
                if (this.personModification()?.occupations) {
                    this.personForm.controls.occupations.patchValue(this.personModification()!.occupations!);
                    this.selectedOccupations = this.personModification()!.occupations!;
                }

                if (this.personModification()?.noEmployment) {
                    this.personForm.controls.federalDuty.disable();
                    this.personForm.controls.employer.disable();
                }

                if (this.personModification()?.federalDuty) {
                    this.personForm.controls.noEmployment.disable();
                    this.personForm.controls.officeId.enable();
                    this.personForm.controls.officeId.setValue((this.personModification() as PersonUpdate)?.officeId);
                    this.personForm.controls.federalAssembly.disable();
                } else {
                    this.personForm.controls.officeId.disable();
                }

                if (this.personModification()?.federalAssembly) {
                    this.personForm.controls.councilId.enable();
                    this.personForm.controls.federalDuty.disable();
                } else {
                    this.personForm.controls.councilId.disable();
                }

                if (this.privateAddressExists()) {
                    this.expansionPanelPrivateAddress.open();
                }
                if (this.officeAddressExists()) {
                    this.expansionPanelOfficeAddress.open();
                }

                if (this.personModification()!.hasActiveMembership && (this.isDepartment || this.isOffice || this.isSecretariat)) {
                    this.personForm.controls.languageId.disable();
                    this.personForm.controls.genderId.disable();
                } else {
                    this.personForm.controls.languageId.enable();
                    this.personForm.controls.genderId.enable();
                }

                // Update validators for address fields based on activeAddress state
                this.personForm.controls.officeAddress.controls.street.updateValueAndValidity();
                this.personForm.controls.officeAddress.controls.zip.updateValueAndValidity();
                this.personForm.controls.officeAddress.controls.city.updateValueAndValidity();
                this.personForm.controls.officeAddress.controls.cantonId.updateValueAndValidity();
                this.personForm.controls.officeAddress.controls.countryCode.updateValueAndValidity();
                this.personForm.controls.privateAddress.controls.street.updateValueAndValidity();
                this.personForm.controls.privateAddress.controls.zip.updateValueAndValidity();
                this.personForm.controls.privateAddress.controls.city.updateValueAndValidity();
                this.personForm.controls.privateAddress.controls.cantonId.updateValueAndValidity();
                this.personForm.controls.privateAddress.controls.countryCode.updateValueAndValidity();

                effectRef.destroy();
            }
        });

        effect(() => {
            const genderId = this.selectedGenderId();
            if (genderId) {
                if (genderId.toString() === configsService.frontendConfig.entityIds.gender.maleId) {
                    this.isFemalePerson = false;
                    this.personForm.controls.salutationId.setValue(configsService.frontendConfig.entityIds.salutation.maleId);
                } else if (genderId.toString() === configsService.frontendConfig.entityIds.gender.femaleId) {
                    this.isFemalePerson = true;
                    this.personForm.controls.salutationId.setValue(configsService.frontendConfig.entityIds.salutation.femaleId);
                }
            }
        });

        this.personForm.valueChanges.pipe(debounceTime(300), takeUntilDestroyed()).subscribe(formValues => {
            const formValuesWithNull = {...this.personForm.getRawValue()};

            this.officeId = formValuesWithNull.officeId;
            this.councilId = formValuesWithNull.councilId;
            this.employer = formValuesWithNull.employer;

            this.personForm.controls.officeAddress.setErrors(null);
            this.personForm.markAllAsTouched();

            // Trigger computed signals to re-evaluate
            this.formState.update(v => v + 1);

            this.personModification.update(
                value =>
                    ({
                        ...value,
                        ...this.GetFormContent(formValues as PersonCreate | PersonUpdate),
                        officeId: this.officeId,
                        councilId: this.councilId,
                        employer: this.employer,
                    }) as PersonCreate | PersonUpdate
            );
        });

        this.subscribeToAddressChanges('officeAddress');
        this.subscribeToAddressChanges('privateAddress');

        combineLatest([
            this.personForm.controls.givenName.valueChanges,
            this.personForm.controls.surname.valueChanges,
            this.personForm.controls.birthYear.valueChanges,
        ])
            .pipe(debounceTime(500), takeUntilDestroyed())
            .subscribe(() => this.searchSimilarPersons());

        this.personForm.controls.federalAssembly.valueChanges
            .pipe(takeUntilDestroyed())
            .subscribe(() => this.personForm.controls.legislaturePeriodIds.updateValueAndValidity({emitEvent: false}));

        this.personForm.controls.privateAddress.controls.street.valueChanges
            .pipe(takeUntilDestroyed())
            .subscribe(() => this.personForm.controls.privateAddress.controls.zip.updateValueAndValidity({emitEvent: false}));

        this.personForm.controls.officeAddress.controls.street.valueChanges
            .pipe(takeUntilDestroyed())
            .subscribe(() => this.personForm.controls.officeAddress.controls.zip.updateValueAndValidity({emitEvent: false}));

        this.personForm.controls.languageId.valueChanges.pipe(takeUntilDestroyed()).subscribe(value => {
            this.personForm.controls.correspondenceLanguageId.setValue(value);
            this.refreshCorrespondenceLanguageState();
        });

        // Effect to generate salutation when surname, title, correspondenceLanguageId, or genderId changes
        effect(() => {
            const params = this.salutationParams();

            if (
                !params.surname ||
                !params.correspondenceLanguageId ||
                !params.genderId ||
                isEmptyId(params.correspondenceLanguageId) ||
                isEmptyId(params.genderId)
            ) {
                return;
            }

            const key = `${params.genderId}|${params.correspondenceLanguageId}|${params.surname}|${params.title ?? ''}`;

            // Prevent initial call on update mode by seeding the key from existing data
            if (this.isUpdateMode && !this.lastSalutationParamsKey && this.personModification()) {
                this.lastSalutationParamsKey = key;
                return;
            }

            if (key === this.lastSalutationParamsKey) {
                return;
            }
            this.lastSalutationParamsKey = key;

            this.personService
                .generateSalutation(params.genderId, params.correspondenceLanguageId, params.surname, params.title ?? '')
                .subscribe(salutationText => {
                    this.personForm.controls.salutationText.setValue(salutationText, {emitEvent: false});
                    this.personModification()!.salutationText = salutationText;
                });
        });

        this.personForm.controls.officeId.valueChanges.pipe(takeUntilDestroyed()).subscribe(value => {
            if (value) {
                this.personForm.controls.councilId.setValue(null);
                this.personForm.controls.councilId.disable();
                this.personForm.controls.employer.setValue('');
                this.personForm.controls.employer.disable();
            }
        });

        this.personForm.controls.councilId.valueChanges.pipe(takeUntilDestroyed()).subscribe(value => {
            if (value) {
                this.personForm.controls.officeId.setValue(null);
                this.personForm.controls.officeId.disable();
            }
        });

        this.personForm.controls.occupations.valueChanges.pipe(takeUntilDestroyed()).subscribe(value => {
            if (value && value.length >= 3 && typeof value === 'string') {
                this.masterDataService.getOccupationsByName(value).subscribe(result => this.filteredOccupationDb.set(result));
            }
        });

        // Subscribe to activeAddress changes to update validators
        this.personForm.controls.officeAddress.controls.activeAddress.valueChanges.pipe(takeUntilDestroyed()).subscribe(() => {
            this.personForm.controls.officeAddress.controls.street.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.zip.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.city.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.cantonId.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.countryCode.updateValueAndValidity();
        });

        this.personForm.controls.privateAddress.controls.activeAddress.valueChanges.pipe(takeUntilDestroyed()).subscribe(() => {
            this.personForm.controls.privateAddress.controls.street.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.zip.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.city.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.cantonId.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.countryCode.updateValueAndValidity();
        });
    }

    ngOnInit(): void {
        if (!this.isUpdateMode) {
            this.toggleNonBasicControls(false);
        }
    }

    changeActiveAddress(selection: AddressControlId) {
        if (selection === 'privateAddress') {
            this.personForm.controls.privateAddress.patchValue({activeAddress: true});
            this.personForm.controls.officeAddress.patchValue({activeAddress: false});
            // Update validators for both addresses
            this.personForm.controls.privateAddress.controls.street.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.zip.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.city.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.cantonId.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.countryCode.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.street.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.zip.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.city.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.cantonId.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.countryCode.updateValueAndValidity();
        } else if (selection === 'officeAddress') {
            this.personForm.controls.officeAddress.patchValue({activeAddress: true});
            this.personForm.controls.privateAddress.patchValue({activeAddress: false});
            // Update validators for both addresses
            this.personForm.controls.officeAddress.controls.street.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.zip.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.city.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.cantonId.updateValueAndValidity();
            this.personForm.controls.officeAddress.controls.countryCode.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.street.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.zip.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.city.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.cantonId.updateValueAndValidity();
            this.personForm.controls.privateAddress.controls.countryCode.updateValueAndValidity();
        }
        this.personForm.markAsDirty();
    }

    validateAddresses() {
        if (this.maskAddress()) {
            // in this case we don't update any addresses on save. Therefore a validation isn't necessary.
            return true;
        }

        if (this.hasAddressErrors()) {
            this.personForm.controls.officeAddress.setErrors({incorrect: true});
            return false;
        }

        return true;
    }

    isNullOrEmpty(value: string | number | undefined | null): boolean {
        return !value?.toString()?.trim();
    }

    setStreet(event: MatAutocompleteSelectedEvent, controlId: AddressControlId) {
        const address = event.option.value as Address;
        this.personForm.controls[controlId].controls.street.setValue(address.street!, {emitEvent: false});
        this.personForm.controls[controlId].controls.zip.setValue(address.zip!, {emitEvent: false});
        this.personForm.controls[controlId].controls.city.setValue(address.city!, {emitEvent: false});
        this.personForm.controls[controlId].controls.cantonId.setValue(address.canton?.id);
        this.personForm.controls[controlId].controls.countryCode.setValue('CH');
        if (controlId === 'officeAddress') {
            this.filteredZipsAndCitiesOffice.set([]);
        } else {
            this.filteredZipsAndCitiesPrivate.set([]);
        }
        // Trigger computed signals to re-evaluate
        this.formState.update(v => v + 1);
    }

    setZipOrCity(event: MatAutocompleteSelectedEvent, controlId: AddressControlId) {
        const address = event.option.value as Address;
        this.personForm.controls[controlId].controls.zip.setValue(address.zip!, {emitEvent: false});
        this.personForm.controls[controlId].controls.city.setValue(address.city!, {emitEvent: false});
        this.personForm.controls[controlId].controls.cantonId.setValue(address.canton?.id);
        this.personForm.controls[controlId].controls.countryCode.setValue('CH');
        if (controlId === 'officeAddress') {
            this.filteredZipsAndCitiesOffice.set([]);
        } else {
            this.filteredZipsAndCitiesPrivate.set([]);
        }
        // Trigger computed signals to re-evaluate
        this.formState.update(v => v + 1);
    }

    setCanton(event: MatSelectChange, controlId: AddressControlId) {
        if (event.value && event.value !== this.configsService.frontendConfig.entityIds.canton.abroadId) {
            this.personForm.controls[controlId].controls.countryCode.setValue('CH');
        } else {
            this.personForm.controls[controlId].controls.countryCode.setValue(null);
        }
    }

    onManualSimilarityCheckConfirmation(event: MatCheckboxChange) {
        this.toggleNonBasicControls(event.checked);
    }

    isBasicDataComplete(): boolean {
        return (
            !this.isNullOrEmpty(this.personForm.controls.surname.value) &&
            !this.isNullOrEmpty(this.personForm.controls.givenName.value) &&
            !this.isNullOrEmpty(this.personForm.controls.birthYear.value) &&
            this.personForm.controls.birthYear.valid
        );
    }

    getPersonsCity(personDetail: PersonDetails): string {
        if (personDetail.officeAddress && personDetail.officeAddress.activeAddress && !this.isNullOrEmpty(personDetail.officeAddress.city)) {
            return `, ${personDetail.officeAddress.city}`;
        }

        if (personDetail.privateAddress && personDetail.privateAddress.activeAddress && !this.isNullOrEmpty(personDetail.privateAddress.city)) {
            return `, ${personDetail.privateAddress.city}`;
        }
        return '';
    }

    searchSimilarPersons() {
        if (!this.isBasicDataComplete()) {
            return;
        }
        if (this.isUpdateMode) {
            this.personForm.controls.confirmNewPerson.setValue(true);
            return;
        }
        this.personService
            .getSimilarPersons(this.personForm.controls.surname.value, this.personForm.controls.givenName.value, this.personForm.controls.birthYear.value!, 5)
            .subscribe({
                next: similarPersons => {
                    this.similarPersons = similarPersons;
                    if (similarPersons.length !== 0) {
                        this.personForm.controls.confirmNewPerson.setValue(false);
                        this.toggleNonBasicControls(false);
                    } else {
                        this.personForm.controls.confirmNewPerson.setValue(true);
                        this.toggleNonBasicControls(true);
                    }
                },
                error: () => this.obNotificationService.error('person.create.validate.name.error'),
            });
    }

    clear(controlId: AddressControlId) {
        this.personForm.controls[controlId].controls.street.patchValue(null, {emitEvent: false});
        this.personForm.controls[controlId].controls.zip.patchValue(null, {emitEvent: false});
        this.personForm.controls[controlId].controls.city.patchValue(null, {emitEvent: false});
        this.personForm.controls[controlId].controls.cantonId.patchValue(null, {emitEvent: false});
        this.personForm.controls[controlId].controls.countryCode.patchValue(null);
        if (controlId === 'officeAddress') {
            this.filteredZipsAndCitiesOffice.set([]);
        } else {
            this.filteredZipsAndCitiesPrivate.set([]);
        }
    }

    noEmploymentChecked(event: MatCheckboxChange) {
        if (event.checked) {
            this.personForm.controls.employer.setValue('');
            this.personForm.controls.employer.disable();
            this.personForm.controls.federalDuty.setValue(false);
            this.personForm.controls.federalDuty.disable();
            this.personForm.controls.officeId.setValue(null);
            this.personForm.controls.officeId.disable();
        } else {
            this.personForm.controls.employer.enable();
            if (!this.personForm.controls.federalAssembly.value) {
                this.personForm.controls.federalDuty.enable();
            }
        }
    }

    noInterestChecked(event: MatCheckboxChange) {
        if (event.checked) {
            this.personForm.controls.councilId.setValue(undefined);
            this.personForm.controls.councilId.disable();
            this.personForm.controls.legislaturePeriodIds.setValue([]);
            this.personForm.controls.legislaturePeriodIds.disable();
        }
    }

    federalDutyChecked(event: MatCheckboxChange) {
        if (event.checked) {
            this.personForm.controls.federalAssembly.disable();
            this.personForm.controls.federalAssembly.setValue(false);
            this.personForm.controls.officeId.enable();
            this.personForm.controls.councilId.setValue(undefined);
            this.personForm.controls.noEmployment.disable();
            this.personForm.controls.noEmployment.setValue(false);
        } else {
            this.personForm.controls.officeId.setValue(null);
            this.personForm.controls.officeId.disable();
            this.personForm.controls.federalAssembly.enable();
            this.personForm.controls.employer.enable();
            this.personForm.controls.noEmployment.enable();
        }
    }

    federalAssemblyChecked(event: MatCheckboxChange) {
        if (event.checked) {
            this.personForm.controls.federalDuty.disable();
            this.personForm.controls.federalDuty.setValue(false);
            this.personForm.controls.councilId.enable();
            this.personForm.controls.officeId.setValue(null);
            this.personForm.controls.legislaturePeriodIds.enable();
        } else {
            this.personForm.controls.councilId.setValue(undefined);
            this.personForm.controls.councilId.disable();
            this.personForm.controls.federalDuty.enable();
            this.personForm.controls.legislaturePeriodIds.disable();

            if (this.isUpdateMode && this.personModification()?.hasInterests) {
                this.personForm.controls.noInterest.disable();
            }
        }
    }

    remove(occupation: Occupation): void {
        const index = this.selectedOccupations.indexOf(occupation);
        if (index >= 0) {
            this.selectedOccupations.splice(index, 1);
            this.personForm.controls.occupations.markAsDirty();
        }
    }

    selected(event: MatAutocompleteSelectedEvent): void {
        const occ = event.option.value as Occupation;
        if (!this.selectedOccupations.find(o => o.id === occ.id)) {
            this.selectedOccupations.push(occ);
        }
        this.personForm.controls.occupations.setValue('');
        this.personForm.controls.occupations.markAsDirty();
        this.filteredOccupationDb.set([]);
    }

    private checkIfAddressIsEmpty(controlId: AddressControlId): boolean {
        return (
            this.isNullOrEmpty(this.personForm.controls[controlId].controls.city.value) &&
            this.isNullOrEmpty(this.personForm.controls[controlId].controls.cantonId.value) &&
            this.isNullOrEmpty(this.personForm.controls[controlId].controls.companyName.value) &&
            this.isNullOrEmpty(this.personForm.controls[controlId].controls.countryCode.value) &&
            this.isNullOrEmpty(this.personForm.controls[controlId].controls.email.value) &&
            this.isNullOrEmpty(this.personForm.controls[controlId].controls.mobile.value) &&
            this.isNullOrEmpty(this.personForm.controls[controlId].controls.phone.value) &&
            this.isNullOrEmpty(this.personForm.controls[controlId].controls.poBox.value) &&
            this.isNullOrEmpty(this.personForm.controls[controlId].controls.street.value) &&
            this.isNullOrEmpty(this.personForm.controls[controlId].controls.zip.value)
        );
    }

    private toggleNonBasicControls(enable: boolean) {
        this.personForm.controls.officeId.disable();
        this.personForm.controls.councilId.disable();
        this.personForm.controls.occupation.disable();

        if (enable) {
            this.occupationsDisabled = false;

            this.personForm.controls.languageId.enable();
            this.refreshCorrespondenceLanguageState();
            this.personForm.controls.title.enable();
            this.personForm.controls.occupations.enable();
            this.personForm.controls.employer.enable();
            this.personForm.controls.genderId.enable();
            this.personForm.controls.noEmployment.enable();
            this.personForm.controls.noInterest.enable();

            this.personForm.controls.federalDuty.enable();
            if (this.personForm.controls.federalDuty.value) {
                this.personForm.controls.officeId.enable();
            }

            this.personForm.controls.federalAssembly.enable();
            if (this.personForm.controls.federalAssembly.value) {
                this.personForm.controls.councilId.enable();
            }

            this.personForm.controls.officeAddress.enable();
            this.personForm.controls.privateAddress.enable();
            if (this.isUpdateMode && this.personModification()?.hasInterests) {
                this.personForm.controls.noInterest.disable();
            }
        } else {
            this.occupationsDisabled = true;
            this.personForm.controls.languageId.disable();
            this.personForm.controls.correspondenceLanguageId.disable();
            this.personForm.controls.title.disable();
            this.personForm.controls.occupations.disable();
            this.personForm.controls.employer.disable();
            this.personForm.controls.genderId.disable();
            this.personForm.controls.noEmployment.disable();
            this.personForm.controls.noInterest.disable();

            this.personForm.controls.federalDuty.disable();
            this.personForm.controls.federalAssembly.disable();
            this.personForm.controls.officeAddress.disable();
            this.personForm.controls.privateAddress.disable();
        }
    }

    private refreshCorrespondenceLanguageState() {
        const romanshId = this.configsService.frontendConfig.entityIds.language.romanshLanguageId;
        const current = this.personForm.controls.languageId.value?.toString();
        if (current === romanshId) {
            this.personForm.controls.correspondenceLanguageId.enable({emitEvent: false});
        } else {
            this.personForm.controls.correspondenceLanguageId.disable({emitEvent: false});
            // Keep correspondenceLanguageId aligned with languageId while disabled
            this.personForm.controls.correspondenceLanguageId.setValue(this.personForm.controls.languageId.value, {emitEvent: false});
        }
    }

    private createForm() {
        const form = new FormGroup({
            salutationId: new FormControl<string | undefined>({value: undefined, disabled: true}),
            salutationText: new FormControl<string | undefined>(undefined, {validators: [Validators.maxLength(200)]}),
            surname: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.maxLength(150)]}),
            givenName: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.maxLength(150)]}),
            birthYear: new FormControl<number | undefined>(undefined, {
                nonNullable: true,
                validators: [Validators.required, Validators.pattern(/^(\d*)?$/), Validators.min(1900), Validators.max(2100)],
            }),
            confirmNewPerson: new FormControl(false, {nonNullable: true, validators: Validators.required}),
            languageId: new FormControl('', {nonNullable: true, validators: Validators.required}),
            correspondenceLanguageId: new FormControl('', {nonNullable: true, validators: Validators.required}),
            title: new FormControl(''),
            occupation: new FormControl<string | null | undefined>({value: null, disabled: true}),
            employer: new FormControl(''),
            occupations: new FormControl<Occupation[] | string>([], {nonNullable: true}),
            officeId: new FormControl<string | null | undefined>({value: null, disabled: false}),
            councilId: new FormControl<string | undefined>({value: undefined, disabled: false}),
            genderId: new FormControl('', {nonNullable: true, validators: Validators.required}),
            federalDuty: new FormControl(false, {nonNullable: true, validators: Validators.required}),
            noEmployment: new FormControl(false, {nonNullable: true, validators: Validators.required}),
            noInterest: new FormControl(false, {nonNullable: true, validators: Validators.required}),
            federalAssembly: new FormControl(false, {nonNullable: true, validators: Validators.required}),
            legislaturePeriodIds: new FormControl<string[]>([], {nonNullable: true}),
            officeAddress: new FormGroup({
                companyName: new FormControl(''),
                street: new FormControl(''),
                poBox: new FormControl(''),
                countryCode: new FormControl(''),
                zip: new FormControl(''),
                city: new FormControl(''),
                phone: new FormControl('', {validators: [Validators.pattern(TEL_PATTERN), Validators.maxLength(20)]}),
                mobile: new FormControl('', {validators: [Validators.pattern(TEL_PATTERN), Validators.maxLength(20)]}),
                email: new FormControl('', {validators: [Validators.maxLength(150), Validators.email]}),
                cantonId: new FormControl<string | undefined>(undefined),
                activeAddress: new FormControl(false, {nonNullable: true, validators: Validators.required}),
            }),
            privateAddress: new FormGroup({
                companyName: new FormControl(''),
                street: new FormControl(''),
                poBox: new FormControl(''),
                countryCode: new FormControl(''),
                zip: new FormControl(''),
                city: new FormControl(''),
                phone: new FormControl('', {validators: [Validators.pattern(TEL_PATTERN), Validators.maxLength(20)]}),
                mobile: new FormControl('', {validators: [Validators.pattern(TEL_PATTERN), Validators.maxLength(20)]}),
                email: new FormControl('', {validators: [Validators.maxLength(150), Validators.email]}),
                cantonId: new FormControl<string | undefined>(undefined),
                activeAddress: new FormControl(false, {nonNullable: true, validators: Validators.required}),
            }),
        });

        form.controls.legislaturePeriodIds.setValidators(conditionalValidator(() => form.controls.federalAssembly.value, Validators.required));
        form.controls.councilId.setValidators(conditionalValidator(() => form.controls.federalAssembly.value, Validators.required));
        form.controls.officeId.setValidators(conditionalValidator(() => form.controls.federalDuty.value, Validators.required));

        // Office address validators - all fields required when active
        form.controls.officeAddress.controls.street.setValidators(
            conditionalValidator(() => form.controls.officeAddress.controls.activeAddress.value, Validators.required)
        );
        form.controls.officeAddress.controls.zip.setValidators(
            conditionalValidator(() => form.controls.officeAddress.controls.activeAddress.value, Validators.required)
        );
        form.controls.officeAddress.controls.city.setValidators(
            conditionalValidator(() => form.controls.officeAddress.controls.activeAddress.value, Validators.required)
        );
        form.controls.officeAddress.controls.cantonId.setValidators(
            conditionalValidator(() => form.controls.officeAddress.controls.activeAddress.value, Validators.required)
        );
        form.controls.officeAddress.controls.countryCode.setValidators(
            conditionalValidator(() => form.controls.officeAddress.controls.activeAddress.value, Validators.required)
        );

        // Private address validators - all fields required when active
        form.controls.privateAddress.controls.street.setValidators(
            conditionalValidator(() => form.controls.privateAddress.controls.activeAddress.value, Validators.required)
        );
        form.controls.privateAddress.controls.zip.setValidators(
            conditionalValidator(() => form.controls.privateAddress.controls.activeAddress.value, Validators.required)
        );
        form.controls.privateAddress.controls.city.setValidators(
            conditionalValidator(() => form.controls.privateAddress.controls.activeAddress.value, Validators.required)
        );
        form.controls.privateAddress.controls.cantonId.setValidators(
            conditionalValidator(() => form.controls.privateAddress.controls.activeAddress.value, Validators.required)
        );
        form.controls.privateAddress.controls.countryCode.setValidators(
            conditionalValidator(() => form.controls.privateAddress.controls.activeAddress.value, Validators.required)
        );

        return form;
    }

    private GetFormContent(person: PersonUpdate | PersonCreate | undefined): PersonUpdate | PersonCreate | undefined {
        if (person) {
            person.occupations = this.selectedOccupations;
            if (!person.correspondenceLanguageId && person.languageId) {
                person.correspondenceLanguageId = person.languageId;
            }
        }

        if (
            person &&
            !person.privateAddress?.cantonId &&
            !person.privateAddress?.city &&
            !person.privateAddress?.companyName &&
            !person.privateAddress?.countryCode &&
            !person.privateAddress?.email &&
            !person.privateAddress?.mobile &&
            !person.privateAddress?.phone &&
            !person.privateAddress?.poBox &&
            !person.privateAddress?.street &&
            !person.privateAddress?.zip
        ) {
            person.privateAddress = undefined;
        }

        if (
            person &&
            !person.officeAddress?.cantonId &&
            !person.officeAddress?.city &&
            !person.officeAddress?.companyName &&
            !person.officeAddress?.countryCode &&
            !person.officeAddress?.email &&
            !person.officeAddress?.mobile &&
            !person.officeAddress?.phone &&
            !person.officeAddress?.poBox &&
            !person.officeAddress?.street &&
            !person.officeAddress?.zip
        ) {
            person.officeAddress = undefined;
        }

        return person;
    }

    private subscribeToAddressChanges(controlId: AddressControlId) {
        merge(
            this.personForm.controls[controlId].controls.street.valueChanges.pipe(
                filter(street => this.checkAddressParam(street, PersonDataFormComponent.RECOMMENDATION_STREET_MIN_LENGTH)),
                map(street => ({street}))
            ),
            this.personForm.controls[controlId].controls.zip.valueChanges.pipe(
                filter(zip => this.checkAddressParam(zip, PersonDataFormComponent.RECOMMENDATION_ZIP_MIN_LENGTH)),
                map(zip => ({zip}))
            ),
            this.personForm.controls[controlId].controls.city.valueChanges.pipe(
                filter(city => this.checkAddressParam(city, PersonDataFormComponent.RECOMMENDATION_CITY_MIN_LENGTH)),
                map(city => ({city}))
            )
        )
            .pipe(
                debounceTime(300),
                switchMap(searchParams => this.addressService.getAddressSuggestions(searchParams)),
                takeUntilDestroyed()
            )
            .subscribe(suggestions => {
                const sortedSuggestions = suggestions
                    .map(item => ({...item, id: `${item.street!};${item.zip!};${item.city};${item.canton?.id}`}))
                    .sort((a, b) => (a.zip! < b.zip! ? -1 : 1));
                if (controlId === 'officeAddress') {
                    this.filteredZipsAndCitiesOffice.set(sortedSuggestions);
                } else {
                    this.filteredZipsAndCitiesPrivate.set(sortedSuggestions);
                }
            });
    }

    private checkAddressParam(param: string | null | undefined, minLength: number): boolean {
        return !!param && param.length >= minLength && !param.includes(';');
    }
}
