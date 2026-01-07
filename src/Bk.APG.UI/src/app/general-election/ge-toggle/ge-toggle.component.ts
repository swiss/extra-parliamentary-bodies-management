import {Component} from '@angular/core';
import {MatButtonToggle, MatButtonToggleChange, MatButtonToggleGroup} from '@angular/material/button-toggle';
import {TranslatePipe} from '@ngx-translate/core';
import {GeneralElectionService} from '../general-election.service';

@Component({
    selector: 'apg-ge-toggle',
    imports: [MatButtonToggleGroup, MatButtonToggle, TranslatePipe],
    templateUrl: './ge-toggle.component.html',
    styleUrl: './ge-toggle.component.scss',
})
export class GeneralElectionToggleComponent {
    constructor(protected readonly generalElectionService: GeneralElectionService) {}

    onToggle($event: MatButtonToggleChange) {
        this.generalElectionService.toggleGeneralElection($event.value);
    }
}
