import {Component, computed, effect, input, model} from '@angular/core';
import {toSignal} from '@angular/core/rxjs-interop';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatInput} from '@angular/material/input';
import {MatFormField, MatLabel, MatSelect, MatOption, MatError} from '@angular/material/select';
import {MatTooltip} from '@angular/material/tooltip';
import {FormLettersSenderCreate} from '@api/FormLettersSenderCreate';
import {FormLettersSenderUpdate} from '@api/FormLettersSenderUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObUnsavedChangesDirective, ObErrorMessagesDirective, ObDropZoneComponent, ObIUploadEvent, ObEUploadEventType} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {TEL_PATTERN} from '@shared/form-validators/validation-patterns';
import {isEmptyId} from '@shared/id-util';
import {MasterDataService} from '@shared/master-data.service';

interface FormLettersSenderForm {
    description: FormControl<string>;
    surname: FormControl<string>;
    givenName: FormControl<string>;
    senderFunctionId: FormControl<string>;
    departmentId: FormControl<string>;
    officeId: FormControl<string>;
    streetGerman: FormControl<string>;
    streetFrench: FormControl<string>;
    streetItalian: FormControl<string>;
    streetRomansh: FormControl<string>;
    zip: FormControl<string>;
    cityGerman: FormControl<string>;
    cityFrench: FormControl<string>;
    cityItalian: FormControl<string>;
    cityRomansh: FormControl<string>;
    phone: FormControl<string | undefined>;
    email: FormControl<string | undefined>;
    website: FormControl<string | undefined>;
}

@Component({
    selector: 'apg-form-letters-sender-data-form',
    templateUrl: './form-letters-sender-data-form.component.html',
    styleUrl: './form-letters-sender-data-form.component.scss',
    imports: [
        ReactiveFormsModule,
        ObUnsavedChangesDirective,
        MatFormField,
        ObErrorMessagesDirective,
        MatLabel,
        MatInput,
        MatSelect,
        MatOption,
        MatError,
        MatIcon,
        MatIconButton,
        MatTooltip,
        ObDropZoneComponent,
        TranslatePipe,
    ],
})
export class FormLettersSenderDataFormComponent {
    isUpdateMode = input(false);
    senderModification = model<FormLettersSenderCreate | FormLettersSenderUpdate>();
    senderForm = this.createForm();

    signature: File | undefined = undefined;
    signatureFileName = '';
    readonly acceptedSignatureFormats = ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp', '.svg', '.tif', '.tiff'];
    readonly maxSignatureFileSizeInMB = 5;

    protected readonly senderFunctions = computed(() => this.masterDataService.formLetterSenderFunctions());
    protected readonly departments = computed(() => this.masterDataService.departments());
    protected readonly departmentOffices = computed(() => {
        const offices = this.masterDataService.offices();
        return offices.filter(office => office.departmentId === this.selectedDepartmentId());
    });

    private readonly selectedDepartmentId = toSignal(this.senderForm.controls.departmentId.valueChanges);

    constructor(
        protected readonly errorService: ErrorService,
        private readonly formBuilder: FormBuilder,
        private readonly masterDataService: MasterDataService
    ) {
        const effectRef = effect(() => {
            if (!this.isUpdateMode() && !isEmptyId((this.senderModification() as FormLettersSenderCreate)?.departmentId)) {
                this.senderForm.controls.departmentId.patchValue(this.senderModification()!.departmentId);
                this.senderForm.controls.departmentId.disable();

                effectRef.destroy();
            }

            if (this.isUpdateMode()) {
                const sender = this.senderModification() as FormLettersSenderUpdate;
                if (sender) {
                    this.senderForm.patchValue(sender);
                    this.senderForm.markAllAsTouched();
                    this.signatureFileName = sender.signatureFileName ?? '';

                    if (!sender.canEditDepartment) {
                        this.senderForm.controls.departmentId.disable();
                    }

                    effectRef.destroy();
                }
            }
        });
    }

    uploadEvent(event: ObIUploadEvent): void {
        if (event.type === ObEUploadEventType.CHOSEN && event.files && event.files.length > 0) {
            const file = event.files[0] as File;
            this.signature = file;
            this.signatureFileName = file.name;
            this.senderForm.markAsDirty();
        }
    }

    clearSignature(): void {
        this.signature = undefined;
        this.signatureFileName = '';
        this.senderForm.markAsDirty();
    }

    private createForm(): FormGroup<FormLettersSenderForm> {
        return this.formBuilder.group({
            description: this.formBuilder.control<string | null>(null, {validators: [Validators.required, Validators.maxLength(150)]}),
            surname: this.formBuilder.control<string | null>(null, {
                validators: [Validators.required, Validators.maxLength(150)],
            }),
            givenName: this.formBuilder.control<string | null>(null, {
                validators: [Validators.required, Validators.maxLength(150)],
            }),
            senderFunctionId: this.formBuilder.control<string | null>(null, {validators: [Validators.required]}),
            departmentId: this.formBuilder.control<string | null>(null, {validators: [Validators.required]}),
            officeId: this.formBuilder.control<string | null>(null, {validators: [Validators.required]}),
            streetGerman: this.formBuilder.control<string | null>(null, {
                validators: [Validators.required, Validators.maxLength(100)],
            }),
            streetFrench: this.formBuilder.control<string | null>(null, {
                validators: [Validators.required, Validators.maxLength(100)],
            }),
            streetItalian: this.formBuilder.control<string | null>(null, {
                validators: [Validators.required, Validators.maxLength(100)],
            }),
            streetRomansh: this.formBuilder.control<string | null>(null, {
                validators: [Validators.required, Validators.maxLength(100)],
            }),
            zip: this.formBuilder.control<string | null>(null, {
                validators: [Validators.required, Validators.maxLength(10)],
            }),
            cityGerman: this.formBuilder.control<string | null>(null, {
                validators: [Validators.required, Validators.maxLength(100)],
            }),
            cityFrench: this.formBuilder.control<string | null>(null, {
                validators: [Validators.required, Validators.maxLength(100)],
            }),
            cityItalian: this.formBuilder.control<string | null>(null, {
                validators: [Validators.required, Validators.maxLength(100)],
            }),
            cityRomansh: this.formBuilder.control<string | null>(null, {
                validators: [Validators.required, Validators.maxLength(100)],
            }),
            phone: this.formBuilder.control<string | null>(null, {
                validators: [Validators.pattern(TEL_PATTERN), Validators.maxLength(20)],
            }),
            email: this.formBuilder.control<string | null>(null, {
                validators: [Validators.email, Validators.maxLength(150)],
            }),
            website: this.formBuilder.control<string | null>(null, {
                validators: [Validators.maxLength(500)],
            }),
        } as FormLettersSenderForm);
    }
}
