import {Address} from "./address";
import {Cuisine} from "./cuisine";

export interface RestaurantAdmin {
    restaurantId?: number;
    name: string;
    website: string;
    description: string;
    address: Address;
    logo: string | null;
    isActive: boolean;
    cuisines: Cuisine[];
}
