import {Component, effect, model} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatInput} from '@angular/material/input';
import {MatFormField, MatLabel, MatError} from '@angular/material/select';
import {CommitteeTypeUpdate} from '@api/CommitteeTypeUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObUnsavedChangesDirective, ObErrorMessagesDirective} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {multiPercentageSumValidator} from '@shared/form-validators/multiPercentageSum.validator';
import {debounceTime} from 'rxjs';

interface CommitteeTypeForm {
    femaleThreshold: FormControl<number | null>;
    maleThreshold: FormControl<number | null>;
    germanMinimalThreshold: FormControl<number | null>;
    frenchMinimalThreshold: FormControl<number | null>;
    italianMinimalThreshold: FormControl<number | null>;
    romanshMinimalThreshold: FormControl<number | null>;
    germanThresholdPercentage: FormControl<number | null>;
    frenchThresholdPercentage: FormControl<number | null>;
    italianThresholdPercentage: FormControl<number | null>;
    romanshThresholdPercentage: FormControl<number | null>;
}

@Component({
    selector: 'apg-committee-type-form',
    templateUrl: './committee-type-form.component.html',
    styleUrl: './committee-type-form.component.scss',
    imports: [ReactiveFormsModule, ObUnsavedChangesDirective, MatFormField, ObErrorMessagesDirective, MatLabel, MatInput, MatError, TranslatePipe],
})
export class CommitteeTypeFormComponent {
    committeeTypeModification = model<CommitteeTypeUpdate>();
    committeeTypeForm: FormGroup<CommitteeTypeForm>;

    minimalFields = ['germanMinimalThreshold', 'frenchMinimalThreshold', 'italianMinimalThreshold', 'romanshMinimalThreshold'];
    percentageFields = ['germanThresholdPercentage', 'frenchThresholdPercentage', 'italianThresholdPercentage', 'romanshThresholdPercentage'];

    constructor(
        protected readonly errorService: ErrorService,
        protected readonly formBuilder: FormBuilder
    ) {
        this.committeeTypeForm = this.createForm();

        const effectRef = effect(() => {
            if ((this.committeeTypeModification() as CommitteeTypeUpdate)?.id) {
                this.committeeTypeForm.patchValue(this.committeeTypeModification()!, {emitEvent: false});
                this.committeeTypeForm.markAllAsTouched();

                this.checkCurrentFormContent();
                effectRef.destroy();
            } else {
                this.committeeTypeForm.markAllAsTouched();
            }
        });

        this.committeeTypeForm.valueChanges.pipe(debounceTime(300), takeUntilDestroyed()).subscribe(() => {
            const formValues = this.committeeTypeForm.getRawValue();
            this.committeeTypeModification.update(value => ({...value, ...(formValues as unknown as CommitteeTypeUpdate)}));
        });

        this.subscribeToMinimalValueChanges();
    }

    private createForm(): FormGroup<CommitteeTypeForm> {
        const formGroup = this.formBuilder.group<CommitteeTypeForm>(
            {
                femaleThreshold: this.formBuilder.control<number | null>(
                    {value: null, disabled: false},
                    {validators: [Validators.min(0), Validators.max(100)]}
                ),
                maleThreshold: this.formBuilder.control<number | null>({value: null, disabled: false}, {validators: [Validators.min(0), Validators.max(100)]}),
                germanMinimalThreshold: this.formBuilder.control<number | null>({value: null, disabled: false}, {validators: [Validators.min(0)]}),
                frenchMinimalThreshold: this.formBuilder.control<number | null>({value: null, disabled: false}, {validators: [Validators.min(0)]}),
                italianMinimalThreshold: this.formBuilder.control<number | null>({value: null, disabled: false}, {validators: [Validators.min(0)]}),
                romanshMinimalThreshold: this.formBuilder.control<number | null>({value: null, disabled: false}, {validators: [Validators.min(0)]}),
                germanThresholdPercentage: this.formBuilder.control<number | null>(
                    {value: null, disabled: false},
                    {validators: [Validators.min(0), Validators.max(100)]}
                ),
                frenchThresholdPercentage: this.formBuilder.control<number | null>(
                    {value: null, disabled: false},
                    {validators: [Validators.min(0), Validators.max(100)]}
                ),
                italianThresholdPercentage: this.formBuilder.control<number | null>(
                    {value: null, disabled: false},
                    {validators: [Validators.min(0), Validators.max(100)]}
                ),
                romanshThresholdPercentage: this.formBuilder.control<number | null>(
                    {value: null, disabled: false},
                    {validators: [Validators.min(0), Validators.max(100)]}
                ),
            },
            {
                validators: multiPercentageSumValidator({
                    groupGender: ['femaleThreshold', 'maleThreshold'],
                    groupLanguage: ['germanThresholdPercentage', 'frenchThresholdPercentage', 'italianThresholdPercentage', 'romanshThresholdPercentage'],
                }),
            }
        );

        return formGroup;
    }

