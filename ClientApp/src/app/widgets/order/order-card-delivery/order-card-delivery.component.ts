import {Component, Input, OnInit} from '@angular/core';
import {Order} from "../../../models/order/order";

@Component({
    selector: 'app-order-card-delivery',
    templateUrl: './order-card-delivery.component.html',
    styleUrls: ['./order-card-delivery.component.scss']
})
export class OrderCardDeliveryComponent implements OnInit {
    @Input("order") order!: Order;
    @Input("formFactor") formFactor: string = "unspecified";

    constructor() {
    }

    ngOnInit(): void {
    }

}
