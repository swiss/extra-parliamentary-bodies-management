/* eslint-disable @typescript-eslint/no-explicit-any */
import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {AbstractControl, ControlEvent, FormBuilder, FormGroup, PristineChangeEvent, ReactiveFormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {ActivatedRoute, ActivatedRouteSnapshot, convertToParamMap, ParamMap, Router} from '@angular/router';
import {InterestUpdate} from '@api/InterestUpdate';
import {TranslateModule, TranslateService} from '@ngx-translate/core';
import {ObButtonModule, ObNotificationService, ObUnsavedChangesDirective} from '@oblique/oblique';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {MockComponents, MockDirectives, MockModule} from 'ng-mocks';
import {BehaviorSubject, of, Subject} from 'rxjs';
import {PersonsService} from '../../persons.service';
import {PersonDetailsService} from '../person-details.service';
import {InterestsEditFormComponent} from './interests-edit-form/interests-edit-form.component';
import {PersonInterestsComponent} from './person-interests.component';
import {PersonInterestsService} from './person-interests.service';

describe('PersonInterestsComponent', () => {
    let component: PersonInterestsComponent;
    let fixture: ComponentFixture<PersonInterestsComponent>;

    let currentInterests: InterestUpdate[];
    let formBuilder: FormBuilder;
    let form: FormGroup;
    let eventsSubject: Subject<ControlEvent>;

    const langChangeSubject = new Subject();
    const translateServiceMock = {
        onLangChange: langChangeSubject.asObservable(),
        currentLang: 'de',
    };

    const masterDataServiceMock = {
        getInterestCommittees: jest.fn(),
        getInterestFunctions: jest.fn(),
        getInterestLegalForms: jest.fn(),
        getLegalForms: jest.fn().mockReturnValue(of([])),
    };

    const personInterestsServiceMock = {
        getInterestsByPersonId: jest.fn(),
        saveInterestForPerson: jest.fn(),
    };

    const reloadSubject = new Subject<void>();
    const personsServiceMock = {
        getPersonDetails: jest.fn(),
        reload$: reloadSubject,
    };

    const personDetailsServiceMock = {
        personDetails: signal({}),
        personName: signal(''),
        isInterestsFormDirty: signal(false),
    };

    const notificationServiceMock = {
        success: jest.fn(),
        error: jest.fn(),
    };

    const reloadEntityAuditLogSubject = new Subject<void>();
    const entityAuditLogServiceMock = {
        reload$: reloadEntityAuditLogSubject,
    };

    const paramMapSubject = new BehaviorSubject<ParamMap>(convertToParamMap({id: '123'}));
    const routeMock = {
        paramMap: paramMapSubject.asObservable(),
        snapshot: {
            paramMap: convertToParamMap({id: '123'}),
        } as unknown as ActivatedRouteSnapshot,
    };

    const mockRouter = {navigate: jest.fn()} as unknown as Router;

    beforeEach(async () => {
        eventsSubject = new Subject<ControlEvent>();

        await TestBed.configureTestingModule({
            imports: [
                ReactiveFormsModule,
                MockModule(TranslateModule),
                MockModule(MatButtonModule),
                MockModule(ObButtonModule),
                MockComponents(InterestsEditFormComponent, HelpTooltipComponent),
                PersonInterestsComponent,
                MockDirectives(ObUnsavedChangesDirective),
            ],
            providers: [
                {provide: PersonInterestsService, useValue: personInterestsServiceMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: PersonsService, useValue: personsServiceMock},
                {provide: PersonDetailsService, useValue: personDetailsServiceMock},
                {provide: Router, useValue: mockRouter},
                {provide: ActivatedRoute, useValue: routeMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: EntityAuditLogService, useValue: entityAuditLogServiceMock},
            ],
        }).compileComponents();

        currentInterests = [
            {
                id: 'myId1',
                personId: 'personId1',
                interestText: 'myText1 AG',
                text: 'myText1',
                interestLegalFormId: 'a',
                interestFunctionId: 'b',
                interestCommitteeId: 'c',
                legalFormId: 'd',
                uidOrganisationId: '123',
                rowVersion: 123,
                isInactive: false,
                isUid: false,
            },
            {
                id: 'myId2',
                personId: 'personId2',
                interestText: 'myText2 AG',
                text: 'myText2',
                interestLegalFormId: 'b',
                interestFunctionId: 'c',
                interestCommitteeId: 'a',
                legalFormId: 'f',
                uidOrganisationId: '342',
                rowVersion: 456,
                isInactive: true,
                isUid: true,
            },
        ];

        formBuilder = TestBed.inject(FormBuilder);
        form = formBuilder.group({interests: formBuilder.array(currentInterests)});

        personInterestsServiceMock.getInterestsByPersonId.mockReturnValue(of(currentInterests));
        personInterestsServiceMock.saveInterestForPerson.mockReturnValue(of(currentInterests));

        fixture = TestBed.createComponent(PersonInterestsComponent);
        component = fixture.componentInstance;

        component.form = form;
        (component.form.controls.interests as any).events = eventsSubject.asObservable();

        fixture.detectChanges();
    });

    afterEach(() => {
        jest.resetAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should initialize form, interestsForm, and personId', () => {
        expect(component.form).toBeTruthy();
        expect(component.form.controls.interests).toBeTruthy();
        expect(component.personId).toBe('123');
    });

    it('reset should call reset on interestsEditFormComponent and mark interestsForm as pristine and untouched', () => {
        const editFormComponentMock = {reset: jest.fn()};
        // @ts-ignore
        component.interestsEditFormComponent = () => editFormComponentMock;

        const markAsPristineSpy = jest.spyOn(component.form, 'markAsPristine');
        const markAsUntouchedSpy = jest.spyOn(component.form, 'markAsUntouched');

        component.reset();

        expect(editFormComponentMock.reset).toHaveBeenCalled();
        expect(markAsPristineSpy).toHaveBeenCalled();
        expect(markAsUntouchedSpy).toHaveBeenCalled();
    });

    it('should call saveInterestForPerson and handle success', async () => {
        const editFormComponentMock = {interests: []};
        // @ts-ignore
        component.interestsEditFormComponent = () => editFormComponentMock;

        const saveSpy = jest.spyOn(personInterestsServiceMock, 'saveInterestForPerson').mockReturnValue(of(currentInterests));
        const reloadSpy = jest.spyOn(personsServiceMock.reload$, 'next');
        const reloadEntityAuditLogSubject = jest.spyOn(entityAuditLogServiceMock.reload$, 'next');
        const navigateSpy = jest.spyOn(mockRouter, 'navigate').mockResolvedValue(true);

        component.saveInterests();

        expect(saveSpy).toHaveBeenCalledWith('123', expect.any(Array));
        expect(reloadSpy).toHaveBeenCalled();
        expect(reloadEntityAuditLogSubject).toHaveBeenCalled();
        await fixture.whenStable();

        expect(navigateSpy).toHaveBeenCalledWith([]);
        expect(notificationServiceMock.success).toHaveBeenCalledWith('interests.save.success');
        expect(editFormComponentMock.interests).toEqual(currentInterests);
    });

    describe('ngOnInit', () => {
        it('should update isDataFormDirty on PristineChangeEvent emission', () => {
            eventsSubject.next(new PristineChangeEvent(false, {} as unknown as AbstractControl));
            expect(personDetailsServiceMock.isInterestsFormDirty()).toEqual(true);
            eventsSubject.next(new PristineChangeEvent(true, {} as unknown as AbstractControl));
            expect(personDetailsServiceMock.isInterestsFormDirty()).toEqual(false);
        });
    });
});
