import {Pipe, PipeTransform, SecurityContext} from '@angular/core';
import {Address} from "../models/address";
import {DomSanitizer, SafeHtml} from "@angular/platform-browser";

@Pipe({
    name: 'multiLineAddress'
})
export class MultiLineAddressPipe implements PipeTransform {
    constructor(private sanitizer: DomSanitizer) {
    }

    transform(value: Address, ...args: unknown[]): SafeHtml {
        const items = [value.addressee, value.firstLine, value.secondLine, value.postCode + " " + value.city];
        return this.sanitizer.bypassSecurityTrustHtml(items.map(i => i === undefined ? "" : this.sanitizer.sanitize(SecurityContext.HTML, i.trim())).filter(i => i).join("<br>"));
    }

}
