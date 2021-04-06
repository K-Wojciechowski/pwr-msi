import {Injectable} from '@angular/core';
import {BehaviorSubject} from "rxjs";
import {UserProfile} from "../models/user-profile";
import {UserAccess} from "../models/user-access";
import {AuthResult} from "../models/auth-result";
import {RefreshResult} from "../models/refresh-result";

@Injectable({
    providedIn: 'root'
})
export class AuthStoreService {
    public user: BehaviorSubject<UserProfile | null> = new BehaviorSubject<UserProfile | null>(null);
    public access: BehaviorSubject<UserAccess | null> = new BehaviorSubject<UserAccess | null>(null);
    public authInitialized: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    public authToken: string | null = null;
    refreshToken: string | null = null;

    constructor() {
        this.loadTokens();
    }


    private loadTokens() {
        this.authToken = localStorage.getItem("authToken");
        this.refreshToken = localStorage.getItem("refreshToken");
    }


    private saveTokens() {
        AuthStoreService.setLocalStorage("authToken", this.authToken);
        AuthStoreService.setLocalStorage("refreshToken", this.refreshToken);
    }

    private static setLocalStorage(key: string, value: string | null) {
        if (value === null) {
            localStorage.removeItem(key);
        } else {
            localStorage.setItem(key, value);
        }
    }

    saveAccess(access: UserAccess | null) {
        this.access.next(access);
        this.user.next(access ? access.profile : null);
        this.authInitialized.next(true);
    }

    saveTokensFromAuth(authRes: AuthResult) {
        this.authToken = authRes.authToken;
        this.refreshToken = authRes.refreshToken;
        this.saveTokens();
    }

    saveTokenFromRefresh(res: RefreshResult) {
        this.authToken = res.authToken;
        this.saveTokens();
    }


    handleLogOut() {
        this.authToken = null;
        this.refreshToken = null;
        this.saveTokens();
        this.user.next(null);
        this.access.next(null);
    }
}
