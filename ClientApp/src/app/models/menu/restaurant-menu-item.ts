import {AmountUnit} from "../enum/amount-unit";
import {RestaurantMenuItemOptionList} from "./restaurant-menu-item-option-list";

export interface RestaurantMenuItem {
    menuItemId?: number | null;
    name: string;
    description: string;
    image: string | null;
    price: string;
    amount: string;
    amountUnit: AmountUnit;
    validFrom?: string | null;
    validUntil?: string | null;
    menuCategoryId: number;
    menuOrder: number;
    options: RestaurantMenuItemOptionList[];
}
