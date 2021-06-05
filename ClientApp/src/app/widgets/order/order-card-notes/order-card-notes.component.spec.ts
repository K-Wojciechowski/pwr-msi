import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderCardNotesComponent } from './order-card-notes.component';

describe('OrderCardNotesComponent', () => {
  let component: OrderCardNotesComponent;
  let fixture: ComponentFixture<OrderCardNotesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderCardNotesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderCardNotesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
