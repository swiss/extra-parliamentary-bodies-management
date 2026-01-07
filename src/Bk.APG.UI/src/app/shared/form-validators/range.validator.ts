import {AbstractControl, ValidatorFn} from '@angular/forms';

export function rangeValidator(comparisonControl: AbstractControl, comparison: Comparison = Comparison.GreaterThan, currentLocale = 'de'): ValidatorFn {
    return (control: AbstractControl) => {
        if (control.value === undefined || control.value === null || comparisonControl.value === undefined || comparisonControl.value === null) {
            return null;
        }

        if (comparison === Comparison.GreaterThan) {
            return control.value > comparisonControl.value
                ? null
                : {rangeGreaterThanError: {params: {value: formatValue(comparisonControl.value, currentLocale)}}};
        }

        if (comparison === Comparison.GreaterThanEqual) {
            return control.value >= comparisonControl.value
                ? null
                : {rangeGreaterThanEqualError: {params: {value: formatValue(comparisonControl.value, currentLocale)}}};
        }

        if (comparison === Comparison.LowerThan) {
            return control.value < comparisonControl.value
                ? null
                : {rangeLowerThanError: {params: {value: formatValue(comparisonControl.value, currentLocale)}}};
        }

        if (comparison === Comparison.LowerThanEqual) {
            return control.value <= comparisonControl.value
                ? null
                : {rangeLowerThanEqualError: {params: {value: formatValue(comparisonControl.value, currentLocale)}}};
        }

        throw new Error('Selected comparison is not valid');
    };
}

function formatValue(value: unknown, locale: string): string {
    if (value instanceof Date) {
        return value.toLocaleDateString(locale, {year: 'numeric', month: '2-digit', day: '2-digit'});
    }

    return `${value}`;
}

export enum Comparison {
    GreaterThan,
    GreaterThanEqual,
    LowerThan,
    LowerThanEqual,
}
