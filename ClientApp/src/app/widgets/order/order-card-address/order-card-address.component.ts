import {Component, Input, OnInit} from '@angular/core';
import {Order} from "../../../models/order/order";

@Component({
    selector: 'app-order-card-address',
    templateUrl: './order-card-address.component.html',
    styleUrls: ['./order-card-address.component.scss']
})
export class OrderCardAddressComponent implements OnInit {
    @Input("order") order!: Order;
    @Input("formFactor") formFactor: string = "unspecified";

    constructor() {
    }

    ngOnInit(): void {
    }

}
