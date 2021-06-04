import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {PaymentAttempt} from "../../../models/payment-attempt";
import {PaymentStatus} from "../../../models/enum/payment-status";

@Component({
    selector: 'app-payments-make',
    templateUrl: './payments-make.component.html',
    styleUrls: ['./payments-make.component.scss']
})
export class PaymentsMakeComponent implements OnInit {
    public showLoading = false;
    public showSuccess = false;
    public showHttpError = false;
    public isBalanceRepayment = false;

    constructor(private http: HttpClient, private route: ActivatedRoute) {
    }

    ngOnInit(): void {
        const paymentId = this.route.snapshot.paramMap.get("id");
        this.isBalanceRepayment = this.route.snapshot.data.isBalanceRepayment === true;
        const endpoint = this.isBalanceRepayment ? "/api/payments/balance/repay/" : `/api/payments/${paymentId}/`;
        this.showLoading = true;

        this.http.post<PaymentAttempt>(endpoint, null).subscribe(
            attempt => this.handleAttempt(attempt),
            () => {
                this.showSuccess = false;
                this.showHttpError = true;
                this.showLoading = false;
            }
        );
    }

    handleAttempt(attempt: PaymentAttempt) {
        this.showHttpError = false;
        if (!!attempt.paymentUrl) {
            const baseUrl = new URL(`/payments/${attempt.paymentId}/check`, window.location.toString());
            const queryParams = new URLSearchParams();
            queryParams.set("next", baseUrl.toString());
            const destinationUrl = new URL(attempt.paymentUrl);
            destinationUrl.search = queryParams.toString();
            window.location.assign(destinationUrl.toString());
            return;
        }
        this.showLoading = false;
        this.showSuccess = attempt.paymentStatus == PaymentStatus.COMPLETED;
    }
}
