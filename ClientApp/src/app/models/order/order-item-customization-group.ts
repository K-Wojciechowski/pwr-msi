import {RestaurantMenuItemOptionList} from "../menu/restaurant-menu-item-option-list";
import {OrderItemCustomization} from "./order-item-customization";
import {RestaurantMenuItemOptionItem} from "../menu/restaurant-menu-item-option-item";

export interface OrderItemCustomizationGroup {
    list: RestaurantMenuItemOptionList;
    items: RestaurantMenuItemOptionItem[];
}
