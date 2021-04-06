import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ProgressSpinnerContentComponent} from './progress-spinner-content.component';

describe('ProgressSpinnerContentComponent', () => {
  let component: ProgressSpinnerContentComponent;
  let fixture: ComponentFixture<ProgressSpinnerContentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProgressSpinnerContentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProgressSpinnerContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
