import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatTableModule} from '@angular/material/table';
import {ParamMap, ActivatedRoute, Router, convertToParamMap, ActivatedRouteSnapshot} from '@angular/router';
import {LangChangeEvent, TranslatePipe, TranslateService} from '@ngx-translate/core';
import {MockPipe, MockModule} from 'ng-mocks';
import {Subject, BehaviorSubject, of} from 'rxjs';
import {CommitteeTypeService} from '../committee-type.service';
import {CommitteeTypeListComponent} from './committee-type-list.component';

describe('CommitteeTypeListComponent', () => {
    let component: CommitteeTypeListComponent;
    let fixture: ComponentFixture<CommitteeTypeListComponent>;
    let committeeTypeServiceMock: Partial<CommitteeTypeService>;
    let paramMapSubject: BehaviorSubject<ParamMap>;
    let routeMock: Partial<ActivatedRoute>;
    let routerMock: Partial<Router>;
    let onLangChangeSubject: Subject<LangChangeEvent>;

    beforeEach(async () => {
        onLangChangeSubject = new Subject();
        const translateServiceMock = {
            onLangChange: onLangChangeSubject.asObservable(),
            getCurrentLang: jest.fn(() => 'de'),
        };

        committeeTypeServiceMock = {
            getCommitteeTypeList: jest.fn().mockReturnValue(of({id: '123'})),
        };

        paramMapSubject = new BehaviorSubject<ParamMap>(convertToParamMap({id: '123'}));
        routeMock = {
            paramMap: paramMapSubject.asObservable(),
            snapshot: {
                paramMap: convertToParamMap({id: '123'}),
            } as unknown as ActivatedRouteSnapshot,
        };

        routerMock = {
            navigate: jest.fn().mockReturnValue({then: jest.fn()}),
        };

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), MockModule(MatTableModule), CommitteeTypeListComponent],
            providers: [
                {provide: CommitteeTypeService, useValue: committeeTypeServiceMock},
                {provide: ActivatedRoute, useValue: routeMock},
                {provide: Router, useValue: routerMock},
                {provide: TranslateService, useValue: translateServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(CommitteeTypeListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
        await fixture.whenStable();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should call getCommitteeTypeList on init', () => {
        expect(committeeTypeServiceMock.getCommitteeTypeList).toHaveBeenCalled();
    });

    it('should react to language changes', () => {
        onLangChangeSubject.next({lang: 'fr'} as unknown as LangChangeEvent);
        expect(committeeTypeServiceMock.getCommitteeTypeList).toHaveBeenCalledTimes(2);
    });

    it('should navigate on edit', () => {
        component.editCommitteeType('234');

        expect(routerMock.navigate).toHaveBeenCalledTimes(1);
    });
});
