import {Component, computed, signal, viewChild, WritableSignal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute, Router} from '@angular/router';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {MembershipCreate} from '@api/MembershipCreate';
import {PersonDetails} from '@api/PersonDetails';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {MasterDataService} from '@shared/master-data.service';
import {CommitteesService} from '../../committees/committees.service';
import {PersonsService} from '../../persons/persons.service';
import {MembershipDataFormComponent} from '../shared/membership-data-form/membership-data-form.component';

@Component({
    selector: 'apg-membership-create',
    templateUrl: './membership-create.component.html',
    styleUrl: './membership-create.component.scss',
    imports: [MatButton, ObButtonDirective, MembershipDataFormComponent, TranslatePipe],
})
export class MembershipCreateComponent {
    membershipToCreate!: WritableSignal<MembershipCreate>;
    committeeEntity!: WritableSignal<CommitteeDetails>;
    personEntity!: WritableSignal<PersonDetails>;
    formComponent = viewChild.required(MembershipDataFormComponent);
    form = computed(() => this.formComponent().membershipForm);
    personSelected = computed(() => this.formComponent().personSelected);
    committeeSelected = computed(() => this.formComponent().committeeSelected);
    isForCommittee = false;
    isForPerson = false;

    constructor(
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly personsService: PersonsService,
        protected readonly masterDataService: MasterDataService,
        private readonly committeesService: CommitteesService,
        private readonly notificationService: ObNotificationService,
        protected readonly errorService: ErrorService,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {
        this.route.parent?.parent?.url.pipe(takeUntilDestroyed()).subscribe(([url]) => {
            if (url.path === 'committees') {
                this.isForCommittee = true;
                this.committeesService.getCommitteeDetails(this.route.snapshot.params.id).subscribe(x => (this.committeeEntity = signal(x)));
            }
            if (url.path === 'persons') {
                this.isForPerson = true;
                this.personsService.getPersonDetails(this.route.snapshot.params.id).subscribe(x => (this.personEntity = signal(x)));
            }
        });

        this.membershipToCreate = signal({} as MembershipCreate);
    }

    selectionMissing() {
        if (this.isForPerson) {
            return this.committeeSelected()() === undefined;
        }
        if (this.isForCommittee) {
            return this.personSelected()() === undefined;
        }
        return false;
    }

    save() {
        if (!this.form().valid || this.selectionMissing()) {
            this.form().markAsTouched();
            return;
        }

        const membershipToCreate = this.formComponent().buildMembershipModification();

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.committeesService.createMember(membershipToCreate).subscribe({
            next: async () => {
                this.form().reset();
                if (this.isForCommittee && this.committeeEntity()) {
                    await this.router.navigate(['committees', this.committeeEntity().id], {replaceUrl: true, queryParams: {tab: 'members'}});
                }
                if (this.isForPerson && this.personEntity()) {
                    await this.router.navigate(['persons', this.personEntity().id], {replaceUrl: true, queryParams: {tab: 'memberships'}});
                }
                return this.notificationService.success('memberships.save.success');
            },
            error: () => this.notificationService.error('memberships.save.error'),
        });
    }

    close() {
        if (this.isForCommittee) {
            void this.router.navigate(['committees', this.committeeEntity()?.id], {replaceUrl: true, queryParams: {tab: 'members'}});
        }
        if (this.isForPerson) {
            void this.router.navigate(['persons', this.personEntity()?.id], {replaceUrl: true, queryParams: {tab: 'memberships'}});
        }
    }

    getHeaderText(): string {
        if (this.isForCommittee && this.committeeEntity) {
            return this.committeeEntity()?.description;
        }
        if (this.isForPerson && this.personEntity) {
            return `${this.personEntity()?.givenName} ${this.personEntity()?.surname}`;
        }

        return '';
    }
}
