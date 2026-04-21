import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatDatepicker, MatDatepickerModule, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormField} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObHttpApiInterceptorEvents, ObNotificationService, WINDOW} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import * as fileUtil from '@shared/file-util';
import {DOCUMENT} from '@shared/injection.tokens';
import {MockComponent, MockComponents, MockModule, MockPipe} from 'ng-mocks';
import {of, Subject, throwError} from 'rxjs';
import {ConfigsService} from '../../../app/configs.service';
import {OpenDataStackComponent} from '../open-data-stack/open-data-stack.component';
import {DataAnalysisComponent} from './data-analysis.component';
import {DataAnalysisService} from './data-analysis.service';

describe('DataAnalysisComponent', () => {
    let component: DataAnalysisComponent;
    let fixture: ComponentFixture<DataAnalysisComponent>;
    let dataAnalysisServiceMock: jest.Mocked<DataAnalysisService>;
    let interceptorEventsMock: jest.Mocked<ObHttpApiInterceptorEvents>;
    let notificationServiceMock: jest.Mocked<ObNotificationService>;
    let configsServiceMock: jest.Mocked<ConfigsService>;

    beforeEach(async () => {
        const translateServiceMock = {
            getCurrentLang: jest.fn(() => 'en'),
            onLangChange: new Subject(),
        };

        const errorServiceMock = {
            getControlError: jest.fn(),
        };

        dataAnalysisServiceMock = {
            generateExport: jest.fn().mockReturnValue(of(new Blob())),
        } as unknown as jest.Mocked<DataAnalysisService>;

        interceptorEventsMock = {
            deactivateSpinnerOnNextAPICalls: jest.fn(),
            deactivateNotificationOnNextAPICalls: jest.fn(),
        } as unknown as jest.Mocked<ObHttpApiInterceptorEvents>;

        notificationServiceMock = {
            info: jest.fn(),
            success: jest.fn(),
            error: jest.fn(),
        } as unknown as jest.Mocked<ObNotificationService>;

        configsServiceMock = {
            frontendConfig: {
                openDataStack: {
                    enabled: true,
                },
            },
        } as unknown as jest.Mocked<ConfigsService>;

        await TestBed.configureTestingModule({
            imports: [
                DataAnalysisComponent,
                MockModule(MatIconModule),
                MockModule(MatDatepickerModule),
                MockComponents(MatFormField, MatDatepicker, MatDatepickerToggle),
                MockPipe(TranslatePipe),
                MockComponent(OpenDataStackComponent),
            ],
            providers: [
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: ErrorService, useValue: errorServiceMock},
                {provide: DataAnalysisService, useValue: dataAnalysisServiceMock},
                {provide: ObHttpApiInterceptorEvents, useValue: interceptorEventsMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: ConfigsService, useValue: configsServiceMock},
                {provide: WINDOW, useValue: window},
                {provide: DOCUMENT, useValue: document},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(DataAnalysisComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should call dataAnalysisService.generateExport with correct parameters', () => {
        const exportType = 'committee-type';
        const analysisDate = new Date(2024, 0, 1);
        component.dataAnalysisForm.controls.analysisDate.setValue(analysisDate);
        dataAnalysisServiceMock.generateExport = jest.fn().mockReturnValue(of(new Blob()));

        component.generateExport(exportType);

        expect(dataAnalysisServiceMock.generateExport).toHaveBeenCalledWith(exportType, analysisDate);
    });

    it('should show info notification when export starts', () => {
        component.generateExport('committee-type');

        expect(notificationServiceMock.info).toHaveBeenCalledWith(
            expect.objectContaining({
                title: 'dataAnalysis.export.title',
                message: 'dataAnalysis.export.message',
                timeout: 10000,
            })
        );
    });

    it('should show success notification and update successful exports on success', () => {
        const exportType = 'committee-type';

        component.generateExport(exportType);

        expect(notificationServiceMock.success).toHaveBeenCalledWith(
            expect.objectContaining({
                message: 'dataAnalysis.export.success',
            })
        );
        expect(component.successfulExports()).toContain(exportType);
    });

    it('should call downloadFileFromHttpResponse on success', () => {
        const downloadSpy = jest.spyOn(fileUtil, 'downloadFileFromHttpResponse').mockImplementation(() => {});

        component.generateExport('committee-type');

        expect(downloadSpy).toHaveBeenCalled();
        downloadSpy.mockRestore();
    });

    it('should show error notification and update failed exportsand update failed exports on error', () => {
        const exportType = 'committee-type';

        dataAnalysisServiceMock.generateExport = jest.fn().mockReturnValue(throwError(() => new Error('fail')));

        component.generateExport(exportType);

        expect(notificationServiceMock.error).toHaveBeenCalledWith(
            expect.objectContaining({
                message: 'dataAnalysis.export.error',
            })
        );
        expect(component.failedExports()).toContain(exportType);
    });
});
