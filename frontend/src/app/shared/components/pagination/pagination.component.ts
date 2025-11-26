import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface PageInfo {
  items: any[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss']
})
export class PaginationComponent {
  @Input() pageInfo: PageInfo | null = null;
  @Output() pageChange = new EventEmitter<number>();

  get pages(): number[] {
    if (!this.pageInfo) return [];
    
    const totalPages = this.pageInfo.totalPages;
    const currentPage = this.pageInfo.pageNumber;
    const pages: number[] = [];
    
    // Lógica para mostrar até 7 páginas
    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      if (currentPage <= 4) {
        // Início: 1 2 3 4 5 ... última
        for (let i = 1; i <= 5; i++) {
          pages.push(i);
        }
        pages.push(-1); // Indicador de "..."
        pages.push(totalPages);
      } else if (currentPage >= totalPages - 3) {
        // Final: 1 ... n-4 n-3 n-2 n-1 n
        pages.push(1);
        pages.push(-1);
        for (let i = totalPages - 4; i <= totalPages; i++) {
          pages.push(i);
        }
      } else {
        // Meio: 1 ... atual-1 atual atual+1 ... última
        pages.push(1);
        pages.push(-1);
        for (let i = currentPage - 1; i <= currentPage + 1; i++) {
          pages.push(i);
        }
        pages.push(-1);
        pages.push(totalPages);
      }
    }
    
    return pages;
  }

  onPageClick(page: number): void {
    if (page === -1 || !this.pageInfo) return;
    if (page === this.pageInfo.pageNumber) return;
    
    this.pageChange.emit(page);
  }

  onPrevious(): void {
    if (!this.pageInfo || !this.pageInfo.hasPreviousPage) return;
    this.pageChange.emit(this.pageInfo.pageNumber - 1);
  }

  onNext(): void {
    if (!this.pageInfo || !this.pageInfo.hasNextPage) return;
    this.pageChange.emit(this.pageInfo.pageNumber + 1);
  }

  getStartItem(): number {
    if (!this.pageInfo) return 0;
    return (this.pageInfo.pageNumber - 1) * this.pageInfo.pageSize + 1;
  }

  getEndItem(): number {
    if (!this.pageInfo) return 0;
    const end = this.pageInfo.pageNumber * this.pageInfo.pageSize;
    return Math.min(end, this.pageInfo.totalCount);
  }
}
