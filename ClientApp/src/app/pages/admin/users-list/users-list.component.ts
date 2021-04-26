import {Component, OnInit} from '@angular/core';
import {UserAdmin} from "../../../models/user-admin";
import {ToastService} from "../../../services/toast.service";
import {MsiHttpService} from "../../../services/msi-http.service";

@Component({
    selector: 'app-users-list',
    templateUrl: './users-list.component.html',
    styleUrls: ['./users-list.component.scss']
})
export class UsersListComponent implements OnInit {
    items: UserAdmin[] = [];
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
        this.msiHttp.getPage<UserAdmin>("/api/admin/users/", this.pageNumber).subscribe(res => {
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
