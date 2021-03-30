import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {BaseModule} from "./base/base.module";
import {WidgetsModule} from "../widgets/widgets.module";
import {AuthModule} from "./auth/auth.module";



@NgModule({
    declarations: [],
    exports: [
        AuthModule,
        BaseModule
    ],
    imports: [
        CommonModule,
        WidgetsModule
    ]
})
export class PagesModule { }
