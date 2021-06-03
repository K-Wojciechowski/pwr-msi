import {Component, OnInit} from '@angular/core';
import {DateTime} from "luxon";
import {EditableWrapper} from "../../../models/editable-wrapper";
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {ActivatedRoute} from "@angular/router";
import {RestaurantContextHelperService} from "../../../services/restaurant-context-helper.service";
import {flatten, max, sortBy} from "lodash";
import {BulkSaveDto} from "../../../models/bulk-save-dto";
import {SimpleMenuCategory} from "../../../models/simple-menu-category";
import {RestaurantMenuItem} from "../../../models/restaurant-menu-management/restaurant-menu-item";
import {AmountUnit} from "../../../models/enum/amount-unit";
import {RestaurantMenuCategoryWithItems} from "../../../models/restaurant-menu-management/restaurant-menu-category-with-items";
import {RestaurantMenuItemOptionList} from "../../../models/restaurant-menu-management/restaurant-menu-item-option-list";
import {RestaurantMenuItemOptionItem} from "../../../models/restaurant-menu-management/restaurant-menu-item-option-item";
import {ResultDto} from "../../../models/result-dto";

@Component({
    selector: 'app-manage-menu-items',
    templateUrl: './manage-menu-items.component.html',
    styleUrls: ['./manage-menu-items.component.scss']
})
export class ManageMenuItemsComponent implements OnInit {
    public validFrom: DateTime | null = null;
    public showLoading: boolean = false;
    public simpleCategories: SimpleMenuCategory[] = [];
    public currentCategory: SimpleMenuCategory | null = null;
    public newItemName: string = "";
    public latestDate: DateTime | null = null;
    public catItems: EditableWrapper<RestaurantMenuItem>[] = [];
    private itemStore: EditableWrapper<RestaurantMenuItem>[] = [];
    private restaurantId!: number;

    public amountUnitOptions = [
        {value: AmountUnit.G, text: "g"},
        {value: AmountUnit.DAG, text: "dag"},
        {value: AmountUnit.KG, text: "kg"},
        {value: AmountUnit.ML, text: "mL"},
        {value: AmountUnit.L, text: "L"},
    ];

    constructor(private http: HttpClient, private toastService: ToastService, private route: ActivatedRoute, private contextHelper: RestaurantContextHelperService) {
    }

    ngOnInit(): void {
        this.route.params.pipe(this.contextHelper.getReq()).subscribe(id => this.restaurantId = id);
        this.prepareDateSelector();
    }

    setValidFrom(dt: DateTime | null) {
        this.validFrom = dt;
        if (dt != null) {
            this.loadData();
        }
    }

    get validFromStr(): string {
        return this.validFrom?.toISO() ?? "";
    }

    get getEndpoint(): string {
        return `/api/restaurants/${this.restaurantId}/menu/`;
    }

    get latestDateEndpoint(): string {
        return `/api/restaurants/${this.restaurantId}/menu/latest/`;
    }

    get bulkSaveEndpoint(): string {
        return `/api/restaurants/${this.restaurantId}/menu/bulk/`;
    }

    get imageEndpoint() {
        return `/api/uploads/${this.restaurantId}/menuitemphoto/`;
    }

    prepareDateSelector() {
        this.showLoading = true;
        this.http.get<ResultDto<string | null>>(this.latestDateEndpoint).subscribe(res => {
            this.latestDate = (res.result === null) ? null : DateTime.fromISO(res.result);
            this.showLoading = false;
        }, error => {
            this.toastService.handleHttpError(error);
            this.showLoading = false;
        });
    }

    loadData() {
        const reqOptions = {params: {validAt: this.validFrom?.toUTC().toISO() ?? ""}}
        this.showLoading = true;

        this.http.get<RestaurantMenuCategoryWithItems[]>(this.getEndpoint, reqOptions).subscribe(cats => {
            this.simpleCategories = sortBy(
                cats.map(cat => new SimpleMenuCategory(cat.menuCategoryId, cat.name, cat.menuCategoryOrder)),
                sc => sc.menuCategoryOrder
            );
            const allItems = flatten(cats.map(cat => cat.items));
            this.itemStore = allItems.map(item => new EditableWrapper(item));
            this.sortItems();
            this.showLoading = false;
        }, error => {
            this.toastService.handleHttpError(error);
            this.showLoading = false;
        });
    }

    selectCategory(category: SimpleMenuCategory) {
        this.currentCategory = category;
        this.sortItems();
    }

    private sortItems() {
        const menuCategoryId = this.currentCategory?.menuCategoryId;
        const catItems = this.itemStore.filter(i => (i.oldValue?.menuCategoryId ?? i.newValue!.menuCategoryId) === menuCategoryId);
        this.catItems = sortBy(catItems, i => [i.newValue?.menuOrder ?? i.oldValue!.menuOrder, i.isDeleted ? 1 : 0]);
        this.catItems.forEach(i => {
            if (i.oldValue !== null) {
                this.sortOptions(i.oldValue);
            }
            if (i.newValue !== null) {
                this.sortOptions(i.newValue);
            }
        });
    }

    private sortOptions(item: RestaurantMenuItem) {
        const optionsSortedContents = item.options.map(ol => {
            return {...ol, items: sortBy(ol.items, i => i.menuItemOptionItemOrder)};
        });
        item.options = sortBy(optionsSortedContents, i => i.menuItemOptionListOrder);
    }

    get newItemOrder() {
        const maxId = max(this.catItems.filter(i => i.newValue !== null).map(i => i.newValue!.menuOrder));
        return (maxId ?? 0) + 1;
    }

