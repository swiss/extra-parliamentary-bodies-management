import {Component, computed, DestroyRef, OnInit, signal, viewChild} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {PristineChangeEvent} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute, Router} from '@angular/router';
import {GeneralElectionCommitteeUpdate} from '@api/GeneralElectionCommitteeUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObNotificationService} from '@oblique/oblique';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {switchMap} from 'rxjs';
import {GeneralElectionCommitteeDataFormComponent} from '../ge-committee-data-form/ge-committee-data-form.component';
import {GeneralElectionCommitteeDetailsService} from '../ge-committee-details.service';
import {GeneralElectionCommitteeDataService} from './ge-committee-data.service';

@Component({
    selector: 'apg-ge-committee-data',
    imports: [MatButton, ObButtonDirective, TranslatePipe, GeneralElectionCommitteeDataFormComponent],
    templateUrl: './ge-committee-data.component.html',
    styleUrl: './ge-committee-data.component.scss',
    providers: [GeneralElectionCommitteeDataService],
})
export class GeneralElectionCommitteeDataComponent implements OnInit {
    committeeUpdate = signal<GeneralElectionCommitteeUpdate | undefined>(undefined);
    formComponent = viewChild.required(GeneralElectionCommitteeDataFormComponent);
    form = computed(() => this.formComponent().committeeForm);
    isExtraParliamentaryCommission = false;

    private unmodifiedCommittee!: GeneralElectionCommitteeUpdate;

    constructor(
        private readonly generalElectionCommitteeDataService: GeneralElectionCommitteeDataService,
        private readonly generalElectionCommitteeDetailsService: GeneralElectionCommitteeDetailsService,
        private readonly dr: DestroyRef,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly notificationService: ObNotificationService,
        private readonly entityAuditLogService: EntityAuditLogService
    ) {
        this.generalElectionCommitteeDetailsService.reload$
            .pipe(
                takeUntilDestroyed(),
                switchMap(() => this.generalElectionCommitteeDataService.getGeneralElectionCommitteeForUpdate(this.route.snapshot.params.id))
            )
            .subscribe(committeeUpdate => {
                this.committeeUpdate.set(committeeUpdate);
                this.form().reset(this.committeeUpdate());
                this.unmodifiedCommittee = committeeUpdate;
            });
    }

    ngOnInit() {
        this.form()
            .events.pipe(takeUntilDestroyed(this.dr))
            .subscribe(event => {
                if (event instanceof PristineChangeEvent) {
                    this.generalElectionCommitteeDetailsService.isDataFormDirty.set(!event.pristine);
                }
            });
    }

    handleExtraParliamentaryCommission(value: boolean) {
        this.isExtraParliamentaryCommission = value;
    }

    reset() {
        this.form().reset(this.unmodifiedCommittee);
    }

    save() {
        if (!this.form().valid) {
            this.form().markAsTouched();
            return;
        }

        this.generalElectionCommitteeDataService.updateGeneralElectionCommittee(this.committeeUpdate()!).subscribe({
            next: async () => {
                this.unmodifiedCommittee = this.committeeUpdate()!;
                this.generalElectionCommitteeDetailsService.reload$.next();
                this.entityAuditLogService.reload$.next();
                await this.router.navigate([]);
                return this.notificationService.success('generalElection.committee.details.data.success');
            },
            error: () => this.notificationService.error('generalElection.committee.details.data.error'),
        });
    }
}
