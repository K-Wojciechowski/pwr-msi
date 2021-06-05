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
import {PipesModule} from "../pipes/pipes.module";
import {OrderItemsComponent} from "./order/order-items/order-items.component";
import {OrderCardBaseComponent} from "./order/order-card-base/order-card-base.component";
import {OrderCardBaseNoborderComponent} from "./order/order-card-base-noborder/order-card-base-noborder.component";
import {OrderCardActionsComponent} from "./order/order-card-actions/order-card-actions.component";
import {OrderCardAddressComponent} from "./order/order-card-address/order-card-address.component";
import {OrderCardNotesComponent} from "./order/order-card-notes/order-card-notes.component";
import {OrderCardProgressComponent} from "./order/order-card-progress/order-card-progress.component";
import {OrderCardProgressIconComponent} from "./order/order-card-progress-icon/order-card-progress-icon.component";
import {OrderCardRestaurantComponent} from "./order/order-card-restaurant/order-card-restaurant.component";
import {CrystalLightboxModule} from "@crystalui/angular-lightbox";

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
    OrderItemsComponent,
    OrderCardBaseComponent,
    OrderCardBaseNoborderComponent,
    OrderCardActionsComponent,
    OrderCardAddressComponent,
    OrderCardNotesComponent,
    OrderCardProgressComponent,
    OrderCardProgressIconComponent,
    OrderCardRestaurantComponent,
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
        NgbDatepickerModule,
        PipesModule,
        CrystalLightboxModule,
    ],
})
export class WidgetsModule {
}
