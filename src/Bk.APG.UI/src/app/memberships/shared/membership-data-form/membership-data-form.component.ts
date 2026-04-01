/* eslint-disable max-lines */
import {Component, computed, DestroyRef, effect, Input, model, OnInit, Signal, signal, ViewChild} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators} from '@angular/forms';
import {MatAutocompleteModule, MatAutocompleteSelectedEvent} from '@angular/material/autocomplete';
import {MatCard, MatCardContent} from '@angular/material/card';
import {MatCheckbox, MatCheckboxChange} from '@angular/material/checkbox';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle} from '@angular/material/expansion';
import {MatIcon} from '@angular/material/icon';
import {MatError, MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatTooltip} from '@angular/material/tooltip';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {CommitteeMembershipValidationRequest} from '@api/CommitteeMembershipValidationRequest';
import {CommitteeMembershipValidationResult} from '@api/CommitteeMembershipValidationResult';
import {FunctionDto} from '@api/Function';
import {MembershipAddition} from '@api/MembershipAddition';
import {MembershipCreate} from '@api/MembershipCreate';
import {MembershipUpdate} from '@api/MembershipUpdate';
import {PersonDetails} from '@api/PersonDetails';
import {TranslatePipe} from '@ngx-translate/core';
import {ObErrorMessagesDirective, ObUnsavedChangesDirective} from '@oblique/oblique';
import {today} from '@shared/date-util';
import {ErrorService} from '@shared/error-service.service';
import {conditionalValidator} from '@shared/form-validators/conditional.validator';
import {dateValidator} from '@shared/form-validators/date.validator';
import {Comparison, rangeValidator} from '@shared/form-validators/range.validator';
import {MasterDataService} from '@shared/master-data.service';
import {RichTextEditorComponent} from '@shared/rich-text-editor/rich-text-editor.component';
import {debounceTime, filter, forkJoin, map, merge, of, switchMap} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';
import {Role} from '../../../auth/Role';
import {CommitteeOverviewBasicDataComponent} from '../../../committees/committee-details/committee-overview/committee-overview-basic-data/committee-overview-basic-data.component';
import {CommitteesService} from '../../../committees/committees.service';
import {CommitteeJustificationsOverviewComponent} from '../../../committees/shared/committee-justifications-overview/committee-justifications-overview.component';
import {ConfigsService} from '../../../configs.service';
import {PersonOverviewInterestsComponent} from '../../../persons/person-details/person-overview/person-overview-interests/person-overview-interests.component';
import {PersonsService} from '../../../persons/persons.service';
import {PersonOverviewBasicDataComponent} from '../../../persons/shared/person-overview-basic-data/person-overview-basic-data.component';
import {PersonSearchComponent} from '../../../persons/shared/person-search/person-search.component';
import {HelpTooltipComponent} from '../../../shared/help-tooltip/help-tooltip.component';
import {CommitteeSearchComponent} from '../committee-search/committee-search.component';

@Component({
    selector: 'apg-membership-data-form',
    templateUrl: './membership-data-form.component.html',
    styleUrl: './membership-data-form.component.scss',
    imports: [
        PersonSearchComponent,
        CommitteeSearchComponent,
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
        MatTooltip,
        MatCheckbox,
        CommitteeOverviewBasicDataComponent,
        CommitteeJustificationsOverviewComponent,
        MatCard,
        MatCardContent,
        PersonOverviewBasicDataComponent,
        TranslatePipe,
        RichTextEditorComponent,
        MatAutocompleteModule,
        MatExpansionPanel,
        MatExpansionPanelHeader,
        MatExpansionPanelTitle,
        PersonOverviewInterestsComponent,
        MatIcon,
        HelpTooltipComponent,
    ],
})
export class MembershipDataFormComponent implements OnInit {
    @Input() isUpdateMode = false;
    @ViewChild(PersonSearchComponent) formComponentPersonSearch: PersonSearchComponent | null = null;
    @ViewChild(CommitteeSearchComponent) formComponentCommitteeSearch: CommitteeSearchComponent | null = null;

