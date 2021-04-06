import {Injectable} from '@angular/core';
import {StandardButton} from "../models/standard-button";

@Injectable({
  providedIn: 'root'
})
export class StandardButtonService {

  constructor() { }

    getButton(buttonType: string): StandardButton {
        switch (buttonType) {
            case "add":
                return {buttonClass: "success", iconName: "plus-circle", text: "Add"};
            case "edit":
                return {buttonClass: "info", iconName: "pencil", text: "Edit"};
            case "delete":
                return {buttonClass: "danger", iconName: "trash", text: "Delete"};
            case "show":
                return {buttonClass: "primary", iconName: "arrow-right-circle", text: "Show"};
            case "save":
                return {buttonClass: "success", iconName: "check-circle", text: "Save"};
            case "cancel":
                return {buttonClass: "dark", iconName: "x-circle", text: "Cancel"};
            default:
                return {buttonClass: "dark", iconName: "question-diamond-fill", text: "Unknown"};
        }
    }
}
