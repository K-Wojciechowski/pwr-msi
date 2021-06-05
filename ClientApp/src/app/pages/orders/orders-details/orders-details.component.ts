import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from "@angular/router";
import {Order} from "../../../models/order/order";
import {ToastService} from "../../../services/toast.service";

@Component({
    selector: 'app-orders-details',
    templateUrl: './orders-details.component.html',
    styleUrls: ['./orders-details.component.scss']
})
export class OrdersDetailsComponent implements OnInit {
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
