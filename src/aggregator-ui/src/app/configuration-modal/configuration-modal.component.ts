import { hostViewClassName } from '@angular/compiler';
import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

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

  constructor(public dialogRef: MatDialogRef<ConfigurationModalComponent>,
              @Inject(MAT_DIALOG_DATA) public data:
                {
                  operation: string,
                  configurationURL?: string,
                  pollIntervalMinutes?: number,
                  active?: boolean
                })
  {
    this.active = data.active ?? true;
    this.configurationURL = data.configurationURL;
    this.pollIntervalMinutes = data.pollIntervalMinutes;
    this.operation = data.operation;
  }

  ngOnInit(): void {
  }

  closeAndAdd(): void {
    this.dialogRef.close(
      {
        doOperation: true,
        configurationURL: this.configurationURL,
        pollIntervalMinutes: this.pollIntervalMinutes,
        active: this.active
      });
  }

  close(): void {
    this.dialogRef.close(
      {
        doOperation: false
      });
  }
}
