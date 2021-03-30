import {Component, OnInit} from '@angular/core';
import {RouteComponent} from "../../../utils/route-component";
import {AuthService} from "../../../services/auth.service";

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


    constructor(private authService: AuthService) {
        super();
    }

    ngOnInit(): void {
        this.authService.user.subscribe(user => {
            this.isAuthenticated = user !== null;
        });
        this.authService.authInitialized.subscribe(initialized => {
            this.showLoading = !initialized;
        })
    }

}
