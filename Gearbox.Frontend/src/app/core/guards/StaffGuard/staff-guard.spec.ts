import { TestBed } from '@angular/core/testing';
import { CanMatchFn } from '@angular/router';

import { staffGuard } from './staff-guard';

describe('staffGuard', () => {
  const executeGuard: CanMatchFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => staffGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
