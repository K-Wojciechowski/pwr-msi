import {TestBed} from '@angular/core/testing';

import {MsiHttpService} from './msi-http.service';

describe('MsiHttpService', () => {
  let service: MsiHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MsiHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
