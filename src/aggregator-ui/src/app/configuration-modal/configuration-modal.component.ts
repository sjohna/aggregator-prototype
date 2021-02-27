import { hostViewClassName } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-configuration-modal',
  templateUrl: './configuration-modal.component.html',
  styleUrls: ['./configuration-modal.component.css']
})
export class ConfigurationModalComponent implements OnInit {

  public configurationURL?: string;
  public pollIntervalMinutes?: number;
  public active: boolean;
  public operation: string;

  constructor(public dialogRef: MatDialogRef<ConfigurationModalComponent>)
  {
    this.active = true;
    this.operation = 'Addily-ho';
  }

  ngOnInit(): void {
  }

  closeAndAdd(): void {
    this.dialogRef.close(
      {
        add: true,
        configurationURL: this.configurationURL,
        pollIntervalMinutes: this.pollIntervalMinutes,
        active: this.active
      });
  }

  close(): void {
    this.dialogRef.close(
      {
        add: false
      });
  }
}
