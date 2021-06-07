import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderCardActionsComponent } from './order-card-actions.component';

describe('OrderCardActionsComponent', () => {
  let component: OrderCardActionsComponent;
  let fixture: ComponentFixture<OrderCardActionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderCardActionsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderCardActionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
