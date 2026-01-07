import {Injectable} from '@angular/core';
import FroalaEditor from 'froala-editor';

declare global {
    interface Window {
        LOOPINDEX?: {
            FLITE?: {
                initFroalaFLITEPlugin: (froalaEditor: typeof FroalaEditor, options: {path: string}) => Promise<void>;
            };
            LANCE?: {
                initFroalaLancePlugin: (froalaEditor: typeof FroalaEditor, options: {path: string}) => Promise<void>;
            };
        };
    }
}

export type FroalaPlugin = 'flite' | 'lance';

@Injectable({
    providedIn: 'root',
})
export class FroalaService {
    async initializePlugins() {
        const flite = window.LOOPINDEX?.FLITE;
        if (flite) {
            await flite.initFroalaFLITEPlugin(FroalaEditor, {path: '/assets/flite'});
        } else {
            console.warn('FLITE plugin not found on window.LOOPINDEX.FLITE');
        }

        const lance = window.LOOPINDEX?.LANCE;
        if (lance) {
            await lance.initFroalaLancePlugin(FroalaEditor, {path: '/assets/lance'});
        } else {
            console.warn('LANCE plugin not found on window.LOOPINDEX.LANCE');
        }
    }
}
