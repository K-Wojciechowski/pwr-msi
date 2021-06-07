import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {MsiHttpService} from "../../../services/msi-http.service";
import {ToastService} from "../../../services/toast.service";
import {OrderBasic} from "../../../models/order/order-basic";
import {OrderBasicWrapper} from "../../../models/order/order-basic-wrapper";

@Component({
    selector: 'app-orders-overview',
    templateUrl: './orders-overview.component.html',
    styleUrls: ['./orders-overview.component.scss']
})
export class OrdersOverviewComponent implements OnInit {
    public showLoading = 0;
    public ordersInProgress: OrderBasicWrapper[] = [];
    public orderHistory: OrderBasicWrapper[] = [];
    public historyPageNumber = 1;
    public historyTotalItems = 0;

    constructor(private http: HttpClient, private msiHttp: MsiHttpService, private toastService: ToastService) {
    }

    ngOnInit(): void {
        this.loadOrdersInProgress();
        this.loadHistory();
    }

    goToHistoryPage(pageNumber: number) {
        this.historyPageNumber = pageNumber;
        this.loadHistory();
    }

    loadOrdersInProgress() {
        this.showLoading++;
        this.http.get<OrderBasic[]>("/api/orders/active/").subscribe(
            orders => {
                this.ordersInProgress = orders.map(i => new OrderBasicWrapper(i, "/orders/"));
                this.showLoading--;
            }, error => {
                this.toastService.handleHttpError(error);
                this.showLoading--;
            }
        )
    }

    loadHistory() {
        this.showLoading++;
        this.msiHttp.getPage<OrderBasic>("/api/orders/", this.historyPageNumber).subscribe(
            page => {
                this.orderHistory = page.items.map(i => new OrderBasicWrapper(i, "/orders/"));
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
