import {RestaurantMenuItem} from "../menu/restaurant-menu-item";
import {OrderItemCustomization} from "./order-item-customization";

export interface OrderItem {
    orderItemId?: number;
    amount: string;
    totalPrice: string;
    notes: string;
    menuItem: RestaurantMenuItem;
    customizations: OrderItemCustomization[];
}
