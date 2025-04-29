import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PlagiarismDetails } from './PlagiarismDetails.model';
import { Chart } from 'chart.js/auto';
import { NotificationService } from '../../../notifications/notification.service';
import { switchMap } from 'rxjs';
import { StudentWork } from '../../studentWork.model';

@Injectable({
  providedIn: 'root',
})
export class DetailsService {
  private httpClient = inject(HttpClient);
  private destroyRef = inject(DestroyRef);
  private notificationService = inject(NotificationService);
  private chart: Chart | null = null;
  private plagiarismStats = signal<PlagiarismDetails[]>([]);
  private author = signal('');
  private fileInfo = signal<StudentWork | undefined>(undefined);

  loadedPlagiarismStats = this.plagiarismStats.asReadonly();
  loadedAuthor = this.author.asReadonly();
  loadedFileInfo = this.fileInfo.asReadonly();

  collectStats(fileId: string) {
    const subscription = this.httpClient
      .get<PlagiarismDetails[]>(
        `http://localhost:5000/api/studentwork/plagiarism/${fileId}`,
      )
      .pipe(
        switchMap((resp1) => {
          this.plagiarismStats.set(resp1);
          return this.httpClient.get(
            `http://localhost:5000/api/student/work/${fileId}`,
            { responseType: 'text' },
          );
        }),
        switchMap((resp2) => {
          this.author.set(resp2);
          return this.httpClient.get<StudentWork>(
            `http://localhost:5000/api/studentwork/${fileId}`,
          );
        }),
      )
      .subscribe({
        next: (resp3) => {
          this.fileInfo.set(resp3);
        },
        error: (err) => {
          this.notificationService.addErrorNotification(err.message);
          console.log(err);
        },
      });

    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  setPercentChart(ctx: CanvasRenderingContext2D, percent: number): void {
    if (this.chart) this.chart.destroy();
    this.chart = new Chart(ctx, {
      type: 'pie',
      data: {
        labels: ['Progress', 'Remaining'],
        datasets: [
          {
            data: [percent, 100 - percent],
            backgroundColor: ['#2c78b4', '#e0e0e0'],
            borderWidth: 0,
          },
        ],
      },
      options: {
        cutout: '70%',
        plugins: {
          legend: { display: false },
          tooltip: { enabled: false },
        },
      },
    });
  }
}
