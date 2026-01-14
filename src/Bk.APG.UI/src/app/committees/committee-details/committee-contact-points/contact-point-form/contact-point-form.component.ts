import {Component, computed, effect, Input, model, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatAutocomplete, MatAutocompleteSelectedEvent, MatAutocompleteTrigger} from '@angular/material/autocomplete';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatIcon} from '@angular/material/icon';
import {MatError, MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {MatRadioButton, MatRadioGroup} from '@angular/material/radio';
import {MatOption, MatSelect} from '@angular/material/select';
import {Address} from '@api/Address';
import {ContactPointCreate} from '@api/ContactPointCreate';
import {ContactPointUpdate} from '@api/ContactPointUpdate';
import {Office} from '@api/Office';
import {TranslatePipe} from '@ngx-translate/core';
import {ObErrorMessagesDirective, ObInputClearDirective, ObUnsavedChangesDirective} from '@oblique/oblique';
import {AddressService} from '@shared/address.service';
import {today} from '@shared/date-util';
import {ErrorService} from '@shared/error-service.service';
import {conditionalValidator} from '@shared/form-validators/conditional.validator';
import {Comparison, rangeValidator} from '@shared/form-validators/range.validator';
import {TEL_PATTERN} from '@shared/form-validators/validation-patterns';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {debounceTime, distinctUntilChanged, filter, map, merge, switchMap} from 'rxjs';

interface ContactPointForm {
    contactPointTypeUri: FormControl<string | null>;
    companyName: FormControl<string | null>;
    section: FormControl<string | null>;
    beginDate: FormControl<Date | null>;
    endDate: FormControl<Date | null>;
    street: FormControl<string | null>;
    poBox: FormControl<string | null>;
    zip: FormControl<string | null>;
    city: FormControl<string | null>;
    phone: FormControl<string | null>;
    email: FormControl<string | null>;
    surname: FormControl<string | null>;
    givenName: FormControl<string | null>;
    title: FormControl<string | null>;
    languageId: FormControl<string | null>;
    genderId: FormControl<string | null>;
    personalPhone: FormControl<string | null>;
    personalMobile: FormControl<string | null>;
    personalEmail: FormControl<string | null>;
    releasePersonData: FormControl<boolean | null>;
    committeeBeginDate: FormControl<Date | null>;
}

@Component({
    selector: 'apg-contact-point-form',
    templateUrl: './contact-point-form.component.html',
    styleUrl: './contact-point-form.component.scss',
    imports: [
        ReactiveFormsModule,
        ObUnsavedChangesDirective,
        MatRadioGroup,
        MatRadioButton,
        MatFormField,
        ObErrorMessagesDirective,
        MatLabel,
        MatInput,
        MatDatepickerInput,
        MatDatepickerToggle,
        MatSuffix,
        MatDatepicker,
        MatError,
        MatAutocompleteTrigger,
        MatAutocomplete,
        MatOption,
        MatSelect,
        MatCheckbox,
        HelpTooltipComponent,
        ObInputClearDirective,
        MatIcon,
        TranslatePipe,
    ],
})
export class ContactPointFormComponent {
    @Input() isUpdateMode = false;
    @Input() isCopyMode = false;
    contactPointModification = model<ContactPointUpdate | ContactPointCreate>();
    isSecretariat = true;

    contactPointForm: FormGroup<ContactPointForm>;

    filteredZipsAndCities = signal<Address[]>([]);
    filteredZipsAndCitiesStreet = computed<Address[]>(() => this.filteredZipsAndCities().filter(address => address.street !== undefined));

    filteredCompanyNames = signal<Office[]>([]);

    private static readonly RECOMMENDATION_STREET_MIN_LENGTH = 4;
    private static readonly RECOMMENDATION_ZIP_MIN_LENGTH = 2;
    private static readonly RECOMMENDATION_CITY_MIN_LENGTH = 2;
    private static readonly RECOMMENDATION_COMPANYNAME_MIN_LENGTH = 2;

    constructor(
        protected readonly masterDataService: MasterDataService,
        protected readonly errorService: ErrorService,
        protected readonly addressService: AddressService,
        protected readonly formBuilder: FormBuilder
    ) {
        this.contactPointForm = this.createForm();

        const effectRef = effect(() => {
            if (this.isUpdateMode && !!(this.contactPointModification() as ContactPointUpdate)?.id) {
                this.contactPointForm.patchValue(this.contactPointModification()!, {emitEvent: false});
                this.contactPointForm.controls.beginDate.patchValue(new Date(this.contactPointModification()!.beginDate));
                if (this.contactPointModification()!.endDate) {
                    this.contactPointForm.controls.endDate.patchValue(new Date(this.contactPointModification()!.endDate));
                }
                this.isSecretariat =
                    this.contactPointModification()?.contactPointTypeUri === 'https://politics.ld.admin.ch/fch/apg/vocabulary/contact-point-type/1';
                this.contactPointForm.markAllAsTouched();
                this.updateValidityOnFormFields(this.contactPointForm);

                effectRef.destroy();
            }
            if (this.contactPointModification()) {
                this.contactPointForm.controls.committeeBeginDate.patchValue(new Date(this.contactPointModification()!.committeeBeginDate));
                this.contactPointForm.controls.beginDate.updateValueAndValidity();
            }
        });

        this.contactPointForm.valueChanges.pipe(debounceTime(300), takeUntilDestroyed()).subscribe(() => {
            const formValues = this.contactPointForm.getRawValue();
            this.contactPointModification.update(value => ({...value, ...(formValues as unknown as ContactPointUpdate | ContactPointCreate)}));
        });

        this.subscribeToAddressChanges();
        this.subscribeToCompanyChanges();
    }

    setStreet(event: MatAutocompleteSelectedEvent) {
        const address = event.option.value as Address;
        this.contactPointForm.controls.street.setValue(address.street!, {emitEvent: false});
        this.contactPointForm.controls.zip.setValue(address.zip!, {emitEvent: false});
        this.contactPointForm.controls.city.setValue(address.city!, {emitEvent: false});
        // To avoid having an object from autocomplete as field value (bug autocomplete?)
        this.contactPointForm.controls.poBox.setValue(this.contactPointForm.controls.poBox.value);

        this.filteredZipsAndCities.set([]);
    }

    setZipOrCity(event: MatAutocompleteSelectedEvent) {
        const address = event.option.value as Address;
        this.contactPointForm.controls.zip.setValue(address.zip!, {emitEvent: false});
        this.contactPointForm.controls.city.setValue(address.city!, {emitEvent: false});
        // To avoid having an object from autocomplete as field value (bug autocomplete?)
        this.contactPointForm.controls.poBox.setValue(this.contactPointForm.controls.poBox.value);
        this.filteredZipsAndCities.set([]);
    }

    setCompanyName(event: MatAutocompleteSelectedEvent) {
        const office = event.option.value as Office;
        this.contactPointForm.controls.companyName.setValue(office.description, {emitEvent: false});

        this.filteredCompanyNames.set([]);
    }

    clear() {
        this.contactPointForm.controls.street.patchValue(null);
        this.contactPointForm.controls.zip.patchValue(null);
        this.contactPointForm.controls.city.patchValue(null);
    }

    changeContactPointType(currentView: string) {
        this.isSecretariat = currentView !== 'https://politics.ld.admin.ch/fch/apg/vocabulary/contact-point-type/2';
    }

    private createForm(): FormGroup<ContactPointForm> {
        const formGroup = this.formBuilder.group<ContactPointForm>({
            contactPointTypeUri: this.formBuilder.control<string | null>('https://politics.ld.admin.ch/fch/apg/vocabulary/contact-point-type/1'),
            beginDate: this.formBuilder.control<Date | null>({value: today(), disabled: false}, {nonNullable: true}),
            endDate: this.formBuilder.control<Date | null>({value: null, disabled: false}),
            companyName: this.formBuilder.control<string | null>(null),
            section: this.formBuilder.control<string | null>(null, {validators: [Validators.maxLength(150)]}),
            street: this.formBuilder.control<string | null>(null, {validators: [Validators.maxLength(100)]}),
            poBox: this.formBuilder.control<string | null>(null, {validators: [Validators.maxLength(50)]}),
            zip: this.formBuilder.control<string | null>(null, {validators: [Validators.required, Validators.maxLength(10)]}),
            city: this.formBuilder.control<string | null>(null, {validators: [Validators.required, Validators.maxLength(100)]}),
            phone: this.formBuilder.control<string | null>(null, {validators: [Validators.pattern(TEL_PATTERN), Validators.maxLength(20)]}),
            email: this.formBuilder.control<string | null>(null),
            surname: this.formBuilder.control<string | null>(null),
            givenName: this.formBuilder.control<string | null>(null),
            title: this.formBuilder.control<string | null>(null, {validators: [Validators.maxLength(100)]}),
            genderId: this.formBuilder.control<string | null>(null),
            languageId: this.formBuilder.control<string | null>(null),
            personalPhone: this.formBuilder.control<string | null>(null, {validators: [Validators.pattern(TEL_PATTERN), Validators.maxLength(20)]}),
            personalMobile: this.formBuilder.control<string | null>(null, {validators: [Validators.pattern(TEL_PATTERN), Validators.maxLength(20)]}),
            personalEmail: this.formBuilder.control<string | null>(null),
            releasePersonData: this.formBuilder.control<boolean | null>(false),
            committeeBeginDate: this.formBuilder.control<Date | null>(null),
        });

        formGroup.controls.companyName.setValidators([
            Validators.maxLength(150),
            conditionalValidator(() => !formGroup.controls.surname.value, Validators.required),
        ]);
        formGroup.controls.surname.setValidators([
            Validators.maxLength(150),
            conditionalValidator(() => !formGroup.controls.companyName.value, Validators.required),
        ]);
        formGroup.controls.givenName.setValidators([
            Validators.maxLength(150),
            conditionalValidator(() => !!formGroup.controls.surname.value?.toString()?.trim(), Validators.required),
        ]);
        formGroup.controls.email.setValidators([
            Validators.maxLength(150),
            Validators.email,
            conditionalValidator(
                () => !!formGroup.controls.companyName.value?.toString()?.trim() && !formGroup.controls.surname.value?.toString()?.trim(),
                Validators.required
            ),
        ]);
        formGroup.controls.personalEmail.setValidators([
            Validators.maxLength(150),
            Validators.email,
            conditionalValidator(() => !!formGroup.controls.surname.value?.toString()?.trim(), Validators.required),
        ]);

        formGroup.controls.genderId.setValidators([conditionalValidator(() => !!formGroup.controls.surname.value?.toString()?.trim(), Validators.required)]);
        formGroup.controls.languageId.setValidators([conditionalValidator(() => !!formGroup.controls.surname.value?.toString()?.trim(), Validators.required)]);

        formGroup.controls.beginDate.setValidators([
            Validators.required,
            rangeValidator(formGroup.controls.endDate, Comparison.LowerThanEqual),
            rangeValidator(formGroup.controls.committeeBeginDate, Comparison.GreaterThanEqual),
        ]);
        formGroup.controls.endDate.setValidators(rangeValidator(formGroup.controls.beginDate, Comparison.GreaterThanEqual));

        formGroup.controls.surname.valueChanges.pipe(takeUntilDestroyed(), distinctUntilChanged()).subscribe(() => {
            this.updateValidityOnFormFields(formGroup);
        });

        formGroup.controls.companyName.valueChanges.pipe(takeUntilDestroyed(), distinctUntilChanged()).subscribe(() => {
            this.updateValidityOnFormFields(formGroup);
        });

        formGroup.controls.beginDate.valueChanges.pipe(takeUntilDestroyed(), distinctUntilChanged()).subscribe(() => {
            formGroup.controls.endDate.updateValueAndValidity();
        });

        formGroup.controls.endDate.valueChanges.pipe(takeUntilDestroyed(), distinctUntilChanged()).subscribe(() => {
            formGroup.controls.beginDate.updateValueAndValidity();
        });

        return formGroup;
    }

    private subscribeToAddressChanges() {
        merge(
            this.contactPointForm.controls.street.valueChanges.pipe(
                filter(street => this.checkAddressParam(street, ContactPointFormComponent.RECOMMENDATION_STREET_MIN_LENGTH)),
                map(street => ({street}))
            ),
            this.contactPointForm.controls.zip.valueChanges.pipe(
                filter(zip => this.checkAddressParam(zip, ContactPointFormComponent.RECOMMENDATION_ZIP_MIN_LENGTH)),
                map(zip => ({zip}))
            ),
            this.contactPointForm.controls.city.valueChanges.pipe(
                filter(city => this.checkAddressParam(city, ContactPointFormComponent.RECOMMENDATION_CITY_MIN_LENGTH)),
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
                    .map(item => ({...item, id: `${item.street!};${item.zip!};${item.city}`}))
                    .sort((a, b) => (a.zip! < b.zip! ? -1 : 1));
                this.filteredZipsAndCities.set(sortedSuggestions);
            });
    }

    private subscribeToCompanyChanges() {
        this.contactPointForm.controls.companyName.valueChanges
            .pipe(
                debounceTime(300),
                filter(companyName => this.checkAddressParam(companyName, ContactPointFormComponent.RECOMMENDATION_COMPANYNAME_MIN_LENGTH)),
                distinctUntilChanged(),
                switchMap(companyName => this.masterDataService.getOfficeByName(companyName!)),
                takeUntilDestroyed()
            )
            .subscribe(suggestions => {
                this.filteredCompanyNames.set(suggestions);
            });
    }

    private checkAddressParam(param: string | null, minLength: number): boolean {
        return !!param && param.length >= minLength && !param.includes(';');
    }

    private updateValidityOnFormFields(formGroup: FormGroup) {
        formGroup.controls.beginDate.updateValueAndValidity();
        formGroup.controls.endDate.updateValueAndValidity();
        formGroup.controls.companyName.updateValueAndValidity();
        formGroup.controls.email.updateValueAndValidity();

        formGroup.controls.surname.updateValueAndValidity();
        formGroup.controls.givenName.updateValueAndValidity();
        formGroup.controls.personalEmail.updateValueAndValidity();
        formGroup.controls.languageId.updateValueAndValidity();
        formGroup.controls.genderId.updateValueAndValidity();
    }
}
