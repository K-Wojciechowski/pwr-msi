import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from "@angular/router";
import {PaymentGroupInfo} from "../../../models/payment-group-info";

@Component({
    selector: 'app-orders-make-payment',
    templateUrl: './orders-make-payment.component.html',
    styleUrls: ['./orders-make-payment.component.scss']
})
export class OrdersMakePaymentComponent implements OnInit {

    public showLoading = false;
    public showSuccess = false;
    public showHttpError = false;
    public isBalanceRepayment = false;

    constructor(private http: HttpClient, private route: ActivatedRoute) {
    }

    ngOnInit(): void {
        const orderId = this.route.snapshot.paramMap.get("id");
        const endpoint = `/api/orders/${orderId}/pay`;
        this.showLoading = true;

        this.http.post<PaymentGroupInfo>(endpoint, null).subscribe(
            attempt => this.handleAttempt(attempt),
            () => {
                this.showSuccess = false;
                this.showHttpError = true;
                this.showLoading = false;
            }
        );
    }

    handleAttempt(attempt: PaymentGroupInfo) {
        this.showHttpError = false;
        if (!!attempt.paymentUrl) {
            const baseUrl = new URL(`/payments/${attempt.externalPaymentId}/check`, window.location.toString());
            const queryParams = new URLSearchParams();
            queryParams.set("next", baseUrl.toString());
            const destinationUrl = new URL(attempt.paymentUrl);
            destinationUrl.search = queryParams.toString();
            window.location.assign(destinationUrl.toString());
            return;
        }
        this.showLoading = false;
        this.showSuccess = attempt.isPaid;
    }
}