    personSelected = signal<PersonDetails | undefined>(undefined);
    committeeSelected = signal<CommitteeDetails | undefined>(undefined);
    validationResults = signal({} as CommitteeMembershipValidationResult);
    validationRequest!: CommitteeMembershipValidationRequest;
    activeFunctions = computed(() => this.masterDataService.functions().filter(y => !y.isDeleted));
    activeElectionTypes = computed(() => this.masterDataService.electionTypes().filter(y => !y.isDeleted));
    activeElectionOffices = computed(() => this.masterDataService.electionOffices().filter(y => !y.isDeleted));
    membershipModification = model<MembershipCreate | MembershipUpdate>();
    personEntity = model<PersonDetails | undefined>();
    committeeEntity = model<CommitteeDetails | undefined>();
    searchTextBoxUpdated = false;
    membershipAdditionId: string | undefined;

    isAdmin = false;
    isDepartment = false;
    isOffice = false;
    isSecretariat = false;
    canEdit = false;
    canEditBeginDate = false;

    currentDate: Date = new Date();

    filterText = signal('');
    membershipAdditions = computed(() => this.masterDataService.membershipAdditions().filter(y => !y.isDeleted));

    filteredMembershipAdditions = computed(() => {
        const text = this.filterText().toLowerCase();
        return this.membershipAdditions().filter(option => option.text.toLowerCase().includes(text));
    });

    membershipForm = this.createForm();

    formBeginDate!: Signal<Date>;
    formEndDate!: Signal<Date | undefined>;
    formInCorrelationWithFederalDuty!: Signal<boolean | null | undefined>;

    public get person() {
        if (this.personEntity()) {
            return this.personEntity;
        }
        if (this.personSelected()) {
            return this.personSelected;
        }
        return signal(undefined);
    }

    public get committee() {
        if (this.committeeEntity()) {
            return this.committeeEntity;
        }
        if (this.committeeSelected()) {
            return this.committeeSelected;
        }
        return signal(undefined);
    }

    private isValidating = false;

    private readonly selectedElectionTypeId = toSignal(this.membershipForm.controls.electionTypeId.valueChanges);

    private readonly justificationLongerDutyNeeded = computed(() => {
        const extraParliamentaryCommission = this.committee()?.extraParliamentaryCommission ?? false;
        const inCorrelationWithFederalDuty = this.formInCorrelationWithFederalDuty() ?? false;
        const estimatedTermOfOffice = this.validationResults().estimatedTermOfOffice ?? 0;

        return extraParliamentaryCommission && !inCorrelationWithFederalDuty && estimatedTermOfOffice > 12;
    });

    private readonly justificationShorterDutyNeeded = computed(() => {
        const period4YearsInGeneralElection = this.committee()?.period4YearsInGeneralElection ?? false;
        const endDate = this.formEndDate();
        const termOfOfficeEndDate = this.committee()?.termOfOfficeEndDate;

        if (endDate == null || termOfOfficeEndDate == null) {
            return false;
        }

        return period4YearsInGeneralElection && endDate.getTime() < termOfOfficeEndDate.getTime();
    });

    private readonly justificationMemberInFederalDutyNeeded = computed(() => {
        const extraParliamentaryCommission = this.committee()?.extraParliamentaryCommission ?? false;
        const federalDuty = this.person()?.federalDuty ?? false;

        return extraParliamentaryCommission && federalDuty;
    });

    private readonly justificationMemberInFederalAssemblyNeeded = computed(() => {
        const extraParliamentaryCommission = this.committee()?.extraParliamentaryCommission ?? false;
        const federalAssembly = this.person()?.federalAssembly ?? false;

        return extraParliamentaryCommission && federalAssembly;
    });

    private readonly requirementsProfileNeeded = computed(() => {
        return (
            (!!(this.committee()?.committeeTypeId === this.configsService.frontendConfig.entityIds.committeeType.managementId) ||
                !!(this.committee()?.committeeTypeId === this.configsService.frontendConfig.entityIds.committeeType.federalAgenciesId) ||
                !!(this.committee()?.supervisionDuty === true)) &&
            this.selectedElectionTypeId() === this.configsService.frontendConfig.entityIds.electionType.newElectionId
        );
    });

