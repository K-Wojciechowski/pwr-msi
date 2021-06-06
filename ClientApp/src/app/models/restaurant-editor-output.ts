import {RestaurantFull} from "./restaurant-full";
import {RestaurantUser} from "./restaurant-user";

export interface RestaurantEditorOutput {
    restaurant: RestaurantFull;
    restaurantUsers: RestaurantUser[];
}
