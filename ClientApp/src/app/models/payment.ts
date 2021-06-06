import {PaymentStatus} from "./enum/payment-status";
import {OrderBasic} from "./order/order-basic";

export interface Payment {
    paymentId: number;
    isReturn: boolean;
    isTargettingBalance: boolean;
    isBalanceRepayment: boolean;
    amount: string;
    status: PaymentStatus;
    errorMessage: string | null;
    order: OrderBasic | null;

    created: string;
    updated: string;
}
