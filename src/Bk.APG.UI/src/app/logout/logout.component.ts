import {Component, inject, OnInit, signal} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute} from '@angular/router';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective} from '@oblique/oblique';
import {AuthService} from '../auth/auth.service';
import {LogoutType} from './logout-type';

@Component({
    selector: 'apg-logout',
    templateUrl: './logout.component.html',
    styleUrls: ['./logout.component.scss'],
    imports: [MatButton, ObButtonDirective, TranslatePipe],
})
export class LogoutComponent implements OnInit {
    readonly title = signal<string>('');
    readonly subtitle = signal<string>('');
    readonly showSignIn = signal<boolean>(false);

    private readonly authService = inject(AuthService);
    private readonly route = inject(ActivatedRoute);

    ngOnInit(): void {
        this.route.queryParamMap.subscribe(queryParams => {
            const reason = (queryParams.get('reason') ?? '') as LogoutType | '';
            this.showSignIn.set(true);

            switch (reason) {
                case 'SessionExpired':
                    this.title.set('logout.sessionExpired.title');
                    this.subtitle.set('logout.default.subtitle');
                    break;
                case 'Logout':
                default:
                    this.title.set('logout.default.title');
                    this.subtitle.set('logout.default.subtitle');
            }
        });
    }

    login(): void {
        this.authService.login();
    }
}
