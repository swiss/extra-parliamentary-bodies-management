import {Component, Inject} from '@angular/core';
import {MatButtonModule} from '@angular/material/button';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective} from '@oblique/oblique';

@Component({
    selector: 'apg-confirm-dialog',
    imports: [TranslatePipe, MatDialogModule, MatButtonModule, ObButtonDirective],
    templateUrl: './confirm-dialog.component.html',
    styleUrl: './confirm-dialog.component.scss',
})
export class ConfirmDialogComponent {
    constructor(
        @Inject(MAT_DIALOG_DATA) public data: {title?: string; message?: string},
        private readonly dialogRef: MatDialogRef<ConfirmDialogComponent>
    ) {}

    onYes() {
        this.dialogRef.close(true);
    }

    onNo() {
        this.dialogRef.close(false);
    }
}
