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
                return {buttonClass: "success", iconName: "plus-circle-fill", text: "Add"};
            case "edit":
                return {buttonClass: "info", iconName: "pencil-fill", text: "Edit"};
            case "delete":
                return {buttonClass: "danger", iconName: "trash-fill", text: "Delete"};
            case "show":
                return {buttonClass: "primary", iconName: "arrow-right-circle-fill", text: "Show"};
            default:
                return {buttonClass: "dark", iconName: "question-diamond-fill", text: "Unknown"};
        }
    }
}
