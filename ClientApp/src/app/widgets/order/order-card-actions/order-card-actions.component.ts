import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {Order} from "../../../models/order/order";
import {OrderAction} from "../../../models/order/order-action";
import {OrderStatus} from "../../../models/enum/order-status";
import {OrderUserRole} from "../../../models/enum/OrderUserRole";
import {AuthStoreService} from "../../../services/auth-store.service";
import {CurrencyPipe} from "@angular/common";

@Component({
    selector: 'app-order-card-actions',
    templateUrl: './order-card-actions.component.html',
    styleUrls: ['./order-card-actions.component.scss']
})
export class OrderCardActionsComponent implements OnInit, OnChanges {
    @Input("order") order!: Order;
    @Input("formFactor") formFactor: string = "unspecified";
    @Input("role") userRole: OrderUserRole = OrderUserRole.CUSTOMER;
    @Output("act") actSignal = new EventEmitter<string>();

    actions: OrderAction[] = [];
    isImportant: boolean = false;

    private currentUserId: number | null = null;

    constructor(private authStore: AuthStoreService, private currencyPipe: CurrencyPipe) {
    }

    ngOnInit(): void {
        this.computeActions(this.order);
        this.authStore.user.subscribe(u => this.currentUserId = u?.userId ?? -1);
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes.order) {
            this.computeActions(changes.order.currentValue);
        }
    }

    actionClick(a: OrderAction) {
        if (a.disabled) return;
        this.actSignal.emit(a.id);
    }

    private computeActions(order: Order) {
        const actions = [];
        let isImportant = true;
        if (order.status == OrderStatus.CREATED && this.userRole == OrderUserRole.CUSTOMER) {
            actions.push(new OrderAction("pay", "Pay " + this.currencyPipe.transform(order.totalPrice, "PLN")));
        }
        if ((order.status == OrderStatus.CREATED || order.status == OrderStatus.PAID) && this.userRole == OrderUserRole.CUSTOMER) {
            actions.push(new OrderAction("cancel", "Cancel order", "danger"));
            isImportant = false;
        }
        if ((order.status == OrderStatus.PAID) && this.userRole == OrderUserRole.MANAGER) {
            actions.push(new OrderAction("accept", "Accept order", "success"));
            actions.push(new OrderAction("reject", "Reject order", "danger"));
        }
        if (order.status == OrderStatus.ACCEPTED && this.userRole == OrderUserRole.MANAGER) {
            actions.push(new OrderAction("prepare", "Mark as prepared"));
        }
        if (order.status == OrderStatus.PREPARED && this.userRole == OrderUserRole.MANAGER) {
            actions.push(new OrderAction("assign", "Assign delivery person"));
        }
        if (order.status == OrderStatus.PREPARED && this.order.deliveryPerson === null && this.userRole == OrderUserRole.DELIVERY) {
            actions.push(new OrderAction("selfassign", "Assign to me"));
        }
        if (order.status == OrderStatus.PREPARED && this.order.deliveryPerson?.userId === this.currentUserId && this.userRole == OrderUserRole.DELIVERY) {
            actions.push(new OrderAction("deliver", "Mark as delivered", "success"));
        }
        this.actions = actions;
        this.isImportant = !!actions && isImportant;
    }
}
