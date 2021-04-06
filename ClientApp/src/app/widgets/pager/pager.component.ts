import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

@Component({
    selector: 'app-pager',
    templateUrl: './pager.component.html',
    styleUrls: ['./pager.component.scss']
})
export class PagerComponent implements OnInit {
    @Input("page") pageNumber!: number;
    @Input("totalItems") totalItems!: number;
    @Output("pageChange") pageChange = new EventEmitter<number>();

    constructor() {
    }

    ngOnInit(): void {
    }

}
