import { Component, OnInit } from '@angular/core';
import {NgForm} from "@angular/forms";
import {AuthService} from "../../../services/auth.service";
import {ToastService} from "../../../services/toast.service";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
    selector: 'app-log-in',
    templateUrl: './log-in.component.html',
    styleUrls: ['./log-in.component.scss']
})
export class LogInComponent implements OnInit {
    showLoading: boolean = false;
    nextUrl: string | null = null;

    constructor(private authService: AuthService, private toastService: ToastService, private router: Router, private route: ActivatedRoute) {
    }

    ngOnInit(): void {
        this.route.queryParamMap.subscribe(queryParamMap => {
            this.nextUrl = queryParamMap.get("next");
        });
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
                const nextUrl = this.nextUrl != null && this.nextUrl.startsWith("/") ? this.nextUrl : "/";
                await this.router.navigateByUrl(nextUrl);
            },
            error => {
                this.showLoading = false;
                console.log(error);
                this.toastService.showError("Failed to log in. Please check your credentials.")
            }
        )

    }
}
