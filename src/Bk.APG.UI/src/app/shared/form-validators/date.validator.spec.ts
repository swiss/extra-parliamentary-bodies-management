import {FormControl} from '@angular/forms';
import {dateValidator} from './date.validator';

describe('dateValidator', () => {
    const validator = dateValidator();

    it('should validate a correct ISO date string', () => {
        const control = new FormControl('2020-12-31');

        expect(validator(control)).toBeNull();
    });

    it('should validate a correct European date string', () => {
        const control = new FormControl('12/31/2020'); // US format, but JS Date parses it

        expect(validator(control)).toBeNull();
    });

    it('should invalidate a date with an out-of-range year (too high)', () => {
        const control = new FormControl('31.12.202666');

        expect(validator(control)).toEqual({invalidDate: {value: '31.12.202666'}});
    });

    it('should invalidate a date with an out-of-range year (too low)', () => {
        const control = new FormControl('01.01.1899');

        expect(validator(control)).toEqual({invalidDate: {value: '01.01.1899'}});
    });

    it('should validate a date with year 1900', () => {
        const control = new FormControl('01.01.1900');

        expect(validator(control)).toBeNull();
    });

    it('should validate a date with year 2999', () => {
        const control = new FormControl('12.31.2999');

        expect(validator(control)).toBeNull();
    });

    it('should invalidate a date with year 3000', () => {
        const control = new FormControl('01.01.3000');

        expect(validator(control)).toEqual({invalidDate: {value: '01.01.3000'}});
    });

    it('should invalidate a nonsense string', () => {
        const control = new FormControl('not-a-date');

        expect(validator(control)).toEqual({invalidDate: {value: 'not-a-date'}});
    });

    it('should invalidate an empty string', () => {
        const control = new FormControl('');

        expect(validator(control)).toEqual({invalidDate: {value: ''}});
    });

    it('should invalidate null', () => {
        const control = new FormControl(null);

        expect(validator(control)).toEqual({invalidDate: {value: null}});
    });

    it('should invalidate an impossible date (e.g., 31.13.2020)', () => {
        const control = new FormControl('31.13.2020');

        expect(validator(control)).toEqual({invalidDate: {value: '31.13.2020'}});
    });

    it('should validate today', () => {
        const today = new Date();
        const iso = today.toISOString().substring(0, 10);
        const control = new FormControl(iso);

        expect(validator(control)).toBeNull();
    });
});
