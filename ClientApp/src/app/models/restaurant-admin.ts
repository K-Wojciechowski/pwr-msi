import {Address} from "./address";

export interface RestaurantAdmin {
    restaurantId?: number;
    name: string;
    website: string;
    description: string;
    address: Address;
}
