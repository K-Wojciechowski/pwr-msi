export interface PaymentGroupInfo {
    paidFromBalance: string;
    paidExternally: string;
    externalPaymentId: number | null;
    paymentUrl: string | null;
    isPaid: boolean;
}
