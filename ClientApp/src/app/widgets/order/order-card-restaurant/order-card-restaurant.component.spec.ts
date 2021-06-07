import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderCardRestaurantComponent } from './order-card-restaurant.component';

describe('OrderCardRestaurantComponent', () => {
  let component: OrderCardRestaurantComponent;
  let fixture: ComponentFixture<OrderCardRestaurantComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderCardRestaurantComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderCardRestaurantComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
