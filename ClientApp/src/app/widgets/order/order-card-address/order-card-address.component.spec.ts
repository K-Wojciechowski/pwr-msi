import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderCardAddressComponent } from './order-card-address.component';

describe('OrderCardAddressComponent', () => {
  let component: OrderCardAddressComponent;
  let fixture: ComponentFixture<OrderCardAddressComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderCardAddressComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderCardAddressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
