import { Component, EventEmitter, Input, Output, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';

interface CropArea {
  x: number;
  y: number;
  width: number;
  height: number;
}

@Component({
  selector: 'app-image-crop-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './image-crop-modal.component.html',
  styleUrls: ['./image-crop-modal.component.scss']
})
export class ImageCropModalComponent implements OnInit, AfterViewInit {
  @Input() imageSource: string = '';
  @Output() close = new EventEmitter<void>();
  @Output() cropComplete = new EventEmitter<string>();

  @ViewChild('mainCanvas', { static: false }) mainCanvas!: ElementRef<HTMLCanvasElement>;
  @ViewChild('previewCanvas', { static: false }) previewCanvas!: ElementRef<HTMLCanvasElement>;

  private ctx!: CanvasRenderingContext2D;
  private previewCtx!: CanvasRenderingContext2D;
  private image: HTMLImageElement = new Image();
  
  cropArea: CropArea = { x: 0, y: 0, width: 200, height: 200 };
  isDragging = false;
  isResizing = false;
  dragStart = { x: 0, y: 0 };
  resizeHandle: string = '';
  
  imageLoaded = false;
  scale = 1;
  maxCanvasWidth = 600;
  maxCanvasHeight = 600;

  ngOnInit(): void {
    this.image.src = this.imageSource;
    this.image.onload = () => {
      this.imageLoaded = true;
      this.initializeCanvas();
    };
  }

  ngAfterViewInit(): void {
    if (this.imageLoaded) {
      this.initializeCanvas();
    }
  }

  initializeCanvas(): void {
    if (!this.mainCanvas || !this.previewCanvas) return;

    const canvas = this.mainCanvas.nativeElement;
    const previewCanvas = this.previewCanvas.nativeElement;
    
    this.ctx = canvas.getContext('2d')!;
    this.previewCtx = previewCanvas.getContext('2d')!;

    // Calculate scale to fit image in canvas
    const scaleX = this.maxCanvasWidth / this.image.width;
    const scaleY = this.maxCanvasHeight / this.image.height;
    this.scale = Math.min(scaleX, scaleY, 1);

    canvas.width = this.image.width * this.scale;
    canvas.height = this.image.height * this.scale;

    // Initialize crop area in center
    const cropSize = Math.min(canvas.width, canvas.height) * 0.6;
    this.cropArea = {
      x: (canvas.width - cropSize) / 2,
      y: (canvas.height - cropSize) / 2,
      width: cropSize,
      height: cropSize
    };

    this.draw();
    this.updatePreview();
  }

  draw(): void {
    if (!this.ctx || !this.imageLoaded) return;

    const canvas = this.mainCanvas.nativeElement;
    
    // Clear canvas
    this.ctx.clearRect(0, 0, canvas.width, canvas.height);
    
    // Draw image
    this.ctx.drawImage(this.image, 0, 0, canvas.width, canvas.height);
    
    // Draw overlay (darken area outside crop)
    this.ctx.fillStyle = 'rgba(0, 0, 0, 0.5)';
    this.ctx.fillRect(0, 0, canvas.width, canvas.height);
    
    // Clear crop area
    this.ctx.clearRect(
      this.cropArea.x,
      this.cropArea.y,
      this.cropArea.width,
      this.cropArea.height
    );
    
    // Redraw image in crop area
    this.ctx.drawImage(
      this.image,
      this.cropArea.x / this.scale,
      this.cropArea.y / this.scale,
      this.cropArea.width / this.scale,
      this.cropArea.height / this.scale,
      this.cropArea.x,
      this.cropArea.y,
      this.cropArea.width,
      this.cropArea.height
    );
    
    // Draw crop border
    this.ctx.strokeStyle = '#2563eb';
    this.ctx.lineWidth = 2;
    this.ctx.strokeRect(
      this.cropArea.x,
      this.cropArea.y,
      this.cropArea.width,
      this.cropArea.height
    );
    
    // Draw resize handles
    this.drawResizeHandles();
  }

  drawResizeHandles(): void {
    const handleSize = 12;
    const handles = [
      { x: this.cropArea.x, y: this.cropArea.y, cursor: 'nw-resize' },
      { x: this.cropArea.x + this.cropArea.width, y: this.cropArea.y, cursor: 'ne-resize' },
      { x: this.cropArea.x, y: this.cropArea.y + this.cropArea.height, cursor: 'sw-resize' },
      { x: this.cropArea.x + this.cropArea.width, y: this.cropArea.y + this.cropArea.height, cursor: 'se-resize' }
    ];

    handles.forEach(handle => {
      this.ctx.fillStyle = '#2563eb';
      this.ctx.fillRect(
        handle.x - handleSize / 2,
        handle.y - handleSize / 2,
        handleSize,
        handleSize
      );
      this.ctx.strokeStyle = 'white';
      this.ctx.lineWidth = 2;
      this.ctx.strokeRect(
        handle.x - handleSize / 2,
        handle.y - handleSize / 2,
        handleSize,
        handleSize
      );
    });
  }

  updatePreview(): void {
    if (!this.previewCtx || !this.imageLoaded) return;

    const previewSize = 150;
    const previewCanvas = this.previewCanvas.nativeElement;
    previewCanvas.width = previewSize;
    previewCanvas.height = previewSize;

    // Draw cropped area in preview
    this.previewCtx.drawImage(
      this.image,
      this.cropArea.x / this.scale,
      this.cropArea.y / this.scale,
      this.cropArea.width / this.scale,
      this.cropArea.height / this.scale,
      0,
      0,
      previewSize,
      previewSize
    );
  }

  onMouseDown(event: MouseEvent): void {
    const canvas = this.mainCanvas.nativeElement;
    const rect = canvas.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;

    // Check if clicking on resize handle
    const handleSize = 12;
    const handles = [
      { x: this.cropArea.x, y: this.cropArea.y, name: 'nw' },
      { x: this.cropArea.x + this.cropArea.width, y: this.cropArea.y, name: 'ne' },
      { x: this.cropArea.x, y: this.cropArea.y + this.cropArea.height, name: 'sw' },
      { x: this.cropArea.x + this.cropArea.width, y: this.cropArea.y + this.cropArea.height, name: 'se' }
    ];

    for (const handle of handles) {
      if (
        x >= handle.x - handleSize &&
        x <= handle.x + handleSize &&
        y >= handle.y - handleSize &&
        y <= handle.y + handleSize
      ) {
        this.isResizing = true;
        this.resizeHandle = handle.name;
        this.dragStart = { x, y };
        return;
      }
    }

    // Check if clicking inside crop area
    if (
      x >= this.cropArea.x &&
      x <= this.cropArea.x + this.cropArea.width &&
      y >= this.cropArea.y &&
      y <= this.cropArea.y + this.cropArea.height
    ) {
      this.isDragging = true;
      this.dragStart = { x: x - this.cropArea.x, y: y - this.cropArea.y };
    }
  }

  onMouseMove(event: MouseEvent): void {
    if (!this.isDragging && !this.isResizing) return;

    const canvas = this.mainCanvas.nativeElement;
    const rect = canvas.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;

    if (this.isDragging) {
      this.cropArea.x = Math.max(0, Math.min(x - this.dragStart.x, canvas.width - this.cropArea.width));
      this.cropArea.y = Math.max(0, Math.min(y - this.dragStart.y, canvas.height - this.cropArea.height));
    } else if (this.isResizing) {
      this.handleResize(x, y);
    }

    this.draw();
    this.updatePreview();
  }

  onMouseUp(): void {
    this.isDragging = false;
    this.isResizing = false;
    this.resizeHandle = '';
  }

  handleResize(x: number, y: number): void {
    const canvas = this.mainCanvas.nativeElement;
    const minSize = 50;

    switch (this.resizeHandle) {
      case 'nw':
        const newWidthNW = this.cropArea.x + this.cropArea.width - x;
        const newHeightNW = this.cropArea.y + this.cropArea.height - y;
        if (newWidthNW >= minSize && x >= 0) {
          this.cropArea.width = newWidthNW;
          this.cropArea.x = x;
        }
        if (newHeightNW >= minSize && y >= 0) {
          this.cropArea.height = newHeightNW;
          this.cropArea.y = y;
        }
        break;
      case 'ne':
        const newWidthNE = x - this.cropArea.x;
        const newHeightNE = this.cropArea.y + this.cropArea.height - y;
        if (newWidthNE >= minSize && x <= canvas.width) {
          this.cropArea.width = newWidthNE;
        }
        if (newHeightNE >= minSize && y >= 0) {
          this.cropArea.height = newHeightNE;
          this.cropArea.y = y;
        }
        break;
      case 'sw':
        const newWidthSW = this.cropArea.x + this.cropArea.width - x;
        const newHeightSW = y - this.cropArea.y;
        if (newWidthSW >= minSize && x >= 0) {
          this.cropArea.width = newWidthSW;
          this.cropArea.x = x;
        }
        if (newHeightSW >= minSize && y <= canvas.height) {
          this.cropArea.height = newHeightSW;
        }
        break;
      case 'se':
        const newWidthSE = x - this.cropArea.x;
        const newHeightSE = y - this.cropArea.y;
        if (newWidthSE >= minSize && x <= canvas.width) {
          this.cropArea.width = newWidthSE;
        }
        if (newHeightSE >= minSize && y <= canvas.height) {
          this.cropArea.height = newHeightSE;
        }
        break;
    }
  }

  onConfirm(): void {
    // Create final cropped image
    const cropCanvas = document.createElement('canvas');
    const cropCtx = cropCanvas.getContext('2d')!;

    // Set canvas size to crop area
    cropCanvas.width = this.cropArea.width / this.scale;
    cropCanvas.height = this.cropArea.height / this.scale;

    // Draw cropped portion
    cropCtx.drawImage(
      this.image,
      this.cropArea.x / this.scale,
      this.cropArea.y / this.scale,
      this.cropArea.width / this.scale,
      this.cropArea.height / this.scale,
      0,
      0,
      cropCanvas.width,
      cropCanvas.height
    );

    // Convert to base64
    const croppedImage = cropCanvas.toDataURL('image/jpeg', 0.9);
    this.cropComplete.emit(croppedImage);
  }

  onCancel(): void {
    this.close.emit();
  }
}
