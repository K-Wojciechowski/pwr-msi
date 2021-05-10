export interface MenuCategory {
    menuCategoryId: number;
    menuCategoryOrder: number;
    name: string;
    validFrom: string;
    validUntil?: string | null;
}
