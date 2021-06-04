import {RestaurantMenuItemOptionItem} from "../menu/restaurant-menu-item-option-item";

export interface OrderItemCustomization {
    orderItemCustomizationId?: number;
    menuItemOptionItem: RestaurantMenuItemOptionItem;
}
