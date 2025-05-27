import {
  Component,
  computed,
  DestroyRef,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { StudentInfoService } from '../studentInfo.service';
import { StudentWork } from './studentWork.model';
import { StudentWorkComponent } from './student-work/student-work.component';
import { HttpClient, HttpEventType, HttpResponse } from '@angular/common/http';
import { NotificationService } from '../notifications/notification.service';
import { FormsModule } from '@angular/forms';
import { DetailsComponent } from './student-work/details/details.component';
import { interval, switchMap, takeWhile } from 'rxjs';

@Component({
  selector: 'app-workspace',
  standalone: true,
  imports: [StudentWorkComponent, FormsModule, DetailsComponent],
  templateUrl: './workspace.component.html',
  styleUrl: './workspace.component.css',
})
export class WorkspaceComponent implements OnInit {
  private notificationService = inject(NotificationService);
  private httpClient = inject(HttpClient);
  private studentInfoService = inject(StudentInfoService);
  private destroyRef = inject(DestroyRef);
  selectedFile = signal<StudentWork | undefined>(undefined);
  isModalOpen = signal<boolean>(false);
  searchReq = signal('');
  studentWorks = computed(() => this.studentInfoService.student()?.works);
  filteredWorks = computed(() => {
    if (this.searchReq() !== '') {
      return this.studentWorks()?.filter((w) =>
        w.fileName.includes(this.searchReq()),
      );
    } else {
      return this.studentWorks();
    }
  });

  ngOnInit(): void {
    this.studentInfoService.loadStudentInfo();
  }

  async onFIleSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;

    if (input.files?.length) {
      const selectedFile = input.files[0];
      const studentId = this.studentInfoService.student()?.id;
      const formData = new FormData();
      formData.append('file', selectedFile);

      const subscription = this.httpClient
        .post<StudentWork | { jobId: string; status: string; message: string }>(
          `http://localhost:5000/api/studentwork/upload/${studentId}`,
          formData,
          {
            observe: 'events',
          },
        )
        .subscribe({
          next: (event) => {
            if (event.type == HttpEventType.Response) {
              if (event.status === 202 || event.status === 201) {
                const resp = event as HttpResponse<{
                  jobId: string;
                  status: string;
                  message: string;
                }>;
                this.pollProcessingStatus(resp.body?.jobId!);
              } else if (event.status === 200) {
                this.notificationService.addSuccessNotification(
                  `File ${selectedFile.name} successfully uploaded`,
                );
              }
            }
          },
          error: (err) => {
            this.notificationService.addErrorNotification(
              `Error uploading file ${selectedFile.name}`,
            );
            console.log(err);
          },
        });

      this.destroyRef.onDestroy(() => subscription.unsubscribe());
    }
    return;
  }

  private pollProcessingStatus(jobId: string) {
    interval(1000)
      .pipe(
        switchMap(() =>
          this.httpClient.get<StudentWork>(
            `http://localhost:5000/api/studentwork/upload/status/${jobId}`,
            {
              observe: 'response',
            },
          ),
        ),
        takeWhile((response) => response.status === 202, true),
      )
      .subscribe({
        next: (resp) => {
          if (resp.status === 200) {
            if (resp?.body) this.studentWorks()?.push(resp?.body);
          }
        },
        error: (err) => {
          this.notificationService.addErrorNotification(
            'Error checking file status.',
          );
          console.log(err);
        },
      });
  }

  onFileDblClick(work: StudentWork) {
    this.selectedFile.set(work);
    this.isModalOpen.set(true);
  }

  onModalClosed() {
    this.isModalOpen.set(false);
  }
}
