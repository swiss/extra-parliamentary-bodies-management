import {Component, computed, DestroyRef, OnInit, signal, viewChild, ViewContainerRef} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {PristineChangeEvent} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatChip} from '@angular/material/chips';
import {MatDialog} from '@angular/material/dialog';
import {ActivatedRoute, Router} from '@angular/router';
import {GeneralElectionCommitteeUpdate} from '@api/GeneralElectionCommitteeUpdate';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObButtonDirective, ObNotificationService} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {filter, switchMap} from 'rxjs';
import {GeneralElectionCommitteeDataFormComponent} from '../ge-committee-data-form/ge-committee-data-form.component';
import {GeneralElectionCommitteeDetailsService} from '../ge-committee-details.service';
import {GeneralElectionCommitteeDataService} from './ge-committee-data.service';
import {ReadyForProposalForwardDialogComponent} from './ready-for-proposal-forward-dialog/ready-for-proposal-forward-dialog.component';

@Component({
    selector: 'apg-ge-committee-data',
    imports: [MatButton, ObButtonDirective, TranslatePipe, GeneralElectionCommitteeDataFormComponent, MatChip],
    templateUrl: './ge-committee-data.component.html',
    styleUrl: './ge-committee-data.component.scss',
})
export class GeneralElectionCommitteeDataComponent implements OnInit {
    committeeUpdate = signal<GeneralElectionCommitteeUpdate | undefined>(undefined);
    formComponent = viewChild.required(GeneralElectionCommitteeDataFormComponent);
    form = computed(() => this.formComponent().committeeForm);
    readyForProposalAssignedTo = computed(() => this.generalElectionCommitteeDetailsService.committeeDetails()?.readyForProposalAssignedTo);
    canForwardReadyForProposal = computed(() => this.generalElectionCommitteeDetailsService.committeeDetails()?.canForwardReadyForProposal ?? false);
    canFinalizeReadyForProposal = computed(() => this.generalElectionCommitteeDetailsService.committeeDetails()?.canFinalizeReadyForProposal ?? false);
    isReadyForProposal = computed(() => this.generalElectionCommitteeDetailsService.committeeDetails()?.isReadyForProposal ?? false);
    isExtraParliamentaryCommission = false;

    private unmodifiedCommittee!: GeneralElectionCommitteeUpdate;

    constructor(
        private readonly generalElectionCommitteeDataService: GeneralElectionCommitteeDataService,
        protected readonly generalElectionCommitteeDetailsService: GeneralElectionCommitteeDetailsService,
        private readonly dr: DestroyRef,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly notificationService: ObNotificationService,
        private readonly translateService: TranslateService,
        private readonly entityAuditLogService: EntityAuditLogService,
        private readonly dialog: MatDialog,
        private readonly viewContainerRef: ViewContainerRef
    ) {
        this.generalElectionCommitteeDetailsService.reload$
            .pipe(
                takeUntilDestroyed(),
                switchMap(() => this.generalElectionCommitteeDataService.getGeneralElectionCommitteeForUpdate(this.route.snapshot.params.id))
            )
            .subscribe(committeeUpdate => {
                this.committeeUpdate.set(committeeUpdate);
                this.form().reset(this.committeeUpdate());
                this.unmodifiedCommittee = committeeUpdate;
            });
    }

    ngOnInit() {
        this.form()
            .events.pipe(takeUntilDestroyed(this.dr))
            .subscribe(event => {
                if (event instanceof PristineChangeEvent) {
                    this.generalElectionCommitteeDetailsService.isDataFormDirty.set(!event.pristine);
                }
            });
    }

    handleExtraParliamentaryCommission(value: boolean) {
        this.isExtraParliamentaryCommission = value;
    }

    openForwardDialog() {
        this.dialog.open(ReadyForProposalForwardDialogComponent, {
            data: {committeeId: this.route.snapshot.params.id},
            viewContainerRef: this.viewContainerRef,
        });
    }

    finalizeReadyForProposal() {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            width: '500px',
            viewContainerRef: this.viewContainerRef,
            data: {
                title: this.translateService.instant('generalElection.committee.data.readyForProposal.finalize.confirm.title'),
                message: this.translateService.instant('generalElection.committee.data.readyForProposal.finalize.confirm.text'),
            },
        });

        dialogRef
            .afterClosed()
            .pipe(
                filter(result => result !== undefined),
                switchMap(() => this.generalElectionCommitteeDataService.finalizeReadyForProposal(this.route.snapshot.params.id)),
                takeUntilDestroyed(this.dr)
            )
            .subscribe({
                next: validationResult => {
                    this.generalElectionCommitteeDetailsService.reload$.next();

                    if (validationResult.allValidationsPassed) {
                        this.entityAuditLogService.reload$.next();
                        this.notificationService.success('generalElection.committee.data.readyForProposal.finalize.success');
                        return;
                    }

                    this.notificationService.warning('generalElection.committee.data.readyForProposal.finalize.openTasksHint');
                },
                error: () => this.notificationService.error('generalElection.committee.data.readyForProposal.finalize.error'),
            });
    }

    reset() {
        this.form().reset(this.unmodifiedCommittee);
    }

    save() {
        if (!this.form().valid) {
            this.form().markAsTouched();
            return;
        }

        this.generalElectionCommitteeDataService.updateGeneralElectionCommittee(this.committeeUpdate()!).subscribe({
            next: async () => {
                this.unmodifiedCommittee = this.committeeUpdate()!;
                this.generalElectionCommitteeDetailsService.reload$.next();
                this.entityAuditLogService.reload$.next();
                await this.router.navigate([]);
                return this.notificationService.success('generalElection.committee.details.data.success');
            },
            error: () => this.notificationService.error('generalElection.committee.details.data.error'),
        });
    }
}
