import {provideHttpClient} from '@angular/common/http';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatButtonToggleGroup, MatButtonToggle, MatButtonToggleChange} from '@angular/material/button-toggle';
import {MatDialog, MatDialogRef} from '@angular/material/dialog';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {MockComponents, MockDirective, MockPipe} from 'ng-mocks';
import {of, Subject, throwError} from 'rxjs';
import {OnlinePublicationComponent} from './online-publication.component';
import {OnlinePublicationService} from './online-publication.service';

describe('OnlinePublicationComponent', () => {
    let component: OnlinePublicationComponent;
    let fixture: ComponentFixture<OnlinePublicationComponent>;

    const mockOnlinePublicationService = {
        getOgdExportSetting: jest.fn().mockReturnValue(of(true)),
        setOgdExportSetting: jest.fn().mockReturnValue(of({})),
        triggerPublication: jest.fn().mockReturnValue(of({})),
    };
    const translateServiceMock = {
        currentLang: 'en',
        onLangChange: new Subject(),
        instant: jest.fn((key: string) => key),
    };
    const mockNotificationService = {
        success: jest.fn(),
        error: jest.fn(),
        info: jest.fn(),
    };
    const mockHttpApiInterceptorEvents = {
        deactivateNotificationOnNextAPICalls: jest.fn(),
        deactivateSpinnerOnNextAPICalls: jest.fn(),
    };
    const dialogAfterClosedSubject = new Subject<boolean>();
    const mockDialog = {
        open: jest.fn().mockReturnValue({
            afterClosed: jest.fn().mockReturnValue(dialogAfterClosedSubject.asObservable()),
        } as Partial<MatDialogRef<ConfirmDialogComponent>>),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [OnlinePublicationComponent, MockComponents(MatButtonToggle), MockPipe(TranslatePipe), MockDirective(MatButtonToggleGroup)],
            providers: [
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: OnlinePublicationService, useValue: mockOnlinePublicationService},
                {provide: ObNotificationService, useValue: mockNotificationService},
                {provide: ObHttpApiInterceptorEvents, useValue: mockHttpApiInterceptorEvents},
                {provide: MatDialog, useValue: mockDialog},
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

    describe('confirmTriggerPublication', () => {
        it('should trigger publication when user confirms dialog', async () => {
            component.confirmTriggerPublication();
            dialogAfterClosedSubject.next(true);

            await fixture.whenStable();

            expect(mockHttpApiInterceptorEvents.deactivateNotificationOnNextAPICalls).toHaveBeenCalled();
            expect(mockNotificationService.info).toHaveBeenCalledWith({
                message: 'onlinePublication.trigger.message',
                timeout: 10000,
            });
            expect(mockOnlinePublicationService.triggerPublication).toHaveBeenCalled();
        });

        it('should not trigger publication when user cancels dialog', async () => {
            component.confirmTriggerPublication();
            dialogAfterClosedSubject.next(false);

            await fixture.whenStable();

            expect(mockOnlinePublicationService.triggerPublication).not.toHaveBeenCalled();
            expect(mockNotificationService.info).not.toHaveBeenCalled();
        });

        it('should show success notification when publication succeeds', async () => {
            component.confirmTriggerPublication();
            dialogAfterClosedSubject.next(true);

            await fixture.whenStable();

            expect(mockNotificationService.success).toHaveBeenCalledWith({
                message: 'onlinePublication.trigger.success',
            });
            expect(mockNotificationService.error).not.toHaveBeenCalled();
        });

        it('should show error notification when publication fails', async () => {
            mockOnlinePublicationService.triggerPublication.mockReturnValueOnce(throwError(() => new Error('Publication failed')));

            component.confirmTriggerPublication();
            dialogAfterClosedSubject.next(true);

            await fixture.whenStable();

            expect(mockNotificationService.error).toHaveBeenCalledWith({
                message: 'onlinePublication.trigger.error',
            });
            expect(mockNotificationService.success).not.toHaveBeenCalled();
        });
    });
});
