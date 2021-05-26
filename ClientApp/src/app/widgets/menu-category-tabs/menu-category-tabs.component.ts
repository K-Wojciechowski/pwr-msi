import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import { orderBy } from 'lodash';
import {SimpleMenuCategory} from "../../models/simple-menu-category";

@Component({
    selector: 'app-menu-category-tabs',
    templateUrl: './menu-category-tabs.component.html',
    styleUrls: ['./menu-category-tabs.component.scss']
})
export class MenuCategoryTabsComponent implements OnInit, OnChanges {
    @Input("categories") categoriesInput!: SimpleMenuCategory[];
    @Input("navClass") navClass: string = "";
    @Output("selection") selectionOutput = new EventEmitter<SimpleMenuCategory>();
    categories: SimpleMenuCategory[] = [];
    selectedId: number = -1;

    ngOnInit(): void {
        this.loadInput();
    }

    ngOnChanges(changes: SimpleChanges): void {
        this.loadInput();
    }

    loadInput() {
        this.categories = orderBy(this.categoriesInput ?? [], c => c.menuCategoryOrder);
        if (this.selectedId === -1 && this.categories.length > 0) {
            this.selectCategory(this.categories[0]);
        }
    }

    selectCategory(category: SimpleMenuCategory) {
        this.selectedId = category.menuCategoryId;
        this.selectionOutput.emit(category);
    }
}
