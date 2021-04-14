import {Component, OnInit} from '@angular/core';
import {UserAdmin, UserCreateAdmin} from "../../../models/user-admin";
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {Router} from "@angular/router";
import {UserEditorOutput} from "../../../models/user-editor-output";
import {RestaurantUser} from "../../../models/restaurant-user";
import {UsersRestaurantsEditorComponent} from "../users-restaurants-editor/users-restaurants-editor.component";

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

    addUser(userEditorOutput: UserEditorOutput) {
        const {user, restaurantUsers} = userEditorOutput;
        this.showLoading = true;
        this.http.post<UserAdmin>("/api/admin/users/", user).subscribe(async newUser => {
            this.toastService.showSuccess(`User ${newUser.username} created.`);
            this.saveRestaurantUsers(newUser, restaurantUsers);
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
    }

    saveRestaurantUsers(user: UserAdmin, restaurantUsers: RestaurantUser[]) {
        const updatedRestaurantUsers = UsersRestaurantsEditorComponent.updateWithUser(restaurantUsers, user);
        this.http.post<RestaurantUser[]>(`/api/admin/users/${user.userId}/restaurants/`, updatedRestaurantUsers).subscribe(async () => {
            this.toastService.showSuccess(`Permissions for ${user.username} saved.`);
            this.showLoading = false;
            await this.router.navigate(["admin", "users", user.userId]);
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
    }
}
