import {AbstractControl, ValidatorFn} from '@angular/forms';

export function dateValidator(): ValidatorFn {
    return (control: AbstractControl) => {
        if (!control.value) {
            return {
                invalidDate: {
                    value: control.value,
                },
            };
        }

        const date = new Date(control.value);

        if (isNaN(date.getTime())) {
            return {
                invalidDate: {
                    value: control.value,
                },
            };
        }

        const year = date.getFullYear();
        if (year < 1900 || year > 2999) {
            return {
                invalidDate: {
                    value: control.value,
                },
            };
        }

        return null;
    };
}
