import {CdkTextareaAutosize} from '@angular/cdk/text-field';
import {Component, Inject, OnInit, signal} from '@angular/core';
import {FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogTitle} from '@angular/material/dialog';
import {MatInput} from '@angular/material/input';
import {MatError, MatFormField, MatLabel, MatOption, MatSelect} from '@angular/material/select';
import {CandidateListForward} from '@api/CandidateListForward';
import {EiamAssignment} from '@api/EiamAssignment';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObErrorMessagesDirective, ObMatErrorDirective, ObNotificationService} from '@oblique/oblique';
import {GeneralElectionCommitteeDetailsService} from '../../ge-committee-details.service';
import {GeneralElectionCommitteeCandidateListService} from '../ge-committee-candidate-list.service';

@Component({
    selector: 'apg-candidate-list-forward-dialog',
    imports: [
        MatDialogContent,
        MatDialogActions,
        MatButton,
        ObButtonDirective,
        MatDialogClose,
        MatDialogTitle,
        TranslatePipe,
        FormsModule,
        MatError,
        MatFormField,
        MatLabel,
        MatOption,
        MatSelect,
        ObErrorMessagesDirective,
        ObMatErrorDirective,
        ReactiveFormsModule,
        CdkTextareaAutosize,
        MatInput,
    ],
    templateUrl: './candidate-list-forward-dialog.component.html',
    styleUrl: './candidate-list-forward-dialog.component.scss',
})
export class CandidateListForwardDialogComponent implements OnInit {
    availableAssignments = signal<EiamAssignment[]>([]);
    form = this.buildForm();

    constructor(
        @Inject(MAT_DIALOG_DATA) private readonly data: {committeeId: string; candidateIds: string[]},
        private readonly formBuilder: FormBuilder,
        private readonly candidateListService: GeneralElectionCommitteeCandidateListService,
        private readonly detailsService: GeneralElectionCommitteeDetailsService,
        private readonly notificationService: ObNotificationService
    ) {}

    ngOnInit(): void {
        this.candidateListService
            .getAssignmentsForCandidateListForward(this.data.committeeId)
            .subscribe(assignments => this.availableAssignments.set(assignments));
    }

    forward() {
        if (!this.form.valid) {
            this.form.markAllAsTouched();
            return;
        }

        this.candidateListService
            .forwardCandidateList(this.data.committeeId, {candidateIds: this.data.candidateIds, ...this.form.getRawValue()} as CandidateListForward)
            .subscribe({
                next: () => {
                    this.detailsService.reload$.next();
                    this.notificationService.success('generalElection.committee.candidateList.forward.success');
                },
                error: () => this.notificationService.error('generalElection.committee.candidateList.forward.error'),
            });
    }

    private buildForm() {
        return this.formBuilder.group({
            forwardToId: new FormControl('', {nonNullable: true, validators: Validators.required}),
            description: new FormControl('', {nonNullable: true, validators: Validators.required}),
        });
    }
}
