import {Address} from "./address";

export interface RestaurantBasicAddress {
    restaurantId: number;
    name: string;
    address?: Address | null;
    logo?: string | null;
}
