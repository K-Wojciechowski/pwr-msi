import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild} from '@angular/core';
import {Address} from "../../models/address";
import {NgForm} from "@angular/forms";
import {setFormValues} from "../../../utils";

@Component({
    selector: 'app-address-editor',
    templateUrl: './address-editor.component.html',
    styleUrls: ['./address-editor.component.scss']
})
export class AddressEditorComponent implements OnInit, OnChanges {
    @Input("required") required: boolean = false;
    @Input("showAddressee") showAddressee: boolean = true;
    @Input("addressId") addressId: number | undefined = undefined;
    @Input("address") addressInput!: Address | undefined;
    @Output("addressChange") addressChange = new EventEmitter<Address>();
    @ViewChild("f", {static: true}) form!: NgForm;
    address: Address = this.newAddress();

    constructor() { }

    ngOnInit(): void {
        if (this.addressInput !== undefined) {
            this.address = this.addressInput;
        }
        this.form.valueChanges?.subscribe(v => {
            this.address = this.getAddress(v);
            this.addressChange.emit(this.address);
        });
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes.addressInput.previousValue !== changes.addressInput.currentValue && this.addressInput !== undefined) {
            setTimeout(() => setFormValues(changes.addressInput.currentValue, this.form), 0);
        }
    }

    getAddress(formValue: any): Address {
        const rawAddress = {
            addressId: this.addressId,
            ...formValue,
            country: "PL"
        };
        return {...rawAddress, latitude: parseFloat(rawAddress.latitude), longitude: parseFloat(rawAddress.longitude)};
    }

    newAddress(): Address {
        return {
            addressId: this.addressId,
            addressee: "",
            firstLine: "",
            secondLine: "",
            postCode: "",
            city: "",
            country: "PL",
            latitude: 0,
            longitude: 0
        }
    }

    setFromLocation() {
        navigator.geolocation.getCurrentPosition((pos) => {
            this.form.setValue({...this.form.value, latitude: pos.coords.latitude, longitude: pos.coords.longitude});
        });
    }
}
