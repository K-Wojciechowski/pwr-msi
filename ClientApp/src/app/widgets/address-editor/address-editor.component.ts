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
    @Input("addressId") addressId: number | undefined = undefined;
    @Input("address") addressInput!: Address | undefined;
    @Output("addressChange") addressChange = new EventEmitter<Address>();
    @ViewChild("f", {static: true}) form!: NgForm;
    address!: Address;

    constructor() { }

    ngOnInit(): void {
        this.address = this.addressInput === undefined ? this.newAddress() : this.addressInput;
        this.form.valueChanges?.subscribe(v => {
            this.address = this.getAddress(v);
            this.addressChange.emit(this.address);
        })
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes.addressInput.previousValue !== changes.addressInput.currentValue && this.addressInput !== undefined) {
            this.address = this.addressInput;
            setFormValues(this.address, this.form);
        }
    }

    getAddress(formValue: any): Address {
        return {
            addressId: this.addressId,
            ...formValue,
            country: "PL"
        };
    }

    newAddress(): Address {
        return {
            addressId: this.addressId,
            addressee: "",
            firstLine: "",
            secondLine: "",
            postCode: "",
            city: "",
            country: "PL"
        }
    }
}
