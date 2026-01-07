import {HttpClient} from '@angular/common/http';
import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatOption} from '@angular/material/autocomplete';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatDatepicker, MatDatepickerModule, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatInput} from '@angular/material/input';
import {MatSelect} from '@angular/material/select';
import {CommitteeCreate} from '@api/CommitteeCreate';
import {CommitteeType} from '@api/CommitteeType';
import {CommitteeUpdate} from '@api/CommitteeUpdate';
import {Level} from '@api/Level';
import {MembershipAddition} from '@api/MembershipAddition';
import {Office} from '@api/Office';
import {TranslatePipe} from '@ngx-translate/core';
import {ObErrorMessagesModule, ObInputClearModule, ObUnsavedChangesDirective} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {MembersTooltipContentComponent} from '@shared/members-tooltip-content/members-tooltip-content.component';
import {MockComponents, MockDirectives, MockModule, MockPipe} from 'ng-mocks';
import {BehaviorSubject} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';
import {Role} from '../../../auth/Role';
import {ConfigsService} from '../../../configs.service';
import {CommitteeDataFormComponent} from './committee-data-form.component';

describe('CommitteeDataFormComponent', () => {
    let createComponent: CommitteeDataFormComponent;
    let updateComponent: CommitteeDataFormComponent;
    let updateFixture: ComponentFixture<CommitteeDataFormComponent>;
    let createFixture: ComponentFixture<CommitteeDataFormComponent>;
    const roles: Role[] = [];
    const rolesSubject = new BehaviorSubject(roles);

    const authServiceMock = {
        roles$: rolesSubject.asObservable(),
    };

    const masterDataServiceMock = {
        levels: signal([{id: '1'}] as Level[]),
        offices: signal([] as Office[]),
        membershipAdditions: signal([] as MembershipAddition[]),
        departments: jest.fn(),
        committeeTypes: signal([
            {id: '1', uri: 'www.todo.uri.Behördenkommissionen'},
            {id: '2', uri: 'www.todo.uri.Verwaltungskommissionen'},
            {id: '3', uri: 'www.todo.uri.Leitungsorgane'},
            {id: '4', uri: 'www.todo.uri.Vertretungen des Bundes'},
            {id: '5', uri: 'www.todo.uri.Vertretungen des Bundes in grenzüberschreitenden Gremien'},
        ] as CommitteeType[]),
        legalForms: jest.fn(),
        terms: jest.fn(),
    };

    const errorServiceMock = {
        getControlError: jest.fn(),
    };

    const configsServiceMock = {
        frontendConfig: {
            entityIds: {
                committeeType: {
                    authorityId: 'authorityId',
                    administrationId: 'administrationId',
                    managementId: 'managementId',
                    federalAgenciesId: 'federalAgenciesId',
                    federalAgenciesCrossBorderId: 'federalAgenciesCrossBorderId',
                },
                committeeLevel: {
                    federalCouncilId: 'federalCouncilId',
                },
                termOfOffice: {
                    period4YearsInGeneralElectionId: 'period4YearsInGeneralElectionId',
                },
            },
        },
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                CommitteeDataFormComponent,
                MockModule(ReactiveFormsModule),
                MockModule(ObErrorMessagesModule),
                MockModule(MatIconModule),
                MockModule(ObInputClearModule),
                MockModule(MatDatepickerModule),
                MockComponents(
                    MatFormField,
                    MatCheckbox,
                    MatOption,
                    MatSelect,
                    MatDatepicker,
                    MatDatepickerToggle,
                    HelpTooltipComponent,
                    MembersTooltipContentComponent
                ),
                MockDirectives(MatInput, MatLabel, ObUnsavedChangesDirective),
                MockPipe(TranslatePipe),
            ],
            providers: [
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: ErrorService, useValue: errorServiceMock},
                {provide: AuthService, useValue: authServiceMock},
                {provide: ConfigsService, useValue: configsServiceMock},
                HttpClient,
            ],
        }).compileComponents();

        updateFixture = TestBed.createComponent(CommitteeDataFormComponent);
        createFixture = TestBed.createComponent(CommitteeDataFormComponent);

        updateComponent = updateFixture.componentInstance;
        updateComponent.committeeModification.set({} as CommitteeUpdate);
        updateComponent.isUpdateMode = true;

        createComponent = createFixture.componentInstance;
        createComponent.committeeModification.set({} as CommitteeUpdate);
        createComponent.isUpdateMode = false;

        updateFixture.detectChanges();
        createFixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();

        updateFixture.destroy();
        createFixture.destroy();

        // Reset rolesSubject to empty state for test isolation
        rolesSubject.next([]);
    });

    it('should create the component', () => {
        expect(updateComponent).toBeTruthy();
    });

    it('should set permissions in create mode', () => {
        createComponent.committeeModification.set({canEditAll: false, canEditDepartment: true, canEditLegalbase: true, departmentId: '11'} as CommitteeCreate);

        createFixture.detectChanges();

        expect(createComponent.committeeForm.controls.descriptionGerman.disabled).toBe(true);
        expect(createComponent.committeeForm.controls.departmentId.value).toBe('11');
        expect(createComponent.committeeForm.controls.departmentId.disabled).toBe(false);
        expect(createComponent.committeeForm.controls.legalBase.disabled).toBe(false);
        expect(createComponent.canEditAll).toBe(false);
    });

    it('should set permissions in update mode', () => {
        updateComponent.committeeModification.set({
            canEditAll: false,
            canEditDepartment: true,
            canEditLegalbase: true,
            id: '1',
            departmentId: '11',
        } as CommitteeUpdate);

        updateFixture.detectChanges();

        expect(updateComponent.committeeForm.controls.descriptionGerman.disabled).toBe(true);
        expect(updateComponent.committeeForm.controls.departmentId.value).toBe('11');
        expect(updateComponent.committeeForm.controls.departmentId.disabled).toBe(false);
        expect(updateComponent.committeeForm.controls.legalBase.disabled).toBe(false);
        expect(updateComponent.canEditAll).toBe(false);
    });

    it('should disable all in update mode', () => {
        updateComponent.committeeModification.set({
            canEditAll: false,
            canEditDepartment: false,
            canEditLegalbase: false,
            id: '1',
            departmentId: '',
        } as CommitteeUpdate);

        updateFixture.detectChanges();

        expect(updateComponent.committeeForm.controls.departmentId.value).toBe('');
        expect(updateComponent.committeeForm.controls.departmentId.disabled).toBe(true);
        expect(updateComponent.committeeForm.controls.legalBase.disabled).toBe(true);
        expect(updateComponent.canEditAll).toBe(false);
    });

    it('should enable all in update mode', () => {
        updateComponent.committeeModification.set({
            canEditAll: true,
            canEditDepartment: true,
            canEditLegalbase: true,
            id: '1',
            departmentId: '11',
        } as CommitteeUpdate);

        updateFixture.detectChanges();

        expect(updateComponent.committeeForm.controls.departmentId.value).toBe('11');
        expect(updateComponent.committeeForm.controls.descriptionGerman.disabled).toBe(false);
        expect(updateComponent.committeeForm.controls.departmentId.disabled).toBe(false);
        expect(updateComponent.committeeForm.controls.legalBase.disabled).toBe(false);
        expect(updateComponent.canEditAll).toBe(true);
    });

    describe('Form Initialization', () => {
        let form: FormGroup;

        beforeEach(() => {
            form = updateComponent.committeeForm;
        });

        it('should initialize the form with default values', () => {
            expect(form).toBeDefined();
            expect(form.controls.descriptionGerman.value).toBe('');
            expect(form.controls.descriptionFrench.value).toBe('');
            expect(form.controls.descriptionItalian.value).toBe('');
            expect(form.controls.descriptionRomansh.value).toBe('');
            expect(form.controls.levelId.value).toBe('');
            expect(form.controls.departmentId.value).toBe('');
            expect(form.controls.officeId.value).toBe('');
            expect(form.controls.committeeTypeId.value).toBe('');
            expect(form.controls.legalFormId.value).toBe(undefined);
            expect(form.controls.oldLegalForm.value).toBe(undefined);
            expect(form.controls.legalBase.value).toBe(undefined);
            expect(form.controls.federalLawEstablishment.value).toBe(null);
            expect(form.controls.supervisionDuty.value).toBe(null);
            expect(form.controls.endDate.value).toBe(undefined);
            expect(form.controls.termOfOfficeId.value).toBe('');
            expect(form.controls.minimalMembers.value).toBe(undefined);
            expect(form.controls.maximalMembers.value).toBe(undefined);
            expect(form.controls.additionalAuthorityMembers.value).toBe(null);
            expect(form.controls.linkAuthorityWebsite.value).toBe(null);
        });

        it('should disable oldLegalForm fields', () => {
            expect(form.controls.oldLegalForm.disabled).toBe(true);
        });
    });

    describe('Form Validation', () => {
        let form: FormGroup;

        beforeEach(() => {
            form = updateComponent.committeeForm;
        });

        it.each([
            'descriptionGerman',
            'descriptionFrench',
            'descriptionItalian',
            'descriptionRomansh',
            'levelId',
            'departmentId',
            'officeId',
            'termOfOfficeId',
        ])('should should require %s', controlPath => {
            const control = form.get(controlPath)!;
            expect(control).not.toBeNull();

            control.setValue('');
            expect(control.valid).toBe(false);
            expect(control.hasError('required')).toBe(true);

            control.setValue('foo');
            expect(control.valid).toBe(true);
        });

        it('should validate beginDate and endDate with correct range', () => {
            const beginDate = form.controls.beginDate;
            const endDate = form.controls.endDate;

            beginDate.setValue(new Date('2021-01-01'));
            endDate.setValue(new Date('2020-02-01'));

            expect(endDate.valid).toBe(false);
        });

        it('should validate minimalMembers and maximalMembers with correct range', () => {
            const minimalMembers = form.controls.minimalMembers;
            const maximalMembers = form.controls.maximalMembers;

            minimalMembers.setValue(10);
            maximalMembers.setValue(5);

            expect(maximalMembers.valid).toBe(false);
        });
    });

    describe('Changing department', () => {
        beforeEach(() => {
            masterDataServiceMock.offices.set([
                {id: '1', departmentId: '1'},
                {id: '2', departmentId: '1'},
                {id: '3', departmentId: '2'},
            ] as Office[]);
        });

        it('should set department offices', () => {
            updateComponent.committeeForm.controls.departmentId.setValue('1');

            expect(updateComponent.departmentOffices()).toEqual([
                {id: '1', departmentId: '1'},
                {id: '2', departmentId: '1'},
            ]);
        });
    });

    describe('Changing committee level', () => {
        it('should add required validators to minimalMembers and maximalMembers if committee level is federal council', () => {
            updateComponent.committeeForm.controls.levelId.setValue(configsServiceMock.frontendConfig.entityIds.committeeLevel.federalCouncilId);
            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.minimalMembers.hasValidator(Validators.required)).toBe(true);
            expect(updateComponent.committeeForm.controls.maximalMembers.hasValidator(Validators.required)).toBe(true);
        });

        it('should remove required validators from minimalMembers and maximalMembers if committee level is not federal council', () => {
            updateComponent.committeeForm.controls.minimalMembers.addValidators(Validators.required);
            updateComponent.committeeForm.controls.maximalMembers.addValidators(Validators.required);

            updateComponent.committeeForm.controls.levelId.setValue('Foo bar');
            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.minimalMembers.hasValidator(Validators.required)).toBe(false);
            expect(updateComponent.committeeForm.controls.maximalMembers.hasValidator(Validators.required)).toBe(false);
        });
    });

    describe('vacanciesInCurrentTermOfOffice', () => {
        it('should return null if selectedCommitteeLevelId is not federalCouncilId', () => {
            updateComponent.committeeForm.controls.levelId.setValue('NOT_FEDERAL');

            updateFixture.detectChanges();

            expect(updateComponent.vacanciesInCurrentTermOfOffice()).toBeNull();
        });

        it('should return correct vacancies when membersCount is below minimum', () => {
            updateComponent.committeeForm.controls.levelId.setValue('federalCouncilId');
            updateComponent.committeeForm.controls.minimalMembers.setValue(5);
            updateComponent.committeeModification.set({membersCount: 3} as CommitteeUpdate);

            updateFixture.detectChanges();

            expect(updateComponent.vacanciesInCurrentTermOfOffice()).toBe(2);
        });

        it('should return 0 when membersCount is above minimum', () => {
            updateComponent.committeeForm.controls.levelId.setValue('federalCouncilId');
            updateComponent.committeeForm.controls.minimalMembers.setValue(5);
            updateComponent.committeeModification.set({membersCount: 7} as CommitteeUpdate);

            updateFixture.detectChanges();

            expect(updateComponent.vacanciesInCurrentTermOfOffice()).toBe(0);
        });

        it('should return 0 when membersCount equals minimum', () => {
            updateComponent.committeeForm.controls.levelId.setValue('federalCouncilId');
            updateComponent.committeeForm.controls.minimalMembers.setValue(5);
            updateComponent.committeeModification.set({membersCount: 5} as CommitteeUpdate);

            updateFixture.detectChanges();

            expect(updateComponent.vacanciesInCurrentTermOfOffice()).toBe(0);
        });

        it('should handle undefined membersCount and minimalMembers', () => {
            updateComponent.committeeForm.controls.levelId.setValue('federalCouncilId');
            updateComponent.committeeForm.controls.minimalMembers.setValue(undefined);
            updateComponent.committeeModification.set({} as CommitteeUpdate);

            updateFixture.detectChanges();

            expect(updateComponent.vacanciesInCurrentTermOfOffice()).toBe(0);
        });
    });

    describe('vacanciesInGeneralElection validator', () => {
        beforeEach(() => {
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(undefined);
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue([]);
        });

        it('should return null when vacancies is undefined', () => {
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(undefined);
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue(['id1', 'id2']);

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(true);
            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.errors).toBe(null);
        });

        it('should return null when vacancies is null', () => {
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(null);
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue(['id1', 'id2']);

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(true);
            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.errors).toBe(null);
        });

        it('should return null when membershipAdditionsInGeneralElection is not an array', () => {
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(5);
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue(null);

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(true);
            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.errors).toBe(null);
        });

        it('should return null when additions length equals vacancies', () => {
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(3);
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue(['id1', 'id2', 'id3']);

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(true);
            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.errors).toBe(null);
        });

        it('should return null when additions length is less than vacancies', () => {
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(5);
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue(['id1', 'id2']);

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(true);
            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.errors).toBe(null);
        });

        it('should return null when additions array is empty', () => {
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(5);
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue([]);

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(true);
            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.errors).toBe(null);
        });

        it('should return tooManyMembershipAdditions error when additions length exceeds vacancies', () => {
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue(['id1', 'id2', 'id3']);
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(2);
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.updateValueAndValidity();

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(false);
            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.errors).toEqual({
                tooManyMembershipAdditions: {current: 3, vacancies: 2},
            });
        });

        it('should return tooManyMembershipAdditions error when additions length is 1 more than vacancies', () => {
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue(['id1', 'id2', 'id3', 'id4', 'id5', 'id6']);
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(5);
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.updateValueAndValidity();

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(false);
            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.errors).toEqual({
                tooManyMembershipAdditions: {current: 6, vacancies: 5},
            });
        });

        it('should return tooManyMembershipAdditions error when vacancies is 0 and there are additions', () => {
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue(['id1']);
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(0);
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.updateValueAndValidity();

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(false);
            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.errors).toEqual({
                tooManyMembershipAdditions: {current: 1, vacancies: 0},
            });
        });

        it('should update validation when vacancies value changes', () => {
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue(['id1', 'id2', 'id3']);

            // Initially set to 5 - should be valid
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(5);

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(true);

            // Change to 2 - should be invalid
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(2);

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(false);
            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.errors).toEqual({
                tooManyMembershipAdditions: {current: 3, vacancies: 2},
            });

            // Change to 3 - should be valid again
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(3);

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(true);
        });

        it('should update validation when membershipAdditions array changes', () => {
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.setValue(2);

            updateFixture.detectChanges();

            // Initially set to 2 additions - should be valid
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue(['id1', 'id2']);
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.updateValueAndValidity();

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(true);

            // Add one more - should be invalid
            updateComponent.committeeForm.controls.membershipAdditionsInGeneralElection.setValue(['id1', 'id2', 'id3']);
            updateComponent.committeeForm.controls.vacanciesInGeneralElection.updateValueAndValidity();

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.valid).toBe(false);
            expect(updateComponent.committeeForm.controls.vacanciesInGeneralElection.errors).toEqual({
                tooManyMembershipAdditions: {current: 3, vacancies: 2},
            });
        });
    });

    describe('Begin date permissions', () => {
        it('should disable beginDate when not in update mode', () => {
            createComponent.isAdmin.set(true);
            createComponent.isDepartment.set(false);

            createFixture.detectChanges();

            expect(createComponent.committeeForm.controls.beginDate.disabled).toBe(true);
        });

        it('should disable beginDate in update mode when neither admin nor department', () => {
            updateComponent.isAdmin.set(false);
            updateComponent.isDepartment.set(false);
            updateComponent.isOffice.set(true);

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.beginDate.disabled).toBe(true);
        });

        it('should disable beginDate when department with past date', () => {
            updateComponent.isAdmin.set(false);

            const pastDate = new Date();
            pastDate.setDate(pastDate.getDate() - 1);
            updateComponent.committeeForm.controls.beginDate.setValue(pastDate);

            updateComponent.isDepartment.set(true);
            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.beginDate.disabled).toBe(true);
        });

        it('should disable beginDate when department with today date', () => {
            updateComponent.isAdmin.set(false);
            updateComponent.isDepartment.set(true);
            updateFixture.detectChanges();

            updateComponent.committeeForm.controls.beginDate.setValue(new Date());

            expect(updateComponent.committeeForm.controls.beginDate.disabled).toBe(true);
        });
    });

    describe('End date permissions', () => {
        it('should disable endDate in create mode', () => {
            expect(createComponent.committeeForm.controls.endDate.disabled).toBe(true);
        });

        it('should enable endDate in update mode when no original end date exists', () => {
            updateComponent.isAdmin.set(false);
            updateComponent.isDepartment.set(true);

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.endDate.disabled).toBe(false);
        });

        it('should disable endDate in update mode when original end date exists', () => {
            updateComponent.committeeModification.set({endDate: new Date('2023-12-31'), id: '123'} as CommitteeUpdate);
            updateComponent.isAdmin.set(false);

            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.endDate.disabled).toBe(true);
        });

        it('should enable endDate for admin even if original end date exists', () => {
            // Create a fresh fixture with admin role
            const adminFixture = TestBed.createComponent(CommitteeDataFormComponent);
            const adminComponent = adminFixture.componentInstance;

            rolesSubject.next([Role.Admin]);
            adminComponent.committeeModification.set({endDate: new Date('2023-12-31'), id: '123'} as CommitteeUpdate);
            adminComponent.isUpdateMode = true;

            adminFixture.detectChanges();

            expect(adminComponent.committeeForm.controls.endDate.disabled).toBe(false);
        });
    });

    describe('markEndDateAsSaved', () => {
        it('should not affect form when in create mode', () => {
            createComponent.committeeForm.controls.endDate.setValue(new Date('2024-12-31'));

            createComponent.markEndDateAsSaved();

            expect(createComponent.committeeForm.controls.endDate.disabled).toBe(true); // Already disabled in create mode
        });

        it('should not affect form when no end date is set', () => {
            updateComponent.isDepartment.set(true);
            updateFixture.detectChanges();
            updateComponent.committeeForm.controls.endDate.setValue(undefined);

            updateComponent.markEndDateAsSaved();

            expect(updateComponent.committeeForm.controls.endDate.disabled).toBe(false);
        });

        it('should disable endDate field after being called', () => {
            updateComponent.isDepartment.set(true);
            updateComponent.committeeForm.controls.endDate.setValue(new Date('2024-12-31'));
            updateFixture.detectChanges();
            expect(updateComponent.committeeForm.controls.endDate.disabled).toBe(false);

            updateComponent.markEndDateAsSaved();
            updateFixture.detectChanges();

            expect(updateComponent.committeeForm.controls.endDate.disabled).toBe(true);
        });
    });
});
