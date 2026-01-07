import {Component, computed, DestroyRef, OnInit, signal, viewChild, WritableSignal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {PristineChangeEvent} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute, Router} from '@angular/router';
import {ContactPointUpdate} from '@api/ContactPointUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObHttpApiInterceptorEvents, ObNotificationService, ObButtonDirective} from '@oblique/oblique';
import {CommitteesService} from '../../../../committees/committees.service';
import {CommitteeDetailsService} from '../../committee-details.service';
import {ContactPointFormComponent} from '../contact-point-form/contact-point-form.component';
import {ContactPointsService} from '../contact-points.service';

@Component({
    selector: 'apg-contact-point-edit',
    templateUrl: './contact-point-edit.component.html',
    styleUrl: './contact-point-edit.component.scss',
    providers: [CommitteeDetailsService],
    imports: [MatButton, ObButtonDirective, ContactPointFormComponent, TranslatePipe],
})
export class ContactPointEditComponent implements OnInit {
    contactPointToUpdate!: WritableSignal<ContactPointUpdate>;
    formComponent = viewChild.required(ContactPointFormComponent);
    form = computed(() => this.formComponent().contactPointForm);

    isUpdateMode = true;
    isCopyMode = false;
    committeeId = '';
    committeeName = '';

    unmodifiedContactPoint!: ContactPointUpdate;

    constructor(
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly contactPointsService: ContactPointsService,
        private readonly committeeDetailsService: CommitteeDetailsService,
        private readonly notificationService: ObNotificationService,
        private readonly committeesService: CommitteesService,
        private readonly dr: DestroyRef,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {
        if (this.route.snapshot.url[1]?.path === 'copy') {
            this.isCopyMode = true;
            this.isUpdateMode = false;
        }

        this.contactPointsService.getContactPointForUpdate(this.route.snapshot.params.id).subscribe(contactPointToUpdate => {
            this.contactPointToUpdate = signal(contactPointToUpdate);
            this.committeeId = this.contactPointToUpdate().committeeId;
            this.unmodifiedContactPoint = contactPointToUpdate;
            if (this.isCopyMode) {
                this.contactPointToUpdate().isCopy = true;
            }

            this.committeesService.getCommitteeDetails(this.committeeId).subscribe(committee => (this.committeeName = committee.description));
        });
    }

    ngOnInit() {
        this.form()
            .events.pipe(takeUntilDestroyed(this.dr))
            .subscribe(event => {
                if (event instanceof PristineChangeEvent) {
                    this.committeeDetailsService.isContactsFormDirty.set(!event.pristine);
                }
            });
    }

    save() {
        if (!this.form().valid) {
            this.form().markAsTouched();
            return;
        }

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();
        this.contactPointsService.updateContactPoint(this.contactPointToUpdate()).subscribe({
            next: async () => {
                this.form().reset(this.contactPointToUpdate(), {emitEvent: false});
                this.unmodifiedContactPoint = this.contactPointToUpdate();
                this.contactPointsService.reload$.next();

                await this.router.navigate(['committees', this.contactPointToUpdate().committeeId], {replaceUrl: true, queryParams: {tab: 'contacts'}});

                return this.notificationService.success('contactPoint.details.data.success');
            },
            error: () => this.notificationService.error('contactPoint.details.data.error'),
        });
    }

    reset() {
        this.form().reset(this.unmodifiedContactPoint, {emitEvent: false});
    }

    async back() {
        await this.router.navigate(['committees', this.contactPointToUpdate().committeeId], {replaceUrl: true, queryParams: {tab: 'contacts'}});
    }
}
