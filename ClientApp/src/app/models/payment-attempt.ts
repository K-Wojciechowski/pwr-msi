import {PaymentStatus} from "./enum/payment-status";

export interface PaymentAttempt {
    paymentId: number;
    paymentStatus: PaymentStatus;
    paymentUrl: string;
}
