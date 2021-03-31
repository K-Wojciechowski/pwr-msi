import {Component, OnInit} from '@angular/core';
import {RouteComponent} from "../../../utils/route-component";
import {AuthService} from "../../../services/auth.service";
import {AuthStoreService} from "../../../services/auth-store.service";

@Component({
    selector: 'app-index',
    templateUrl: './index.component.html',
    styleUrls: ['./index.component.scss']
})
export class IndexComponent extends RouteComponent implements OnInit {
    showLoading: boolean = true;
    isAuthenticated: boolean = false;

    USE_NAVBAR: boolean = true;
    USE_SIDEBAR: boolean = false;


    constructor(private authService: AuthService, private authStore: AuthStoreService) {
        super();
        this.authService.ensureAuthReady();
    }

    ngOnInit(): void {
        this.authStore.user.subscribe(user => {
            this.isAuthenticated = user !== null;
        });
        this.authStore.authInitialized.subscribe(initialized => {
            this.showLoading = !initialized;
        })
    }

}