    addNewItem() {
        const item: RestaurantMenuItem = {
            menuCategoryId: this.currentCategory!.menuCategoryId,
            name: this.newItemName,
            description: "",
            image: null,
            price: "0",
            amount: "0",
            amountUnit: AmountUnit.G,
            menuOrder: this.newItemOrder,
            options: []
        };
        const wrapper = EditableWrapper.create(item);
        wrapper.isHighlighted = true;
        this.itemStore.push(wrapper);
        this.catItems.push(wrapper);
        this.newItemName = "";
    }

    doMove(thisIndex: number, otherIndex: number) {
        const movedItem = this.catItems[thisIndex];
        const displacedItem = this.catItems[otherIndex];
        const savedOrder = movedItem.newValue!.menuOrder;
        movedItem.newValue!.menuOrder = displacedItem.newValue?.menuOrder ?? displacedItem.oldValue!.menuOrder;
        if (displacedItem.newValue !== null) displacedItem.newValue.menuOrder = savedOrder;
        this.catItems[otherIndex] = movedItem;
        this.catItems[thisIndex] = displacedItem;
    }

    moveUp(i: number) {
        this.doMove(i, i - 1);
    }

    moveDown(i: number) {
        this.doMove(i, i + 1);
    }

    canMoveDown(i: number) {
        return i < (this.catItems.length - 1) && !this.catItems[i + 1].isDeleted;
    }

    undoChanges(item: EditableWrapper<RestaurantMenuItem>) {
        item.undoChanges();
        this.sortItems();
    }

    delete(item: EditableWrapper<RestaurantMenuItem>) {
        if (item.isAdded) {
            this.itemStore = this.itemStore.filter(storeItem => storeItem != item);
        } else {
            item.delete();
            item.isHighlighted = false;
        }
        this.sortItems();
    }

    addOptionList(item: EditableWrapper<RestaurantMenuItem>) {
        const order = (max(item.newValue!.options.map(o => o.menuItemOptionListOrder)) ?? 0) + 1;
        const newOption: RestaurantMenuItemOptionList = {
            name: "",
            isMultipleChoice: false,
            items: [],
            menuItemOptionListOrder: order
        };
        item.newValue!.options.push(newOption);
    }

    addOptionItem(opt: RestaurantMenuItemOptionList) {
        const order = (max(opt.items.map(o => o.menuItemOptionItemOrder)) ?? 0) + 1;
        const newItem: RestaurantMenuItemOptionItem = {
            name: "",
            price: "0",
            menuItemOptionItemOrder: order
        };
        opt.items.push(newItem);
    }

    doOptionListMove(list: RestaurantMenuItemOptionList[], thisIndex: number, otherIndex: number) {
        const movedItem = list[thisIndex];
        const displacedItem = list[otherIndex];
        const savedOrder = movedItem.menuItemOptionListOrder;
        movedItem.menuItemOptionListOrder = displacedItem.menuItemOptionListOrder;
        displacedItem.menuItemOptionListOrder = savedOrder;
        list[otherIndex] = movedItem;
        list[thisIndex] = displacedItem;
    }

    doOptionItemMove(list: RestaurantMenuItemOptionItem[], thisIndex: number, otherIndex: number) {
        const movedItem = list[thisIndex];
        const displacedItem = list[otherIndex];
        const savedOrder = movedItem.menuItemOptionItemOrder;
        movedItem.menuItemOptionItemOrder = displacedItem.menuItemOptionItemOrder;
        displacedItem.menuItemOptionItemOrder = savedOrder;
        list[otherIndex] = movedItem;
        list[thisIndex] = displacedItem;
    }


    moveOptionListUp(item: EditableWrapper<RestaurantMenuItem>, i: number) {
        this.doOptionListMove(item.newValue!.options, i, i - 1);
    }

    moveOptionListDown(item: EditableWrapper<RestaurantMenuItem>, i: number) {
        this.doOptionListMove(item.newValue!.options, i, i + 1);
    }

    moveOptionItemUp(list: RestaurantMenuItemOptionList, i: number) {
        this.doOptionItemMove(list.items, i, i - 1);
    }

    moveOptionItemDown(list: RestaurantMenuItemOptionList, i: number) {
        this.doOptionItemMove(list.items, i, i + 1);
    }

    deleteOptionList(item: EditableWrapper<RestaurantMenuItem>, i: number) {
        item.newValue!.options = item.newValue!.options.filter((_, index) => index !== i);
    }

    deleteOptionItem(list: RestaurantMenuItemOptionList, i: number) {
        list.items = list.items.filter((_, index) => index !== i);
    }

    filterMapItems(predicate: (e: EditableWrapper<RestaurantMenuItem>) => boolean): RestaurantMenuItem[] {
        return this.itemStore.filter(predicate).map(e => e.newValue ?? e.oldValue!);
    }

    submit() {
        const blankNames = this.itemStore.filter(item => item.newValue !== null && item.newValue.name.trim().length === 0);
        if (blankNames.length != 0) {
            this.toastService.showError("Cannot save an item with a blank name.");
            return;
        }

        const bulkSaveDto: BulkSaveDto<RestaurantMenuItem> = {
            added: this.filterMapItems(e => e.isAdded),
            edited: this.filterMapItems(e => e.isEdited),
            deleted: this.filterMapItems(e => e.isDeleted),
            validFrom: this.validFrom!.toISO()
        };

        this.showLoading = true;

        this.http.post(this.bulkSaveEndpoint, bulkSaveDto).subscribe(
            () => {
                this.toastService.showSuccess("Changes saved.");
                this.showLoading = false;
                this.loadData();
            }, error => {
                this.toastService.handleHttpError(error);
                this.showLoading = false;
            }
        )
    }
}
