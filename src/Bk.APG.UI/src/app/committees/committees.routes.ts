import {Routes} from '@angular/router';
import {ObUnsavedChangesGuard} from '@oblique/oblique';
import {AuthGuard} from '../auth/auth.guard';
import {Role} from '../auth/Role';
import {RoleGuard} from '../auth/role.guard';
import {MembershipCreateComponent} from '../memberships/membership-create/membership-create.component';
import {MembershipEditComponent} from '../memberships/membership-edit/membership-edit.component';
import {CommitteeCreateComponent} from './committee-create/committee-create.component';
import {AppointmentDecisionCreateComponent} from './committee-details/appointment-decisions/appointment-decision-create/appointment-decision-create.component';
import {AppointmentDecisionEditComponent} from './committee-details/appointment-decisions/appointment-decision-edit/appointment-decision-edit.component';
import {ContactPointCreateComponent} from './committee-details/committee-contact-points/contact-point-create/contact-point-create.component';
import {ContactPointEditComponent} from './committee-details/committee-contact-points/contact-point-edit/contact-point-edit.component';
import {CommitteeDetailsComponent} from './committee-details/committee-details.component';
import {CommitteesComponent} from './committees.component';

export const committeeRoutes: Routes = [
    {
        path: '',
        component: CommitteesComponent,
    },
    {
        path: 'create',
        component: CommitteeCreateComponent,
        data: {
            allowedRoles: [Role.Admin, Role.Department],
        },
        canDeactivate: [ObUnsavedChangesGuard],
    },
    {
        path: ':id/members',
        children: [
            {
                path: 'create',
                component: MembershipCreateComponent,
                canDeactivate: [ObUnsavedChangesGuard],
            },
            {
                path: ':id',
                component: MembershipEditComponent,
                canDeactivate: [ObUnsavedChangesGuard],
            },
        ],
        data: {
            allowedRoles: [Role.Admin, Role.Department, Role.Office, Role.Secretariat],
        },
    },
    {
        path: ':id/appointmentDecisions',
        children: [
            {
                path: 'create',
                component: AppointmentDecisionCreateComponent,
                canActivate: [AuthGuard, RoleGuard],
                canDeactivate: [ObUnsavedChangesGuard],
            },
            {
                path: ':id',
                component: AppointmentDecisionEditComponent,
                canActivate: [AuthGuard, RoleGuard],
                canDeactivate: [ObUnsavedChangesGuard],
            },
        ],
        data: {
            allowedRoles: [Role.Admin, Role.Department, Role.Office, Role.Secretariat],
        },
    },
    {
        path: ':id',
        component: CommitteeDetailsComponent,
        canDeactivate: [ObUnsavedChangesGuard],
    },
    {
        path: ':id/contactpoints',
        children: [
            {
                path: 'create',
                component: ContactPointCreateComponent,
                canDeactivate: [ObUnsavedChangesGuard],
            },
            {
                path: ':id/copy',
                component: ContactPointEditComponent,
                canDeactivate: [ObUnsavedChangesGuard],
            },
            {
                path: ':id',
                component: ContactPointEditComponent,
                canDeactivate: [ObUnsavedChangesGuard],
            },
        ],
        data: {
            allowedRoles: [Role.Admin, Role.Department, Role.Office, Role.Secretariat],
        },
    },
];
