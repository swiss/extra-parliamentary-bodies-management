import {Component, OnDestroy} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {ActivatedRoute} from '@angular/router';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObAlertComponent} from '@oblique/oblique';
import {combineLatest, distinctUntilChanged, map, merge, of, startWith, Subject, switchMap} from 'rxjs';
import {PersonsService} from '../../persons.service';
import {PersonOverviewBasicDataComponent} from '../../shared/person-overview-basic-data/person-overview-basic-data.component';
import {PersonDetailsService} from '../person-details.service';
import {PersonOverviewAddressComponent} from './person-overview-address/person-overview-address.component';
import {PersonOverviewInterestsComponent} from './person-overview-interests/person-overview-interests.component';
import {PersonOverviewMembershipsComponent} from './person-overview-memberships/person-overview-memberships.component';

@Component({
    selector: 'apg-person-details-overview',
    templateUrl: './person-overview.component.html',
    styleUrl: './person-overview.component.scss',
    imports: [
        PersonOverviewBasicDataComponent,
        PersonOverviewAddressComponent,
        PersonOverviewMembershipsComponent,
        PersonOverviewInterestsComponent,
        ObAlertComponent,
        TranslatePipe,
    ],
})
export class PersonOverviewComponent implements OnDestroy {
    personId!: string;

    private readonly destroyed$ = new Subject<void>();
    private readonly refresh = new Subject<void>();

    constructor(
        private readonly route: ActivatedRoute,
        private readonly translateService: TranslateService,
        private readonly personsService: PersonsService,
        protected readonly personDetailsService: PersonDetailsService
    ) {
        const currentLanguage$ = this.translateService.onLangChange.pipe(
            startWith({lang: this.translateService.currentLang}),
            map(lang => lang.lang),
            distinctUntilChanged()
        );

        const refresh$ = merge(
            this.refresh.pipe(switchMap(() => of(this.route.snapshot.paramMap.get('id')))),
            this.route.paramMap.pipe(
                map(params => {
                    this.personId = params.get('id')!;
                })
            ),
            this.personsService.reload$
        );
        const loading$ = combineLatest([refresh$, currentLanguage$]);

        loading$
            .pipe(
                switchMap(() => this.personsService.getPersonDetails(this.personId)),
                takeUntilDestroyed()
            )
            .subscribe(personDetails => this.personDetailsService.personDetails.set(personDetails));
    }

    ngOnDestroy(): void {
        this.destroyed$.next();
        this.destroyed$.complete();
    }
}
