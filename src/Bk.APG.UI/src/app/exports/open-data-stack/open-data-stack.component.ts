import {AfterViewInit, Component, ElementRef, ViewChild, inject} from '@angular/core';
import {TranslatePipe} from '@ngx-translate/core';
import {take} from 'rxjs';
import {ConfigsService} from '../../configs.service';
import {OpenDataStackService} from './open-data-stack.service';

@Component({
    selector: 'apg-open-data-stack',
    templateUrl: './open-data-stack.component.html',
    styleUrl: './open-data-stack.component.scss',
    imports: [TranslatePipe],
})
export class OpenDataStackComponent implements AfterViewInit {
    private readonly configsService = inject(ConfigsService);
    private readonly openDataStackService = inject(OpenDataStackService);

    private readonly dashboardUrl: string;
    private readonly odsOrigin: string;

    @ViewChild('odsEmbed')
    private readonly iframeRef?: ElementRef<HTMLIFrameElement>;

    constructor() {
        this.dashboardUrl = `${this.configsService.frontendConfig.openDataStack.baseUrl}/embedded-analytics/bootstrap/?redirect=${encodeURIComponent(
            this.configsService.frontendConfig.openDataStack.dashboard
        )}`;
        this.odsOrigin = new URL(this.configsService.frontendConfig.openDataStack.baseUrl).origin;
    }

    public ngAfterViewInit(): void {
        this.openDataStackService
            .exchangeToken()
            .pipe(take(1))
            .subscribe(code => {
                const iframe = this.iframeRef?.nativeElement;
                if (!iframe) {
                    return;
                }

                iframe.src = this.dashboardUrl;

                iframe.onload = () => {
                    iframe.contentWindow?.postMessage({type: 'embedded-auth', code}, this.odsOrigin);
                    iframe.onload = null;
                };
            });
    }
}
