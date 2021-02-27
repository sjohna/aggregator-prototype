import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-confirm-modal',
  templateUrl: './confirm-modal.component.html',
  styleUrls: ['./confirm-modal.component.css']
})
export class ConfirmModalComponent implements OnInit {

  public title: string;
  public message: string;
  public confirmText: string;
  public cancelText: string;

  constructor(public dialogRef: MatDialogRef<ConfirmModalComponent>,
              @Inject(MAT_DIALOG_DATA) public data:
                {
                  title: string,
                  message: string,
                  confirmText?: string,
                  cancelText?: string
                })
    {
      this.title = data.title;
      this.message = data.message;
      this.confirmText = data.confirmText ?? 'Confirm';
      this.cancelText = data.cancelText ?? 'Cancel';
    }

  ngOnInit(): void {
  }

  confirm(): void {
    this.dialogRef.close({
      confirm: true
    });
  }

  cancel(): void {
    this.dialogRef.close({
      confirm: false
    });
  }

}
