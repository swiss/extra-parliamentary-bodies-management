/* eslint-disable @typescript-eslint/no-explicit-any */
import {provideHttpClient} from '@angular/common/http';
import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {FormBuilder, ReactiveFormsModule} from '@angular/forms';
import {MatIconModule} from '@angular/material/icon';
import {ActivatedRoute} from '@angular/router';
import {DuplicateReason} from '@api/DuplicateReason';
import {GeneralElectionCommitteeDetails} from '@api/GeneralElectionCommitteeDetails';
import {MembershipCandidateDetail} from '@api/MembershipCandidateDetail';
import {PersonDetails} from '@api/PersonDetails';
import {ObHttpApiInterceptorEvents, ObNotificationService, WINDOW} from '@oblique/oblique';
import * as fileUtil from '@shared/file-util';
import {DOCUMENT} from '@shared/injection.tokens';
import {MasterDataService} from '@shared/master-data.service';
import {MockModule} from 'ng-mocks';
import {of, Subject, throwError} from 'rxjs';
import {AuthService} from '../../../../auth/auth.service';
import {GeneralElectionCommitteesService} from '../../ge-committees.service';
import {GeneralElectionCommitteeDetailsService} from '../ge-committee-details.service';
import {GeneralElectionCommitteeCandidateListComponent} from './ge-committee-candidate-list.component';
import {GeneralElectionCommitteeCandidateListService} from './ge-committee-candidate-list.service';

