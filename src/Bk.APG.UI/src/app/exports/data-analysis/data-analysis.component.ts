import {Component, Inject, signal, effect, DOCUMENT, inject, computed, Signal} from '@angular/core';
import {toSignal} from '@angular/core/rxjs-interop';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatCard, MatCardContent} from '@angular/material/card';
import {MatDatepickerInput, MatDatepickerToggle, MatDatepicker} from '@angular/material/datepicker';
import {MatIcon} from '@angular/material/icon';
import {MatInput} from '@angular/material/input';
import {MatError, MatFormField, MatLabel, MatSuffix} from '@angular/material/select';
import {ExportType} from '@api/DataAnalysis';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonModule, ObHttpApiInterceptorEvents, ObNotificationService, WINDOW} from '@oblique/oblique';
import {today} from '@shared/date-util';
import {ErrorService} from '@shared/error-service.service';
import {downloadFileFromHttpResponse} from '@shared/file-util';
import {defer, finalize} from 'rxjs';
import {ConfigsService} from '../../../app/configs.service';
import {OpenDataStackComponent} from '../open-data-stack/open-data-stack.component';
import {DataAnalysisService} from './data-analysis.service';

@Component({
    selector: 'apg-data-analysis',
    templateUrl: './data-analysis.component.html',
    styleUrl: './data-analysis.component.scss',
    imports: [
        MatCard,
        MatCardContent,
        MatError,
        MatFormField,
        MatLabel,
        MatInput,
        MatIcon,
        MatDatepickerInput,
        MatDatepickerToggle,
        MatDatepicker,
        MatSuffix,
        ReactiveFormsModule,
        TranslatePipe,
        ObButtonModule,
        MatButton,
        OpenDataStackComponent,
    ],
})
export class DataAnalysisComponent {
    dataAnalysisForm = new FormGroup({
        analysisDate: new FormControl<Date>({value: today(), disabled: false}, {nonNullable: true, validators: [Validators.required]}),
    });

    exportTypes: ExportType[] = [
        'committee-types',
        'committees',
        'memberships',
        'membership-interests',
        'persons',
        'ages',
        'regions',
        'secretariats',
        'data-protection-officers',
    ];

    loadingExports = signal<ExportType[]>([]);
    successfulExports = signal<ExportType[]>([]);
    failedExports = signal<ExportType[]>([]);

    analysisDate = toSignal(this.dataAnalysisForm.controls.analysisDate.valueChanges, {initialValue: today()});

    protected readonly openDataStackEnabled: Signal<boolean>;

    protected readonly errorService = inject(ErrorService);
    private readonly dataAnalysisService = inject(DataAnalysisService);
    private readonly interceptorEvents = inject(ObHttpApiInterceptorEvents);
    private readonly notificationService = inject(ObNotificationService);
    private readonly configsService = inject(ConfigsService);

    constructor(
        @Inject(WINDOW) private readonly window: Window,
        @Inject(DOCUMENT) private readonly document: Document
    ) {
        this.openDataStackEnabled = computed(() => this.configsService.frontendConfig.openDataStack.enabled);

        effect(() => {
            this.successfulExports.set([]);
            this.failedExports.set([]);
            this.analysisDate();
        });
    }

    generateExport(exportType: ExportType): void {
        const fallbackFilename = `export.xlsx`;

        defer(() => {
            this.failedExports.update(exports => exports.filter(e => e !== exportType));
            this.successfulExports.update(exports => exports.filter(e => e !== exportType));
            this.loadingExports.update(exports => [...exports, exportType]);

            this.interceptorEvents.deactivateSpinnerOnNextAPICalls(1);
            this.interceptorEvents.deactivateNotificationOnNextAPICalls(1);

            this.notificationService.info({
                title: 'dataAnalysis.export.title',
                message: 'dataAnalysis.export.message',
                timeout: 10000,
            });

            return this.dataAnalysisService.generateExport(exportType, this.analysisDate());
        })
            .pipe(finalize(() => this.loadingExports.update(exports => exports.filter(e => e !== exportType))))
            .subscribe({
                next: response => {
                    this.notificationService.success({
                        message: 'dataAnalysis.export.success',
                    });

                    this.successfulExports.update(exports => [...exports, exportType]);

                    downloadFileFromHttpResponse(response, fallbackFilename, this.window, this.document);
                },
                error: () => {
                    this.notificationService.error({
                        message: 'dataAnalysis.export.error',
                    });

                    this.failedExports.update(exports => [...exports, exportType]);
                },
            });
    }
}