    private get personId() {
        if (this.personEntity()) {
            return this.personEntity()!.id;
        }
        if (this.personSelected()) {
            return this.personSelected()!.id;
        }
        return undefined;
    }

    private get committeeId() {
        if (this.committeeSelected()) {
            return this.committeeSelected()!.id;
        }
        if (this.committeeEntity()) {
            return this.committeeEntity()!.id;
        }
        return undefined;
    }

    constructor(
        protected readonly masterDataService: MasterDataService,
        private readonly committeesService: CommitteesService,
        private readonly personService: PersonsService,
        protected readonly errorService: ErrorService,
        private readonly authService: AuthService,
        private readonly destroyRef: DestroyRef,
        private readonly configsService: ConfigsService
    ) {
        this.authService.roles$.subscribe(roles => {
            this.isAdmin = roles.includes(Role.Admin);
            this.isOffice = roles.includes(Role.Office);
            this.isDepartment = roles.includes(Role.Department);
            this.isSecretariat = roles.includes(Role.Secretariat);
        });

        this.formBeginDate = toSignal(this.membershipForm.controls.beginDate.valueChanges, {
            initialValue: this.membershipForm.controls.beginDate.value,
        });
        this.formEndDate = toSignal(this.membershipForm.controls.endDate.valueChanges, {
            initialValue: this.membershipForm.controls.endDate.value,
        });
        this.formInCorrelationWithFederalDuty = toSignal(this.membershipForm.controls.inCorrelationWithFederalDuty.valueChanges, {
            initialValue: this.membershipForm.controls.inCorrelationWithFederalDuty.value,
        });

        effect(() => {
            this.justificationLongerDutyNeeded();
            this.justificationShorterDutyNeeded();
            this.justificationMemberInFederalDutyNeeded();
            this.justificationMemberInFederalAssemblyNeeded();
            this.requirementsProfileNeeded();

            if (this.validationResults().hasErrors) {
                return;
            }

            this.toggleJustificationFields(false);

            this.membershipForm.controls.justificationLongerDuty.updateValueAndValidity();
            this.membershipForm.controls.justificationShorterDuty.updateValueAndValidity();
            this.membershipForm.controls.justificationMemberInFederalDuty.updateValueAndValidity();
            this.membershipForm.controls.justificationMemberInFederalAssembly.updateValueAndValidity();
            this.membershipForm.controls.requirementsProfile.updateValueAndValidity();
        });

        this.membershipForm.controls.inCorrelationWithFederalDuty.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => {
            this.updateValidity(false);
        });

        this.subscribeToDateRangeChanges();

        this.membershipForm.controls.electionTypeId.valueChanges.pipe(takeUntilDestroyed()).subscribe(() => {
            this.validateElectionType();
            this.membershipForm.controls.endDate.updateValueAndValidity({emitEvent: false});
            this.membershipForm.controls.requirementsProfile.updateValueAndValidity();
        });

        this.membershipForm.controls.membershipAdditionId.valueChanges.pipe(takeUntilDestroyed()).subscribe(value => {
            if (typeof value === 'string') {
                this.filterText.set(value);
            } else {
                this.filterText.set('');
            }
        });

        effect(() => {
            if (!this.isUpdateMode || !this.membershipModification() || !(this.committeeEntity() || this.personEntity())) {
                return;
            }

            const committee$ = this.committeeEntity()
                ? of(this.committeeEntity())
                : this.committeesService.getCommitteeDetails(this.membershipModification()!.committeeId);

            const person$ = this.personEntity() ? of(this.personEntity()) : this.personService.getPersonDetails(this.membershipModification()!.personId);

            forkJoin({committee: committee$, person: person$})
                .pipe(takeUntilDestroyed(this.destroyRef))
                .subscribe(({committee, person}) => {
                    if (!this.committeeEntity()) {
                        this.committeeSelected.set(committee);
                    }
                    if (!this.personEntity()) {
                        this.personSelected.set(person);
                    }

                    this.handleElectionOfficeField();
                    this.resetForm(this.membershipModification()!);

                    this.membershipAdditionId = this.membershipModification()?.membershipAdditionId;

                    this.canEdit = (this.membershipModification() as MembershipUpdate).canEdit;
                    this.canEditBeginDate = (this.membershipModification() as MembershipUpdate).canEditBeginDate;

                    const beginDateControl = this.membershipForm.controls.beginDate;
                    const endDateControl = this.membershipForm.controls.endDate;

                    if (!this.canEdit) {
                        beginDateControl.disable();
                        endDateControl.disable();
                    } else {
                        endDateControl.enable();
                        if (this.canEditBeginDate) {
                            beginDateControl.enable();
                        } else {
                            beginDateControl.disable();
                        }
                    }

                    this.updateValidity(false);

                    this.setupForm();
                });
        });

