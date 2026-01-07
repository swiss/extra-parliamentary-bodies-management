import {Component, computed, signal, viewChild, WritableSignal} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute, Router} from '@angular/router';
import {AppointmentDecisionUpdate} from '@api/AppointmentDecisionUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService, ObButtonDirective} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {ConfigsService} from '../../../../configs.service';
import {CommitteesService} from '../../../committees.service';
import {AppointmentDecisionService} from '../appointment-decision.service';
import {AppointmentDecisionDataFormComponent} from '../shared/appointment-decision-data-form/appointment-decision-data-form.component';

@Component({
    selector: 'apg-appointment-decision-edit',
    imports: [MatButton, ObButtonDirective, AppointmentDecisionDataFormComponent, TranslatePipe],
    templateUrl: './appointment-decision-edit.component.html',
    styleUrl: './appointment-decision-edit.component.scss',
})
export class AppointmentDecisionEditComponent {
    appointmentDecisionToUpdate!: WritableSignal<AppointmentDecisionUpdate>;

    formComponent = viewChild.required(AppointmentDecisionDataFormComponent);
    form = computed(() => this.formComponent().appointmentDecisionForm);
    documentForm = computed(() => this.formComponent().documentsForm);
    isOriginalSelected = computed(() => !!this.appointmentDecisionToUpdate()?.documents?.find(doc => doc.isOriginal));
    isLinkProvided = computed(() => !!this.appointmentDecisionToUpdate()?.link);

    isInstitution = computed(
        () =>
            this.appointmentDecisionToUpdate()?.appointmentDecisionTypeId === this.configsService.frontendConfig.entityIds.appointmentDecisionType.institutionId
    );

    isReport = computed(
        () => this.appointmentDecisionToUpdate()?.appointmentDecisionTypeId === this.configsService.frontendConfig.entityIds.appointmentDecisionType.reportId
    );

    committeeId = '';
    appointmentDecisionId = '';
    private unmodifiedAppointmentDecision!: AppointmentDecisionUpdate;

    constructor(
        protected readonly errorService: ErrorService,
        protected readonly appointmentDecisionService: AppointmentDecisionService,
        private readonly committeesService: CommitteesService,
        private readonly notificationService: ObNotificationService,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly configsService: ConfigsService
    ) {
        this.committeeId = this.route.parent?.snapshot.params.id;
        this.appointmentDecisionId = this.route.snapshot.params.id;
        this.appointmentDecisionService.getAppointmentDecisionForUpdate(this.appointmentDecisionId).subscribe(appointmentDecision => {
            this.appointmentDecisionToUpdate = signal(appointmentDecision);
            this.unmodifiedAppointmentDecision = appointmentDecision;
        });
    }

    save() {
        if (!this.form().valid) {
            this.form().markAsTouched();
            return;
        }
        this.appointmentDecisionService.updateAppointmentDecision(this.appointmentDecisionId, this.appointmentDecisionToUpdate()).subscribe({
            next: async () => {
                this.form().reset();
                this.committeesService.reload$.next();
                void this.router.navigate(['committees', this.committeeId], {replaceUrl: true, queryParams: {tab: 'decisions'}});

                return this.notificationService.success('appointmentDecision.save.success');
            },
            error: () => this.notificationService.error('appointmentDecision.save.error'),
        });
    }

    reset() {
        this.form().reset(this.unmodifiedAppointmentDecision, {emitEvent: true});
        this.documentForm().reset();
        this.formComponent().appointmentDecisionModification.set(this.unmodifiedAppointmentDecision);
        this.formComponent().rebuildDocumentsForm(this.unmodifiedAppointmentDecision);
    }

    back() {
        void this.router.navigate(['committees', this.committeeId], {replaceUrl: true, queryParams: {tab: 'decisions'}});
    }
}
