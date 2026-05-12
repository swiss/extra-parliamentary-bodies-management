import {AsyncPipe} from '@angular/common';
import {ChangeDetectionStrategy, Component} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatCard, MatCardContent} from '@angular/material/card';
import {ActivatedRoute} from '@angular/router';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {catchError, EMPTY, distinctUntilChanged, map, merge, shareReplay, skip, startWith, switchMap} from 'rxjs';
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
        startWith({lang: this.translateService.getCurrentLang()}),
        map(lang => lang.lang),
        distinctUntilChanged(),
        shareReplay({bufferSize: 1, refCount: true})
    );

    constructor(
        protected readonly geCommitteeDetailsService: GeneralElectionCommitteeDetailsService,
        private readonly route: ActivatedRoute,
        private readonly translateService: TranslateService
    ) {
        // Reload on language changes and explicit reloads (skip initial BehaviorSubject emit)
        merge(
            this.currentLanguage$.pipe(skip(1)), // Skip initial language to avoid double-load with guard
            this.geCommitteeDetailsService.reload$.pipe(skip(1)) // Skip initial BehaviorSubject emit
        )
            .pipe(
                switchMap(() =>
                    this.geCommitteeDetailsService.generalElectionCommitteeDetails(this.route.snapshot.paramMap.get('id')!).pipe(catchError(() => EMPTY))
                ),
                takeUntilDestroyed()
            )
            .subscribe(committeeDetails => this.geCommitteeDetailsService.committeeDetails.set(committeeDetails));
    }
}
