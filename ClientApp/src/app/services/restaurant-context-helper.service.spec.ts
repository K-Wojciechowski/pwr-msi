import { TestBed } from '@angular/core/testing';

import { RestaurantContextHelperService } from './restaurant-context-helper.service';

describe('RestaurantContextHelperService', () => {
  let service: RestaurantContextHelperService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RestaurantContextHelperService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
