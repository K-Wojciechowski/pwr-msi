import {Component, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {Order} from "../../../models/order/order";
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute, Router} from "@angular/router";
import {ToastService} from "../../../services/toast.service";
import {NgbModal} from "@ng-bootstrap/ng-bootstrap";
import {RestaurantContextHelperService} from "../../../services/restaurant-context-helper.service";
import {OrderUserRole} from "../../../models/enum/OrderUserRole";
import {Deliverer} from "../../../models/deliverer";

@Component({
    selector: 'app-manage-orders-details',
    templateUrl: './manage-orders-details.component.html',
    styleUrls: ['./manage-orders-details.component.scss']
})
export class ManageOrdersDetailsComponent implements OnInit {
    public showLoading: boolean = false;
    public showDeliverersLoading: boolean = false;
    public order!: Order;
    public orderId: string = "";
    public role: OrderUserRole = OrderUserRole.MANAGER;
    public restaurantId!: number;
    public assignType: string = "unassigned";
    public assigneeId: number = -1;
    public deliverers: Deliverer[] = [];
    @ViewChild("confirmReject") confirmRejectModal!: TemplateRef<any>;
    @ViewChild("assignDelivery") assignDeliveryModal!: TemplateRef<any>;

    constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router, private toastService: ToastService, private modalService: NgbModal, private contextHelper: RestaurantContextHelperService) {
    }

    ngOnInit(): void {
        this.route.params.pipe(this.contextHelper.getReq()).subscribe(id => this.restaurantId = id);
        this.orderId = this.route.snapshot.paramMap.get("id") ?? "";
        this.loadData();
    }

    private loadData() {
        if (this.restaurantId === undefined) setTimeout(() => this.loadData(), 100);
        this.showLoading = true;
        this.http.get<Order>(`/api/restaurants/${this.restaurantId}/orders/${this.orderId}/`).subscribe(o => {
            this.order = o;
            this.showLoading = false;
        }, err => {
            this.toastService.handleHttpError(err);
            this.showLoading = false;
        });
    }

    private rejectOrder() {
        this.showLoading = true;
        this.http.delete<Order>(`/api/restaurants/${this.restaurantId}/orders/${this.orderId}/`).subscribe(o => {
            this.order = o;
            this.toastService.showSuccess("Order has been rejected.");
            this.showLoading = false;
        }, err => {
            this.toastService.handleHttpError(err);
            this.showLoading = false;
        });
    }

    private acceptOrder() {
        this.showLoading = true;
        this.http.post<Order>(`/api/restaurants/${this.restaurantId}/orders/${this.orderId}/accept/`, null).subscribe(o => {
            this.order = o;
            this.toastService.showSuccess("Order has been accepted.");
            this.showLoading = false;
        }, err => {
            this.toastService.handleHttpError(err);
            this.showLoading = false;
        });
    }

    private prepareOrder() {
        this.showLoading = true;
        this.http.post<Order>(`/api/restaurants/${this.restaurantId}/orders/${this.orderId}/prepare/`, null).subscribe(o => {
            this.order = o;
            this.toastService.showSuccess("Order has been marked as prepared.");
            this.showLoading = false;
        }, err => {
            this.toastService.handleHttpError(err);
            this.showLoading = false;
        });
    }


    private assignDelivery() {
        this.showLoading = true;
        const params = {assign: this.assignType, id: this.assigneeId.toString()};
        this.http.post<Order>(`/api/restaurants/${this.restaurantId}/orders/${this.orderId}/delivery/`, null, {params}).subscribe(o => {
            this.order = o;
            this.toastService.showSuccess("Assignment has been updated.");
            this.showLoading = false;
        }, err => {
            this.toastService.handleHttpError(err);
            this.showLoading = false;
        });
    }


    private loadAssigneeList() {
        this.showDeliverersLoading = true;
        this.http.post<Deliverer[]>(`/api/restaurants/${this.restaurantId}/deliverers/`, null).subscribe(d => {
            this.deliverers = d;
            this.showDeliverersLoading = false;
        }, err => {
            this.toastService.handleHttpError(err);
            this.showDeliverersLoading = false;
        });
    }

    handleAction(action: string) {
        switch (action) {
            case "pay":
                this.router.navigate(['pay'], {relativeTo: this.route}).then(() => undefined);
                break;
            case "accept":
                this.acceptOrder();
                break;
            case "reject":
                this.modalService.open(this.confirmRejectModal).closed.subscribe(reason => {
                    if (reason === "ok") this.rejectOrder();
                });
                break;
            case "prepare":
                this.prepareOrder();
                break;
            case "assign":
                this.loadAssigneeList();
                this.modalService.open(this.assignDeliveryModal).closed.subscribe(reason => {
                    if (reason === "ok") this.assignDelivery();
                });
                break;

            default:
                console.log(`Unknown action ${action}`);
        }
    }
}
