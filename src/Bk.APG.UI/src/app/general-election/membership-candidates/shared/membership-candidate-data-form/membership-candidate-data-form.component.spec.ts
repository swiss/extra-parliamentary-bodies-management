import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MembershipCandidateUpdate} from '@api/MembershipCandidateUpdate';
import {ErrorService} from '@shared/error-service.service';
import {MasterDataService} from '@shared/master-data.service';
import {ConfigsService} from '../../../../configs.service';
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
});
