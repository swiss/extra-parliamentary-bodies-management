/* eslint-disable dot-notation */
import {of} from 'rxjs';
import {ObPaginatorFixService} from './ob-paginator-fix.service';

describe('ObPaginatorFixService', () => {
    let service: ObPaginatorFixService;

    beforeEach(() => {
        service = new ObPaginatorFixService({stream: jest.fn(() => of({}))} as never);
        service['ofLabel'] = 'of';
    });

    it('should return "0 of 0" when length is 0', () => {
        const label = service.getRangeLabel(0, 10, 0);
        expect(label).toBe('0 of 0');
    });

    it('should return "0 of 10" when pageSize is 0', () => {
        const label = service.getRangeLabel(0, 0, 10);
        expect(label).toBe('0 of 10');
    });

    it('should return correct range for the first page', () => {
        const label = service.getRangeLabel(0, 10, 30); // Page 0: items 1-10
        expect(label).toBe('1 – 10 of 30');
    });

    it('should return correct range for a middle page', () => {
        const label = service.getRangeLabel(1, 10, 30); // Page 1: items 11-20
        expect(label).toBe('11 – 20 of 30');
    });

    it('should return correct range for the last page with full items', () => {
        const label = service.getRangeLabel(2, 10, 30); // Page 2: items 21-30
        expect(label).toBe('21 – 30 of 30');
    });

    it('should return correct range for the last page with partial items', () => {
        const label = service.getRangeLabel(2, 10, 25); // Page 2: items 21-25
        expect(label).toBe('21 – 25 of 25');
    });

    it('should handle page * pageSize >= length', () => {
        const label = service.getRangeLabel(3, 10, 25); // Page 3: items 31-40, but length is 25
        expect(label).toBe('31 – 40 of 25');
    });

    it('should handle page number resulting in startIndex equal to length', () => {
        const label = service.getRangeLabel(2, 10, 20); // Page 2: items 21-30, length 20
        expect(label).toBe('21 – 30 of 20');
    });

    it('should handle page number resulting in startIndex greater than length', () => {
        const label = service.getRangeLabel(5, 10, 45); // Page 5: items 51-60, length 45
        expect(label).toBe('51 – 60 of 45');
    });
});
