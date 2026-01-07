import {Component, computed, DestroyRef, OnInit, signal, viewChild} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {PristineChangeEvent} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatDialog} from '@angular/material/dialog';
import {ActivatedRoute, Router} from '@angular/router';
import {CommitteeUpdate} from '@api/CommitteeUpdate';
import {TranslateService, TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService, ObButtonDirective, ObHttpApiInterceptorEvents} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {filter, switchMap} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';
import {CommitteesService} from '../../committees.service';
import {CommitteeDataFormComponent} from '../../shared/committee-data-form/committee-data-form.component';
import {CommitteeDetailsService} from '../committee-details.service';

@Component({
    selector: 'apg-committee-data',
    templateUrl: './committee-data.component.html',
    styleUrl: './committee-data.component.scss',
    imports: [MatButton, ObButtonDirective, CommitteeDataFormComponent, TranslatePipe],
})
export class CommitteeDataComponent implements OnInit {
    committeeUpdate = signal<CommitteeUpdate | undefined>(undefined);
    formComponent = viewChild.required(CommitteeDataFormComponent);
    form = computed(() => this.formComponent().committeeForm);
    isExtraParliamentaryCommission = false;

    private unmodifiedCommittee!: CommitteeUpdate;

    private readonly isAdmin = toSignal(this.authService.isAdmin$);

    constructor(
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly committeesService: CommitteesService,
        private readonly committeeDetailsService: CommitteeDetailsService,
        private readonly notificationService: ObNotificationService,
        private readonly dr: DestroyRef,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents,
        private readonly entityAuditLogService: EntityAuditLogService,
        private readonly dialog: MatDialog,
        private readonly translateService: TranslateService,
        private readonly authService: AuthService
    ) {
        this.committeesService.reload$
            .pipe(
                takeUntilDestroyed(),
                switchMap(() => this.committeesService.getCommitteeForUpdate(this.route.snapshot.params.id))
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
                    this.committeeDetailsService.isDataFormDirty.set(!event.pristine);
                }
            });
    }

    save() {
        if (!this.form().valid) {
            this.form().markAsTouched();
            return;
        }

        // if a non-admin user sets the end date in update update, ask for confirmation
        if (!this.isAdmin() && !!this.committeeUpdate()?.endDate && this.committeeUpdate()?.endDate !== this.unmodifiedCommittee.endDate) {
            this.dialog
                .open(ConfirmDialogComponent, {
                    width: '600px',
                    data: {
                        title: this.translateService.instant('committee.endDate.confirmation.title'),
                        message: this.translateService.instant('committee.endDate.confirmation.message'),
                    },
                })
                .afterClosed()
                .pipe(
                    takeUntilDestroyed(this.dr),
                    filter(result => result === true)
                )
                .subscribe(() => this.performSave());
        } else {
            this.performSave();
        }
    }

    reset() {
        this.form().reset(this.unmodifiedCommittee);
    }

    handleExtraParliamentaryCommission(value: boolean) {
        this.isExtraParliamentaryCommission = value;
    }

    private performSave() {
        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();
        this.committeesService.updateCommittee(this.committeeUpdate()!).subscribe({
            next: async () => {
                this.unmodifiedCommittee = this.committeeUpdate()!;
                this.formComponent().markEndDateAsSaved();
                this.committeesService.reload$.next();
                this.entityAuditLogService.reload$.next();
                await this.router.navigate([]);
                return this.notificationService.success('committee.details.data.success');
            },
            error: () => this.notificationService.error('committee.details.data.error'),
        });
    }
}
