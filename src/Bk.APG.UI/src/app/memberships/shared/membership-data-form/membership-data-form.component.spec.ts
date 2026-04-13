import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatOption} from '@angular/material/autocomplete';
import {MatDatepicker, MatDatepickerModule, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormField, MatFormFieldModule, MatLabel} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatSelect} from '@angular/material/select';
import {MatTooltipModule} from '@angular/material/tooltip';
import {ActivatedRoute, ActivatedRouteSnapshot, Router} from '@angular/router';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {MembershipCreate} from '@api/MembershipCreate';
import {MembershipUpdate} from '@api/MembershipUpdate';
import {PersonDetails} from '@api/PersonDetails';
import {TranslateModule, TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObNotificationService, ObUnsavedChangesDirective} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {MasterDataService} from '@shared/master-data.service';
import {MockComponents, MockDirectives, MockModule, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';
import {Role} from '../../../auth/Role';
import {CommitteeOverviewBasicDataComponent} from '../../../committees/committee-details/committee-overview/committee-overview-basic-data/committee-overview-basic-data.component';
import {CommitteesService} from '../../../committees/committees.service';
import {ConfigsService} from '../../../configs.service';
import {PersonsService} from '../../../persons/persons.service';
import {PersonSearchComponent} from '../../../persons/shared/person-search/person-search.component';
import {CommitteeSearchComponent} from '../committee-search/committee-search.component';
import {MembershipDataFormComponent} from './membership-data-form.component';

describe('MembershipDataFormComponent', () => {
    let component: MembershipDataFormComponent;
    let fixture: ComponentFixture<MembershipDataFormComponent>;
    let membershipAdditionsSignal: ReturnType<typeof signal>;

    let activatedRouteMock: Partial<ActivatedRoute>;
    let notificationServiceMock: Partial<ObNotificationService>;
    let committeesServiceMock: Partial<CommitteesService>;
    let routerMock: Partial<Router>;
    let reloadSubject: BehaviorSubject<void>;

    const translateServiceMock = {
        instant: jest.fn(),
    };

    const roles: Role[] = [];
    const rolesSubject = new BehaviorSubject(roles);
    const authServiceMock = {
        roles$: rolesSubject.asObservable(),
    };

    beforeEach(async () => {
        activatedRouteMock = {
            snapshot: {
                params: {id: '123'},
            } as unknown as ActivatedRouteSnapshot,
        };
        membershipAdditionsSignal = signal([
            {id: 'addition-1', text: 'Membership Addition EN', isDeleted: false},
            {id: 'addition-2', text: 'Deleted Addition', isDeleted: true},
        ]);

        const masterDataServiceMock = {
            electionTypes: signal([
                {id: 'id1', text: 'type1', description: 'desc1', isDeleted: false},
                {id: 'id2', text: 'type2', description: 'desc2', isDeleted: true},
            ]),
            electionOffices: signal([
                {id: 'id1', text: 'office1', description: 'desc1', isDeleted: false},
                {id: 'id2', text: 'office2', description: 'desc2', isDeleted: true},
            ]),
            functions: signal([
                {id: 'id1', text: 'function1', description: 'desc1', isDeleted: false},
                {id: 'id2', text: 'function2', description: 'desc2', isDeleted: true},
            ]),
            committeeTypes: jest.fn(),
            terms: jest.fn(),
            membershipAdditions: membershipAdditionsSignal,
        } as unknown as Partial<MasterDataService>;

        notificationServiceMock = {
            success: jest.fn(),
            error: jest.fn(),
        };

        reloadSubject = new BehaviorSubject<void>(undefined);

        committeesServiceMock = {
            createMember: jest.fn(),
            validateMembership: jest
                .fn()
                .mockReturnValue(of({hasErrors: false, tooManyMembers: false, maximumDurationExceeded: false, isAlreadyActiveMember: false})),
            reload$: reloadSubject,
        };

        const errorServiceMock = {
            getControlError: jest.fn(),
        };

        const configsServiceMock = {
            frontendConfig: {
                entityIds: {
                    gender: {
                        maleId: 'maleId',
                        femaleId: 'femaleId',
                    },
                    electionOffice: {
                        federalGovernmentId: 'federalGovernmentId',
                        otherId: 'otherId',
                    },
                    electionType: {
                        newElectionId: 'newElectionId',
                        reElectionId: 'reElectionId',
                        maximumDutyRetirementId: 'maximumDutyRetirementId',
                        deceasedId: 'deceasedId',
                    },
                    committeeType: {
                        managementId: 'managementId',
                        federalAgenciesId: 'federalAgenciesId',
                    },
                },
            },
        };

        routerMock = {navigate: jest.fn().mockResolvedValue(true)};

        const personsServiceMock: Partial<PersonsService> = {
            getPersonsByName: jest.fn(),
        };

        await TestBed.configureTestingModule({
            imports: [
                MembershipDataFormComponent,
                MockPipe(TranslatePipe),
                MockModule(ReactiveFormsModule),
                MockModule(MatFormFieldModule),
                MockModule(MatDatepickerModule),
                MockModule(MatTooltipModule),
                MockModule(TranslateModule),
                MockComponents(
                    MatFormField,
                    MatOption,
                    MatSelect,
                    MatDatepicker,
                    MatDatepickerToggle,
                    PersonSearchComponent,
                    CommitteeSearchComponent,
                    CommitteeOverviewBasicDataComponent
                ),
                MockDirectives(MatInput, MatLabel, ObUnsavedChangesDirective),
            ],
            providers: [
                {provide: Router, useValue: routerMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: CommitteesService, useValue: committeesServiceMock},
                {provide: ErrorService, useValue: errorServiceMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: PersonsService, useValue: personsServiceMock},
                {provide: AuthService, useValue: authServiceMock},
                {provide: ConfigsService, useValue: configsServiceMock},
            ],
        })
            .overrideTemplateUsingTestingModule(MembershipDataFormComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(MembershipDataFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
        component.membershipModification.set({} as MembershipCreate);
        component.membershipForm.controls.membershipAdditionId.setValue('membershipAddition');
        component.membershipForm.controls.justificationLongerDuty.setValue('justificationLongerDuty');
        component.membershipForm.controls.justificationShorterDuty.setValue('justificationShorterDuty');
        component.membershipForm.controls.maximumEmploymentLevel.setValue(20);
        component.membershipForm.controls.remarks.setValue('remarks');
        component.membershipForm.controls.remarksStatus.setValue('remarksStatus');
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should return activeFunctions correctly', () => {
        expect(component.activeFunctions().length).toBe(1);
    });

    it('should return activeElectionTypes correctly', () => {
        expect(component.activeElectionTypes().length).toBe(1);
    });

    it('should return activeElectionOffices correctly', () => {
        expect(component.activeElectionOffices().length).toBe(1);
    });

    it('should be invalid when justificationLongerDuty missing', () => {
        const committeeDetails = {id: '1', extraParliamentaryCommission: true} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);

        component.membershipForm.controls.justificationLongerDuty.setValue('');
        component.membershipForm.controls.beginDate.setValue(new Date(2020, 1, 1));
        component.membershipForm.controls.endDate.setValue(new Date(2033, 1, 1));
        component.membershipForm.controls.inCorrelationWithFederalDuty.setValue(false);

        component.personSelected.set({id: '100'} as PersonDetails);
        component.updateValidity(true);

        expect(component.membershipForm.controls.justificationLongerDuty.valid).toBe(false);
    });

    it('should be invalid when justificationShorterDuty missing', () => {
        const committeeDetails = {id: '1', period4YearsInGeneralElection: true} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);

        component.membershipForm.controls.justificationShorterDuty.setValue('');
        component.membershipForm.controls.beginDate.setValue(new Date(2020, 1, 1));
        component.membershipForm.controls.endDate.setValue(new Date(2023, 1, 1));

        component.personSelected.set({id: '100'} as PersonDetails);
        component.updateValidity(true);

        expect(component.membershipForm.controls.justificationShorterDuty.valid).toBe(false);
    });

    it('should be invalid when justificationMemberInFederalDuty missing', () => {
        const committeeDetails = {id: '1', period4YearsInGeneralElection: true} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);

        component.membershipForm.controls.justificationMemberInFederalDuty.setValue('');
        component.membershipForm.controls.inCorrelationWithFederalDuty.setValue(true);

        component.personSelected.set({id: '100', federalDuty: true} as PersonDetails);
        component.updateValidity(true);

        expect(component.membershipForm.controls.justificationMemberInFederalDuty.valid).toBe(false);
    });

    it('should be invalid when justificationMemberInFederalAssembly missing', () => {
        const committeeDetails = {id: '1', period4YearsInGeneralElection: true} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);

        component.membershipForm.controls.justificationMemberInFederalAssembly.setValue('');
        component.membershipForm.controls.inCorrelationWithFederalDuty.setValue(true);

        component.personSelected.set({id: '100', federalAssembly: true} as PersonDetails);
        component.updateValidity(true);

        expect(component.membershipForm.controls.justificationMemberInFederalAssembly.valid).toBe(false);
    });

    it('should be invalid when requirementsProfile missing', () => {
        const committeeDetails = {committeeTypeId: 'managementId'} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);

        component.membershipForm.controls.requirementsProfile.setValue('');
        component.membershipForm.controls.electionTypeId.setValue('newElectionId');

        component.personSelected.set({id: '100'} as PersonDetails);

        expect(component.membershipForm.controls.requirementsProfile.valid).toBe(false);
    });

    it('should enable requirementsProfile', () => {
        const committeeDetails = {id: '5', committeeTypeId: 'managementId'} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);

        component.personSelected.set({id: '100'} as PersonDetails);
        component.membershipForm.controls.beginDate.setValue(new Date(2020, 2, 1));
        component.membershipForm.controls.endDate.setValue(new Date(2030, 2, 1));
        component.membershipForm.controls.electionTypeId.setValue('newElectionId');

        component.updateValidity(false);

        expect(component.membershipForm.controls.requirementsProfile.enabled).toBe(true);
    });

    it('should disable requirementsProfile for other election types', () => {
        const committeeDetails = {id: '5', committeeTypeId: 'managementId'} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);

        component.personSelected.set({id: '100'} as PersonDetails);
        component.membershipForm.controls.beginDate.setValue(new Date(2020, 2, 1));
        component.membershipForm.controls.endDate.setValue(new Date(2030, 2, 1));
        component.membershipForm.controls.electionTypeId.setValue('maximumDutyRetirementId');

        expect(component.membershipForm.controls.requirementsProfile.enabled).toBe(false);
    });

    it('should disable requirementsProfile for other committee types', () => {
        const committeeDetails = {id: '5', committeeTypeId: 'otherId'} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);

        component.personSelected.set({id: '100'} as PersonDetails);
        component.membershipForm.controls.beginDate.setValue(new Date(2020, 2, 1));
        component.membershipForm.controls.endDate.setValue(new Date(2030, 2, 1));
        component.membershipForm.controls.electionTypeId.setValue('newElectionId');

        expect(component.membershipForm.controls.requirementsProfile.enabled).toBe(false);
    });

    it('should disable requirementsProfile for other election office', () => {
        const committeeDetails = {id: '5', committeeTypeId: 'managementId'} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);

        component.personSelected.set({id: '100'} as PersonDetails);
        component.membershipForm.controls.beginDate.setValue(new Date(2020, 2, 1));
        component.membershipForm.controls.endDate.setValue(new Date(2099, 2, 1));
        component.membershipForm.controls.electionTypeId.setValue('newElectionId');
        component.membershipForm.controls.electionOfficeId.setValue('otherId');

        component.updateValidity(false);

        expect(component.membershipForm.controls.requirementsProfile.enabled).toBe(false);
    });

    it('should updateValidity set checkbox inCorrelationWithFederalDutyCheckbox true', () => {
        component.personSelected.set({id: '100', federalDuty: true, federalAssembly: true} as PersonDetails);
        component.updateValidity(true);
        expect(component.membershipForm.controls.inCorrelationWithFederalDuty.value).toBe(true);
    });

    it('should updateValidity set checkbox inCorrelationWithFederalDutyCheckbox false', () => {
        component.personSelected.set({id: '100', federalDuty: false, federalAssembly: true} as PersonDetails);
        component.updateValidity(true);
        expect(component.membershipForm.controls.inCorrelationWithFederalDuty.value).toBe(false);
    });

    it('should updateValidity call validation', () => {
        component.personSelected.set({id: '100', federalDuty: true, federalAssembly: true} as PersonDetails);
        component.committeeEntity.set({id: '200'} as CommitteeDetails);
        component.membershipForm.controls.beginDate.setValue(new Date(2001, 5, 5));
        component.membershipForm.controls.endDate.setValue(new Date(2002, 5, 5));
        component.updateValidity(true);
        expect(committeesServiceMock.validateMembership).toHaveBeenCalled();
    });

    it('should hasDeletedElectionOffice return correctly using deleted electionOffice', () => {
        component.membershipModification.set({electionOfficeId: 'id1'} as MembershipUpdate);
        expect(component.hasDeletedElectionOffice()).toBe(false);
    });

    it('should hasDeletedElectionOffice return correctly using active electionOffice', () => {
        component.membershipModification.set({electionOfficeId: 'id2'} as MembershipUpdate);
        expect(component.hasDeletedElectionOffice()).toBe(true);
    });

    it('should hasDeletedFunction return correctly using deleted function', () => {
        component.membershipModification.set({functionId: 'id1'} as MembershipUpdate);
        expect(component.hasDeletedFunction()).toBe(false);
    });

    it('should hasDeletedFunction return correctly using active function', () => {
        component.membershipModification.set({functionId: 'id2'} as MembershipUpdate);
        expect(component.hasDeletedFunction()).toBe(true);
    });

    it('should disable election type in create mode', () => {
        expect(component.membershipForm.controls.electionTypeId.disabled).toBe(true);
    });

    describe('termOfOfficeEndDate', () => {
        it('should set endDate to termOfOfficeEndDate when creating new membership and committee is selected', () => {
            component.isUpdateMode = false;
            const termOfOfficeEndDate = new Date(2025, 11, 31);
            const committeeDetails = {id: '1', termOfOfficeEndDate} as CommitteeDetails;

            component.committeeSelected.set(committeeDetails);
            fixture.detectChanges();

            expect(component.membershipForm.controls.endDate.value).toEqual(termOfOfficeEndDate);
        });

        it('should not set endDate when in update mode', () => {
            component.isUpdateMode = true;
            const termOfOfficeEndDate = new Date(2025, 11, 31);
            const committeeDetails = {id: '1', termOfOfficeEndDate} as CommitteeDetails;
            const originalEndDate = new Date(2024, 5, 15);

            component.membershipForm.controls.endDate.setValue(originalEndDate);
            component.committeeSelected.set(committeeDetails);
            fixture.detectChanges();

            expect(component.membershipForm.controls.endDate.value).toEqual(originalEndDate);
        });

        it('should not set endDate when committee has no termOfOfficeEndDate', () => {
            component.isUpdateMode = false;
            const committeeDetails = {id: '1'} as CommitteeDetails;
            const originalEndDate = component.membershipForm.controls.endDate.value;

            component.committeeSelected.set(committeeDetails);
            fixture.detectChanges();

            expect(component.membershipForm.controls.endDate.value).toEqual(originalEndDate);
        });
    });

    it('should refresh selected membership addition text when membership additions are updated', () => {
        component.membershipAdditionId = 'addition-1';
        component.membershipForm.controls.membershipAdditionId.setValue('Mitgliedschaftszusatz DE', {emitEvent: false});

        membershipAdditionsSignal.set([
            {id: 'addition-1', text: 'Membership Addition FR', isDeleted: false},
            {id: 'addition-2', text: 'Deleted Addition', isDeleted: true},
        ]);
        fixture.detectChanges();

        expect(component.membershipForm.controls.membershipAdditionId.value).toBe('Membership Addition FR');
    });
});
