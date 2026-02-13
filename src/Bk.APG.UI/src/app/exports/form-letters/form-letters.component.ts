import {Component} from '@angular/core';
import {MatTabGroup, MatTab} from '@angular/material/tabs';
import {TranslatePipe} from '@ngx-translate/core';
import {FormLettersSenderComponent} from '../form-letters-sender/form-letters-sender.component';
import {RecipientsComponent} from '../recipients/recipients.component';

@Component({
    selector: 'apg-form-letters',
    imports: [TranslatePipe, MatTabGroup, MatTab, RecipientsComponent, FormLettersSenderComponent],
    templateUrl: './form-letters.component.html',
    styleUrl: './form-letters.component.scss',
})
export class FormLettersComponent {}
