import {Component, OnInit, TemplateRef} from '@angular/core';
import {PaymentWrapper} from "../../../models/payment-wrapper";
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {Payment} from "../../../models/payment";
import {NgbModal} from "@ng-bootstrap/ng-bootstrap";
import {ActivatedRoute} from "@angular/router";

@Component({
    selector: 'app-payments-info',
    templateUrl: './payments-info.component.html',
    styleUrls: ['./payments-info.component.scss']
})
export class PaymentsInfoComponent implements OnInit {
    public paymentId!: number;
    public payment: PaymentWrapper | null = null;
    public showLoading: boolean = false;

    constructor(private http: HttpClient, private activatedRoute: ActivatedRoute, private toastService: ToastService, private modalService: NgbModal) {
    }

    get makePaymentLink() {
        return `/payments/${this.paymentId}/make`;
    }

    ngOnInit(): void {
        const paymentIdString = this.activatedRoute.snapshot.paramMap.get("id");
        if (paymentIdString == null) return;
        this.paymentId = parseInt(paymentIdString);
        this.loadPayment();
    }

    loadPayment(): void {
        this.showLoading = true;
        this.http.get<Payment>(`/api/payments/${this.paymentId}`).subscribe(payment => {
            this.payment = new PaymentWrapper(payment);
            this.showLoading = false;
        }, error => {
            this.toastService.handleHttpError(error);
            this.showLoading = false;
        });
    }

    cancelPayment(): void {
        this.showLoading = true;
        this.http.delete<Payment>(`/api/payments/${this.paymentId}`).subscribe(() => {
            this.loadPayment();
        }, error => {
            this.toastService.handleHttpError(error);
            this.showLoading = false;
        });
    }

    openConfirmModal(confirmModal: TemplateRef<any>) {
        this.modalService.open(confirmModal, {ariaLabelledBy: 'modal-basic-title'}).result.then(() => {
            this.cancelPayment();
        });
    }
}
