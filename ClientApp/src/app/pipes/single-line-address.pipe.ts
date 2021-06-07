import {Pipe, PipeTransform} from '@angular/core';
import {Address} from "../models/address";

@Pipe({
    name: 'singleLineAddress'
})
export class SingleLineAddressPipe implements PipeTransform {

    transform(value: Address, ...args: unknown[]): string {
        const items = [value.firstLine, value.secondLine, value.postCode + " " + value.city];
        return items.map(i => i === undefined ? "" : i.trim()).filter(i => i).join(", ")
    }

}
