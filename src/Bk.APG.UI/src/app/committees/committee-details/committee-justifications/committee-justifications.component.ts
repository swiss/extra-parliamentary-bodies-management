import {Component, computed, effect, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute, Router} from '@angular/router';
import {CommitteeJustificationForm} from '@api/CommitteeJustificationForm';
import {CommitteeJustificationUpdate} from '@api/CommitteeJustificationUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObAlertModule, ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {RichTextEditorComponent} from '@shared/rich-text-editor/rich-text-editor.component';
import {switchMap} from 'rxjs';
import {GeneralElectionService} from '../../../general-election/general-election.service';
import {CommitteesService} from '../../committees.service';
import {CommitteeDetailsService} from '../committee-details.service';

@Component({
    selector: 'apg-committee-justifications',
    templateUrl: './committee-justifications.component.html',
    styleUrl: './committee-justifications.component.scss',
    imports: [ReactiveFormsModule, MatButton, ObButtonDirective, HelpTooltipComponent, RichTextEditorComponent, TranslatePipe, ObAlertModule],
})
export class CommitteeJustificationsComponent {
    committeeJustificationForm!: FormGroup<CommitteeJustificationForm>;
    committeeJustificationUpdate = signal<CommitteeJustificationUpdate | undefined>(undefined);

    canEdit = computed(() => this.committeeDetails()?.canCreateJustification);

    protected readonly committeeDetails = computed(() => this.committeeDetailsService.committeeDetails());

    private unmodifiedCommitteeJustification!: CommitteeJustificationUpdate;

    constructor(
        protected readonly formBuilder: FormBuilder,
        protected readonly generalElectionService: GeneralElectionService,
        private readonly committeeDetailsService: CommitteeDetailsService,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly committeesService: CommitteesService,
        private readonly notificationService: ObNotificationService,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents,
        private readonly entityAuditLogService: EntityAuditLogService
    ) {
        this.committeeJustificationForm = this.createForm();

        effect(() => {
            const committeeJustificationUpdate = this.committeeJustificationUpdate();
            if (committeeJustificationUpdate?.id) {
                this.committeeJustificationForm.patchValue(committeeJustificationUpdate);
            }
        });

        effect(() => {
            const canEdit = this.canEdit();
            if (canEdit) {
                this.committeeJustificationForm.enable();
            } else {
                this.committeeJustificationForm.disable();
            }
        });

        this.committeesService.reload$
            .pipe(
                takeUntilDestroyed(),
                switchMap(() => this.committeesService.getCommitteeJustificationForUpdate(this.route.snapshot.params.id))
            )
            .subscribe(committeeJustificationUpdate => {
                this.committeeJustificationUpdate.set(committeeJustificationUpdate);
                this.unmodifiedCommitteeJustification = committeeJustificationUpdate;
            });
    }

    reset() {
        this.committeeJustificationForm.reset(this.unmodifiedCommitteeJustification);
    }

    save() {
        if (!this.committeeJustificationForm.valid) {
            this.committeeJustificationForm.markAsTouched();
            return;
        }

        const formValues = this.committeeJustificationForm.getRawValue();
        const update = {...this.committeeJustificationUpdate(), ...formValues} as CommitteeJustificationUpdate;

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.committeesService.updateCommitteeJustification(update).subscribe({
            next: async () => {
                this.committeeJustificationForm.reset(update);
                this.unmodifiedCommitteeJustification = update;
                this.committeesService.reload$.next();
                this.entityAuditLogService.reload$.next();
                await this.router.navigate([]);
                return this.notificationService.success('committee.details.justification.success');
            },
            error: () => this.notificationService.error('committee.details.justification.error'),
        });
    }

    private createForm(): FormGroup<CommitteeJustificationForm> {
        return this.formBuilder.group<CommitteeJustificationForm>({
            justificationMembers: this.formBuilder.control<string | null>(null),
            justificationGenders: this.formBuilder.control<string | null>(null),
            measuresGenders: this.formBuilder.control<string | null>(null),
            justificationLanguages: this.formBuilder.control<string | null>(null),
            measuresLanguages: this.formBuilder.control<string | null>(null),
        });
    }
}
