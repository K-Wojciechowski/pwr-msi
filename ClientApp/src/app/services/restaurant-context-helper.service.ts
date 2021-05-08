import {Injectable} from '@angular/core';
import {ActivatedRoute, Params} from "@angular/router";
import {filter, map} from "rxjs/operators";
import {MonoTypeOperatorFunction, Observable, OperatorFunction, pipe} from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class RestaurantContextHelperService {

    constructor(private route: ActivatedRoute) {
        this.route.params.subscribe(r => {
            console.log(r);
        })
    }

    get(): OperatorFunction<Params, number | null> {
        return pipe(map(m => {
            const restaurantId = +m["restaurantId"];
            return isNaN(restaurantId) ? null : restaurantId;
        }));
    }

    getReq(): OperatorFunction<Params, number> {
        return pipe(map(m => +m["restaurantId"]), filter(r => !isNaN(r)));
    }

}
