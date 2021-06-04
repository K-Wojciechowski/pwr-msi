import {RestaurantMenuItem} from "./menu/restaurant-menu-item";
import {MenuItemOptionListWrapper} from "./menu-item-option-list-wrapper";
import {flatMap, sumBy} from "lodash";
import {OrderItem} from "./order/order-item";

export class MenuItemWrapper {
    public value: RestaurantMenuItem;
    public options: MenuItemOptionListWrapper[];
    public isHighlighted: boolean;
    public total: number;
    public notes: string;

    constructor(value: RestaurantMenuItem, options: MenuItemOptionListWrapper[] = [], isHighlighted: boolean = false) {
        this.value = value;
        this.options = options;
        this.isHighlighted = isHighlighted;
        this.total = parseFloat(value.price);
        this.notes = "";
    }

    recalculateTotal() {
        this.total = parseFloat(this.value.price) + sumBy(
            flatMap(this.options, ol => ol.getChoices()),
            oi => parseFloat(oi.value.price)
        );
    }

    toggle() {
        this.isHighlighted = !this.isHighlighted;
    }

    asOrderItem(): OrderItem {
        return {
            amount: "1", // no way in UI to order a different amount
            totalPrice: this.total.toString(),
            notes: this.notes,
            menuItem: this.value,
            customizations: flatMap(this.options, o => o.asOrderItemCustomizations())
        };
    }
}
