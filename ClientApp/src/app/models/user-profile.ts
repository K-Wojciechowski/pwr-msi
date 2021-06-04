import {Address} from "./address";

export interface UserProfile {
    userId?: number;
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    balance: number;

    billingAddress: Address;
}
