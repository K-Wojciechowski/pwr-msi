import {RestaurantBasic} from "../restaurant-basic";
import {UserBasic} from "../user-basic";
import {Address} from "../address";
import {OrderStatus} from "../enum/order-status";

export interface OrderBasic {
    orderId: number;
    restaurant: RestaurantBasic;
    customer: UserBasic;
    deliveryPerson?: UserBasic | null;
    address: Address;
    status: OrderStatus;
    totalPrice: string;
    deliveryNotes: string | null;
    itemNames: string[];

    created: string;
    updated: string;
    delivered: string | null;
}
