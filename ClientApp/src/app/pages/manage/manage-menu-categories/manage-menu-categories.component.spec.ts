import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageMenuCategoriesComponent } from './manage-menu-categories.component';

describe('ManageMenuCategoriesComponent', () => {
  let component: ManageMenuCategoriesComponent;
  let fixture: ComponentFixture<ManageMenuCategoriesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageMenuCategoriesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageMenuCategoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
