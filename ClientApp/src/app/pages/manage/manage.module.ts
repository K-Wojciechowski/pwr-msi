import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ManagePickContextComponent} from './manage-pick-context/manage-pick-context.component';
import {WidgetsModule} from "../../widgets/widgets.module";
import {ManageIndexComponent} from "./manage-index/manage-index.component";
import { ManageMenuCategoriesComponent } from './manage-menu-categories/manage-menu-categories.component';
import {FormsModule} from "@angular/forms";


@NgModule({
    declarations: [ManageIndexComponent, ManagePickContextComponent, ManageMenuCategoriesComponent],
    imports: [
        CommonModule,
        WidgetsModule,
        FormsModule
    ]
})
export class ManageModule {
}
