import {Routes} from '@angular/router';
import {ObUnsavedChangesGuard} from '@oblique/oblique';
import {Role} from '../auth/Role';
import {GeneralElectionCommitteeExistsGuard} from './ge-committees/ge-committee-exists.guard';

export const generalElectionRoutes: Routes = [
    {path: '', redirectTo: 'committees', pathMatch: 'full'},
    {
        path: 'committees',
        loadComponent: () => import('./ge-committees/ge-committees.component').then(m => m.GeneralElectionCommitteesComponent),
    },
    {
        path: 'committees/:id/membership-candidate',
        canActivate: [GeneralElectionCommitteeExistsGuard],
        children: [
            {
                path: ':membershipCandidateId',
                canActivate: [GeneralElectionCommitteeExistsGuard],
                canDeactivate: [ObUnsavedChangesGuard],
                loadComponent: () =>
                    import('./membership-candidates/membership-candidate-edit/membership-candidate-edit.component').then(
                        m => m.MembershipCandidateEditComponent
                    ),
            },
        ],
        data: {
            allowedRoles: [Role.Admin, Role.Department, Role.Office, Role.Secretariat],
        },
    },
    {
        path: 'committees/:id',
        canActivate: [GeneralElectionCommitteeExistsGuard],
        canDeactivate: [ObUnsavedChangesGuard],
        loadComponent: () =>
            import('./ge-committees/ge-committee-details/ge-committee-details.component').then(m => m.GeneralElectionCommitteeDetailsComponent),
    },
    {
        path: 'exports/requestsAndReports',
        loadComponent: () => import('../exports/requests-and-reports/requests-and-reports.component').then(m => m.RequestsAndReportsComponent),
        data: {
            isGeneralElection: true,
            allowedRoles: [Role.Allow],
        },
    },
    {
        path: 'exports/formLetters',
        loadComponent: () => import('../exports/form-letters/form-letters.component').then(m => m.FormLettersComponent),
        data: {
            isGeneralElection: true,
            allowedRoles: [Role.Allow],
        },
    },
    {
        path: '**',
        redirectTo: 'committees',
    },
];
