import {Pipe, PipeTransform} from '@angular/core';
import {DateTime} from "luxon";

@Pipe({
    name: 'ldate'
})
export class LuxonDatePipe implements PipeTransform {

    transform(value: string | DateTime | null, ...args: string[]): string | null {
        if (value === null) return null;
        const dtValue = typeof value === 'string' ? DateTime.fromISO(value) : value;
        return dtValue.toLocaleString(DateTime.DATETIME_FULL);
    }

}
