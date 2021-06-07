import {Component, OnInit} from '@angular/core';
import {MsiHttpService} from "../../../services/msi-http.service";
import {ToastService} from "../../../services/toast.service";
import {Address} from "../../../models/address";
import {HttpClient} from "@angular/common/http";

@Component({
    selector: 'app-address-list',
    templateUrl: './address-list.component.html',
    styleUrls: ['./address-list.component.scss']
})
export class AddressListComponent implements OnInit {
    items: Address[] = [];
    showLoading = 0;
    pageNumber: number = 1;
    totalItems!: number;
    billingAddressId: number = -1;

    constructor(private msiHttp: MsiHttpService, private http: HttpClient, private toastService: ToastService) {
    }

    ngOnInit(): void {
        this.loadBillingAddressId();
        this.loadPageItems();
    }

    loadBillingAddressId() {
        this.showLoading++;
        this.http.get<Address>("/api/addresses/default/").subscribe(address => {
            this.showLoading--;
            this.billingAddressId = address.addressId!;
        }, error => {
            this.showLoading--;
            if (error.status == 404)  {
                this.billingAddressId = -1;
            } else {
                this.toastService.handleHttpError(error);
            }
        });
    }

    loadPageItems() {
        this.showLoading++;
        this.msiHttp.getPage<Address>("/api/addresses/", this.pageNumber).subscribe(res => {
            this.showLoading--;
            this.items = res.items;
            this.pageNumber = res.page;
            this.totalItems = res.itemCount;
        }, error => {
            this.showLoading--;
            this.toastService.handleHttpError(error);
        });

    }

    goToPage(pageNumber: number) {
        this.pageNumber = pageNumber;
        this.loadPageItems();
    }

    deleteAddress(address: Address) {
        this.showLoading++;
        this.http.delete<Address>(`/api/addresses/${address.addressId}/`).subscribe(res => {
            this.showLoading--;
            this.toastService.showSuccess(`Address: ${res.firstLine} ${res.secondLine} deleted.`);
            this.loadPageItems();
        }, error => {
            this.showLoading--;
            this.toastService.handleHttpError(error);
        });
    }

    makeBilling(address: Address) {
        this.showLoading++;
        this.http.post<Address>("/api/addresses/default/", {input: address.addressId!}).subscribe(res => {
            this.showLoading--;
            this.toastService.showSuccess(`Address: ${res.firstLine} ${res.secondLine} made the billing address.`);
            this.billingAddressId = res.addressId!;
            this.loadPageItems();
        }, error => {
            this.showLoading--;
            this.toastService.handleHttpError(error);
        });
    }


}