    private subscribeToMinimalValueChanges() {
        this.committeeTypeForm.valueChanges.subscribe(values => {
            const minimalHasValue =
                (values.germanMinimalThreshold != null && values.germanMinimalThreshold >= 0) ||
                (values.frenchMinimalThreshold != null && values.frenchMinimalThreshold >= 0) ||
                (values.italianMinimalThreshold != null && values.italianMinimalThreshold >= 0) ||
                (values.romanshMinimalThreshold != null && values.romanshMinimalThreshold >= 0);

            const percentageHasValue =
                (values.germanThresholdPercentage != null && values.germanThresholdPercentage >= 0) ||
                (values.frenchThresholdPercentage != null && values.frenchThresholdPercentage >= 0) ||
                (values.italianThresholdPercentage != null && values.italianThresholdPercentage >= 0) ||
                (values.romanshThresholdPercentage != null && values.romanshThresholdPercentage >= 0);

            this.enableOrDisbleLanguageFields(minimalHasValue, percentageHasValue);
        });
    }

    private checkCurrentFormContent() {
        const minimalHasValue =
            (this.committeeTypeForm.controls.germanMinimalThreshold.value != null && this.committeeTypeForm.controls.germanMinimalThreshold.value >= 0) ||
            (this.committeeTypeForm.controls.frenchMinimalThreshold.value != null && this.committeeTypeForm.controls.frenchMinimalThreshold.value >= 0) ||
            (this.committeeTypeForm.controls.italianMinimalThreshold.value != null && this.committeeTypeForm.controls.italianMinimalThreshold.value >= 0) ||
            (this.committeeTypeForm.controls.romanshMinimalThreshold.value != null && this.committeeTypeForm.controls.romanshMinimalThreshold.value >= 0);

        const percentageHasValue =
            (this.committeeTypeForm.controls.germanThresholdPercentage.value != null && this.committeeTypeForm.controls.germanThresholdPercentage.value >= 0) ||
            (this.committeeTypeForm.controls.frenchThresholdPercentage.value != null && this.committeeTypeForm.controls.frenchThresholdPercentage.value >= 0) ||
            (this.committeeTypeForm.controls.italianThresholdPercentage.value != null &&
                this.committeeTypeForm.controls.italianThresholdPercentage.value >= 0) ||
            (this.committeeTypeForm.controls.romanshThresholdPercentage.value != null && this.committeeTypeForm.controls.romanshThresholdPercentage.value >= 0);

        this.enableOrDisbleLanguageFields(minimalHasValue, percentageHasValue);
    }

    private enableOrDisbleLanguageFields(minimalHasValue: boolean, percentageHasValue: boolean) {
        if (minimalHasValue) {
            this.percentageFields.forEach(field => {
                this.committeeTypeForm.get(field)!.disable({emitEvent: false});
            });
        } else {
            this.percentageFields.forEach(field => {
                this.committeeTypeForm.get(field)!.enable({emitEvent: false});
            });
        }

        if (percentageHasValue) {
            this.minimalFields.forEach(field => {
                this.committeeTypeForm.get(field)!.disable({emitEvent: false});
            });
        } else {
            this.minimalFields.forEach(field => {
                this.committeeTypeForm.get(field)!.enable({emitEvent: false});
            });
        }
    }
}
