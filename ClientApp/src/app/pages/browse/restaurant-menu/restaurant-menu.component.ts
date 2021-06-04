import {Component, OnInit, TemplateRef} from '@angular/core';
import {RestaurantFull} from "../../../models/restaurant-full";
import {RestaurantMenuCategoryWithItems} from "../../../models/menu/restaurant-menu-category-with-items";
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {ActivatedRoute, Router} from "@angular/router";
import {SimpleMenuCategory} from "../../../models/simple-menu-category";
import {cloneDeep, flatMap, sortBy, sumBy} from "lodash";
import {RestaurantMenuItem} from "../../../models/menu/restaurant-menu-item";
import {MenuItemWrapper} from 'src/app/models/menu-item-wrapper';
import {MenuItemOptionListWrapper} from "../../../models/menu-item-option-list-wrapper";
import {MenuItemOptionItemWrapper} from "../../../models/menu-item-option-item-wrapper";
import {Address} from "../../../models/address";
import {NgbModal, NgbModalRef} from "@ng-bootstrap/ng-bootstrap";
import {Order} from "../../../models/order/order";
import {OrderStatus} from "../../../models/enum/order-status";
import {DateTime} from "luxon";
import {AuthStoreService} from "../../../services/auth-store.service";

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
    cart: MenuItemWrapper[] = [];
    cartTotal: number = 0;
    addresses: Address[] = [];
    deliveryAddress: Address | null = null;
    deliveryNotes: string = "";
    payAfterOrder: boolean = true;
    addressModal: NgbModalRef | null = null;

    constructor(private http: HttpClient, private toastService: ToastService, private router: Router, private route: ActivatedRoute, private authStore: AuthStoreService, private modalService: NgbModal) {
    }

    ngOnInit(): void {
        this.loadData();
    }

    loadData() {
        this.showLoading = 4;
        const restaurantIdString = this.route.snapshot.paramMap.get("id");
        if (restaurantIdString === null) return;
        this.restaurantId = parseInt(restaurantIdString);
        this.http.get<RestaurantFull>(`/api/restaurants/${this.restaurantId}/`).subscribe(r => {
            this.restaurant = r;
            // TODO for testing
            this.addresses = [r.address, {...r.address, addressee: "test2", addressId: 1234}];
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
        this.http.get<Address[]>(`/api/addresses/`).subscribe(a => {
            this.addresses = a;
            --this.showLoading;
        }, err => {
            this.toastService.handleHttpError(err);
            --this.showLoading;
        });
        this.http.get<Address | null>(`/api/addresses/default/`).subscribe(a => {
            if (a !== null) {
                this.deliveryAddress = a;
            }
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
        this.cart.push(cloneDeep(item));
        this.cartTotal = sumBy(this.cart, i => i.total);
    }

    delete(index: number) {
        this.cart = this.cart.filter((_, itemIndex) => itemIndex != index);
        this.cartTotal = sumBy(this.cart, i => i.total);
    }

    reloadAddressesSilently() {
        this.http.get<Address[]>(`/api/addresses/`).subscribe(a => {
            this.addresses = a;
        }, err => {
            this.toastService.handleHttpError(err);
        });
    }

    openAddressPicker(addressModal: TemplateRef<any>) {
        this.reloadAddressesSilently();
        this.addressModal = this.modalService.open(addressModal, {size: 'lg'});
    }

    selectAddress(address: Address) {
        this.deliveryAddress = address;
        this.addressModal?.close("Selected");
    }

    placeOrder() {
        const now = DateTime.now().toISO();
        const items = this.cart.map(c => c.asOrderItem());
        const order: Order = {
            totalPrice: this.cartTotal.toString(),
            deliveryNotes: this.deliveryNotes,
            status: OrderStatus.CREATED,

            created: now,
            updated: now,

            restaurant: {
                restaurantId: this.restaurant!.restaurantId ?? -1,
                name: this.restaurant!.name,
            },
            deliveryPerson: null,
            address: this.deliveryAddress!,

            items: items
        };
        this.showLoading += 1;
        this.http.post<Order>("/api/orders/", order).subscribe(o => {
            this.toastService.showSuccess("Order created successfully!");
            this.showLoading -= 1;
            const nextUrl = this.payAfterOrder ? ['orders', o.orderId, 'pay'] : ['orders', o.orderId];
            this.router.navigate(nextUrl).then(_ => undefined);
        }, err => {
            this.toastService.handleHttpError(err);
            this.showLoading -= 1;
        });
    }
}
