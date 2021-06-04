import {Component, OnInit} from '@angular/core';
import {UserAdmin} from "../../../models/user-admin";
import {MsiHttpService} from "../../../services/msi-http.service";
import {ToastService} from "../../../services/toast.service";
import {RestaurantFull} from "../../../models/restaurant-full";

@Component({
    selector: 'app-restaurants-list',
    templateUrl: './restaurants-list.component.html',
    styleUrls: ['./restaurants-list.component.scss']
})
export class RestaurantsListComponent implements OnInit {
    items: RestaurantFull[] = [];
    showLoading = true;
    pageNumber: number = 1;
    totalItems!: number;

    constructor(private msiHttp: MsiHttpService, private toastService: ToastService) {
    }

    ngOnInit(): void {
        this.loadItems();
    }

    loadItems() {
        this.showLoading = true;
        this.msiHttp.getPage<RestaurantFull>("/api/admin/restaurants/", this.pageNumber).subscribe(res => {
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
