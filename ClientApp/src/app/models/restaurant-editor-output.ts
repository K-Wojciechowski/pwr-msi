import {RestaurantAdmin} from "./restaurant-admin";
import {RestaurantUser} from "./restaurant-user";

export interface RestaurantEditorOutput {
    restaurant: RestaurantAdmin;
    restaurantUsers: RestaurantUser[];
}
