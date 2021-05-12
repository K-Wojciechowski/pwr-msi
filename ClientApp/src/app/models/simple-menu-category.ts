export class SimpleMenuCategory {
    menuCategoryId: number;
    name: string;
    menuCategoryOrder: number;

    constructor(menuCategoryId: number, name: string, menuCategoryOrder: number = 0) {
        this.menuCategoryId = menuCategoryId;
        this.name = name;
        this.menuCategoryOrder = menuCategoryOrder;
    }
}
