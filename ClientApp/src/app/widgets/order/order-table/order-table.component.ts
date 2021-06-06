import {Component, Input, OnInit} from '@angular/core';
import {OrderBasicWrapper} from "../../../models/order/order-basic-wrapper";

@Component({
    selector: 'app-order-table',
    templateUrl: './order-table.component.html',
    styleUrls: ['./order-table.component.scss']
})
export class OrderTableComponent implements OnInit {
    @Input("showLoading") showLoading: boolean = false;
    @Input("showRestaurant") showRestaurant: boolean = false;
    @Input("showCustomer") showCustomer: boolean = false;
    @Input("showDelivery") showDelivery: boolean = false;
    @Input("ilnkBase") linkBase: string = "/order/";
    @Input("orders") items!: OrderBasicWrapper[];

    constructor() {
    }

    ngOnInit(): void {
    }

}
