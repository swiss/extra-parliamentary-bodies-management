import {AbstractControl, ValidationErrors, ValidatorFn} from '@angular/forms';

export function multiPercentageSumValidator(subsets: {[key: string]: string[]}): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const errors: ValidationErrors = {};

        for (const key in subsets) {
            if (Object.hasOwn(subsets, key)) {
                const fields = subsets[key];
                const total = fields.reduce((sum, fieldName) => {
                    const val = control.get(fieldName)?.value;
                    const num = typeof val === 'number' ? val : parseFloat(val);
                    return sum + (isNaN(num) ? 0 : num);
                }, 0);

                if (total > 100) {
                    errors[`${key}SumExceeded`] = {total};
                }
            }
        }
        return Object.keys(errors).length ? errors : null;
    };
}
