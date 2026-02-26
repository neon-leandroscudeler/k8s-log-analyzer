export interface LogEntry {
  timestamp: Date;
  level: string;
  message: string;
  podName: string;
}

export interface LogQueryRequest {
  namespace: string;
  podName: string;
}

export interface MultiPodLogQueryRequest {
  namespace: string;
  podNamePrefix: string;
}
