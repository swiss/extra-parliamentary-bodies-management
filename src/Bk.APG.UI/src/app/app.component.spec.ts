import {provideHttpClient} from '@angular/common/http';
import {provideHttpClientTesting} from '@angular/common/http/testing';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatTooltipModule} from '@angular/material/tooltip';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObMasterLayoutModule} from '@oblique/oblique';
import {FroalaService} from '@shared/froala.service';
import {MockModule, MockPipe} from 'ng-mocks';
import {BehaviorSubject, Subject} from 'rxjs';
import {AppComponent} from './app.component';
import {AuthService} from './auth/auth.service';
import {Role} from './auth/Role';
import {ConfigsService} from './configs.service';
import {GeneralElectionService} from './general-election/general-election.service';
import {InformationService} from './information.service';

describe('AppComponent', () => {
    let component: AppComponent;
    let fixture: ComponentFixture<AppComponent>;

    const translateServiceMock = {
        currentLang: 'de',
        use: jest.fn(),
    };
    const isAuthenticated$ = new BehaviorSubject<boolean>(true);
    const userName$ = new BehaviorSubject<string>('test_name');
    const roles$Subject = new Subject<Role[]>();
    const authServiceMock = {
        logout: jest.fn(),
        isAuthenticated$: isAuthenticated$.asObservable(),
        userName$: userName$.asObservable(),
        roles$: roles$Subject.asObservable(),
    };

    const configsServiceMock = {
        frontendConfig: {
            eiamMyAccountUrl: 'https://example.com/account',
        },
    };

    const generalElectionServiceMock = {
        isGeneralElectionEnabled: jest.fn().mockReturnValue(false),
    };

    const informationServiceMock = {
        applicationVersion: '1.0.0',
    };

    const froalaServiceMock = {
        initializePlugins: jest.fn(),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                MockModule(ObMasterLayoutModule),
                MockModule(MatButtonModule),
                MockModule(MatIconModule),
                MockModule(MatTooltipModule),
                MockPipe(TranslatePipe),
                AppComponent,
            ],
            providers: [
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: AuthService, useValue: authServiceMock},
                {provide: ConfigsService, useValue: configsServiceMock},
                {provide: GeneralElectionService, useValue: generalElectionServiceMock},
                {provide: InformationService, useValue: informationServiceMock},
                {provide: FroalaService, useValue: froalaServiceMock},
                provideHttpClient(),
                provideHttpClientTesting(),
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(AppComponent);
        component = fixture.componentInstance;

        await fixture.whenStable();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create the app', () => {
        expect(component).toBeTruthy();
    });

    it.each([
        {role: Role.Admin, name: 'Admin', expected: 5},
        {role: Role.Allow, name: 'Allow', expected: 4},
        {role: Role.Department, name: 'Department', expected: 5},
        {role: Role.Office, name: 'Office', expected: 4},
        {role: Role.Secretariat, name: 'Secretariat', expected: 4},
        {role: Role.Observer, name: 'Observer', expected: 3},
    ])('should load navigation for role $name with $expected items', async ({role, expected}) => {
        roles$Subject.next([role]);
        fixture.detectChanges();
        await fixture.whenStable();

        expect(component.navigation().length).toBe(expected);
    });

    it.each([
        {role: Role.Admin, name: 'Admin', expected: 3},
        {role: Role.Allow, name: 'Allow', expected: undefined},
        {role: Role.Department, name: 'Department', expected: 1},
        {role: Role.Office, name: 'Office', expected: undefined},
        {role: Role.Secretariat, name: 'Secretariat', expected: undefined},
        {role: Role.Observer, name: 'Observer', expected: undefined},
    ])('should load administration navigation items for role $name with $expected items', async ({role, expected}) => {
        roles$Subject.next([role]);
        fixture.detectChanges();
        await fixture.whenStable();

        const administrationNavigation = component.navigation().find(nav => nav.id === 'administration')?.children;

        expect(administrationNavigation?.length).toBe(expected);
    });

    it('should call logout', () => {
        component.logout();

        expect(authServiceMock.logout).toHaveBeenCalledTimes(1);
    });

    it.each([{isAuthenticated: true}, {isAuthenticated: false}])(`should set isAuthenticated`, async ({isAuthenticated}) => {
        isAuthenticated$.next(isAuthenticated);

        expect(component.isAuthenticated()).toEqual(isAuthenticated);

        fixture.detectChanges();

        await fixture.whenStable();
    });

    it('should call froalaService.initializePlugins on initialization', async () => {
        await component.ngOnInit();

        expect(froalaServiceMock.initializePlugins).toHaveBeenCalledTimes(1);
    });
});
