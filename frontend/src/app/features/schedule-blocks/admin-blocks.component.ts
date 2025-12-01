import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ScheduleBlocksService, ScheduleBlock } from './schedule-blocks.service';

@Component({
  selector: 'app-admin-blocks',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './admin-blocks.component.html',
  styleUrls: ['./admin-blocks.component.scss']
})
export class AdminBlocksComponent implements OnInit {
  blocks: ScheduleBlock[] = [];
  loading = false;
  selectedBlock: ScheduleBlock | null = null;
  action: 'accept' | 'reject' | null = null;
  justification = '';
  error = '';

  constructor(private blocksService: ScheduleBlocksService) {}

  ngOnInit(): void {
    this.fetchBlocks();
  }

  fetchBlocks() {
    this.loading = true;
    this.blocksService.getAllBlocks().subscribe({
      next: (blocks) => {
        this.blocks = blocks;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  openAction(block: ScheduleBlock, action: 'accept' | 'reject') {
    this.selectedBlock = block;
    this.action = action;
    this.justification = '';
    this.error = '';
  }

  closeAction() {
    this.selectedBlock = null;
    this.action = null;
    this.justification = '';
    this.error = '';
  }

  submitAction() {
    if (!this.selectedBlock || !this.action) return;
    if (!this.justification) {
      this.error = 'Justificativa obrigatória.';
      return;
    }
    const fn = this.action === 'accept' ? this.blocksService.acceptBlock : this.blocksService.rejectBlock;
    fn.call(this.blocksService, this.selectedBlock.id, this.justification).subscribe({
      next: () => {
        this.closeAction();
        this.fetchBlocks();
      },
      error: (err) => {
        this.error = err?.error?.message || 'Erro ao processar ação.';
      }
    });
  }

  getStatusLabel(block: ScheduleBlock): string {
    const statusMap: Record<string | number, string> = {
      0: 'Pendente',
      1: 'Aceita',
      2: 'Recusada',
      3: 'Passada',
      'Pending': 'Pendente',
      'Accepted': 'Aceita',
      'Rejected': 'Recusada',
      'Expired': 'Passada',
      'Pendente': 'Pendente',
      'Aceita': 'Aceita',
      'Recusada': 'Recusada',
      'Passada': 'Passada',
    };
    return statusMap[block.status] || block.status;
  }
}
