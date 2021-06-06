import {Component, Input, OnInit} from '@angular/core';
import {Order} from "../../../models/order/order";
import {OrderItem} from "../../../models/order/order-item";
import {OrderItemCustomization} from "../../../models/order/order-item-customization";
import {flatMap, groupBy, sumBy} from "lodash";
import {OrderItemCustomizationGroup} from "../../../models/order/order-item-customization-group";

@Component({
    selector: 'app-order-items',
    templateUrl: './order-items.component.html',
    styleUrls: ['./order-items.component.scss']
})
export class OrderItemsComponent implements OnInit {
    @Input("order") order!: Order;
    @Input("showTotal") showTotal: boolean = true;

    constructor() {
    }

    ngOnInit(): void {
    }

    groupCustomizations(orderItem: OrderItem): OrderItemCustomizationGroup[] {
        const groups = groupBy(orderItem.customizations, c => c.menuItemOptionItem.menuItemOptionListId);
        return flatMap(orderItem.menuItem.options, ol => {
            // @ts-ignore
            const inGroup = groups[ol.menuItemOptionListId];
            if (inGroup === undefined) return [];
            const group: OrderItemCustomizationGroup = {
                list: ol,
                items: inGroup.map((c: OrderItemCustomization) => c.menuItemOptionItem),
            };
            return [group];
        });
    }
}
