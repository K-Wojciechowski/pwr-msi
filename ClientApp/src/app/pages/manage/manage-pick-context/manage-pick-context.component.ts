import {Component, OnInit} from '@angular/core';
import {AuthStoreService} from "../../../services/auth-store.service";
import {RestaurantBasic} from "../../../models/restaurant-basic";
import {concat, sortBy, uniqBy} from "lodash";

@Component({
    selector: 'app-manage-pick-context',
    templateUrl: './manage-pick-context.component.html',
    styleUrls: ['./manage-pick-context.component.scss']
})
export class ManagePickContextComponent implements OnInit {
    public restaurants: RestaurantBasic[] = [];
    constructor(private authStore: AuthStoreService) {
    }

    ngOnInit(): void {
        this.authStore.access.subscribe(access => {
            if (access == null) {
                this.restaurants = [];
                return;
            }

            this.restaurants = uniqBy(sortBy(concat(access.manage, access.accept), r => r.name), r => r.restaurantId);
        })
    }

}
