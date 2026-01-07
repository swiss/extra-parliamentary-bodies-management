/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable dot-notation */
import {provideHttpClient} from '@angular/common/http';
import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatCard, MatCardContent} from '@angular/material/card';
import {MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle} from '@angular/material/expansion';
import {Department} from '@api/Department';
import {GeneralMeasure} from '@api/GeneralMeasure';
import {GeneralMeasureUpdate} from '@api/GeneralMeasureUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObErrorMessagesDirective, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {MasterDataService} from '@shared/master-data.service';
import {RichTextEditorComponent} from '@shared/rich-text-editor/rich-text-editor.component';
import {MockComponent, MockComponents, MockDirectives, MockPipe} from 'ng-mocks';
import {of, throwError} from 'rxjs';
import {GeneralMeasuresComponent} from './general-measures.component';
import {GeneralMeasuresService} from './general-measures.service';

describe('GeneralMeasuresComponent', () => {
    let component: GeneralMeasuresComponent;
    let fixture: ComponentFixture<GeneralMeasuresComponent>;
    let mockMasterDataService: Partial<MasterDataService>;
    let mockGeneralMeasuresService: Partial<GeneralMeasuresService>;
    let mockNotificationService: Partial<ObNotificationService>;
    let mockHttpApiInterceptorEvents: Partial<ObHttpApiInterceptorEvents>;

    const mockDepartments: Department[] = [
        {
            id: 'dept-1',
            text: 'Department 1',
            description: 'Department 1 Description',
            uri: 'http://dept1.com',
            externalId: 'ext-1',
        },
        {
            id: 'dept-2',
            text: 'Department 2',
            description: 'Department 2 Description',
            uri: 'http://dept2.com',
            externalId: 'ext-2',
        },
    ];

    const mockGeneralMeasures: GeneralMeasure[] = [
        {
            departmentId: 'dept-1',
            department: 'Department 1',
            justificationLanguages: '<p>Language justification 1</p>',
            justificationGenders: '<p>Gender justification 1</p>',
        },
        {
            departmentId: 'dept-2',
            department: 'Department 2',
            justificationLanguages: '<p>Language justification 2</p>',
            justificationGenders: '<p>Gender justification 2</p>',
        },
    ];

    beforeEach(async () => {
        mockMasterDataService = {
            departments: signal<Department[]>([]),
        };

        mockGeneralMeasuresService = {
            getGeneralMeasures: jest.fn().mockReturnValue(of(mockGeneralMeasures)),
            saveGeneralMeasure: jest.fn().mockReturnValue(of({})),
        };

        mockNotificationService = {
            success: jest.fn(),
            error: jest.fn(),
        };

        mockHttpApiInterceptorEvents = {
            deactivateNotificationOnNextAPICalls: jest.fn(),
        };

        await TestBed.configureTestingModule({
            imports: [
                GeneralMeasuresComponent,
                ReactiveFormsModule,
                MockComponents(MatButton, MatCard, MatExpansionPanel, MatExpansionPanelHeader),
                MockDirectives(MatCardContent, MatExpansionPanelTitle, ObButtonDirective, ObErrorMessagesDirective),
                MockComponent(RichTextEditorComponent),
                MockPipe(TranslatePipe),
            ],
            providers: [
                {provide: MasterDataService, useValue: mockMasterDataService},
                {provide: GeneralMeasuresService, useValue: mockGeneralMeasuresService},
                {provide: ObNotificationService, useValue: mockNotificationService},
                {provide: ObHttpApiInterceptorEvents, useValue: mockHttpApiInterceptorEvents},
                provideHttpClient(),
            ],
        })
            .overrideTemplateUsingTestingModule(GeneralMeasuresComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(GeneralMeasuresComponent);
        component = fixture.componentInstance;
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('initialization', () => {
        it('should not create form groups when departments are empty', () => {
            fixture.detectChanges();

            expect(Object.keys(component['form'].controls)).toHaveLength(0);
            expect(mockGeneralMeasuresService.getGeneralMeasures).not.toHaveBeenCalled();
        });

        it('should create form groups for each department', () => {
            mockMasterDataService.departments!.set(mockDepartments);
            fixture.detectChanges();

            expect(Object.keys(component['form'].controls)).toHaveLength(2);
            expect(component['form'].get('dept-1')).toBeDefined();
            expect(component['form'].get('dept-2')).toBeDefined();
        });

        it('should create form controls for justificationLanguages and justificationGenders', () => {
            mockMasterDataService.departments!.set(mockDepartments);
            fixture.detectChanges();

            const dept1Group = component['getGeneralMeasureFormGroup']('dept-1');
            expect(dept1Group.get('justificationLanguages')).toBeDefined();
            expect(dept1Group.get('justificationGenders')).toBeDefined();
        });

        it('should fetch general measures when departments are available', () => {
            mockMasterDataService.departments!.set(mockDepartments);
            fixture.detectChanges();

            expect(mockGeneralMeasuresService.getGeneralMeasures).toHaveBeenCalled();
        });

        it('should populate form with fetched general measures', () => {
            mockMasterDataService.departments!.set(mockDepartments);
            fixture.detectChanges();

            const dept1Group = component['getGeneralMeasureFormGroup']('dept-1');
            expect(dept1Group.get('justificationLanguages')?.value).toBe('<p>Language justification 1</p>');
            expect(dept1Group.get('justificationGenders')?.value).toBe('<p>Gender justification 1</p>');

            const dept2Group = component['getGeneralMeasureFormGroup']('dept-2');
            expect(dept2Group.get('justificationLanguages')?.value).toBe('<p>Language justification 2</p>');
            expect(dept2Group.get('justificationGenders')?.value).toBe('<p>Gender justification 2</p>');
        });

        it('should set general measures signal with fetched data', () => {
            mockMasterDataService.departments!.set(mockDepartments);
            fixture.detectChanges();

            expect(component['generalMeasures']()).toEqual(mockGeneralMeasures);
        });
    });

    describe('saveGeneralMeasure', () => {
        beforeEach(() => {
            mockMasterDataService.departments!.set(mockDepartments);
            fixture.detectChanges();
        });

        it('should return early if form group does not exist', () => {
            component['saveGeneralMeasure']('non-existent-dept');

            expect(mockHttpApiInterceptorEvents.deactivateNotificationOnNextAPICalls).not.toHaveBeenCalled();
            expect(mockGeneralMeasuresService.saveGeneralMeasure).not.toHaveBeenCalled();
        });

        it('should mark form as touched and return early when form is invalid', () => {
            const dept1Group = component['getGeneralMeasureFormGroup']('dept-1');
            jest.spyOn(dept1Group, 'markAsTouched');
            Object.defineProperty(dept1Group, 'valid', {value: false, writable: true});

            component['saveGeneralMeasure']('dept-1');

            expect(dept1Group.markAsTouched).toHaveBeenCalled();
            expect(mockGeneralMeasuresService.saveGeneralMeasure).not.toHaveBeenCalled();
        });

        it('should deactivate notification on next API calls before saving', () => {
            const dept1Group = component['getGeneralMeasureFormGroup']('dept-1');
            dept1Group.patchValue({
                justificationLanguages: '<p>Updated language</p>',
                justificationGenders: '<p>Updated gender</p>',
            });

            component['saveGeneralMeasure']('dept-1');

            expect(mockHttpApiInterceptorEvents.deactivateNotificationOnNextAPICalls).toHaveBeenCalled();
        });

        it('should call saveGeneralMeasure with correct data when form is valid', () => {
            const dept1Group = component['getGeneralMeasureFormGroup']('dept-1');
            dept1Group.patchValue({
                justificationLanguages: '<p>Updated language</p>',
                justificationGenders: '<p>Updated gender</p>',
            });

            const expectedUpdate: GeneralMeasureUpdate = {
                departmentId: 'dept-1',
                justificationLanguages: '<p>Updated language</p>',
                justificationGenders: '<p>Updated gender</p>',
            };

            component['saveGeneralMeasure']('dept-1');

            expect(mockGeneralMeasuresService.saveGeneralMeasure).toHaveBeenCalledWith(expectedUpdate);
        });

        it('should mark form as pristine and untouched on successful save', () => {
            const dept1Group = component['getGeneralMeasureFormGroup']('dept-1');
            const markAsPristineSpy = jest.spyOn(dept1Group, 'markAsPristine');
            const markAsUntouchedSpy = jest.spyOn(dept1Group, 'markAsUntouched');

            component['saveGeneralMeasure']('dept-1');

            expect(markAsPristineSpy).toHaveBeenCalled();
            expect(markAsUntouchedSpy).toHaveBeenCalled();
        });

        it('should show success notification on successful save', () => {
            component['saveGeneralMeasure']('dept-1');

            expect(mockNotificationService.success).toHaveBeenCalledWith('generalMeasures.save.success');
        });

        it('should show error notification when save fails', () => {
            (mockGeneralMeasuresService.saveGeneralMeasure as any).mockReturnValue(throwError(() => new Error('Save failed')));

            component['saveGeneralMeasure']('dept-1');

            expect(mockNotificationService.error).toHaveBeenCalledWith('generalMeasures.save.error');
        });

        it('should not show success notification when save fails', () => {
            (mockGeneralMeasuresService.saveGeneralMeasure as any).mockReturnValue(throwError(() => new Error('Save failed')));

            component['saveGeneralMeasure']('dept-1');

            expect(mockNotificationService.success).not.toHaveBeenCalled();
        });
    });

    describe('resetGeneralMeasure', () => {
        beforeEach(() => {
            mockMasterDataService.departments!.set(mockDepartments);
            fixture.detectChanges();
        });

        it('should return early if form group does not exist', () => {
            const resetSpy = jest.spyOn(component['form'], 'reset');

            component['resetGeneralMeasure']('non-existent-dept');

            expect(resetSpy).not.toHaveBeenCalled();
        });

        it('should reset form to original values', () => {
            const dept1Group = component['getGeneralMeasureFormGroup']('dept-1');
            dept1Group.patchValue({
                justificationLanguages: '<p>Modified language</p>',
                justificationGenders: '<p>Modified gender</p>',
            });

            component['resetGeneralMeasure']('dept-1');

            expect(dept1Group.get('justificationLanguages')?.value).toBe('<p>Language justification 1</p>');
            expect(dept1Group.get('justificationGenders')?.value).toBe('<p>Gender justification 1</p>');
        });

        it('should reset to null values when no original measure exists', () => {
            const newDepartment: Department = {
                id: 'dept-3',
                text: 'Department 3',
                description: 'Department 3 Description',
                uri: 'http://dept3.com',
                externalId: 'ext-3',
            };

            mockMasterDataService.departments!.set([...mockDepartments, newDepartment]);
            fixture.detectChanges();

            const dept3Group = component['getGeneralMeasureFormGroup']('dept-3');
            dept3Group.patchValue({
                justificationLanguages: '<p>Some language</p>',
                justificationGenders: '<p>Some gender</p>',
            });

            component['resetGeneralMeasure']('dept-3');

            expect(dept3Group.get('justificationLanguages')?.value).toBeNull();
            expect(dept3Group.get('justificationGenders')?.value).toBeNull();
        });
    });

    describe('getGeneralMeasureFormGroup', () => {
        beforeEach(() => {
            mockMasterDataService.departments!.set(mockDepartments);
            fixture.detectChanges();
        });

        it('should return the correct form group for a given department ID', () => {
            const dept1Group = component['getGeneralMeasureFormGroup']('dept-1');

            expect(dept1Group).toBeDefined();
            expect(dept1Group.get('justificationLanguages')).toBeDefined();
            expect(dept1Group.get('justificationGenders')).toBeDefined();
        });

        it('should return undefined for non-existent department ID', () => {
            const nonExistentGroup = component['getGeneralMeasureFormGroup']('non-existent');

            expect(nonExistentGroup).toBeNull();
        });
    });

    describe('form value extraction', () => {
        beforeEach(() => {
            mockMasterDataService.departments!.set(mockDepartments);
            fixture.detectChanges();
        });

        it('should extract form value with null values when controls are empty', () => {
            const dept1Group = component['getGeneralMeasureFormGroup']('dept-1');
            dept1Group.patchValue({
                justificationLanguages: undefined,
                justificationGenders: undefined,
            });

            component['saveGeneralMeasure']('dept-1');

            const expectedUpdate: GeneralMeasureUpdate = {
                departmentId: 'dept-1',
                justificationLanguages: undefined,
                justificationGenders: undefined,
            };

            expect(mockGeneralMeasuresService.saveGeneralMeasure).toHaveBeenCalledWith(expectedUpdate);
        });

        it('should extract form value correctly for different departments', () => {
            const dept2Group = component['getGeneralMeasureFormGroup']('dept-2');
            dept2Group.patchValue({
                justificationLanguages: '<p>New language for dept 2</p>',
                justificationGenders: '<p>New gender for dept 2</p>',
            });

            component['saveGeneralMeasure']('dept-2');

            const expectedUpdate: GeneralMeasureUpdate = {
                departmentId: 'dept-2',
                justificationLanguages: '<p>New language for dept 2</p>',
                justificationGenders: '<p>New gender for dept 2</p>',
            };

            expect(mockGeneralMeasuresService.saveGeneralMeasure).toHaveBeenCalledWith(expectedUpdate);
        });
    });
});
