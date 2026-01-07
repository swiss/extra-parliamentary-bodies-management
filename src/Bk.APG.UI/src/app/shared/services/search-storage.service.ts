/* eslint-disable @typescript-eslint/no-explicit-any */
import {Inject, Injectable} from '@angular/core';
import {SESSION_STORAGE} from '@shared/injection.tokens';
import {dateReviver} from './date-reviver';

@Injectable({
    providedIn: 'root',
})
export class SearchStorageService {
    private readonly keyPrefix = 'search-params';

    constructor(@Inject(SESSION_STORAGE) private readonly storage: Storage) {}

    getParams(name: string): any {
        return JSON.parse(this.storage.getItem(this.key(name))!, dateReviver);
    }

    setParams(name: string, params: any): void {
        this.storage.setItem(this.key(name), JSON.stringify(params));
    }

    patchParams(name: string, params: any): void {
        const item = this.getParams(name);
        this.setParams(name, {...item, ...params});
    }

    removeParams(name: string): void {
        this.storage.removeItem(this.key(name));
    }

    private key(name: string): string {
        return `${this.keyPrefix}-${name}`;
    }
}
