import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {toDateOnlyString} from '@shared/DateAdapter';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';

/**
 * HTTP Interceptor that handles DateOnly conversion:
 * - Outgoing: Converts Date objects to DateOnly strings (yyyy-MM-dd)
 * - Incoming: Converts DateOnly strings to Date objects for Material datepickers
 */
@Injectable()
export class DateOnlyInterceptor implements HttpInterceptor {
    // Pattern to match DateOnly strings: yyyy-MM-dd (no time component)
    private readonly dateOnlyPattern = /^\d{4}-\d{2}-\d{2}$/;

    // Pattern to match ISO datetime strings with time component
    private readonly isoDateTimePattern = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}/;

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        // Convert outgoing Date objects to DateOnly strings (skip FormData as it's handled manually)
        if (request.body && !(request.body instanceof FormData) && !(request.body instanceof Blob)) {
            const convertedBody = this.convertDatesToDateOnlyStrings(request.body);
            if (convertedBody !== request.body) {
                request = request.clone({body: convertedBody});
            }
        }

        // Convert incoming DateOnly strings to Date objects
        return next.handle(request).pipe(
            map(event => {
                if (event instanceof HttpResponse && event.body && !(event.body instanceof Blob)) {
                    return event.clone({body: this.convertDateOnlyStringsToDates(event.body)});
                }
                return event;
            })
        );
    }

    /**
     * Recursively converts Date objects to DateOnly strings (yyyy-MM-dd) in the request payload
     */
    private convertDatesToDateOnlyStrings(obj: unknown): unknown {
        if (obj === null || obj === undefined) {
            return obj;
        }

        if (obj instanceof Date) {
            return toDateOnlyString(obj);
        }

        if (Array.isArray(obj)) {
            return obj.map(item => this.convertDatesToDateOnlyStrings(item));
        }

        if (typeof obj === 'object') {
            const converted: Record<string, unknown> = {};
            for (const [key, value] of Object.entries(obj)) {
                converted[key] = this.convertDatesToDateOnlyStrings(value);
            }
            return converted;
        }

        return obj;
    }

    /**
     * Recursively converts DateOnly strings (yyyy-MM-dd) to Date objects in the response
     * Leaves ISO datetime strings (with time component) as strings for Angular's HttpClient to handle
     */
    private convertDateOnlyStringsToDates(obj: unknown): unknown {
        if (obj === null || obj === undefined) {
            return obj;
        }

        if (typeof obj === 'string') {
            // Only convert pure date strings (yyyy-MM-dd), not datetime strings
            if (this.dateOnlyPattern.test(obj) && !this.isoDateTimePattern.test(obj)) {
                const [year, month, day] = obj.split('-').map(Number);
                return new Date(year, month - 1, day);
            }
            return obj;
        }

        if (Array.isArray(obj)) {
            return obj.map(item => this.convertDateOnlyStringsToDates(item));
        }

        if (typeof obj === 'object') {
            const converted: Record<string, unknown> = {};
            for (const [key, value] of Object.entries(obj)) {
                converted[key] = this.convertDateOnlyStringsToDates(value);
            }
            return converted;
        }

        return obj;
    }
}
