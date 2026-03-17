import {Component, DestroyRef, inject, OnDestroy, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatButton} from '@angular/material/button';
import {MatButtonToggleGroup, MatButtonToggle, MatButtonToggleChange} from '@angular/material/button-toggle';
import {MatDialog} from '@angular/material/dialog';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService, ObSpinnerComponent, ObSpinnerService} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {startWith, map, distinctUntilChanged, switchMap, filter, finalize} from 'rxjs';
import {OnlinePublicationService} from './online-publication.service';

@Component({
    selector: 'apg-online-publication',
    imports: [MatButton, MatButtonToggleGroup, MatButtonToggle, ObButtonDirective, ObSpinnerComponent, TranslatePipe],
    templateUrl: './online-publication.component.html',
    styleUrl: './online-publication.component.scss',
})
export class OnlinePublicationComponent implements OnDestroy {
    protected readonly ogdPublicationEnabled = signal(false);

    protected readonly spinnerChannel = 'triggerChannel';

    private readonly onlinePublicationService = inject(OnlinePublicationService);
    private readonly translateService = inject(TranslateService);
    private readonly notificationService = inject(ObNotificationService);
    private readonly httpApiInterceptorEvents = inject(ObHttpApiInterceptorEvents);
    private readonly dialog = inject(MatDialog);
    private readonly dr = inject(DestroyRef);
    private readonly spinnerService = inject(ObSpinnerService);

    constructor() {
        this.translateService.onLangChange
            .pipe(
                startWith({lang: this.translateService.getCurrentLang()}),
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
    ngOnDestroy(): void {
        this.spinnerService.forceDeactivate(this.spinnerChannel);
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
        this.httpApiInterceptorEvents.deactivateSpinnerOnNextAPICalls(1);
        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls(1);

        this.spinnerService.activate(this.spinnerChannel);

        this.notificationService.info({
            message: 'onlinePublication.trigger.message',
            timeout: 10000,
        });

        this.onlinePublicationService
            .triggerPublication()
            .pipe(finalize(() => this.spinnerService.deactivate(this.spinnerChannel)))
            .subscribe({
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
