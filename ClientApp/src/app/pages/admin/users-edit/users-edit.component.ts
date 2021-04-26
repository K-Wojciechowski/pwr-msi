import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {ActivatedRoute} from "@angular/router";
import {UserAdmin} from "../../../models/user-admin";
import {PasswordResetInput} from "../../../models/password-reset-input";
import {RestaurantUser} from "../../../models/restaurant-user";
import {UserEditorOutput} from "../../../models/user-editor-output";
import {UsersRestaurantsEditorComponent} from "../users-restaurants-editor/users-restaurants-editor.component";

@Component({
    selector: 'app-users-edit',
    templateUrl: './users-edit.component.html',
    styleUrls: ['./users-edit.component.scss']
})
export class UsersEditComponent implements OnInit {
    showLoading: number = 0;
    user: UserAdmin | undefined;
    restaurantUsers: RestaurantUser[] | undefined;
    userId!: number;

    constructor(private http: HttpClient, private toastService: ToastService, private route: ActivatedRoute) {
    }

    ngOnInit(): void {
        this.loadUser();
    }

    loadUser() {
        this.showLoading = 2;
        const userIdString = this.route.snapshot.paramMap.get("id");
        if (userIdString === null) return;
        this.userId = parseInt(userIdString);
        this.http.get<UserAdmin>(this.endpoint).subscribe(user => {
            this.user = user;
            this.showLoading -= 1;
        }, error => {
            this.showLoading -= 1;
            this.toastService.handleHttpError(error);
        });
        this.http.get<RestaurantUser[]>(this.restaurantUsersEndpoint).subscribe(restaurantUsers => {
            this.restaurantUsers = restaurantUsers;
            this.showLoading -= 1;
        }, error => {
            this.showLoading -= 1;
            this.toastService.handleHttpError(error);
        });
    }

    get endpoint() {
        return `/api/admin/users/${this.userId}/`;
    }

    get passwordEndpoint() {
        return this.endpoint + "password/";
    }

    get restaurantUsersEndpoint() {
        return this.endpoint + "restaurants/";
    }

    editUser(userEditorOutput: UserEditorOutput) {
        const {user, restaurantUsers} = userEditorOutput;
        this.showLoading = 2;
        this.http.put<UserAdmin>(this.endpoint, user).subscribe(newUser => {
            this.showLoading -= 1;
            this.user = newUser;
            this.saveRestaurantUsers(newUser, restaurantUsers);
            this.toastService.showSuccess(`Changes to ${newUser.username} saved.`);
            this.saveRestaurantUsers(newUser, restaurantUsers);
        }, error => {
            this.showLoading = 0;
            this.toastService.handleHttpError(error);
        });
    }

    resetPassword(data: PasswordResetInput) {
        this.showLoading = 1;
        this.http.post(this.passwordEndpoint, data).subscribe(() => {
            this.showLoading = 0;
            this.toastService.showSuccess("Password has been reset.");
        }, error => {
            this.showLoading = 0;
            this.toastService.handleHttpError(error);
        });
    }

    saveRestaurantUsers(user: UserAdmin, restaurantUsers: RestaurantUser[]) {
        const updatedRestaurantUsers = UsersRestaurantsEditorComponent.updateWithUser(restaurantUsers, user);
        this.http.post<RestaurantUser[]>(this.restaurantUsersEndpoint, updatedRestaurantUsers).subscribe(restaurantUsers => {
            this.restaurantUsers = restaurantUsers;
            this.toastService.showSuccess(`Permissions for ${user.username} saved.`);
            this.showLoading = 0;
        }, error => {
            this.showLoading = 0;
            this.toastService.handleHttpError(error);
        });
    }
}
