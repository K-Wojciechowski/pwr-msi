import { Component, OnInit } from '@angular/core';
import {RouteComponent} from "../../../utils/route-component";
import {NgForm} from "@angular/forms";
import {AuthService} from "../../../services/auth.service";
import {ToastService} from "../../../services/toast.service";
import {Router} from "@angular/router";

@Component({
    selector: 'app-log-in',
    templateUrl: './log-in.component.html',
    styleUrls: ['./log-in.component.scss']
})
export class LogInComponent extends RouteComponent implements OnInit {
    USE_SIDEBAR: boolean = false;
    USE_NAVBAR: boolean = false;

    username: string = "";
    password: string = "";
    showLoading: boolean = false;

    constructor(private authService: AuthService, private toastService: ToastService, private router: Router) {
        super();
    }

    ngOnInit(): void {
    }

    canSubmit(f: NgForm) {
        return f.value.username !== undefined && f.value.password !== undefined && f.value.username.length > 0 && f.value.password.length > 0;
    }

    submit(f: NgForm) {
        if (!this.canSubmit(f)) {
            this.toastService.showError("Invalid data!");
            return;
        }
        this.showLoading = true;
        this.authService.logIn(f.value.username, f.value.password).subscribe(
            async access => {
                this.showLoading = false;
                this.toastService.showSuccess(`Welcome, ${access.profile.firstName}!`);
                await this.router.navigateByUrl("/");
            },
            error => {
                this.showLoading = false;
                console.log(error);
                this.toastService.showError("Failed to log in. Please check your credentials.")
            }
        )

    }
}
