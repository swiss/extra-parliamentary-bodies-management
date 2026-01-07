import {Component, effect, inject, signal} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatCard, MatCardContent} from '@angular/material/card';
import {MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle} from '@angular/material/expansion';
import {GeneralMeasure} from '@api/GeneralMeasure';
import {GeneralMeasureUpdate} from '@api/GeneralMeasureUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObErrorMessagesDirective, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {RichTextEditorComponent} from '@shared/rich-text-editor/rich-text-editor.component';
import {GeneralMeasuresService} from './general-measures.service';

@Component({
    selector: 'apg-general-measures',
    templateUrl: './general-measures.component.html',
    styleUrl: './general-measures.component.scss',
    imports: [
        ReactiveFormsModule,
        MatButton,
        MatExpansionPanel,
        MatExpansionPanelHeader,
        MatExpansionPanelTitle,
        MatCard,
        MatCardContent,
        ObButtonDirective,
        ObErrorMessagesDirective,
        HelpTooltipComponent,
        RichTextEditorComponent,
        TranslatePipe,
    ],
})
export class GeneralMeasuresComponent {
    protected readonly generalMeasures = signal<GeneralMeasure[]>([]);
    protected form = new FormGroup({});

    private readonly masterDataService = inject(MasterDataService);
    private readonly generalMeasuresService = inject(GeneralMeasuresService);
    private readonly notificationService = inject(ObNotificationService);
    private readonly httpApiInterceptorEvents = inject(ObHttpApiInterceptorEvents);

    constructor() {
        effect(() => {
            const departments = this.masterDataService.departments();

            if (departments.length === 0) {
                return;
            }

            const formGroups: {[key: string]: FormGroup} = {};

            for (const department of departments) {
                formGroups[department.id] = new FormGroup({
                    justificationLanguages: new FormControl<string | null>(null),
                    justificationGenders: new FormControl<string | null>(null),
                });
            }

            this.form = new FormGroup(formGroups);

            this.generalMeasuresService.getGeneralMeasures().subscribe(generalMeasures => {
                this.generalMeasures.set(generalMeasures);
                for (const measure of generalMeasures) {
                    const generalMeasureGroup = this.getGeneralMeasureFormGroup(measure.departmentId);
                    if (generalMeasureGroup) {
                        generalMeasureGroup.patchValue({
                            justificationLanguages: measure.justificationLanguages ?? null,
                            justificationGenders: measure.justificationGenders ?? null,
                        });
                    }
                }
            });
        });
    }

    protected saveGeneralMeasure(departmentId: string): void {
        const generalMeasureGroup = this.getGeneralMeasureFormGroup(departmentId);
        if (!generalMeasureGroup) {
            return;
        }

        if (!generalMeasureGroup.valid) {
            generalMeasureGroup.markAsTouched();
            return;
        }

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        const generalMeasureUpdate = this.getGeneralMeasureFormValue(departmentId);
        this.generalMeasuresService.saveGeneralMeasure(generalMeasureUpdate).subscribe({
            next: async () => {
                generalMeasureGroup.markAsPristine();
                generalMeasureGroup.markAsUntouched();
                return this.notificationService.success('generalMeasures.save.success');
            },
            error: () => this.notificationService.error('generalMeasures.save.error'),
        });
    }

    protected resetGeneralMeasure(departmentId: string): void {
        const generalMeasureGroup = this.getGeneralMeasureFormGroup(departmentId);
        if (!generalMeasureGroup) {
            return;
        }

        const originalMeasure = this.generalMeasures().find(x => x.departmentId === departmentId);
        generalMeasureGroup.reset({
            justificationLanguages: originalMeasure?.justificationLanguages ?? null,
            justificationGenders: originalMeasure?.justificationGenders ?? null,
        });
    }

    protected getGeneralMeasureFormGroup(departmentId: string): FormGroup {
        return this.form.get(departmentId) as FormGroup;
    }

    private getGeneralMeasureFormValue(departmentId: string): GeneralMeasureUpdate {
        const generalMeasureGroup = this.getGeneralMeasureFormGroup(departmentId);

        if (!generalMeasureGroup) {
            return {departmentId};
        }

        const {justificationLanguages, justificationGenders} = generalMeasureGroup.getRawValue();

        return {
            departmentId,
            justificationLanguages,
            justificationGenders,
        };
    }
}
