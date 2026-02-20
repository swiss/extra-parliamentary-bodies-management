import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatDialog} from '@angular/material/dialog';
import {Router} from '@angular/router';
import {FormLettersSenderList} from '@api/FormLettersSenderList';
import {TranslateService} from '@ngx-translate/core';
import {of, Subject} from 'rxjs';
import {FormLettersSenderComponent} from './form-letters-sender.component';
import {FormLettersSenderService} from './form-letters-sender.service';

describe('FormLettersSenderComponent', () => {
    let component: FormLettersSenderComponent;
    let fixture: ComponentFixture<FormLettersSenderComponent>;

    const languageChangeSubject = new Subject<{lang: string}>();
    const reloadSubject = new Subject<void>();

    const senders: FormLettersSenderList[] = [
        {
            id: 'sender-1',
            description: 'Sender 1',
            surname: 'Doe',
            givenName: 'Jane',
            senderFunction: 'Secretary',
            department: 'Dept A',
        },
    ];

    const formLettersSenderServiceMock = {
        reload$: reloadSubject,
        getFormLettersSenderList: jest.fn().mockReturnValue(of(senders)),
        deleteFormLettersSender: jest.fn().mockReturnValue(of(undefined)),
    } as unknown as jest.Mocked<FormLettersSenderService>;

    const translateServiceMock = {
        onLangChange: languageChangeSubject,
        getCurrentLang: jest.fn().mockReturnValue('de'),
        instant: jest.fn().mockReturnValue('Delete message'),
    } as unknown as jest.Mocked<TranslateService>;

    const routerMock = {
        navigate: jest.fn().mockResolvedValue(true),
    } as unknown as jest.Mocked<Router>;

    const dialogMock = {
        open: jest.fn(),
    } as unknown as jest.Mocked<MatDialog>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [FormLettersSenderComponent],
            providers: [
                {provide: FormLettersSenderService, useValue: formLettersSenderServiceMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: MatDialog, useValue: dialogMock},
            ],
        })
            .overrideComponent(FormLettersSenderComponent, {
                set: {
                    template: '',
                },
            })
            .compileComponents();

        fixture = TestBed.createComponent(FormLettersSenderComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should load senders on init', () => {
        expect(formLettersSenderServiceMock.getFormLettersSenderList).toHaveBeenCalledTimes(1);
        expect(component.dataSource.data).toEqual(senders);
    });

    it('should reload senders on reload event', () => {
        reloadSubject.next();

        expect(formLettersSenderServiceMock.getFormLettersSenderList).toHaveBeenCalledTimes(2);
        expect(component.dataSource.data).toEqual(senders);
    });

    it('should navigate to edit sender route', () => {
        component.editSender('sender-1');

        expect(routerMock.navigate).toHaveBeenCalledWith(['general-election', 'exports', 'formLettersSenders', 'sender-1']);
    });

    it('should navigate to create sender route', () => {
        component.createSender();

        expect(routerMock.navigate).toHaveBeenCalledWith(['general-election', 'exports', 'formLettersSenders', 'create']);
    });

    it('should delete sender and trigger reload when confirmed', () => {
        const sender = senders[0];
        const reloadNextSpy = jest.spyOn(formLettersSenderServiceMock.reload$, 'next');
        dialogMock.open.mockReturnValue({afterClosed: () => of(true)} as never);

        component.deleteSender(sender);

        expect(translateServiceMock.instant).toHaveBeenCalledWith('formLetter.sender.delete.confirm', {
            sender: sender.description,
        });
        expect(formLettersSenderServiceMock.deleteFormLettersSender).toHaveBeenCalledWith(sender.id);
        expect(reloadNextSpy).toHaveBeenCalledTimes(1);
    });

    it('should not delete sender when dialog is cancelled', () => {
        dialogMock.open.mockReturnValue({afterClosed: () => of(false)} as never);

        component.deleteSender(senders[0]);

        expect(formLettersSenderServiceMock.deleteFormLettersSender).not.toHaveBeenCalled();
    });
});
