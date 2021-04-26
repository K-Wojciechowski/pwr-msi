import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {BsIconComponent} from "./bs-icon/bs-icon.component";
import {DashboardAppBoxComponent} from "./dashboard-app-box/dashboard-app-box.component";
import {FormInputComponent} from "./form-input/form-input.component";
import {HeadingButtonsComponent} from "./heading-buttons/heading-buttons.component";
import {ProgressSpinnerComponent} from './progress-spinner/progress-spinner.component';
import {ProgressSpinnerContentComponent} from './progress-spinner-content/progress-spinner-content.component';
import {ProgressSpinnerFullwidthComponent} from './progress-spinner-fullwidth/progress-spinner-fullwidth.component';
import {ProgressSpinnerOverlayComponent} from './progress-spinner-overlay/progress-spinner-overlay.component';
import {ToastsComponent} from "./toasts/toasts.component";
import {StandardButtonComponent} from "./standard-button/standard-button.component";
import {NgbPaginationModule, NgbToastModule} from "@ng-bootstrap/ng-bootstrap";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {AppRoutingModule} from "../app-routing.module";
import {PagerComponent} from "./pager/pager.component";
import {AddressEditorComponent} from "./address-editor/address-editor.component";
import {EmptyStateComponent} from "./empty-state/empty-state.component";
import {FormTextareaComponent} from "./form-textarea/form-textarea.component";

const components = [
    AddressEditorComponent,
    BsIconComponent,
    DashboardAppBoxComponent,
    EmptyStateComponent,
    FormInputComponent,
    FormTextareaComponent,
    HeadingButtonsComponent,
    PagerComponent,
    ProgressSpinnerComponent,
    ProgressSpinnerContentComponent,
    ProgressSpinnerFullwidthComponent,
    ProgressSpinnerOverlayComponent,
    ToastsComponent,
    StandardButtonComponent,
];

@NgModule({
    declarations: components,
    exports: components,
    imports: [
        CommonModule,
        NgbToastModule,
        FormsModule,
        ReactiveFormsModule,
        AppRoutingModule,
        NgbPaginationModule
    ],
})
export class WidgetsModule {
}
