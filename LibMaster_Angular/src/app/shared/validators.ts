import { AbstractControl, ValidationErrors } from '@angular/forms';

export function passwordValidator(control: AbstractControl): ValidationErrors | null {
    const pattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    return pattern.test(control.value) ? null : { invalidPassword: true };
}

import { ValidatorFn } from '@angular/forms';
import * as _moment from 'moment';
import { default as _rollupMoment, Moment } from 'moment';
const moment = _rollupMoment || _moment;

export function expiryDateValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const currentDate = moment();
        const selectedDate = moment(control.value);

        if (selectedDate.isBefore(currentDate, 'month')) {
            return { expiryDateInvalid: 'Expiry date must be in the future' };
        }
        return null;
    };
}
