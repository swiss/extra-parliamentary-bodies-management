import {CdkTextareaAutosize} from '@angular/cdk/text-field';
import {Component, Inject, OnInit, signal} from '@angular/core';
import {FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogTitle} from '@angular/material/dialog';
import {MatInput} from '@angular/material/input';
import {MatError, MatFormField, MatLabel, MatOption, MatSelect} from '@angular/material/select';
import {EiamAssignment} from '@api/EiamAssignment';
import {ReadyForProposalForward} from '@api/ReadyForProposalForward';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObErrorMessagesDirective, ObMatErrorDirective, ObNotificationService} from '@oblique/oblique';
import {GeneralElectionCommitteeDetailsService} from '../../ge-committee-details.service';
import {GeneralElectionCommitteeDataService} from '../ge-committee-data.service';

@Component({
    selector: 'apg-ready-for-proposal-forward-dialog',
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
    templateUrl: './ready-for-proposal-forward-dialog.component.html',
    styleUrl: './ready-for-proposal-forward-dialog.component.scss',
})
export class ReadyForProposalForwardDialogComponent implements OnInit {
    availableAssignments = signal<EiamAssignment[]>([]);
    form = this.buildForm();

    constructor(
        @Inject(MAT_DIALOG_DATA) private readonly data: {committeeId: string},
        private readonly formBuilder: FormBuilder,
        private readonly dataService: GeneralElectionCommitteeDataService,
        private readonly detailsService: GeneralElectionCommitteeDetailsService,
        private readonly notificationService: ObNotificationService
    ) {}

    ngOnInit(): void {
        this.dataService.getAssignmentsForReadyForProposalForward(this.data.committeeId).subscribe(assignments => this.availableAssignments.set(assignments));
    }

    forward() {
        if (!this.form.valid) {
            this.form.markAllAsTouched();
            return;
        }

        this.dataService.forwardReadyForProposal(this.data.committeeId, this.form.getRawValue() as ReadyForProposalForward).subscribe({
            next: () => {
                this.detailsService.reload$.next();
                this.notificationService.success('generalElection.committee.data.readyForProposal.forward.success');
            },
            error: () => this.notificationService.error('generalElection.committee.data.readyForProposal.forward.error'),
        });
    }

    private buildForm() {
        return this.formBuilder.group({
            forwardToId: new FormControl('', {nonNullable: true, validators: Validators.required}),
            description: new FormControl('', {nonNullable: true, validators: Validators.required}),
        });
    }
}
