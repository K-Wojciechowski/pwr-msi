import {RestaurantBasic} from "../restaurant-basic";
import {UserBasic} from "../user-basic";
import {Address} from "../address";
import {OrderStatus} from "../enum/order-status";

export interface OrderBasic {
    orderId: number;
    restaurant: RestaurantBasic;
    customer: UserBasic;
    address: Address;
    stauts: OrderStatus;
    totalPrice: string;
    deliveryNotes: string | null;

    created: string;
    updated: string;
    delivered: string | null;
}
