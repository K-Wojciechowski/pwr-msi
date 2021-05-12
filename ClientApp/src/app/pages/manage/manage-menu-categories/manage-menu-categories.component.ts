import {Component, OnInit} from '@angular/core';
import {max, sortBy} from "lodash";
import {DateTime} from "luxon";
import {EditableWrapper} from "../../../models/editable-wrapper";
import {RestaurantMenuCategory} from "../../../models/restaurant-menu-management/restaurant-menu-category";
import {HttpClient} from "@angular/common/http";
import {ToastService} from "../../../services/toast.service";
import {RestaurantContextHelperService} from "../../../services/restaurant-context-helper.service";
import {ActivatedRoute} from "@angular/router";
import {BulkSaveDto} from "../../../models/bulk-save-dto";

@Component({
    selector: 'app-manage-menu-categories',
    templateUrl: './manage-menu-categories.component.html',
    styleUrls: ['./manage-menu-categories.component.scss']
})
export class ManageMenuCategoriesComponent implements OnInit {
    public validFrom: DateTime | null = null;
    public showLoading: boolean = false;
    public categories: EditableWrapper<RestaurantMenuCategory>[] = [];
    public newItemName: string = "";
    private restaurantId!: number;

    constructor(private http: HttpClient, private toastService: ToastService, private route: ActivatedRoute, private contextHelper: RestaurantContextHelperService) {
    }

    ngOnInit(): void {
        this.route.params.pipe(this.contextHelper.getReq()).subscribe(id => this.restaurantId = id);
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
        return `/api/restaurants/${this.restaurantId}/menu/categories/`;
    }

    get bulkSaveEndpoint(): string {
        return `/api/restaurants/${this.restaurantId}/menu/categories/bulk/`;
    }

    loadData() {
        const reqOptions = {params: {validAt: this.validFrom?.toUTC().toISO() ?? ""}}
        this.showLoading = true;
        this.http.get<RestaurantMenuCategory[]>(this.getEndpoint, reqOptions).subscribe(cats => {
            this.categories = cats.map(cat => new EditableWrapper<RestaurantMenuCategory>(cat));
            this.sortCategories();
            this.showLoading = false;
        }, error => {
            this.toastService.handleHttpError(error);
            this.showLoading = false;
        });
    }

    sortCategories() {
        this.categories = sortBy(this.categories, cat => [cat.newValue?.menuCategoryOrder ?? cat.oldValue!.menuCategoryOrder, cat.isDeleted ? 1 : 0]);
    }

    get newItemOrder() {
        const maxId = max(this.categories.filter(c => c.newValue !== null).map(c => c.newValue!.menuCategoryOrder));
        return (maxId ?? 0) + 1;
    }

    addNewItem() {
        const category: RestaurantMenuCategory = {
            menuCategoryId: 0,
            menuCategoryOrder: this.newItemOrder,
            name: this.newItemName,
            validFrom: this.validFromStr
        };
        const wrapper = EditableWrapper.create(category);
        this.categories.push(wrapper);
        this.newItemName = "";
    }

    doMove(thisIndex: number, otherIndex: number) {
        const movedCategory = this.categories[thisIndex];
        const displacedCategory = this.categories[otherIndex];
        const savedOrder = movedCategory.newValue!.menuCategoryOrder;
        movedCategory.newValue!.menuCategoryOrder = displacedCategory.newValue?.menuCategoryOrder ?? displacedCategory.oldValue!.menuCategoryOrder;
        if (displacedCategory.newValue !== null) displacedCategory.newValue.menuCategoryOrder = savedOrder;
        this.categories[otherIndex] = movedCategory;
        this.categories[thisIndex] = displacedCategory;
    }

    moveUp(i: number) {
        this.doMove(i, i - 1);
    }

    moveDown(i: number) {
        this.doMove(i, i + 1);
    }

    canMoveDown(i: number) {
        return i < (this.categories.length - 1) && !this.categories[i + 1].isDeleted;
    }

    undoChanges(cat: EditableWrapper<RestaurantMenuCategory>) {
        cat.undoChanges();
        this.sortCategories();
    }

    delete(cat: EditableWrapper<RestaurantMenuCategory>, i: number) {
        if (cat.isAdded) {
            this.categories = this.categories.filter((_, index) => index != i);
        } else {
            cat.delete();
        }
        this.sortCategories();
    }

    filterMapCategories(predicate: (e: EditableWrapper<RestaurantMenuCategory>) => boolean): RestaurantMenuCategory[] {
        return this.categories.filter(predicate).map(e => e.newValue ?? e.oldValue!);
    }

    submit() {
        const blankNames = this.categories.filter(cat => cat.newValue !== null && cat.newValue.name.trim().length === 0);
        if (blankNames.length != 0) {
            this.toastService.showError("Cannot save a category with a blank name.");
            return;
        }

        this.showLoading = true;

        const bulkSaveDto: BulkSaveDto<RestaurantMenuCategory> = {
            added: this.filterMapCategories(e => e.isAdded),
            edited: this.filterMapCategories(e => e.isEdited),
            deleted: this.filterMapCategories(e => e.isDeleted),
            validFrom: this.validFrom!.toISO()
        };

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
