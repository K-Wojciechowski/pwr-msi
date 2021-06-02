import {RestaurantMenuItemOptionItem} from "./restaurant-menu-management/restaurant-menu-item-option-item";

export class MenuItemOptionItemWrapper {
    public value: RestaurantMenuItemOptionItem;
    public checked: boolean;

    constructor(value: RestaurantMenuItemOptionItem, checked: boolean = false) {
        this.value = value;
        this.checked = checked;
    }
}
