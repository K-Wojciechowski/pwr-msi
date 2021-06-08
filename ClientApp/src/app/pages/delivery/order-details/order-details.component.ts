import { Component, OnInit } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {MsiHttpService} from "../../../services/msi-http.service";
import {ToastService} from "../../../services/toast.service";
import {ActivatedRoute} from "@angular/router";
import {Order} from "../../../models/order/order";
import {OrderUserRole} from "../../../models/enum/OrderUserRole";

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss']
})
export class OrderDetailsComponent implements OnInit {
    showLoading: boolean = false;
    order!: Order;
    orderId: string = "";
    public role: OrderUserRole = OrderUserRole.DELIVERY;
    constructor(private http: HttpClient, private msiHttp: MsiHttpService, private toastService: ToastService, private route: ActivatedRoute) { }

    ngOnInit(): void {
      this.orderId = this.route.snapshot.paramMap.get("id") ?? "";
      this.loadData();
    }
    
    private loadData() {
        this.showLoading = true;
        this.http.get<Order>(`/api/delivery/order/${this.orderId}/`).subscribe(o => {
            this.order = o;
            this.showLoading = false;
        }, err => {
            this.toastService.handleHttpError(err);
            this.showLoading = false;
        });
    }
    private assign(){
        this.showLoading = true;
        this.http.post<Order>(`/api/delivery/order/${this.orderId}/assign`, this.order).subscribe(o => {
            this.order = o;
            this.showLoading = false;
        }, err => {
            this.toastService.handleHttpError(err);
            this.showLoading = false;
        });
        
    }
    private markAsDelivered(){
        this.showLoading = true;
        this.http.post<Order>(`/api/delivery/order/${this.orderId}/complete`, this.order).subscribe(o => {
            this.order = o;
            this.showLoading = false;
        }, err => {
            this.toastService.handleHttpError(err);
            this.showLoading = false;
        });
    }

    handleAction(action: string) {
        switch (action) {
            case "deliver":
                this.markAsDelivered();
                break;
            case "selfassign":
                this.assign()
                break;
            default:
                console.log(`Unknown action ${action}`);
        }
    }
    


}
