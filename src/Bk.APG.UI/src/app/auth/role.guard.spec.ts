import {TestBed} from '@angular/core/testing';
import {ActivatedRouteSnapshot, Router, UrlTree} from '@angular/router';
import {BehaviorSubject, Observable} from 'rxjs';
import {AuthService} from './auth.service';
import {Role} from './Role';
import {RoleGuard} from './role.guard';

describe('RoleGuard', () => {
    let guard: RoleGuard;
    const routeMock = {} as ActivatedRouteSnapshot;
    const routerMock = {
        createUrlTree: jest.fn(),
    };

    const urlTree = {};
    const rolesSubject = new BehaviorSubject([] as Role[]);
    const authServiceMock = {
        roles$: rolesSubject.asObservable(),
    };

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                {provide: Router, useValue: routerMock},
                {provide: AuthService, useValue: authServiceMock},
            ],
        });
        routeMock.data = {allowedRoles: [Role.Allow, Role.Admin]};
        guard = TestBed.inject(RoleGuard);
        routerMock.createUrlTree.mockReturnValue(urlTree);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(guard).toBeTruthy();
    });

    it('reroutes to unauthorized if user doesnt have any role', done => {
        const result = guard.canActivate(routeMock) as Observable<UrlTree>;

        result.subscribe(resultValue => {
            expect(resultValue).toEqual(urlTree);
            expect(routerMock.createUrlTree).toHaveBeenCalledWith(['/unauthorized']);
            done();
        });
    });

    it('doesnt reroute if user is admin', done => {
        rolesSubject.next([Role.Admin]);

        const result = guard.canActivate(routeMock) as Observable<boolean>;

        result.subscribe(resultValue => {
            expect(resultValue).toEqual(true);
            expect(routerMock.createUrlTree).toHaveBeenCalledTimes(0);
            done();
        });
    });

    it('doesnt reroute if user is allow', done => {
        rolesSubject.next([Role.Allow]);

        const result = guard.canActivate(routeMock) as Observable<boolean>;

        result.subscribe(resultValue => {
            expect(resultValue).toEqual(true);
            expect(routerMock.createUrlTree).toHaveBeenCalledTimes(0);
            done();
        });
    });
});
