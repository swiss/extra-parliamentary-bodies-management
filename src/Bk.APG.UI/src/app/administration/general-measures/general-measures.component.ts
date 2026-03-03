import {ChangeDetectionStrategy, Component, DestroyRef, effect, inject, signal, ViewContainerRef} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatCard, MatCardContent} from '@angular/material/card';
import {MatDialog} from '@angular/material/dialog';
import {MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle} from '@angular/material/expansion';
import {GeneralMeasure} from '@api/GeneralMeasure';
import {GeneralMeasureUpdate} from '@api/GeneralMeasureUpdate';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObButtonDirective, ObErrorMessagesDirective, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {RichTextEditorComponent} from '@shared/rich-text-editor/rich-text-editor.component';
import {filter, map, switchMap, tap} from 'rxjs';
import {
    GeneralMeasuresForwardDialogComponent,
    GeneralMeasuresForwardDialogResult,
} from './general-measures-forward-dialog/general-measures-forward-dialog.component';
import {GeneralMeasuresService} from './general-measures.service';

@Component({
    selector: 'apg-general-measures',
    templateUrl: './general-measures.component.html',
    styleUrl: './general-measures.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        ReactiveFormsModule,
        MatButton,
        MatExpansionPanel,
        MatExpansionPanelHeader,
        MatExpansionPanelTitle,
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
    private readonly dialog = inject(MatDialog);
    private readonly viewContainerRef = inject(ViewContainerRef);
    private readonly translateService = inject(TranslateService);
    private readonly destroyRef = inject(DestroyRef);

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
            this.loadGeneralMeasures();
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

    protected openForwardDialog(generalMeasure: GeneralMeasure): void {
        this.dialog
            .open(GeneralMeasuresForwardDialogComponent, {
                viewContainerRef: this.viewContainerRef,
                data: {forwardToAdmin: generalMeasure.canForwardToAdmin},
            })
            .afterClosed()
            .pipe(
                filter((result): result is GeneralMeasuresForwardDialogResult => typeof result?.message === 'string' && result.message.length > 0),
                tap(() => this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls()),
                switchMap(result =>
                    this.generalMeasuresService
                        .forward(generalMeasure.departmentId, result.message, result.forwardToAdmin)
                        .pipe(map(() => result.forwardToAdmin))
                ),
                takeUntilDestroyed(this.destroyRef)
            )
            .subscribe({
                next: forwardToAdmin => {
                    this.notificationService.success(
                        forwardToAdmin ? 'generalMeasures.workflow.forwardToAdmin.success' : 'generalMeasures.workflow.forwardToDepartment.success'
                    );
                    this.loadGeneralMeasures();
                },
                error: () => {
                    this.notificationService.error(
                        generalMeasure.canForwardToAdmin
                            ? 'generalMeasures.workflow.forwardToAdmin.error'
                            : 'generalMeasures.workflow.forwardToDepartment.error'
                    );
                },
            });
    }

    protected openValidateDialog(departmentId: string): void {
        this.dialog
            .open(ConfirmDialogComponent, {
                width: '500px',
                viewContainerRef: this.viewContainerRef,
                data: {
                    title: this.translateService.instant('generalMeasures.workflow.validate.confirm.title'),
                    message: this.translateService.instant('generalMeasures.workflow.validate.confirm.text'),
                },
            })
            .afterClosed()
            .pipe(
                filter((result): result is boolean => result === true),
                tap(() => this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls()),
                switchMap(() => this.generalMeasuresService.validate(departmentId)),
                takeUntilDestroyed(this.destroyRef)
            )
            .subscribe({
                next: () => {
                    this.notificationService.success('generalMeasures.workflow.validate.success');
                    this.loadGeneralMeasures();
                },
                error: () => this.notificationService.error('generalMeasures.workflow.validate.error'),
            });
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

    private loadGeneralMeasures(): void {
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
    }
}
