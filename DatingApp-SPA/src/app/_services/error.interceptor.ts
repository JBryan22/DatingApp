import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpEvent, HttpHandler, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

// error handling for login and registration really needs to be revamped later on if it isn't later in the course
// should add front end error handling to make verify password length prior to sending to db
// can eventually add username exists check before they click on send - not necessary though, just don't clear fields in this case

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            // catchError is an rxjs method which is why we have to use the pipe method above (also rxjs)
            catchError(error => {
                // http error is from our api and our api can only really return http errors because we have to communicate via http
                if (error instanceof HttpErrorResponse) {
                    if (error.status === 401) {
                        return throwError(error.statusText);
                    }
                    // application-error is a header we added in our API to send an error message
                    const applicationError = error.headers.get('Application-Error');
                    if (applicationError) {
                        console.error(applicationError);
                        return throwError(applicationError);
                    }
                    const serverError = error.error;
                    let modelStateErrors = '';
                    if (serverError && typeof serverError === 'object') {
                        for (const key in serverError) {
                            if (serverError[key]) {
                                modelStateErrors += serverError[key] + '\n';
                            }
                        }
                    }
                    return throwError(modelStateErrors || serverError || 'Server Error');
                }
            })
        );
    }
}

// need to export
// we are adding our own custom interceptor on top of angular interceptor
// multi means we add it to existing interceptors rather than replace them
export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
}