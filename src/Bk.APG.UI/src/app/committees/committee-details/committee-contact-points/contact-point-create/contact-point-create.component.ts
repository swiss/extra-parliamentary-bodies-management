import {AfterViewInit, Component, computed, OnDestroy, OnInit, signal, viewChild, WritableSignal} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute, Router} from '@angular/router';
import {ContactPointCreate} from '@api/ContactPointCreate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObNotificationService} from '@oblique/oblique';
import {Subject, takeUntil} from 'rxjs';
import {CommitteesService} from '../../../../committees/committees.service';
import {CommitteeDetailsService} from '../../committee-details.service';
import {ContactPointFormComponent} from '../contact-point-form/contact-point-form.component';
import {ContactPointsService} from '../contact-points.service';

@Component({
    selector: 'apg-contact-point-create',
    templateUrl: './contact-point-create.component.html',
    styleUrl: './contact-point-create.component.scss',
    providers: [CommitteeDetailsService],
    imports: [MatButton, ObButtonDirective, ContactPointFormComponent, TranslatePipe],
})
export class ContactPointCreateComponent implements OnInit, AfterViewInit, OnDestroy {
    contactPointToCreate!: WritableSignal<ContactPointCreate>;
    formComponent = viewChild.required(ContactPointFormComponent);
    form = computed(() => this.formComponent().contactPointForm);

    committeeId = '';
    committeeName = '';
    private readonly destroy$ = new Subject<void>();

    constructor(
        protected readonly committeeDetailsService: CommitteeDetailsService,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly contactPointsService: ContactPointsService,
        private readonly notificationService: ObNotificationService,
        private readonly committeesService: CommitteesService
    ) {
        this.committeeId = this.route.snapshot.params.id;
        this.contactPointsService
            .getContactPointForCreate(this.committeeId)
            .subscribe(contactPointToCreate => (this.contactPointToCreate = signal(contactPointToCreate)));

        this.committeesService.getCommitteeDetails(this.committeeId).subscribe(committee => (this.committeeName = committee.description));
    }

    ngOnInit() {
        const form = this.form();
        const company = form.controls.companyName;

        company.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(() => {
            if (company.dirty) {
                form.markAllAsTouched();
                form.updateValueAndValidity({emitEvent: false});
            }
        });
    }

    ngAfterViewInit() {
        const form = this.form();
        form.controls.companyName.markAllAsTouched();
        form.controls.companyName.updateValueAndValidity();
    }

    ngOnDestroy() {
        this.destroy$.next();
        this.destroy$.complete();
    }

    save() {
        if (!this.form().valid) {
            this.form().markAllAsTouched();
            return;
        }

        this.contactPointToCreate().committeeId = this.committeeId;

        this.contactPointsService.createContactPoint(this.contactPointToCreate()).subscribe({
            next: async () => {
                this.form().reset(this.contactPointToCreate());
                this.contactPointsService.reload$.next();
                await this.router.navigate(['committees', this.committeeId], {replaceUrl: true, queryParams: {tab: 'contacts'}});
                return this.notificationService.success('contactPoint.details.data.success');
            },
            error: () => this.notificationService.error('contactPoint.details.data.error'),
        });
    }

    close() {
        void this.router.navigate(['committees', this.committeeId], {replaceUrl: true, queryParams: {tab: 'contacts'}});
    }
}
