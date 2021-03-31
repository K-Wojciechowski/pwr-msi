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
    public showSidebar: boolean = true;

    constructor(private authService: AuthService, private authStore: AuthStoreService, private toastService: ToastService, private router: Router, private activatedRoute: ActivatedRoute) {
    }

    ngOnInit() {
        this.router.events.pipe(
            filter(event => event instanceof NavigationEnd),
            map(() => this.activatedRoute),
            map(route => route.firstChild),
            switchMap(route => route === null ? of({}) : route.data))
            .subscribe((data: any) => {
                debugger;
                this.showNavbar = !data.hideNavbar;
                this.showSidebar = !data.hideSidebar;
            });

        this.authStore.user.subscribe(user => {
            this.isLoggedIn = user !== null;
            this.userName = user === null ? "?" : user.firstName + " " + user.lastName;
        });
    }

    async handleLogOut() {
        this.authService.logOut();
        this.toastService.showSuccess("Logged out!");
        await this.router.navigateByUrl("/");
    }
}
