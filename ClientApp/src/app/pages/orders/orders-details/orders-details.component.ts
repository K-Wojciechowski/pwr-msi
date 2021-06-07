import {Component, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute, Router} from "@angular/router";
import {Order} from "../../../models/order/order";
import {ToastService} from "../../../services/toast.service";
import {NgbModal} from "@ng-bootstrap/ng-bootstrap";

@Component({
    selector: 'app-orders-details',
    templateUrl: './orders-details.component.html',
    styleUrls: ['./orders-details.component.scss']
})
export class OrdersDetailsComponent implements OnInit {
    showLoading: boolean = false;
    order!: Order;
    orderId: string = "";
    @ViewChild("confirmCancel") confirmCancelModal!: TemplateRef<any>;

    constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router, private toastService: ToastService, private modalService: NgbModal) {
    }

    ngOnInit(): void {
        this.orderId = this.route.snapshot.paramMap.get("id") ?? "";
        this.loadData();
    }

    private loadData() {
        this.showLoading = true;
        this.http.get<Order>(`/api/orders/${this.orderId}/`).subscribe(o => {
            this.order = o;
            this.showLoading = false;
        }, err => {
            this.toastService.handleHttpError(err);
            this.showLoading = false;
        });
    }

    private cancelOrder() {
        this.showLoading = true;
        this.http.delete<Order>(`/api/orders/${this.orderId}/`).subscribe(o => {
            this.order = o;
            this.toastService.showSuccess("Order has been cancelled.");
            this.showLoading = false;
        }, err => {
            this.toastService.handleHttpError(err);
            this.showLoading = false;
        });
    }

    handleAction(action: string) {
        if (action === "pay") {
           this.router.navigate(['pay'], {relativeTo: this.route}).then(() => undefined);
        } else if (action === "cancel") {
            this.modalService.open(this.confirmCancelModal).closed.subscribe(reason => {
                if (reason === "ok") this.cancelOrder();
            })
        } else {
            console.log(`Unknown action ${action}`);
        }
    }
}
