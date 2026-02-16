import {DecimalPipe, NgClass} from '@angular/common';
import {Component, computed, input} from '@angular/core';
import {MatCard, MatCardContent} from '@angular/material/card';
import {DomSanitizer} from '@angular/platform-browser';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';

@Component({
    selector: 'apg-committee-justifications-overview',
    templateUrl: './committee-justifications-overview.component.html',
    styleUrl: './committee-justifications-overview.component.scss',
    imports: [MatCard, MatCardContent, NgClass, DecimalPipe, TranslatePipe],
})
export class CommitteeJustificationsOverviewComponent {
    committeeDetails = input.required<CommitteeDetails | undefined>();

    percentageSymbol = computed(() => (this.committeeDetails()?.isPercentageBased ? ' %' : ''));

    showMembersJustifications = computed(() => !!this.committeeDetails()?.justificationMembers);
    showGendersJustifications = computed(
        () => this.committeeDetails()?.justificationGenders || this.committeeDetails()?.measuresGenders || this.committeeDetails()?.generalGenderMeasure
    );
    showLanguagesJustifications = computed(
        () => this.committeeDetails()?.justificationLanguages || this.committeeDetails()?.measuresLanguages || this.committeeDetails()?.generalLanguageMeasure
    );

    none = computed(() => {
        this.committeeDetails();

        return this.translateService.instant('common.none');
    });

    justificationMembers = computed(() =>
        this.committeeDetails()?.justificationMembers !== undefined
            ? this.sanitizer.bypassSecurityTrustHtml(this.committeeDetails()!.justificationMembers!)
            : this.none()
    );

    justificationGenders = computed(() =>
        this.committeeDetails()?.justificationGenders !== undefined
            ? this.sanitizer.bypassSecurityTrustHtml(this.committeeDetails()!.justificationGenders!)
            : this.none()
    );

    measuresGenders = computed(() =>
        this.committeeDetails()?.measuresGenders !== undefined ? this.sanitizer.bypassSecurityTrustHtml(this.committeeDetails()!.measuresGenders!) : this.none()
    );

    justificationLanguages = computed(() =>
        this.committeeDetails()?.justificationLanguages !== undefined
            ? this.sanitizer.bypassSecurityTrustHtml(this.committeeDetails()!.justificationLanguages!)
            : this.none()
    );

    measuresLanguages = computed(() =>
        this.committeeDetails()?.measuresLanguages !== undefined
            ? this.sanitizer.bypassSecurityTrustHtml(this.committeeDetails()!.measuresLanguages!)
            : this.none()
    );

    constructor(
        private readonly translateService: TranslateService,
        private readonly sanitizer: DomSanitizer
    ) {}
}
