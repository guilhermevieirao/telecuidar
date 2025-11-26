import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpEventType } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent, PageInfo } from '../../shared/components/pagination/pagination.component';
import { ThemeToggleComponent } from '../../shared/components/theme-toggle/theme-toggle.component';
import { environment } from '../../../environments/environment';

interface FileUpload {
  id: number;
  fileName: string;
  originalFileName: string;
  contentType: string;
  fileSize: number;
  fileSizeFormatted: string;
  fileCategory: string;
  description?: string;
  isPublic: boolean;
  uploadedByUserId: number;
  uploadedByUserName: string;
  relatedUserId?: number;
  relatedUserName?: string;
  createdAt: string;
  downloadUrl: string;
}

@Component({
  selector: 'app-files',
  standalone: true,
  imports: [CommonModule, FormsModule, PaginationComponent, ThemeToggleComponent],
  templateUrl: './files.component.html',
  styleUrls: ['./files.component.scss']
})
export class FilesComponent implements OnInit {
  files: FileUpload[] = [];
  pageInfo: PageInfo | null = null;
  currentPage = 1;
  pageSize = 12;
  
  selectedCategory: string | null = null;
  categories = ['Document', 'Image', 'Medical', 'Other'];
  
  uploading = false;
  uploadProgress = 0;
  selectedFile: File | null = null;
  fileCategory = 'Document';
  fileDescription = '';
  isPublic = false;
  
  showUploadModal = false;

  constructor(
    private http: HttpClient,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.loadFiles();
  }

  loadFiles(): void {
    const params = new URLSearchParams({
      pageNumber: this.currentPage.toString(),
      pageSize: this.pageSize.toString()
    });

    if (this.selectedCategory) {
      params.append('fileCategory', this.selectedCategory);
    }

    this.http.get<any>(`${environment.apiUrl}/files/my-files?${params.toString()}`)
      .subscribe({
        next: (response) => {
          if (response.isSuccess) {
            const pagedResult = response.data;
            this.files = pagedResult.items;
            this.pageInfo = {
              items: pagedResult.items,
              pageNumber: pagedResult.pageNumber,
              pageSize: pagedResult.pageSize,
              totalCount: pagedResult.totalCount,
              totalPages: pagedResult.totalPages,
              hasPreviousPage: pagedResult.hasPreviousPage,
              hasNextPage: pagedResult.hasNextPage
            };
          }
        },
        error: (error) => {
          console.error('Erro ao carregar arquivos:', error);
          this.toastService.error('Erro ao carregar arquivos');
        }
      });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  uploadFile(): void {
    if (!this.selectedFile) {
      this.toastService.error('Selecione um arquivo');
      return;
    }

    const formData = new FormData();
    formData.append('uploadedFile', this.selectedFile);
    formData.append('fileCategory', this.fileCategory);
    formData.append('description', this.fileDescription);
    formData.append('isPublic', this.isPublic.toString());

    this.uploading = true;
    this.uploadProgress = 0;

    this.http.post<any>(`${environment.apiUrl}/files/upload`, formData, {
      reportProgress: true,
      observe: 'events'
    }).subscribe({
      next: (event) => {
        if (event.type === HttpEventType.UploadProgress) {
          if (event.total) {
            this.uploadProgress = Math.round((100 * event.loaded) / event.total);
          }
        } else if (event.type === HttpEventType.Response) {
          if (event.body?.isSuccess) {
            this.toastService.success('Arquivo enviado com sucesso!');
            this.closeUploadModal();
            this.loadFiles();
          }
        }
      },
      error: (error) => {
        console.error('Erro ao fazer upload:', error);
        this.toastService.error(error.error?.message || 'Erro ao fazer upload do arquivo');
        this.uploading = false;
      },
      complete: () => {
        this.uploading = false;
      }
    });
  }

  downloadFile(file: FileUpload): void {
    this.http.get(`${environment.apiUrl}/files/${file.id}/download`, {
      responseType: 'blob'
    }).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = file.originalFileName;
        link.click();
        window.URL.revokeObjectURL(url);
        this.toastService.success('Download iniciado');
      },
      error: (error) => {
        console.error('Erro ao baixar arquivo:', error);
        this.toastService.error('Erro ao baixar arquivo');
      }
    });
  }

  deleteFile(file: FileUpload): void {
    if (!confirm(`Tem certeza que deseja excluir o arquivo "${file.originalFileName}"?`)) {
      return;
    }

    this.http.delete<any>(`${environment.apiUrl}/files/${file.id}`)
      .subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.toastService.success('Arquivo excluído com sucesso');
            this.loadFiles();
          }
        },
        error: (error) => {
          console.error('Erro ao excluir arquivo:', error);
          this.toastService.error(error.error?.message || 'Erro ao excluir arquivo');
        }
      });
  }

  filterByCategory(category: string | null): void {
    this.selectedCategory = category;
    this.currentPage = 1;
    this.loadFiles();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadFiles();
  }

  openUploadModal(): void {
    this.showUploadModal = true;
    this.selectedFile = null;
    this.fileCategory = 'Document';
    this.fileDescription = '';
    this.isPublic = false;
    this.uploadProgress = 0;
  }

  closeUploadModal(): void {
    this.showUploadModal = false;
    this.selectedFile = null;
    this.uploading = false;
  }

  getFileIcon(contentType: string): string {
    if (contentType.startsWith('image/')) return '🖼️';
    if (contentType.includes('pdf')) return '📄';
    if (contentType.includes('word') || contentType.includes('doc')) return '📝';
    if (contentType.includes('excel') || contentType.includes('spreadsheet')) return '📊';
    if (contentType.includes('text')) return '📃';
    return '📎';
  }

  getCategoryBadgeClass(category: string): string {
    switch (category) {
      case 'Document': return 'bg-blue-100 text-blue-800';
      case 'Image': return 'bg-green-100 text-green-800';
      case 'Medical': return 'bg-red-100 text-red-800';
      case 'Other': return 'bg-gray-100 text-gray-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }

  getCategoryName(category: string): string {
    switch (category) {
      case 'Document': return 'Documento';
      case 'Image': return 'Imagem';
      case 'Medical': return 'Médico';
      case 'Other': return 'Outro';
      default: return category;
    }
  }
}
