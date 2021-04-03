import {NgModule} from '@angular/core';
import {AdminModule} from "./admin/admin.module";
import {AuthModule} from "./auth/auth.module";
import {BaseModule} from "./base/base.module";
import {CommonModule} from '@angular/common';
import {WidgetsModule} from "../widgets/widgets.module";

@NgModule({
    declarations: [],
    exports: [
        AdminModule,
        AuthModule,
        BaseModule
    ],
    imports: [
        CommonModule,
        WidgetsModule
    ]
})
export class PagesModule {
}
