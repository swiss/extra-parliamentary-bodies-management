import {AfterViewInit, Component, computed, signal, viewChild, WritableSignal} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute, Router} from '@angular/router';
import {AppointmentDecisionCreate} from '@api/AppointmentDecisionCreate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService, ObButtonDirective} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {ConfigsService} from '../../../../configs.service';
import {CommitteesService} from '../../../committees.service';
import {AppointmentDecisionService} from '../appointment-decision.service';
import {AppointmentDecisionDataFormComponent} from '../shared/appointment-decision-data-form/appointment-decision-data-form.component';

@Component({
    selector: 'apg-appointment-decision-create',
    templateUrl: './appointment-decision-create.component.html',
    styleUrl: './appointment-decision-create.component.scss',
    imports: [MatButton, ObButtonDirective, HelpTooltipComponent, AppointmentDecisionDataFormComponent, TranslatePipe],
})
export class AppointmentDecisionCreateComponent implements AfterViewInit {
    appointmentDecisionToCreate!: WritableSignal<AppointmentDecisionCreate>;

    formComponent = viewChild.required(AppointmentDecisionDataFormComponent);
    form = computed(() => this.formComponent().appointmentDecisionForm);
    documentForm = computed(() => this.formComponent().documentsForm);
    isOriginalSelected = computed(() => !!this.appointmentDecisionToCreate()?.documents?.find(doc => doc.isOriginal));
    isLinkProvided = computed(() => !!this.appointmentDecisionToCreate()?.link);
    isInstitution = computed(
        () =>
            this.appointmentDecisionToCreate()?.appointmentDecisionTypeId === this.configsService.frontendConfig.entityIds.appointmentDecisionType.institutionId
    );

    isReport = computed(
        () => this.appointmentDecisionToCreate()?.appointmentDecisionTypeId === this.configsService.frontendConfig.entityIds.appointmentDecisionType.reportId
    );

    committeeId = '';

    constructor(
        protected readonly errorService: ErrorService,
        protected readonly appointmentDecisionService: AppointmentDecisionService,
        private readonly committeesService: CommitteesService,
        private readonly notificationService: ObNotificationService,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly configsService: ConfigsService
    ) {
        this.committeeId = this.route.snapshot.params.id;
        this.appointmentDecisionService
            .getAppointmentDecisionForCreate()
            .subscribe(appointmentDecision => (this.appointmentDecisionToCreate = signal(appointmentDecision)));
    }

    ngAfterViewInit() {
        const form = this.form();
        form.controls.appointmentDecisionTypeId.markAllAsTouched();
        form.controls.appointmentDecisionTypeId.updateValueAndValidity();
    }

    close() {
        void this.router.navigate(['committees', this.committeeId], {replaceUrl: true, queryParams: {tab: 'decisions'}});
    }

    save() {
        if (!this.form().valid) {
            this.form().markAsTouched();
            return;
        }
        this.appointmentDecisionService.createAppointmentDecision({...this.appointmentDecisionToCreate(), committeeId: this.committeeId}).subscribe({
            next: async () => {
                this.form().reset();
                this.committeesService.reload$.next();
                void this.router.navigate(['committees', this.committeeId], {replaceUrl: true, queryParams: {tab: 'decisions'}});

                return this.notificationService.success('appointmentDecision.save.success');
            },
            error: () => this.notificationService.error('appointmentDecision.save.error'),
        });
    }
}
