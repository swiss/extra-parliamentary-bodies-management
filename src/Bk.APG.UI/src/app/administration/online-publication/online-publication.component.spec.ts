import {provideHttpClient} from '@angular/common/http';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatButtonToggleGroup, MatButtonToggle, MatButtonToggleChange} from '@angular/material/button-toggle';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObNotificationService} from '@oblique/oblique';
import {MockComponents, MockDirective, MockPipe} from 'ng-mocks';
import {of, Subject} from 'rxjs';
import {OnlinePublicationComponent} from './online-publication.component';
import {OnlinePublicationService} from './online-publication.service';

describe('OnlinePublicationComponent', () => {
    let component: OnlinePublicationComponent;
    let fixture: ComponentFixture<OnlinePublicationComponent>;

    const mockOnlinePublicationService = {
        getOgdExportSetting: jest.fn().mockReturnValue(of(true)),
        setOgdExportSetting: jest.fn().mockReturnValue(of({})),
    };
    const translateServiceMock = {
        currentLang: 'en',
        onLangChange: new Subject(),
    };
    const mockNotificationService = {
        success: jest.fn(),
        error: jest.fn(),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [OnlinePublicationComponent, MockComponents(MatButtonToggle), MockPipe(TranslatePipe), MockDirective(MatButtonToggleGroup)],
            providers: [
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: OnlinePublicationService, useValue: mockOnlinePublicationService},
                {provide: ObNotificationService, useValue: mockNotificationService},
                provideHttpClient(),
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(OnlinePublicationComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('onToggle', () => {
        it('should call OnlinePublicationService with true when toggle event value is true', () => {
            const mockEvent = {value: true} as MatButtonToggleChange;

            component.onToggle(mockEvent);

            expect(mockOnlinePublicationService.setOgdExportSetting).toHaveBeenCalledWith(true);
            expect(mockOnlinePublicationService.setOgdExportSetting).toHaveBeenCalledTimes(1);
            expect(mockNotificationService.success).toHaveBeenCalledWith('onlinePublication.activated');
        });

        it('should call OnlinePublicationService with false when toggle event value is false', () => {
            const mockEvent = {value: false} as MatButtonToggleChange;

            component.onToggle(mockEvent);

            expect(mockOnlinePublicationService.setOgdExportSetting).toHaveBeenCalledWith(false);
            expect(mockOnlinePublicationService.setOgdExportSetting).toHaveBeenCalledTimes(1);
            expect(mockNotificationService.success).toHaveBeenCalledWith('onlinePublication.deactivated');
        });
    });
});
