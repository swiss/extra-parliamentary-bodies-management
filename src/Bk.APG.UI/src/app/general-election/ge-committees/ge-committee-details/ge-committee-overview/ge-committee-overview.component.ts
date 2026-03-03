import {AsyncPipe} from '@angular/common';
import {ChangeDetectionStrategy, Component} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatCard, MatCardContent} from '@angular/material/card';
import {ActivatedRoute} from '@angular/router';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {catchError, combineLatest, EMPTY, distinctUntilChanged, map, startWith, switchMap} from 'rxjs';
import {CommitteeOverviewBasicDataComponent} from '../../../../committees/committee-details/committee-overview/committee-overview-basic-data/committee-overview-basic-data.component';
import {GeneralElectionCommitteeDetailsService} from '../ge-committee-details.service';

@Component({
    selector: 'apg-ge-committee-overview',
    imports: [TranslatePipe, CommitteeOverviewBasicDataComponent, MatCard, MatCardContent, AsyncPipe],
    templateUrl: './ge-committee-overview.component.html',
    styleUrl: './ge-committee-overview.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GeneralElectionCommitteeOverviewComponent {
    committeeDetails = this.geCommitteeDetailsService.committeeDetails;
    currentLanguage$ = this.translateService.onLangChange.pipe(
        startWith({lang: this.translateService.currentLang}),
        map(lang => lang.lang),
        distinctUntilChanged(),
        takeUntilDestroyed()
    );

    constructor(
        protected readonly geCommitteeDetailsService: GeneralElectionCommitteeDetailsService,
        private readonly route: ActivatedRoute,
        private readonly translateService: TranslateService
    ) {
        const loading$ = combineLatest([this.route.paramMap, this.currentLanguage$, this.geCommitteeDetailsService.reload$]);

        loading$
            .pipe(
                switchMap(() =>
                    this.geCommitteeDetailsService.generalElectionCommitteeDetails(this.route.snapshot.paramMap.get('id')!).pipe(catchError(() => EMPTY))
                ),
                takeUntilDestroyed()
            )
            .subscribe(committeeDetails => this.geCommitteeDetailsService.committeeDetails.set(committeeDetails));
    }
}
