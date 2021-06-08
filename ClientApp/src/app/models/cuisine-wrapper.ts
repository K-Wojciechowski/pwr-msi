import {ObjectWrapper} from "./object-wrapper";
import {Cuisine} from "./cuisine";

export class CuisineWrapper extends ObjectWrapper<Cuisine> {
    isChecked: boolean = false;

    constructor(cuisine: Cuisine) {
        super(cuisine);
    }
}

