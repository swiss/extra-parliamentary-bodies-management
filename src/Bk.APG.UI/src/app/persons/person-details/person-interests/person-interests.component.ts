import {Component, computed, DestroyRef, OnInit, viewChild} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {FormGroup, PristineChangeEvent, ReactiveFormsModule, UntypedFormArray} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute, Router} from '@angular/router';
import {InterestUpdate} from '@api/InterestUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObAlertModule, ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService, ObUnsavedChangesDirective} from '@oblique/oblique';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {PersonsService} from '../../persons.service';
import {PersonDetailsService} from '../person-details.service';
import {InterestsEditFormComponent} from './interests-edit-form/interests-edit-form.component';
import {PersonInterestsService} from './person-interests.service';

@Component({
    selector: 'apg-person-interests',
    templateUrl: './person-interests.component.html',
    styleUrl: './person-interests.component.scss',
    imports: [
        ReactiveFormsModule,
        ObUnsavedChangesDirective,
        MatButton,
        ObButtonDirective,
        InterestsEditFormComponent,
        TranslatePipe,
        ObAlertModule,
        HelpTooltipComponent,
    ],
})
export class PersonInterestsComponent implements OnInit {
    public personId!: string;
    public reload = false;

    form = new FormGroup({interests: new UntypedFormArray([])});

    interestsEditFormComponent = viewChild.required(InterestsEditFormComponent);

    noInterest = computed(() => this.personDetailsService.personDetails().noInterest);

    constructor(
        private readonly personInterestsService: PersonInterestsService,
        private readonly personsService: PersonsService,
        private readonly notificationService: ObNotificationService,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        protected readonly personDetailsService: PersonDetailsService,
        private readonly dr: DestroyRef,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents,
        private readonly entityAuditLogService: EntityAuditLogService
    ) {
        this.personId = this.route.snapshot.paramMap.get('id')!;
        this.route.paramMap.pipe(takeUntilDestroyed()).subscribe(paramMap => (this.personId = paramMap.get('id')!));
    }

    ngOnInit() {
        this.form.controls.interests.events.pipe(takeUntilDestroyed(this.dr)).subscribe(event => {
            if (event instanceof PristineChangeEvent) {
                this.personDetailsService.isInterestsFormDirty.set(!event.pristine);

                if (event.pristine) {
                    this.reload = true;
                }
            }
        });
    }

    saveInterests() {
        this.reload = false;
        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();
        const interests = this.form.controls.interests.getRawValue() as InterestUpdate[];
        this.personInterestsService.saveInterestForPerson(this.personId, interests).subscribe({
            next: async () => {
                this.interestsEditFormComponent().interests = interests;
                this.personsService.reload$.next();
                this.entityAuditLogService.reload$.next();
                this.form.markAsPristine();
                this.form.markAsUntouched();
                await this.router.navigate([]);
                return this.notificationService.success('interests.save.success');
            },
            error: () => this.notificationService.error('interests.save.error'),
        });
    }

    reset() {
        this.interestsEditFormComponent().reset();
        this.form.markAsPristine();
        this.form.markAsUntouched();
    }
}
