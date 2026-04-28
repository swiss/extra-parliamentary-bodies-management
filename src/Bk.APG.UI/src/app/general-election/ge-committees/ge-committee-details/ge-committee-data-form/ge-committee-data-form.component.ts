import {CdkTextareaAutosize} from '@angular/cdk/text-field';
import {Component, computed, effect, EventEmitter, model, Output} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatError, MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {GeneralElectionCommitteeUpdate} from '@api/GeneralElectionCommitteeUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObErrorMessagesDirective, ObUnsavedChangesDirective} from '@oblique/oblique';
import {today} from '@shared/date-util';
import {ErrorService} from '@shared/error-service.service';
import {conditionalValidator} from '@shared/form-validators/conditional.validator';
import {Comparison, rangeValidator} from '@shared/form-validators/range.validator';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {MembersTooltipContentComponent} from '@shared/members-tooltip-content/members-tooltip-content.component';
import {debounceTime} from 'rxjs';
import {AuthService} from '../../../../auth/auth.service';
import {ConfigsService} from '../../../../configs.service';

@Component({
    selector: 'apg-ge-committee-data-form',
    templateUrl: './ge-committee-data-form.component.html',
    styleUrl: './ge-committee-data-form.component.scss',
    imports: [
        ReactiveFormsModule,
        ObUnsavedChangesDirective,
        MatFormField,
        ObErrorMessagesDirective,
        MatLabel,
        MatInput,
        CdkTextareaAutosize,
        MatError,
        MatSelect,
        MatOption,
        MatDatepickerInput,
        MatDatepickerToggle,
        MatSuffix,
        MatDatepicker,
        TranslatePipe,
        HelpTooltipComponent,
        MembersTooltipContentComponent,
    ],
})
export class GeneralElectionCommitteeDataFormComponent {
    @Output() readonly isExtraParliamentaryCommmission: EventEmitter<boolean> = new EventEmitter();
    committeeModification = model<GeneralElectionCommitteeUpdate>();

    committeeForm = this.createForm();
    canEditAll = false;

    departmentOffices = computed(() => {
        const offices = this.masterDataService.offices();
        return offices.filter(office => office.departmentId === this.selectedDepartmentId());
    });

    vacanciesInCurrentTermOfOffice = computed(() => {
        if (this.selectedCommitteeLevelId() !== this.configsService.frontendConfig.entityIds.committeeLevel.federalCouncilId) {
            return null;
        }

        const membersCount = this.committeeModification()?.membersCount ?? 0;
        const membersCountAboveMinimum = membersCount - (this.minimmalMembers() ?? 0);
        return membersCountAboveMinimum >= 0 ? 0 : membersCountAboveMinimum * -1;
    });

    protected isAuthorityOrAdministration = computed(
        () =>
            this.selectedCommitteeTypeId() === this.configsService.frontendConfig.entityIds.committeeType.authorityId ||
            this.selectedCommitteeTypeId() === this.configsService.frontendConfig.entityIds.committeeType.administrationId
    );
    protected isManagementCommittee = computed(
        () => this.selectedCommitteeTypeId() === this.configsService.frontendConfig.entityIds.committeeType.managementId
    );

    private readonly isAdmin = toSignal(this.authService.isAdmin$);

    private readonly selectedCommitteeTypeId = toSignal(this.committeeForm.controls.committeeTypeId.valueChanges);
    private readonly selectedDepartmentId = toSignal(this.committeeForm.controls.departmentId.valueChanges);
    private readonly selectedCommitteeLevelId = toSignal(this.committeeForm.controls.levelId.valueChanges);
    private readonly minimmalMembers = toSignal(this.committeeForm.controls.minimalMembers.valueChanges);
    private readonly maximalMembers = toSignal(this.committeeForm.controls.maximalMembers.valueChanges);
    private readonly beginDate = toSignal(this.committeeForm.controls.beginDate.valueChanges);
    private readonly endDate = toSignal(this.committeeForm.controls.endDate.valueChanges);
    private readonly additionalAuthorityMembers = toSignal(this.committeeForm.controls.additionalAuthorityMembers.valueChanges);

