import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {UntypedFormArray, UntypedFormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatTooltip} from '@angular/material/tooltip';
import {InterestUpdate} from '@api/InterestUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective} from '@oblique/oblique';
import {rangeValidator, Comparison} from '@shared/form-validators/range.validator';
import {firstValueFrom} from 'rxjs';
import {InterestsEditFormDetailComponent} from '../interests-edit-form-detail/interests-edit-form-detail.component';
import {PersonInterestsService} from '../person-interests.service';

@Component({
    selector: 'apg-interests-edit-form',
    templateUrl: './interests-edit-form.component.html',
    styleUrl: './interests-edit-form.component.scss',
    imports: [InterestsEditFormDetailComponent, ReactiveFormsModule, MatButton, ObButtonDirective, MatTooltip, MatIcon, TranslatePipe],
})
export class InterestsEditFormComponent implements OnChanges {
    @Input() formArray!: UntypedFormArray;
    @Input() personId!: string;
    @Input() reload = false;

    interests: InterestUpdate[] = [];

    constructor(
        private readonly formBuilder: UntypedFormBuilder,
        private readonly personInterestsService: PersonInterestsService
    ) {}

    ngOnChanges(changes: SimpleChanges) {
        if (changes.personId || changes.reload) {
            void firstValueFrom(this.personInterestsService.getInterestsByPersonId(this.personId)).then(interestUpdates => {
                this.interests = interestUpdates;
                this.setupForm(interestUpdates);
            });
        }
    }

    reset() {
        this.setupForm(this.interests);
    }

    addInterest() {
        this.addEmptyInterest();
        this.markForm();
    }

    removeInterest(index: number) {
        this.formArray.removeAt(index);
        this.markForm();
    }

    private setupForm(interests?: InterestUpdate[]): void {
        if (!interests?.length || !this.formArray) {
            return;
        }

        this.formArray.clear();
        interests.forEach(i => {
            const formGroup = this.formBuilder.group({
                id: this.formBuilder.control(i.id),
                personId: this.formBuilder.control(i.personId),
                interestText: this.formBuilder.control(i.interestText, {validators: [Validators.required]}),
                text: this.formBuilder.control({value: i.text, disabled: true}),
                interestCommitteeId: this.formBuilder.control(i.interestCommitteeId, {validators: [Validators.required]}),
                interestFunctionId: this.formBuilder.control(i.interestFunctionId, {validators: [Validators.required]}),
                interestLegalFormId: this.formBuilder.control({value: i.interestLegalFormId, disabled: true}),
                legalFormId: this.formBuilder.control(i.legalFormId, {validators: [Validators.required]}),
                uidOrganisationId: this.formBuilder.control(i.uidOrganisationId),
                beginDate: this.formBuilder.control(i.beginDate),
                endDate: this.formBuilder.control(i.endDate),
                rowVersion: this.formBuilder.control(i.rowVersion),
                isInactiveRow: this.formBuilder.control(i.isInactive),
                isUidRow: this.formBuilder.control(i.uidOrganisationId != null),
            });

            formGroup.controls.beginDate.setValidators([rangeValidator(formGroup.controls.endDate, Comparison.LowerThanEqual)]);
            formGroup.controls.endDate.setValidators(rangeValidator(formGroup.controls.beginDate, Comparison.GreaterThanEqual));

            this.formArray.push(formGroup);
        });
    }

    private addEmptyInterest() {
        const formGroup = new UntypedFormGroup({
            id: this.formBuilder.control(null),
            personId: this.formBuilder.control(this.personId),
            interestText: this.formBuilder.control(null, {validators: [Validators.required]}),
            text: this.formBuilder.control({value: null, disabled: true}),
            interestCommitteeId: this.formBuilder.control(null, {validators: [Validators.required]}),
            interestFunctionId: this.formBuilder.control(null, {validators: [Validators.required]}),
            interestLegalFormId: this.formBuilder.control({value: null, disabled: true}),
            legalFormId: this.formBuilder.control(null, {validators: [Validators.required]}),
            uidOrganisationId: this.formBuilder.control(null),
            beginDate: this.formBuilder.control(null),
            endDate: this.formBuilder.control(null),
            rowVersion: this.formBuilder.control(0),
            isInactiveRow: this.formBuilder.control(null),
            isUidRow: this.formBuilder.control(null),
        });

        formGroup.controls.beginDate.setValidators([rangeValidator(formGroup.controls.endDate, Comparison.LowerThanEqual)]);
        formGroup.controls.endDate.setValidators(rangeValidator(formGroup.controls.beginDate, Comparison.GreaterThanEqual));

        this.formArray.push(formGroup);
    }

    private markForm(): void {
        this.formArray.markAsDirty();
        this.formArray.markAsTouched();
    }
}
