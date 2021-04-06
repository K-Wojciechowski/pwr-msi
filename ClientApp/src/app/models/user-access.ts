import {UserProfile} from "./user-profile";
import {RestaurantBasic} from "./restaurant-basic";

export interface UserAccess {
    profile: UserProfile;
    admin: boolean;
    manage: RestaurantBasic[];
    accept: RestaurantBasic[];
    deliver: RestaurantBasic[];
}
