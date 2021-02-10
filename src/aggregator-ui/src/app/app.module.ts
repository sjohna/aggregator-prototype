import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { PollConfigurationViewerComponent } from './poll-configuration-viewer/poll-configuration-viewer.component';
import { AddConfigurationModalComponent } from './add-configuration-modal/add-configuration-modal.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatDialogModule } from '@angular/material/dialog';

@NgModule({
  declarations: [
    AppComponent,
    PollConfigurationViewerComponent,
    AddConfigurationModalComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    MatDialogModule
  ],
  entryComponents: [
    AddConfigurationModalComponent
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
