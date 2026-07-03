import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ObIBanner} from '@oblique/oblique';
import {LogLevel, OpenIdConfiguration} from 'angular-auth-oidc-client';
import {Observable, tap} from 'rxjs';

export interface FrontendConfig {
    banner: string;
    isProduction: boolean;
    keyCloakUrl: string;
    keyCloakClientId: string;
    keyCloakRedirectUrl: string;
    keyCloakRoleAllow: string;
    keyCloakRoleAdmin: string;
    keyCloakRoleDepartment: string;
    keyCloakRoleOffice: string;
    keyCloakRoleSecretariat: string;
    keyCloakRoleObserver: string;
    eiamMyAccountUrl: string;
    froalaKey: string;
    entityIds: EntityIdConfig;
    openDataStack: OpenDataStackConfig;
}

export interface EntityIdConfig {
    appointmentDecisionType: AppointmentDecisionTypeConfig;
    appointmentDecisionLinkType: AppointmentDecisionLinkTypeConfig;
    language: LanguageIdConfig;
    gender: GenderIdConfig;
    salutation: SalutationIdConfig;
    canton: CantonIdConfig;
    electionOffice: ElectionOfficeIdConfig;
    committeeType: CommitteeTypeIdConfig;
    committeeLevel: CommitteeLevelIdConfig;
    electionType: ElectionTypeIdConfig;
    worklistTaskType: WorklistTaskTypeConfig;
    termOfOffice: TermOfOfficeIdConfig;
    contactPoint: ContactPointIdConfig;
    country: CountryIdConfig;
}

export interface ContactPointIdConfig {
    secretariatId: string;
    dpoId: string;
}

export interface TermOfOfficeIdConfig {
    period4YearsInGeneralElectionId: string;
}

export interface WorklistTaskTypeConfig {
    generalElectionStartId: string;
    generalElectionEndId: string;
}

export interface AppointmentDecisionTypeConfig {
    decisionFederalCouncilId: string;
    institutionId: string;
    reportId: string;
    otherId: string;
    regulationsId: string;
}

export interface AppointmentDecisionLinkTypeConfig {
    exeLinkTypeId: string;
    standardLinkTypeId: string;
}
export interface LanguageIdConfig {
    germanLanguageId: string;
    frenchLanguageId: string;
    italianLanguageId: string;
    romanshLanguageId: string;
}

export interface GenderIdConfig {
    maleId: string;
    femaleId: string;
}

export interface SalutationIdConfig {
    maleId: string;
    femaleId: string;
}

export interface CantonIdConfig {
    abroadId: string;
}

export interface ElectionOfficeIdConfig {
    federalGovernmentId: string;
    otherId: string;
}

export interface CommitteeTypeIdConfig {
    /** Behördenkommission */
    authorityId: string;

    /** Verwaltungskommission */
    administrationId: string;

    /** Leitungsorgane */
    managementId: string;

    /** Vertretungen des Bundes */
    federalAgenciesId: string;

    /** Vertretungen des Bundes in grenzüberschreitenden Gremien */
    federalAgenciesCrossBorderId: string;
}

export interface CommitteeLevelIdConfig {
    federalCouncilId: string;
}

export interface ElectionTypeIdConfig {
    newElectionId: string;
    reElectionId: string;
    maximumDutyRetirementId: string;
    deceasedId: string;
}

export interface CountryIdConfig {
    switzerlandId: string;
}

export interface OpenDataStackConfig {
    baseUrl: string;
    initialDashboardId: string;
}

@Injectable({providedIn: 'root'})
export class ConfigsService {
    bannerInfo!: ObIBanner;
    openIdConfiguration!: OpenIdConfiguration;
    frontendConfig!: FrontendConfig;

    constructor(private readonly http: HttpClient) {}

    loadFrontendConfig(): Observable<FrontendConfig> {
        return this.http.get<FrontendConfig>('/api/configs/frontend').pipe(
            tap(c => {
                this.bannerInfo = {text: c.banner};

                this.openIdConfiguration = {
                    authority: c.keyCloakUrl,
                    redirectUrl: c.keyCloakRedirectUrl,
                    postLogoutRedirectUri: `${c.keyCloakRedirectUrl}/logout`,
                    unauthorizedRoute: `${c.keyCloakRedirectUrl}/unauthorized`,
                    clientId: c.keyCloakClientId,
                    scope: 'openid',
                    responseType: 'code',
                    ignoreNonceAfterRefresh: true,
                    useRefreshToken: true,
                    silentRenew: true,
                    postLoginRoute: '/',
                    renewTimeBeforeTokenExpiresInSeconds: 30,
                    logLevel: c.isProduction ? LogLevel.Error : LogLevel.Warn,
                    secureRoutes: ['/api/'],
                };

                this.frontendConfig = c;
            })
        );
    }
}
