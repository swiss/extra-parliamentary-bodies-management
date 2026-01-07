import {Component, Input} from '@angular/core';
import {PersonDetails} from '@api/PersonDetails';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
    selector: 'apg-person-overview-basic-data',
    templateUrl: './person-overview-basic-data.component.html',
    styleUrl: './person-overview-basic-data.component.scss',
    imports: [TranslatePipe],
})
export class PersonOverviewBasicDataComponent {
    @Input() personDetails!: PersonDetails | undefined;
}
