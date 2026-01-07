import {Injectable} from '@angular/core';
import {MatDateFormats, NativeDateAdapter} from '@angular/material/core';

/**
 * DateOnly type represents a date without time component (yyyy-MM-dd format).
 * This corresponds to C# DateOnly type from the backend.
 *
 * Usage pattern:
 * - Backend (C#): DateOnly fields are serialized as "yyyy-MM-dd" strings
 * - Transport: HTTP interceptor automatically converts between Date and DateOnly string
 * - Frontend (TypeScript models): Keep as Date for Material datepicker compatibility
 * - Conversion helpers: Use toDateOnlyString() and fromDateOnlyString() when needed
 *
 * The DateOnlyInterceptor handles automatic conversion, so you typically don't need
 * to manually convert in components or services.
 */
export type DateOnly = string & {__brand: 'DateOnly'};

const pad = (n: number) => `0${n}`.slice(-2);

/** Date -> "yyyy-MM-dd" (no timezone) */
export function toDateOnlyString(d: Date): DateOnly {
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}` as DateOnly;
}

/** "yyyy-MM-dd" -> Date (local) */
export function fromDateOnlyString(s: string): Date {
    const [y, m, d] = s.split('-').map(Number);
    return new Date(y, (m ?? 1) - 1, d ?? 1);
}

@Injectable({
    providedIn: 'root',
})
export class ApgDateAdapter extends NativeDateAdapter {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    override parse(value: any): Date | null {
        if (value instanceof Date) {
            return value;
        }
        if (typeof value !== 'string' || value.trim() === '') {
            return null;
        }

        // dd.MM.yyyy
        if (value.includes('.')) {
            const [day, month, year] = value.split('.').map(Number);
            const y = year < 100 ? year + 2000 : year;
            const d = new Date(y, month - 1, day);
            return isNaN(d.getTime()) ? null : d;
        }

        // yyyy-MM-dd (DateOnly from backend)
        if (value.includes('-')) {
            const [yearStr, monthStr, dayStr] = value.split('-');
            const y = Number(yearStr);
            const m = Number(monthStr);
            const dd = Number(dayStr);
            const d = new Date(y, m - 1, dd);
            return isNaN(d.getTime()) ? null : d;
        }

        return super.parse(value);
    }

    override format(date: Date, displayFormat: object): string {
        // @ts-ignore
        return displayFormat === 'apg-format' ? `${pad(date.getDate())}.${pad(date.getMonth() + 1)}.${date.getFullYear()}` : super.format(date, displayFormat);
    }

    override getFirstDayOfWeek(): number {
        return 1;
    }
}

export const APG_DATE_FORMATS: MatDateFormats = {
    parse: {
        dateInput: 'dd.MM.yyyy',
    },
    display: {
        dateInput: 'apg-format',
        monthYearLabel: {year: 'numeric', month: 'long'},
        dateA11yLabel: {year: 'numeric', month: 'long', day: 'numeric'},
        monthYearA11yLabel: {year: 'numeric', month: 'long'},
    },
};
