/* eslint-disable @typescript-eslint/no-explicit-any */
// noinspection JSUnusedGlobalSymbols

import {NgClass} from '@angular/common';
import {
    ChangeDetectorRef,
    Component,
    computed,
    DestroyRef,
    effect,
    ElementRef,
    inject,
    Injector,
    input,
    OnInit,
    runInInjectionContext,
    Signal,
    signal,
    viewChild,
} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {ControlContainer, FormGroupDirective, ReactiveFormsModule, StatusChangeEvent} from '@angular/forms';
import {TranslateService} from '@ngx-translate/core';
import {FroalaPlugin} from '@shared/froala.service';
import {FroalaEditorDirective, FroalaEditorModule} from 'angular-froala-wysiwyg';
import {debounceTime, distinctUntilChanged, filter, map, startWith} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {ConfigsService} from '../../configs.service';
import {HelpTooltipComponent} from '../help-tooltip/help-tooltip.component';

import 'froala-editor/js/languages/de.js';
import 'froala-editor/js/languages/fr.js';
import 'froala-editor/js/languages/it.js';
import 'froala-editor/js/plugins/colors.min.js';
import 'froala-editor/js/plugins/lists.min.js';
import 'froala-editor/js/plugins/table.min.js';

@Component({
    selector: 'apg-rich-text-editor',
    templateUrl: './rich-text-editor.component.html',
    styleUrls: ['./rich-text-editor.component.scss'],
    imports: [ReactiveFormsModule, FroalaEditorModule, NgClass, HelpTooltipComponent],
    viewProviders: [
        {
            provide: ControlContainer,
            useExisting: FormGroupDirective,
        },
    ],
})
export class RichTextEditorComponent implements OnInit {
    editorDirective = viewChild<any>(FroalaEditorDirective);
    isUpdating = signal<boolean>(false);

    controlName = input.required<string>();
    controlStatus = computed<StatusChangeEvent | undefined>(() => undefined);

    title = input.required<string>();
    helpTooltipTexts = input<string[]>();
    heightMin = input<number>();
    heightMax = input<number>();
    plugins = input<FroalaPlugin[]>([]);

    language: Signal<string | undefined>;
    options: Signal<object>;

    private readonly injector = inject(Injector);
    private readonly elementRef = inject(ElementRef<HTMLElement>);
    private readonly configsService = inject(ConfigsService);
    private readonly translateService = inject(TranslateService);
    private readonly cdr = inject(ChangeDetectorRef);
    private readonly form = inject(FormGroupDirective);
    private readonly dr = inject(DestroyRef);
    private readonly authService = inject(AuthService);

    constructor() {
        this.language = toSignal(
            this.translateService.onLangChange.pipe(
                map(event => event.lang),
                startWith(this.translateService.getCurrentLang()),
                distinctUntilChanged(),
                takeUntilDestroyed()
            )
        );

        this.options = computed<object>(() => ({
            attribution: false,
            charCounterCount: false,
            heightMax: this.heightMax() ?? 9999,
            heightMin: this.heightMin() ?? 100,
            immediatelyUpdateAngularComponent: true,
            key: this.configsService.frontendConfig.froalaKey,
            language: this.language(),
            placeholderText: '',
            toolbarButtons: {
                moreText: {
                    buttons: ['bold', 'italic', 'subscript', 'superscript'],
                    buttonsVisible: 10,
                },
                moreParagraph: {
                    buttons: [
                        ...(this.plugins().includes('flite')
                            ? ['flite-toggletracking', 'flite-toggleshow', 'flite-acceptall', 'flite-rejectall', 'flite-acceptone', 'flite-rejectone']
                            : []),
                        ...(this.plugins().includes('lance') ? ['annotate'] : []),
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
            pluginsEnabled: this.plugins(),
            flite: {
                allowQuitWithChanges: true,
            },
            lance: {
                undoPolicy: 'all',
                useTextSelection: 'all',
            },
            events: {
                'flite:init': async (event: any) => {
                    event.flite.addDictionary('it', {});
                    await event.flite.setLanguage(this.translateService.getCurrentLang() || 'de');

                    this.authService.userInfo$
                        .pipe(takeUntilDestroyed(this.dr))
                        .subscribe(userInfo =>
                            event.flite.setUserInfo({id: userInfo.email, name: `${userInfo.firstName ?? ''} ${userInfo.lastName ?? ''}`.trim()})
                        );

                    this.refreshToolbar();
                },
                'lance::init': async (event: any) => {
                    event.lance.addDictionary('it', {});
                    await event.lance.setLanguage(this.translateService.getCurrentLang() || 'de');

                    const container = (this.elementRef.nativeElement as HTMLElement).querySelector('.annotations-container') as HTMLElement;
                    if (container) {
                        const lanceUI = event.lance.App.LANCE.createAnnotationsUI({type: 'aligned'});
                        await lanceUI.init({
                            owner: event.lance.getAnnotations(),
                            generateUI: true,
                            container,
                            resolvedDisplayPolicy: 'fold',
                        });
                    }

                    this.authService.userInfo$.pipe(takeUntilDestroyed(this.dr)).subscribe(userInfo => {
                        const annotations = event.lance.getAnnotations();
                        annotations.addUsers([{id: userInfo.email, name: `${userInfo.firstName ?? ''} ${userInfo.lastName ?? ''}`.trim()}]);
                        annotations.setUserId(userInfo.email);
                    });

                    this.refreshToolbar();
                },
            },
        }));
    }

    ngOnInit() {
        runInInjectionContext(this.injector, () => {
            this.reloadEditor();
            this.monitorControlStatus();
            this.enableOrDisableEditor();
        });
    }

    private monitorControlStatus() {
        this.controlStatus = toSignal(
            this.form.control.get(this.controlName())!.events.pipe(
                filter(event => event instanceof StatusChangeEvent),
                debounceTime(100),
                takeUntilDestroyed(this.dr)
            )
        );
    }

    private reloadEditor() {
        effect(() => {
            this.options();
            this.language();

            this.isUpdating.set(true);
            this.cdr.detectChanges();
            this.isUpdating.set(false);
        });
    }

    private enableOrDisableEditor() {
        effect(() => {
            const status = this.controlStatus();

            if (status?.status === 'DISABLED') {
                this.editorDirective()?._editor?.edit?.off();
            } else {
                this.editorDirective()?._editor?.edit?.on();
            }

            this.form.control.get(this.controlName())!.updateValueAndValidity();
        });
    }

    private refreshToolbar() {
        const editor = this.editorDirective()?._editor;
        if (editor?.toolbar?.refresh) {
            editor.toolbar.refresh();
        }
    }
}
