import {CdkTextareaAutosize} from '@angular/cdk/text-field';
import {Component, Input, computed, effect, model} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {FormBuilder, FormControl, FormGroup, Validators, ReactiveFormsModule, UntypedFormArray} from '@angular/forms';
import {MatIconButton} from '@angular/material/button';
import {MatDatepickerInput, MatDatepickerToggle, MatDatepicker} from '@angular/material/datepicker';
import {MatIcon} from '@angular/material/icon';
import {MatFormField, MatLabel, MatInput, MatSuffix, MatError} from '@angular/material/input';
import {MatSelectChange, MatSelect, MatOption} from '@angular/material/select';
import {MatTooltip} from '@angular/material/tooltip';
import {AppointmentDecisionCreate} from '@api/AppointmentDecisionCreate';
import {AppointmentDecisionUpdate} from '@api/AppointmentDecisionUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {
    ObUnsavedChangesDirective,
    ObErrorMessagesDirective,
    ObDropZoneComponent,
    ObNotificationService,
    ObIUploadEvent,
    ObEUploadEventType,
} from '@oblique/oblique';
import {today} from '@shared/date-util';
import {ErrorService} from '@shared/error-service.service';
import {conditionalValidator} from '@shared/form-validators/conditional.validator';
import {MasterDataService} from '@shared/master-data.service';
import {debounceTime, merge} from 'rxjs';
import {ConfigsService} from '../../../../../configs.service';
import {AppointmentDecisionService} from '../../appointment-decision.service';

/* eslint-disable dot-notation */
@Component({
    selector: 'apg-appointment-decision-data-form',
    templateUrl: './appointment-decision-data-form.component.html',
    styleUrl: './appointment-decision-data-form.component.scss',
    imports: [
        ReactiveFormsModule,
        ObUnsavedChangesDirective,
        MatFormField,
        ObErrorMessagesDirective,
        MatLabel,
        MatInput,
        MatDatepickerInput,
        MatDatepickerToggle,
        MatSuffix,
        MatDatepicker,
        MatError,
        MatSelect,
        MatOption,
        MatIcon,
        MatIconButton,
        MatTooltip,
        CdkTextareaAutosize,
        ObDropZoneComponent,
        TranslatePipe,
    ],
})
export class AppointmentDecisionDataFormComponent {
    @Input() isUpdateMode = false;
    appointmentDecisionForm = this.createForm();
    appointmentDecisionTypes = computed(() => this.masterDataService.appointmentDecisionTypes());
    appointmentDecisionLinkTypes = computed(() => this.masterDataService.appointmentDecisionLinkTypes());
    appointmentDecisionModification = model<AppointmentDecisionCreate | AppointmentDecisionUpdate>();

    isTypeDecisionFederalCouncilSelected = computed(
        () => this.selectedAppointmentDecisionTypeId() === this.configsService.frontendConfig.entityIds.appointmentDecisionType.decisionFederalCouncilId
    );
    isInstitutionSelected = computed(
        () => this.selectedAppointmentDecisionTypeId() === this.configsService.frontendConfig.entityIds.appointmentDecisionType.institutionId
    );
    isReportSelected = computed(
        () => this.selectedAppointmentDecisionTypeId() === this.configsService.frontendConfig.entityIds.appointmentDecisionType.reportId
    );
    isOtherSelected = computed(() => this.selectedAppointmentDecisionTypeId() === this.configsService.frontendConfig.entityIds.appointmentDecisionType.otherId);
    isRegulationsSelected = computed(
        () => this.selectedAppointmentDecisionTypeId() === this.configsService.frontendConfig.entityIds.appointmentDecisionType.regulationsId
    );

    files?: File[] = [];
    documentsForm!: UntypedFormArray;

    private readonly selectedAppointmentDecisionTypeId = toSignal(this.appointmentDecisionForm.controls.appointmentDecisionTypeId.valueChanges);

    private get isOriginalSelected() {
        return this.documentsForm.controls.some(control => !!(control as FormGroup).controls.isOriginal?.value);
    }

