import {Injectable} from '@angular/core';
import {Params} from "@angular/router";
import {filter, map} from "rxjs/operators";
import {OperatorFunction, pipe} from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class RestaurantContextHelperService {
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
