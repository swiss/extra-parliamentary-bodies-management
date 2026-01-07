/* eslint-disable dot-notation */
import {Injectable} from '@angular/core';
import {ObPaginatorService} from '@oblique/oblique';

@Injectable()
export class ObPaginatorFixService extends ObPaginatorService {
    override getRangeLabel: (page: number, pageSize: number, length: number) => string = (page: number, pageSize: number, length: number) => {
        if (length === 0 || pageSize === 0) {
            return `0 ${this['ofLabel']} ${length}`;
        }

        const startIndex = page * pageSize;
        const endIndex = startIndex < length ? Math.min(startIndex + pageSize, length) : startIndex + pageSize;

        return `${startIndex + 1} – ${endIndex} ${this['ofLabel']} ${length}`;
    };
}
