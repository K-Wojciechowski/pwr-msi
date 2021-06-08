import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, NavigationEnd, ParamMap, Params, Router} from "@angular/router";
import {AuthService} from "./services/auth.service";
import {AuthStoreService} from "./services/auth-store.service";
import {ToastService} from "./services/toast.service";
import {filter, map, switchMap} from "rxjs/operators";
import {BehaviorSubject, of, zip} from "rxjs";
import {union} from "lodash";
import {RestaurantBasic} from "./models/restaurant-basic";
import {RestaurantContextHelperService} from "./services/restaurant-context-helper.service";
import {UserAccess} from "./models/user-access";

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
    title = 'MSI';

    public isLoggedIn: boolean = false;
    public userName = "?";
    public showNavbar: boolean = true;
    public sidebar: string | null = null;
    public canDeliver: boolean = false;
    public canManage: boolean = false;
    public canManageMenu: boolean = false;
    public canManageAccept: boolean = false;
    public isAdmin: boolean = false;
    public isMenuCollapsed = true;

    public currentRestaurantId: BehaviorSubject<number | null> = new BehaviorSubject<number | null>(null);
    public currentRestaurantIdValue: number | null = null;
    public currentRestaurantName: string = "?";
    public currentCanManageMenu: boolean = false;
    public currentCanManageAccept: boolean = false;
    public accessValue: UserAccess | null | undefined = undefined;

    constructor(private authService: AuthService, private authStore: AuthStoreService, private toastService: ToastService, private router: Router, private activatedRoute: ActivatedRoute) {
    }

    private rootRoute(route: ActivatedRoute): ActivatedRoute {
        while (route.firstChild) {
            route = route.firstChild;
        }
        return route;
    }

    ngOnInit() {
        this.authService.initialize();

        this.router.events.pipe(
            filter(event => event instanceof NavigationEnd),
            map(() => this.rootRoute(this.router.routerState.root)),
            switchMap(route => route === null ? of({}) : route.data))
            .subscribe((data: any) => {
                this.showNavbar = !data.hideNavbar;
                this.sidebar = data.sidebar;
            });

        this.router.events.pipe(
            filter(event => event instanceof NavigationEnd),
            map(() => this.rootRoute(this.router.routerState.root)),
            switchMap(route => route === null ? of(undefined) : route.params))
            .subscribe((params) => {
                if (params === undefined) return;
                const restaurantId = +params["restaurantId"];
                this.currentRestaurantId.next(restaurantId);
                this.currentRestaurantIdValue = restaurantId;
                this.updateSidebarWithRestaurantAndAccess();
            });

        this.authStore.user.subscribe(user => {
            this.isLoggedIn = user !== null;
            this.userName = user === null ? "?" : user.firstName + " " + user.lastName;
        });

        this.authStore.access.subscribe(access => {
            this.canDeliver = access !== null && access !== undefined && access.deliver.length > 0;
            this.canManageMenu = access !== null && access !== undefined && access.manage.length > 0;
            this.canManageAccept = access !== null && access !== undefined && access.accept.length > 0;
            this.canManage = this.canManageMenu || this.canManageAccept;
            this.isAdmin = !!access?.admin;
            this.accessValue = access;
            this.updateSidebarWithRestaurantAndAccess();
        });
    }

    private updateSidebarWithRestaurantAndAccess() {
        const access = this.accessValue;
        const restaurantId = this.currentRestaurantIdValue;
        const allAccess: RestaurantBasic[] = (access === null || access === undefined) ? [] : union(access.accept, access.manage, access.deliver);
        const restaurant = allAccess.find(r => r.restaurantId == restaurantId);
        this.currentRestaurantName = restaurant?.name ?? "Restaurant";
        this.currentCanManageAccept = access?.accept.find(r => r.restaurantId == restaurantId) !== undefined;
        this.currentCanManageMenu = access?.manage.find(r => r.restaurantId == restaurantId) !== undefined;
    }

    async handleLogOut() {
        this.authService.logOut();
        this.toastService.showSuccess("Logged out!");
        await this.router.navigateByUrl("/");
    }
}
