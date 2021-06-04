import {Component, EventEmitter, Input, OnInit, Output, SimpleChanges, ViewChild} from '@angular/core';
import {NgForm} from "@angular/forms";
import {setFormValues} from "../../../../utils";
import {UserProfile} from "../../../models/user-profile";
import {Address} from "../../../models/address";

@Component({
  selector: 'app-profile-editor',
  templateUrl: './profile-editor.component.html',
  styleUrls: ['./profile-editor.component.scss']
})
export class ProfileEditorComponent implements OnInit {
    @Input("required") required: boolean = false;
    @Input("user") userInput!: UserProfile | undefined;
    @Output("userChange") userChange = new EventEmitter<UserProfile>();
    @ViewChild("f", {static: true}) form!: NgForm;
    user: UserProfile = this.newUser();
    userId: number | undefined;


    constructor() { }

    ngOnInit(): void {
        if (this.userInput !== undefined) {
            this.user = this.userInput;
        }
        this.form.valueChanges?.subscribe(v => {
            this.user = this.getUser(v);

        });
    }
    submit(){
        this.userChange.emit(this.user);
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes.userInput.previousValue !== changes.userInput.currentValue && this.userInput !== undefined) {
            console.log(this.user);
            setTimeout(() => setFormValues(changes.userInput.currentValue, this.form), 0);
        }
    }

    getUser(formValue: any): UserProfile {
        return {
            userId: this.userId,
            ...formValue,
          
        };
    }
    newUser(): UserProfile {
        return {
            userId: this.userId,
            firstName: "",
            lastName: "",
            username: "",
            email: "",
            balance: 0,
            billingAddress: this.newAddress()
         
        }
    }
    newAddress(): Address {
        return {
            addressee: "",
            firstLine: "",
            secondLine: "",
            postCode: "",
            city: "",
            country: "PL"
        }
    }


}