describe('GeneralElectionCommitteeCandidateListComponent', () => {
    let component: GeneralElectionCommitteeCandidateListComponent;
    let fixture: ComponentFixture<GeneralElectionCommitteeCandidateListComponent>;

    let isAdminSubject: Subject<boolean>;
    let isSecretariatSubject: Subject<boolean>;
    let isDepartmentSubject: Subject<boolean>;
    let isOfficeSubject: Subject<boolean>;
    let isObserverSubject: Subject<boolean>;
    let generalElectionCommitteeDetailsServiceMock: {
        committeeDetails: ReturnType<typeof signal<GeneralElectionCommitteeDetails>>;
        reload$: Subject<void>;
    };

    const mockMembershipCandidates: MembershipCandidateDetail[] = [
        {
            id: 'candidate-1',
            surname: 'Doe',
            givenName: 'John',
            gender: 'Male',
            genderId: 'maleId',
            language: 'English',
            birthYear: 1985,
            function: 'President',
            functionId: 'func-1',
            beginDate: new Date('2023-01-01'),
            endDate: new Date('2024-12-31'),
            electionType: 'Direct',
            electionTypeId: 'Type-1',
            estimatedTermOfOffice: 1,
            membershipAddition: 'Full Member',
            remarks: 'Active member',
            remarksStatus: 'Confirmed',
            isSelected: false,
        },
        {
            id: 'candidate-2',
            surname: 'Smith',
            givenName: 'Jane',
            gender: 'Female',
            genderId: 'femaleId',
            language: 'French',
            birthYear: 1990,
            function: 'Secretary',
            functionId: 'func-2',
            beginDate: new Date('2023-06-01'),
            endDate: undefined,
            electionType: 'Indirect',
            electionTypeId: 'Type-2',
            estimatedTermOfOffice: 1,
            membershipAddition: 'Associate Member',
            remarks: 'New member',
            remarksStatus: 'Pending',
            isSelected: false,
        },
    ];

    const membershipCandidateListServiceMock = {
        getMembershipCandidates: jest.fn().mockReturnValue(of(mockMembershipCandidates)),
        partialUpdateMembershipCandidate: jest.fn().mockReturnValue(of(undefined)),
        deleteMembershipCandidate: jest.fn().mockReturnValue(of(undefined)),
        generateExport: jest.fn().mockReturnValue(of(new Blob())),
    } as any;

    const httpApiInterceptorEventsMock = {
        deactivateNotificationOnNextAPICalls: jest.fn(),
        deactivateSpinnerOnNextAPICalls: jest.fn(),
    };

    let authServiceMock: {
        isAdmin$: Subject<boolean>;
        isSecretariatUser$: Subject<boolean>;
        isDepartmentUser$: Subject<boolean>;
        isOfficeUser$: Subject<boolean>;
        isObserver$: Subject<boolean>;
    };

    const notificationServiceMock = {
        success: jest.fn(),
        info: jest.fn(),
        error: jest.fn(),
    };

    const masterDataServiceMock = {
        functions: jest.fn().mockReturnValue([
            {id: 'func-1', text: 'President'},
            {id: 'func-2', text: 'Secretary'},
        ]),
    };

    const activatedRouteMock = {
        snapshot: {
            params: {id: 'committee-123'},
        },
    };

    beforeEach(async () => {
        isAdminSubject = new Subject<boolean>();
        isSecretariatSubject = new Subject<boolean>();
        isDepartmentSubject = new Subject<boolean>();
        isOfficeSubject = new Subject<boolean>();
        isObserverSubject = new Subject<boolean>();

        authServiceMock = {
            isAdmin$: isAdminSubject,
            isSecretariatUser$: isSecretariatSubject,
            isDepartmentUser$: isDepartmentSubject,
            isOfficeUser$: isOfficeSubject,
            isObserver$: isObserverSubject,
        };

        generalElectionCommitteeDetailsServiceMock = {
            committeeDetails: signal<GeneralElectionCommitteeDetails>({canEdit: true} as GeneralElectionCommitteeDetails),
            reload$: new Subject<void>(),
        };

        const generalElectionCommitteeServiceMock = {
            updateGeneralElectionCommitteeVacancies: jest.fn(),
        };

        await TestBed.configureTestingModule({
            imports: [GeneralElectionCommitteeCandidateListComponent, MockModule(ReactiveFormsModule), MockModule(MatIconModule)],
            providers: [
                FormBuilder,
                {provide: GeneralElectionCommitteeCandidateListService, useValue: membershipCandidateListServiceMock},
                {provide: AuthService, useValue: authServiceMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: GeneralElectionCommitteeDetailsService, useValue: generalElectionCommitteeDetailsServiceMock},
                {provide: GeneralElectionCommitteesService, useValue: generalElectionCommitteeServiceMock},
                {provide: ObHttpApiInterceptorEvents, useValue: httpApiInterceptorEventsMock},
                {provide: WINDOW, useValue: window},
                {provide: DOCUMENT, useValue: document},
                provideHttpClient(),
            ],
        })
            .overrideTemplate(GeneralElectionCommitteeCandidateListComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(GeneralElectionCommitteeCandidateListComponent);
        component = fixture.componentInstance;
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should have correct page size options', () => {
        expect(component.pageSizeOptions).toEqual([25, 50, 100]);
    });

    it('should initialize membership candidate forms map', () => {
        expect(component.membershipCandidateForms).toBeInstanceOf(Map);
    });

    it('should get membership candidate form by ID', () => {
        const form = new FormBuilder().group({
            functionId: ['func-1'],
            remarks: ['Test remarks'],
            remarksStatus: ['Test status'],
        });
        component.membershipCandidateForms.set('candidate-1', form);

        const retrievedForm = component.getMembershipCandidateForm('candidate-1');
        expect(retrievedForm).toBeDefined();
        expect(retrievedForm.get('functionId')?.value).toBe('func-1');
    });

    it('should detect unsaved form changes', () => {
        const form = new FormBuilder().group({
            functionId: ['func-1'],
            remarks: ['Test remarks'],
            remarksStatus: ['Test status'],
        });
        component.membershipCandidateForms.set('candidate-1', form);

        expect(component.hasUnsavedFormChanges('candidate-1')).toBe(false);

        const remarksControl = form.get('remarks');
        remarksControl?.setValue('Updated remarks');
        remarksControl?.markAsDirty();

        expect(component.hasUnsavedFormChanges('candidate-1')).toBe(true);
    });

    it('should return false for non-existent membership candidate forms', () => {
        expect(component.hasUnsavedFormChanges('non-existent')).toBe(false);
    });

    it('should not save membership candidate when form has no changes', () => {
        const form = new FormBuilder().group({
            functionId: ['func-1'],
            remarks: ['Test remarks'],
            remarksStatus: ['Test status'],
        });
        component.membershipCandidateForms.set('candidate-1', form);

        component.saveMembershipCandidate(mockMembershipCandidates[0]);

        expect(membershipCandidateListServiceMock.partialUpdateMembershipCandidate).not.toHaveBeenCalled();
    });

    it('should save membership candidate when form has changes', () => {
        const form = new FormBuilder().group({
            functionId: ['func-1'],
            remarks: ['Test remarks'],
            remarksStatus: ['Test status'],
        });
        form.markAsDirty();
        component.membershipCandidateForms.set('candidate-1', form);

        component.saveMembershipCandidate(mockMembershipCandidates[0]);

        expect(membershipCandidateListServiceMock.partialUpdateMembershipCandidate).toHaveBeenCalledWith('candidate-1', {
            functionId: 'func-1',
            remarks: 'Test remarks',
            remarksStatus: 'Test status',
        });
    });

    it('should handle save errors', () => {
        membershipCandidateListServiceMock.partialUpdateMembershipCandidate.mockReturnValue(throwError(() => new Error('Save failed')));

        const form = new FormBuilder().group({
            functionId: ['func-1'],
            remarks: ['Test remarks'],
            remarksStatus: ['Test status'],
        });
        form.markAsDirty();
        component.membershipCandidateForms.set('candidate-1', form);

        component.saveMembershipCandidate(mockMembershipCandidates[0]);

        expect(notificationServiceMock.error).toHaveBeenCalledWith('generalElection.membershipCandidate.saveChanges.error');
    });

    it('should set up sort and paginator after view init', () => {
        component.ngAfterViewInit();

        expect(component.dataSource.sort).toBeDefined();
        expect(component.dataSource.paginator).toBeDefined();
    });

    describe('Authorization tests', () => {
        beforeEach(() => {
            isAdminSubject.next(false);
            isSecretariatSubject.next(false);
            isDepartmentSubject.next(false);
            isObserverSubject.next(false);
            isOfficeSubject.next(false);
            fixture.detectChanges();
        });

        it('should allow editing for admin users', () => {
            isAdminSubject.next(true);
            fixture.detectChanges();

            expect((component as any).isAllowed()).toBe(true);
        });

        it('should allow editing for secretariat users', () => {
            isSecretariatSubject.next(true);
            fixture.detectChanges();

            expect((component as any).isAllowed()).toBe(true);
        });

        it('should allow editing for department users', () => {
            isDepartmentSubject.next(true);
            fixture.detectChanges();

            expect((component as any).isAllowed()).toBe(true);
        });

        it('should allow editing for office users', () => {
            isOfficeSubject.next(true);
            fixture.detectChanges();

            expect((component as any).isAllowed()).toBe(true);
        });

        it('should not allow editing for unauthorized users', () => {
            expect((component as any).isAllowed()).toBe(false);
        });

        it('should include actions column for authorized users', () => {
            isAdminSubject.next(true);
            fixture.detectChanges();

            expect((component as any).displayedColumnsWithPermissions()).toContain('actions');
        });

        it('should not include actions column for unauthorized users', () => {
            // All subjects already set to false in beforeEach
            expect((component as any).displayedColumnsWithPermissions()).not.toContain('actions');
        });

        it('should allow editing for multiple roles simultaneously', () => {
            isAdminSubject.next(true);
            isSecretariatSubject.next(true);
            fixture.detectChanges();

            expect((component as any).isAllowed()).toBe(true);
        });

        it('should disable forms for unauthorized users', () => {
            const form = new FormBuilder().group({
                functionId: ['func-1'],
                remarks: ['Test remarks'],
                remarksStatus: ['Test status'],
            });
            component.membershipCandidateForms.set('candidate-1', form);

            if (!(component as any).canEdit()) {
                form.disable();
            }

            expect(form.disabled).toBe(true);
        });

        it('should enable forms for authorized users', () => {
            isAdminSubject.next(true);
            fixture.detectChanges();

            const form = new FormBuilder().group({
                functionId: ['func-1'],
                remarks: ['Test remarks'],
                remarksStatus: ['Test status'],
            });
            component.membershipCandidateForms.set('candidate-1', form);

            if (!(component as any).isAllowed()) {
                form.disable();
            }

            expect(form.disabled).toBe(false);
        });

        it('should react to auth changes dynamically', () => {
            expect((component as any).isAllowed()).toBe(false);

            isAdminSubject.next(true);
            fixture.detectChanges();
            expect((component as any).isAllowed()).toBe(true);

            isAdminSubject.next(false);
            isSecretariatSubject.next(true);
            fixture.detectChanges();
            expect((component as any).isAllowed()).toBe(true);

            isSecretariatSubject.next(false);
            fixture.detectChanges();
            expect((component as any).isAllowed()).toBe(false);
        });
    });

    describe('Selection functionality tests', () => {
        beforeEach(() => {
            component.dataSource.data = mockMembershipCandidates;
            component.selectedIds = [];
            component.allSelected = false;
        });

        describe('isSelected', () => {
            it('should return true when ID is in selectedIds array', () => {
                component.selectedIds = ['candidate-1'];

                expect(component.isSelected('candidate-1')).toBe(true);
            });

            it('should return false when ID is not in selectedIds array', () => {
                component.selectedIds = ['candidate-1'];

                expect(component.isSelected('candidate-2')).toBe(false);
            });

            it('should return false when selectedIds is empty', () => {
                component.selectedIds = [];

                expect(component.isSelected('candidate-1')).toBe(false);
            });
        });

        describe('toggleSelection', () => {
            it('should add ID to selectedIds when not already selected', () => {
                component.selectedIds = [];

                component.toggleSelection('candidate-1');

                expect(component.selectedIds).toContain('candidate-1');
                expect(component.selectedIds.length).toBe(1);
            });

            it('should remove ID from selectedIds when already selected', () => {
                component.selectedIds = ['candidate-1', 'candidate-2'];

                component.toggleSelection('candidate-1');

                expect(component.selectedIds).not.toContain('candidate-1');
                expect(component.selectedIds).toContain('candidate-2');
                expect(component.selectedIds.length).toBe(1);
            });

            it('should update allSelected state when all items become selected', () => {
                component.selectedIds = ['candidate-1'];

                component.toggleSelection('candidate-2');

                expect(component.selectedIds.length).toBe(2);
                expect(component.allSelected).toBe(true);
            });

            it('should update allSelected state when not all items are selected', () => {
                component.selectedIds = ['candidate-1', 'candidate-2'];
                component.allSelected = true;

                component.toggleSelection('candidate-1');

                expect(component.allSelected).toBe(false);
            });
        });

        describe('toggleSelectAll', () => {
            it('should select all items when none are selected', () => {
                component.selectedIds = [];
                component.allSelected = false;

                component.toggleSelectAll();

                expect(component.selectedIds.length).toBe(mockMembershipCandidates.length);
                expect(component.selectedIds).toContain('candidate-1');
                expect(component.selectedIds).toContain('candidate-2');
                expect(component.allSelected).toBe(true);
            });

            it('should deselect all items when all are selected', () => {
                component.selectedIds = ['candidate-1', 'candidate-2'];
                component.allSelected = true;

                component.toggleSelectAll();

                expect(component.selectedIds.length).toBe(0);
                expect(component.allSelected).toBe(false);
            });

            it('should select all items when some are selected', () => {
                component.selectedIds = ['candidate-1'];
                component.allSelected = false;

                component.toggleSelectAll();

                expect(component.selectedIds.length).toBe(mockMembershipCandidates.length);
                expect(component.selectedIds).toContain('candidate-1');
                expect(component.selectedIds).toContain('candidate-2');
                expect(component.allSelected).toBe(true);
            });
        });

        describe('isPartiallySelected', () => {
            it('should return true when some but not all items are selected', () => {
                component.selectedIds = ['candidate-1'];

                expect(component.isPartiallySelected()).toBe(true);
            });

            it('should return false when no items are selected', () => {
                component.selectedIds = [];

                expect(component.isPartiallySelected()).toBe(false);
            });

            it('should return false when all items are selected', () => {
                component.selectedIds = ['candidate-1', 'candidate-2'];

                expect(component.isPartiallySelected()).toBe(false);
            });

            it('should return false when dataSource is empty', () => {
                component.dataSource.data = [];
                component.selectedIds = [];

                expect(component.isPartiallySelected()).toBe(false);
            });
        });

        describe('selectionChanged signal', () => {
            it('should be false initially', () => {
                expect(component.selectionChanged()).toBe(false);
            });

            it('should become true after toggleSelection', () => {
                component.toggleSelection('candidate-1');

                expect(component.selectionChanged()).toBe(true);
            });

            it('should become true after toggleSelectAll selects all', () => {
                component.toggleSelectAll();

                expect(component.selectionChanged()).toBe(true);
            });

            it('should become true after toggleSelectAll deselects all', () => {
                component.selectedIds = ['candidate-1', 'candidate-2'];
                component.allSelected = true;

                component.toggleSelectAll();

                expect(component.selectionChanged()).toBe(true);
            });

            it('should reset to false after saveSelectedIds succeeds', () => {
                const saveCandidateListMock = jest.fn().mockReturnValue(of(undefined));
                membershipCandidateListServiceMock.saveCandidateList = saveCandidateListMock;
                component.toggleSelection('candidate-1');
                expect(component.selectionChanged()).toBe(true);

                component.saveSelectedIds();

                expect(component.selectionChanged()).toBe(false);
            });

            it('should remain true after saveSelectedIds fails', () => {
                const saveCandidateListMock = jest.fn().mockReturnValue(throwError(() => new Error('fail')));
                membershipCandidateListServiceMock.saveCandidateList = saveCandidateListMock;
                component.toggleSelection('candidate-1');

                component.saveSelectedIds();

                expect(component.selectionChanged()).toBe(true);
            });
        });

        describe('deleteMembershipCandidate', () => {
            it('should delete membership candidate', () => {
                component.deleteMembershipCandidate('foo');

                expect(membershipCandidateListServiceMock.deleteMembershipCandidate).toHaveBeenCalledWith('foo');
                expect(notificationServiceMock.success).toHaveBeenCalledWith('generalElection.membershipCandidate.delete.success');
            });

            it('should handle delete errors', () => {
                membershipCandidateListServiceMock.deleteMembershipCandidate.mockReturnValue(throwError(() => new Error('Delete failed')));

                component.deleteMembershipCandidate('foo');

                expect(notificationServiceMock.error).toHaveBeenCalledWith('generalElection.membershipCandidate.delete.error');
            });
        });
    });

    describe('openForwardDialog', () => {
        it('should open forward dialog with correct data', () => {
            const dialogServiceMock = {
                open: jest.fn(),
            };
            (component as any).dialog = dialogServiceMock;
            component.selectedIds = ['candidate-1', 'candidate-2'];

            component.openForwardDialog();

            expect(dialogServiceMock.open).toHaveBeenCalledWith(
                expect.anything(),
                expect.objectContaining({
                    data: {
                        committeeId: 'committee-123',
                        candidateIds: ['candidate-1', 'candidate-2'],
                    },
                    viewContainerRef: expect.anything(),
                })
            );
        });

        it('should open dialog with empty candidate list', () => {
            const dialogServiceMock = {
                open: jest.fn(),
            };
            (component as any).dialog = dialogServiceMock;
            component.selectedIds = [];

            component.openForwardDialog();

            expect(dialogServiceMock.open).toHaveBeenCalledWith(
                expect.anything(),
                expect.objectContaining({
                    data: {
                        committeeId: 'committee-123',
                        candidateIds: [],
                    },
                    viewContainerRef: expect.anything(),
                })
            );
        });
    });

    describe('saveSelectedIds', () => {
        it('should save selected candidate list successfully', () => {
            const saveCandidateListMock = jest.fn().mockReturnValue(of(undefined));
            membershipCandidateListServiceMock.saveCandidateList = saveCandidateListMock;
            component.selectedIds = ['candidate-1', 'candidate-2'];

            const nextSpy = jest.spyOn(generalElectionCommitteeDetailsServiceMock.reload$, 'next');

            component.saveSelectedIds();

            expect(saveCandidateListMock).toHaveBeenCalledWith('committee-123', ['candidate-1', 'candidate-2']);
            expect(nextSpy).toHaveBeenCalled();
            expect(notificationServiceMock.success).toHaveBeenCalledWith('generalElection.candidateList.save.success');
        });

        it('should handle save candidate list errors', () => {
            const saveCandidateListMock = jest.fn().mockReturnValue(throwError(() => new Error('Save failed')));
            membershipCandidateListServiceMock.saveCandidateList = saveCandidateListMock;
            component.selectedIds = ['candidate-1'];

            component.saveSelectedIds();

            expect(saveCandidateListMock).toHaveBeenCalledWith('committee-123', ['candidate-1']);
            expect(notificationServiceMock.error).toHaveBeenCalledWith('generalElection.candidateList.save.error');
        });

        it('should save empty candidate list', () => {
            const saveCandidateListMock = jest.fn().mockReturnValue(of(undefined));
            membershipCandidateListServiceMock.saveCandidateList = saveCandidateListMock;
            component.selectedIds = [];

            component.saveSelectedIds();

            expect(saveCandidateListMock).toHaveBeenCalledWith('committee-123', []);
        });
    });

    describe('validateCandidateList', () => {
        it('should validate candidate list successfully', () => {
            const validateCandidateListMock = jest.fn().mockReturnValue(
                of({
                    isValid: true,
                    errors: [],
                    duplicateCheckResults: [],
                    createdPersons: [],
                    existingPersons: [],
                    areJustificationsMissing: false,
                    areContactPointsMissing: false,
                    isReadyForProposalActivated: false,
                    allValidationsPassed: false,
                    personsWithMissingInterests: [],
                    personsWithMissingBaseData: [],
                    personsWithMembershipValidationIssues: [],
                })
            );
            const reloadSubject = new Subject<void>();
            const routerMock = {navigate: jest.fn().mockResolvedValue(true)};
            membershipCandidateListServiceMock.validateCandidateList = validateCandidateListMock;
            (component as any).generalElectionCommitteeDetailsService.reload$ = reloadSubject;
            (component as any).router = routerMock;
            component.selectedIds = ['candidate-1', 'candidate-2'];

            const nextSpy = jest.spyOn(reloadSubject, 'next');

            component.validateCandidateList();

            expect(validateCandidateListMock).toHaveBeenCalledWith('committee-123', ['candidate-1', 'candidate-2'], false);
            expect(nextSpy).toHaveBeenCalled();
            expect(notificationServiceMock.success).toHaveBeenCalledWith({message: 'generalElection.candidateList.validate.success', timeout: 10000});
        });

        it('should handle validation errors when validation fails', () => {
            const validateCandidateListMock = jest.fn().mockReturnValue(
                of({
                    isValid: false,
                    errors: ['Error 1', 'Error 2'],
                    duplicateCheckResults: [],
                    createdPersons: [],
                    existingPersons: [],
                    areJustificationsMissing: true,
                    areContactPointsMissing: false,
                    isReadyForProposalActivated: false,
                    allValidationsPassed: false,
                    personsWithMissingInterests: [],
                    personsWithMissingBaseData: [],
                    personsWithMembershipValidationIssues: [],
                })
            );
            const routerMock = {navigate: jest.fn().mockResolvedValue(true)};
            membershipCandidateListServiceMock.validateCandidateList = validateCandidateListMock;
            (component as any).router = routerMock;
            component.selectedIds = ['candidate-1'];

            component.validateCandidateList();

            expect(validateCandidateListMock).toHaveBeenCalledWith('committee-123', ['candidate-1'], false);
            expect(component.validationResult()?.errors).toEqual(['Error 1', 'Error 2']);
            expect(component.validationResult()?.areJustificationsMissing).toEqual(true);
            expect(routerMock.navigate).not.toHaveBeenCalled();
            expect(notificationServiceMock.error).toHaveBeenCalledWith('generalElection.candidateList.validate.error');
            expect(notificationServiceMock.success).not.toHaveBeenCalled();
        });

        it('should handle person duplicates during validation', () => {
            const validateCandidateListMock = jest.fn().mockReturnValue(
                of({
                    isValid: true,
                    errors: [],
                    duplicateCheckResults: [
                        {membershipCandidateToCheck: mockMembershipCandidates[0], duplicateReason: DuplicateReason.FullMatch},
                        {membershipCandidateToCheck: mockMembershipCandidates[1], duplicateReason: DuplicateReason.NoDuplicateFound},
                    ],
                    createdPersons: [],
                    existingPersons: [],
                    areJustificationsMissing: false,
                    areContactPointsMissing: false,
                    isReadyForProposalActivated: false,
                    allValidationsPassed: false,
                    personsWithMissingInterests: [],
                    personsWithMissingBaseData: [],
                    personsWithMembershipValidationIssues: [],
                })
            );
            const routerMock = {navigate: jest.fn().mockResolvedValue(true)};
            membershipCandidateListServiceMock.validateCandidateList = validateCandidateListMock;
            (component as any).router = routerMock;
            component.selectedIds = ['candidate-1'];

            component.validateCandidateList();

            expect(validateCandidateListMock).toHaveBeenCalledWith('committee-123', ['candidate-1'], false);
            expect(component.personDuplicates().length).toBe(1);
            expect(routerMock.navigate).not.toHaveBeenCalled();
            expect(notificationServiceMock.success).not.toHaveBeenCalled();
        });

        it('should set existing/created persons during validation', () => {
            const validateCandidateListMock = jest.fn().mockReturnValue(
                of({
                    isValid: true,
                    errors: [],
                    duplicateCheckResults: [],
                    createdPersons: [{id: '1', surname: 'Clark', givenName: 'Jim', birthYear: 1936} as PersonDetails],
                    existingPersons: [{id: '2', surname: 'Regazzoni', givenName: 'Clay', birthYear: 1939} as PersonDetails],
                    areJustificationsMissing: false,
                    areContactPointsMissing: false,
                    isReadyForProposalActivated: false,
                    allValidationsPassed: false,
                    personsWithMissingInterests: [],
                    personsWithMissingBaseData: [],
                    personsWithMembershipValidationIssues: [],
                })
            );
            const reloadSubject = new Subject<void>();
            (component as any).generalElectionCommitteeDetailsService.reload$ = reloadSubject;
            const routerMock = {navigate: jest.fn().mockResolvedValue(true)};
            membershipCandidateListServiceMock.validateCandidateList = validateCandidateListMock;
            (component as any).router = routerMock;
            component.selectedIds = ['candidate-1'];

            const nextSpy = jest.spyOn(reloadSubject, 'next');

            component.validateCandidateList();

            expect(validateCandidateListMock).toHaveBeenCalledWith('committee-123', ['candidate-1'], false);
            expect(component.personDuplicates().length).toBe(0);
            expect(component.validationResult()?.createdPersons.length).toBe(1);
            expect(component.validationResult()?.existingPersons.length).toBe(1);
            expect(routerMock.navigate).not.toHaveBeenCalled();
            expect(nextSpy).toHaveBeenCalled();
            expect(notificationServiceMock.success).toHaveBeenCalledWith({message: 'generalElection.candidateList.validate.success', timeout: 10000});
        });

        it('should handle validate candidate list errors', () => {
            const validateCandidateListMock = jest.fn().mockReturnValue(throwError(() => new Error('Validation failed')));
            membershipCandidateListServiceMock.validateCandidateList = validateCandidateListMock;
            component.selectedIds = ['candidate-1'];

            component.validateCandidateList();

            expect(validateCandidateListMock).toHaveBeenCalledWith('committee-123', ['candidate-1'], false);
            expect(notificationServiceMock.error).toHaveBeenCalledWith('generalElection.candidateList.validate.error');
        });

        it('should validate empty candidate list', () => {
            const validateCandidateListMock = jest.fn().mockReturnValue(
                of({
                    isValid: true,
                    errors: [],
                    duplicateCheckResults: [],
                    createdPersons: [],
                    existingPersons: [],
                    areJustificationsMissing: false,
                    areContactPointsMissing: false,
                    isReadyForProposalActivated: false,
                    allValidationsPassed: false,
                    personsWithMissingInterests: [],
                    personsWithMissingBaseData: [],
                    personsWithMembershipValidationIssues: [],
                })
            );
            const reloadSubject = new Subject<void>();
            const routerMock = {navigate: jest.fn().mockResolvedValue(true)};
            membershipCandidateListServiceMock.validateCandidateList = validateCandidateListMock;
            (component as any).generalElectionCommitteeDetailsService.reload$ = reloadSubject;
            (component as any).router = routerMock;
            component.selectedIds = [];

            component.validateCandidateList();

            expect(validateCandidateListMock).toHaveBeenCalledWith('committee-123', [], false);
        });
    });

    describe('clearValidationList', () => {
        it('clears the targeted list when validation result exists', () => {
            component.validationResult.set({
                isValid: false,
                errors: ['Error 1'],
                duplicateCheckResults: [{membershipCandidateToCheck: mockMembershipCandidates[0], duplicateReason: DuplicateReason.FullMatch}],
                createdPersons: [],
                existingPersons: [],
                areJustificationsMissing: false,
                areContactPointsMissing: false,
                isReadyForProposalActivated: false,
                allValidationsPassed: false,
                personsWithMissingInterests: [],
                personsWithMissingBaseData: [],
                personsWithMembershipValidationIssues: [],
            } as any);

            component.clearValidationList('errors');

            expect(component.validationResult()?.errors).toEqual([]);
            expect(component.validationResult()?.duplicateCheckResults.length).toBe(1);
        });

        it('leaves validation result undefined when not set', () => {
            component.validationResult.set(undefined);

            component.clearValidationList('errors');

            expect(component.validationResult()).toBeUndefined();
        });
    });

    it('should show info notification when export starts', () => {
        component.downloadCandidateList();

        expect(notificationServiceMock.info).toHaveBeenCalledWith(
            expect.objectContaining({
                title: 'generalElection.candidateList.export.title',
                message: 'generalElection.candidateList.export.message',
                timeout: 10000,
            })
        );
    });

    it('should show success notification and update successful exports on success', () => {
        component.downloadCandidateList();

        expect(notificationServiceMock.success).toHaveBeenCalledWith(
            expect.objectContaining({
                message: 'generalElection.candidateList.export.success',
            })
        );
    });

    it('should call downloadFileFromHttpResponse on success', () => {
        const downloadSpy = jest.spyOn(fileUtil, 'downloadFileFromHttpResponse').mockImplementation(() => {});

        component.downloadCandidateList();

        expect(downloadSpy).toHaveBeenCalled();
        downloadSpy.mockRestore();
    });

    it('should show error notification and update failed exportsand update failed exports on error', () => {
        membershipCandidateListServiceMock.generateExport = jest.fn().mockReturnValue(throwError(() => new Error('fail')));

        component.downloadCandidateList();

        expect(notificationServiceMock.error).toHaveBeenCalledWith(
            expect.objectContaining({
                message: 'generalElection.candidateList.export.error',
            })
        );
    });
});
