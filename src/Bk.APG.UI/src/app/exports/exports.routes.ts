import {Routes} from '@angular/router';
import {AuthGuard} from '../auth/auth.guard';
import {Role} from '../auth/Role';
import {RoleGuard} from '../auth/role.guard';

export const exportsRoutes: Routes = [
    {path: '', redirectTo: 'exports', pathMatch: 'full'},
    {
        path: 'data-analysis',
        loadComponent: () => import('./data-analysis/data-analysis.component').then(m => m.DataAnalysisComponent),
        canActivate: [AuthGuard, RoleGuard],
        data: {
            allowedRoles: [Role.Allow],
        },
    },
    {
        path: 'requestsAndReports',
        loadComponent: () => import('./requests-and-reports/requests-and-reports.component').then(m => m.RequestsAndReportsComponent),
        canActivate: [AuthGuard, RoleGuard],
        data: {
            allowedRoles: [Role.Allow],
        },
    },
    {
        path: '**',
        redirectTo: 'unknown-route',
    },
];
