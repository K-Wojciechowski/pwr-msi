import { Component, OnInit } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {ActivatedRoute, Router} from "@angular/router";
import {Address} from "../../../models/address";
import {RestaurantFull} from "../../../models/restaurant-full";
import {RestaurantUser} from "../../../models/restaurant-user";

@Component({
  selector: 'app-address-edit',
  templateUrl: './address-edit.component.html',
  styleUrls: ['./address-edit.component.scss']
})
export class AddressEditComponent implements OnInit {
    showLoading: boolean = false;
    address: Address | undefined;
    addressId: number | undefined;
    constructor(private http: HttpClient, private toastService: ToastService, private router: Router, private route: ActivatedRoute) { }

    ngOnInit(): void {
       this.loadAddress();
    }
    
    
    loadAddress() {
        this.showLoading = true;
        const addressIdString = this.route.snapshot.paramMap.get("id");
        if (addressIdString === null) return;
        this.addressId = parseInt(addressIdString);
        this.http.get<Address>("/api/address/"+addressIdString+"/").subscribe(address => {
            this.address = address;
            this.showLoading = false;
        }, error => {
            this.showLoading = false ;
            this.toastService.handleHttpError(error);
        });
    }
    editAddress(address: Address) {
        this.showLoading = true;
        this.http.put<Address>("/api/address/"+this.addressId+"/", address).subscribe(async newAddress => {
            this.toastService.showSuccess(`Address ${newAddress.firstLine} ${newAddress.secondLine} ${newAddress.city} changed.`);
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
        this.router.navigateByUrl("/account/address");
    }
}
