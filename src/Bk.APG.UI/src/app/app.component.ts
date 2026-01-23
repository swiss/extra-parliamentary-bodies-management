import {Component, computed, OnInit, signal} from '@angular/core';
import {toSignal} from '@angular/core/rxjs-interop';
import {MatAnchor, MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatTooltip} from '@angular/material/tooltip';
import {TranslatePipe} from '@ngx-translate/core';
import {ObHttpApiInterceptorConfig, ObINavigationLink, ObMasterLayoutConfig, ObMasterLayoutModule} from '@oblique/oblique';
import {FroalaService} from '@shared/froala.service';
import {AuthService} from './auth/auth.service';
import {Role} from './auth/Role';
import {ConfigsService} from './configs.service';
import {GeneralElectionToggleComponent} from './general-election/ge-toggle/ge-toggle.component';
import {GeneralElectionService} from './general-election/general-election.service';
import {InformationService} from './information.service';

@Component({
    selector: 'apg-root',
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss',
    imports: [TranslatePipe, MatTooltip, MatIcon, ObMasterLayoutModule, MatAnchor, MatIconButton, GeneralElectionToggleComponent],
})
export class AppComponent implements OnInit {
    readonly myAccountHref: string;
    readonly isAuthenticated = signal<boolean | undefined>(undefined);
    readonly userName = signal<string>('');
    readonly currentYear = signal<number>(new Date().getFullYear());

    readonly roles = toSignal(this.authService.roles$, {initialValue: []});
    readonly navigation = computed<ObINavigationLink[]>(() => {
        const navigationLinks: ObINavigationLink[] = [
            {
                id: 'worklist',
                url: 'worklist',
                label: 'navigation.worklist',
                isExternal: false,
            },
            {
                id: 'persons',
                url: 'persons',
                label: 'navigation.persons',
                isExternal: false,
            },
            {
                id: 'committees',
                url: this.generalElectionService.isGeneralElectionEnabled() ? 'general-election/committees' : 'committees',
                label: 'navigation.committees',
                isExternal: false,
            },
            {
                id: 'exports',
                url: this.generalElectionService.isGeneralElectionEnabled() ? 'general-election/exports' : 'exports',
                label: 'navigation.exports',
                isExternal: false,
                children: [
                    {
                        id: 'data-analysis',
                        url: 'data-analysis',
                        label: 'navigation.exports.dataAnalysis',
                        isExternal: false,
                    },
                    {
                        id: 'requestsAndReports',
                        url: 'requestsAndReports',
                        label: 'navigation.exports.requestsAndReports',
                        isExternal: false,
                    },
                    {
                        id: 'formLetters',
                        url: 'formLetters',
                        label: 'navigation.exports.formLetters',
                        isExternal: false,
                    },
                    {
                        id: 'recipients',
                        url: 'recipients',
                        label: 'navigation.exports.recipients',
                        isExternal: false,
                    },
                ],
            },
        ];

        if (this.roles().includes(Role.Admin) || this.roles().includes(Role.Department)) {
            const children = [];

            if (this.roles().includes(Role.Admin)) {
                children.push({url: 'committeeTypes', label: 'navigation.administration.committeeTypes'});
            }

            if (this.roles().includes(Role.Admin) || this.roles().includes(Role.Department)) {
                children.push({url: 'generalMeasures', label: 'navigation.administration.generalMeasures'});
            }

            if (this.roles().includes(Role.Admin)) {
                children.push({url: 'onlinePublication', label: 'navigation.administration.onlinePublication'});
            }

            navigationLinks.push({
                id: 'administration',
                url: 'administration',
                label: 'navigation.administration',
                isExternal: false,
                children,
            });
        }

        return navigationLinks;
    });

    constructor(
        private readonly config: ObMasterLayoutConfig,
        private readonly infoService: InformationService,
        private readonly authService: AuthService,
        readonly configsService: ConfigsService,
        readonly httpApiInterceptorConfig: ObHttpApiInterceptorConfig,
        protected readonly generalElectionService: GeneralElectionService,
        private readonly froalaService: FroalaService
    ) {
        this.myAccountHref = configsService.frontendConfig.eiamMyAccountUrl;
        this.authService.isAuthenticated$.subscribe(value => this.isAuthenticated.set(value));
        this.authService.userName$.subscribe(value => this.userName.set(value));
        config.homePageRoute = '';
        config.locale.locales = ['de', 'fr', 'it'];
        httpApiInterceptorConfig.api.spinner = true;
        httpApiInterceptorConfig.api.url = '/api/';
        httpApiInterceptorConfig.timeout = 60000;
        config.header.isSmall = true;
        config.header.isSticky = true;
        config.footer.isSticky = true;
    }

    async ngOnInit(): Promise<void> {
        await this.froalaService.initializePlugins();
        this.initializeScrollToTop();
    }

    logout(): void {
        this.authService.logout();
    }

    applicationVersion(): string | undefined {
        return this.infoService.applicationVersion;
    }

    // Oblique scroll to top fix for tables and tabs
    private initializeScrollToTop(): void {
        const hasTabs = !!document?.querySelector('mat-tab-group');

        if (hasTabs) {
            this.setupTabScrolling();
        } else {
            this.addScrollTarget(document?.querySelector('main'));
        }

        this.setupScrollButton();
        this.observeTabChanges();
    }

    private addScrollTarget(container: Element | null): void {
        if (container && !container.querySelector('h1.scroll-target')) {
            const h1 = document.createElement('h1');
            h1.className = 'scroll-target';
            h1.style.cssText = 'position: absolute; top: 0; left: 0;';
            container.insertBefore(h1, container.firstChild);
        }
    }

    private setupTabScrolling(): void {
        document?.querySelectorAll('.mat-mdc-tab-body-content').forEach(content => {
            if (!content.hasAttribute('data-scroll-listener')) {
                content.setAttribute('data-scroll-listener', 'true');
                content.addEventListener('scroll', () => {
                    document.querySelector('.ob-master-layout')?.classList.toggle('ob-master-layout-scrolling', (content as HTMLElement).scrollTop > 0);
                });
                this.addScrollTarget(content);
            }
        });
    }

    private observeTabChanges(): void {
        new MutationObserver(() => this.setupTabScrolling()).observe(document.body, {childList: true, subtree: true});
    }

    private setupScrollButton(): void {
        const button = document.querySelector('.ob-top-control-btn');
        if (button && !button.hasAttribute('data-custom-handler')) {
            button.setAttribute('data-custom-handler', 'true');
            button.addEventListener(
                'click',
                e => {
                    e.stopPropagation();
                    const activeTab = document.querySelector('.mat-mdc-tab-body-active .mat-mdc-tab-body-content');
                    const scrollTarget = (activeTab || document.querySelector('main')) as HTMLElement;
                    scrollTarget?.scrollTo({top: 0, behavior: 'smooth'});
                },
                true
            );
        }
    }
}
