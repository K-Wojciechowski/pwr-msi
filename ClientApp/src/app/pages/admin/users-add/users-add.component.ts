import {Component, OnInit} from '@angular/core';
import {UserAdmin, UserCreateAdmin} from "../../../models/user-admin";
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {Router} from "@angular/router";

@Component({
    selector: 'app-users-add',
    templateUrl: './users-add.component.html',
    styleUrls: ['./users-add.component.scss']
})
export class UsersAddComponent implements OnInit {
    showLoading: boolean = false;

    defaultUser = {
        userId: undefined,
        email: "",
        firstName: "",
        lastName: "",
        username: "",
        password: "",
        repeatPassword: "",
        balance: "",
        isActive: true,
        isAdmin: false,
        isVerified: false,
        billingAddress: null
    };


    constructor(private http: HttpClient, private toastService: ToastService, private router: Router) {
    }

    ngOnInit(): void {
    }

    addUser(user: UserAdmin) {
        this.showLoading = true;
        this.http.post<UserAdmin>("/api/admin/users/", user).subscribe(async newUser => {
            this.showLoading = false;
            this.toastService.showSuccess(`User ${newUser.username} created.`);
            await this.router.navigate(["admin", "users", newUser.userId]);
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
    }
}
