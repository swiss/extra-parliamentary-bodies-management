import {DatePipe} from '@angular/common';
import {Component, computed, signal, WritableSignal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
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
import {ActivatedRoute, Router} from '@angular/router';
import {CommitteeMember} from '@api/CommitteeMember';
import {CommitteeQuotas} from '@api/CommitteeQuotas';
import {MembershipList} from '@api/MembershipList';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {MembersQuotasComponent} from '@shared/members-quotas/members-quotas.component';
import {distinctUntilChanged, merge, switchMap} from 'rxjs';
import {GeneralElectionCommitteesService} from '../../ge-committees.service';
import {GeneralElectionCommitteeDetailsService} from '../ge-committee-details.service';

@Component({
    selector: 'apg-ge-committee-memberships',
    templateUrl: './ge-committee-memberships.component.html',
    styleUrl: './ge-committee-memberships.component.scss',
    imports: [
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
        MatNoDataRow,
        DatePipe,
        TranslatePipe,
        MembersQuotasComponent,
    ],
})
export class GeneralElectionCommitteeMembershipsComponent {
    membershipList = signal<MembershipList>({
        committeeQuotas: {} as CommitteeQuotas,
        activeMemberships: [],
        inactiveMemberships: [],
    });
    activeMembers = computed(() => this.membershipList().activeMemberships);
    activeMembersSort = signal<Sort>({active: 'beginDate', direction: 'desc'});
    activeMembersData = computed(() => this.prepareData(this.activeMembers(), this.activeMembersSort()));

    displayedColumns = computed(() =>
        this.generalElectionCommitteeDetailsService.committeeDetails()?.marketOrientated
            ? ['surname', 'givenName', 'gender', 'language', 'function', 'beginDate', 'endDate', 'electionType', 'hasMembershipAddition', 'employmentLevel']
            : ['surname', 'givenName', 'gender', 'language', 'function', 'beginDate', 'endDate', 'electionType', 'hasMembershipAddition']
    );

    constructor(
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly translateService: TranslateService,
        private readonly generalElectionCommitteesService: GeneralElectionCommitteesService,
        protected readonly generalElectionCommitteeDetailsService: GeneralElectionCommitteeDetailsService
    ) {
        merge(
            generalElectionCommitteeDetailsService.reload$,
            this.translateService.onLangChange.pipe(distinctUntilChanged((prev, curr) => prev.lang === curr.lang))
        )
            .pipe(
                switchMap(() => this.generalElectionCommitteesService.getGeneralElectionCommitteeMembers(this.route.snapshot.params.id)),
                takeUntilDestroyed()
            )
            .subscribe(membershipList => this.membershipList.set(membershipList));
    }

    sortData(sort: Sort, currentSort: WritableSignal<Sort>) {
        currentSort.set(sort);
    }

    openMembershipCandidate(membershipCandidateId: string) {
        void this.router.navigate(['general-election', 'committees', this.route.snapshot.params.id, 'membership-candidate', membershipCandidateId]);
    }

    private prepareData(members: CommitteeMember[], sort: Sort) {
        return members.sort((a, b) => this.compare(a, b, sort));
    }

    private compare(a: CommitteeMember, b: CommitteeMember, sort: Sort) {
        const key = sort.active as keyof CommitteeMember;
        return (a[key] < b[key] ? -1 : 1) * (sort.direction === 'asc' ? 1 : -1);
    }
}
