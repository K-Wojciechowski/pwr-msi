import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-order-card-base',
  templateUrl: './order-card-base.component.html',
  styleUrls: ['./order-card-base.component.scss']
})
export class OrderCardBaseComponent implements OnInit {
    @Input("title") title!: string;
    @Input("hideTitle") hideTitle: boolean = false;
    @Input("formFactor") formFactor: string = "unspecified";
    @Input("isImportant") isImportant: boolean = true;

  constructor() { }

  ngOnInit(): void {
  }

}
