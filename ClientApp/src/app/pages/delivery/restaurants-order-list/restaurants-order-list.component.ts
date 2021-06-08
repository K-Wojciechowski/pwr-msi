import { Component, OnInit } from '@angular/core';
import {RestaurantBasic} from "../../../models/restaurant-basic";
import {MsiHttpService} from "../../../services/msi-http.service";
import {ToastService} from "../../../services/toast.service";
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-restaurants-order-list',
  templateUrl: './restaurants-order-list.component.html',
  styleUrls: ['./restaurants-order-list.component.scss']
})
export class RestaurantsOrderListComponent implements OnInit {
    items: RestaurantBasic[] = [];
    showLoading = true;
    totalItems!: number;
    pos: any;

    constructor(private http: HttpClient, private toastService: ToastService) { }

    ngOnInit(): void {
        this.getPosition();
        this.loadItems();
    }
    loadItems() {
        this.showLoading = true;
        //this.msiHttp.getPage<RestaurantBasic>("/api/offer/restaurants/all?lng="+this.pos.coords.longitude+"&lng="+this.pos.coords.latitude, this.pageNumber)
        this.http.get<RestaurantBasic[]>("/api/delivery/restaurants/")
            .subscribe(res => {
                this.showLoading = false;
                this.items = res;
                this.totalItems = res.length;
            }, error => {
                this.showLoading = false;
                this.toastService.handleHttpError(error);
            });

    }
    getPosition = () => {
        navigator.geolocation.getCurrentPosition((pos) => {
            this.pos = pos;
        });
    }
}
