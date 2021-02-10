import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-add-configuration-modal',
  templateUrl: './add-configuration-modal.component.html',
  styleUrls: ['./add-configuration-modal.component.css']
})
export class AddConfigurationModalComponent implements OnInit {

  public configurationURL?: string;
  public pollIntervalMinutes?: number;

  constructor(public dialogRef: MatDialogRef<AddConfigurationModalComponent>) { }

  ngOnInit(): void {
  }

  closeAndAdd(): void {
    this.dialogRef.close(
      {
        add: true,
        configurationURL: this.configurationURL,
        pollIntervalMinutes: this.pollIntervalMinutes
      });
  }

  close(): void {
    this.dialogRef.close(
      {
        add: false
      });
  }
}
