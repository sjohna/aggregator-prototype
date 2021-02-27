import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PollConfiguration } from './pollConfiguration';
import { ConfigurationModalComponent } from '../configuration-modal/configuration-modal.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-poll-configuration-viewer',
  templateUrl: './poll-configuration-viewer.component.html',
  styleUrls: ['./poll-configuration-viewer.component.css']
})
export class PollConfigurationViewerComponent implements OnInit {

  configurations: PollConfiguration[] = [];

  constructor(private httpClient: HttpClient, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.getConfigurations();
  }

  getConfigurations(): void {
    this.httpClient.get<PollConfiguration[]>('https://localhost:44335/api/configuration/poll')
      .subscribe(value => this.configurations = value);
  }

  showAddConfigurationDialog(): void {
    const dialogRef = this.dialog.open(
      ConfigurationModalComponent, {
        data: { operation: 'Add' }
      });

    dialogRef.afterClosed().subscribe(result => {
      if (result.doOperation)
      {
        console.log(`Adding: ${result.configurationURL}, interval ${result.pollIntervalMinutes}`);

        this.httpClient.post<PollConfiguration>('https://localhost:44335/api/configuration/poll',
          {
            URL: result.configurationURL,
            pollIntervalMinutes: Number(result.pollIntervalMinutes),
            active: result.active
          }).subscribe(value => { console.log('Add successful'); this.getConfigurations(); } );

        this.getConfigurations();
      }
      else
      {
        console.log(`Add cancelled`);
      }
    });
  }

  showUpdateConfigurationDialog(configuration: PollConfiguration): void {
    const dialogRef = this.dialog.open(
      ConfigurationModalComponent, {
        data:
        {
          operation: 'Update',
          configurationURL: configuration.url,
          pollIntervalMinutes: configuration.pollIntervalMinutes,
          active: configuration.active,
          enableURLInput: false
        }
      });

    dialogRef.afterClosed().subscribe(result => {
      if (result.doOperation)
      {
        console.log(`Updating: ${configuration.id}`);

        this.httpClient.put<PollConfiguration>(`https://localhost:44335/api/configuration/poll/${configuration.id}`,
          {
            pollIntervalMinutes: Number(result.pollIntervalMinutes),
            active: result.active
          }).subscribe(value => { console.log('Update successful'); this.getConfigurations(); } );

        this.getConfigurations();
      }
      else
      {
        console.log(`Update cancelled`);
      }
    });
  }
}
