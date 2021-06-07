import {UserBasic} from "./user-basic";

export interface Deliverer {
    user: UserBasic;
    activeTasks: number;
}
