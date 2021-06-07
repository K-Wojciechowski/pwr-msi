import {Component, Input, OnInit} from '@angular/core';
import {Order} from "../../../models/order/order";

@Component({
    selector: 'app-order-card-notes',
    templateUrl: './order-card-notes.component.html',
    styleUrls: ['./order-card-notes.component.scss']
})
export class OrderCardNotesComponent implements OnInit {
    @Input("order") order!: Order;
    @Input("formFactor") formFactor: string = "unspecified";

    constructor() {
    }

    ngOnInit(): void {
    }

}
