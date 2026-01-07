import {DatePipe} from '@angular/common';
import {Component, Input} from '@angular/core';
import {MatCard, MatCardContent} from '@angular/material/card';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
    selector: 'apg-committee-overview-basic-data',
    templateUrl: './committee-overview-basic-data.component.html',
    styleUrl: './committee-overview-basic-data.component.scss',
    imports: [MatCard, MatCardContent, DatePipe, TranslatePipe],
})
export class CommitteeOverviewBasicDataComponent {
    @Input() committee!: CommitteeDetails | undefined;
}
