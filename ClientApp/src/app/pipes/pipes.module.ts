import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {LuxonDatePipe} from "./luxon-date.pipe";
import { SingleLineAddressPipe } from './single-line-address.pipe';
import { AmountPipe } from './amount.pipe';


@NgModule({
    declarations: [
        LuxonDatePipe,
        SingleLineAddressPipe,
        AmountPipe
    ],
    imports: [
        CommonModule
    ],
    exports: [
        LuxonDatePipe,
        SingleLineAddressPipe,
        AmountPipe
    ]
})
export class PipesModule {
}
