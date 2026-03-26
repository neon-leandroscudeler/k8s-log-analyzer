import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { LogEntry } from '../models/log-entry.model';

export interface ConnectionStatus {
  connected: boolean;
  status: string;
  clusterName?: string;
  contextName?: string;
  serverUrl?: string;
}

export interface KubernetesContext {
  name: string;
  cluster: string;
  user: string;
  isCurrent: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class LogService {
  private apiUrl = '/api/logs';

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

  checkConnectionStatus(): Observable<ConnectionStatus> {
    return this.http.get<ConnectionStatus>(`${this.apiUrl}/status`);
  }

  getAvailableContexts(): Observable<KubernetesContext[]> {
    return this.http.get<KubernetesContext[]>(`${this.apiUrl}/contexts`);
  }

  switchContext(contextName: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/contexts/switch`, JSON.stringify(contextName), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
