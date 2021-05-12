export interface RestaurantMenuCategory {
    menuCategoryId: number;
    menuCategoryOrder: number;
    name: string;
    validFrom: string;
    validUntil?: string | null;
}
