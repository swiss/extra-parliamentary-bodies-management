import {Routes} from '@angular/router';
import {ObUnsavedChangesGuard} from '@oblique/oblique';
import {Role} from '../auth/Role';
import {MembershipCreateComponent} from '../memberships/membership-create/membership-create.component';
import {MembershipEditComponent} from '../memberships/membership-edit/membership-edit.component';
import {PersonCreateComponent} from './person-create/person-create.component';
import {PersonDetailsComponent} from './person-details/person-details.component';
import {PersonsComponent} from './persons.component';

export const personRoutes: Routes = [
    {
        path: '',
        component: PersonsComponent,
    },
    {
        path: 'create',
        component: PersonCreateComponent,
        canDeactivate: [ObUnsavedChangesGuard],
        data: {
            allowedRoles: [Role.Admin, Role.Department, Role.Office, Role.Secretariat],
        },
    },
    {
        path: ':id/memberships',
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
        path: ':id',
        component: PersonDetailsComponent,
        canDeactivate: [ObUnsavedChangesGuard],
    },
];
