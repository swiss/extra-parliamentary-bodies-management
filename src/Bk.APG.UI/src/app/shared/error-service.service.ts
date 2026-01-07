import {Injectable} from '@angular/core';
import {ValidationErrors} from '@angular/forms';
import {TranslateService} from '@ngx-translate/core';

@Injectable({
    providedIn: 'root',
})
export class ErrorService {
    constructor(private readonly translateService: TranslateService) {}

    getControlError(errors: ValidationErrors | null, label: string, labelPrefix: string, data: Record<string, unknown> = {}): string {
        if (errors === null) {
            return '';
        }

        const errorKeys = Object.keys(errors);

        if (errorKeys.length === 0) {
            return '';
        }

        const errorData = errors[errorKeys[0]] ?? {};

        const params = data ? {...errorData, ...data, field: label} : {field: label};

        return this.translateService.instant(`${labelPrefix}.${errorKeys[0]}`, params);
    }
}
