import {DecimalPipe, NgClass} from '@angular/common';
import {Component, input} from '@angular/core';
import {CommitteeQuotas} from '@api/CommitteeQuotas';
import {TranslatePipe} from '@ngx-translate/core';
import {ObEToggleType, ObPopoverDirective} from '@oblique/oblique';

@Component({
    selector: 'apg-members-quotas',
    imports: [DecimalPipe, TranslatePipe, ObPopoverDirective, NgClass],
    templateUrl: './members-quotas.component.html',
    styleUrl: './members-quotas.component.scss',
})
export class MembersQuotasComponent {
    membersQuotas = input.required<CommitteeQuotas>();

    protected readonly ObEToggleType = ObEToggleType;
}
