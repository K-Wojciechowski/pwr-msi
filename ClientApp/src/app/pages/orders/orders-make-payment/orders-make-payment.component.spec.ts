import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdersMakePaymentComponent } from './orders-make-payment.component';

describe('OrdersMakePaymentComponent', () => {
  let component: OrdersMakePaymentComponent;
  let fixture: ComponentFixture<OrdersMakePaymentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrdersMakePaymentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdersMakePaymentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
