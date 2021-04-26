import {Component, ElementRef, forwardRef, Input, ViewChild} from '@angular/core';
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from "@angular/forms";

@Component({
    selector: 'app-form-textarea',
    templateUrl: './form-textarea.component.html',
    styleUrls: ['./form-textarea.component.scss'],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            multi: true,
            useExisting: forwardRef(() => FormTextareaComponent),
        }
    ]
})
export class FormTextareaComponent implements ControlValueAccessor {
    @Input("name") name!: string;
    @Input("desc") desc!: string;
    @Input("type") type!: string;
    @Input("required") required!: boolean;
    @ViewChild("input") input!: ElementRef;

    private _value: any = '';
    get value(): any {
        return this._value;
    };

    set value(v: any) {
        if (v !== this._value) {
            this._value = v;
            this.onChange(v);
        }
    }

    writeValue(value: any) {
        this._value = value;
        this.onChange(value);
    }

    onChange = (_: any) => {
    };
    onTouched = () => {
    };

    registerOnChange(fn: (_: any) => void): void {
        this.onChange = fn;
    }

    registerOnTouched(fn: () => void): void {
        this.onTouched = fn;
    }
}