    constructor(
        protected readonly masterDataService: MasterDataService,
        protected readonly errorService: ErrorService,
        protected readonly configsService: ConfigsService,
        private readonly fb: FormBuilder,
        private readonly notificationService: ObNotificationService,
        private readonly appointmentDecisionService: AppointmentDecisionService
    ) {
        this.documentsForm = new UntypedFormArray([]);
        merge(this.appointmentDecisionForm.valueChanges, this.documentsForm.valueChanges)
            .pipe(debounceTime(300), takeUntilDestroyed())
            .subscribe(() => {
                const formValues = {...this.appointmentDecisionForm.getRawValue(), documents: this.documentsForm.getRawValue()};
                this.appointmentDecisionModification.update(
                    value =>
                        ({...value, ...(formValues as AppointmentDecisionCreate | AppointmentDecisionUpdate)}) as
                            | AppointmentDecisionCreate
                            | AppointmentDecisionUpdate
                );
            });

        effect(() => {
            if (this.isInstitutionSelected()) {
                for (let i = 0; i < this.documentsForm.controls.length; i++) {
                    const group = this.documentsForm.controls[i] as FormGroup;
                    if (!group.contains('isOriginal')) {
                        group.addControl('isOriginal', this.fb.control(!this.isOriginalSelected, {validators: [Validators.required]}));
                    }
                }
            } else {
                for (let i = 0; i < this.documentsForm.controls.length; i++) {
                    const group = this.documentsForm.controls[i] as FormGroup;
                    if (group.contains('isOriginal')) {
                        group.removeControl('isOriginal');
                    }
                }
            }
        });
        const effectRef = effect(() => {
            const modification = this.appointmentDecisionModification();
            if (!modification) {
                return;
            }

            this.appointmentDecisionForm.reset(modification);

            if (this.isUpdateMode) {
                this.appointmentDecisionForm.controls.appointmentDecisionTypeId.disable();
                this.setDecisionTypeInternal(this.appointmentDecisionForm.controls.appointmentDecisionTypeId.value);
                this.rebuildDocumentsForm(modification as AppointmentDecisionUpdate);
            } else {
                this.appointmentDecisionForm.controls.appointmentDecisionTypeId.markAllAsTouched();
                this.appointmentDecisionForm.controls.appointmentDecisionTypeId.updateValueAndValidity();
            }

            effectRef.destroy();
        });
    }

    public rebuildDocumentsForm(appointmentDecisionUpdate: AppointmentDecisionUpdate) {
        this.files = [];
        this.documentsForm.clear();
        appointmentDecisionUpdate?.documents.forEach(element => {
            this.addExistingToDocumentForm(element.displayName, element.id, element.languageId, element.isOriginal, element.documentStorageId);
            this.files?.push({} as File);
        });
    }

    public setDecisionLinkType() {
        this.appointmentDecisionForm.controls.link.markAsTouched();
        this.appointmentDecisionForm.controls.link.updateValueAndValidity();
    }

    public setDecisionType(event: MatSelectChange) {
        this.setDecisionTypeInternal(event.value);
    }

    public setIsOriginal(event: MatSelectChange, selectedIndex: number) {
        if (event.value) {
            for (let i = 0; i < this.documentsForm.controls.length; i++) {
                if (selectedIndex !== i) {
                    (this.documentsForm.controls[i] as FormGroup).controls['isOriginal'].setValue(false);
                }
            }
        }
    }

    public clearLanguageDuplicates(event: MatSelectChange, selectedIndex: number) {
        if (event.value) {
            for (let i = 0; i < this.documentsForm.controls.length; i++) {
                if (selectedIndex !== i && (this.documentsForm.controls[i] as FormGroup).controls['languageId'].value === event.value) {
                    (this.documentsForm.controls[i] as FormGroup).controls['languageId'].setValue('');
                }
            }
        }
    }

    public uploadEvent(event: ObIUploadEvent): void {
        if (event.type === ObEUploadEventType.CHOSEN) {
            if (event.files.length + this.files!.length > 4) {
                this.notificationService.error('appointmentDecision.files.count.error');
                return;
            }
            event.files.forEach(element => {
                this.files?.push(element as File);
                this.addNewToDocumentForm(element as File);
            });
        }
    }

    public downloadDocument(i: number) {
        const documentFormGroup = this.documentsForm.controls[i] as FormGroup;

        this.appointmentDecisionService.downloadFile(documentFormGroup.get('documentStorageId')!.value).subscribe({
            next: blob => {
                const suggestedFileName = documentFormGroup.get('displayName')!.value;
                const url = window.URL.createObjectURL(blob);
                const anchor = document.createElement('a');
                anchor.href = url;
                anchor.download = suggestedFileName;
                anchor.click();
                window.URL.revokeObjectURL(url);
            },
            error: error => {
                console.error('Download failed:', error);
            },
        });
    }

    public removeUploadedDocument(i: number) {
        this.documentsForm.removeAt(i);
        this.files!.splice(i, 1);
    }

    public addNewToDocumentForm(element: File) {
        const formGroup = this.fb.group({
            id: '',
            displayName: this.fb.control(element.name, {validators: [Validators.required]}),
            ...(this.isInstitutionSelected() ? {isOriginal: this.fb.control(!this.isOriginalSelected, {validators: [Validators.required]})} : {}),
            languageId: this.fb.control(
                element.name.toUpperCase().includes('DE.') && !this.hasLanguageAlready(this.configsService.frontendConfig.entityIds.language.germanLanguageId)
                    ? this.configsService.frontendConfig.entityIds.language.germanLanguageId
                    : element.name.toUpperCase().includes('IT.') &&
                        !this.hasLanguageAlready(this.configsService.frontendConfig.entityIds.language.italianLanguageId)
                      ? this.configsService.frontendConfig.entityIds.language.italianLanguageId
                      : element.name.toUpperCase().includes('FR.') &&
                          !this.hasLanguageAlready(this.configsService.frontendConfig.entityIds.language.frenchLanguageId)
                        ? this.configsService.frontendConfig.entityIds.language.frenchLanguageId
                        : element.name.toUpperCase().includes('RM.') &&
                            !this.hasLanguageAlready(this.configsService.frontendConfig.entityIds.language.romanshLanguageId)
                          ? this.configsService.frontendConfig.entityIds.language.romanshLanguageId
                          : '',
                {validators: [Validators.required]}
            ),
            file: element,
        });

        this.documentsForm.push(formGroup);
    }

