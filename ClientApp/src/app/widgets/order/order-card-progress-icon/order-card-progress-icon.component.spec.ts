import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderCardProgressIconComponent } from './order-card-progress-icon.component';

describe('OrderCardProgressIconComponent', () => {
  let component: OrderCardProgressIconComponent;
  let fixture: ComponentFixture<OrderCardProgressIconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderCardProgressIconComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderCardProgressIconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
