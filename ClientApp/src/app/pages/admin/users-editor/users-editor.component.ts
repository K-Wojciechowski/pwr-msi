import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild} from '@angular/core';
import {NgForm} from "@angular/forms";
import {UserAdmin, UserCreateAdmin} from "../../../models/user-admin";
import {Address} from "../../../models/address";
import {PasswordResetInput} from "../../../models/password-reset-input";
import {setFormValues} from "../../../../utils";
import {UserEditorOutput} from "../../../models/user-editor-output";
import {RestaurantUser} from "../../../models/restaurant-user";
import {UsersRestaurantsEditorComponent} from "../users-restaurants-editor/users-restaurants-editor.component";

@Component({
    selector: 'app-users-editor',
    templateUrl: './users-editor.component.html',
    styleUrls: ['./users-editor.component.scss']
})
export class UsersEditorComponent implements OnInit, OnChanges {
    @ViewChild("f", {static: true}) form!: NgForm;
    @ViewChild("ruEditor", {static: true}) ruEditor!: UsersRestaurantsEditorComponent;
    @Input("isAdding") isAdding!: boolean;
    @Input("user") userInput!: UserAdmin | UserCreateAdmin | undefined;
    @Input("restarantUsers") restaurantUsersInput!: RestaurantUser[] | undefined;
    @Output("userSubmit") submitEvent = new EventEmitter<UserEditorOutput>();
    @Output("reset") resetEvent = new EventEmitter<PasswordResetInput>();
    billingAddress!: Address | undefined;
    newBillingAddress!: Address | undefined;
    billingAddressId!: number | undefined;
    hasBillingAddress: boolean = false;

    constructor() {
    }

    ngOnInit(): void {
        setTimeout(() => {
            this.loadUserInput();
        }, 0);
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
            this.newBillingAddress = this.billingAddress;
        } else {
            this.form.setValue({...this.form.value, isActive: true});
        }
    }

    submit(f: NgForm) {
        const user = {
            ...f.value,
            isActive: !!f.value.isActive,
            isVerified: !!f.value.isVerified,
            isAdmin: !!f.value.isAdmin,
            billingAddress: this.hasBillingAddress ? this.newBillingAddress : null
        };
        const restaurantUsers = this.ruEditor.restaurantUsers;
        this.submitEvent.emit({user, restaurantUsers});
    }

    reset(f: NgForm) {
        this.resetEvent.emit(f.value);
    }
}
