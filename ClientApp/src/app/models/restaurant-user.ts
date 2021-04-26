import {UserBasic} from "./user-basic";
import {RestaurantBasic} from "./restaurant-basic";

export interface RestaurantUser {
    user?: UserBasic;
    restaurant?: RestaurantBasic;
    canManage: boolean;
    canAcceptOrders: boolean;
    canDeliverOrders: boolean;
}
