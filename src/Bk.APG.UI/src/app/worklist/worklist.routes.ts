import {Routes} from '@angular/router';
import {ObUnsavedChangesGuard} from '@oblique/oblique';
import {Role} from '../auth/Role';

export const worklistRoutes: Routes = [
    {path: '', loadComponent: () => import('./worklist.component').then(x => x.WorklistComponent), pathMatch: 'full'},
    {
        path: 'create',
        loadComponent: () => import('./worklist-task-create/worklist-task-create.component').then(x => x.WorklistTaskCreateComponent),
        canDeactivate: [ObUnsavedChangesGuard],
        data: {
            allowedRoles: [Role.Admin],
        },
    },
    {
        path: ':id',
        loadComponent: () => import('./worklist-task-edit/worklist-task-edit.component').then(x => x.WorklistTaskEditComponent),
        canDeactivate: [ObUnsavedChangesGuard],
        data: {
            allowedRoles: [Role.Allow],
        },
    },
];
