import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalesInvoices } from './sales-invoices';

describe('SalesInvoices', () => {
  let component: SalesInvoices;
  let fixture: ComponentFixture<SalesInvoices>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SalesInvoices]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SalesInvoices);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
