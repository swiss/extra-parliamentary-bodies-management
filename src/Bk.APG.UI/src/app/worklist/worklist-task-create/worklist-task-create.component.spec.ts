/* eslint-disable @typescript-eslint/no-explicit-any */
import {provideHttpClient} from '@angular/common/http';
import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {FormBuilder} from '@angular/forms';
import {MatDialog, MatDialogRef} from '@angular/material/dialog';
import {Router} from '@angular/router';
import {GeneralElectionCommitteeList} from '@api/GeneralElectionCommitteeList';
import {WorklistTaskCreate} from '@api/WorklistTaskCreate';
import {LangChangeEvent, TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObHttpApiInterceptorEvents, ObNotificationService, ObUnsavedChangesDirective} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {MasterDataService} from '@shared/master-data.service';
import {EiamAssignmentService} from '@shared/services/eiam-assignment.service';
import {MockDirective, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, Subject, throwError} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {ConfigsService} from '../../configs.service';
import {GeneralElectionCommitteesService} from '../../general-election/ge-committees/ge-committees.service';
import {GeneralElectionService} from '../../general-election/general-election.service';
import {WorklistService} from '../worklist.service';
import {WorklistTaskCreateComponent} from './worklist-task-create.component';

describe('WorklistTaskCreateComponent', () => {
    let component: WorklistTaskCreateComponent;
    let fixture: ComponentFixture<WorklistTaskCreateComponent>;
    let mockMasterDataService: Partial<MasterDataService>;
    let mockWorklistService: Partial<WorklistService>;
    let mockNotificationService: Partial<ObNotificationService>;
    let mockRouter: Partial<Router>;
    let mockEiamAssignmentService: Partial<EiamAssignmentService>;
    let mockGeneralElectionCommitteesService: Partial<GeneralElectionCommitteesService>;
    let mockHttpApiInterceptorEvents: Partial<ObHttpApiInterceptorEvents>;
    let mockAuthService: Partial<AuthService>;
    let mockDialog: Partial<MatDialog>;

    beforeEach(async () => {
        const langChangeSubject = new Subject<LangChangeEvent>();

        const translateServiceMock = {
            getCurrentLang: jest.fn(() => 'en'),
            onLangChange: langChangeSubject,
            get: jest.fn(),
            instant: jest.fn((key: string) => key),
        };

        mockAuthService = {
            isAdmin$: new BehaviorSubject(true),
        };

        mockMasterDataService = {
            worklistTaskTypes: signal([]),
        };

        mockWorklistService = {
            create: jest.fn(),
        };

        mockNotificationService = {
            success: jest.fn(),
            error: jest.fn(),
        };

        mockHttpApiInterceptorEvents = {
            deactivateNotificationOnNextAPICalls: jest.fn(),
        };

        mockRouter = {
            navigate: jest.fn().mockResolvedValue(true),
        };

        mockEiamAssignmentService = {
            getAvailableEiamAssignments: jest.fn().mockReturnValue(of([])),
        };

        const dialogAfterClosedSubject = new Subject<boolean>();
        mockDialog = {
            open: jest.fn().mockReturnValue({
                afterClosed: jest.fn().mockReturnValue(dialogAfterClosedSubject.asObservable()),
            } as Partial<MatDialogRef<ConfirmDialogComponent>>),
        };

        const committeeList: GeneralElectionCommitteeList[] = [
            {id: 'id1', description: 'desc1'} as GeneralElectionCommitteeList,
            {id: 'id2', description: 'desc2'} as GeneralElectionCommitteeList,
        ];

        mockGeneralElectionCommitteesService = {
            getUnfinishedGeneralElectionCommitteeList: jest.fn().mockReturnValue(of(committeeList)),
        };

        const configsServiceMock = {
            frontendConfig: {
                entityIds: {
                    worklistTaskType: {generalElectionStartId: '', generalElectionEndId: ''},
                },
            },
        } as Partial<ConfigsService>;

        await TestBed.configureTestingModule({
            imports: [WorklistTaskCreateComponent, MockPipe(TranslatePipe), MockDirective(ObUnsavedChangesDirective)],
            providers: [
                FormBuilder,
                {provide: MasterDataService, useValue: mockMasterDataService},
                {provide: WorklistService, useValue: mockWorklistService},
                {provide: ObNotificationService, useValue: mockNotificationService},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: GeneralElectionService, useValue: {}},
                {provide: ConfigsService, useValue: configsServiceMock},
                {provide: Router, useValue: mockRouter},
                {provide: EiamAssignmentService, useValue: mockEiamAssignmentService},
                {provide: AuthService, useValue: mockAuthService},
                {provide: GeneralElectionCommitteesService, useValue: mockGeneralElectionCommitteesService},
                {provide: ObHttpApiInterceptorEvents, useValue: mockHttpApiInterceptorEvents},
                {provide: MatDialog, useValue: mockDialog},
                provideHttpClient(),
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(WorklistTaskCreateComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('ngOnInit', () => {
        it('should get availableEiamAssignments', () => {
            expect(mockEiamAssignmentService.getAvailableEiamAssignments).toHaveBeenCalled();
        });
    });

    describe('form initialization', () => {
        it('should initialize form with correct structure', () => {
            expect(component.form).toBeDefined();
            expect(component.form.get('worklistTaskTypeId')).toBeDefined();
            expect(component.form.get('description')).toBeDefined();
            expect(component.form.get('dueDate')).toBeDefined();
            expect(component.form.get('assignedToId')).toBeDefined();
        });

        it('should set required validators on required fields', () => {
            const worklistTaskTypeIdControl = component.form.get('worklistTaskTypeId');
            const dueDateControl = component.form.get('dueDate');
            const assignedToIdControl = component.form.get('assignedToId');

            expect(worklistTaskTypeIdControl?.hasError('required')).toBe(true);
            expect(dueDateControl?.hasError('required')).toBe(true);
            expect(assignedToIdControl?.hasError('required')).toBe(true);
        });

        it('should not set required validator on description field', () => {
            const descriptionControl = component.form.get('description');

            expect(descriptionControl?.hasError('required')).toBe(false);
        });
    });

    describe('save', () => {
        it('should mark all form controls as touched and return early when form is invalid', () => {
            const markAllAsTouchedSpy = jest.spyOn(component.form, 'markAllAsTouched');

            component.save();

            expect(markAllAsTouchedSpy).toHaveBeenCalled();
            expect(mockWorklistService.create).not.toHaveBeenCalled();
        });

        it('should call worklistService.create with form values when form is valid', () => {
            const formValue: WorklistTaskCreate = {
                worklistTaskTypeId: 'type1',
                description: 'Test description',
                dueDate: new Date(),
                assignedToId: 'role1',
            };

            component.form.patchValue(formValue);
            (mockWorklistService.create as any).mockReturnValue(of({}));

            component.save();

            expect(mockWorklistService.create).toHaveBeenCalledWith(formValue);
        });

        it('should mark form as pristine, navigate to worklist, and show success notification on successful save', () => {
            const formValue: WorklistTaskCreate = {
                worklistTaskTypeId: 'type1',
                description: 'Test description',
                dueDate: new Date(),
                assignedToId: 'role1',
            };

            component.form.patchValue(formValue);
            (mockWorklistService.create as any).mockReturnValue(of({}));
            const markAsPristineSpy = jest.spyOn(component.form, 'markAsPristine');

            component.save();

            expect(markAsPristineSpy).toHaveBeenCalled();
            expect(mockRouter.navigate).toHaveBeenCalledWith(['/worklist']);
            expect(mockNotificationService.success).toHaveBeenCalledWith('worklist.task.create.success');
        });

        it('should show error notification when save fails', () => {
            const formValue: WorklistTaskCreate = {
                worklistTaskTypeId: 'type1',
                description: 'Test description',
                dueDate: new Date(),
                assignedToId: 'role1',
            };

            component.form.patchValue(formValue);
            (mockWorklistService.create as any).mockReturnValue(throwError(() => new Error('Save failed')));

            component.save();

            expect(mockNotificationService.error).toHaveBeenCalledWith('worklist.task.create.error');
        });
    });

    describe('close', () => {
        it('should navigate to worklist', () => {
            component.close();

            expect(mockRouter.navigate).toHaveBeenCalledWith(['/worklist']);
        });
    });
});