    private setDecisionTypeInternal(type: string) {
        if (this.isDecisionOrInstitution(type)) {
            this.appointmentDecisionForm.controls.appointmentDecisionLinkTypeId.setValue(
                this.configsService.frontendConfig.entityIds.appointmentDecisionLinkType.exeLinkTypeId
            );
            this.appointmentDecisionForm.controls.appointmentDecisionLinkTypeId.disable();
        } else if (this.isReport(type)) {
            this.appointmentDecisionForm.controls.appointmentDecisionLinkTypeId.setValue(
                this.configsService.frontendConfig.entityIds.appointmentDecisionLinkType.standardLinkTypeId
            );
            this.appointmentDecisionForm.controls.appointmentDecisionLinkTypeId.disable();
        } else {
            this.appointmentDecisionForm.controls.appointmentDecisionLinkTypeId.enable();
        }
        this.appointmentDecisionForm.controls.link.markAsTouched();
        this.appointmentDecisionForm.controls.link.updateValueAndValidity();
        this.appointmentDecisionForm.controls.text.markAsTouched();
        this.appointmentDecisionForm.controls.text.updateValueAndValidity();
    }

    private addExistingToDocumentForm(displayName: string, id: string, languageId: string, isOriginal: boolean, documentStorageId?: string) {
        const formGroup = this.fb.group({
            id,
            documentStorageId,
            displayName: this.fb.control(displayName, {validators: [Validators.required]}),
            ...(this.isInstitutionSelected() ? {isOriginal: this.fb.control(isOriginal, {validators: [Validators.required]})} : {}),
            languageId: this.fb.control(languageId, {validators: [Validators.required]}),
            file: {name: displayName} as File,
        });

        this.documentsForm.push(formGroup);
    }

    private hasLanguageAlready(languageId: string): boolean {
        return this.documentsForm.controls.some(control => (control as FormGroup).get('languageId')?.value === languageId);
    }

    private isDecisionOrInstitution(selectedValue: string): boolean {
        return [
            this.configsService.frontendConfig.entityIds.appointmentDecisionType.decisionFederalCouncilId,
            this.configsService.frontendConfig.entityIds.appointmentDecisionType.institutionId,
        ].some(y => y === selectedValue);
    }

    private isReport(selectedValue: string): boolean {
        return this.configsService.frontendConfig.entityIds.appointmentDecisionType.reportId === selectedValue;
    }

    private createForm() {
        const form = new FormGroup({
            appointmentDecisionDate: new FormControl(today(), {nonNullable: true}),
            appointmentDecisionLinkTypeId: new FormControl('', {nonNullable: false}),
            appointmentDecisionTypeId: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
            text: new FormControl('', {nonNullable: false, validators: [Validators.maxLength(2000)]}),
            link: new FormControl('', {nonNullable: false, validators: [Validators.maxLength(250)]}),
        });

        form.controls.appointmentDecisionDate.setValidators([Validators.required]);

        form.controls.text.setValidators([
            conditionalValidator(
                () =>
                    this.appointmentDecisionForm.controls.appointmentDecisionTypeId.value ===
                    this.configsService.frontendConfig.entityIds.appointmentDecisionType.decisionFederalCouncilId,
                Validators.required
            ),
        ]);

        form.controls.link.setValidators([
            conditionalValidator(
                () =>
                    this.appointmentDecisionForm.controls.appointmentDecisionLinkTypeId.value ===
                    this.configsService.frontendConfig.entityIds.appointmentDecisionLinkType.exeLinkTypeId,
                Validators.pattern(/^\d{4}\.\d{4}$/)
            ),
            conditionalValidator(
                () =>
                    this.appointmentDecisionForm.controls.appointmentDecisionLinkTypeId.value ===
                    this.configsService.frontendConfig.entityIds.appointmentDecisionLinkType.standardLinkTypeId,
                Validators.pattern(/(https:\/\/www\.|http:\/\/www\.|https:\/\/|http:\/\/)?[a-zA-Z0-9]{2,}(\.[a-zA-Z0-9]{2,})(\.[a-zA-Z0-9]{2,})/)
            ),
        ]);
        return form;
    }
}
