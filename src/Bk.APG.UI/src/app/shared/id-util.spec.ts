import {isEmptyId} from './id-util';

describe('IdUtil', () => {
    describe('isEmptyId', () => {
        it.each([[undefined], [null], [''], ['00000000-0000-0000-0000-000000000000']])("should return true for empty id '%s'", value => {
            expect(isEmptyId(value as string | undefined)).toBe(true);
        });
    });
});
