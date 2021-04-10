import {NgForm} from "@angular/forms";
import {keys, pick} from "lodash";

export function setFormValues(source: any, form: NgForm) {
    const formKeys = keys(form.controls);
    form.setValue(pick(source, formKeys));
}
