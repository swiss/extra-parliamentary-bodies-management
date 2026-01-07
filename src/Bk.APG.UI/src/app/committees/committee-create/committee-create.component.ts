import {Component, computed, signal, viewChild} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {Router} from '@angular/router';
import {CommitteeCreate} from '@api/CommitteeCreate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {CommitteesService} from '../committees.service';
import {CommitteeDataFormComponent} from '../shared/committee-data-form/committee-data-form.component';

@Component({
    selector: 'apg-committee-create',
    templateUrl: './committee-create.component.html',
    styleUrl: './committee-create.component.scss',
    imports: [MatButton, ObButtonDirective, CommitteeDataFormComponent, TranslatePipe, HelpTooltipComponent],
})
export class CommitteeCreateComponent {
    committeeToCreate = signal({} as CommitteeCreate);
    formComponent = viewChild.required(CommitteeDataFormComponent);
    form = computed(() => this.formComponent().committeeForm);
    isExtraParliamentaryCommission = false;

    constructor(
        private readonly router: Router,
        private readonly committeesService: CommitteesService,
        private readonly notificationService: ObNotificationService,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {
        this.committeesService.getCommitteeForCreate().subscribe(committeeToCreate => this.committeeToCreate.set(committeeToCreate));
    }

    save() {
        if (!this.form().valid) {
            this.form().markAsTouched();
            return;
        }

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.committeesService.createCommittee(this.committeeToCreate()).subscribe({
            next: async committee => {
                this.form().reset(this.committeeToCreate());
                this.committeesService.reload$.next();
                await this.router.navigate(['committees', committee.id], {replaceUrl: true, queryParams: {tab: 'data'}});
                return this.notificationService.success('committee.details.data.success');
            },
            error: () => this.notificationService.error('committee.details.data.error'),
        });
    }

    close() {
        void this.router.navigate(['committees']);
    }

    handleExtraParliamentaryCommission(value: boolean) {
        this.isExtraParliamentaryCommission = value;
    }
}
