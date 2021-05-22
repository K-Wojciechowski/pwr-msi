import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {LuxonDatePipe} from "./luxon-date.pipe";


@NgModule({
    declarations: [
        LuxonDatePipe
    ],
    imports: [
        CommonModule
    ],
    exports: [
        LuxonDatePipe
    ]
})
export class PipesModule {
}
