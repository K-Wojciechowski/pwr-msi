export class OrderAction {
    id: string;
    text: string;
    btnCls: string;
    disabled: boolean;

    constructor(id: string, text: string, btnCls: string = "primary", disabled: boolean = false) {
        this.id = id;
        this.text = text;
        this.btnCls = btnCls;
        this.disabled = disabled;
    }
}
