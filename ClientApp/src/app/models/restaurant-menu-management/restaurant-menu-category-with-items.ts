import {RestaurantMenuCategory} from "./restaurant-menu-category";
import {RestaurantMenuItem} from "./restaurant-menu-item";

export interface RestaurantMenuCategoryWithItems extends RestaurantMenuCategory {
    items: RestaurantMenuItem[];
}
