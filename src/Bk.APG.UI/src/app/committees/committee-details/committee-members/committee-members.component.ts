import {DatePipe, NgClass} from '@angular/common';
import {Component, computed, signal, WritableSignal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatSort, MatSortHeader, Sort} from '@angular/material/sort';
import {
    MatCell,
    MatCellDef,
    MatColumnDef,
    MatHeaderCell,
    MatHeaderCellDef,
    MatHeaderRow,
    MatHeaderRowDef,
    MatNoDataRow,
    MatRow,
    MatRowDef,
    MatTable,
} from '@angular/material/table';
import {MatTooltip} from '@angular/material/tooltip';
import {ActivatedRoute, Router} from '@angular/router';
import {CommitteeMember} from '@api/CommitteeMember';
import {CommitteeQuotas} from '@api/CommitteeQuotas';
import {MembershipList} from '@api/MembershipList';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObAlertModule, ObButtonDirective} from '@oblique/oblique';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MembersQuotasComponent} from '@shared/members-quotas/members-quotas.component';
import {distinctUntilChanged, startWith, switchMap} from 'rxjs';
import {GeneralElectionService} from '../../../general-election/general-election.service';
import {CommitteesService} from '../../committees.service';
import {CommitteeDetailsService} from '../committee-details.service';

interface PagingParams {
    pageIndex: number;
    pageSize: number;
}

@Component({
    selector: 'apg-committee-members',
    templateUrl: './committee-members.component.html',
    styleUrl: './committee-members.component.scss',
    imports: [
        MatButton,
        ObButtonDirective,
        MatIcon,
        MatTable,
        MatSort,
        MatColumnDef,
        MatHeaderCellDef,
        MatHeaderCell,
        MatSortHeader,
        MatCellDef,
        MatCell,
        MatHeaderRowDef,
        MatHeaderRow,
        MatRowDef,
        MatRow,
        MatTooltip,
        NgClass,
        MatNoDataRow,
        MatPaginator,
        DatePipe,
        TranslatePipe,
        HelpTooltipComponent,
        MembersQuotasComponent,
        ObAlertModule,
    ],
})
export class CommitteeMembersComponent {
    membershipList = signal<MembershipList>({
        committeeQuotas: {} as CommitteeQuotas,
        activeMemberships: [],
        inactiveMemberships: [],
    });
    activeMembers = computed(() => this.membershipList().activeMemberships);
    inactiveMembers = computed(() => this.membershipList().inactiveMemberships);

    currentDate: Date = new Date();

    activeMembersSort = signal<Sort>({active: 'beginDate', direction: 'desc'});
    activeMembersPaging = signal<PagingParams>({pageIndex: 0, pageSize: 25});
    activeMembersData = computed(() => this.prepareData(this.activeMembers(), this.activeMembersPaging(), this.activeMembersSort()));

    inactiveMembersSort = signal<Sort>({active: 'beginDate', direction: 'desc'});
    inactiveMembersPaging = signal<PagingParams>({pageIndex: 0, pageSize: 25});
    inactiveMembersData = computed(() => this.prepareData(this.inactiveMembers(), this.inactiveMembersPaging(), this.inactiveMembersSort()));

    displayedColumns = computed(() =>
        this.committeeDetailsService.committeeDetails()?.marketOrientated
            ? ['surname', 'givenName', 'gender', 'language', 'function', 'beginDate', 'endDate', 'electionType', 'hasMembershipAddition', 'employmentLevel']
            : ['surname', 'givenName', 'gender', 'language', 'function', 'beginDate', 'endDate', 'electionType', 'hasMembershipAddition']
    );
    canEdit = computed(() => this.committeeDetailsService.committeeDetails()?.canEdit);

    tables = [
        {
            name: 'activeMembers',
            length: computed(() => this.activeMembers().length),
            data: this.activeMembersData,
            sort: this.activeMembersSort,
            paging: this.activeMembersPaging,
        },
        {
            name: 'inactiveMembers',
            length: computed(() => this.inactiveMembers().length),
            data: this.inactiveMembersData,
            sort: this.inactiveMembersSort,
            paging: this.inactiveMembersPaging,
        },
    ];

    constructor(
        protected readonly generalElectionService: GeneralElectionService,
        protected readonly committeeDetailsService: CommitteeDetailsService,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly translateService: TranslateService,
        private readonly committeesService: CommitteesService
    ) {
        this.translateService.onLangChange
            .pipe(
                startWith({lang: this.translateService.currentLang}),
                distinctUntilChanged((prev, curr) => prev.lang === curr.lang),
                switchMap(() => this.committeesService.getCommitteeMembers(this.route.snapshot.params.id)),
                takeUntilDestroyed()
            )
            .subscribe(membershipList => this.membershipList.set(membershipList));
    }

    editMember(committeeMember: CommitteeMember) {
        void this.router.navigate(['members', committeeMember.id], {relativeTo: this.route});
    }

    createMember() {
        void this.router.navigate(['members', 'create'], {relativeTo: this.route});
    }

    sortData(sort: Sort, currentSort: WritableSignal<Sort>) {
        currentSort.set(sort);
    }

    onPageChange($event: PageEvent, paging: WritableSignal<PagingParams>) {
        paging.set({pageIndex: $event.pageIndex, pageSize: $event.pageSize});
    }

    private prepareData(members: CommitteeMember[], paging: PagingParams, sort: Sort) {
        const {pageIndex, pageSize} = paging;
        const start = pageIndex * pageSize;
        return members.sort((a, b) => this.compare(a, b, sort)).slice(start, start + pageSize);
    }

    private compare(a: CommitteeMember, b: CommitteeMember, sort: Sort) {
        const key = sort.active as keyof CommitteeMember;
        return (a[key] < b[key] ? -1 : 1) * (sort.direction === 'asc' ? 1 : -1);
    }
}
