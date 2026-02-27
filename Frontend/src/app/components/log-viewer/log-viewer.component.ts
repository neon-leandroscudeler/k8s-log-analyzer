import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { LogService } from '../../services/log.service';
import { LogEntry } from '../../models/log-entry.model';

@Component({
  selector: 'app-log-viewer',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatSnackBarModule,
    MatSelectModule,
    MatTooltipModule
  ],
  templateUrl: './log-viewer.component.html',
  styleUrls: ['./log-viewer.component.scss']
})
export class LogViewerComponent implements OnInit, OnDestroy {
  displayedColumns: string[] = ['timestamp', 'level', 'podName', 'message'];
  dataSource: MatTableDataSource<LogEntry>;
  
  namespace: string = '';
  podName: string = '';
  filterValue: string = '';
  isLoading: boolean = false;
  searchMode: 'primary' | 'all' | 'without-canary' = 'primary';
  
  // Status de conexão
  k8sConnected: boolean = false;
  checkingConnection: boolean = true;
  clusterName: string = '';
  contextName: string = '';
  serverUrl: string = '';
  private statusCheckInterval: any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private logService: LogService,
    private snackBar: MatSnackBar
  ) {
    this.dataSource = new MatTableDataSource<LogEntry>([]);
  }

  ngOnInit(): void {
    this.dataSource.filterPredicate = (data: LogEntry, filter: string) => {
      const searchStr = filter.toLowerCase();
      return (
        data.timestamp.toLocaleString().toLowerCase().includes(searchStr) ||
        data.level.toLowerCase().includes(searchStr) ||
        data.podName.toLowerCase().includes(searchStr) ||
        data.message.toLowerCase().includes(searchStr)
      );
    };

    // Verificar status inicial
    this.checkK8sConnectionStatus();

    // Verificar status a cada 60 segundos
    this.statusCheckInterval = setInterval(() => {
      this.checkK8sConnectionStatus();
    }, 60000);
  }

  ngOnDestroy(): void {
    if (this.statusCheckInterval) {
      clearInterval(this.statusCheckInterval);
    }
  }

  checkK8sConnectionStatus(): void {
    this.logService.checkConnectionStatus().subscribe({
      next: (status) => {
        this.k8sConnected = status.connected;
        this.clusterName = status.clusterName || 'Unknown';
        this.contextName = status.contextName || 'Unknown';
        this.serverUrl = status.serverUrl || 'Unknown';
        this.checkingConnection = false;
      },
      error: () => {
        this.k8sConnected = false;
        this.clusterName = 'Unknown';
        this.contextName = 'Unknown';
        this.serverUrl = 'Unknown';
        this.checkingConnection = false;
      }
    });
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  fetchLogs(): void {
    if (!this.namespace || !this.podName) {
      this.snackBar.open('Please provide both namespace and pod name', 'Close', {
        duration: 3000,
        horizontalPosition: 'center',
        verticalPosition: 'top'
      });
      return;
    }

    this.isLoading = true;
    
    // Sempre buscar múltiplos pods
    this.logService.getMultiplePodLogs(this.namespace, this.podName).subscribe({
      next: (allLogs) => {
        // Filtrar conforme modo selecionado
        let logs = allLogs;
        
        if (this.searchMode === 'primary') {
          // Apenas pods com -primary no nome
          logs = allLogs.filter(log => log.podName.includes('-primary'));
        } else if (this.searchMode === 'without-canary') {
          // Sem canary nem primary
          logs = allLogs.filter(log => 
            !log.podName.includes('-canary') && 
            !log.podName.includes('-primary')
          );
        }
        // 'all' não filtra nada
        
        this.dataSource.data = logs;
        this.isLoading = false;
        
        const uniquePods = new Set(logs.map(l => l.podName));
        const modeLabels = {
          'primary': 'primary',
          'all': 'todos os pods',
          'without-canary': 'sem canary/primary'
        };
        
        this.snackBar.open(`${logs.length} logs de ${uniquePods.size} pod(s) (${modeLabels[this.searchMode]})`, 'Close', {
          duration: 4000,
          horizontalPosition: 'center',
          verticalPosition: 'bottom'
        });
      },
      error: (error) => {
        this.isLoading = false;
        let errorMessage = 'Error fetching logs';
        
        if (error.status === 404) {
          errorMessage = 'Pod not found';
        } else if (error.status === 403) {
          errorMessage = 'Access denied - check RBAC permissions';
        } else if (error.error?.error) {
          errorMessage = error.error.error;
        }

        this.snackBar.open(errorMessage, 'Close', {
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'top',
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  getClusterTooltip(): string {
    if (!this.k8sConnected) {
      return 'Não conectado ao cluster Kubernetes';
    }
    return `Cluster: ${this.clusterName}\nContexto: ${this.contextName}\nServidor: ${this.serverUrl}`;
  }

  getLogLevelClass(level: string): string {
    switch (level.toUpperCase()) {
      case 'ERROR':
        return 'log-level-error';
      case 'WARN':
      case 'WARNING':
        return 'log-level-warn';
      case 'INFO':
        return 'log-level-info';
      case 'DEBUG':
        return 'log-level-debug';
      default:
        return 'log-level-default';
    }
  }
}
