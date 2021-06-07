import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderCardDeliveryComponent } from './order-card-delivery.component';

describe('OrderCardDeliveryComponent', () => {
  let component: OrderCardDeliveryComponent;
  let fixture: ComponentFixture<OrderCardDeliveryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderCardDeliveryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderCardDeliveryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
