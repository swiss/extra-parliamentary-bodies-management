import {CdkTextareaAutosize} from '@angular/cdk/text-field';
import {Component, inject} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatOption} from '@angular/material/core';
import {MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle} from '@angular/material/dialog';
import {MatError, MatFormField, MatLabel} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatSelect} from '@angular/material/select';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObErrorMessagesDirective, ObMatErrorDirective} from '@oblique/oblique';

export interface GeneralMeasuresForwardDialogData {
    forwardToAdmin: boolean;
}

export interface GeneralMeasuresForwardDialogResult {
    message: string;
    forwardToAdmin: boolean;
}

@Component({
    selector: 'apg-general-measures-forward-dialog',
    imports: [
        MatDialogTitle,
        MatDialogContent,
        MatDialogActions,
        MatButton,
        MatDialogClose,
        ObButtonDirective,
        TranslatePipe,
        ReactiveFormsModule,
        MatError,
        MatFormField,
        MatLabel,
        MatSelect,
        MatOption,
        MatInput,
        ObErrorMessagesDirective,
        ObMatErrorDirective,
        CdkTextareaAutosize,
    ],
    templateUrl: './general-measures-forward-dialog.component.html',
    styleUrl: './general-measures-forward-dialog.component.scss',
})
export class GeneralMeasuresForwardDialogComponent {
    form = new FormGroup({
        message: new FormControl('', {nonNullable: true, validators: Validators.required}),
    });

    protected readonly forwardToAdmin = inject<GeneralMeasuresForwardDialogData>(MAT_DIALOG_DATA).forwardToAdmin;
    protected readonly forwardToLabelKey = this.forwardToAdmin
        ? 'generalMeasures.workflow.forward.dialog.forwardTo.admin'
        : 'generalMeasures.workflow.forward.dialog.forwardTo.department';

    private readonly dialogRef = inject(MatDialogRef<GeneralMeasuresForwardDialogComponent, GeneralMeasuresForwardDialogResult>);

    protected forward(): void {
        if (!this.form.valid) {
            this.form.markAllAsTouched();
            return;
        }

        this.dialogRef.close({
            message: this.form.controls.message.value,
            forwardToAdmin: this.forwardToAdmin,
        });
    }
}
