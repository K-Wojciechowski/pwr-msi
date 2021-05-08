import {Component, OnInit} from '@angular/core';
import {RestaurantAdmin} from "../../../models/restaurant-admin";
import {HttpClient} from "@angular/common/http";
import {RestaurantContextHelperService} from "../../../services/restaurant-context-helper.service";
import {ToastService} from "../../../services/toast.service";
import {ActivatedRoute} from "@angular/router";

@Component({
    selector: 'app-manage-index',
    templateUrl: './manage-index.component.html',
    styleUrls: ['./manage-index.component.scss']
})
export class ManageIndexComponent implements OnInit {
    public restaurant: RestaurantAdmin | null = null;
    public showLoading: boolean = true;

    constructor(private http: HttpClient, private contextHelper: RestaurantContextHelperService, private toastService: ToastService, private route: ActivatedRoute) {
    }

    async ngOnInit() {
        this.loadData();
    }

    loadData() {
        this.route.params.pipe(this.contextHelper.getReq()).subscribe(restaurantId => {
            console.log("Load " + restaurantId);
            this.showLoading = true;
            this.http.get<RestaurantAdmin>(`/api/restaurants/${restaurantId}/`).subscribe(ra => {
                this.restaurant = ra;
                this.showLoading = false;
            }, err => {
                this.toastService.handleHttpError(err);
                this.showLoading = false;
            });
        });
    }

}
