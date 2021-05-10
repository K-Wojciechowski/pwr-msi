import { cloneDeep, isEqual } from "lodash";

export class EditableWrapper<T> {
    oldValue: T | null;
    newValue: T | null;

    constructor(value: T | null) {
        this.oldValue = value;
        this.newValue = cloneDeep(value);
    }

    get isAdded() {
        return this.oldValue === null;
    }

    get isEdited() {
        return this.oldValue !== null && this.newValue !== null && !isEqual(this.oldValue, this.newValue);
    }

    get isDeleted() {
        return this.newValue === null;
    }

    get canUndo() {
        return this.isDeleted || this.isEdited;
    }

    get canDelete() {
        return this.newValue != null;
    }

    delete() {
        this.newValue = null;
    }

    undoChanges() {
        this.newValue = cloneDeep(this.oldValue);
    }

    static create<T>(value: T) {
        const wrapper = new EditableWrapper<T>(null);
        wrapper.newValue = value;
        return wrapper;
    }
}
