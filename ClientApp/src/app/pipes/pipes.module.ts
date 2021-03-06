import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {LuxonDatePipe} from "./luxon-date.pipe";
import { SingleLineAddressPipe } from './single-line-address.pipe';
import { AmountPipe } from './amount.pipe';
import { MultiLineAddressPipe } from './multi-line-address.pipe';


@NgModule({
    declarations: [
        LuxonDatePipe,
        SingleLineAddressPipe,
        AmountPipe,
        MultiLineAddressPipe
    ],
    imports: [
        CommonModule
    ],
    exports: [
        LuxonDatePipe,
        SingleLineAddressPipe,
        AmountPipe,
        MultiLineAddressPipe
    ]
})
export class PipesModule {
}
