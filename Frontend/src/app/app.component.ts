import { Component } from '@angular/core';
import { LogViewerComponent } from './components/log-viewer/log-viewer.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [LogViewerComponent],
  template: '<app-log-viewer></app-log-viewer>',
  styles: [`
    :host {
      display: block;
      height: 100vh;
    }
  `]
})
export class AppComponent {
  title = 'K8s Log Analyzer';
}
