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
import { HttpClient, HttpEventType } from '@angular/common/http';
import { NotificationService } from '../notifications/notification.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-workspace',
  standalone: true,
  imports: [StudentWorkComponent, FormsModule],
  templateUrl: './workspace.component.html',
  styleUrl: './workspace.component.css',
})
export class WorkspaceComponent implements OnInit {
  private notificationService = inject(NotificationService);
  private httpClient = inject(HttpClient);
  studentInfoService = inject(StudentInfoService);
  private destroyRef = inject(DestroyRef);
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
        .post<StudentWork>(
          `http://localhost:5000/api/studentwork/upload/${studentId}`,
          formData,
          {
            observe: 'events',
          },
        )
        .subscribe({
          next: (resp) => {
            if (resp.type == HttpEventType.Response) {
              this.notificationService.addSuccessNotification(
                `File ${selectedFile.name} successfully uploaded`,
              );
              if (resp?.body) this.studentWorks()?.push(resp?.body);
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
}
