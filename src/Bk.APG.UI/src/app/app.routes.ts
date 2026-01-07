import {Routes} from '@angular/router';
import {AuthGuard} from './auth/auth.guard';
import {Role} from './auth/Role';
import {RoleGuard} from './auth/role.guard';
import {GeneralElectionCommitteesService} from './general-election/ge-committees/ge-committees.service';
import {LogoutComponent} from './logout/logout.component';
import {UnauthorizedComponent} from './unauthorized/unauthorized.component';
import {WorklistService} from './worklist/worklist.service';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'worklist',
        pathMatch: 'full',
    },
    {
        path: 'worklist',
        loadChildren: () => import('./worklist/worklist.routes').then(m => m.worklistRoutes),
        canActivate: [AuthGuard, RoleGuard],
        data: {
            allowedRoles: [Role.Department, Role.Office, Role.Secretariat, Role.Admin],
        },
        providers: [WorklistService],
    },
    {
        path: 'persons',
        loadChildren: () => import('./persons/persons.routes').then(m => m.personRoutes),
        canActivate: [AuthGuard, RoleGuard],
        data: {
            allowedRoles: [Role.Allow],
        },
    },
    {
        path: 'committees',
        loadChildren: () => import('./committees/committees.routes').then(m => m.committeeRoutes),
        canActivate: [AuthGuard, RoleGuard],
        data: {
            allowedRoles: [Role.Allow],
        },
    },
    {
        path: 'administration',
        loadChildren: () => import('./administration/administration.routes').then(m => m.adminRoutes),
        canActivate: [AuthGuard, RoleGuard],
        data: {
            allowedRoles: [Role.Admin, Role.Department],
        },
    },
    {
        path: 'exports',
        loadChildren: () => import('./exports/exports.routes').then(m => m.exportsRoutes),
        canActivate: [AuthGuard, RoleGuard],
        data: {
            allowedRoles: [Role.Allow],
        },
    },
    {
        path: 'general-election',
        loadChildren: () => import('./general-election/general-election.routes').then(m => m.generalElectionRoutes),
        canActivate: [AuthGuard, RoleGuard],
        data: {
            allowedRoles: [Role.Department, Role.Office, Role.Secretariat, Role.Admin, Role.Observer],
        },
        providers: [GeneralElectionCommitteesService],
    },
    {
        path: 'logout',
        component: LogoutComponent,
    },
    {
        path: 'unauthorized',
        component: UnauthorizedComponent,
    },
    {
        path: '**',
        redirectTo: 'unknown-route',
    },
];
