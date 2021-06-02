import { Component, OnInit } from '@angular/core';
import {RestaurantBasic} from "../../../models/restaurant-basic";
import {MsiHttpService} from "../../../services/msi-http.service";
import {RestaurantAdmin} from "../../../models/restaurant-admin";
import {ToastService} from "../../../services/toast.service";

@Component({
  selector: 'app-browse-restaurants',
  templateUrl: './browse-restaurants.component.html',
  styleUrls: ['./browse-restaurants.component.scss']
})
export class BrowseRestaurantsComponent implements OnInit {
    items: RestaurantBasic[] = [];
    showLoading = true;
    pageNumber: number = 1;
    totalItems!: number;

  constructor(private msiHttp: MsiHttpService, private toastService: ToastService) { }

  ngOnInit(): void {
      this.loadItems();
  }
    loadItems() {
        this.showLoading = true;
        this.msiHttp.getPage<RestaurantBasic>("/api/offer/restaurants/", this.pageNumber).subscribe(res => {
            this.showLoading = false;
            this.items = res.items;
            this.pageNumber = res.page;
            this.totalItems = res.itemCount;
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
    }

    goToPage(pageNumber: number) {
        this.pageNumber = pageNumber;
        this.loadItems();
    }

}
