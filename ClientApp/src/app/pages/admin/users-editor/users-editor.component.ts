import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild} from '@angular/core';
import {NgForm, ValidationErrors} from "@angular/forms";
import {UserAdmin, UserCreateAdmin} from "../../../models/user-admin";
import {Address} from "../../../models/address";
import {PasswordResetInput} from "../../../models/password-reset-input";
import {setFormValues} from "../../../../utils";

@Component({
    selector: 'app-users-editor',
    templateUrl: './users-editor.component.html',
    styleUrls: ['./users-editor.component.scss']
})
export class UsersEditorComponent implements OnInit, OnChanges {
    @ViewChild("f", {static: true}) form!: NgForm;
    @Input("isAdding") isAdding!: boolean;
    @Input("user") userInput!: UserAdmin | UserCreateAdmin | undefined;
    @Output("userSubmit") submitEvent = new EventEmitter<UserAdmin>();
    @Output("reset") resetEvent = new EventEmitter<PasswordResetInput>();
    billingAddress!: Address | undefined;
    billingAddressId!: number | undefined;
    hasBillingAddress: boolean = false;

    constructor() {
    }

    ngOnInit(): void {
        setTimeout(() => this.loadUserInput(), 0);
    }

    ngOnChanges(changes: SimpleChanges) {
        if (!!changes.userInput && changes.userInput.previousValue !== changes.userInput.currentValue) {
            this.loadUserInput();
        }
    }

    loadUserInput() {
        if (this.userInput !== undefined) {
            setFormValues(this.userInput, this.form);
            this.hasBillingAddress = this.userInput.billingAddress !== null && this.userInput.billingAddress !== undefined;
            this.billingAddress = (this.userInput.billingAddress === null || this.userInput.billingAddress === undefined) ? undefined : this.userInput.billingAddress;
            this.billingAddressId = this.userInput.billingAddress?.addressId;
        } else {
            this.form.setValue({...this.form.value, isActive: true});
        }
    }

    submit(f: NgForm) {
        const data = {
            ...f.value,
            isActive: !!f.value.isActive,
            isVerified: !!f.value.isVerified,
            isAdmin: !!f.value.isAdmin,
            billingAddress: this.hasBillingAddress ? this.billingAddress : null
        };
        this.submitEvent.emit(data);
    }

    reset(f: NgForm) {
        this.resetEvent.emit(f.value);
    }
}
