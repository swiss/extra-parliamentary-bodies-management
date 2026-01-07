import {today} from './date-util';

describe('DateUtil', () => {
    describe('today', () => {
        it('should return a date with time set to 00:00:00.000', () => {
            const expected = new Date();
            expected.setHours(0, 0, 0, 0);

            expect(today()).toEqual(expected);
        });
    });
});
