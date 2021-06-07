import { Component, OnInit } from '@angular/core';
import {RestaurantEditorOutput} from "../../../models/restaurant-editor-output";
import {RestaurantFull} from "../../../models/restaurant-full";
import {RestaurantUser} from "../../../models/restaurant-user";
import {UsersRestaurantsEditorComponent} from "../../admin/users-restaurants-editor/users-restaurants-editor.component";
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

  constructor(private http: HttpClient, private toastService: ToastService, private router: Router) { }

  ngOnInit(): void {
  }
    addAddress(address: Address) {
        this.showLoading = true;
        this.http.post<Address>("/api/address/", address).subscribe(async newAddress => {
            this.toastService.showSuccess(`Address ${newAddress.firstLine} ${newAddress.secondLine} ${newAddress.city} created.`);
        }, error => {
            this.showLoading = false;
            this.toastService.handleHttpError(error);
        });
        this.router.navigateByUrl("/account/address");
    }

}
