import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild} from '@angular/core';
import {NgForm} from "@angular/forms";
import {UsersRestaurantsEditorComponent} from "../users-restaurants-editor/users-restaurants-editor.component";
import {RestaurantUser} from "../../../models/restaurant-user";
import {RestaurantEditorOutput} from "../../../models/restaurant-editor-output";
import {Address} from "../../../models/address";
import {setFormValues} from "../../../../utils";
import {RestaurantAdmin} from "../../../models/restaurant-admin";

@Component({
    selector: 'app-restaurants-editor',
    templateUrl: './restaurants-editor.component.html',
    styleUrls: ['./restaurants-editor.component.scss']
})
export class RestaurantsEditorComponent implements OnInit, OnChanges {
    @ViewChild("f", {static: true}) form!: NgForm;
    @ViewChild("ruEditor", {static: true}) ruEditor!: UsersRestaurantsEditorComponent;
    @Input("restaurant") restaurantInput!: RestaurantAdmin | undefined;
    @Input("restarantUsers") restaurantUsersInput!: RestaurantUser[] | undefined;
    @Output("restaurantSubmit") submitEvent = new EventEmitter<RestaurantEditorOutput>();
    address!: Address | undefined;
    addressId!: number | undefined;

    constructor() {
    }

    ngOnInit(): void {
        setTimeout(() => {
            this.loadRestaurantInput();
        }, 0);
    }

    ngOnChanges(changes: SimpleChanges) {
        if (!!changes.restaurantInput && changes.restaurantInput.previousValue !== changes.restaurantInput.currentValue) {
            this.loadRestaurantInput();
        }
    }

    loadRestaurantInput() {
        if (this.restaurantInput !== undefined) {
            setFormValues(this.restaurantInput, this.form);
            this.address = (this.restaurantInput.address === null || this.restaurantInput.address === undefined) ? undefined : this.restaurantInput.address;
            this.addressId = this.restaurantInput.address?.addressId;
        } else {
            this.form.setValue({...this.form.value, isActive: true});
        }
    }

    submit(f: NgForm) {
        const restaurant = {
            ...f.value,
            address: this.address
        };
        const restaurantUsers = this.ruEditor.restaurantUsers;
        this.submitEvent.emit({restaurant, restaurantUsers});
    }
}
