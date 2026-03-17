import {DatePipe, NgClass} from '@angular/common';
import {Component, computed, signal, WritableSignal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
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
import {PersonMembership} from '@api/PersonMembership';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObAlertModule, ObButtonDirective} from '@oblique/oblique';
import {distinctUntilChanged, startWith, switchMap} from 'rxjs';
import {PersonsService} from '../../persons.service';
import {PersonDetailsService} from '../person-details.service';

@Component({
    selector: 'apg-person-memberships',
    templateUrl: './person-memberships.component.html',
    styleUrl: './person-memberships.component.scss',
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
        NgClass,
        MatNoDataRow,
        DatePipe,
        TranslatePipe,
        ObAlertModule,
        MatTooltip,
    ],
})
export class PersonMembershipsComponent {
    readonly displayedColumns: (keyof PersonMembership)[] = ['committee', 'department', 'function', 'beginDate', 'endDate', 'electionType'];
    memberships = signal<PersonMembership[]>([]);
    currentActiveMembershipsSort = signal<Sort>({active: 'beginDate', direction: 'desc'});
    currentInactiveMembershipsSort = signal<Sort>({active: 'beginDate', direction: 'desc'});
    activeMemberships = computed(() =>
        this.memberships()
            .sort((a, b) => this.compare(a, b, this.currentActiveMembershipsSort()))
            .filter(membership => !!membership.isActive || !!membership.isFuture)
    );
    inactiveMemberships = computed(() =>
        this.memberships()
            .sort((a, b) => this.compare(a, b, this.currentInactiveMembershipsSort()))
            .filter(membership => !membership.isActive && !membership.isFuture)
    );

    tables = [
        {
            name: 'activeMemberships',
            data: this.activeMemberships,
            currentSort: this.currentActiveMembershipsSort,
        },
        {
            name: 'inactiveMemberships',
            data: this.inactiveMemberships,
            currentSort: this.currentInactiveMembershipsSort,
        },
    ];

    constructor(
        protected readonly personDetailsService: PersonDetailsService,
        private readonly personsService: PersonsService,
        private readonly route: ActivatedRoute,
        private readonly translateService: TranslateService,
        private readonly router: Router
    ) {
        this.translateService.onLangChange
            .pipe(
                startWith({lang: this.translateService.getCurrentLang()}),
                distinctUntilChanged((prev, curr) => prev.lang === curr.lang),
                switchMap(() => this.personsService.getPersonMemberships(this.route.snapshot.params.id)),
                takeUntilDestroyed()
            )
            .subscribe(memberships => this.memberships.set(memberships));
    }

    sortData(sort: Sort, currentSort: WritableSignal<Sort>) {
        currentSort.set(sort);
    }

    editMembership(personMembership: PersonMembership) {
        void this.router.navigate(['memberships', personMembership.id], {relativeTo: this.route});
    }

    createMembership() {
        void this.router.navigate(['memberships', 'create'], {relativeTo: this.route});
    }

    private compare(a: PersonMembership, b: PersonMembership, sort: Sort) {
        const key = sort.active as keyof PersonMembership;
        return (a[key] < b[key] ? -1 : 1) * (sort.direction === 'asc' ? 1 : -1);
    }
}
