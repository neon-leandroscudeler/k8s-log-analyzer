import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { LogEntry } from '../models/log-entry.model';

@Injectable({
  providedIn: 'root'
})
export class LogService {
  private apiUrl = 'http://localhost:5000/api/logs';

  constructor(private http: HttpClient) { }

  getLogs(namespace: string, podName: string): Observable<LogEntry[]> {
    const params = new HttpParams()
      .set('namespace', namespace)
      .set('podName', podName);

    return this.http.get<LogEntry[]>(this.apiUrl, { params }).pipe(
      map(logs => logs.map(log => ({
        ...log,
        timestamp: new Date(log.timestamp)
      })))
    );
  }

  getMultiplePodLogs(namespace: string, podNamePrefix: string): Observable<LogEntry[]> {
    const params = new HttpParams()
      .set('namespace', namespace)
      .set('podNamePrefix', podNamePrefix);

    return this.http.get<LogEntry[]>(`${this.apiUrl}/multiple`, { params }).pipe(
      map(logs => logs.map(log => ({
        ...log,
        timestamp: new Date(log.timestamp)
      })))
    );
  }
}
