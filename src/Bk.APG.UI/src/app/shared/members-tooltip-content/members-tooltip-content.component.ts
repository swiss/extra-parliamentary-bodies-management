import {Component} from '@angular/core';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
    selector: 'apg-members-tooltip-content',
    imports: [TranslatePipe],
    templateUrl: './members-tooltip-content.component.html',
    styleUrl: './members-tooltip-content.component.scss',
})
export class MembersTooltipContentComponent {}
