import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageOrdersDetailsComponent } from './manage-orders-details.component';

describe('ManageOrdersDetailsComponent', () => {
  let component: ManageOrdersDetailsComponent;
  let fixture: ComponentFixture<ManageOrdersDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageOrdersDetailsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageOrdersDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
