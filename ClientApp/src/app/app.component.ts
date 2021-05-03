import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, NavigationEnd, Router} from "@angular/router";
import {AuthService} from "./services/auth.service";
import {AuthStoreService} from "./services/auth-store.service";
import {ToastService} from "./services/toast.service";
import {filter, map, switchMap} from "rxjs/operators";
import {of} from "rxjs";

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
    public isAdmin: boolean = false;

    constructor(private authService: AuthService, private authStore: AuthStoreService, private toastService: ToastService, private router: Router, private activatedRoute: ActivatedRoute) {
    }

    ngOnInit() {
        this.authService.initialize();

        this.router.events.pipe(
            filter(event => event instanceof NavigationEnd),
            map(() => this.activatedRoute),
            map(route => route.firstChild),
            switchMap(route => route === null ? of({}) : route.data))
            .subscribe((data: any) => {
                this.showNavbar = !data.hideNavbar;
                this.sidebar = data.sidebar;
            });

        this.authStore.user.subscribe(user => {
            this.isLoggedIn = user !== null;
            this.userName = user === null ? "?" : user.firstName + " " + user.lastName;
        });

        this.authStore.access.subscribe(access => {
            this.canDeliver = access !== null && access !== undefined && access.deliver.length > 0;
            this.canManage = access !== null && access !== undefined && (access.manage.length > 0 || access.accept.length > 0);
            this.isAdmin = !!access?.admin;
        });
    }

    async handleLogOut() {
        this.authService.logOut();
        this.toastService.showSuccess("Logged out!");
        await this.router.navigateByUrl("/");
    }
}
