import {UserAdmin, UserCreateAdmin} from "./user-admin";
import {RestaurantUser} from "./restaurant-user";

export interface UserEditorOutput {
    user: UserAdmin | UserCreateAdmin;
    restaurantUsers: RestaurantUser[];
}
