import {TestBed} from '@angular/core/testing';

import {StandardButtonService} from './standard-button.service';

describe('StandardButtonService', () => {
  let service: StandardButtonService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StandardButtonService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
