import {Component, DestroyRef, inject, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatButton} from '@angular/material/button';
import {MatButtonToggleGroup, MatButtonToggle, MatButtonToggleChange} from '@angular/material/button-toggle';
import {MatDialog} from '@angular/material/dialog';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {startWith, map, distinctUntilChanged, switchMap, filter} from 'rxjs';
import {OnlinePublicationService} from './online-publication.service';

@Component({
    selector: 'apg-online-publication',
    imports: [MatButton, MatButtonToggleGroup, MatButtonToggle, ObButtonDirective, TranslatePipe],
    templateUrl: './online-publication.component.html',
    styleUrl: './online-publication.component.scss',
})
export class OnlinePublicationComponent {
    protected readonly ogdPublicationEnabled = signal(false);

    private readonly onlinePublicationService = inject(OnlinePublicationService);
    private readonly translateService = inject(TranslateService);
    private readonly notificationService = inject(ObNotificationService);
    private readonly httpApiInterceptorEvents = inject(ObHttpApiInterceptorEvents);
    private readonly dialog = inject(MatDialog);
    private readonly dr = inject(DestroyRef);

    constructor() {
        this.translateService.onLangChange
            .pipe(
                startWith({lang: this.translateService.currentLang}),
                map(lang => lang.lang),
                distinctUntilChanged()
            )
            .pipe(
                switchMap(() => this.onlinePublicationService.getOgdExportSetting()),
                takeUntilDestroyed()
            )
            .subscribe(result => {
                this.ogdPublicationEnabled.set(result);
            });
    }

    onToggle($event: MatButtonToggleChange) {
        this.ogdPublicationEnabled.set($event.value);

        const messageText = this.ogdPublicationEnabled() ? 'onlinePublication.activated' : 'onlinePublication.deactivated';

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.onlinePublicationService.setOgdExportSetting($event.value).subscribe({
            next: async () => {
                return this.notificationService.success(messageText);
            },
            error: () => this.notificationService.error('onlinePublication.activation.error'),
        });
    }

    confirmTriggerPublication() {
        this.dialog
            .open(ConfirmDialogComponent, {
                width: '600px',
                data: {
                    title: this.translateService.instant('onlinePublication.trigger.confirmation.title'),
                    message: this.translateService.instant('onlinePublication.trigger.confirmation.message'),
                },
            })
            .afterClosed()
            .pipe(
                takeUntilDestroyed(this.dr),
                filter(result => result === true)
            )
            .subscribe(() => this.triggerPublication());
    }

    private triggerPublication() {
        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.notificationService.info({
            message: 'onlinePublication.trigger.message',
            timeout: 10000,
        });

        this.onlinePublicationService.triggerPublication().subscribe({
            next: () => {
                this.notificationService.success({
                    message: 'onlinePublication.trigger.success',
                });
            },
            error: () => {
                this.notificationService.error({
                    message: 'onlinePublication.trigger.error',
                });
            },
        });
    }
}
