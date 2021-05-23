import {Component, Input, OnInit} from '@angular/core';
import {RestaurantBasic} from "../../models/restaurant-basic";
import {Router} from "@angular/router";

@Component({
    selector: 'app-context-picker',
    templateUrl: './context-picker.component.html',
    styleUrls: ['./context-picker.component.scss']
})
export class ContextPickerComponent implements OnInit {
    @Input("restaurants") restaurants: RestaurantBasic[] = [];
    @Input("nextPath") nextPath!: string[];

    constructor(private router: Router) {
    }

    ngOnInit(): void {
    }

    nextUrl(restaurant: RestaurantBasic) {
        return this.nextPath[0] + restaurant.restaurantId + this.nextPath[1];
    }
}
