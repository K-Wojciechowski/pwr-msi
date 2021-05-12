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
import {
    NgbDatepickerModule,
    NgbPaginationModule,
    NgbTimepickerModule,
    NgbToastModule
} from "@ng-bootstrap/ng-bootstrap";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {AppRoutingModule} from "../app-routing.module";
import {PagerComponent} from "./pager/pager.component";
import {AddressDisplayComponent} from "./address-display/address-display.component";
import {AddressEditorComponent} from "./address-editor/address-editor.component";
import {EmptyStateComponent} from "./empty-state/empty-state.component";
import {FormTextareaComponent} from "./form-textarea/form-textarea.component";
import {ImageUploadComponent} from "./image-upload/image-upload.component";
import {ContextPickerComponent} from "./context-picker/context-picker.component";
import {SelectEditDateComponent} from "./select-edit-date/select-edit-date.component";
import {MenuCategoryTabsComponent} from "./menu-category-tabs/menu-category-tabs.component";

const components = [
    AddressDisplayComponent,
    AddressEditorComponent,
    BsIconComponent,
    ContextPickerComponent,
    DashboardAppBoxComponent,
    EmptyStateComponent,
    FormInputComponent,
    FormTextareaComponent,
    HeadingButtonsComponent,
    ImageUploadComponent,
    MenuCategoryTabsComponent,
    PagerComponent,
    ProgressSpinnerComponent,
    ProgressSpinnerContentComponent,
    ProgressSpinnerFullwidthComponent,
    ProgressSpinnerOverlayComponent,
    ToastsComponent,
    SelectEditDateComponent,
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
        NgbPaginationModule,
        NgbTimepickerModule,
        NgbDatepickerModule
    ],
})
export class WidgetsModule {
}
