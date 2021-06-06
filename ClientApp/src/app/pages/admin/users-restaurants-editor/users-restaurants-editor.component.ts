import {Component, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import {RestaurantUser} from "../../../models/restaurant-user";
import {UserBasic} from "../../../models/user-basic";
import {RestaurantBasic} from "../../../models/restaurant-basic";
import {Observable, of, OperatorFunction} from "rxjs";
import {catchError, debounceTime, distinctUntilChanged, switchMap, tap} from "rxjs/operators";
import {HttpClient} from "@angular/common/http";
import {UserAdmin} from "../../../models/user-admin";
import {RestaurantFull} from "../../../models/restaurant-full";

@Component({
    selector: 'app-users-restaurants-editor',
    templateUrl: './users-restaurants-editor.component.html',
    styleUrls: ['./users-restaurants-editor.component.scss']
})
export class UsersRestaurantsEditorComponent implements OnInit, OnChanges {
    @Input("given") given!: string;
    @Input("user") user!: UserBasic | undefined;
    @Input("restaurant") restaurant!: RestaurantBasic | undefined;
    @Input("restaurantUsersInput") restaurantUsersInput!: RestaurantUser[] | undefined;
    restaurantUsers: RestaurantUser[] = [];

    searchFailed: boolean = false;


    ngOnInit(): void {
        setTimeout(() => {
            this.loadRestaurantUsersInput();
            this.clearNewRu();
        }, 0);
    }

    ngOnChanges(changes: SimpleChanges) {
        if (!!changes.restaurantUsersInput && changes.restaurantUsersInput.previousValue !== changes.restaurantUsersInput.currentValue) {
            this.loadRestaurantUsersInput();
        }
    }

    loadRestaurantUsersInput() {
        if (this.restaurantUsersInput !== undefined) {
            this.restaurantUsers = this.restaurantUsersInput;
        }
    }

    get givenUser() {
        return this.given === "user";
    }

    get givenRestaurant() {
        return this.given === "restaurant";
    }

    newRu: RestaurantUser = {
        canManage: false,
        canAcceptOrders: false,
        canDeliverOrders: false
    };

    constructor(private http: HttpClient) {
    }

    addNewRu() {
        this.restaurantUsers.push(this.newRu);
        this.clearNewRu();
    }

    clearNewRu() {
        this.newRu = {
            canManage: false,
            canAcceptOrders: false,
            canDeliverOrders: false,
        };
        if (this.givenRestaurant) {
            this.newRu.restaurant = this.restaurant;
        }
        if (this.givenUser) {
            this.newRu.user = this.user;
        }
    }

    userInputFormatter(u: UserBasic) {
        return `${u.username} (${u.firstName} ${u.lastName})`;
    }

    restaurantInputFormatter(r: RestaurantBasic) {
        return r.name;
    }

    typeaheadRequest(query: string): Observable<UserBasic[] | RestaurantBasic[]> {
        if (this.givenUser) {
            return this.http.get<RestaurantBasic[]>("/api/admin/restaurants/typeahead/", {params: {q: query}})
        } else {
            return this.http.get<UserBasic[]>("/api/admin/users/typeahead/", {params: {q: query}})
        }
    }

    typeahead: OperatorFunction<string, readonly UserBasic[] | RestaurantBasic[]> = (text$: Observable<string>) =>
        text$.pipe(
            debounceTime(200),
            distinctUntilChanged(),
            switchMap(term => {
                return (term === "") ? of([]) :
                this.typeaheadRequest(term).pipe(
                    tap(() => this.searchFailed = false),
                    catchError(() => {
                        this.searchFailed = true;
                        return of([]);
                    })
                )
            })
        )

    static updateWithUser(restaurantUsers: RestaurantUser[], user: UserAdmin): RestaurantUser[] {
        if (user.userId === undefined) return restaurantUsers;
        const userBasic = {userId: user.userId, username: user.username, firstName: user.firstName, lastName: user.lastName};
        return restaurantUsers.map(ru => {
            ru.user = userBasic;
            return ru;
        });
    }

    static updateWithRestaurant(restaurantUsers: RestaurantUser[], restaurant: RestaurantFull) {
        if (restaurant.restaurantId === undefined) return restaurantUsers;
        const restaurantBasic = {restaurantId: restaurant.restaurantId, name: restaurant.name};
        return restaurantUsers.map(ru => {
            ru.restaurant = restaurantBasic;
            return ru;
        });
    }
}
