import {Address} from "./address";

export interface UserAdmin {
    userId: number;
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    balance: string; // TODO verify
    isActive: boolean;
    isAdmin: boolean;
    isVerified: boolean;
    billingAddress: Address;
}
