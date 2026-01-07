import {ChangeDetectionStrategy, Component, computed, effect, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatError} from '@angular/material/input';
import {ActivatedRoute, Router} from '@angular/router';
import {GeneralElectionCommitteeJustificationForm} from '@api/GeneralElectionCommitteeJustificationForm';
import {GeneralElectionCommitteeJustificationUpdate} from '@api/GeneralElectionCommitteeJustificationUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObAlertComponent, ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {ErrorService} from '@shared/error-service.service';
import {conditionalValidator} from '@shared/form-validators/conditional.validator';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {RichTextEditorComponent} from '@shared/rich-text-editor/rich-text-editor.component';
import {switchMap} from 'rxjs';
import {GeneralElectionCommitteesService} from '../../ge-committees.service';
import {GeneralElectionCommitteeDetailsService} from '../ge-committee-details.service';

@Component({
    selector: 'apg-ge-committee-justifications',
    templateUrl: './ge-committee-justifications.component.html',
    styleUrl: './ge-committee-justifications.component.scss',
    imports: [ReactiveFormsModule, MatButton, MatError, ObButtonDirective, RichTextEditorComponent, TranslatePipe, ObAlertComponent, HelpTooltipComponent],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GeneralElectionCommitteeJustificationsComponent {
    committeeJustificationForm!: FormGroup<GeneralElectionCommitteeJustificationForm>;
    committeeJustificationUpdate = signal<GeneralElectionCommitteeJustificationUpdate | undefined>(undefined);

    protected readonly committeeDetails = computed(() => this.generalElectionCommitteeDetailsService.committeeDetails());

    private unmodifiedCommitteeJustification!: GeneralElectionCommitteeJustificationUpdate;

    constructor(
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        protected readonly formBuilder: FormBuilder,
        private readonly generalElectionCommitteeDetailsService: GeneralElectionCommitteeDetailsService,
        private readonly generalElectionCommitteesService: GeneralElectionCommitteesService,
        private readonly notificationService: ObNotificationService,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents,
        protected readonly errorService: ErrorService,
        private readonly entityAuditLogService: EntityAuditLogService
    ) {
        this.committeeJustificationForm = this.createForm();
        this.setDynamicValidators();

        const effectRef = effect(() => {
            if ((this.committeeJustificationUpdate() as GeneralElectionCommitteeJustificationUpdate)?.id) {
                this.committeeJustificationForm.patchValue(this.committeeJustificationUpdate()!);

                effectRef.destroy();
            }
        });

        this.committeeJustificationForm.valueChanges.pipe(takeUntilDestroyed()).subscribe(() => {
            const formValues = this.committeeJustificationForm.getRawValue();
            this.committeeJustificationUpdate.update(value => ({...value, ...(formValues as unknown as GeneralElectionCommitteeJustificationUpdate)}));
        });

        this.generalElectionCommitteeDetailsService.reload$
            .pipe(
                switchMap(() => this.generalElectionCommitteesService.getGeneralElectionCommitteeJustificationForUpdate(this.route.snapshot.params.id)),
                takeUntilDestroyed()
            )
            .subscribe(committeeJustificationUpdate => {
                this.committeeJustificationUpdate.set(committeeJustificationUpdate);
                this.unmodifiedCommitteeJustification = committeeJustificationUpdate;
            });

        effect(() => {
            if (this.committeeDetails()) {
                if (!this.committeeDetails()!.canEdit || !this.committeeDetails()!.isValidated) {
                    this.committeeJustificationForm.disable();
                } else {
                    this.committeeJustificationForm.enable();
                    if (!this.committeeDetails()!.canEditSelectionProcedure) {
                        this.committeeJustificationForm.controls.selectionProcedure.disable();
                    }
                }
            }
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
        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();
        this.generalElectionCommitteesService.updateGeneralElectionCommitteeJustification(this.committeeJustificationUpdate()!).subscribe({
            next: async () => {
                this.committeeJustificationForm.reset(this.committeeJustificationUpdate());
                this.unmodifiedCommitteeJustification = this.committeeJustificationUpdate()!;
                this.generalElectionCommitteeDetailsService.reload$.next();
                this.entityAuditLogService.reload$.next();
                await this.router.navigate([]);
                return this.notificationService.success('committee.details.justification.success');
            },
            error: () => this.notificationService.error('committee.details.justification.error'),
        });
    }

    private setDynamicValidators() {
        effect(() => {
            const justificationGendersControl = this.committeeJustificationForm.controls.justificationGenders;
            const measuresGendersControl = this.committeeJustificationForm.controls.measuresGenders;
            const justificationLanguagesControl = this.committeeJustificationForm.controls.justificationLanguages;
            const measuresLanguagesControl = this.committeeJustificationForm.controls.measuresLanguages;

            if (this.committeeJustificationUpdate()?.isJustificationGendersRequired) {
                justificationGendersControl.setValidators([Validators.required]);

                if (!this.committeeDetails()?.generalGenderMeasure) {
                    measuresGendersControl.setValidators([Validators.required]);
                } else {
                    measuresGendersControl.clearValidators();
                }
            } else {
                justificationGendersControl.clearValidators();
                measuresGendersControl.clearValidators();
            }

            if (this.committeeJustificationUpdate()?.isJustificationLanguagesRequired) {
                justificationLanguagesControl.setValidators([Validators.required]);

                if (!this.committeeDetails()?.generalLanguageMeasure) {
                    measuresLanguagesControl.setValidators([Validators.required]);
                } else {
                    measuresLanguagesControl.clearValidators();
                }
            } else {
                justificationLanguagesControl.clearValidators();
                measuresLanguagesControl.clearValidators();
            }

            justificationGendersControl.updateValueAndValidity({emitEvent: false});
            measuresGendersControl.updateValueAndValidity({emitEvent: false});
            justificationLanguagesControl.updateValueAndValidity({emitEvent: false});
            measuresLanguagesControl.updateValueAndValidity({emitEvent: false});
        });
    }

    private createForm(): FormGroup<GeneralElectionCommitteeJustificationForm> {
        const form = this.formBuilder.group<GeneralElectionCommitteeJustificationForm>({
            selectionProcedure: this.formBuilder.control<string | null>(null),
            justificationMembers: this.formBuilder.control<string | null>(null),
            justificationGenders: this.formBuilder.control<string | null>(null),
            measuresGenders: this.formBuilder.control<string | null>(null),
            justificationLanguages: this.formBuilder.control<string | null>(null),
            measuresLanguages: this.formBuilder.control<string | null>(null),
        });

        form.controls.selectionProcedure.setValidators([
            conditionalValidator(
                () =>
                    !!this.generalElectionCommitteeDetailsService.committeeDetails()?.canEditSelectionProcedure &&
                    !!this.generalElectionCommitteeDetailsService.committeeDetails()?.federalInstitution,
                Validators.required
            ),
        ]);
        return form;
    }
}
