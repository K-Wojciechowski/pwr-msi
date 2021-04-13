import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from "@angular/router";
import {AuthService} from "./auth.service";
import {AuthType} from "../models/auth-type";
import {AuthStoreService} from "./auth-store.service";
import {map} from "rxjs/operators";
import {Observable, of} from "rxjs";
import {RestaurantBasic} from "../models/restaurant-basic";
import {some} from "lodash";

@Injectable({
    providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

    constructor(private authService: AuthService, private authStore: AuthStoreService, private router: Router) {
    }

    handleAuthResult(result: boolean): boolean | UrlTree {
        return result ? result : this.router.createUrlTree(["/auth", "login"]);
    }

    findInAccessGroup(accessGroup: RestaurantBasic[] | null | undefined, restaurantId: number | undefined): boolean | UrlTree {
        const authResult = !accessGroup || restaurantId === undefined ? false : some(accessGroup, r => r.restaurantId === restaurantId);
        return this.handleAuthResult(authResult);
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> {
        const auth: AuthType | undefined = route.data.auth;
        const authRestaurantKey: string = route.data.authRestaurantKey === undefined ? "restaurantId" : route.data.authRestaurantKey;
        const restaurantIdStr: string | null | undefined = route.paramMap.get(authRestaurantKey);
        const restaurantId: number | undefined = restaurantIdStr === null || restaurantIdStr === undefined ? undefined : parseInt(restaurantIdStr);
        switch (auth) {
            case AuthType.ADMIN:
                return this.authStore.access.pipe(map((access) => this.handleAuthResult(access?.admin === true)));
            case AuthType.MANAGE:
                return this.authStore.access.pipe(map(access => this.findInAccessGroup(access?.manage, restaurantId)));
            case AuthType.ACCEPT:
                return this.authStore.access.pipe(map(access => this.findInAccessGroup(access?.accept, restaurantId)));
            case AuthType.DELIVER:
                return this.authStore.access.pipe(map(access => this.findInAccessGroup(access?.deliver, restaurantId)));
            default:
                return of(true);
        }
    }
}
