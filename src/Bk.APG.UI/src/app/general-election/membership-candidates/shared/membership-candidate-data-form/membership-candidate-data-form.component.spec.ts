import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MembershipCandidateUpdate} from '@api/MembershipCandidateUpdate';
import {ErrorService} from '@shared/error-service.service';
import {MasterDataService} from '@shared/master-data.service';
import {ConfigsService} from '../../../../configs.service';
import {federalDutyJustificationTexts} from '../../../../memberships/shared/federal-duty-justification';
import {PersonsService} from '../../../../persons/persons.service';
import {MembershipCandidateDataFormComponent} from './membership-candidate-data-form.component';

describe('MembershipCandidateDataFormComponent', () => {
    let component: MembershipCandidateDataFormComponent;
    let fixture: ComponentFixture<MembershipCandidateDataFormComponent>;

    const masterDataServiceMock = {
        electionOffices: signal([]),
        functions: signal([]),
        genders: signal([]),
        languages: signal([]),
        electionTypes: signal([]),
        membershipAdditions: signal([]),
    } as unknown as Partial<MasterDataService>;

    const errorServiceMock = {
        getControlError: jest.fn(),
    };

    const personsServiceMock = {
        getPersonDetails: jest.fn(),
    };

    const configsServiceMock = {
        frontendConfig: {
            entityIds: {
                gender: {
                    femaleId: 'femaleId',
                },
                electionOffice: {
                    federalGovernmentId: 'federalGovernmentId',
                },
            },
        },
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MembershipCandidateDataFormComponent],
            providers: [
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: ErrorService, useValue: errorServiceMock},
                {provide: PersonsService, useValue: personsServiceMock},
                {provide: ConfigsService, useValue: configsServiceMock},
            ],
        })
            .overrideTemplateUsingTestingModule(MembershipCandidateDataFormComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(MembershipCandidateDataFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it.each([
        ['can edit', true],
        ['cannot edit', false],
    ])('should %s endDate and functionId according to canEditEndDate', (_, canEditEndDate) => {
        component.membershipCandidateModification.set({
            canEditEndDate,
        } as MembershipCandidateUpdate);

        fixture.detectChanges();

        expect(component.membershipCandidateForm.controls.endDate.enabled).toBe(canEditEndDate);
        expect(component.membershipCandidateForm.controls.functionId.enabled).toBe(canEditEndDate);
    });

    it.each([
        ['can edit', true],
        ['cannot edit', false],
    ])('should %s beginDate according to canEditBeginDate', (_, canEditBeginDate) => {
        component.membershipCandidateModification.set({
            canEditBeginDate,
        } as MembershipCandidateUpdate);

        component.membershipCandidateForm.controls.beginDate.disable();

        fixture.detectChanges();

        expect(component.membershipCandidateForm.controls.beginDate.enabled).toBe(canEditBeginDate);
    });

    it('should set the German automatic justification regardless of term length', () => {
        component.generalElectionCommittee.set({
            extraParliamentaryCommission: true,
            isValidated: false,
            committeeTypeId: 'f2e2af70-d1d4-42b5-b23a-793cbc220064',
        } as never);
        component.membershipCandidateModification.set({
            estimatedTermOfOffice: 0,
            inCorrelationWithFederalDuty: true,
        } as MembershipCandidateUpdate);
        component.membershipCandidateForm.controls.justificationLongerDuty.setValue('Existing justification');

        component.onFederalDutyCheckboxChange({checked: true} as never);

        expect(component.membershipCandidateForm.controls.justificationLongerDuty.value).toBe(federalDutyJustificationTexts.de);
        expect(component.membershipCandidateForm.controls.justificationLongerDuty.disabled).toBe(true);
    });

    it('should clear the German automatic justification when federal duty is disabled', () => {
        component.generalElectionCommittee.set({
            extraParliamentaryCommission: true,
            isValidated: false,
            committeeTypeId: '0a4b7f1d-d8bf-4932-bece-dd2a51cc2d59',
        } as never);
        component.membershipCandidateModification.set({
            estimatedTermOfOffice: 13,
            inCorrelationWithFederalDuty: true,
        } as MembershipCandidateUpdate);
        component.onFederalDutyCheckboxChange({checked: true} as never);

        component.membershipCandidateForm.controls.inCorrelationWithFederalDuty.setValue(false);
        component.onFederalDutyCheckboxChange({checked: false} as never);

        expect(component.membershipCandidateForm.controls.justificationLongerDuty.value).toBe('');
        expect(component.membershipCandidateForm.controls.justificationLongerDuty.enabled).toBe(true);
        expect(component.membershipCandidateForm.controls.justificationLongerDuty.hasError('required')).toBe(true);
    });

    it('should not apply the automatic justification for other committee types', () => {
        component.generalElectionCommittee.set({
            extraParliamentaryCommission: true,
            isValidated: false,
            committeeTypeId: 'other',
        } as never);
        component.membershipCandidateModification.set({
            estimatedTermOfOffice: 13,
            inCorrelationWithFederalDuty: true,
        } as MembershipCandidateUpdate);
        component.membershipCandidateForm.controls.justificationLongerDuty.setValue('Existing justification');

        component.onFederalDutyCheckboxChange({checked: true} as never);

        expect(component.membershipCandidateForm.controls.justificationLongerDuty.value).toBe('Existing justification');
        expect(component.membershipCandidateForm.controls.justificationLongerDuty.enabled).toBe(true);
    });
});
