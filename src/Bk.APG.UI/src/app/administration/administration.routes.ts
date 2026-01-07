import {Routes} from '@angular/router';
import {ObUnsavedChangesGuard} from '@oblique/oblique';
import {Role} from '../auth/Role';
import {RoleGuard} from '../auth/role.guard';
import {CommitteeTypeEditComponent} from './committee-type/committee-type-edit/committee-type-edit.component';
import {CommitteeTypeListComponent} from './committee-type/committee-type-list/committee-type-list.component';
import {GeneralMeasuresComponent} from './general-measures/general-measures.component';
import {OnlinePublicationComponent} from './online-publication/online-publication.component';

export const adminRoutes: Routes = [
    {
        path: 'committeeTypes',
        component: CommitteeTypeListComponent,
        canActivate: [RoleGuard],
        data: {
            allowedRoles: [Role.Admin],
        },
    },
    {
        path: 'committeeTypes/:id',
        component: CommitteeTypeEditComponent,
        canActivate: [RoleGuard],
        canDeactivate: [ObUnsavedChangesGuard],
        data: {
            allowedRoles: [Role.Admin],
        },
    },
    {
        path: 'generalMeasures',
        component: GeneralMeasuresComponent,
        canActivate: [RoleGuard],
        data: {
            allowedRoles: [Role.Admin, Role.Department],
        },
    },
    {
        path: 'onlinePublication',
        component: OnlinePublicationComponent,
        canActivate: [RoleGuard],
        data: {
            allowedRoles: [Role.Admin, Role.Department],
        },
    },
];
