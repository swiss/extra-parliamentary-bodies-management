import {Component, computed, signal, viewChild, WritableSignal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatButton} from '@angular/material/button';
import {MatDialog} from '@angular/material/dialog';
import {ActivatedRoute, Router} from '@angular/router';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {MembershipUpdate} from '@api/MembershipUpdate';
import {PersonDetails} from '@api/PersonDetails';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObAlertModule, ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {MasterDataService} from '@shared/master-data.service';
import {filter, Subject, switchMap} from 'rxjs';
import {CommitteesService} from '../../committees/committees.service';
import {PersonsService} from '../../persons/persons.service';
import {MembershipsService} from '../memberships.service';
import {MembershipDataFormComponent} from '../shared/membership-data-form/membership-data-form.component';

@Component({
    selector: 'apg-membership-edit',
    templateUrl: './membership-edit.component.html',
    styleUrl: './membership-edit.component.scss',
    imports: [MatButton, ObButtonDirective, MembershipDataFormComponent, TranslatePipe, ObAlertModule],
})
export class MembershipEditComponent {
    membershipToUpdate!: WritableSignal<MembershipUpdate>;
    committeeEntity!: WritableSignal<CommitteeDetails>;
    personEntity!: WritableSignal<PersonDetails>;
    formComponent = viewChild.required(MembershipDataFormComponent);
    form = computed(() => this.formComponent().membershipForm);
    personSelected = computed(() => this.formComponent().personSelected);
    committeeSelected = computed(() => this.formComponent().committeeSelected);

    isForCommittee = false;
    isForPerson = false;
    canDelete = false;

    protected hasValidationErrors = computed(() => this.formComponent().validationResults().hasErrors);

    private unmodifiedMembership!: MembershipUpdate;
    private readonly deleteClick$ = new Subject<void>();

    constructor(
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly personsService: PersonsService,
        private readonly membershipsService: MembershipsService,
        protected readonly masterDataService: MasterDataService,
        private readonly committeesService: CommitteesService,
        private readonly notificationService: ObNotificationService,
        private readonly translateService: TranslateService,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents,
        private readonly dialog: MatDialog
    ) {
        this.membershipsService.getMembershipForUpdate(this.route.snapshot.params.id).subscribe(membership => {
            this.membershipToUpdate = signal(membership);
            this.canDelete = this.membershipToUpdate().canDelete;
            this.unmodifiedMembership = membership;
        });
        this.route.parent?.parent?.url.pipe(takeUntilDestroyed()).subscribe(([url]) => {
            if (url.path === 'committees') {
                this.isForCommittee = true;
                this.committeesService.getCommitteeDetails(this.route.parent?.snapshot.params.id).subscribe(x => (this.committeeEntity = signal(x)));
            }
            if (url.path === 'persons') {
                this.isForPerson = true;
                this.personsService.getPersonDetails(this.route.parent?.snapshot.params.id).subscribe(x => (this.personEntity = signal(x)));
            }
        });

        this.deleteClick$
            .pipe(
                takeUntilDestroyed(),
                switchMap(() =>
                    this.dialog
                        .open(ConfirmDialogComponent, {
                            width: '600px',
                            data: {
                                title: this.translateService.instant('membership.delete.title'),
                                message: this.translateService.instant('membership.delete.text'),
                            },
                        })
                        .afterClosed()
                ),
                filter(result => result === true),
                switchMap(() => this.membershipsService.deleteMembership(this.membershipToUpdate().id))
            )
            .subscribe({
                next: async () => {
                    this.membershipsService.reload$.next();
                    this.back();
                    return this.notificationService.success('membership.delete.success');
                },
                error: () => this.notificationService.error('membership.delete.error'),
            });
    }

    get needsAttentionInterests() {
        if (this.isForPerson) {
            return this.personEntity?.().needsAttentionInterests;
        }
        if (this.isForCommittee && this.formComponent().person()) {
            return this.formComponent().person()!.needsAttentionInterests;
        }
        return false;
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

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        const membershipToUpdate = this.formComponent().buildMembershipModification() as MembershipUpdate;

        this.membershipsService.updateMembership(membershipToUpdate).subscribe({
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

    reset() {
        this.form().reset();
        this.formComponent().resetForm(this.unmodifiedMembership);
    }

    back() {
        if (this.isForCommittee) {
            void this.router.navigate(['committees', this.committeeEntity()?.id], {replaceUrl: true, queryParams: {tab: 'members'}});
        }
        if (this.isForPerson) {
            void this.router.navigate(['persons', this.personEntity()?.id], {replaceUrl: true, queryParams: {tab: 'memberships'}});
        }
    }

    delete() {
        this.deleteClick$.next();
    }

    getHeaderText(): string {
        if (this.isForCommittee && this.committeeEntity) {
            return this.committeeEntity()?.description;
        }
        if (this.isForPerson && this.personEntity) {
            return `${this.personEntity()?.surname} ${this.personEntity()?.givenName}`;
        }

        return '';
    }
}
