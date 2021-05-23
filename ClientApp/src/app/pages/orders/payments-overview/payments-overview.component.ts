import {Component, OnInit} from '@angular/core';
import {PaymentWrapper} from "../../../models/payment-wrapper";
import {HttpClient} from "@angular/common/http";
import {MsiHttpService} from "../../../services/msi-http.service";
import {Payment} from "../../../models/payment";
import {ToastService} from "../../../services/toast.service";
import {ResultDto} from "../../../models/result-dto";

@Component({
    selector: 'app-payments-overview',
    templateUrl: './payments-overview.component.html',
    styleUrls: ['./payments-overview.component.scss']
})
export class PaymentsOverviewComponent implements OnInit {
    public showLoading = 0;
    public balance = 0;
    public pendingPayments: PaymentWrapper[] = [];
    public paymentHistory: PaymentWrapper[] = [];
    public historyPageNumber = 1;
    public historyTotalItems = 0;


    constructor(private http: HttpClient, private msiHttp: MsiHttpService, private toastService: ToastService) {
    }

    ngOnInit(): void {
        this.loadBalance();
        this.loadPendingPayments();
        this.loadHistory();
    }

    goToHistoryPage(pageNumber: number) {
        this.historyPageNumber = pageNumber;
        this.loadHistory();
    }

    loadPendingPayments() {
        this.showLoading++;
        this.http.get<Payment[]>("/api/payments/pending/").subscribe(
            payments => {
                this.pendingPayments = payments.map(i => new PaymentWrapper(i));
                this.showLoading--;
            }, error => {
                this.toastService.handleHttpError(error);
                this.showLoading--;
            }
        )
    }

    loadBalance() {
        this.showLoading++;
        this.http.get<ResultDto<number>>("/api/payments/balance/").subscribe(
            result => {
                this.balance = result.result;
                this.showLoading--;
            }, error => {
                this.toastService.handleHttpError(error);
                this.showLoading--;
            }
        )
    }

    loadHistory() {
        this.showLoading++;
        this.msiHttp.getPage<Payment>("/api/payments/", this.historyPageNumber).subscribe(
            page => {
                this.paymentHistory = page.items.map(i => new PaymentWrapper(i));
                this.historyPageNumber = page.page;
                this.historyTotalItems = page.itemCount;
                this.showLoading--;
            }, error => {
                this.toastService.handleHttpError(error);
                this.showLoading--;
            }
        )
    }
}
