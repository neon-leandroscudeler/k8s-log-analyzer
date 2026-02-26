import { Component, OnInit, ViewChild } from '@angular/core';
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
import { MatRadioModule } from '@angular/material/radio';
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
    MatRadioModule
  ],
  templateUrl: './log-viewer.component.html',
  styleUrls: ['./log-viewer.component.scss']
})
export class LogViewerComponent implements OnInit {
  displayedColumns: string[] = ['timestamp', 'level', 'podName', 'message'];
  dataSource: MatTableDataSource<LogEntry>;
  
  namespace: string = '';
  podName: string = '';
  filterValue: string = '';
  isLoading: boolean = false;
  searchMode: 'single' | 'all' | 'without-canary' = 'single';

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
    
    const logObservable = this.searchMode === 'single'
      ? this.logService.getLogs(this.namespace, this.podName)
      : this.logService.getMultiplePodLogs(this.namespace, this.podName);

    logObservable.subscribe({
      next: (allLogs) => {
        // Filtrar canary se necessário
        let logs = allLogs;
        if (this.searchMode === 'without-canary') {
          logs = allLogs.filter(log => 
            !log.podName.includes('-canary') && 
            !log.podName.includes('-primary')
          );
        }
        
        this.dataSource.data = logs;
        this.isLoading = false;
        
        if (this.searchMode !== 'single') {
          const uniquePods = new Set(logs.map(l => l.podName));
          const modeLabel = this.searchMode === 'all' ? 'todos os pods' : 'pods sem canary';
          this.snackBar.open(`${logs.length} log entries de ${uniquePods.size} pod(s) (${modeLabel})`, 'Close', {
            duration: 4000,
            horizontalPosition: 'center',
            verticalPosition: 'bottom'
          });
        } else {
          this.snackBar.open(`Successfully loaded ${logs.length} log entries`, 'Close', {
            duration: 3000,
            horizontalPosition: 'center',
            verticalPosition: 'bottom'
          });
        }
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
