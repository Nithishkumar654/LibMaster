import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BorrowViewComponent } from './borrow-view.component';

describe('BorrowViewComponent', () => {
  let component: BorrowViewComponent;
  let fixture: ComponentFixture<BorrowViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BorrowViewComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(BorrowViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
