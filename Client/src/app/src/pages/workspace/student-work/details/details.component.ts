import {
  AfterViewInit,
  Component,
  computed,
  effect,
  ElementRef,
  inject,
  input,
  OnInit,
  output,
  viewChild,
} from '@angular/core';
import { StudentWork } from '../../studentWork.model';
import { DetailsService } from './details.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-details',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './details.component.html',
  styleUrl: './details.component.css',
})
export class DetailsComponent implements OnInit, AfterViewInit {
  inputFileInfo = input<StudentWork>();
  private fileDetailsService = inject(DetailsService);
  close = output();

  dialog = viewChild<ElementRef<HTMLDialogElement>>('dialog');
  chart = viewChild.required<ElementRef<HTMLCanvasElement>>('chart');

  ctx: CanvasRenderingContext2D | null = null;
  percentage: number = -1;
  fileInfo: StudentWork | undefined;

  plagiarismStats = computed(() =>
    this.fileDetailsService.loadedPlagiarismStats(),
  );

  author = computed(() => this.fileDetailsService.loadedAuthor());

  get content() {
    const content = this.fileInfo?.content;
    if (content) {
      return atob(content);
    } else return 'Content not found';
  }

  ngOnInit(): void {
    this.fileInfo = this.inputFileInfo();
    this.dialog()?.nativeElement.showModal();
    const fileId = this.inputFileInfo()?.id;
    if (fileId) this.fileDetailsService.collectStats(fileId);
  }

  constructor() {
    effect(() => {
      this.percentage = this.plagiarismStats()[0]?.similarityPercentage;
      if (this.ctx && this.percentage !== -1)
        this.fileDetailsService.setPercentChart(this.ctx, this.percentage);
    });

    effect(() => {
      this.fileInfo = this.fileDetailsService.loadedFileInfo();
    });
  }

  ngAfterViewInit(): void {
    this.ctx = this.chart().nativeElement.getContext('2d');
  }

  onBackdropClick(event: Event): void {
    const backdrop = this.dialog()?.nativeElement;
    if (event.target === backdrop) backdrop.close();
  }

  onXClick(): void {
    this.dialog()?.nativeElement.close();
  }

  onSimilarFileClick(fileId: string): void {
    this.fileDetailsService.collectStats(fileId);
  }
}
