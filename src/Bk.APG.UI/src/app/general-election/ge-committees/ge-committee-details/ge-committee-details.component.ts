import {Component, effect, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatTab, MatTabGroup, MatTabLabel} from '@angular/material/tabs';
import {ActivatedRoute, Router} from '@angular/router';
import {TranslatePipe} from '@ngx-translate/core';
import {EntityAuditLogComponent} from '@shared/entity-audit-log/entity-audit-log.component';
import {UnsavedChangesIconComponent} from '@shared/unsaved-changes-icon/unsaved-changes-icon.component';
import {GeneralElectionCommitteeCandidateListComponent} from './ge-committee-candidate-list/ge-committee-candidate-list.component';
import {GeneralElectionCommitteeDataComponent} from './ge-committee-data/ge-committee-data.component';
import {GeneralElectionCommitteeDetailsService} from './ge-committee-details.service';
import {GeneralElectionCommitteeJustificationsComponent} from './ge-committee-justifications/ge-committee-justifications.component';
import {GeneralElectionCommitteeMembershipsComponent} from './ge-committee-memberships/ge-committee-memberships.component';
import {GeneralElectionCommitteeOverviewComponent} from './ge-committee-overview/ge-committee-overview.component';

@Component({
    selector: 'apg-ge-committee-details',
    templateUrl: './ge-committee-details.component.html',
    styleUrl: './ge-committee-details.component.scss',
    providers: [GeneralElectionCommitteeDetailsService],
    imports: [
        MatTabGroup,
        MatTab,
        MatTabLabel,
        TranslatePipe,
        GeneralElectionCommitteeOverviewComponent,
        GeneralElectionCommitteeCandidateListComponent,
        GeneralElectionCommitteeDataComponent,
        GeneralElectionCommitteeJustificationsComponent,
        GeneralElectionCommitteeMembershipsComponent,
        UnsavedChangesIconComponent,
        EntityAuditLogComponent,
    ],
})
export class GeneralElectionCommitteeDetailsComponent {
    selectedIndex = signal(0);
    private readonly tabMap = [
        {name: 'overview', index: 0},
        {name: 'data', index: 1},
        {name: 'members', index: 2},
        {name: 'candidateList', index: 3},
        {name: 'contacts', index: 4},
        {name: 'justifications', index: 5},
        {name: 'decisions', index: 6},
        {name: 'audit', index: 7},
    ];

    constructor(
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        protected readonly generalElectionCommitteeDetailsService: GeneralElectionCommitteeDetailsService
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
}
