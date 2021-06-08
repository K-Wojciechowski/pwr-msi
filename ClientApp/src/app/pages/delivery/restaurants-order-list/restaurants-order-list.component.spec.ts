import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RestaurantsOrderListComponent } from './restaurants-order-list.component';

describe('RestaurantsOrderListComponent', () => {
  let component: RestaurantsOrderListComponent;
  let fixture: ComponentFixture<RestaurantsOrderListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RestaurantsOrderListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RestaurantsOrderListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
