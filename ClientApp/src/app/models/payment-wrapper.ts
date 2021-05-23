import {ObjectWrapper} from "./object-wrapper";
import {Payment} from "./payment";
import {PaymentStatus} from "./enum/payment-status";

export class PaymentWrapper extends ObjectWrapper<Payment> {
    get typeText(): string {
        if (this.v.isBalanceRepayment) return "Balance return";
        else if (this.v.isReturn) return "Return from order";
        else if (this.v.isTargettingBalance) return "Payment from balance";
        else return "Payment";
    }

    get statusText(): string {
        switch (this.v.status) {
            case PaymentStatus.CREATED:
                return "New";
            case PaymentStatus.REQUESTED:
                return "In progress";
            case PaymentStatus.COMPLETED:
                return "Completed";
            case PaymentStatus.FAILED:
                return "Failed";
            case PaymentStatus.CANCELLED:
                return "Cancelled";
        }
        return "?";
    }

    get canPay(): boolean {
        return this.v.status == PaymentStatus.CREATED || this.v.status == PaymentStatus.REQUESTED;
    }
}
