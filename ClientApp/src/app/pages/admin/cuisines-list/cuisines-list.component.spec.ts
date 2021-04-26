import {ComponentFixture, TestBed} from '@angular/core/testing';

import {CuisinesListComponent} from './cuisines-list.component';

describe('CuisinesListComponent', () => {
  let component: CuisinesListComponent;
  let fixture: ComponentFixture<CuisinesListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CuisinesListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CuisinesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
