import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileEditorComponent } from './profile-editor/profile-editor.component';
import { AddressListComponent } from './address-list/address-list.component';
import { AddressEditComponent } from './address-edit/address-edit.component';
import { WidgetsModule } from "../../widgets/widgets.module";
import { AppRoutingModule } from "../../app-routing.module";
import { AddressAddComponent } from './address-add/address-add.component';
import { ProfileEditComponent } from './profile-edit/profile-edit.component';
import {FormsModule} from "@angular/forms";



@NgModule({
  declarations: [ProfileEditorComponent, AddressListComponent, AddressEditComponent, AddressAddComponent, ProfileEditComponent],
    imports: [
        CommonModule,
        WidgetsModule,
        AppRoutingModule,
        FormsModule
    ]
})
export class AccountModule { }
