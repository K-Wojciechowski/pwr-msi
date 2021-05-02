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
            case "generic":
                return {buttonClass: "primary", iconName: "arrow-right-circle", text: "Show"};
            case "save":
                return {buttonClass: "success", iconName: "check-circle", text: "Save"};
            case "cancel":
                return {buttonClass: "dark", iconName: "x-circle", text: "Cancel"};
            case "pay":
                return {buttonClass: "success", iconName: "cash", text: "Pay"};
            case "makepayment":
                return {buttonClass: "primary", iconName: "cash", text: "Make Payment"};
            case "refresh":
                return {buttonClass: "dark", iconName: "refresh", text: "Refresh"};
            default:
                return {buttonClass: "dark", iconName: "question-diamond-fill", text: "Unknown"};
        }
    }
}
