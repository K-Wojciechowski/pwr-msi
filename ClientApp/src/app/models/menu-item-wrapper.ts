import {RestaurantMenuItem} from "./restaurant-menu-management/restaurant-menu-item";
import {MenuItemOptionListWrapper} from "./menu-item-option-list-wrapper";
import {flatMap, sumBy} from "lodash";

export class MenuItemWrapper {
    public value: RestaurantMenuItem;
    public options: MenuItemOptionListWrapper[];
    public isHighlighted: boolean;
    public total: number;

    constructor(value: RestaurantMenuItem, options: MenuItemOptionListWrapper[] = [], isHighlighted: boolean = false) {
        this.value = value;
        this.options = options;
        this.isHighlighted = isHighlighted;
        this.total = parseFloat(value.price);
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
}