    constructor(
        protected readonly masterDataService: MasterDataService,
        protected readonly errorService: ErrorService,
        private readonly authService: AuthService,
        private readonly configsService: ConfigsService
    ) {
        const effectRef = effect(() => {
            if (this.committeeModification()?.id) {
                this.committeeForm.patchValue(this.committeeModification()!);
                this.setEditPermissions();
                effectRef.destroy();
            }
        });

        effect(() => {
            if (this.minimmalMembers() !== undefined && this.maximalMembers() !== undefined) {
                this.committeeForm.controls.minimalMembers.updateValueAndValidity();
                this.committeeForm.controls.maximalMembers.updateValueAndValidity();
            }
        });

        effect(() => {
            if (this.beginDate() !== undefined && this.endDate() !== undefined) {
                this.committeeForm.controls.beginDate.updateValueAndValidity();
                this.committeeForm.controls.endDate.updateValueAndValidity();
            }
        });

        effect(() => {
            const isAdmin = this.isAdmin();
            if (isAdmin) {
                this.committeeForm.controls.beginDate.enable();
            }
        });

        this.committeeForm.valueChanges.pipe(debounceTime(300), takeUntilDestroyed()).subscribe(() => {
            const formValues = this.committeeForm.getRawValue(); // getRawValue because we need the disabled values as well
            this.committeeModification.update(value => ({...value, ...(formValues as GeneralElectionCommitteeUpdate)}) as GeneralElectionCommitteeUpdate);
        });

        effect(() => {
            const committeeTypeId = this.selectedCommitteeTypeId();

            if (!committeeTypeId) {
                return;
            }

            if (committeeTypeId === this.configsService.frontendConfig.entityIds.committeeType.authorityId) {
                this.setAuthorityCommitteeFields();
            } else if (committeeTypeId === this.configsService.frontendConfig.entityIds.committeeType.administrationId) {
                this.setAdminisistrationCommitteeFields();
            } else if (committeeTypeId === this.configsService.frontendConfig.entityIds.committeeType.managementId) {
                this.setManagementCommitteeFields();
            } else if (committeeTypeId === this.configsService.frontendConfig.entityIds.committeeType.federalAgenciesId) {
                this.setFederalAgenciesCommitteeFields();
            } else if (committeeTypeId === this.configsService.frontendConfig.entityIds.committeeType.federalAgenciesCrossBorderId) {
                this.setFederalAgenciesCrossBorderCommitteeFields();
            }

            this.updateValidityForForm();
        });

        this.committeeForm.controls.federalLawEstablishment.valueChanges
            .pipe(takeUntilDestroyed())
            .subscribe(() => this.committeeForm.controls.legalBase.updateValueAndValidity({emitEvent: false}));

        effect(() => {
            if (!this.canEditAll) {
                return;
            }

            if (this.additionalAuthorityMembers()) {
                this.committeeForm.controls.linkAuthorityWebsite.enable();
            } else {
                this.committeeForm.controls.linkAuthorityWebsite.disable();
                this.committeeForm.controls.linkAuthorityWebsite.setValue(null);
            }
        });

        effect(() => {
            if (this.selectedCommitteeLevelId() === this.configsService.frontendConfig.entityIds.committeeLevel.federalCouncilId) {
                this.committeeForm.controls.minimalMembers.addValidators([Validators.required]);
                this.committeeForm.controls.maximalMembers.addValidators([Validators.required]);
            } else {
                this.committeeForm.controls.minimalMembers.removeValidators([Validators.required]);
                this.committeeForm.controls.maximalMembers.removeValidators([Validators.required]);
            }

            this.committeeForm.controls.minimalMembers.updateValueAndValidity();
            this.committeeForm.controls.maximalMembers.updateValueAndValidity();
        });
    }

    private setEditPermissions() {
        this.canEditAll = this.committeeModification()!.canEditAll;
        if (!this.canEditAll) {
            this.committeeForm.disable();
            if (!this.committeeModification()!.canEditLegalbase) {
                this.committeeForm.controls.legalBase.disable();
            } else {
                this.committeeForm.controls.legalBase.enable();
            }
        }
        if (!this.committeeModification()!.canEditDepartment) {
            this.committeeForm.controls.departmentId.disable();
        } else {
            this.committeeForm.controls.departmentId.enable();
        }
    }

