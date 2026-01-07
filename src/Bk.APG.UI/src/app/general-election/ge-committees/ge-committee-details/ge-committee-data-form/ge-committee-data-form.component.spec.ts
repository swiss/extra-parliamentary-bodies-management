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
import {CommitteeType} from '@api/CommitteeType';
import {GeneralElectionCommitteeUpdate} from '@api/GeneralElectionCommitteeUpdate';
import {Level} from '@api/Level';
import {Office} from '@api/Office';
import {TranslatePipe} from '@ngx-translate/core';
import {ObErrorMessagesModule, ObInputClearModule, ObUnsavedChangesDirective} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {MembersTooltipContentComponent} from '@shared/members-tooltip-content/members-tooltip-content.component';
import {MockComponents, MockDirectives, MockModule, MockPipe} from 'ng-mocks';
import {BehaviorSubject} from 'rxjs';
import {AuthService} from '../../../../auth/auth.service';
import {Role} from '../../../../auth/Role';
import {ConfigsService} from '../../../../configs.service';
import {GeneralElectionCommitteeDataFormComponent} from './ge-committee-data-form.component';

describe('GeneralElectionCommitteeDataFormComponent', () => {
    let component: GeneralElectionCommitteeDataFormComponent;
    let fixture: ComponentFixture<GeneralElectionCommitteeDataFormComponent>;
    const roles: Role[] = [];
    const rolesSubject = new BehaviorSubject(roles);

    const authServiceMock = {
        roles$: rolesSubject.asObservable(),
    };

    const masterDataServiceMock = {
        levels: signal([{id: '1'}] as Level[]),
        offices: signal([] as Office[]),
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
                GeneralElectionCommitteeDataFormComponent,
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

        fixture = TestBed.createComponent(GeneralElectionCommitteeDataFormComponent);
        component = fixture.componentInstance;
        component.committeeModification.set({} as GeneralElectionCommitteeUpdate);
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create the component', () => {
        expect(component).toBeTruthy();
    });

    it('should set permissions', () => {
        component.committeeModification.set({
            canEditAll: false,
            canEditDepartment: true,
            canEditLegalbase: true,
            id: '1',
            departmentId: '11',
        } as GeneralElectionCommitteeUpdate);
        fixture.detectChanges();
        expect(component.committeeForm.controls.descriptionGerman.disabled).toBe(true);
        expect(component.committeeForm.controls.departmentId.value).toBe('11');
        expect(component.committeeForm.controls.departmentId.disabled).toBe(false);
        expect(component.committeeForm.controls.legalBase.disabled).toBe(false);
        expect(component.canEditAll).toBe(false);
    });

    it('should disable all', () => {
        component.committeeModification.set({
            canEditAll: false,
            canEditDepartment: false,
            canEditLegalbase: false,
            id: '1',
            departmentId: '',
        } as GeneralElectionCommitteeUpdate);
        fixture.detectChanges();
        expect(component.committeeForm.controls.departmentId.value).toBe('');
        expect(component.committeeForm.controls.departmentId.disabled).toBe(true);
        expect(component.committeeForm.controls.legalBase.disabled).toBe(true);
        expect(component.canEditAll).toBe(false);
    });

    it('should enable all', () => {
        component.committeeModification.set({
            canEditAll: true,
            canEditDepartment: true,
            canEditLegalbase: true,
            id: '1',
            departmentId: '11',
        } as GeneralElectionCommitteeUpdate);
        fixture.detectChanges();
        expect(component.committeeForm.controls.departmentId.value).toBe('11');
        expect(component.committeeForm.controls.descriptionGerman.disabled).toBe(false);
        expect(component.committeeForm.controls.departmentId.disabled).toBe(false);
        expect(component.committeeForm.controls.legalBase.disabled).toBe(false);
        expect(component.canEditAll).toBe(true);
    });

    describe('Form Initialization', () => {
        let form: FormGroup;

        beforeEach(() => {
            form = component.committeeForm;
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
            expect(form.controls.releaseGeneralElection.value).toBe(null);
            expect(form.controls.federalLawEstablishment.value).toBe(null);
            expect(form.controls.supervisionDuty.value).toBe(null);
            expect(form.controls.endDate.value).toBe(undefined);
            expect(form.controls.termOfOfficeId.value).toBe('');
            expect(form.controls.minimalMembers.value).toBe(undefined);
            expect(form.controls.maximalMembers.value).toBe(undefined);
            expect(form.controls.additionalAuthorityMembers.value).toBe(null);
            expect(form.controls.linkAuthorityWebsite.value).toBe(undefined);
        });

        it('should disable certain fields', () => {
            expect(form.controls.oldLegalForm.disabled).toBe(true);
            expect(form.controls.beginDate.disabled).toBe(true);
        });
    });

    describe('Form Validation', () => {
        let form: FormGroup;

        beforeEach(() => {
            form = component.committeeForm;
        });

        it.each([
            'descriptionGerman',
            'descriptionFrench',
            'descriptionItalian',
            'descriptionRomansh',
            'levelId',
            'departmentId',
            'officeId',
            'committeeTypeId',
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
            component.committeeForm.controls.departmentId.setValue('1');

            expect(component.departmentOffices()).toEqual([
                {id: '1', departmentId: '1'},
                {id: '2', departmentId: '1'},
            ]);
        });
    });

    describe('Changing committee level', () => {
        it('should add required validators to minimalMembers and maximalMembers if committee level is federal council', () => {
            component.committeeForm.controls.levelId.setValue(configsServiceMock.frontendConfig.entityIds.committeeLevel.federalCouncilId);
            fixture.detectChanges();

            expect(component.committeeForm.controls.minimalMembers.hasValidator(Validators.required)).toBe(true);
            expect(component.committeeForm.controls.maximalMembers.hasValidator(Validators.required)).toBe(true);
        });

        it('should remove required validators from minimalMembers and maximalMembers if committee level is not federal council', () => {
            component.committeeForm.controls.minimalMembers.addValidators(Validators.required);
            component.committeeForm.controls.maximalMembers.addValidators(Validators.required);

            component.committeeForm.controls.levelId.setValue('Foo bar');
            fixture.detectChanges();

            expect(component.committeeForm.controls.minimalMembers.hasValidator(Validators.required)).toBe(false);
            expect(component.committeeForm.controls.maximalMembers.hasValidator(Validators.required)).toBe(false);
        });
    });

    describe('Changing committee type', () => {
        let updateValidityForFormSpy: jest.SpyInstance;

        beforeEach(() => {
            component.canEditAll = true;

            jest.spyOn(component.isExtraParliamentaryCommmission, 'emit');

            updateValidityForFormSpy = jest.spyOn(component, 'updateValidityForForm' as never);
        });

        describe('Authority Committee (Behördenkommissionen)', () => {
            beforeEach(() => {
                component.committeeForm.controls.committeeTypeId.setValue(configsServiceMock.frontendConfig.entityIds.committeeType.authorityId);
                fixture.detectChanges();
            });

            it('should emit extraParliamentaryCommission as true', () => {
                expect(component.isExtraParliamentaryCommmission.emit).toHaveBeenCalledWith(true);
            });

            it('should set levelId to federalCouncilId and disable it', () => {
                expect(component.committeeForm.controls.levelId.value).toBe(configsServiceMock.frontendConfig.entityIds.committeeLevel.federalCouncilId);
                expect(component.committeeForm.controls.levelId.disabled).toBe(true);
            });

            it('should enable marketOrientated when canEditAll is true', () => {
                expect(component.committeeForm.controls.marketOrientated.enabled).toBe(true);
            });

            it('should set federalInstitution to null and disable it', () => {
                expect(component.committeeForm.controls.federalInstitution.value).toBe(null);
                expect(component.committeeForm.controls.federalInstitution.disabled).toBe(true);
            });

            it('should set federalLawEstablishment to true and disable it', () => {
                expect(component.committeeForm.controls.federalLawEstablishment.value).toBe(true);
                expect(component.committeeForm.controls.federalLawEstablishment.disabled).toBe(true);
            });

            it('should enable additionalAuthorityMembers when canEditAll is true', () => {
                expect(component.committeeForm.controls.additionalAuthorityMembers.enabled).toBe(true);
            });

            it('should disable legalFormId and set to null', () => {
                expect(component.committeeForm.controls.legalFormId.value).toBe(null);
                expect(component.committeeForm.controls.legalFormId.disabled).toBe(true);
            });

            it('should call updateValidityForForm', () => {
                expect(updateValidityForFormSpy).toHaveBeenCalled();
            });
        });

        describe('Administration Committee (Verwaltungskommissionen)', () => {
            beforeEach(() => {
                component.committeeForm.controls.committeeTypeId.setValue(configsServiceMock.frontendConfig.entityIds.committeeType.administrationId);
                fixture.detectChanges();
            });

            it('should emit extraParliamentaryCommission as true', () => {
                expect(component.isExtraParliamentaryCommmission.emit).toHaveBeenCalledWith(true);
            });

            it('should set levelId to federalCouncilId and disable it', () => {
                expect(component.committeeForm.controls.levelId.value).toBe(configsServiceMock.frontendConfig.entityIds.committeeLevel.federalCouncilId);
                expect(component.committeeForm.controls.levelId.disabled).toBe(true);
            });

            it('should enable marketOrientated when canEditAll is true', () => {
                expect(component.committeeForm.controls.marketOrientated.enabled).toBe(true);
            });

            it('should set federalInstitution to null and disable it', () => {
                expect(component.committeeForm.controls.federalInstitution.value).toBe(null);
                expect(component.committeeForm.controls.federalInstitution.disabled).toBe(true);
            });

            it('should enable federalLawEstablishment when canEditAll is true', () => {
                expect(component.committeeForm.controls.federalLawEstablishment.enabled).toBe(true);
            });

            it('should enable additionalAuthorityMembers when canEditAll is true', () => {
                expect(component.committeeForm.controls.additionalAuthorityMembers.enabled).toBe(true);
            });

            it('should disable legalFormId and set to null', () => {
                expect(component.committeeForm.controls.legalFormId.value).toBe(null);
                expect(component.committeeForm.controls.legalFormId.disabled).toBe(true);
            });

            it('should call updateValidityForForm', () => {
                expect(updateValidityForFormSpy).toHaveBeenCalled();
            });
        });

        describe('Management Committee (Leitungsorgane)', () => {
            beforeEach(() => {
                component.committeeForm.controls.committeeTypeId.setValue(configsServiceMock.frontendConfig.entityIds.committeeType.managementId);
                fixture.detectChanges();
            });

            it('should emit extraParliamentaryCommission as false', () => {
                expect(component.isExtraParliamentaryCommmission.emit).toHaveBeenCalledWith(false);
            });

            it('should enable levelId when canEditAll is true', () => {
                expect(component.committeeForm.controls.levelId.enabled).toBe(true);
            });

            it('should disable marketOrientated and set to null', () => {
                expect(component.committeeForm.controls.marketOrientated.value).toBe(null);
                expect(component.committeeForm.controls.marketOrientated.disabled).toBe(true);
            });

            it('should enable federalInstitution when canEditAll is true', () => {
                expect(component.committeeForm.controls.federalInstitution.enabled).toBe(true);
            });

            it('should enable federalLawEstablishment when canEditAll is true', () => {
                expect(component.committeeForm.controls.federalLawEstablishment.enabled).toBe(true);
            });

            it('should enable legalFormId when canEditAll is true', () => {
                expect(component.committeeForm.controls.legalFormId.enabled).toBe(true);
            });

            it('should disable additionalAuthorityMembers and set to false', () => {
                expect(component.committeeForm.controls.additionalAuthorityMembers.value).toBe(false);
                expect(component.committeeForm.controls.additionalAuthorityMembers.disabled).toBe(true);
            });

            it('should call updateValidityForForm', () => {
                expect(updateValidityForFormSpy).toHaveBeenCalled();
            });
        });

        describe('Federal Agencies Committee (Vertretungen des Bundes)', () => {
            beforeEach(() => {
                component.committeeForm.controls.committeeTypeId.setValue(configsServiceMock.frontendConfig.entityIds.committeeType.federalAgenciesId);
                fixture.detectChanges();
            });

            it('should emit extraParliamentaryCommission as false', () => {
                expect(component.isExtraParliamentaryCommmission.emit).toHaveBeenCalledWith(false);
            });

            it('should enable levelId when canEditAll is true', () => {
                expect(component.committeeForm.controls.levelId.enabled).toBe(true);
            });

            it('should disable marketOrientated and set to null', () => {
                expect(component.committeeForm.controls.marketOrientated.value).toBe(null);
                expect(component.committeeForm.controls.marketOrientated.disabled).toBe(true);
            });

            it('should enable federalInstitution when canEditAll is true', () => {
                expect(component.committeeForm.controls.federalInstitution.enabled).toBe(true);
            });

            it('should enable legalFormId when canEditAll is true', () => {
                expect(component.committeeForm.controls.legalFormId.enabled).toBe(true);
            });

            it('should disable additionalAuthorityMembers and set to true', () => {
                expect(component.committeeForm.controls.additionalAuthorityMembers.value).toBe(true);
                expect(component.committeeForm.controls.additionalAuthorityMembers.disabled).toBe(true);
            });

            it('should call updateValidityForForm', () => {
                expect(updateValidityForFormSpy).toHaveBeenCalled();
            });
        });

        describe('Federal Agencies Cross Border Committee (Vertretungen des Bundes in grenzüberschreitenden Gremien)', () => {
            beforeEach(() => {
                component.committeeForm.controls.committeeTypeId.setValue(
                    configsServiceMock.frontendConfig.entityIds.committeeType.federalAgenciesCrossBorderId
                );
                fixture.detectChanges();
            });

            it('should emit extraParliamentaryCommission as false', () => {
                expect(component.isExtraParliamentaryCommmission.emit).toHaveBeenCalledWith(false);
            });

            it('should disable marketOrientated and set to null', () => {
                expect(component.committeeForm.controls.marketOrientated.value).toBe(null);
                expect(component.committeeForm.controls.marketOrientated.disabled).toBe(true);
            });

            it('should disable federalInstitution and set to null', () => {
                expect(component.committeeForm.controls.federalInstitution.value).toBe(null);
                expect(component.committeeForm.controls.federalInstitution.disabled).toBe(true);
            });

            it('should disable additionalAuthorityMembers and set to true', () => {
                expect(component.committeeForm.controls.additionalAuthorityMembers.value).toBe(true);
                expect(component.committeeForm.controls.additionalAuthorityMembers.disabled).toBe(true);
            });

            it('should enable legalFormId when canEditAll is true', () => {
                expect(component.committeeForm.controls.legalFormId.enabled).toBe(true);
            });

            it('should call updateValidityForForm', () => {
                expect(updateValidityForFormSpy).toHaveBeenCalled();
            });
        });

        describe('No committee type selected', () => {
            it('should not call any setter methods when committeeTypeId is empty', () => {
                const setAuthoritySpy = jest.spyOn(component, 'setAuthorityCommitteeFields' as never);
                const setAdministrationSpy = jest.spyOn(component, 'setAdminisistrationCommitteeFields' as never);
                const setManagementSpy = jest.spyOn(component, 'setManagementCommitteeFields' as never);
                const setFederalAgenciesSpy = jest.spyOn(component, 'setFederalAgenciesCommitteeFields' as never);
                const setFederalAgenciesCrossBorderSpy = jest.spyOn(component, 'setFederalAgenciesCrossBorderCommitteeFields' as never);

                component.committeeForm.controls.committeeTypeId.setValue('');
                fixture.detectChanges();

                expect(setAuthoritySpy).not.toHaveBeenCalled();
                expect(setAdministrationSpy).not.toHaveBeenCalled();
                expect(setManagementSpy).not.toHaveBeenCalled();
                expect(setFederalAgenciesSpy).not.toHaveBeenCalled();
                expect(setFederalAgenciesCrossBorderSpy).not.toHaveBeenCalled();
            });

            it('should not call any setter methods when committeeTypeId is null', () => {
                const setAuthoritySpy = jest.spyOn(component, 'setAuthorityCommitteeFields' as never);
                const setAdministrationSpy = jest.spyOn(component, 'setAdminisistrationCommitteeFields' as never);
                const setManagementSpy = jest.spyOn(component, 'setManagementCommitteeFields' as never);
                const setFederalAgenciesSpy = jest.spyOn(component, 'setFederalAgenciesCommitteeFields' as never);
                const setFederalAgenciesCrossBorderSpy = jest.spyOn(component, 'setFederalAgenciesCrossBorderCommitteeFields' as never);

                component.committeeForm.controls.committeeTypeId.setValue('');
                fixture.detectChanges();

                expect(setAuthoritySpy).not.toHaveBeenCalled();
                expect(setAdministrationSpy).not.toHaveBeenCalled();
                expect(setManagementSpy).not.toHaveBeenCalled();
                expect(setFederalAgenciesSpy).not.toHaveBeenCalled();
                expect(setFederalAgenciesCrossBorderSpy).not.toHaveBeenCalled();
            });
        });

        describe('Unknown committee type', () => {
            it('should not call any setter methods for unknown committee type', () => {
                const setAuthoritySpy = jest.spyOn(component, 'setAuthorityCommitteeFields' as never);
                const setAdministrationSpy = jest.spyOn(component, 'setAdminisistrationCommitteeFields' as never);
                const setManagementSpy = jest.spyOn(component, 'setManagementCommitteeFields' as never);
                const setFederalAgenciesSpy = jest.spyOn(component, 'setFederalAgenciesCommitteeFields' as never);
                const setFederalAgenciesCrossBorderSpy = jest.spyOn(component, 'setFederalAgenciesCrossBorderCommitteeFields' as never);

                component.committeeForm.controls.committeeTypeId.setValue('unknown-committee-type-id');
                fixture.detectChanges();

                expect(setAuthoritySpy).not.toHaveBeenCalled();
                expect(setAdministrationSpy).not.toHaveBeenCalled();
                expect(setManagementSpy).not.toHaveBeenCalled();
                expect(setFederalAgenciesSpy).not.toHaveBeenCalled();
                expect(setFederalAgenciesCrossBorderSpy).not.toHaveBeenCalled();
            });
        });
    });

    describe('vacanciesInCurrentTermOfOffice', () => {
        it('should return null if selectedCommitteeLevelId is not federalCouncilId', () => {
            component.committeeForm.controls.levelId.setValue('NOT_FEDERAL');

            fixture.detectChanges();

            expect(component.vacanciesInCurrentTermOfOffice()).toBeNull();
        });

        it('should return correct vacancies when membersCount is below minimum', () => {
            component.committeeForm.controls.levelId.setValue('federalCouncilId');
            component.committeeForm.controls.minimalMembers.setValue(5);
            component.committeeModification.set({membersCount: 3} as GeneralElectionCommitteeUpdate);

            fixture.detectChanges();

            expect(component.vacanciesInCurrentTermOfOffice()).toBe(2);
        });

        it('should return 0 when membersCount is above minimum', () => {
            component.committeeForm.controls.levelId.setValue('federalCouncilId');
            component.committeeForm.controls.minimalMembers.setValue(5);
            component.committeeModification.set({membersCount: 7} as GeneralElectionCommitteeUpdate);

            fixture.detectChanges();

            expect(component.vacanciesInCurrentTermOfOffice()).toBe(0);
        });

        it('should return 0 when membersCount equals minimum', () => {
            component.committeeForm.controls.levelId.setValue('federalCouncilId');
            component.committeeForm.controls.minimalMembers.setValue(5);
            component.committeeModification.set({membersCount: 5} as GeneralElectionCommitteeUpdate);

            fixture.detectChanges();

            expect(component.vacanciesInCurrentTermOfOffice()).toBe(0);
        });

        it('should handle undefined membersCount and minimalMembers', () => {
            component.committeeForm.controls.levelId.setValue('federalCouncilId');
            component.committeeForm.controls.minimalMembers.setValue(undefined);
            component.committeeModification.set({} as GeneralElectionCommitteeUpdate);

            fixture.detectChanges();

            expect(component.vacanciesInCurrentTermOfOffice()).toBe(0);
        });
    });
});
