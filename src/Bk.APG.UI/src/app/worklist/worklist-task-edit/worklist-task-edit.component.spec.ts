/* eslint-disable @typescript-eslint/no-explicit-any */
import {provideHttpClient} from '@angular/common/http';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {provideNativeDateAdapter} from '@angular/material/core';
import {ActivatedRoute, Router} from '@angular/router';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObMatErrorDirective, ObNotificationService, ObUnsavedChangesDirective} from '@oblique/oblique';
import {MockDirectives, MockPipe} from 'ng-mocks';
import {of} from 'rxjs';
import {WorklistService} from '../worklist.service';
import {WorklistTaskEditComponent} from './worklist-task-edit.component';

describe('WorklistTaskEditComponent', () => {
    let component: WorklistTaskEditComponent;
    let fixture: ComponentFixture<WorklistTaskEditComponent>;

    let routerSpy: jest.Mocked<Router>;
    let notificationServiceSpy: jest.Mocked<ObNotificationService>;
    let worklistServiceSpy: jest.Mocked<WorklistService>;
    let activatedRouteSpy: Partial<ActivatedRoute>;

    const translateServiceMock = {
        getCurrentLang: jest.fn(() => 'de'),
        use: jest.fn(),
        onLangChange: of({lang: 'en'}),
    };

    beforeEach(async () => {
        routerSpy = {navigate: jest.fn()} as unknown as jest.Mocked<Router>;
        notificationServiceSpy = {
            success: jest.fn(),
            error: jest.fn(),
        } as unknown as jest.Mocked<ObNotificationService>;
        worklistServiceSpy = {
            getWorklistTaskForUpdate: jest.fn().mockReturnValue(of()),
            update: jest.fn().mockReturnValue(of({})),
        } as unknown as jest.Mocked<WorklistService>;
        activatedRouteSpy = {
            snapshot: {
                params: {
                    id: 'test-id',
                },
            } as any,
        };

        await TestBed.configureTestingModule({
            imports: [WorklistTaskEditComponent, MockPipe(TranslatePipe), MockDirectives(ObUnsavedChangesDirective, ObMatErrorDirective)],
            providers: [
                {provide: Router, useValue: routerSpy},
                {provide: ObNotificationService, useValue: notificationServiceSpy},
                {provide: WorklistService, useValue: worklistServiceSpy},
                {provide: ActivatedRoute, useValue: activatedRouteSpy},
                {provide: TranslateService, useValue: translateServiceMock},
                provideNativeDateAdapter(),
                provideHttpClient(),
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(WorklistTaskEditComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('save', () => {
        it('should navigate to worklist and show success notification when form is valid', () => {
            component.form.controls.dueDate.setValue(new Date('2024-12-31'));
            component.save();
            expect(routerSpy.navigate).toHaveBeenCalledWith(['/worklist']);
            expect(notificationServiceSpy.success).toHaveBeenCalledWith('worklist.task.edit.success');
        });

        it('should mark form as touched when form is invalid', () => {
            component.form.controls.dueDate.setValue(undefined);
            component.save();
            expect(component.form.touched).toBeTruthy();
            expect(routerSpy.navigate).not.toHaveBeenCalled();
        });
    });

    it('should navigate to worklist when close is called', () => {
        component.close();
        expect(routerSpy.navigate).toHaveBeenCalledWith(['/worklist']);
    });
});
