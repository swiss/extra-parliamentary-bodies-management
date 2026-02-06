import {AfterViewChecked, Component, effect, model, signal, ViewChild} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatError, MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatTooltip} from '@angular/material/tooltip';
import {FunctionDto} from '@api/Function';
import {GeneralElectionCommitteeDetails} from '@api/GeneralElectionCommitteeDetails';
import {MembershipCandidateUpdate} from '@api/MembershipCandidateUpdate';
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
import {debounceTime} from 'rxjs';
import {ConfigsService} from '../../../../configs.service';
import {PersonsService} from '../../../../persons/persons.service';
import {PersonSearchComponent} from '../../../../persons/shared/person-search/person-search.component';

@Component({
    selector: 'apg-membership-candidate-data-form',
    templateUrl: './membership-candidate-data-form.component.html',
    styleUrl: './membership-candidate-data-form.component.scss',
    imports: [
        PersonSearchComponent,
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
        TranslatePipe,
        RichTextEditorComponent,
    ],
})
export class MembershipCandidateDataFormComponent implements AfterViewChecked {
    @ViewChild(PersonSearchComponent) formComponentPersonSearch: PersonSearchComponent | null = null;

    selectedPerson = signal<PersonDetails | undefined>(undefined);
    membershipCandidateModification = model<MembershipCandidateUpdate>();
    generalElectionCommittee = model<GeneralElectionCommitteeDetails | undefined>();
    searchTextBoxUpdated = false;

    membershipCandidateForm = this.createForm();

    constructor(
        protected readonly masterDataService: MasterDataService,
        private readonly personService: PersonsService,
        protected readonly errorService: ErrorService,
        readonly configsService: ConfigsService
    ) {
        this.membershipCandidateForm.valueChanges.pipe(debounceTime(300), takeUntilDestroyed()).subscribe(() => {
            const formValues = {
                ...this.membershipCandidateForm.getRawValue(),
                personId: this.selectedPerson()?.id,
            };
            this.membershipCandidateModification.update(value => ({...value, ...formValues}) as MembershipCandidateUpdate);
        });

        this.toggleFormFields();

        effect(() => {
            if (this.membershipCandidateModification()) {
                const modification = this.membershipCandidateModification()!;

                if (!this.selectedPerson() && modification?.personId) {
                    this.personService.getPersonDetails(modification.personId).subscribe(p => {
                        this.selectedPerson.set(p);
                    });
                }

                this.resetForm(modification);
            }
        });
    }

    public buildMembershipModification(): MembershipCandidateUpdate {
        const formValues = {
            ...this.membershipCandidateForm.getRawValue(),
            personId: this.selectedPerson()?.id,
        };

        return {...this.membershipCandidateModification(), ...formValues} as MembershipCandidateUpdate;
    }

    ngAfterViewChecked() {
        if (!this.searchTextBoxUpdated) {
            if (this.selectedPerson() && this.formComponentPersonSearch) {
                this.formComponentPersonSearch.setTextAndDisable(
                    `${this.selectedPerson()!.givenName} ${this.selectedPerson()!.surname} (${this.selectedPerson()!.birthYear})`
                );
                this.searchTextBoxUpdated = true;
            }
        }
    }

    resetForm(modification: MembershipCandidateUpdate) {
        if (!this.selectedPerson()) {
            this.membershipCandidateForm.controls.surname.patchValue(modification.surname!, {emitEvent: false});
            this.membershipCandidateForm.controls.givenName.patchValue(modification.givenName!, {emitEvent: false});
            this.membershipCandidateForm.controls.birthYear.patchValue(modification.birthYear, {emitEvent: false});
            this.membershipCandidateForm.controls.genderId.patchValue(modification.genderId!, {emitEvent: false});
            this.membershipCandidateForm.controls.languageId.patchValue(modification.languageId!, {emitEvent: false});
        }

        this.membershipCandidateForm.controls.beginDate.patchValue(new Date(modification.beginDate), {emitEvent: false});
        this.membershipCandidateForm.controls.endDate.patchValue(new Date(modification.endDate!), {emitEvent: false});
        this.membershipCandidateForm.controls.electionTypeId.patchValue(modification.electionTypeId, {emitEvent: false});
        this.membershipCandidateForm.controls.electionOfficeId.patchValue(modification.electionOfficeId, {emitEvent: false});
        this.membershipCandidateForm.controls.functionId.patchValue(modification.functionId, {emitEvent: false});
        this.membershipCandidateForm.controls.membershipAdditionId.patchValue(modification.membershipAdditionId, {emitEvent: false});
        this.membershipCandidateForm.controls.maximumEmploymentLevel.patchValue(modification.maximumEmploymentLevel, {emitEvent: false});
        this.membershipCandidateForm.controls.inCorrelationWithFederalDuty.patchValue(modification.inCorrelationWithFederalDuty, {
            emitEvent: false,
        });
        this.membershipCandidateForm.controls.justificationLongerDuty.patchValue(modification.justificationLongerDuty, {emitEvent: false});
        this.membershipCandidateForm.controls.justificationShorterDuty.patchValue(modification.justificationShorterDuty, {emitEvent: false});
        this.membershipCandidateForm.controls.justificationMemberInFederalDuty.patchValue(modification.justificationMemberInFederalDuty, {emitEvent: false});
        this.membershipCandidateForm.controls.justificationMemberInFederalAssembly.patchValue(modification.justificationMemberInFederalAssembly, {
            emitEvent: false,
        });
        this.membershipCandidateForm.controls.requirementsProfile.patchValue(modification.requirementsProfile, {
            emitEvent: false,
        });

        this.membershipCandidateForm.markAllAsTouched({emitEvent: false});
    }