    private createForm() {
        const form = new FormGroup({
            descriptionGerman: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(1), Validators.maxLength(500)]}),
            descriptionFrench: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(1), Validators.maxLength(500)]}),
            descriptionItalian: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(1), Validators.maxLength(500)]}),
            descriptionRomansh: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(1), Validators.maxLength(500)]}),
            levelId: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
            departmentId: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
            officeId: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
            committeeTypeId: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
            legalFormId: new FormControl<string | undefined>({value: undefined, disabled: false}),
            oldLegalForm: new FormControl<string | undefined>({disabled: true, value: undefined}),
            legalBase: new FormControl<string | undefined>({value: undefined, disabled: false}, {validators: [Validators.maxLength(2000)]}),
            federalLawEstablishment: new FormControl<boolean | null>(null),
            selfOrganized: new FormControl<boolean | null>(null),
            supervisionDuty: new FormControl<boolean | null>(null),
            marketOrientated: new FormControl<boolean | null>(null),
            beginDate: new FormControl<Date>({value: today(), disabled: true}, {nonNullable: true, validators: [Validators.required]}),
            endDate: new FormControl<Date | undefined>({value: undefined, disabled: false}),
            termOfOfficeId: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
            minimalMembers: new FormControl<number | undefined>({value: undefined, disabled: false}),
            maximalMembers: new FormControl<number | undefined>({value: undefined, disabled: false}),
            additionalAuthorityMembers: new FormControl<boolean | null>(null, {validators: [Validators.required]}),
            linkAuthorityWebsite: new FormControl<string | undefined>({value: undefined, disabled: true}, {validators: [Validators.maxLength(500)]}),
            federalInstitution: new FormControl<boolean | null>(null),
            linkHomepageGerman: new FormControl<string | undefined>({value: undefined, disabled: false}, {validators: [Validators.maxLength(500)]}),
            linkHomepageFrench: new FormControl<string | undefined>({value: undefined, disabled: false}, {validators: [Validators.maxLength(500)]}),
            linkHomepageItalian: new FormControl<string | undefined>({value: undefined, disabled: false}, {validators: [Validators.maxLength(500)]}),
            linkHomepageRomansh: new FormControl<string | undefined>({value: undefined, disabled: false}, {validators: [Validators.maxLength(500)]}),
        });

        form.controls.minimalMembers.addValidators(rangeValidator(form.controls.maximalMembers, Comparison.LowerThanEqual));
        form.controls.maximalMembers.addValidators(rangeValidator(form.controls.minimalMembers, Comparison.GreaterThanEqual));
        form.controls.beginDate.addValidators(rangeValidator(form.controls.endDate, Comparison.LowerThanEqual));
        form.controls.endDate.addValidators(rangeValidator(form.controls.beginDate, Comparison.GreaterThanEqual));

        form.controls.federalLawEstablishment.setValidators([
            conditionalValidator(
                () =>
                    form.controls.committeeTypeId.value === this.configsService.frontendConfig.entityIds.committeeType.administrationId ||
                    form.controls.committeeTypeId.value === this.configsService.frontendConfig.entityIds.committeeType.authorityId,
                Validators.required
            ),
        ]);

        form.controls.legalBase.setValidators([conditionalValidator(() => form.controls.federalLawEstablishment.value === true, Validators.required)]);

        form.controls.supervisionDuty.setValidators([
            conditionalValidator(
                () =>
                    form.controls.committeeTypeId.value === this.configsService.frontendConfig.entityIds.committeeType.administrationId ||
                    form.controls.committeeTypeId.value === this.configsService.frontendConfig.entityIds.committeeType.authorityId,
                Validators.required
            ),
        ]);

        form.controls.marketOrientated.setValidators([
            conditionalValidator(
                () =>
                    form.controls.committeeTypeId.value === this.configsService.frontendConfig.entityIds.committeeType.administrationId ||
                    form.controls.committeeTypeId.value === this.configsService.frontendConfig.entityIds.committeeType.authorityId,
                Validators.required
            ),
        ]);

        form.controls.federalInstitution.setValidators([
            conditionalValidator(
                () =>
                    form.controls.committeeTypeId.value === this.configsService.frontendConfig.entityIds.committeeType.federalAgenciesId ||
                    form.controls.committeeTypeId.value === this.configsService.frontendConfig.entityIds.committeeType.managementId,
                Validators.required
            ),
        ]);

        form.controls.legalFormId.setValidators([
            conditionalValidator(
                () =>
                    form.controls.committeeTypeId.value === this.configsService.frontendConfig.entityIds.committeeType.managementId ||
                    form.controls.committeeTypeId.value === this.configsService.frontendConfig.entityIds.committeeType.federalAgenciesId,
                Validators.required
            ),
        ]);

        return form;
    }

    // handling Behördenkommissionen
    private setAuthorityCommitteeFields() {
        this.emitExtraParliamentaryCommmission(true);

        this.committeeForm.controls.levelId.setValue(this.configsService.frontendConfig.entityIds.committeeLevel.federalCouncilId);
        this.committeeForm.controls.levelId.disable();

        if (this.canEditAll) {
            this.committeeForm.controls.marketOrientated.enable();
            this.committeeForm.controls.additionalAuthorityMembers.enable();
        }

        this.committeeForm.controls.federalInstitution.setValue(null);
        this.committeeForm.controls.federalInstitution.disable();

        this.committeeForm.controls.federalLawEstablishment.setValue(true);
        this.committeeForm.controls.federalLawEstablishment.disable();

        this.committeeForm.controls.legalFormId.setValue(null);
        this.committeeForm.controls.legalFormId.disable();

        this.updateValidityForForm();
    }

    // handling Verwaltungskommissionen
    private setAdminisistrationCommitteeFields() {
        this.emitExtraParliamentaryCommmission(true);

        this.committeeForm.controls.levelId.setValue(this.configsService.frontendConfig.entityIds.committeeLevel.federalCouncilId);
        this.committeeForm.controls.levelId.disable();

        this.committeeForm.controls.federalInstitution.setValue(null);
        this.committeeForm.controls.federalInstitution.disable();

        this.committeeForm.controls.legalFormId.setValue(null);
        this.committeeForm.controls.legalFormId.disable();

        if (this.canEditAll) {
            this.committeeForm.controls.federalLawEstablishment.enable();
            this.committeeForm.controls.additionalAuthorityMembers.enable();
            this.committeeForm.controls.marketOrientated.enable();
        }

        this.updateValidityForForm();
    }

    // handling Leitungsorgane
    private setManagementCommitteeFields() {
        this.emitExtraParliamentaryCommmission(false);

        this.committeeForm.controls.marketOrientated.setValue(null);
        this.committeeForm.controls.marketOrientated.disable();

        if (this.canEditAll) {
            this.committeeForm.controls.levelId.enable();
            this.committeeForm.controls.federalInstitution.enable();
            this.committeeForm.controls.federalLawEstablishment.enable();
            this.committeeForm.controls.legalFormId.enable();
        }

        this.committeeForm.controls.additionalAuthorityMembers.setValue(false);
        this.committeeForm.controls.additionalAuthorityMembers.disable();

        this.updateValidityForForm();
    }

    // handling Vertretungen des Bundes
    private setFederalAgenciesCommitteeFields() {
        this.emitExtraParliamentaryCommmission(false);

        if (this.canEditAll) {
            this.committeeForm.controls.levelId.enable();
            this.committeeForm.controls.legalFormId.enable();
            this.committeeForm.controls.federalInstitution.enable();
            this.committeeForm.controls.federalLawEstablishment.enable();
        }

        this.committeeForm.controls.marketOrientated.setValue(null);
        this.committeeForm.controls.marketOrientated.disable();

        this.committeeForm.controls.additionalAuthorityMembers.setValue(true);
        this.committeeForm.controls.additionalAuthorityMembers.disable();

        this.updateValidityForForm();
    }

    // handling Vertretungen des Bundes in grenzüberschreitenden Gremien
    private setFederalAgenciesCrossBorderCommitteeFields() {
        this.emitExtraParliamentaryCommmission(false);

        if (this.canEditAll) {
            this.committeeForm.controls.levelId.enable();
            this.committeeForm.controls.legalFormId.enable();
        }

        this.committeeForm.controls.marketOrientated.setValue(null);
        this.committeeForm.controls.marketOrientated.disable();

        this.committeeForm.controls.federalInstitution.setValue(null);
        this.committeeForm.controls.federalInstitution.disable();

        this.committeeForm.controls.additionalAuthorityMembers.setValue(true);
        this.committeeForm.controls.additionalAuthorityMembers.disable();

        this.updateValidityForForm();
    }

    private emitExtraParliamentaryCommmission(value: boolean) {
        this.isExtraParliamentaryCommmission.emit(value);
    }

    private updateValidityForForm() {
        this.committeeForm.controls.departmentId.updateValueAndValidity();
        this.committeeForm.controls.officeId.updateValueAndValidity();
        this.committeeForm.controls.federalLawEstablishment.updateValueAndValidity();
        this.committeeForm.controls.legalBase.updateValueAndValidity();
        this.committeeForm.controls.legalFormId.updateValueAndValidity();
        this.committeeForm.controls.federalInstitution.updateValueAndValidity();
        this.committeeForm.controls.marketOrientated.updateValueAndValidity();
        this.committeeForm.controls.supervisionDuty.updateValueAndValidity();

        this.committeeForm.markAllAsTouched();
    }
}
