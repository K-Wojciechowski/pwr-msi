import {Component, OnInit} from '@angular/core';
import {RestaurantAdmin} from "../../../models/restaurant-admin";
import {RestaurantUser} from "../../../models/restaurant-user";
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {ActivatedRoute} from "@angular/router";
import {RestaurantEditorOutput} from "../../../models/restaurant-editor-output";
import {UsersRestaurantsEditorComponent} from "../users-restaurants-editor/users-restaurants-editor.component";

@Component({
    selector: 'app-restaurants-edit',
    templateUrl: './restaurants-edit.component.html',
    styleUrls: ['./restaurants-edit.component.scss']
})
export class RestaurantsEditComponent implements OnInit {
    showLoading: number = 0;
    restaurant: RestaurantAdmin | undefined;
    restaurantUsers: RestaurantUser[] | undefined;
    restaurantId!: number;

    constructor(private http: HttpClient, private toastService: ToastService, private route: ActivatedRoute) {
    }

    ngOnInit(): void {
        this.loadRestaurant();
    }

    loadRestaurant() {
        this.showLoading = 2;
        const restaurantIdString = this.route.snapshot.paramMap.get("id");
        if (restaurantIdString === null) return;
        this.restaurantId = parseInt(restaurantIdString);
        this.http.get<RestaurantAdmin>(this.endpoint).subscribe(restaurant => {
            this.restaurant = restaurant;
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
        return `/api/admin/restaurants/${this.restaurantId}/`;
    }

    get restaurantUsersEndpoint() {
        return this.endpoint + "users/";
    }

    editRestaurant(restaurantEditorOutput: RestaurantEditorOutput) {
        const {restaurant, restaurantUsers} = restaurantEditorOutput;
        this.showLoading = 2;
        this.http.put<RestaurantAdmin>(this.endpoint, restaurant).subscribe(newRestaurant => {
            this.showLoading -= 1;
            this.restaurant = newRestaurant;
            this.saveRestaurantUsers(newRestaurant, restaurantUsers);
            this.toastService.showSuccess(`Changes to ${newRestaurant.name} saved.`);
            this.saveRestaurantUsers(newRestaurant, restaurantUsers);
        }, error => {
            this.showLoading = 0;
            this.toastService.handleHttpError(error);
        });
    }

    saveRestaurantUsers(restaurant: RestaurantAdmin, restaurantUsers: RestaurantUser[]) {
        const updatedRestaurantUsers = UsersRestaurantsEditorComponent.updateWithRestaurant(restaurantUsers, restaurant);
        this.http.post<RestaurantUser[]>(this.restaurantUsersEndpoint, updatedRestaurantUsers).subscribe(restaurantUsers => {
            this.restaurantUsers = restaurantUsers;
            this.toastService.showSuccess(`Permissions for ${restaurant.name} saved.`);
            this.showLoading = 0;
        }, error => {
            this.showLoading = 0;
            this.toastService.handleHttpError(error);
        });
    }
}
