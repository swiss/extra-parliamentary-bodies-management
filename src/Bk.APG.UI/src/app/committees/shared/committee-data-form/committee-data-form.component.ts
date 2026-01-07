/* eslint-disable max-lines */
import {CdkTextareaAutosize} from '@angular/cdk/text-field';
import {Component, computed, effect, EventEmitter, Input, model, OnInit, Output, signal} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatAutocomplete, MatAutocompleteSelectedEvent, MatAutocompleteTrigger} from '@angular/material/autocomplete';
import {MatChipsModule} from '@angular/material/chips';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatIcon} from '@angular/material/icon';
import {MatError, MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {CommitteeCreate} from '@api/CommitteeCreate';
import {CommitteeUpdate} from '@api/CommitteeUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObErrorMessagesDirective, ObUnsavedChangesDirective} from '@oblique/oblique';
import {today} from '@shared/date-util';
import {ErrorService} from '@shared/error-service.service';
import {conditionalValidator} from '@shared/form-validators/conditional.validator';
import {Comparison, rangeValidator} from '@shared/form-validators/range.validator';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {isEmptyId} from '@shared/id-util';
import {MasterDataService} from '@shared/master-data.service';
import {MembersTooltipContentComponent} from '@shared/members-tooltip-content/members-tooltip-content.component';
import {debounceTime} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';
import {Role} from '../../../auth/Role';
import {ConfigsService} from '../../../configs.service';

@Component({
    selector: 'apg-committee-data-form',
    templateUrl: './committee-data-form.component.html',
    styleUrl: './committee-data-form.component.scss',
    imports: [
        ReactiveFormsModule,
        ObUnsavedChangesDirective,
        MatFormField,
        ObErrorMessagesDirective,
        MatLabel,
        MatInput,
        CdkTextareaAutosize,
        MatAutocomplete,
        MatAutocompleteTrigger,
        MatError,
        MatSelect,
        MatOption,
        MatDatepickerInput,
        MatDatepickerToggle,
        MatSuffix,
        MatDatepicker,
        HelpTooltipComponent,
        MembersTooltipContentComponent,
        TranslatePipe,
        MatChipsModule,
        MatIcon,
    ],
})
export class CommitteeDataFormComponent implements OnInit {
    @Input() isUpdateMode = false;
    @Output() readonly isExtraParliamentaryCommmission: EventEmitter<boolean> = new EventEmitter();

    committeeModification = model<CommitteeUpdate | CommitteeCreate>();

    committeeForm = this.createForm();
    canEditAll = false;
    isDepartment = signal(false);
    isOffice = signal(false);
    isSecretariat = signal(false);
    isAdmin = signal(false);

    departmentOffices = computed(() => {
        const offices = this.masterDataService.offices();
        return offices.filter(office => office.departmentId === this.selectedDepartmentId());
    });

    vacanciesInCurrentTermOfOffice = computed(() => {
        if (this.selectedCommitteeLevelId() !== this.configsService.frontendConfig.entityIds.committeeLevel.federalCouncilId) {
            return null;
        }

        const membersCount = (this.committeeModification() as CommitteeUpdate)?.membersCount ?? 0;
        const membersCountAboveMinimum = membersCount - (this.minimmalMembers() ?? 0);
        return membersCountAboveMinimum >= 0 ? 0 : membersCountAboveMinimum * -1;
    });

    protected readonly isGeneralElectionCommittee = computed(
        () =>
            this.selectedCommitteeLevelId() === this.configsService.frontendConfig.entityIds.committeeLevel.federalCouncilId &&
            this.selectedTermOfOfficeId() === this.configsService.frontendConfig.entityIds.termOfOffice.period4YearsInGeneralElectionId
    );

    // Multi-select membership additions (general election)
    protected membershipAdditionsInGeneralElectionInput = new FormControl({value: '', disabled: false});
    protected readonly allMembershipAdditions = computed(() => this.masterDataService.membershipAdditions().filter(m => !m.isDeleted));
    protected readonly filteredMembershipAdditionsInGeneralElection = computed(() => {
        const filterValue = (this.membershipAdditionsInGeneralElectionInputValue() || '').toLowerCase();
        const selectedIds = this.membershipAdditionsInGeneralElectionSelected() || [];
        return this.allMembershipAdditions().filter(x => !selectedIds.includes(x.id) && x.text.toLowerCase().includes(filterValue));
    });

    protected readonly canEditBeginDate = computed(() => {
        if (!this.isUpdateMode && !this.firstStepInCreateModeCompleted()) {
            return false;
        }

        return !this.isUpdateMode || this.isAdmin() || (this.isDepartment() && this.beginDate()! > today());
    });

    protected isAuthorityOrAdministration = computed(
        () =>
            this.selectedCommitteeTypeId() === this.configsService.frontendConfig.entityIds.committeeType.authorityId ||
            this.selectedCommitteeTypeId() === this.configsService.frontendConfig.entityIds.committeeType.administrationId
    );
    protected isManagementCommittee = computed(
        () => this.selectedCommitteeTypeId() === this.configsService.frontendConfig.entityIds.committeeType.managementId
    );

    protected readonly canEditEndDate = computed(() => {
        if (!this.isUpdateMode && !this.firstStepInCreateModeCompleted()) {
            return false;
        }

        if (this.isAdmin()) {
            return true;
        }

        // For department users: end date is only editable in update mode and when it hasn't been saved yet
        return this.isDepartment() && this.isUpdateMode && !this.hasOriginalEndDate();
    });

    private readonly hasOriginalEndDate = signal(false);

    private readonly membershipAdditionsInGeneralElectionInputValue = toSignal(this.membershipAdditionsInGeneralElectionInput.valueChanges, {
        initialValue: this.membershipAdditionsInGeneralElectionInput.value,
    });
    private readonly membershipAdditionsInGeneralElectionSelected = toSignal(this.committeeForm.controls.membershipAdditionsInGeneralElection.valueChanges, {
        initialValue: this.committeeForm.controls.membershipAdditionsInGeneralElection.value,
    });

    private readonly selectedCommitteeTypeId = toSignal(this.committeeForm.controls.committeeTypeId.valueChanges);
    private readonly selectedDepartmentId = toSignal(this.committeeForm.controls.departmentId.valueChanges);
    private readonly selectedCommitteeLevelId = toSignal(this.committeeForm.controls.levelId.valueChanges);
    private readonly selectedTermOfOfficeId = toSignal(this.committeeForm.controls.termOfOfficeId.valueChanges);
    private readonly minimmalMembers = toSignal(this.committeeForm.controls.minimalMembers.valueChanges);
    private readonly maximalMembers = toSignal(this.committeeForm.controls.maximalMembers.valueChanges);
    private readonly beginDate = toSignal(this.committeeForm.controls.beginDate.valueChanges);
    private readonly endDate = toSignal(this.committeeForm.controls.endDate.valueChanges);
    private readonly additionalAuthorityMembers = toSignal(this.committeeForm.controls.additionalAuthorityMembers.valueChanges);
    private readonly vacanciesInGeneralElection = toSignal(this.committeeForm.controls.vacanciesInGeneralElection.valueChanges, {
        initialValue: this.committeeForm.controls.vacanciesInGeneralElection.value,
    });

    private readonly firstStepInCreateModeCompleted = signal(false);

    constructor(
        protected readonly masterDataService: MasterDataService,
        protected readonly errorService: ErrorService,
        private readonly authService: AuthService,
        private readonly configsService: ConfigsService
    ) {
        this.authService.roles$.subscribe(roles => {
            this.isAdmin.set(roles.includes(Role.Admin));
            this.isOffice.set(roles.includes(Role.Office));
            this.isDepartment.set(roles.includes(Role.Department));
            this.isSecretariat.set(roles.includes(Role.Secretariat));
        });

        const effectRef = effect(() => {
            if (!this.isUpdateMode && !isEmptyId((this.committeeModification() as CommitteeCreate)?.departmentId)) {
                this.committeeForm.controls.departmentId.patchValue(this.committeeModification()!.departmentId);
                this.setDepartmentLegalBaseEditPermissions();
                effectRef.destroy();
            }

            if (this.isUpdateMode && (this.committeeModification() as CommitteeUpdate)?.id) {
                this.committeeForm.patchValue(this.committeeModification()!);
                this.setDepartmentLegalBaseEditPermissions();
                this.setFieldsBasedOnSelectedCommitteeType();
                // Check if the original data has an end date (meaning it was saved before)
                if ((this.committeeModification() as CommitteeUpdate)?.endDate) {
                    this.hasOriginalEndDate.set(true);
                }
                effectRef.destroy();
            }
        });

        effect(() => {
            if (this.isUpdateMode && this.minimmalMembers() !== undefined && this.maximalMembers() !== undefined) {
                this.committeeForm.controls.minimalMembers.updateValueAndValidity();
                this.committeeForm.controls.maximalMembers.updateValueAndValidity();
            }
        });

        effect(() => {
            if (this.isUpdateMode && this.beginDate() !== undefined && this.endDate() !== undefined) {
                this.committeeForm.controls.beginDate.updateValueAndValidity();
                this.committeeForm.controls.endDate.updateValueAndValidity();
            }
        });

        this.committeeForm.valueChanges.pipe(debounceTime(300), takeUntilDestroyed()).subscribe(() => {
            const formValues = this.committeeForm.getRawValue(); // getRawValue because we need the disabled values as well
            this.committeeModification.update(value => ({...value, ...(formValues as CommitteeUpdate | CommitteeCreate)}) as CommitteeUpdate | CommitteeCreate);

            if (
                !this.isUpdateMode &&
                !this.firstStepInCreateModeCompleted() &&
                formValues.committeeTypeId &&
                formValues.descriptionGerman &&
                formValues.descriptionFrench &&
                formValues.descriptionItalian &&
                formValues.descriptionRomansh
            ) {
                this.firstStepInCreateModeCompleted.set(true);
                this.toggleNonBasicControls(true);
            }
        });

        this.committeeForm.controls.federalLawEstablishment.valueChanges
            .pipe(takeUntilDestroyed())
            .subscribe(() => this.committeeForm.controls.legalBase.updateValueAndValidity({emitEvent: false}));

        effect(() => {
            if (!this.isUpdateMode && !this.firstStepInCreateModeCompleted()) {
                return;
            }

            const additionalAuthorityMembers = this.additionalAuthorityMembers();
            if (additionalAuthorityMembers) {
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

        effect(() => {
            if (!this.isUpdateMode && !this.firstStepInCreateModeCompleted()) {
                return;
            }

            this.setMembershipAdditionPermissions();
        });

        effect(() => {
            const canEditBeginDate = this.canEditBeginDate();
            if (canEditBeginDate) {
                this.committeeForm.controls.beginDate.enable();
            } else {
                this.committeeForm.controls.beginDate.disable();
            }
        });

        effect(() => {
            const canEditEndDate = this.canEditEndDate();
            if (canEditEndDate) {
                this.committeeForm.controls.endDate.enable();
            } else {
                this.committeeForm.controls.endDate.disable();
            }
        });
    }

    ngOnInit(): void {
        if (!this.isUpdateMode) {
            this.committeeForm.markAllAsTouched();
            this.toggleNonBasicControls(false);
        } else {
            this.committeeForm.controls.committeeTypeId.disable();
        }
    }

    markEndDateAsSaved(): void {
        if (this.isUpdateMode && this.committeeForm.controls.endDate.value) {
            this.hasOriginalEndDate.set(true);
        }
    }

    setMembershipAdditionInGeneralElection(event: MatAutocompleteSelectedEvent) {
        const addition = event.option.value as {id: string};
        const control = this.committeeForm.controls.membershipAdditionsInGeneralElection;
        if (!control) {
            return;
        }
        const current = control.value ? [...control.value] : [];
        if (!current.includes(addition.id)) {
            current.push(addition.id);
            control.setValue(current);
            control.markAsDirty();

            this.committeeForm.controls.vacanciesInGeneralElection.updateValueAndValidity();
        }
        this.membershipAdditionsInGeneralElectionInput.setValue('');
    }

    removeMembershipAdditionInGeneralElection(id: string) {
        const control = this.committeeForm.controls.membershipAdditionsInGeneralElection;
        if (!control?.value) {
            return;
        }
        control.setValue(control.value.filter((x: string) => x !== id));
        control.markAsDirty();

        this.committeeForm.controls.vacanciesInGeneralElection.updateValueAndValidity();
    }

    protected getMembershipAdditionInGeneralElection(id: string) {
        return this.allMembershipAdditions().find(a => a.id === id);
    }

    private setFieldsBasedOnSelectedCommitteeType() {
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
    }

    private setMembershipAdditionPermissions() {
        const vacancies = this.vacanciesInGeneralElection();
        const inRange = vacancies !== undefined && vacancies !== null && vacancies >= 1 && vacancies <= 99;

        if (inRange) {
            this.committeeForm.controls.membershipAdditionsInGeneralElection.enable();
            this.membershipAdditionsInGeneralElectionInput.enable();
        } else {
            this.committeeForm.controls.membershipAdditionsInGeneralElection.disable();
            this.committeeForm.controls.membershipAdditionsInGeneralElection.setValue([]);
            this.membershipAdditionsInGeneralElectionInput.disable();
        }

        this.committeeForm.controls.membershipAdditionsInGeneralElection.updateValueAndValidity();
        this.membershipAdditionsInGeneralElectionInput.updateValueAndValidity();

        // Mark as touched to show validation errors when the control becomes invalid
        if (this.committeeForm.controls.membershipAdditionsInGeneralElection.invalid) {
            this.committeeForm.controls.membershipAdditionsInGeneralElection.markAsTouched();
        }
    }

    private setDepartmentLegalBaseEditPermissions() {
        this.canEditAll = (this.committeeModification() as CommitteeUpdate | CommitteeCreate).canEditAll;
        if (!this.canEditAll) {
            this.committeeForm.disable();
            if (!(this.committeeModification() as CommitteeUpdate | CommitteeCreate).canEditLegalbase) {
                this.committeeForm.controls.legalBase.disable();
            } else {
                this.committeeForm.controls.legalBase.enable();
            }
        }
        if (!(this.committeeModification() as CommitteeUpdate | CommitteeCreate).canEditDepartment) {
            this.committeeForm.controls.departmentId.disable();
        } else {
            this.committeeForm.controls.departmentId.enable();
        }
    }

    private toggleNonBasicControls(enable: boolean) {
        if (enable) {
            this.committeeForm.controls.committeeTypeId.disable();
            this.committeeForm.controls.levelId.enable();
            this.committeeForm.controls.departmentId.enable();

            if (!this.isUpdateMode && !isEmptyId((this.committeeModification() as CommitteeCreate)?.departmentId)) {
                this.committeeForm.controls.departmentId.patchValue(this.committeeModification()!.departmentId);
                this.setDepartmentLegalBaseEditPermissions();
            }

            this.committeeForm.controls.officeId.enable();
            this.committeeForm.controls.minimalMembers.enable();
            this.committeeForm.controls.maximalMembers.enable();

            this.setMembershipAdditionPermissions();
            this.committeeForm.controls.additionalAuthorityMembers.enable();
            this.committeeForm.controls.termOfOfficeId.enable();
            this.committeeForm.controls.federalLawEstablishment.enable();
            this.committeeForm.controls.supervisionDuty.enable();
            this.committeeForm.controls.marketOrientated.enable();
            this.committeeForm.controls.federalInstitution.enable();

            if (this.committeeForm.controls.additionalAuthorityMembers.value) {
                this.committeeForm.controls.linkAuthorityWebsite.enable();
            } else {
                this.committeeForm.controls.linkAuthorityWebsite.disable();
                this.committeeForm.controls.linkAuthorityWebsite.setValue(null);
            }

            this.committeeForm.controls.legalFormId.enable();
            this.committeeForm.controls.legalBase.enable();
            this.committeeForm.controls.linkHomepageGerman.enable();
            this.committeeForm.controls.linkHomepageFrench.enable();
            this.committeeForm.controls.linkHomepageItalian.enable();
            this.committeeForm.controls.linkHomepageRomansh.enable();
            this.setFieldsBasedOnSelectedCommitteeType();
        } else {
            this.committeeForm.controls.levelId.disable();
            this.committeeForm.controls.departmentId.disable();
            this.committeeForm.controls.officeId.disable();
            this.committeeForm.controls.beginDate.disable();
            this.committeeForm.controls.endDate.disable();
            this.committeeForm.controls.minimalMembers.disable();
            this.committeeForm.controls.maximalMembers.disable();
            this.committeeForm.controls.membershipAdditionsInGeneralElection.disable();
            this.membershipAdditionsInGeneralElectionInput.disable();
            this.committeeForm.controls.additionalAuthorityMembers.disable();
            this.committeeForm.controls.termOfOfficeId.disable();
            this.committeeForm.controls.federalLawEstablishment.disable();
            this.committeeForm.controls.supervisionDuty.disable();
            this.committeeForm.controls.marketOrientated.disable();
            this.committeeForm.controls.federalInstitution.disable();
            this.committeeForm.controls.linkAuthorityWebsite.disable();
            this.committeeForm.controls.legalFormId.disable();
            this.committeeForm.controls.legalBase.disable();
            this.committeeForm.controls.linkHomepageGerman.disable();
            this.committeeForm.controls.linkHomepageFrench.disable();
            this.committeeForm.controls.linkHomepageItalian.disable();
            this.committeeForm.controls.linkHomepageRomansh.disable();
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
            supervisionDuty: new FormControl<boolean | null>(null),
            marketOrientated: new FormControl<boolean | null>(null),
            beginDate: new FormControl<Date>({value: today(), disabled: false}, {nonNullable: true, validators: [Validators.required]}),
            endDate: new FormControl<Date | undefined>({value: undefined, disabled: false}),
            termOfOfficeId: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
            minimalMembers: new FormControl<number | undefined>({value: undefined, disabled: false}, {validators: [Validators.pattern('^[0-9]*$')]}),
            maximalMembers: new FormControl<number | undefined>({value: undefined, disabled: false}, {validators: [Validators.pattern('^[0-9]*$')]}),
            additionalAuthorityMembers: new FormControl<boolean | null>(null, {validators: [Validators.required]}),
            linkAuthorityWebsite: new FormControl<string | undefined>({value: undefined, disabled: true}, {validators: [Validators.maxLength(500)]}),
            federalInstitution: new FormControl<boolean | null>(null),
            linkHomepageGerman: new FormControl<string | undefined>({value: undefined, disabled: false}, {validators: [Validators.maxLength(500)]}),
            linkHomepageFrench: new FormControl<string | undefined>({value: undefined, disabled: false}, {validators: [Validators.maxLength(500)]}),
            linkHomepageItalian: new FormControl<string | undefined>({value: undefined, disabled: false}, {validators: [Validators.maxLength(500)]}),
            linkHomepageRomansh: new FormControl<string | undefined>({value: undefined, disabled: false}, {validators: [Validators.maxLength(500)]}),
            vacanciesInGeneralElection: new FormControl<number | undefined>(
                {value: undefined, disabled: false},
                {validators: [Validators.min(0), Validators.max(99), Validators.pattern('^[0-9]*$')]}
            ),
            membershipAdditionsInGeneralElection: new FormControl<string[]>({value: [], disabled: false}),
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

        /* Workaround: this validator should be defined on the control 'membershipAdditionsInGeneralElection'.
           But since the mat-chip-grid has no binded form control, the validation errors are not shown. */
        form.controls.vacanciesInGeneralElection.addValidators(control => {
            const vacancies = control.value;
            if (vacancies === undefined || vacancies === null) {
                return null;
            }
            const additions = form.controls.membershipAdditionsInGeneralElection.value;
            if (!Array.isArray(additions)) {
                return null;
            }
            if (additions.length <= vacancies) {
                return null;
            }
            return {tooManyMembershipAdditions: {current: additions.length, vacancies}};
        });

        return form;
    }

    // handling Behördenkommissionen
    private setAuthorityCommitteeFields() {
        this.emitExtraParliamentaryCommmission(true);

        this.committeeForm.controls.levelId.setValue(this.configsService.frontendConfig.entityIds.committeeLevel.federalCouncilId);
        this.committeeForm.controls.levelId.disable();

        this.committeeForm.controls.federalInstitution.setValue(null);
        this.committeeForm.controls.federalInstitution.disable();

        this.committeeForm.controls.federalLawEstablishment.setValue(true);
        this.committeeForm.controls.federalLawEstablishment.disable();

        this.committeeForm.controls.legalFormId.setValue(null);
        this.committeeForm.controls.legalFormId.disable();

        this.committeeForm.controls.termOfOfficeId.setValue(this.configsService.frontendConfig.entityIds.termOfOffice.period4YearsInGeneralElectionId);
        this.committeeForm.controls.termOfOfficeId.disable();

        if (this.canEditAll) {
            this.committeeForm.controls.marketOrientated.enable();
            this.committeeForm.controls.additionalAuthorityMembers.enable();
        }

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

        this.committeeForm.controls.termOfOfficeId.setValue(this.configsService.frontendConfig.entityIds.termOfOffice.period4YearsInGeneralElectionId);
        this.committeeForm.controls.termOfOfficeId.disable();

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

        this.committeeForm.controls.additionalAuthorityMembers.setValue(false);
        this.committeeForm.controls.additionalAuthorityMembers.disable();

        if (this.canEditAll) {
            this.committeeForm.controls.levelId.enable();
            this.committeeForm.controls.federalInstitution.enable();
            this.committeeForm.controls.federalLawEstablishment.enable();
            this.committeeForm.controls.legalFormId.enable();
            this.committeeForm.controls.termOfOfficeId.enable();
        }

        this.updateValidityForForm();
    }

    // handling Vertretungen des Bundes
    private setFederalAgenciesCommitteeFields() {
        this.emitExtraParliamentaryCommmission(false);

        this.committeeForm.controls.marketOrientated.setValue(null);
        this.committeeForm.controls.marketOrientated.disable();

        this.committeeForm.controls.additionalAuthorityMembers.setValue(true);
        this.committeeForm.controls.additionalAuthorityMembers.disable();

        if (this.canEditAll) {
            this.committeeForm.controls.levelId.enable();
            this.committeeForm.controls.legalFormId.enable();
            this.committeeForm.controls.federalInstitution.enable();
            this.committeeForm.controls.federalLawEstablishment.enable();
            this.committeeForm.controls.termOfOfficeId.enable();
        }

        this.updateValidityForForm();
    }

    // handling Vertretungen des Bundes in grenzüberschreitenden Gremien
    private setFederalAgenciesCrossBorderCommitteeFields() {
        this.emitExtraParliamentaryCommmission(false);

        this.committeeForm.controls.marketOrientated.setValue(null);
        this.committeeForm.controls.marketOrientated.disable();

        this.committeeForm.controls.federalInstitution.setValue(null);
        this.committeeForm.controls.federalInstitution.disable();

        this.committeeForm.controls.additionalAuthorityMembers.setValue(true);
        this.committeeForm.controls.additionalAuthorityMembers.disable();

        if (this.canEditAll) {
            this.committeeForm.controls.levelId.enable();
            this.committeeForm.controls.legalFormId.enable();
            this.committeeForm.controls.termOfOfficeId.enable();
        }

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
