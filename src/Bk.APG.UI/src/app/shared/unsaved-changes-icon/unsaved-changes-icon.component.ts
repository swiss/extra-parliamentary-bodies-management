import {Component, Input} from '@angular/core';
import {MatIcon} from '@angular/material/icon';
import {MatTooltip} from '@angular/material/tooltip';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
    selector: 'apg-unsaved-changes-icon',
    templateUrl: './unsaved-changes-icon.component.html',
    styleUrl: './unsaved-changes-icon.component.scss',
    imports: [MatIcon, MatTooltip, TranslatePipe],
})
export class UnsavedChangesIconComponent {
    @Input() toolTip: string | undefined;
}
