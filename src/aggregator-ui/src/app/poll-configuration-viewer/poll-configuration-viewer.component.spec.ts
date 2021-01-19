import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PollConfigurationViewerComponent } from './poll-configuration-viewer.component';

describe('PollConfigurationViewerComponent', () => {
  let component: PollConfigurationViewerComponent;
  let fixture: ComponentFixture<PollConfigurationViewerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PollConfigurationViewerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PollConfigurationViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
