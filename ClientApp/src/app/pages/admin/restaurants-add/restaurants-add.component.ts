import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {Router} from "@angular/router";
import {RestaurantEditorOutput} from "../../../models/restaurant-editor-output";
import {RestaurantAdmin} from "../../../models/restaurant-admin";
import {RestaurantUser} from "../../../models/restaurant-user";
import {UsersRestaurantsEditorComponent} from "../users-restaurants-editor/users-restaurants-editor.component";

@Component({
    selector: 'app-restaurants-add',
    templateUrl: './restaurants-add.component.html',
    styleUrls: ['./restaurants-add.component.scss']
})
export class RestaurantsAddComponent implements OnInit {
    showLoading: boolean = false;

    defaultRestaurant = {
        name: "",
        website: "",
        description: "",
    };


    constructor(private http: HttpClient, private toastService: ToastService, private router: Router) {
    }

    ngOnInit(): void {
    }

    addRestaurant(restaurantEditorOutput: RestaurantEditorOutput) {
        const {restaurant, restaurantUsers} = restaurantEditorOutput;
        this.showLoading = true;
        this.http.post<RestaurantAdmin>("/api/admin/restaurants/", restaurant).subscribe(async newRestaurant => {
            this.toastService.showSuccess(`Restaurant ${newRestaurant.name} created.`);
            this.saveRestaurantUsers(newRestaurant, restaurantUsers);
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
    }

    saveRestaurantUsers(restaurant: RestaurantAdmin, restaurantUsers: RestaurantUser[]) {
        const updatedRestaurantUsers = UsersRestaurantsEditorComponent.updateWithRestaurant(restaurantUsers, restaurant);
        this.http.post<RestaurantUser[]>(`/api/admin/restaurants/${restaurant.restaurantId}/users/`, updatedRestaurantUsers).subscribe(async () => {
            this.toastService.showSuccess(`Permissions for ${restaurant.name} saved.`);
            this.showLoading = false;
            await this.router.navigate(["admin", "restaurants", restaurant.restaurantId]);
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
    }
}
