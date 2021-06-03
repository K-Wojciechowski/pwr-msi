import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from "@angular/router";
import {AuthService} from "./auth.service";
import {AuthType} from "../models/enum/auth-type";
import {AuthStoreService} from "./auth-store.service";
import {map, skipWhile} from "rxjs/operators";
import {Observable, of} from "rxjs";
import {RestaurantBasic} from "../models/restaurant-basic";
import {concat, some} from "lodash";

@Injectable({
    providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

    constructor(private authService: AuthService, private authStore: AuthStoreService, private router: Router) {
    }

    handleAuthResult(route: ActivatedRouteSnapshot, result: boolean): boolean | UrlTree {
        if (result) return result;
        const queryParams = {next: "/" + route.url.join("/")};
        return this.router.createUrlTree(["/auth", "login"], {queryParams: queryParams});
    }

    findInAccessGroup(route: ActivatedRouteSnapshot, accessGroup: RestaurantBasic[] | null | undefined, restaurantId: number | undefined): boolean | UrlTree {
        const authResult = !accessGroup || restaurantId === undefined ? false : some(accessGroup, r => r.restaurantId === restaurantId);
        return this.handleAuthResult(route, authResult);
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> {
        const authType: AuthType | undefined = route.data.auth;
        const authRestaurantKey: string = "restaurantId";
        const restaurantIdStr: string | null | undefined = route.paramMap.get(authRestaurantKey);
        const restaurantId: number | undefined = restaurantIdStr === null || restaurantIdStr === undefined ? undefined : parseInt(restaurantIdStr);
        if (authType === undefined) return of(true);
        return this.authStore.access.pipe(
            skipWhile(value => value === undefined),
            map(access => {
                    if (access == null) return false;
                    switch (authType) {
                        case AuthType.USER:
                            return true;
                        case AuthType.ADMIN:
                            return this.handleAuthResult(route, access.admin);
                        case AuthType.MANAGE:
                            return this.findInAccessGroup(route, access.manage, restaurantId);
                        case AuthType.ACCEPT:
                            return this.findInAccessGroup(route, access.accept, restaurantId);
                        case AuthType.ACCEPT_OR_MANAGE:
                            return this.findInAccessGroup(route, concat(access.manage, access.accept), restaurantId);
                        case AuthType.DELIVER:
                            return this.findInAccessGroup(route, access.deliver, restaurantId);
                    }
                }
            )
        );
    }
}
