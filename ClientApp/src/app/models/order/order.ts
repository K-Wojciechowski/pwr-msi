import {OrderStatus} from "../enum/order-status";
import {RestaurantBasicAddress} from "../restaurant-basic-address";
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

    restaurant: RestaurantBasicAddress;
    customer?: UserBasic;
    deliveryPerson: UserBasic | null;
    address: Address;

    items: OrderItem[];
}
