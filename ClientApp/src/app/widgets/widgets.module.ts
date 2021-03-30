import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {BsIconComponent} from "./bs-icon/bs-icon.component";
import {ProgressSpinnerComponent} from './progress-spinner/progress-spinner.component';
import {ProgressSpinnerOverlayComponent} from './progress-spinner-overlay/progress-spinner-overlay.component';
import {ToastsComponent} from "./toasts/toasts.component";
import {NgbToastModule} from "@ng-bootstrap/ng-bootstrap";

const components = [
    BsIconComponent,
    ProgressSpinnerComponent,
    ProgressSpinnerOverlayComponent,
    ToastsComponent,
];

@NgModule({
    declarations: components,
    exports: components,
    imports: [
        CommonModule,
        NgbToastModule
    ],
})
export class WidgetsModule {
}
