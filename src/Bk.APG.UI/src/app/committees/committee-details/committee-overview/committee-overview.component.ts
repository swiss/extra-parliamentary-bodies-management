import {AsyncPipe, NgClass} from '@angular/common';
import {Component} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatCard, MatCardContent} from '@angular/material/card';
import {ActivatedRoute} from '@angular/router';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObAlertModule} from '@oblique/oblique';
import {combineLatest, distinctUntilChanged, map, merge, Observable, startWith, switchMap, tap} from 'rxjs';
import {ConfigsService} from '../../../../app/configs.service';
import {CommitteesService} from '../../committees.service';
import {CommitteeJustificationsOverviewComponent} from '../../shared/committee-justifications-overview/committee-justifications-overview.component';
import {CommitteeDetailsService} from '../committee-details.service';
import {CommitteeOverviewBasicDataComponent} from './committee-overview-basic-data/committee-overview-basic-data.component';
import {ContactPointsComponent} from './contact-points/contact-points.component';

@Component({
    selector: 'apg-committee-overview',
    templateUrl: './committee-overview.component.html',
    styleUrl: './committee-overview.component.scss',
    imports: [
        NgClass,
        MatCard,
        MatCardContent,
        ContactPointsComponent,
        CommitteeJustificationsOverviewComponent,
        CommitteeOverviewBasicDataComponent,
        AsyncPipe,
        TranslatePipe,
        ObAlertModule,
    ],
})
export class CommitteeOverviewComponent {
    committeeId!: string;
    currentLanguage$: Observable<string>;
    committeeDetails = this.committeeDetailsService.committeeDetails;

    constructor(
        private readonly route: ActivatedRoute,
        private readonly translateService: TranslateService,
        protected readonly configsService: ConfigsService,
        private readonly committeesService: CommitteesService,
        protected readonly committeeDetailsService: CommitteeDetailsService
    ) {
        this.currentLanguage$ = this.translateService.onLangChange.pipe(
            startWith({lang: this.translateService.getCurrentLang()}),
            map(lang => lang.lang),
            distinctUntilChanged(),
            takeUntilDestroyed()
        );

        const refresh$ = merge(this.route.paramMap.pipe(tap(paramMap => (this.committeeId = paramMap.get('id')!))), this.committeesService.reload$);
        const loading$ = combineLatest([refresh$, this.currentLanguage$]);

        loading$
            .pipe(switchMap(() => this.committeesService.getCommitteeDetails(this.committeeId)))
            .subscribe(committeeDetails => this.committeeDetailsService.committeeDetails.set(committeeDetails));
    }

    hasDataProtectionOfficer(): boolean {
        return (
            this.committeeDetailsService
                .committeeDetails()
                ?.contactPoints?.some(
                    y => (!y.endDate || y.endDate > new Date()) && y.contactPointTypeId === this.configsService.frontendConfig.entityIds.contactPoint.dpoId
                ) ?? false
        );
    }

    hasSecretariat(): boolean {
        return (
            this.committeeDetailsService
                .committeeDetails()
                ?.contactPoints?.some(
                    y =>
                        (!y.endDate || y.endDate > new Date()) &&
                        y.contactPointTypeId === this.configsService.frontendConfig.entityIds.contactPoint.secretariatId
                ) ?? false
        );
    }
}
