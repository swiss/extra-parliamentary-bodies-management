import {Component, computed, input, ViewChild} from '@angular/core';
import {MatIconModule} from '@angular/material/icon';
import {TranslatePipe} from '@ngx-translate/core';
import {ObEToggleType, ObPopoverDirective} from '@oblique/oblique';

@Component({
    selector: 'apg-help-tooltip',
    imports: [MatIconModule, ObPopoverDirective, TranslatePipe],
    templateUrl: './help-tooltip.component.html',
    styleUrl: './help-tooltip.component.scss',
})
export class HelpTooltipComponent {
    text = input<string>();
    texts = input<string[]>();
    size = input<'normal' | 'big'>('normal');
    toggleType = input<'hover' | 'click'>('hover');
    openOnHover = input<boolean>(false);

    @ViewChild(ObPopoverDirective) protected popoverDirective?: ObPopoverDirective;

    protected toggleHandle = computed<ObEToggleType>(() => (this.toggleType() === 'click' ? ObEToggleType.CLICK : ObEToggleType.HOVER));

    protected onMouseEnter(): void {
        if (this.openOnHover() && this.toggleType() === 'click') {
            if (this.popoverDirective && !this.popoverDirective.isExpanded) {
                this.popoverDirective.open();
            }
        }
    }
}
