import {provideHttpClient} from '@angular/common/http';
import {provideHttpClientTesting} from '@angular/common/http/testing';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatInput} from '@angular/material/input';
import {MatFormField, MatLabel} from '@angular/material/select';
import {Router} from '@angular/router';
import {TranslatePipe} from '@ngx-translate/core';
import {ObErrorMessagesModule, ObInputClearModule, ObUnsavedChangesDirective} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {MockModule, MockComponents, MockDirectives, MockPipe} from 'ng-mocks';
import {CommitteeTypeFormComponent} from './committee-type-form.component';

describe('CommitteeTypeFormComponent', () => {
    let component: CommitteeTypeFormComponent;
    let fixture: ComponentFixture<CommitteeTypeFormComponent>;

    const routerMock = {
        navigate: jest.fn(),
    };

    const errorServiceMock = {
        getControlError: jest.fn(),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                MockModule(ReactiveFormsModule),
                MockModule(ObErrorMessagesModule),
                MockModule(ObInputClearModule),
                MockComponents(MatFormField),
                MockDirectives(MatInput, MatLabel, ObUnsavedChangesDirective),
                MockPipe(TranslatePipe),
                CommitteeTypeFormComponent,
            ],
            providers: [
                {provide: Router, useValue: routerMock},
                {provide: ErrorService, useValue: errorServiceMock},
                provideHttpClient(),
                provideHttpClientTesting(),
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(CommitteeTypeFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('Form Initialization', () => {
        let form: FormGroup;

        beforeEach(() => {
            form = component.committeeTypeForm;
        });

        it('should disable percentage fields when minimal fields have values', () => {
            form.controls.germanMinimalThreshold.setValue(10);
            form.controls.frenchMinimalThreshold.setValue(10);
            form.controls.italianMinimalThreshold.setValue(10);
            form.controls.romanshMinimalThreshold.setValue(10);
            form.controls.germanThresholdPercentage.setValue(null);
            form.controls.frenchThresholdPercentage.setValue(null);
            form.controls.italianThresholdPercentage.setValue(null);
            form.controls.romanshThresholdPercentage.setValue(null);

            expect(form.controls.germanThresholdPercentage.disabled).toBe(true);
            expect(form.controls.frenchThresholdPercentage.disabled).toBe(true);
            expect(form.controls.italianThresholdPercentage.disabled).toBe(true);
            expect(form.controls.romanshThresholdPercentage.disabled).toBe(true);
        });

        it('should disable minimal fields when percentage fields have values', () => {
            form.controls.germanThresholdPercentage.setValue(24);
            form.controls.frenchThresholdPercentage.setValue(12);
            form.controls.italianThresholdPercentage.setValue(6);
            form.controls.romanshThresholdPercentage.setValue(3);
            form.controls.germanMinimalThreshold.setValue(null);
            form.controls.frenchMinimalThreshold.setValue(null);
            form.controls.italianMinimalThreshold.setValue(null);
            form.controls.romanshMinimalThreshold.setValue(null);

            expect(form.controls.germanMinimalThreshold.disabled).toBe(true);
            expect(form.controls.frenchMinimalThreshold.disabled).toBe(true);
            expect(form.controls.italianMinimalThreshold.disabled).toBe(true);
            expect(form.controls.romanshMinimalThreshold.disabled).toBe(true);
        });
    });
});
