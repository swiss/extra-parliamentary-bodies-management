import {registerLocaleData} from '@angular/common';
import {HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import localeDECH from '@angular/common/locales/de-CH';
import localeFRCH from '@angular/common/locales/fr-CH';
import localeITCH from '@angular/common/locales/it-CH';
import {inject, LOCALE_ID, provideAppInitializer} from '@angular/core';
import {DateAdapter, MAT_DATE_FORMATS} from '@angular/material/core';
import {MatPaginatorIntl} from '@angular/material/paginator';
import {bootstrapApplication} from '@angular/platform-browser';
import {provideAnimations} from '@angular/platform-browser/animations';
import {provideRouter} from '@angular/router';
import {OB_BANNER, ObHttpApiInterceptor, provideObliqueConfiguration} from '@oblique/oblique';
import {APG_DATE_FORMATS, ApgDateAdapter} from '@shared/DateAdapter';
import {DOCUMENT, LOCAL_STORAGE, SESSION_STORAGE, WINDOW} from '@shared/injection.tokens';
import {ObPaginatorFixService} from '@shared/ob-paginator-fix.service';
import {AuthInterceptor, provideAuth, StsConfigLoader} from 'angular-auth-oidc-client';
import {firstValueFrom, of} from 'rxjs';
import {AppComponent} from './app/app.component';
import {routes} from './app/app.routes';
import {ConfigsService} from './app/configs.service';
import {InformationService} from './app/information.service';
import {DateOnlyInterceptor} from './app/interceptor/date-only.interceptor';
import {ErrorInterceptor} from './app/interceptor/error.interceptor';
import {LanguageHeaderInterceptor} from './app/interceptor/language-header.interceptor';

registerLocaleData(localeDECH);
registerLocaleData(localeFRCH);
registerLocaleData(localeITCH);

bootstrapApplication(AppComponent, {
    providers: [
        provideObliqueConfiguration({
            accessibilityStatement: {
                createdOn: new Date('2025-11-20'),
                conformity: 'none',
                applicationName: 'APG',
                applicationOperator: 'BK DevOps',
                contact: [{email: 'BK_DevOPS@bit.admin.ch'}],
            },
            translate: {
                additionalFiles: [
                    {prefix: './assets/i18n/oblique-', suffix: `.json?v=${new Date().toISOString()}`},
                    {prefix: './assets/i18n/', suffix: `.json?v=${new Date().toISOString()}`},
                ],
            },
        }),
        provideAppInitializer(() => {
            const configsService = inject(ConfigsService);
            return firstValueFrom(configsService.loadFrontendConfig());
        }),
        provideAppInitializer(() => {
            const infoService = inject(InformationService);
            infoService.loadApplicationVersion();
        }),
        provideAuth({
            loader: {
                provide: StsConfigLoader,
                useFactory: (configsService: ConfigsService): StsConfigLoader => {
                    return {
                        loadConfigs: () => of([configsService.openIdConfiguration]),
                    };
                },
                deps: [ConfigsService],
            },
        }),
        {provide: LOCALE_ID, useValue: 'de-CH'},
        {provide: OB_BANNER, useFactory: (configsService: ConfigsService) => configsService.bannerInfo, deps: [ConfigsService]},
        {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
        {provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true},
        {provide: HTTP_INTERCEPTORS, useClass: ObHttpApiInterceptor, multi: true},
        {provide: HTTP_INTERCEPTORS, useClass: LanguageHeaderInterceptor, multi: true},
        {provide: HTTP_INTERCEPTORS, useClass: DateOnlyInterceptor, multi: true},
        {provide: DateAdapter, useClass: ApgDateAdapter},
        {provide: MAT_DATE_FORMATS, useValue: APG_DATE_FORMATS},
        {provide: LOCAL_STORAGE, useValue: window.localStorage},
        {provide: SESSION_STORAGE, useValue: window.sessionStorage},
        {provide: WINDOW, useValue: window},
        {provide: DOCUMENT, useValue: document},
        {provide: MatPaginatorIntl, useClass: ObPaginatorFixService},
        provideHttpClient(withInterceptorsFromDi()),
        provideAnimations(),
        provideRouter(routes),
    ],
}).catch(err => console.error(err));
