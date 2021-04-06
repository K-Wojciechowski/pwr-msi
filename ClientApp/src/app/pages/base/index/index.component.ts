import {Component, OnInit} from '@angular/core';
import {AuthService} from "../../../services/auth.service";
import {AuthStoreService} from "../../../services/auth-store.service";

@Component({
    selector: 'app-index',
    templateUrl: './index.component.html',
    styleUrls: ['./index.component.scss']
})
export class IndexComponent implements OnInit {
    showLoading: boolean = true;
    isAuthenticated: boolean = false;

    constructor(private authService: AuthService, private authStore: AuthStoreService) {
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
