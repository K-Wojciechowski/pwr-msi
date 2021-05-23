import {RestaurantMenuItemOptionItem} from "./restaurant-menu-item-option-item";

export interface RestaurantMenuItemOptionList {
    menuItemOptionListId?: number;
    name: string;
    isMultipleChoice: boolean;
    menuItemOptionListOrder: number;
    menuItemId?: number;
    items: RestaurantMenuItemOptionItem[];
}
