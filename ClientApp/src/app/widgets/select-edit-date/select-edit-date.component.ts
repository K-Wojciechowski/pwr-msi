import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {NgbDateStruct, NgbTimeStruct} from "@ng-bootstrap/ng-bootstrap";
import {DateTime, Duration} from "luxon";

@Component({
    selector: 'app-select-edit-date',
    templateUrl: './select-edit-date.component.html',
    styleUrls: ['./select-edit-date.component.scss']
})
export class SelectEditDateComponent implements OnInit {
    public date!: NgbDateStruct;
    public time!: NgbTimeStruct;
    public selectedDt: DateTime | null = null;
    public selectedDtStr: string = "?";
    @Output("selectedDate") selectedDate = new EventEmitter<DateTime | null>();

    constructor() {
        this.updateDateTime();
    }

    ngOnInit(): void {
        this.updateDateTime();
    }

    private updateDateTime() {
        const dt = DateTime.now().plus(Duration.fromObject({days: 1}));
        this.date = {day: dt.day, month: dt.month, year: dt.year};
        this.time = {hour: 4, minute: 0, second: 0};
    }

    public submit() {
        this.selectedDt = DateTime.fromObject({...this.date, ...this.time});
        this.selectedDtStr = this.selectedDt.toLocaleString(DateTime.DATETIME_FULL);
        this.selectedDate.emit(this.selectedDt);
    }

    public unselect() {
        this.selectedDt = null;
        this.selectedDtStr = "?";
        this.selectedDate.emit(this.selectedDt);
    }
}
