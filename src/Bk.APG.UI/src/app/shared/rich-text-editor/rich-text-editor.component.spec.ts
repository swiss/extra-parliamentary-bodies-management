/* eslint-disable @typescript-eslint/no-explicit-any,dot-notation */
import {Component, signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {FormBuilder, ReactiveFormsModule} from '@angular/forms';
import {By} from '@angular/platform-browser';
import {LangChangeEvent, TranslateService} from '@ngx-translate/core';
import {FroalaEditorModule} from 'angular-froala-wysiwyg';
import {MockModule} from 'ng-mocks';
import {BehaviorSubject, Observable} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {ConfigsService} from '../../configs.service';
import {RichTextEditorComponent} from './rich-text-editor.component';

@Component({
    selector: 'apg-test-host',
    template: `
        <form [formGroup]="form">
            <apg-rich-text-editor controlName="name" title="Name" [heightMin]="heightMin()" [heightMax]="heightMax()" [plugins]="['flite', 'lance']" />
        </form>
    `,
    standalone: true,
    imports: [ReactiveFormsModule, RichTextEditorComponent],
})
class TestHostComponent {
    form = new FormBuilder().group({
        name: ['test content'],
        age: [30],
    });

    heightMin = signal<number | undefined>(undefined);
    heightMax = signal<number | undefined>(undefined);
}

describe('RichTextEditorComponent', () => {
    let component: RichTextEditorComponent;
    let fixture: ComponentFixture<TestHostComponent>;

    const configsServiceMock = {
        frontendConfig: {
            froalaKey: 'test-key',
        },
    };

    const authServiceMock = {
        userInfo$: new BehaviorSubject({}),
    };

    let langChangeSub: BehaviorSubject<LangChangeEvent>;
    let translateServiceMock: {onLangChange: Observable<LangChangeEvent>; getCurrentLang: () => string};

    beforeEach(async () => {
        langChangeSub = new BehaviorSubject<LangChangeEvent>({lang: 'fr', translations: {}});
        translateServiceMock = {
            onLangChange: langChangeSub.asObservable(),
            getCurrentLang: () => 'fr',
        };

        await TestBed.configureTestingModule({
            imports: [TestHostComponent, ReactiveFormsModule, MockModule(FroalaEditorModule), RichTextEditorComponent],
            providers: [
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: ConfigsService, useValue: configsServiceMock},
                {provide: AuthService, useValue: authServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(TestHostComponent);
        component = fixture.debugElement.query(By.directive(RichTextEditorComponent)).componentInstance;

        Object.defineProperty(component, 'editorDirective', {
            value: () => ({
                _editor: {
                    edit: {
                        on: jest.fn(),
                        off: jest.fn(),
                    },
                },
            }),
        });

        jest.spyOn(component as any, 'monitorControlStatus').mockImplementation(() => {});
        jest.spyOn(component as any, 'reloadEditor').mockImplementation(() => {});
        jest.spyOn(component as any, 'enableOrDisableEditor').mockImplementation(() => {});

        fixture.detectChanges();
        await fixture.whenStable();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should initialize with input values', () => {
        expect(component.controlName()).toBe('name');
        expect(component.title()).toBe('Name');
    });

    it('should have default height values', () => {
        expect(component.heightMin()).toBeUndefined();
        expect(component.heightMax()).toBeUndefined();
    });

    it('should set language from translate service', () => {
        expect(component.language()).toBe('fr');

        langChangeSub.next({lang: 'de', translations: {}});
        fixture.detectChanges();

        expect(component.language()).toBe('de');
    });

    it('should correctly generate options based on inputs', () => {
        const expectedOptions = {
            attribution: false,
            charCounterCount: false,
            events: {
                'flite:init': expect.any(Function),
                'lance::init': expect.any(Function),
            },
            flite: {
                allowQuitWithChanges: true,
            },
            lance: {
                undoPolicy: 'all',
                useTextSelection: 'all',
            },
            heightMax: 9999,
            heightMin: 100,
            immediatelyUpdateAngularComponent: true,
            key: 'test-key',
            language: 'fr',
            placeholderText: '',
            pluginsEnabled: ['flite', 'lance'],
            toolbarButtons: {
                moreText: {
                    buttons: ['bold', 'italic', 'subscript', 'superscript'],
                    buttonsVisible: 10,
                },
                moreParagraph: {
                    buttons: [
                        'flite-toggletracking',
                        'flite-toggleshow',
                        'flite-acceptall',
                        'flite-rejectall',
                        'flite-acceptone',
                        'flite-rejectone',
                        'annotate',
                    ],
                    buttonsVisible: 10,
                },
                moreMisc: {
                    buttons: ['undo', 'redo'],
                    align: 'right',
                    buttonsVisible: 4,
                },
            },
            wordPasteKeepFormatting: false,
            wordPasteModal: false,
        };

        expect(component.options()).toEqual(expectedOptions);
    });

    it('should update options with custom heights', () => {
        fixture.componentInstance.heightMin.set(200);
        fixture.componentInstance.heightMax.set(800);
        fixture.detectChanges();

        expect(component.options()).toEqual(
            expect.objectContaining({
                heightMin: 200,
                heightMax: 800,
            })
        );
    });

    it('should track updating state', () => {
        expect(component.isUpdating()).toBe(false);
    });

    it('should set up lifecycle hooks and initialize component correctly', () => {
        expect(component['monitorControlStatus']).toHaveBeenCalled();
        expect(component['reloadEditor']).toHaveBeenCalled();
        expect(component['enableOrDisableEditor']).toHaveBeenCalled();
    });

    describe('flite plugin', () => {
        const mockAddDictionary = jest.fn();
        const mockSetLanguage = jest.fn();
        const mockSetUserInfo = jest.fn();

        const mockRefresh = jest.fn();
        const mockEditor = {
            toolbar: {
                refresh: mockRefresh,
            },
        };

        beforeEach(() => {
            const mockFliteEvent = {
                flite: {
                    setLanguage: mockSetLanguage,
                    addDictionary: mockAddDictionary,
                    setUserInfo: mockSetUserInfo,
                },
            };

            const options = component.options() as any;
            options.events['flite:init'](mockFliteEvent);

            Object.defineProperty(component, 'editorDirective', {
                value: () => ({
                    _editor: mockEditor,
                }),
            });
        });

        it('should call event.flite.setUserInfo when flite:init event is triggered', () => {
            const userInfo = {
                email: 'test@example.com',
                firstName: 'John',
                lastName: 'Doe',
            };

            authServiceMock.userInfo$.next(userInfo);

            expect(mockSetUserInfo).toHaveBeenCalledWith({
                id: 'test@example.com',
                name: 'John Doe',
            });
        });

        it('should refresh toolbar when flite:init event is triggered', () => {
            expect(mockRefresh).toHaveBeenCalled();
        });

        it('should set language on flite:init event', () => {
            expect(mockAddDictionary).toHaveBeenCalledWith('it', {});
            expect(mockSetLanguage).toHaveBeenCalledWith('fr');
        });
    });

    describe('lance plugin', () => {
        const mockCreateAnnotationsUI = jest.fn();
        const mockRefresh = jest.fn();
        const mockGetAnnotations = jest.fn();
        const mockAddUsers = jest.fn();
        const mockSetUserId = jest.fn();

        const mockAnnotations = {
            addUsers: mockAddUsers,
            setUserId: mockSetUserId,
        };

        const mockLanceUI = {
            init: jest.fn(),
        };

        const mockEditor = {
            toolbar: {
                refresh: mockRefresh,
            },
        };

        let lanceEventHandler: any;

        beforeEach(() => {
            mockCreateAnnotationsUI.mockReturnValue(mockLanceUI);
            mockGetAnnotations.mockReturnValue(mockAnnotations);

            const options = component.options() as any;
            lanceEventHandler = options.events['lance::init'];

            Object.defineProperty(component, 'editorDirective', {
                value: () => ({
                    _editor: mockEditor,
                }),
            });
        });

        it('should add Italian dictionary when lance::init event is triggered', async () => {
            const mockSetLanguage = jest.fn().mockResolvedValue(undefined);
            const mockAddDictionary = jest.fn();
            const mockLanceUIInit = jest.fn().mockResolvedValue(undefined);
            const mockAddUsers = jest.fn();
            const mockSetUserId = jest.fn();

            const mockLanceEvent = {
                lance: {
                    setLanguage: mockSetLanguage,
                    addDictionary: mockAddDictionary,
                    App: {
                        LANCE: {
                            createAnnotationsUI: jest.fn().mockReturnValue({init: mockLanceUIInit}),
                        },
                    },
                    getAnnotations: jest.fn().mockReturnValue({
                        addUsers: mockAddUsers,
                        setUserId: mockSetUserId,
                    }),
                },
            };

            await lanceEventHandler(mockLanceEvent);

            expect(mockAddDictionary).toHaveBeenCalledWith('it', {});
        });

        it('should set language on lance::init event', async () => {
            const mockLanceEvent = {
                lance: {
                    setLanguage: jest.fn().mockResolvedValue(undefined),
                    addDictionary: jest.fn(),
                    App: {
                        LANCE: {
                            createAnnotationsUI: jest.fn().mockReturnValue({init: jest.fn().mockResolvedValue(undefined)}),
                        },
                    },
                    getAnnotations: jest.fn().mockReturnValue({
                        addUsers: jest.fn(),
                        setUserId: jest.fn(),
                    }),
                },
            };

            await lanceEventHandler(mockLanceEvent);

            expect(mockLanceEvent.lance.setLanguage).toHaveBeenCalledWith('fr');
        });

        it('should create annotations UI with correct configuration when container exists', async () => {
            const mockContainer = document.createElement('div');
            jest.spyOn(fixture.nativeElement, 'querySelector').mockReturnValue(mockContainer);

            const mockLanceEvent = {
                lance: {
                    setLanguage: jest.fn().mockResolvedValue(undefined),
                    addDictionary: jest.fn(),
                    App: {
                        LANCE: {
                            createAnnotationsUI: jest.fn().mockReturnValue({init: jest.fn().mockResolvedValue(undefined)}),
                        },
                    },
                    getAnnotations: jest.fn().mockReturnValue({
                        addUsers: jest.fn(),
                        setUserId: jest.fn(),
                    }),
                },
            };

            await lanceEventHandler(mockLanceEvent);

            expect(mockLanceEvent.lance.App.LANCE.createAnnotationsUI).toHaveBeenCalledWith({type: 'aligned'});
        });

        it('should initialize annotations UI with correct options', async () => {
            const mockContainer = document.createElement('div');
            mockContainer.className = 'annotations-container';
            const mockLanceUIInit = jest.fn().mockResolvedValue(undefined);
            jest.spyOn(fixture.nativeElement, 'querySelector').mockReturnValue(mockContainer);

            const mockLanceEvent = {
                lance: {
                    setLanguage: jest.fn().mockResolvedValue(undefined),
                    addDictionary: jest.fn(),
                    App: {
                        LANCE: {
                            createAnnotationsUI: jest.fn().mockReturnValue({init: mockLanceUIInit}),
                        },
                    },
                    getAnnotations: jest.fn().mockReturnValue({
                        addUsers: jest.fn(),
                        setUserId: jest.fn(),
                    }),
                },
            };

            await lanceEventHandler(mockLanceEvent);

            expect(mockLanceUIInit).toHaveBeenCalledWith({
                owner: mockLanceEvent.lance.getAnnotations(),
                generateUI: true,
                container: mockContainer,
                resolvedDisplayPolicy: 'fold',
            });
        });

        it('should add user info to annotations when lance::init event is triggered', async () => {
            const userInfo = {
                email: 'test@example.com',
                firstName: 'Jane',
                lastName: 'Smith',
            };

            const mockAddUsers = jest.fn();
            const mockSetUserId = jest.fn();

            const mockLanceEvent = {
                lance: {
                    setLanguage: jest.fn().mockResolvedValue(undefined),
                    addDictionary: jest.fn(),
                    App: {
                        LANCE: {
                            createAnnotationsUI: jest.fn().mockReturnValue({init: jest.fn().mockResolvedValue(undefined)}),
                        },
                    },
                    getAnnotations: jest.fn().mockReturnValue({
                        addUsers: mockAddUsers,
                        setUserId: mockSetUserId,
                    }),
                },
            };

            authServiceMock.userInfo$.next(userInfo);

            await lanceEventHandler(mockLanceEvent);

            expect(mockAddUsers).toHaveBeenCalledWith([{id: 'test@example.com', name: 'Jane Smith'}]);
        });

        it('should set user ID in annotations when lance::init event is triggered', async () => {
            const userInfo = {
                email: 'test@example.com',
                firstName: 'Jane',
                lastName: 'Smith',
            };

            const mockSetUserId = jest.fn();

            const mockLanceEvent = {
                lance: {
                    setLanguage: jest.fn().mockResolvedValue(undefined),
                    addDictionary: jest.fn(),
                    App: {
                        LANCE: {
                            createAnnotationsUI: jest.fn().mockReturnValue({init: jest.fn().mockResolvedValue(undefined)}),
                        },
                    },
                    getAnnotations: jest.fn().mockReturnValue({
                        addUsers: jest.fn(),
                        setUserId: mockSetUserId,
                    }),
                },
            };

            authServiceMock.userInfo$.next(userInfo);

            await lanceEventHandler(mockLanceEvent);

            expect(mockSetUserId).toHaveBeenCalledWith('test@example.com');
        });

        it('should refresh toolbar when lance::init event is triggered', async () => {
            const mockLanceEvent = {
                lance: {
                    setLanguage: jest.fn().mockResolvedValue(undefined),
                    addDictionary: jest.fn(),
                    App: {
                        LANCE: {
                            createAnnotationsUI: jest.fn().mockReturnValue({init: jest.fn().mockResolvedValue(undefined)}),
                        },
                    },
                    getAnnotations: jest.fn().mockReturnValue({
                        addUsers: jest.fn(),
                        setUserId: jest.fn(),
                    }),
                },
            };

            await lanceEventHandler(mockLanceEvent);

            expect(mockRefresh).toHaveBeenCalled();
        });
    });
});
