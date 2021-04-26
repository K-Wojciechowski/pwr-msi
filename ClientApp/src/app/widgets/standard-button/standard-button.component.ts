import {Component, ElementRef, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import {StandardButtonService} from "../../services/standard-button.service";

@Component({
  selector: '[standardButton]',
  templateUrl: './standard-button.component.html',
  styleUrls: ['./standard-button.component.scss']
})
export class StandardButtonComponent implements OnInit, OnChanges {
    @Input("btnType") buttonType!: string;
    @Input("btnStyle") buttonStyle!: string;
    @Input("text") userText!: string;

    buttonClasses: string = "btn";
    iconName: string = "";
    text: string = "";

    constructor(private el: ElementRef, private standardButtonService: StandardButtonService) {
    }

    ngOnInit(): void {
        this.recompute();
    }

    ngOnChanges(changes: SimpleChanges) {
        this.recompute();
    }

    recompute() {
        const standardButton = this.standardButtonService.getButton(this.buttonType);
        let btnClassBase: string;
        switch (this.buttonStyle) {
            case "heading":
                btnClassBase = "btn btn-";
                break;
            case "table":
                btnClassBase = "btn btn-sm btn-outline-";
                break;
            case "form":
                btnClassBase = "btn btn-lg btn-";
                break;
            default:
                btnClassBase = "btn btn-";
        }
        this.buttonClasses = btnClassBase + standardButton.buttonClass;
        this.text = standardButton.text;
        this.iconName = standardButton.iconName;
        if (this.userText !== "" && this.userText !== undefined) {
            this.text = this.userText;
        }
        this.el.nativeElement.className = this.buttonClasses;
    }
}
