import {OrderStatus} from "../enum/order-status";
import {RestaurantBasic} from "../restaurant-basic";
import {UserBasic} from "../user-basic";
import {Address} from "../address";
import {OrderItem} from "./order-item";

export interface Order {
    orderId?: number;
    totalPrice: string;
    deliveryNotes: string;
    status: OrderStatus;

    created: string;
    updated: string;

    restaurant: RestaurantBasic;
    customer?: UserBasic;
    deliveryPerson: UserBasic | null;
    address: Address;

    items: OrderItem[];
}
