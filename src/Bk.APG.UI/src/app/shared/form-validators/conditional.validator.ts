import {ValidatorFn} from '@angular/forms';

export const conditionalValidator = (predicate: () => boolean, validator: ValidatorFn): ValidatorFn => {
    return formControl => {
        if (predicate()) {
            return validator(formControl);
        }
        return null;
    };
};
