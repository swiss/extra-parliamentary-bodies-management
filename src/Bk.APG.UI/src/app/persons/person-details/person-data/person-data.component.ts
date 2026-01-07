import {Component, computed, DestroyRef, OnInit, signal, viewChild, WritableSignal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {PristineChangeEvent} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatDialog} from '@angular/material/dialog';
import {ActivatedRoute, Router} from '@angular/router';
import {PersonUpdate} from '@api/PersonUpdate';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObAlertModule, ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {combineLatest, distinctUntilChanged, filter, map, merge, of, startWith, Subject, switchMap, tap} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';
import {PersonsService} from '../../persons.service';
import {PersonDataFormComponent} from '../../shared/person-data-form/person-data-form.component';
import {PersonDetailsService} from '../person-details.service';

@Component({
    selector: 'apg-person-data',
    templateUrl: './person-data.component.html',
    styleUrl: './person-data.component.scss',
    imports: [MatButton, ObButtonDirective, PersonDataFormComponent, TranslatePipe, ObAlertModule],
})
export class PersonDataComponent implements OnInit {
    personUpdate: WritableSignal<PersonUpdate | undefined> = signal(undefined);
    formComponent = viewChild.required(PersonDataFormComponent);
    form = computed(() => this.formComponent().personForm);
    isObserver = false;
    isAdmin = false;
    personId!: string;
    get person() {
        return this.personUpdate();
    }

    private readonly refresh = new Subject<void>();
    private unmodifiedPerson!: PersonUpdate;

    constructor(
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly personsService: PersonsService,
        private readonly notificationService: ObNotificationService,
        private readonly personDetailsService: PersonDetailsService,
        private readonly translateService: TranslateService,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents,
        private readonly dr: DestroyRef,
        private readonly authService: AuthService,
        private readonly entityAuditLogService: EntityAuditLogService,
        private readonly dialog: MatDialog
    ) {
        this.authService.isObserver$.pipe(takeUntilDestroyed()).subscribe(isObserver => (this.isObserver = isObserver));
        this.authService.isAdmin$.pipe(takeUntilDestroyed()).subscribe(isAdmin => (this.isAdmin = isAdmin));

        if (!this.isObserver) {
            const currentLanguage$ = this.translateService.onLangChange.pipe(
                startWith({lang: this.translateService.currentLang}),
                map(lang => lang.lang),
                distinctUntilChanged(),
                takeUntilDestroyed()
            );

            const refresh$ = merge(
                this.refresh.pipe(switchMap(() => of(this.route.snapshot.paramMap.get('id')))),
                this.route.paramMap.pipe(tap(paramMap => (this.personId = paramMap.get('id')!))),
                this.personsService.reload$
            );
            const loading$ = combineLatest([refresh$, currentLanguage$]);

            loading$
                .pipe(
                    switchMap(() => this.personsService.getPersonForUpdate(this.personId)),
                    takeUntilDestroyed()
                )
                .subscribe(personUpdate => {
                    this.personUpdate = signal(personUpdate);
                    this.unmodifiedPerson = personUpdate;
                });
        }
    }

    ngOnInit() {
        this.form()
            .events.pipe(takeUntilDestroyed(this.dr))
            .subscribe(event => {
                if (event instanceof PristineChangeEvent) {
                    this.personDetailsService.isDataFormDirty.set(!event.pristine);
                }
            });
    }

    save() {
        this.formComponent().validateAddresses();

        if (!this.form().valid) {
            this.form().markAsTouched();
            return;
        }

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.personsService.updatePerson(this.person!).subscribe({
            next: async person => {
                this.personUpdate.set(person);
                this.form().reset(this.personUpdate(), {emitEvent: false});
                this.unmodifiedPerson = this.person!;
                this.personsService.reload$.next();
                this.entityAuditLogService.reload$.next();
                await this.router.navigate([]);
                return this.notificationService.success('person.details.data.success');
            },
            error: () => this.notificationService.error('person.details.data.error'),
        });
    }

    reset() {
        this.form().reset(this.unmodifiedPerson, {emitEvent: false});
        this.formComponent().personModification.set(this.unmodifiedPerson);
    }

    deletePerson() {
        this.dialog
            .open(ConfirmDialogComponent, {
                width: '600px',
                data: {
                    title: this.translateService.instant('person.delete.title'),
                    message: this.getDeleteDialogMessage(),
                },
            })
            .afterClosed()
            .pipe(
                takeUntilDestroyed(this.dr),
                filter(result => result === true),
                switchMap(() => this.personsService.deletePerson(this.personUpdate()!.id))
            )
            .subscribe({
                next: async () => {
                    this.personsService.reload$.next();
                    await this.router.navigate(['persons']);
                    return this.notificationService.success('person.delete.success');
                },
                error: () => this.notificationService.error('person.delete.error'),
            });
    }

    private getDeleteDialogMessage(): string {
        const deleteMessage = this.translateService.instant('person.delete.text');
        if (this.isAdmin) {
            const membershipWarning = this.translateService.instant('person.delete.membershipWarning');
            return `${deleteMessage} ${membershipWarning}`;
        }
        return deleteMessage;
    }
}
