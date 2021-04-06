import {Injectable} from '@angular/core';
import {Observable} from "rxjs";
import {UserAccess} from "../models/user-access";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {AuthResult} from "../models/auth-result";
import {map, switchMap, tap} from 'rxjs/operators';
import {RefreshResult} from "../models/refresh-result";
import {DateTime, Duration} from "luxon";
import {SKIP_AUTH_HEADER_AUTH_REFRESH} from "../interceptors/auth.types";
import {AuthStoreService} from "./auth-store.service";

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private refreshTimeout: number | null = null;
    private static REFRESH_TOKEN_BEFORE = Duration.fromMillis(10000);

    constructor(private http: HttpClient, private authStore: AuthStoreService) {
        this.updateAccess().subscribe(() => null);
    }

    private updateAccess(): Observable<UserAccess> {
        const access$ = this.http.get<UserAccess>("/api/auth/access/");
        return access$.pipe(
            tap(
                access => this.authStore.saveAccess(access),
                () => this.authStore.saveAccess(null)
            )
        );
    }

    public logIn(username: string, password: string): Observable<UserAccess> {
        const headers = new HttpHeaders(SKIP_AUTH_HEADER_AUTH_REFRESH);
        const req = this.http.post<AuthResult>("/api/auth/", {username, password}, {headers});
        return req.pipe(
            tap(authRes => {
                this.authStore.saveTokensFromAuth(authRes);
                this.scheduleRefresh(authRes.refreshAt);
            }),
            switchMap(() => this.updateAccess())
        );
    }

    public logOut() {
        if (this.refreshTimeout !== null) {
            window.clearTimeout(this.refreshTimeout);
        }
        this.authStore.handleLogOut();
    }

    public refresh(): Observable<boolean> {
        const headers = new HttpHeaders(SKIP_AUTH_HEADER_AUTH_REFRESH);
        const req = this.http.post<RefreshResult>("/api/auth/refresh/", {refreshToken: this.authStore.refreshToken}, {headers});
        return req.pipe(
            map((res) => {
                this.authStore.saveTokenFromRefresh(res);
                this.scheduleRefresh(res.refreshAt);
                return true;
            })
        );
    }

    private scheduleRefresh(refreshDate: string) {
        if (this.refreshTimeout !== null) {
            window.clearTimeout(this.refreshTimeout);
        }
        const timeToRefresh = DateTime.fromISO(refreshDate).diffNow("milliseconds").minus(AuthService.REFRESH_TOKEN_BEFORE).get("milliseconds");
        this.refreshTimeout = window.setTimeout(() => this.refresh(), timeToRefresh);
    }

    ensureAuthReady() {
    }
}
