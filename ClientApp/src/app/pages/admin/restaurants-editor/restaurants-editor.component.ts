import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild} from '@angular/core';
import {NgForm} from "@angular/forms";
import {UsersRestaurantsEditorComponent} from "../users-restaurants-editor/users-restaurants-editor.component";
import {RestaurantUser} from "../../../models/restaurant-user";
import {RestaurantEditorOutput} from "../../../models/restaurant-editor-output";
import {Address} from "../../../models/address";
import {setFormValues} from "../../../../utils";
import {RestaurantAdmin} from "../../../models/restaurant-admin";
import {Cuisine} from "../../../models/cuisine";
import {RestaurantBasic} from "../../../models/restaurant-basic";
import {Observable, of, OperatorFunction} from "rxjs";
import {UserBasic} from "../../../models/user-basic";
import {catchError, debounceTime, distinctUntilChanged, switchMap, tap} from "rxjs/operators";
import {HttpClient} from "@angular/common/http";

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
    @Input("isAdding") isAdding!: boolean;
    @Output("restaurantSubmit") submitEvent = new EventEmitter<RestaurantEditorOutput>();
    cuisines: Cuisine[] = [];
    address!: Address | undefined;
    addressId!: number | undefined;
    imageUrl: string | null = null;
    searchFailed: boolean = false;
    newCuisine: Cuisine | null = null;

    constructor(private http: HttpClient) {
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
            this.cuisines = this.restaurantInput.cuisines;
            this.address = (this.restaurantInput.address === null || this.restaurantInput.address === undefined) ? undefined : this.restaurantInput.address;
            this.addressId = this.restaurantInput.address?.addressId;
            this.imageUrl = this.restaurantInput.logo;
        } else {
            this.form.setValue({...this.form.value, isActive: true});
        }
    }

    submit(f: NgForm) {
        const address = this.address === undefined ? undefined : {...this.address, addressee: f.value.name};
        const restaurant = {
            ...f.value,
            logo: this.imageUrl,
            cuisines: this.cuisines,
            address: address
        };
        const restaurantUsers = this.ruEditor.restaurantUsers;
        this.submitEvent.emit({restaurant, restaurantUsers});
    }

    get cuisineEndpoint() {
        return `/api/admin/cuisines/typeahead/`;
    }

    get imageEndpoint() {
        return `/api/uploads/${this.restaurantInput?.restaurantId}/logo/`;
    }

    logoChanged(logo: string | null) {
        this.imageUrl = logo;
    }

    addCuisine() {
        if (this.newCuisine === null) return;
        this.cuisines.push(this.newCuisine);
        this.newCuisine = null;
    }

    deleteCuisine(cuisine: Cuisine) {
        this.cuisines = this.cuisines.filter(c => c.cuisineId !== cuisine.cuisineId);
    }

    cuisineInputFormatter(c: Cuisine) {
        return c.name;
    }

    typeahead: OperatorFunction<string, readonly Cuisine[]> = (text$: Observable<string>) =>
        text$.pipe(
            debounceTime(200),
            distinctUntilChanged(),
            switchMap(term => {
                return (term === "") ? of([]) :
                    this.http.get<Cuisine[]>(this.cuisineEndpoint).pipe(
                        tap(() => this.searchFailed = false),
                        catchError(() => {
                            this.searchFailed = true;
                            return of([]);
                        })
                    )
            })
        )
}
