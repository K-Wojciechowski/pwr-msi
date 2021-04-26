import {Cuisine} from "./cuisine";

export interface EditableCuisine extends Cuisine {
    isEditing: boolean;
    newName: string;
}
