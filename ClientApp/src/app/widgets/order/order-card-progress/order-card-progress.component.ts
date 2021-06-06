import {Component, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import {Order} from "../../../models/order/order";
import {OrderStatus} from "../../../models/enum/order-status";

@Component({
    selector: 'app-order-card-progress',
    templateUrl: './order-card-progress.component.html',
    styleUrls: ['./order-card-progress.component.scss']
})
export class OrderCardProgressComponent implements OnInit, OnChanges {
    @Input("order") order!: Order;
    orderProgress: number = 0;
    showCancellation: boolean = false;
    showRejection: boolean = false;

    constructor() {
    }

    ngOnInit(): void {
        this.updateOrderProgress(this.order);
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes.order) {
            this.updateOrderProgress(changes.order.currentValue);
        }
    }

    private static getOrderProgress(status: OrderStatus): number {
        switch (status) {
            case OrderStatus.CREATED:
                return 0;
            case OrderStatus.PAID:
                return 1;
            case OrderStatus.DECIDED:
                return 1;
            case OrderStatus.ACCEPTED:
                return 2;
            case OrderStatus.PREPARED:
                return 3;
            case OrderStatus.DELIVERED:
                return 4;
            case OrderStatus.COMPLETED:
                return 5;
            case OrderStatus.CANCELLED:
                return -1;
            case OrderStatus.REJECTED:
                return -2;
        }
        return 0;
    }

    private updateOrderProgress(order: Order) {
        this.orderProgress = OrderCardProgressComponent.getOrderProgress(order.status);
        this.showCancellation = this.orderProgress == -1;
        this.showRejection = this.orderProgress == -2;
        this.orderProgress = this.orderProgress < -1 ? -this.orderProgress : this.orderProgress;
    }
}
