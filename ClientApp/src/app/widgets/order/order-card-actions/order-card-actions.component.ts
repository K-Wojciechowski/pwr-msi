import {Component, Input, OnInit} from '@angular/core';
import {Order} from "../../../models/order/order";

@Component({
    selector: 'app-order-card-actions',
    templateUrl: './order-card-actions.component.html',
    styleUrls: ['./order-card-actions.component.scss']
})
export class OrderCardActionsComponent implements OnInit {
    @Input("order") order!: Order;
    @Input("formFactor") formFactor: string = "unspecified";

    constructor() {
    }

    ngOnInit(): void {
    }

}
