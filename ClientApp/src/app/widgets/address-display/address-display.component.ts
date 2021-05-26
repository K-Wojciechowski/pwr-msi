import {Component, Input, OnInit} from '@angular/core';
import {Address} from "../../models/address";

@Component({
    selector: 'app-address-display',
    templateUrl: './address-display.component.html',
    styleUrls: ['./address-display.component.scss']
})
export class AddressDisplayComponent {
    @Input("address") address!: Address;
    @Input("showAddressee") showAddressee: boolean = true;

}
