import {Injectable, Injector} from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor, HttpErrorResponse
} from '@angular/common/http';
import {Observable, throwError} from 'rxjs';
import {AuthService} from "../services/auth.service";
import {catchError, switchMap} from "rxjs/operators";
import {SKIP_AUTH_HEADER_NAME, SKIP_AUTH_HEADER_VALUE_AUTH, SKIP_AUTH_HEADER_VALUE_REFRESH} from "./auth.types";
import {AuthStoreService} from "../services/auth-store.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    public static AUTH_HEADER_KEY = "Authorization";
    public static AUTH_HEADER_VALUE_BEFORE = "Bearer ";

    constructor(private authService: AuthService, private authStore: AuthStoreService) {
    }

    private setAuthHeader(request: HttpRequest<unknown>): HttpRequest<unknown> {
        const authToken = this.authStore.authToken;
        if (authToken !== null) {
            const headers: any = {};
            headers[AuthInterceptor.AUTH_HEADER_KEY] = AuthInterceptor.AUTH_HEADER_VALUE_BEFORE + authToken;
            return request.clone({setHeaders: headers});
        } else {
            return request;
        }
    }

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        let authReq: HttpRequest<unknown> = request;
        let skipAuthHeader: string | null = null;
        if (request.headers.has(SKIP_AUTH_HEADER_NAME)) {
            skipAuthHeader = request.headers.get(SKIP_AUTH_HEADER_NAME);
            authReq = request.clone({headers: request.headers.delete(SKIP_AUTH_HEADER_NAME)});
        }

        const skipAuthHeaderValue = skipAuthHeader === null ? 0 : parseInt(skipAuthHeader);
        if ((skipAuthHeaderValue & SKIP_AUTH_HEADER_VALUE_AUTH) == 0) {
            authReq = this.setAuthHeader(authReq);
        }
        return next.handle(authReq)
            .pipe(
                catchError((err: HttpErrorResponse) => {
                    if (err.status === 401 && (skipAuthHeaderValue & SKIP_AUTH_HEADER_VALUE_REFRESH) === 0) {
                        // Try to refresh the token.
                        return this.authService.refresh().pipe(
                            switchMap(() => {
                                const noRefreshHeaders: any = {};
                                noRefreshHeaders[SKIP_AUTH_HEADER_NAME] = (skipAuthHeaderValue | SKIP_AUTH_HEADER_VALUE_REFRESH).toString();
                                const reqNoRefresh = authReq.clone({
                                    setHeaders: noRefreshHeaders
                                });
                                const authReqNoRefresh = this.setAuthHeader(reqNoRefresh);
                                return next.handle(authReqNoRefresh);
                            })
                        );
                    } else {
                        return throwError(err);
                    }
                })
            );
    }
}
