import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PollConfiguration } from './pollConfiguration';

@Component({
  selector: 'app-poll-configuration-viewer',
  templateUrl: './poll-configuration-viewer.component.html',
  styleUrls: ['./poll-configuration-viewer.component.css']
})
export class PollConfigurationViewerComponent implements OnInit {

  configurations: PollConfiguration[] = [];

  constructor(private httpClient: HttpClient) { }

  ngOnInit(): void {
  }

  getConfigurations(): void
  {
    this.httpClient.get<PollConfiguration[]>('https://localhost:44335/api/configuration/poll')
      .subscribe(value => this.configurations = value);
  }

}
