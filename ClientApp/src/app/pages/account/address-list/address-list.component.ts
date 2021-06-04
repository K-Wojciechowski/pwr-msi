import {Component, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import {MsiHttpService} from "../../../services/msi-http.service";
import {ToastService} from "../../../services/toast.service";
import {Address} from "../../../models/address";
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-address-list',
  templateUrl: './address-list.component.html',
  styleUrls: ['./address-list.component.scss']
})
export class AddressListComponent implements OnInit, OnChanges {
    items: Address[] = [];
    showLoading = false;
    pageNumber: number = 1;
    totalItems!: number;

  constructor(private msiHttp: MsiHttpService, private http: HttpClient, private toastService: ToastService) { }

    ngOnInit(): void {
        this.loadAllItems();
    }
    ngOnChanges(changes: SimpleChanges) {
        this.loadAllItems()
    }

    loadPageItems() {
        this.showLoading = true;
        this.msiHttp.getPage<Address>("/api/address/", this.pageNumber).subscribe(res => {
            this.showLoading = false;
            this.items = res.items;
            this.pageNumber = res.page;
            this.totalItems = res.itemCount;
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
       
    }
    loadAllItems() {
        this.showLoading = true;
        this.http.get<Address[]>("/api/address/all/").subscribe(res => {
            this.showLoading = false;
            this.items = res;
            this.totalItems = res.length;
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });

    }

    goToPage(pageNumber: number) {
        this.pageNumber = pageNumber;
        this.loadPageItems();
    }
    
    deleteAddress(address: Address){
        this.http.delete<Address>("/api/address/"+address.addressId).subscribe(res => {
            this.showLoading = false;
            this.toastService.showSuccess(`Address: ${res.firstLine} ${res.secondLine} deleted`)
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
        this.loadAllItems();
    }

}
