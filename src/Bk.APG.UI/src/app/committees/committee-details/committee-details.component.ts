import {Component, effect, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatTab, MatTabGroup, MatTabLabel} from '@angular/material/tabs';
import {ActivatedRoute, Router} from '@angular/router';
import {TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService} from '@oblique/oblique';
import {EntityAuditLogComponent} from '@shared/entity-audit-log/entity-audit-log.component';
import {UnsavedChangesIconComponent} from '@shared/unsaved-changes-icon/unsaved-changes-icon.component';
import {AuthService} from '../../auth/auth.service';
import {AppointmentDecisionListComponent} from './appointment-decisions/appointment-decision-list/appointment-decision-list.component';
import {ContactPointListComponent} from './committee-contact-points/contact-point-list/contact-point-list.component';
import {CommitteeDataComponent} from './committee-data/committee-data.component';
import {CommitteeDetailsService} from './committee-details.service';
import {CommitteeJustificationsComponent} from './committee-justifications/committee-justifications.component';
import {CommitteeMembersComponent} from './committee-members/committee-members.component';
import {CommitteeOverviewComponent} from './committee-overview/committee-overview.component';

@Component({
    selector: 'apg-committee-details',
    templateUrl: './committee-details.component.html',
    styleUrl: './committee-details.component.scss',
    providers: [CommitteeDetailsService],
    imports: [
        MatTabGroup,
        MatTab,
        CommitteeOverviewComponent,
        MatTabLabel,
        UnsavedChangesIconComponent,
        CommitteeDataComponent,
        CommitteeMembersComponent,
        ContactPointListComponent,
        CommitteeJustificationsComponent,
        AppointmentDecisionListComponent,
        EntityAuditLogComponent,
        TranslatePipe,
    ],
})
export class CommitteeDetailsComponent {
    selectedIndex = signal(0);
    private readonly tabMap = [
        {name: 'overview', index: 0},
        {name: 'data', index: 1},
        {name: 'members', index: 2},
        {name: 'candidateList', index: 2},
        {name: 'contacts', index: 3},
        {name: 'justifications', index: 4},
        {name: 'decisions', index: 5},
        {name: 'audit', index: 6},
    ];

    constructor(
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly notificationService: ObNotificationService,
        protected readonly committeeDetailsService: CommitteeDetailsService,
        protected readonly authService: AuthService
    ) {
        effect(() => {
            const tabName = this.tabMap.find(x => x.index === this.selectedIndex())?.name || 'overview';
            void this.router.navigate([], {replaceUrl: true, queryParams: {tab: tabName}});
        });

        this.route.queryParams.pipe(takeUntilDestroyed()).subscribe(params => {
            const index = this.tabMap.find(x => x.name === params.tab)?.index || 0;
            this.selectedIndex.set(index);
        });
    }

    onTabGroupClick(event: MouseEvent) {
        const target = event.target as HTMLElement;
        if (target.classList.contains('mat-mdc-tab-disabled')) {
            this.notificationService.warning({message: 'common.saveReminder', timeout: 7000});
        }
    }
}
