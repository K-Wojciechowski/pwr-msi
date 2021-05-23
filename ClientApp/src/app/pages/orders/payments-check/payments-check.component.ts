import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from "@angular/router";
import {ToastService} from "../../../services/toast.service";
import {Payment} from "../../../models/payment";
import {PaymentStatus} from "../../../models/enum/payment-status";

const ATTEMPTS_WARNING_AFTER = 5;
const ATTEMPTS_STOP_AFTER = 20;
const REFRESH_EVERY = 2500;

@Component({
    selector: 'app-payments-check',
    templateUrl: './payments-check.component.html',
    styleUrls: ['./payments-check.component.scss']
})
export class PaymentsCheckComponent implements OnInit {
    public showLoading: boolean = true;
    public showSuccess: boolean = false;
    public showThankYou: boolean = false;
    public showFailure: boolean = false;
    public showTimeWarning: boolean = false;
    public showGoToOrder: boolean = false;
    public orderId: number = 0;
    public paymentId: number = 0;
    private attemptCounter = 0;

    constructor(private http: HttpClient, private activatedRoute: ActivatedRoute, private toastService: ToastService) {
    }

    ngOnInit(): void {
        const paymentIdString = this.activatedRoute.snapshot.paramMap.get("id");
        if (paymentIdString == null) return;
        this.paymentId = parseInt(paymentIdString);
        this.loadPayment();
    }

    loadPayment(): void {
        this.http.get<Payment>(`/api/payments/${this.paymentId}`).subscribe(payment => {
            this.showLoading = payment.status == PaymentStatus.CREATED || payment.status == PaymentStatus.REQUESTED;
            this.showSuccess = payment.status == PaymentStatus.COMPLETED;
            this.showFailure = payment.status == PaymentStatus.FAILED || payment.status == PaymentStatus.CANCELLED || this.attemptCounter > ATTEMPTS_STOP_AFTER;
            this.showThankYou = !payment.isReturn && !payment.isBalanceRepayment;
            this.showLoading = this.showLoading && !this.showSuccess && !this.showFailure;
            this.showTimeWarning = this.attemptCounter > ATTEMPTS_WARNING_AFTER;
            this.attemptCounter++;
            if (this.showLoading) {
                window.setTimeout(() => this.loadPayment(), REFRESH_EVERY);
            }
        }, error => {
            this.toastService.handleHttpError(error);
            this.attemptCounter++;
            this.showTimeWarning = this.attemptCounter > ATTEMPTS_WARNING_AFTER;
            if (this.attemptCounter > ATTEMPTS_STOP_AFTER) {
                this.showFailure = true;
                this.showSuccess = false;
                this.showLoading = false;
            } else {
                window.setTimeout(() => this.loadPayment(), REFRESH_EVERY);
            }
        });
    }

}
