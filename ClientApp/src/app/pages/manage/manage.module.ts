import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ManagePickContextComponent} from './manage-pick-context/manage-pick-context.component';
import {WidgetsModule} from "../../widgets/widgets.module";
import {ManageIndexComponent} from "./manage-index/manage-index.component";
import { ManageMenuCategoriesComponent } from './manage-menu-categories/manage-menu-categories.component';
import {FormsModule} from "@angular/forms";
import { ManageMenuItemsComponent } from './manage-menu-items/manage-menu-items.component';
import {NgbModule} from "@ng-bootstrap/ng-bootstrap";


@NgModule({
    declarations: [ManageIndexComponent, ManagePickContextComponent, ManageMenuCategoriesComponent, ManageMenuItemsComponent],
    imports: [
        CommonModule,
        WidgetsModule,
        FormsModule,
        NgbModule
    ]
})
export class ManageModule {
}
