import {Component, effect, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatTabGroup, MatTab, MatTabLabel} from '@angular/material/tabs';
import {ActivatedRoute, Router} from '@angular/router';
import {TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService} from '@oblique/oblique';
import {EntityAuditLogComponent} from '@shared/entity-audit-log/entity-audit-log.component';
import {Subject, takeUntil} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {UnsavedChangesIconComponent} from '../../shared/unsaved-changes-icon/unsaved-changes-icon.component';
import {PersonDataComponent} from './person-data/person-data.component';
import {PersonDetailsService} from './person-details.service';
import {PersonInterestsComponent} from './person-interests/person-interests.component';
import {PersonMembershipsComponent} from './person-memberships/person-memberships.component';
import {PersonOverviewComponent} from './person-overview/person-overview.component';

@Component({
    selector: 'apg-person-details',
    templateUrl: './person-details.component.html',
    styleUrl: './person-details.component.scss',
    providers: [PersonDetailsService],
    imports: [
        MatTabGroup,
        MatTab,
        PersonOverviewComponent,
        MatTabLabel,
        UnsavedChangesIconComponent,
        PersonDataComponent,
        PersonInterestsComponent,
        PersonMembershipsComponent,
        EntityAuditLogComponent,
        TranslatePipe,
    ],
})
export class PersonDetailsComponent {
    selectedIndex = signal(0);

    isObserver = false;

    private readonly tabMap = [
        {name: 'overview', index: 0},
        {name: 'data', index: 1},
        {name: 'interests', index: 2},
        {name: 'memberships', index: 3},
        {name: 'audit', index: 4},
    ];

    private readonly unsubscribe = new Subject<void>();

    constructor(
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly notificationService: ObNotificationService,
        private readonly authService: AuthService,
        protected readonly personDetailsService: PersonDetailsService
    ) {
        effect(() => {
            const tabName = this.tabMap.find(x => x.index === this.selectedIndex())?.name || 'overview';
            void this.router.navigate([], {replaceUrl: true, queryParams: {tab: tabName}});
        });

        this.route.queryParams.pipe(takeUntilDestroyed()).subscribe(params => {
            const index = this.tabMap.find(x => x.name === params.tab)?.index || 0;
            this.selectedIndex.set(index);
        });

        this.authService.isObserver$.pipe(takeUntil(this.unsubscribe)).subscribe(isObserver => {
            this.isObserver = isObserver;
        });
    }

    onTabGroupClick(event: MouseEvent) {
        const target = event.target as HTMLElement;
        if (target.classList.contains('mat-mdc-tab-disabled') && !this.isObserver) {
            this.notificationService.warning({message: 'common.saveReminder', timeout: 7000});
        }
    }
}
