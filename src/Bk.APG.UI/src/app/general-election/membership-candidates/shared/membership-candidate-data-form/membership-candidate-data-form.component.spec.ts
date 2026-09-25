import {signal} from '@angular/core';
import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {MembershipCandidateTermCalculation} from '@api/MembershipCandidateTermCalculation';
import {MembershipCandidateUpdate} from '@api/MembershipCandidateUpdate';
import {ErrorService} from '@shared/error-service.service';
import {MasterDataService} from '@shared/master-data.service';
import {of} from 'rxjs';
import {ConfigsService} from '../../../../configs.service';
import {federalDutyJustificationTexts} from '../../../../memberships/shared/federal-duty-justification';
import {PersonsService} from '../../../../persons/persons.service';
import {MembershipCandidateService} from '../../membership-candidate-service';
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

    const membershipCandidateServiceMock = {
        calculateMembershipCandidateTerm: jest.fn(),
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
                {provide: MembershipCandidateService, useValue: membershipCandidateServiceMock},
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

    describe('term of office recalculation', () => {
        beforeEach(() => {
            component.membershipCandidateModification.set({
                id: '1',
                beginDate: new Date(2018, 1, 1),
                endDate: new Date(2022, 1, 1),
                canEditBeginDate: true,
                canEditEndDate: true,
            } as MembershipCandidateUpdate);
            fixture.detectChanges();
        });

        it('should call calculateMembershipCandidateTerm and update term values when endDate changes', fakeAsync(() => {
            const term: MembershipCandidateTermCalculation = {currentTermOfOffice: 4, estimatedTermOfOffice: 8};
            membershipCandidateServiceMock.calculateMembershipCandidateTerm.mockReturnValue(of(term));

            const newEndDate = new Date(2033, 1, 1);
            component.membershipCandidateForm.controls.beginDate.setValue(new Date(2020, 1, 1));
            component.membershipCandidateForm.controls.endDate.setValue(newEndDate);
            tick(300);

            expect(membershipCandidateServiceMock.calculateMembershipCandidateTerm).toHaveBeenCalledWith('1', new Date(2020, 1, 1), newEndDate);
            expect(component.membershipCandidateModification()?.currentTermOfOffice).toEqual(4);
            expect(component.membershipCandidateModification()?.estimatedTermOfOffice).toEqual(8);
        }));

        it('should not overwrite the changed date after the term values are updated', fakeAsync(() => {
            const term: MembershipCandidateTermCalculation = {currentTermOfOffice: 4, estimatedTermOfOffice: 8};
            membershipCandidateServiceMock.calculateMembershipCandidateTerm.mockReturnValue(of(term));

            const newEndDate = new Date(2033, 1, 1);
            component.membershipCandidateForm.controls.endDate.setValue(newEndDate);
            tick(300);

            expect(component.membershipCandidateForm.controls.endDate.value).toEqual(newEndDate);
        }));

        it('should not call calculateMembershipCandidateTerm when the form is invalid', fakeAsync(() => {
            component.membershipCandidateForm.controls.endDate.setValue(undefined);
            tick(300);

            expect(membershipCandidateServiceMock.calculateMembershipCandidateTerm).not.toHaveBeenCalled();
        }));

        it('should not call calculateMembershipCandidateTerm when the candidate has no id yet', fakeAsync(() => {
            component.membershipCandidateModification.set({
                canEditBeginDate: true,
                canEditEndDate: true,
            } as MembershipCandidateUpdate);
            fixture.detectChanges();

            component.membershipCandidateForm.controls.endDate.setValue(new Date(2033, 1, 1));
            tick(300);

            expect(membershipCandidateServiceMock.calculateMembershipCandidateTerm).not.toHaveBeenCalled();
        }));

        it('should call calculateMembershipCandidateTerm when beginDate is disabled but endDate changes', fakeAsync(() => {
            const term: MembershipCandidateTermCalculation = {currentTermOfOffice: 4, estimatedTermOfOffice: 8};
            membershipCandidateServiceMock.calculateMembershipCandidateTerm.mockReturnValue(of(term));

            component.membershipCandidateModification.set({
                id: '1',
                beginDate: new Date(2018, 1, 1),
                endDate: new Date(2022, 1, 1),
                canEditBeginDate: false,
                canEditEndDate: true,
            } as MembershipCandidateUpdate);
            fixture.detectChanges();

            component.membershipCandidateForm.controls.endDate.setValue(new Date(2033, 1, 1));
            tick(300);

            expect(membershipCandidateServiceMock.calculateMembershipCandidateTerm).toHaveBeenCalled();
        }));
    });
});
