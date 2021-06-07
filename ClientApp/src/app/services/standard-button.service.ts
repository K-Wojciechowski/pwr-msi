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
            case "continue":
                return {buttonClass: "primary", iconName: "arrow-right-circle", text: "Continue"};
            case "save":
                return {buttonClass: "success", iconName: "check-circle", text: "Save"};
            case "cancel":
                return {buttonClass: "dark", iconName: "x-circle", text: "Cancel"};
            case "orderprimary":
                return {buttonClass: "primary", iconName: "bag", text: "Place Order"};
            case "order":
                return {buttonClass: "success", iconName: "bag-check", text: "Place Order"};
            case "orderpay":
                return {buttonClass: "success", iconName: "bag-check", text: "Order & Pay"};
            case "pay":
                return {buttonClass: "success", iconName: "cash", text: "Pay"};
            case "makepayment":
                return {buttonClass: "primary", iconName: "cash", text: "Make Payment"};
            case "refresh":
                return {buttonClass: "dark", iconName: "refresh", text: "Refresh"};
            case "moveup":
                return {buttonClass: "secondary", iconName: "arrow-up", text: "Move Up"};
            case "movedown":
                return {buttonClass: "secondary", iconName: "arrow-down", text: "Move Down"};
            case "undo":
                return {buttonClass: "warning", iconName: "arrow-counterclockwise", text: "Undo"}
            case "secondary":
                return {buttonClass: "secondary", iconName: "arrow-right-circle", text: "Continue"}
            default:
                return {buttonClass: "dark", iconName: "question-diamond-fill", text: "Unknown"};
        }
    }
}
