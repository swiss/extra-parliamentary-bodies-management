import {TestBed} from '@angular/core/testing';
import {AbstractControl, FormBuilder, ReactiveFormsModule, ValidatorFn} from '@angular/forms';
import {Comparison, rangeValidator} from './range.validator';

describe('RangeValidator', () => {
    let control1: AbstractControl;
    let control2: AbstractControl;
    let formBuilder: FormBuilder;
    let validator: ValidatorFn;
    const locale = 'de';

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [ReactiveFormsModule],
        });

        formBuilder = TestBed.inject(FormBuilder);

        control1 = formBuilder.control(null);
        control2 = formBuilder.control(null);
    });

    it.each([
        [0, 1],
        [10, 20],
        [10.5, 20.5],
        [new Date(2023, 2, 1), new Date(2023, 2, 2)],
        ['a', 'b'],
    ])('compares greater than correctly %#', (value1, value2) => {
        control1.setValue(value1);
        control2.setValue(value2);

        validator = rangeValidator(control1, Comparison.GreaterThan);
        const validationResult = validator(control2);

        expect(validationResult).toBeNull();
    });

    it.each([
        [0, 1],
        [10, 20],
        [10, 10],
        [10.5, 20.5],
        [10.5, 10.5],
        [new Date(2023, 2, 1), new Date(2023, 2, 2)],
        [new Date(2023, 2, 1), new Date(2023, 2, 1)],
        ['a', 'b'],
        ['a', 'a'],
    ])('compares greater than equal correctly %#', (value1, value2) => {
        control1.setValue(value1);
        control2.setValue(value2);

        validator = rangeValidator(control1, Comparison.GreaterThanEqual);
        const validationResult = validator(control2);

        expect(validationResult).toBeNull();
    });

    it.each([
        [0, 1],
        [10, 20],
        [10.5, 20.5],
        [new Date(2023, 2, 1), new Date(2023, 2, 2)],
        ['a', 'b'],
    ])('compares lower than correctly %#', (value1, value2) => {
        control1.setValue(value1);
        control2.setValue(value2);

        validator = rangeValidator(control2, Comparison.LowerThan);
        const validationResult = validator(control1);

        expect(validationResult).toBeNull();
    });

    it.each([
        [0, 1],
        [10, 20],
        [10, 10],
        [10.5, 20.5],
        [10.5, 10.5],
        [new Date(2023, 2, 1), new Date(2023, 2, 2)],
        [new Date(2023, 2, 1), new Date(2023, 2, 1)],
        ['a', 'b'],
        ['a', 'a'],
    ])('compares greater than equal correctly %#', (value1, value2) => {
        control1.setValue(value1);
        control2.setValue(value2);

        validator = rangeValidator(control2, Comparison.LowerThanEqual);
        const validationResult = validator(control1);

        expect(validationResult).toBeNull();
    });

    it.each([
        [null, 20],
        [10, null],
        [undefined, 10],
        [1, undefined],
    ])('does not validate null or undefined values %#', (value1, value2) => {
        control1.setValue(value1);
        control2.setValue(value2);

        validator = rangeValidator(control2);
        const validationResult = validator(control1);

        expect(validationResult).toBeNull();
    });

    it('formats error for greater than correctly', () => {
        control1.setValue(10);
        control2.setValue(20);

        validator = rangeValidator(control2, Comparison.GreaterThan);
        const validationResult = validator(control1);

        expect(validationResult).toEqual({
            rangeGreaterThanError: {
                params: {
                    value: '20',
                },
            },
        });
    });

    it('formats error for greater than equal correctly', () => {
        control1.setValue(10);
        control2.setValue(20);

        validator = rangeValidator(control2, Comparison.GreaterThanEqual);
        const validationResult = validator(control1);

        expect(validationResult).toEqual({
            rangeGreaterThanEqualError: {
                params: {
                    value: '20',
                },
            },
        });
    });

    it('formats error for lower than correctly', () => {
        control1.setValue(10);
        control2.setValue(20);

        validator = rangeValidator(control1, Comparison.LowerThan);
        const validationResult = validator(control2);

        expect(validationResult).toEqual({
            rangeLowerThanError: {
                params: {
                    value: '10',
                },
            },
        });
    });

    it('formats error for lower than equal correctly', () => {
        control1.setValue(10);
        control2.setValue(20);

        validator = rangeValidator(control1, Comparison.LowerThanEqual);
        const validationResult = validator(control2);

        expect(validationResult).toEqual({
            rangeLowerThanEqualError: {
                params: {
                    value: '10',
                },
            },
        });
    });

    it('formats error with date values correctly', () => {
        control1.setValue(new Date(2023, 2, 1));
        control2.setValue(new Date(2023, 2, 2));

        validator = rangeValidator(control2, Comparison.GreaterThan, locale);
        const validationResult = validator(control1);

        expect(validationResult).toEqual({
            rangeGreaterThanError: {
                params: {
                    value: '02.03.2023',
                },
            },
        });
    });

    it('throws error for unknown comparison', () => {
        control1.setValue(10);
        control2.setValue(20);

        validator = rangeValidator(control1, null as unknown as Comparison);

        expect(() => validator(control2)).toThrow();
    });
});
