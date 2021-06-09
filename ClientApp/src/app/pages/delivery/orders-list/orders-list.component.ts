import {Component, OnInit} from '@angular/core';
import {OrderBasicWrapper} from "../../../models/order/order-basic-wrapper";
import {HttpClient} from "@angular/common/http";
import {MsiHttpService} from "../../../services/msi-http.service";
import {ToastService} from "../../../services/toast.service";
import {OrderBasic} from "../../../models/order/order-basic";
import {ActivatedRoute} from "@angular/router";
import {OrderStatus} from "../../../models/enum/order-status";


@Component({
    selector: 'app-orders-list',
    templateUrl: './orders-list.component.html',
    styleUrls: ['./orders-list.component.scss']
})
export class OrdersListComponent implements OnInit {
    lat: number | undefined;
    lng: number | undefined;
    restIdString: string | null = "";
    mockData: OrderBasic [] = [];
    public showLoading = 0;
    public ordersInProgress: OrderBasicWrapper[] = [];
    public orderHistory: OrderBasicWrapper[] = [];
    public ordersAwaiting: OrderBasicWrapper[] = [];

    onlyNearbyOrders: boolean = false;

    constructor(private http: HttpClient, private msiHttp: MsiHttpService, private toastService: ToastService, private route: ActivatedRoute) {
    }

    ngOnInit(): void {
        for (var i = 0; i < 3; i++) {

        }
        this.restIdString = this.route.snapshot.paramMap.get("restaurantId");
        if (this.restIdString == "all" || this.restIdString == null) {
            this.restIdString = "-1";
        }
        this.loadActiveOrders();
        this.loadWaitingOrders();
        this.loadHistoryOrders();

    }


    loadActiveOrders() {
        //this.showLoading++;
        this.http.get<OrderBasic[]>("/api/delivery/active/?restaurantId=" + this.restIdString).subscribe(
            orders => {
                this.ordersInProgress = orders.map(i => new OrderBasicWrapper(i, "/deliver/order/"));
                //this.ordersInProgress = this.mockData.map(i => new OrderBasicWrapper(i, "/delivery/active/?restaurantId="+this.restIdString));
                this.showLoading--;
            }, error => {
                this.toastService.handleHttpError(error);
                this.showLoading--;
            }
        )
    }

    loadWaitingOrders() {
        console.log(this.onlyNearbyOrders);
        //this.showLoading++;
        if (this.lat != undefined && this.lng != undefined && this.onlyNearbyOrders) {
            this.http.get<OrderBasic[]>("/api/delivery/waiting/?restaurantId=" + this.restIdString + "&lng=" + this.lng.toString() + "&lat=" + this.lat.toString() + "&range=10000").subscribe(
                orders => {
                    this.ordersAwaiting = orders.map(i => new OrderBasicWrapper(i, "/deliver/order/"));
                    //this.ordersAwaiting = this.mockData.map(i => new OrderBasicWrapper(i,
                    // "/delivery/waiting/?restaurantId="+this.restIdString+"&lng="+this.lng?.toString()+"&lat="+this.lat?.toString()+"&range=10000"));
                    this.showLoading--;
                }, error => {
                    this.toastService.handleHttpError(error);
                    this.showLoading--;
                }
            )
        } else {
            this.http.get<OrderBasic[]>("/api/delivery/waiting/?restaurantId=" + this.restIdString).subscribe(
                orders => {
                    this.ordersAwaiting = orders.map(i => new OrderBasicWrapper(i, "/deliver/order/"));
                    // this.ordersAwaiting = this.mockData.map(i => new OrderBasicWrapper(i, "/delivery/waiting/?restaurantId="+this.restIdString));
                    this.showLoading--;
                }, error => {
                    this.toastService.handleHttpError(error);
                    this.showLoading--;
                }
            )
        }
    }

    getCoords() {
        this.ordersAwaiting = [];
        if (this.onlyNearbyOrders) {
            navigator.geolocation.getCurrentPosition(position => {
                console.log(position);
                this.lat = position.coords.latitude;
                this.lng = position.coords.longitude;
                this.loadWaitingOrders()
            });
        } else {
            this.loadWaitingOrders()
        }
    }

    loadHistoryOrders() {
        // this.showLoading++;
        this.http.get<OrderBasic[]>("/api/delivery/history/?restaurantId=" + this.restIdString).subscribe(
            orders => {
                this.orderHistory = orders.map(i => new OrderBasicWrapper(i, "/deliver/order/"));
                //this.orderHistory = this.mockData.map(i => new OrderBasicWrapper(i, "/delivery/history/?restaurantId="+this.restIdString));
                this.showLoading--;
            }, error => {
                this.toastService.handleHttpError(error);
                this.showLoading--;
            }
        )
    }

}
