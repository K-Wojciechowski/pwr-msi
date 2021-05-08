import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ManagePickContextComponent} from './manage-pick-context/manage-pick-context.component';
import {WidgetsModule} from "../../widgets/widgets.module";
import {ManageIndexComponent} from "./manage-index/manage-index.component";


@NgModule({
    declarations: [ManageIndexComponent, ManagePickContextComponent],
    imports: [
        CommonModule,
        WidgetsModule
    ]
})
export class ManageModule {
}
