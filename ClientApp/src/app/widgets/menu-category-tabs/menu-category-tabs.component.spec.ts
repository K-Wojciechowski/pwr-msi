import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MenuCategoryTabsComponent } from './menu-category-tabs.component';

describe('MenuCategoryTabsComponent', () => {
  let component: MenuCategoryTabsComponent;
  let fixture: ComponentFixture<MenuCategoryTabsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MenuCategoryTabsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MenuCategoryTabsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
