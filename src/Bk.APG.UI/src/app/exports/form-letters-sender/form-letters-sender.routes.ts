import {Routes} from '@angular/router';
import {ObUnsavedChangesGuard} from '@oblique/oblique';
import {FormLettersSenderCreateComponent} from './form-letters-sender-create/form-letters-sender-create.component';
import {FormLettersSenderEditComponent} from './form-letters-sender-edit/form-letters-sender-edit.component';
import {FormLettersSenderComponent} from './form-letters-sender.component';

export const formLettersSenderRoutes: Routes = [
    {
        path: '',
        component: FormLettersSenderComponent,
    },
    {
        path: 'create',
        component: FormLettersSenderCreateComponent,
        canDeactivate: [ObUnsavedChangesGuard],
    },
    {
        path: ':id',
        component: FormLettersSenderEditComponent,
        canDeactivate: [ObUnsavedChangesGuard],
    },
];
