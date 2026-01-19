import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatButtonModule} from '@angular/material/button';
import {MatDialog} from '@angular/material/dialog';
import {MatIconModule} from '@angular/material/icon';
import {MatTableModule} from '@angular/material/table';
import {MatTooltipModule} from '@angular/material/tooltip';
import {ActivatedRoute, ActivatedRouteSnapshot, convertToParamMap, ParamMap, Router} from '@angular/router';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {LangChangeEvent, TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObAlertModule, ObNotificationService} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {MockComponent, MockModule, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, Subject} from 'rxjs';
import {ConfigsService} from '../../../../configs.service';
import {CommitteeDetailsService} from '../../committee-details.service';
import {ContactPointsService} from '../contact-points.service';
import {ContactPointListComponent} from './contact-point-list.component';

describe('ContactPointListComponent', () => {
    let component: ContactPointListComponent;
    let fixture: ComponentFixture<ContactPointListComponent>;
    let contactPointsServiceMock: Partial<ContactPointsService>;
    let committeeDetailsServiceMock: Partial<CommitteeDetailsService>;
    let reloadSubject: Subject<void>;
    let paramMapSubject: BehaviorSubject<ParamMap>;
    let routeMock: Partial<ActivatedRoute>;
    let routerMock: Partial<Router>;
    let onLangChangeSubject: Subject<LangChangeEvent>;
    let matDialogMock: Partial<MatDialog>;
    let notificationServiceMock: Partial<ObNotificationService>;

    beforeEach(async () => {
        reloadSubject = new Subject<void>();

        onLangChangeSubject = new Subject();
        const translateServiceMock = {
            onLangChange: onLangChangeSubject.asObservable(),
            currentLang: 'de',
            instant: jest.fn(),
        };

        contactPointsServiceMock = {
            getContactPointList: jest.fn().mockReturnValue(of({id: '123'})),
            reload$: reloadSubject,
            deleteContactPoint: jest.fn().mockReturnValue(of({})),
        };

        committeeDetailsServiceMock = {
            committeeDetails: signal<CommitteeDetails>({canEdit: true} as CommitteeDetails),
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

        matDialogMock = {
            open: jest.fn().mockReturnValue({
                afterClosed: jest.fn().mockReturnValue(of(true)),
            }),
        };

        notificationServiceMock = {
            success: jest.fn(),
            error: jest.fn(),
        };

        const configsServiceMock = {
            frontendConfig: {
                entityIds: {
                    contactPoint: {
                        secretariatId: 'secretariatId',
                        dpoId: 'dpoId',
                    },
                },
            },
        };
        await TestBed.configureTestingModule({
            imports: [
                MockPipe(TranslatePipe),
                MockModule(MatTableModule),
                MockModule(MatIconModule),
                MockModule(MatTooltipModule),
                MockModule(MatButtonModule),
                MockModule(ObAlertModule),
                MockComponent(ConfirmDialogComponent),
                ContactPointListComponent,
            ],
            providers: [
                {provide: ContactPointsService, useValue: contactPointsServiceMock},
                {provide: ActivatedRoute, useValue: routeMock},
                {provide: Router, useValue: routerMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: CommitteeDetailsService, useValue: committeeDetailsServiceMock},
                {provide: MatDialog, useValue: matDialogMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: ConfigsService, useValue: configsServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(ContactPointListComponent);
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

    it('should set canEdit', () => {
        expect(component.canEdit()).toBe(true);
    });

    it('should initialize committeeId from route params', () => {
        expect(component.committeeId).toBe('123');
    });

    it('should call getCommitteeDetails on init', () => {
        expect(contactPointsServiceMock.getContactPointList).toHaveBeenCalledWith('123');
    });

    it('should update committeeId when route param changes', () => {
        routeMock.snapshot!.paramMap.get = jest.fn().mockReturnValue('456');
        paramMapSubject.next({
            get: jest.fn().mockReturnValue('456'),
        } as unknown as ParamMap);

        expect(component.committeeId).toBe('456');
        expect(contactPointsServiceMock.getContactPointList).toHaveBeenCalledWith('456');
    });

    it('should react to language changes', () => {
        onLangChangeSubject.next({lang: 'fr'} as unknown as LangChangeEvent);

        expect(contactPointsServiceMock.getContactPointList).toHaveBeenCalledTimes(2);
        expect(contactPointsServiceMock.getContactPointList).toHaveBeenCalledWith('123');
    });

    it('should navigate on edit', () => {
        component.editContactPoint('234');

        expect(routerMock.navigate).toHaveBeenCalledTimes(1);
    });

    it('should navigate on copy', () => {
        component.copyContactPoint('234');

        expect(routerMock.navigate).toHaveBeenCalledTimes(1);
    });

    it('should delete a contact point after confirmation', () => {
        component.deleteContactPoint('234');

        expect(matDialogMock.open as jest.Mock).toHaveBeenCalledTimes(1);
        expect(contactPointsServiceMock.deleteContactPoint as jest.Mock).toHaveBeenCalledWith('234');
        expect(notificationServiceMock.success as jest.Mock).toHaveBeenCalledWith('contactPoint.delete.success');
    });

    it('should navigate on create', () => {
        component.createContactPoint();

        expect(routerMock.navigate).toHaveBeenCalledTimes(1);
    });
});
