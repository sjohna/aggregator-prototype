import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddConfigurationModalComponent } from './add-configuration-modal.component';

describe('AddConfigurationModalComponent', () => {
  let component: AddConfigurationModalComponent;
  let fixture: ComponentFixture<AddConfigurationModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddConfigurationModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddConfigurationModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
