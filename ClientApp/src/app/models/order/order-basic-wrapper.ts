import {ObjectWrapper} from "../object-wrapper";
import {OrderBasic} from "./order-basic";
import {OrderStatus} from "../enum/order-status";

export class OrderBasicWrapper extends ObjectWrapper<OrderBasic> {
    private linkBase: string;

    constructor(order: OrderBasic, linkBase: string) {
        super(order);
        this.linkBase = linkBase;
    }

    get statusText(): string {
        switch (this.v.status) {
            case OrderStatus.CREATED:
                return "New";
            case OrderStatus.PAID:
                return "Paid";
            case OrderStatus.ACCEPTED:
                return "In preparation";
            case OrderStatus.PREPARED:
                return "In delivery";
            case OrderStatus.DELIVERED:
                return "Delivered";
            case OrderStatus.COMPLETED:
                return "Completed";
            case OrderStatus.REJECTED:
                return "Rejected";
            case OrderStatus.CANCELLED:
                return "Cancelled";
            case OrderStatus.DECIDED:
                return "Decided";
        }
        return "?";
    }

    get itemNames(): string {
        return this.v.itemNames.join(", ");
    }

    get customerName(): string {
        return this.v.customer.firstName + " " + this.v.customer.lastName;
    }

    get deliveryPersonName(): string {
        return this.v.deliveryPerson ? this.v.deliveryPerson.firstName + " " + this.v.deliveryPerson.lastName : "—";
    }

    get routerLink(): string {
        return this.linkBase + this.v.orderId;
    }
}
