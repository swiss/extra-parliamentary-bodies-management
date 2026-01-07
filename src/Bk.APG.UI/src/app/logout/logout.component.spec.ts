import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {ActivatedRoute, RouterModule} from '@angular/router';
import {TranslateModule} from '@ngx-translate/core';
import {MockModule} from 'ng-mocks';
import {of} from 'rxjs';
import {AuthService} from '../auth/auth.service';
import {LogoutComponent} from './logout.component';

describe('LogoutComponent', () => {
    let component: LogoutComponent;
    let fixture: ComponentFixture<LogoutComponent>;

    const authServiceMock = {
        login: jest.fn(),
    };

    beforeEach(async () => {
        const activatedRouteStub = {
            queryParamMap: of({
                get: () => 'Logout',
            }),
        };

        await TestBed.configureTestingModule({
            imports: [MockModule(RouterModule), MockModule(MatButtonModule), MockModule(MatIconModule), MockModule(TranslateModule), LogoutComponent],
            providers: [
                {provide: ActivatedRoute, useValue: activatedRouteStub},
                {provide: AuthService, useValue: authServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(LogoutComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should call login on login', () => {
        component.login();

        expect(authServiceMock.login).toHaveBeenCalledTimes(1);
    });
});
