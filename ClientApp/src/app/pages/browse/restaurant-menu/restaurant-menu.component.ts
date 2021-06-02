import {Component, OnInit} from '@angular/core';
import {RestaurantFull} from "../../../models/restaurant-full";
import {RestaurantMenuCategoryWithItems} from "../../../models/restaurant-menu-management/restaurant-menu-category-with-items";
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {ActivatedRoute} from "@angular/router";
import {SimpleMenuCategory} from "../../../models/simple-menu-category";
import {every, filter, flatMap, sortBy, sumBy} from "lodash";
import {RestaurantMenuItem} from "../../../models/restaurant-menu-management/restaurant-menu-item";
import {MenuItemWrapper} from 'src/app/models/menu-item-wrapper';
import {MenuItemOptionListWrapper} from "../../../models/menu-item-option-list-wrapper";
import {MenuItemOptionItemWrapper} from "../../../models/menu-item-option-item-wrapper";

@Component({
    selector: 'app-restaurant-menu',
    templateUrl: './restaurant-menu.component.html',
    styleUrls: ['./restaurant-menu.component.scss']
})
export class RestaurantMenuComponent implements OnInit {
    showLoading: number = 0;
    restaurantId!: number;
    restaurant: RestaurantFull | null = null;
    menu: RestaurantMenuCategoryWithItems[] = [];
    wrappedMenu: MenuItemWrapper[] = [];
    menuCategories: SimpleMenuCategory[] = [];
    currentCategory: SimpleMenuCategory | null = null;
    catItems: MenuItemWrapper[] = [];

    constructor(private http: HttpClient, private toastService: ToastService, private route: ActivatedRoute) {
    }

    ngOnInit(): void {
        this.loadData();
    }

    loadData() {
        this.showLoading = 2;
        const restaurantIdString = this.route.snapshot.paramMap.get("id");
        if (restaurantIdString === null) return;
        this.restaurantId = parseInt(restaurantIdString);
        this.http.get<RestaurantFull>(`/api/restaurants/${this.restaurantId}/`).subscribe(r => {
            this.restaurant = r;
            --this.showLoading;
        }, err => {
            this.toastService.handleHttpError(err);
            --this.showLoading;
        });
        this.http.get<RestaurantMenuCategoryWithItems[]>(`/api/restaurants/${this.restaurantId}/menu/`).subscribe(m => {
            this.menu = m;
            this.menuCategories = m;
            this.buildWrappedMenu();
            this.sortItems();
            --this.showLoading;
        }, err => {
            this.toastService.handleHttpError(err);
            --this.showLoading;
        });
    }

    selectCategory(category: SimpleMenuCategory) {
        this.currentCategory = category;
        this.sortItems();
    }

    private buildWrappedMenu() {
        const allItems: RestaurantMenuItem[] = flatMap(this.menu, i => i.items);
        this.wrappedMenu = allItems.map(i => {
            const options = i.options.map(ol => {
                const items = ol.items.map(oi => new MenuItemOptionItemWrapper(oi));
                const sortedItems = sortBy(items, oi => oi.value.menuItemOptionItemOrder);
                return new MenuItemOptionListWrapper(ol, sortedItems);
            })
            const sortedOptions = sortBy(options, ol => ol.value.menuItemOptionListOrder);
            return new MenuItemWrapper(i, sortedOptions);
        })
    }

    private sortItems() {
        if (this.menu === null) return;
        const menuCategoryId = this.currentCategory?.menuCategoryId;
        const catItems = this.wrappedMenu.filter(i => i.value.menuCategoryId === menuCategoryId);
        this.catItems = sortBy(catItems, i => i.value.menuOrder);
    }

    add(item: MenuItemWrapper) {
        console.log(item);
        // TODO
    }
}
