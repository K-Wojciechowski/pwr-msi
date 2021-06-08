import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {Router} from "@angular/router";
import {Address} from "../../../models/address";

@Component({
    selector: 'app-address-add',
    templateUrl: './address-add.component.html',
    styleUrls: ['./address-add.component.scss']
})
export class AddressAddComponent implements OnInit {
    showLoading: boolean = false;
    newAddress: Address | undefined;

    constructor(private http: HttpClient, private toastService: ToastService, private router: Router) {
    }

    ngOnInit(): void {
    }

    saveAddress() {
        this.showLoading = true;
        this.http.post<Address>("/api/addresses/", this.newAddress).subscribe(async newAddress => {
            this.toastService.showSuccess(`Address ${newAddress.firstLine} ${newAddress.secondLine} ${newAddress.city} created.`);
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
        this.router.navigateByUrl("/account/address");
    }

}
