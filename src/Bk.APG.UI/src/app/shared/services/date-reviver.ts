/* eslint-disable @typescript-eslint/no-explicit-any */
export function dateReviver(key: string, value: any): any {
    const isoDateRegex = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)(?:Z|(\+|-)([\d|:]*))?$/;

    if (!(isString(value) && isoDateRegex.test(value))) {
        return value;
    }

    return new Date(value);
}

function isString(value: unknown): value is string {
    return typeof value === 'string';
}
