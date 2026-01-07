import {Component} from '@angular/core';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
    selector: 'apg-form-letters',
    imports: [TranslatePipe],
    templateUrl: './form-letters.component.html',
    styleUrl: './form-letters.component.scss',
})
export class FormLettersComponent {}
