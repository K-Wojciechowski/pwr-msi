import {Pipe, PipeTransform} from '@angular/core';
import {AmountUnit} from "../models/enum/amount-unit";

@Pipe({
    name: 'amount'
})
export class AmountPipe implements PipeTransform {

    transform(value: string | number, unit: string | AmountUnit, ...args: unknown[]): string {
        return value + " " + this.unitToString(unit);
    }

    unitToString(unit: string | AmountUnit): string {
        if (unit == "G" || unit == AmountUnit.G) return "g";
        if (unit == "DAG" || unit == AmountUnit.DAG) return "dag";
        if (unit == "KG" || unit == AmountUnit.KG) return "kg";
        if (unit == "ML" || unit == AmountUnit.ML) return "mL";
        if (unit == "L" || unit == AmountUnit.L) return "L";
        return "?";
    }
}