        effect(() => {
            if (!this.isUpdateMode) {
                const committee = this.committee();
                if (committee?.termOfOfficeEndDate) {
                    this.membershipForm.controls.endDate.patchValue(new Date(committee.termOfOfficeEndDate));
                }
            }
        });
    }

    setupForm() {
        if (!this.isUpdateMode) {
            this.membershipForm.controls.endDate.markAllAsTouched();
            if (this.formComponentCommitteeSearch) {
                this.formComponentCommitteeSearch.markTextBoxAsTouched();
            }
            if (this.formComponentPersonSearch) {
                this.formComponentPersonSearch.markTextBoxAsTouched();
            }
        }
        if (this.isUpdateMode && !this.searchTextBoxUpdated) {
            if (this.committeeSelected() && this.formComponentCommitteeSearch) {
                this.formComponentCommitteeSearch.setTextAndDisable(this.committeeSelected()!.description);
                this.searchTextBoxUpdated = true;
            }
            if (this.personSelected() && this.formComponentPersonSearch) {
                this.formComponentPersonSearch.setTextAndDisable(
                    `${this.personSelected()!.givenName} ${this.personSelected()!.surname} (${this.personSelected()!.birthYear})`
                );
                this.searchTextBoxUpdated = true;
            }

            if (!this.canEdit) {
                this.toggleFormFields(false);
            }
        }
    }

    hasDeletedElectionOffice(): boolean {
        return this.activeElectionOffices().find(y => y.id === this.membershipModification()?.electionOfficeId) === undefined;
    }

    hasDeletedFunction(): boolean {
        return this.activeFunctions().find(y => y.id === this.membershipModification()?.functionId) === undefined;
    }

    updateValidity(updateInCorrelationWithFederalDutyCheckbox: boolean = true) {
        this.validateData({checked: true} as MatCheckboxChange);

        if (updateInCorrelationWithFederalDutyCheckbox) {
            if (this.person()?.federalDuty) {
                this.membershipForm.controls.inCorrelationWithFederalDuty.setValue(true);
            } else {
                this.membershipForm.controls.inCorrelationWithFederalDuty.setValue(false);
            }
        }

        this.toggleJustificationFields(false);
        this.membershipForm.controls.maximumEmploymentLevel.updateValueAndValidity();
        this.membershipForm.controls.justificationLongerDuty.updateValueAndValidity();
        this.membershipForm.controls.justificationShorterDuty.updateValueAndValidity();
        this.membershipForm.controls.justificationMemberInFederalDuty.updateValueAndValidity();
        this.membershipForm.controls.justificationMemberInFederalAssembly.updateValueAndValidity();

        this.membershipForm.controls.maximumEmploymentLevel.markAsTouched();
        this.membershipForm.controls.justificationLongerDuty.markAsTouched();
        this.membershipForm.controls.justificationShorterDuty.markAsTouched();
        this.membershipForm.controls.justificationMemberInFederalDuty.markAsTouched();
        this.membershipForm.controls.justificationMemberInFederalAssembly.markAsTouched();
    }

    ngOnInit() {
        if (!this.isUpdateMode) {
            this.toggleFormFields(true);
        } else {
            this.membershipForm.controls.endDate.updateValueAndValidity();
            this.membershipForm.controls.endDate.markAsTouched();
        }
    }

    validateElectionType() {
        this.toggleJustificationFields(false);
    }

    resetForm(modification: MembershipCreate | MembershipUpdate) {
        this.membershipForm.controls.beginDate.patchValue(new Date(modification.beginDate), {emitEvent: false});
        this.membershipForm.controls.endDate.patchValue(new Date(modification.endDate!), {emitEvent: false});
        this.membershipForm.controls.electionTypeId.patchValue(modification.electionTypeId, {emitEvent: false});
        if (this.activeElectionOffices().find(f => f.id === modification.electionOfficeId) !== undefined) {
            this.membershipForm.controls.electionOfficeId.patchValue(modification.electionOfficeId, {emitEvent: false});
        } else {
            this.membershipForm.controls.electionOfficeId.patchValue('', {emitEvent: false});
        }
        if (this.activeFunctions().find(f => f.id === modification.functionId) !== undefined) {
            this.membershipForm.controls.functionId.patchValue(modification.functionId, {emitEvent: false});
        } else {
            this.membershipForm.controls.functionId.patchValue('', {emitEvent: false});
        }
        this.membershipForm.controls.membershipAdditionId.patchValue(modification.membershipAddition?.text, {emitEvent: false});
        this.membershipForm.controls.justificationLongerDuty.patchValue(modification.justificationLongerDuty, {emitEvent: false});
        this.membershipForm.controls.justificationShorterDuty.patchValue(modification.justificationShorterDuty, {
            emitEvent: false,
        });
        this.membershipForm.controls.justificationMemberInFederalDuty.patchValue(modification.justificationMemberInFederalDuty, {
            emitEvent: false,
        });
        this.membershipForm.controls.justificationMemberInFederalAssembly.patchValue(modification.justificationMemberInFederalAssembly, {emitEvent: false});
        this.membershipForm.controls.requirementsProfile.patchValue(modification.requirementsProfile, {emitEvent: false});
        this.membershipForm.controls.maximumEmploymentLevel.patchValue(modification.maximumEmploymentLevel, {emitEvent: false});
        this.membershipForm.controls.remarks.patchValue(modification.remarks, {emitEvent: false});
        this.membershipForm.controls.remarksStatus.patchValue(modification.remarksStatus, {emitEvent: false});
        this.membershipForm.controls.inCorrelationWithFederalDuty.patchValue(modification.inCorrelationWithFederalDuty);

        this.membershipForm.controls.endDate.updateValueAndValidity();
        this.membershipForm.controls.electionTypeId.updateValueAndValidity();
        this.membershipForm.controls.endDate.markAsTouched();
        this.membershipForm.controls.electionTypeId.markAsTouched();
        this.membershipForm.controls.beginDate.updateValueAndValidity();
    }

    validateData(event: MatCheckboxChange) {
        if (event.checked && !this.isValidating) {
            this.validationRequest = {
                isUpdateMode: this.isUpdateMode,
                beginDate: this.membershipForm.controls.beginDate.value,
                endDate: this.membershipForm.controls.endDate.value!,
                personId: this.personId,
                committeeid: this.committeeId,
                inCorrelationWithFederalDuty: this.membershipForm.controls.inCorrelationWithFederalDuty.value,
                currentMembershipId: this.isUpdateMode ? (this.membershipModification() as MembershipUpdate).id : undefined,
            } as CommitteeMembershipValidationRequest;

            if (
                this.isAfterYear2000(this.validationRequest.beginDate) &&
                this.isAfterYear2000(this.validationRequest.endDate) &&
                this.validationRequest.committeeid &&
                this.validationRequest.personId
            ) {
                this.isValidating = true;
                this.committeesService.validateMembership(this.committeeId!, this.validationRequest).subscribe(validations => {
                    this.isValidating = false;
                    this.validationResults.set(validations);
                    if (this.validationResults().hasErrors) {
                        this.toggleFormFields(true);
                    } else {
                        this.toggleFormFields(false);
                    }

                    this.membershipForm.controls.justificationLongerDuty.updateValueAndValidity();
                    this.membershipForm.controls.justificationShorterDuty.updateValueAndValidity();
                    this.membershipForm.controls.justificationMemberInFederalDuty.updateValueAndValidity();
                    this.membershipForm.controls.justificationMemberInFederalAssembly.updateValueAndValidity();
                    this.membershipForm.controls.requirementsProfile.updateValueAndValidity();

                    this.membershipForm.controls.justificationLongerDuty.markAsTouched();
                    this.membershipForm.controls.justificationShorterDuty.markAsTouched();
                    this.membershipForm.controls.justificationMemberInFederalDuty.markAsTouched();
                    this.membershipForm.controls.justificationMemberInFederalAssembly.markAsTouched();
                    this.membershipForm.controls.requirementsProfile.markAsTouched();
                });
            }
        }
    }

    setMembershipAddition(event: MatAutocompleteSelectedEvent) {
        const membershipAddition = event.option.value as MembershipAddition;
        this.membershipAdditionId = membershipAddition.id;
        this.membershipForm.controls.membershipAdditionId.setValue(membershipAddition.text);
    }

    public getFunctionText(f: FunctionDto): string {
        return this.personSelected()?.genderId === this.configsService.frontendConfig.entityIds.gender.femaleId ||
            this.personEntity()?.genderId === this.configsService.frontendConfig.entityIds.gender.femaleId
            ? f.textFemale
            : f.text;
    }

    public buildMembershipModification(): MembershipCreate | MembershipUpdate {
        const formValues = {
            ...this.membershipForm.getRawValue(),
            personId: this.person()?.id,
            committeeId: this.committee()?.id,
            membershipAdditionId: this.membershipForm.controls.membershipAdditionId.value === '' ? undefined : this.membershipAdditionId,
        };

        return {...this.membershipModification(), ...formValues} as MembershipCreate | MembershipUpdate;
    }

    private createForm() {
        const form = new FormGroup({
            beginDate: new FormControl(today(), {nonNullable: true}),
            endDate: new FormControl<Date | undefined>(undefined, {nonNullable: true}),
            electionTypeId: new FormControl(this.configsService.frontendConfig.entityIds.electionType.newElectionId, {
                nonNullable: true,
                validators: [
                    Validators.required,
                    conditionalStatusEndDateValidator([
                        this.configsService.frontendConfig.entityIds.electionType.newElectionId,
                        this.configsService.frontendConfig.entityIds.electionType.reElectionId,
                    ]),
                ],
            }),
            functionId: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
            electionOfficeId: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
            membershipAdditionId: new FormControl<string | undefined>({value: undefined, disabled: false}),
            justificationLongerDuty: new FormControl<string | undefined>({value: '', disabled: false}),
            justificationShorterDuty: new FormControl<string | undefined>({value: '', disabled: false}),
            justificationMemberInFederalDuty: new FormControl<string | undefined>({value: '', disabled: false}),
            justificationMemberInFederalAssembly: new FormControl<string | undefined>({value: '', disabled: false}),
            requirementsProfile: new FormControl<string | undefined>({value: '', disabled: false}),
            maximumEmploymentLevel: new FormControl<number | undefined>(
                {value: undefined, disabled: false},
                {validators: [Validators.min(1), Validators.max(100)]}
            ),
            remarks: new FormControl<string | undefined>('', {validators: [Validators.maxLength(1000)]}),
            remarksStatus: new FormControl<string | undefined>('', {validators: [Validators.maxLength(1000)]}),
            inCorrelationWithFederalDuty: new FormControl<boolean | undefined>({value: true, disabled: false}),
        });

        form.controls.beginDate.setValidators([
            Validators.required,
            rangeValidator(form.controls.endDate, Comparison.LowerThanEqual),
            rangeValidator(form.controls.beginDate, Comparison.GreaterThanEqual),
            dateValidator(),
        ]);
        form.controls.endDate.setValidators([
            Validators.required,
            rangeValidator(form.controls.beginDate, Comparison.GreaterThanEqual),
            dateValidator(),
            conditionalStatusEndDateValidator([
                this.configsService.frontendConfig.entityIds.electionType.newElectionId,
                this.configsService.frontendConfig.entityIds.electionType.reElectionId,
            ]),
        ]);
        form.controls.maximumEmploymentLevel.setValidators([
            Validators.min(1),
            Validators.max(100),
            conditionalValidator(() => this.committee()?.marketOrientated === true, Validators.required),
        ]);

        form.controls.justificationLongerDuty.setValidators([conditionalValidator(() => this.justificationLongerDutyNeeded(), Validators.required)]);
        form.controls.justificationShorterDuty.setValidators([conditionalValidator(() => this.justificationShorterDutyNeeded(), Validators.required)]);
        form.controls.justificationMemberInFederalDuty.setValidators([
            conditionalValidator(() => this.justificationMemberInFederalDutyNeeded(), Validators.required),
        ]);
        form.controls.justificationMemberInFederalAssembly.setValidators([
            conditionalValidator(() => this.justificationMemberInFederalAssemblyNeeded(), Validators.required),
        ]);
        form.controls.requirementsProfile.setValidators([conditionalValidator(() => this.requirementsProfileNeeded(), Validators.required)]);

        return form;
    }

    private subscribeToDateRangeChanges() {
        const isControlValidOrDisabled = (control: AbstractControl) => control.disabled || control.valid;

        merge(
            this.membershipForm.controls.beginDate.valueChanges.pipe(map(beginDate => ({beginDate}))),
            this.membershipForm.controls.endDate.valueChanges.pipe(map(endDate => ({endDate})))
        )
            .pipe(
                debounceTime(300),
                filter(
                    () => isControlValidOrDisabled(this.membershipForm.controls.endDate) && isControlValidOrDisabled(this.membershipForm.controls.beginDate)
                ),
                filter(() => !this.isValidating),
                switchMap(() => {
                    this.validationRequest = {
                        isUpdateMode: this.isUpdateMode,
                        beginDate: this.membershipForm.controls.beginDate.value,
                        endDate: this.membershipForm.controls.endDate.value!,
                        personId: this.personId,
                        committeeid: this.committeeId,
                        inCorrelationWithFederalDuty: this.membershipForm.controls.inCorrelationWithFederalDuty.value,
                        currentMembershipId: this.isUpdateMode ? (this.membershipModification() as MembershipUpdate).id : undefined,
                    } as CommitteeMembershipValidationRequest;

                    if (
                        this.isAfterYear2000(this.validationRequest.beginDate) &&
                        this.isAfterYear2000(this.validationRequest.endDate) &&
                        this.validationRequest.committeeid &&
                        this.validationRequest.personId
                    ) {
                        this.isValidating = true;
                        return this.committeesService.validateMembership(this.committeeId!, this.validationRequest);
                    }
                    this.validationResults().hasErrors = !this.isUpdateMode;
                    return of(this.validationResults());
                }),
                takeUntilDestroyed(this.destroyRef)
            )
            .subscribe(validations => {
                this.isValidating = false;
                this.validationResults.set(validations);
                if (this.validationResults().hasErrors) {
                    this.toggleFormFields(true);
                } else {
                    this.toggleFormFields(false);
                }

                this.membershipForm.controls.justificationLongerDuty.updateValueAndValidity();
                this.membershipForm.controls.justificationShorterDuty.updateValueAndValidity();
                this.membershipForm.controls.justificationMemberInFederalDuty.updateValueAndValidity();
                this.membershipForm.controls.justificationMemberInFederalAssembly.updateValueAndValidity();
                this.membershipForm.controls.requirementsProfile.updateValueAndValidity();

                this.membershipForm.controls.justificationLongerDuty.markAsTouched();
                this.membershipForm.controls.justificationShorterDuty.markAsTouched();
                this.membershipForm.controls.justificationMemberInFederalDuty.markAsTouched();
                this.membershipForm.controls.justificationMemberInFederalAssembly.markAsTouched();
                this.membershipForm.controls.requirementsProfile.markAsTouched();
            });
    }

    private toggleFormFields(disable: boolean) {
        if (!this.canEdit && this.isUpdateMode) {
            disable = true;
        }

        if (this.membershipForm) {
            if (disable) {
                this.membershipForm.controls.electionTypeId.disable();
                this.membershipForm.controls.functionId.disable();
                this.membershipForm.controls.electionOfficeId.disable();
                this.membershipForm.controls.membershipAdditionId.disable();
                this.membershipForm.controls.maximumEmploymentLevel.disable();
                this.membershipForm.controls.remarks.disable();
                this.membershipForm.controls.remarksStatus.disable();
            } else {
                if (this.isUpdateMode) {
                    this.membershipForm.controls.electionTypeId.enable();
                    this.membershipForm.controls.electionTypeId.updateValueAndValidity();
                    this.membershipForm.controls.electionTypeId.markAllAsTouched();
                }

                this.membershipForm.controls.functionId.enable();
                this.membershipForm.controls.functionId.updateValueAndValidity();
                this.membershipForm.controls.functionId.markAllAsTouched();

                this.handleElectionOfficeField();
                this.membershipForm.controls.membershipAdditionId.enable();
                this.membershipForm.controls.maximumEmploymentLevel.enable();
                this.membershipForm.controls.remarks.enable();
                this.membershipForm.controls.remarksStatus.enable();
            }
            this.toggleJustificationFields(disable);
        }
    }

    private handleElectionOfficeField() {
        const isExtraParliamentaryCommmission =
            !!this.committeeEntity()?.extraParliamentaryCommission || !!this.committeeSelected()?.extraParliamentaryCommission;

        if (isExtraParliamentaryCommmission) {
            this.membershipForm.controls.electionOfficeId.setValue(this.configsService.frontendConfig.entityIds.electionOffice.federalGovernmentId);
            this.membershipForm.controls.electionOfficeId.disable();
        } else {
            this.membershipForm.controls.electionOfficeId.enable();
            this.membershipForm.controls.electionOfficeId.updateValueAndValidity();
            this.membershipForm.controls.electionOfficeId.markAllAsTouched();
        }
    }

    private toggleJustificationFields(disable: boolean) {
        const form = this.membershipForm;
        disable = disable || (this.isUpdateMode && form.controls.electionTypeId.disabled);
        if (disable || !this.person()?.federalDuty) {
            form.controls.inCorrelationWithFederalDuty.disable({emitEvent: false});
        } else {
            form.controls.inCorrelationWithFederalDuty.enable({emitEvent: false});
        }
        if (disable || !this.justificationLongerDutyNeeded()) {
            form.controls.justificationLongerDuty.disable();
        } else {
            form.controls.justificationLongerDuty.enable();
        }
        if (disable || !this.justificationShorterDutyNeeded()) {
            form.controls.justificationShorterDuty.disable();
        } else {
            form.controls.justificationShorterDuty.enable();
        }
        if (disable || !this.justificationMemberInFederalDutyNeeded()) {
            form.controls.justificationMemberInFederalDuty.disable();
        } else {
            form.controls.justificationMemberInFederalDuty.enable();
        }
        if (disable || !this.justificationMemberInFederalAssemblyNeeded()) {
            form.controls.justificationMemberInFederalAssembly.disable();
        } else {
            form.controls.justificationMemberInFederalAssembly.enable();
        }
        if (disable || !this.requirementsProfileNeeded()) {
            form.controls.requirementsProfile.disable();
        } else {
            form.controls.requirementsProfile.enable();
        }
    }

    private isAfterYear2000(date: Date): boolean {
        const year2000 = new Date(2000, 0, 1);
        return date > year2000;
    }
}

export function conditionalStatusEndDateValidator(electionTypeValues: string[]): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const form = control.parent;
        if (!form) {
            return null;
        }

        const endDate = form.get('endDate');
        const electionTypeId = form.get('electionTypeId');
        const currentDate = new Date();

        if (!endDate || !electionTypeId) {
            return null;
        }

        const isEndDateInvalid = new Date(endDate.value) < currentDate;
        const electionTypeIdValue = electionTypeId.value;

        const isElectionTypeInvalid = electionTypeValues.includes(electionTypeIdValue);

        if (isEndDateInvalid && isElectionTypeInvalid) {
            return {invalidDateAndStatus: true};
        }
        return null;
    };
}