    getFunctionText(f: FunctionDto): string {
        return this.selectedPerson()?.genderId === this.configsService.frontendConfig.entityIds.gender.femaleId ? f.textFemale : f.text;
    }

    private createForm() {
        const form = new FormGroup({
            surname: new FormControl('', {validators: [Validators.maxLength(150)]}),
            givenName: new FormControl('', {validators: [Validators.maxLength(150)]}),
            birthYear: new FormControl<number | undefined>(undefined),
            genderId: new FormControl(''),
            languageId: new FormControl(''),
            beginDate: new FormControl<Date>({value: today(), disabled: true}),
            endDate: new FormControl<Date | undefined>(undefined, {nonNullable: true}),
            electionTypeId: new FormControl({value: '', disabled: true}),
            functionId: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
            electionOfficeId: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
            membershipAdditionId: new FormControl<string | undefined>({value: undefined, disabled: false}),
            maximumEmploymentLevel: new FormControl<number | undefined>(
                {value: undefined, disabled: false},
                {validators: [Validators.min(1), Validators.max(100)]}
            ),
            inCorrelationWithFederalDuty: new FormControl<boolean | undefined>({value: true, disabled: false}),
            justificationLongerDuty: new FormControl<string | undefined>({value: undefined, disabled: false}),
            justificationShorterDuty: new FormControl<string | undefined>({value: undefined, disabled: false}),
            justificationMemberInFederalDuty: new FormControl<string | undefined>({value: undefined, disabled: false}),
            justificationMemberInFederalAssembly: new FormControl<string | undefined>({value: undefined, disabled: false}),
            requirementsProfile: new FormControl<string | undefined>({value: undefined, disabled: false}),
        });

        form.controls.beginDate.setValidators([
            Validators.required,
            rangeValidator(form.controls.endDate, Comparison.LowerThanEqual),
            rangeValidator(form.controls.beginDate, Comparison.GreaterThanEqual),
            dateValidator(),
        ]);
        form.controls.endDate.setValidators([Validators.required, rangeValidator(form.controls.beginDate, Comparison.GreaterThanEqual), dateValidator()]);
        form.controls.maximumEmploymentLevel.setValidators([
            Validators.min(1),
            Validators.max(100),
            conditionalValidator(() => this.generalElectionCommittee()?.marketOrientated === true, Validators.required),
        ]);
        form.controls.justificationLongerDuty.setValidators([
            conditionalValidator(() => this.membershipCandidateModification()?.needsLongerDutyJustification === true, Validators.required),
        ]);
        form.controls.justificationShorterDuty.setValidators([
            conditionalValidator(() => this.membershipCandidateModification()?.needsShorterDutyJustification === true, Validators.required),
        ]);
        form.controls.justificationMemberInFederalDuty.setValidators([
            conditionalValidator(() => this.membershipCandidateModification()?.needsFederalDutyJustification === true, Validators.required),
        ]);
        form.controls.justificationMemberInFederalAssembly.setValidators([
            conditionalValidator(() => this.membershipCandidateModification()?.needsFederalAssemblyJustification === true, Validators.required),
        ]);
        form.controls.requirementsProfile.setValidators([
            conditionalValidator(() => this.membershipCandidateModification()?.needsRequirementsProfile === true, Validators.required),
        ]);

        if (!this.selectedPerson()) {
            form.controls.surname.setValidators([Validators.required]);
            form.controls.givenName.setValidators([Validators.required]);
            form.controls.genderId.setValidators([Validators.required]);
            form.controls.languageId.setValidators([Validators.required]);
        }

        return form;
    }

    private toggleFormFields() {
        effect(() => {
            const isCandidateListCompleted = !!this.generalElectionCommittee()?.isCandidateListCompleted;
            if (isCandidateListCompleted) {
                this.membershipCandidateForm.controls.endDate.disable({emitEvent: false});
                this.membershipCandidateForm.controls.functionId.disable({emitEvent: false});
            } else {
                this.membershipCandidateForm.controls.endDate.enable({emitEvent: false});
                this.membershipCandidateForm.controls.functionId.enable({emitEvent: false});
            }

            const isExtraParliamentaryCommmission = !!this.generalElectionCommittee()?.extraParliamentaryCommission;
            if (isExtraParliamentaryCommmission) {
                this.membershipCandidateForm.controls.electionOfficeId.setValue(
                    this.configsService.frontendConfig.entityIds.electionOffice.federalGovernmentId
                );
                this.membershipCandidateForm.controls.electionOfficeId.disable({emitEvent: false});
            } else {
                this.membershipCandidateForm.controls.electionOfficeId.enable({emitEvent: false});
            }
        });
    }
}
