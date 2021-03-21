import { Component, OnInit } from '@angular/core';
import { WebDocument } from './webDocument';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-document-display',
  templateUrl: './document-display.component.html',
  styleUrls: ['./document-display.component.css']
})
export class DocumentDisplayComponent implements OnInit {

  selectedDocument?: WebDocument;

  documents: WebDocument[] = [];

  constructor(private httpClient: HttpClient) { }

  ngOnInit(): void {
  }

  getDocuments(): void {
    this.httpClient.get<WebDocument[]>('https://localhost:44335/api/document')
      .subscribe(value => this.documents = value);
  }

  onSelect(document: WebDocument): void {
    this.selectedDocument = document;
  }

}
