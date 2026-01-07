import {Component, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {MatButtonToggleGroup, MatButtonToggle, MatButtonToggleChange} from '@angular/material/button-toggle';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {startWith, map, distinctUntilChanged, switchMap} from 'rxjs';
import {OnlinePublicationService} from './online-publication.service';

@Component({
    selector: 'apg-online-publication',
    imports: [MatButtonToggleGroup, MatButtonToggle, TranslatePipe],
    templateUrl: './online-publication.component.html',
    styleUrl: './online-publication.component.scss',
})
export class OnlinePublicationComponent {
    ogdPublicationEnabled = signal(false);

    constructor(
        protected readonly onlinePublicationService: OnlinePublicationService,
        private readonly translateService: TranslateService,
        private readonly notificationService: ObNotificationService,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {
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
}
