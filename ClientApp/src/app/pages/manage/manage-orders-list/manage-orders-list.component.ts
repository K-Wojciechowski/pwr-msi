import {Component, OnInit} from '@angular/core';
import {OrderBasicWrapper} from "../../../models/order/order-basic-wrapper";
import {HttpClient} from "@angular/common/http";
import {MsiHttpService} from "../../../services/msi-http.service";
import {ToastService} from "../../../services/toast.service";
import {OrderBasic} from "../../../models/order/order-basic";
import {ActivatedRoute} from "@angular/router";
import {RestaurantContextHelperService} from "../../../services/restaurant-context-helper.service";

@Component({
    selector: 'app-manage-orders-list',
    templateUrl: './manage-orders-list.component.html',
    styleUrls: ['./manage-orders-list.component.scss']
})
export class ManageOrdersListComponent implements OnInit {
    public showLoading = 0;
    public ordersAwaiting: OrderBasicWrapper[] = [];
    public ordersInPreparation: OrderBasicWrapper[] = [];
    public ordersInDelivery: OrderBasicWrapper[] = [];
    public orderHistory: OrderBasicWrapper[] = [];
    public historyPageNumber = 1;
    public historyTotalItems = 0;
    public restaurantId!: number;

    constructor(private http: HttpClient, private msiHttp: MsiHttpService, private toastService: ToastService, private route: ActivatedRoute, private contextHelper: RestaurantContextHelperService) {
    }

    ngOnInit(): void {
        this.route.params.pipe(this.contextHelper.getReq()).subscribe(id => this.restaurantId = id);
        this.loadOrdersAwaiting();
        this.loadOrdersInPreparation();
        this.loadOrdersInDelivery();
        this.loadHistory();
    }

    goToHistoryPage(pageNumber: number) {
        this.historyPageNumber = pageNumber;
        this.loadHistory();
    }

    get endpoint() {
        return `/api/restaurants/${this.restaurantId}/orders/`;
    }

    get linkBase() {
        return `/manage/${this.restaurantId}/orders/`;
    }

    loadOrders(endpoint: string, destination: string) {
        this.showLoading++;
        this.http.get<OrderBasic[]>(endpoint).subscribe(
            orders => {
                // @ts-ignore
                this[destination] = orders.map(i => new OrderBasicWrapper(i, this.linkBase));
                this.showLoading--;
            }, error => {
                this.toastService.handleHttpError(error);
                this.showLoading--;
            }
        );
    }

    loadOrdersAwaiting() {
        this.loadOrders(this.endpoint + "awaiting/", "ordersAwaiting");
    }

    loadOrdersInPreparation() {
        this.loadOrders(this.endpoint + "preparing/", "ordersInPreparation");
    }

    loadOrdersInDelivery() {
        this.loadOrders(this.endpoint + "delivery/", "ordersInDelivery");
    }

    loadHistory() {
        this.showLoading++;
        this.msiHttp.getPage<OrderBasic>(this.endpoint, this.historyPageNumber).subscribe(
            page => {
                this.orderHistory = page.items.map(i => new OrderBasicWrapper(i, this.linkBase));
                this.historyPageNumber = page.page;
                this.historyTotalItems = page.itemCount;
                this.showLoading--;
            }, error => {
                this.toastService.handleHttpError(error);
                this.showLoading--;
            }
        );
    }
}
