import {Component, Input, OnInit} from '@angular/core';

@Component({
    selector: 'app-empty-state',
    templateUrl: './empty-state.component.html',
    styleUrls: ['./empty-state.component.scss']
})
export class EmptyStateComponent implements OnInit {
    @Input("text") text: string = "No items found.";
    @Input("empty") empty!: boolean;

    constructor() {
    }

    ngOnInit(): void {
    }

}
