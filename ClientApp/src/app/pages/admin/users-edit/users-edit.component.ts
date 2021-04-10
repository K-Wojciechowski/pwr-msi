import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {ActivatedRoute} from "@angular/router";
import {UserAdmin} from "../../../models/user-admin";
import {PasswordResetInput} from "../../../models/password-reset-input";

@Component({
    selector: 'app-users-edit',
    templateUrl: './users-edit.component.html',
    styleUrls: ['./users-edit.component.scss']
})
export class UsersEditComponent implements OnInit {
    showLoading: boolean = false;
    user: UserAdmin | undefined;
    userId!: number;

    constructor(private http: HttpClient, private toastService: ToastService, private route: ActivatedRoute) {
    }

    ngOnInit(): void {
        this.loadUser();
    }

    loadUser() {
        this.showLoading = true;
        const userIdString = this.route.snapshot.paramMap.get("id");
        if (userIdString === null) return;
        this.userId = parseInt(userIdString);
        this.http.get<UserAdmin>(this.endpoint).subscribe(user => {
            this.user = user;
            this.showLoading = false;
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        })
    }

    get endpoint() {
        return `/api/admin/users/${this.userId}/`;
    }

    get passwordEndpoint() {
        return this.endpoint + "password/";
    }

    editUser(user: UserAdmin) {
        this.showLoading = true;
        this.http.put<UserAdmin>(this.endpoint, user).subscribe(newUser => {
            this.showLoading = false;
            this.user = newUser;
            this.toastService.showSuccess("Changes saved.");
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
    }

    resetPassword(data: PasswordResetInput) {
        this.showLoading = true;
        this.http.post(this.passwordEndpoint, data).subscribe(() => {
            this.showLoading = false;
            this.toastService.showSuccess("Password has been reset.");
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
    }
}
