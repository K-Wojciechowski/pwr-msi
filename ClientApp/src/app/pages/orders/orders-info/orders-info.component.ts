import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from "@angular/router";
import {Order} from "../../../models/order/order";
import {ToastService} from "../../../services/toast.service";

@Component({
    selector: 'app-orders-info',
    templateUrl: './orders-info.component.html',
    styleUrls: ['./orders-info.component.scss']
})
export class OrdersInfoComponent implements OnInit {
    showLoading: boolean = false;
    order!: Order;

    constructor(private http: HttpClient, private route: ActivatedRoute, private toastService: ToastService) {
    }

    ngOnInit(): void {
        const orderId = this.route.snapshot.paramMap.get("id");
        this.showLoading = true;
        this.http.get<Order>(`/api/orders/${orderId}/`).subscribe(o => {
            this.order = o;
            this.showLoading = false;
        }, err => {
            this.toastService.handleHttpError(err);
            this.showLoading = false;
        });
    }

}
