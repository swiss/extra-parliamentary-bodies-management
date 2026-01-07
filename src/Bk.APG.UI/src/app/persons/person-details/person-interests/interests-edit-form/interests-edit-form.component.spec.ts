import {NO_ERRORS_SCHEMA} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {FormBuilder, ReactiveFormsModule, UntypedFormArray, UntypedFormGroup} from '@angular/forms';
import {MatIcon} from '@angular/material/icon';
import {InterestUpdate} from '@api/InterestUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {MockComponents, MockPipe} from 'ng-mocks';
import {of} from 'rxjs';
import {InterestsEditFormDetailComponent} from '../interests-edit-form-detail/interests-edit-form-detail.component';
import {PersonInterestsService} from '../person-interests.service';
import {InterestsEditFormComponent} from './interests-edit-form.component';

describe('InterestsEditFormComponent', () => {
    let component: InterestsEditFormComponent;
    let fixture: ComponentFixture<InterestsEditFormComponent>;
    let formBuilder: FormBuilder;
    let formArray: UntypedFormArray;

    const interests: InterestUpdate[] = [
        {
            id: '1',
            personId: 'p1',
            interestText: 'Beta AG',
            text: 'Beta',
            interestLegalFormId: 'legal1',
            interestFunctionId: 'func1',
            interestCommitteeId: 'comm1',
            legalFormId: 'lf1',
            uidOrganisationId: 'org1',
            rowVersion: 123,
            isInactive: false,
            isUid: false,
        },
        {
            id: '2',
            personId: 'p1',
            interestText: 'Alpha AG',
            text: 'Alpha',
            interestLegalFormId: 'legal2',
            interestFunctionId: 'func2',
            interestCommitteeId: 'comm2',
            legalFormId: 'lf2',
            uidOrganisationId: 'org2',
            rowVersion: 456,
            isInactive: true,
            isUid: true,
        },
    ];

    const personInterestsServiceMock = {
        getInterestsByPersonId: jest.fn().mockReturnValue(of(interests)),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ReactiveFormsModule, MockPipe(TranslatePipe), InterestsEditFormComponent, MockComponents(MatIcon, InterestsEditFormDetailComponent)],
            providers: [FormBuilder, {provide: PersonInterestsService, useValue: personInterestsServiceMock}],
            schemas: [NO_ERRORS_SCHEMA],
        }).compileComponents();

        formBuilder = TestBed.inject(FormBuilder);
        formArray = formBuilder.array([]);
        fixture = TestBed.createComponent(InterestsEditFormComponent);
        component = fixture.componentInstance;
        component.formArray = formArray;
        component.personId = 'p1';
        component.ngOnChanges({personId: {currentValue: 'p1', previousValue: null, firstChange: true, isFirstChange: () => true}});
        fixture.detectChanges();
        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('reset method', () => {
        it('should repopulate formArray with the original interests', async () => {
            component.formArray.clear();
            expect(component.formArray.length).toBe(0);
            component.reset();
            fixture.detectChanges();
            expect(component.formArray.length).toBe(2);
            interests.forEach((intUpdate, index) => {
                const fg = component.formArray.at(index) as UntypedFormGroup;
                expect(fg.get('id')?.value).toBe(intUpdate.id);
                expect(fg.get('text')?.value).toBe(intUpdate.text);
                expect(fg.get('interestCommitteeId')?.value).toBe(intUpdate.interestCommitteeId);
                expect(fg.get('interestFunctionId')?.value).toBe(intUpdate.interestFunctionId);
                expect(fg.get('interestLegalFormId')?.value).toBe(intUpdate.interestLegalFormId);
                expect(fg.get('legalFormId')?.value).toBe(intUpdate.legalFormId);
                expect(fg.get('uidOrganisationId')?.value).toBe(intUpdate.uidOrganisationId);
            });
        });
    });

    describe('addInterest', () => {
        it('should add an empty interest and mark form as dirty and touched', () => {
            const initialLength = component.formArray.length;
            component.addInterest();
            expect(component.formArray.length).toBe(initialLength + 1);
            expect(component.formArray.dirty).toBe(true);
            expect(component.formArray.touched).toBe(true);
            const newGroup = component.formArray.at(component.formArray.length - 1) as UntypedFormGroup;
            expect(newGroup.get('text')?.value).toBeNull();
            expect(newGroup.get('interestCommitteeId')?.value).toBeNull();
        });
    });

    describe('removeInterest', () => {
        it('should remove an interest and mark form as dirty and touched', () => {
            const initialLength = component.formArray.length;
            component.removeInterest(0);
            expect(component.formArray.length).toBe(initialLength - 1);
            expect(component.formArray.dirty).toBe(true);
            expect(component.formArray.touched).toBe(true);
        });
    });
});
