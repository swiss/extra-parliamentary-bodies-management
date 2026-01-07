import {ComponentFixture, TestBed} from '@angular/core/testing';
import {FormBuilder} from '@angular/forms';
import {provideNativeDateAdapter} from '@angular/material/core';
import {Router} from '@angular/router';
import {TranslatePipe} from '@ngx-translate/core';
import {ObAlertComponent, ObMatErrorDirective, ObNotificationService, ObUnsavedChangesDirective} from '@oblique/oblique';
import {MockComponents, MockDirectives, MockPipes} from 'ng-mocks';
import {of, throwError} from 'rxjs';
import {WorklistService} from '../worklist.service';
import {WorklistTaskForwardComponent} from './worklist-task-forward.component';

describe('WorklistTaskForwardComponent', () => {
    let component: WorklistTaskForwardComponent;
    let fixture: ComponentFixture<WorklistTaskForwardComponent>;
    let worklistService: Partial<WorklistService>;
    let notificationService: Partial<ObNotificationService>;
    let router: Partial<Router>;

    beforeEach(async () => {
        const worklistServiceMock = {
            forward: jest.fn(),
        } as Partial<WorklistService>;

        const notificationServiceMock = {
            success: jest.fn(),
            error: jest.fn(),
        } as Partial<ObNotificationService>;

        const routerMock = {
            navigate: jest.fn().mockResolvedValue(true),
        } as Partial<Router>;

        await TestBed.configureTestingModule({
            imports: [
                WorklistTaskForwardComponent,
                MockDirectives(ObUnsavedChangesDirective, ObMatErrorDirective),
                MockPipes(TranslatePipe),
                MockComponents(ObAlertComponent),
            ],
            providers: [
                FormBuilder,
                {provide: WorklistService, useValue: worklistServiceMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: Router, useValue: routerMock},
                provideNativeDateAdapter(),
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(WorklistTaskForwardComponent);
        component = fixture.componentInstance;

        worklistService = TestBed.inject(WorklistService);
        notificationService = TestBed.inject(ObNotificationService);
        router = TestBed.inject(Router);

        fixture.componentRef.setInput('worklistTaskId', 'test-task-id');
        fixture.componentRef.setInput('isBigDepartment', true);

        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('forward', () => {
        it('should call markAllAsTouched and return early when form is invalid', () => {
            const markAllAsTouchedSpy = jest.spyOn(component.form, 'markAllAsTouched');

            component.forward();

            expect(markAllAsTouchedSpy).toHaveBeenCalled();
            expect(worklistService.forward).not.toHaveBeenCalled();
        });

        it('should forward task successfully when form is valid', () => {
            const formValue = {
                candidateListDueDate: new Date(),
                candidateListDescription: 'Test description',
                committeeDueDate: new Date(),
                committeeDescription: 'Committee description',
            };

            component.form.patchValue(formValue);
            (worklistService.forward as jest.Mock).mockReturnValue(of({}));
            const markAsPristineSpy = jest.spyOn(component.form, 'markAsPristine');

            component.forward();

            expect(worklistService.forward).toHaveBeenCalledWith('test-task-id', formValue);
            expect(markAsPristineSpy).toHaveBeenCalled();
            expect(router.navigate).toHaveBeenCalledWith(['/worklist']);
            expect(notificationService.success).toHaveBeenCalledWith('worklist.task.forward.success');
        });

        it('should show error notification when forward fails', () => {
            const formValue = {
                candidateListDueDate: new Date(),
                candidateListDescription: 'Test description',
                committeeDueDate: new Date(),
                committeeDescription: 'Committee description',
            };

            component.form.patchValue(formValue);
            (worklistService.forward as jest.Mock).mockReturnValue(throwError(() => new Error('Service error')));

            component.forward();

            expect(notificationService.error).toHaveBeenCalledWith('worklist.task.forward.error');
            expect(router.navigate).not.toHaveBeenCalled();
        });
    });
});
