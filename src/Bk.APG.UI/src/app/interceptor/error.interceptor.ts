import {HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ObNotificationService} from '@oblique/oblique';
import {Observable, throwError} from 'rxjs';
import {catchError} from 'rxjs/operators';

const CONFLICT_ERROR_CODE = 409;

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private readonly notificationService: ObNotificationService) {}

    intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        return next.handle(req).pipe(
            catchError((error: HttpErrorResponse) => {
                if (error.status === CONFLICT_ERROR_CODE) {
                    this.notificationService.error('error.conflict');
                    return new Observable<HttpEvent<unknown>>();
                }
                return throwError(() => error);
            })
        );
    }
}
