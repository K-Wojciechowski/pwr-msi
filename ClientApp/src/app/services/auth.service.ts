import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable} from "rxjs";
import {UserProfile} from "../models/user-profile";
import {UserAccess} from "../models/user-access";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {AuthResult} from "../models/auth-result";
import {map, switchMap} from 'rxjs/operators';
import {RefreshResult} from "../models/refresh-result";
import {DateTime, Duration} from "luxon";
import {SKIP_AUTH_HEADER_AUTH_REFRESH} from "../interceptors/auth.types";

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    public user: BehaviorSubject<UserProfile | null> = new BehaviorSubject<UserProfile | null>(null);
    public access: BehaviorSubject<UserAccess | null> = new BehaviorSubject<UserAccess | null>(null);
    public authInitialized: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    public authToken: string | null = null;
    private refreshToken: string | null = null;
    private refreshTimeout: number | null = null;
    private static REFRESH_TOKEN_BEFORE = Duration.fromMillis(10000);

    constructor(private http: HttpClient) {
        this.loadTokens();
        this.updateAccess();
    }

    private loadTokens() {
        this.authToken = localStorage.getItem("authToken");
        this.refreshToken = localStorage.getItem("refreshToken");
    }

    private saveTokens() {
        AuthService.setLocalStorage("authToken", this.authToken);
        AuthService.setLocalStorage("refreshToken", this.refreshToken);
    }

    private static setLocalStorage(key: string, value: string | null) {
        if (value === null) {
            localStorage.removeItem(key);
        } else {
            localStorage.setItem(key, value);
        }
    }

    private updateAccess(): Observable<UserAccess> {
        const access$ = this.http.get<UserAccess>("/api/auth/access/");
        access$.subscribe(access => {
            this.access.next(access);
            this.user.next(access.profile);
            this.authInitialized.next(true);
        }, () => {
            this.access.next(null);
            this.user.next(null);
            this.authInitialized.next(true);
        });
        return access$;
    }

    public logIn(username: string, password: string): Observable<UserAccess> {
        const headers = new HttpHeaders(SKIP_AUTH_HEADER_AUTH_REFRESH);
        const req = this.http.post<AuthResult>("/api/auth/", {username, password}, {headers});
        req.subscribe(authRes => {
            this.authToken = authRes.authToken;
            this.refreshToken = authRes.refreshToken;
            this.scheduleRefresh(authRes.refreshAt);
            this.saveTokens();
        });
        return req.pipe(
            switchMap(() => this.updateAccess())
        );
    }

    public logOut() {
        if (this.refreshTimeout !== null) {
            window.clearTimeout(this.refreshTimeout);
        }
        this.authToken = null;
        this.refreshToken = null;
        this.saveTokens();
        this.user.next(null);
        this.access.next(null);
    }

    public refresh(): Observable<boolean> {
        const headers = new HttpHeaders(SKIP_AUTH_HEADER_AUTH_REFRESH);
        const req = this.http.post<RefreshResult>("/api/auth/refresh/", {refreshToken: this.refreshToken}, {headers});
        req.subscribe(res => {
            this.authToken = res.authToken;
            this.scheduleRefresh(res.refreshAt);
        });
        return req.pipe(
            map(() => true)
        );
    }

    private scheduleRefresh(refreshDate: string) {
        if (this.refreshTimeout !== null) {
            window.clearTimeout(this.refreshTimeout);
        }
        const timeToRefresh = DateTime.fromISO(refreshDate).diffNow("milliseconds").minus(AuthService.REFRESH_TOKEN_BEFORE).get("milliseconds");
        this.refreshTimeout = window.setTimeout(() => this.refresh(), timeToRefresh);
    }
}
