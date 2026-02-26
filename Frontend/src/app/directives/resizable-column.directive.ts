import { Directive, ElementRef, OnInit, Renderer2 } from '@angular/core';

@Directive({
  selector: '[appResizableColumn]',
  standalone: true
})
export class ResizableColumnDirective implements OnInit {
  private startX = 0;
  private startWidth = 0;
  private column: HTMLElement;
  private resizer!: HTMLElement;
  private isResizing = false;

  constructor(
    private el: ElementRef,
    private renderer: Renderer2
  ) {
    this.column = this.el.nativeElement;
  }

  ngOnInit(): void {
    // Create resizer element
    this.resizer = this.renderer.createElement('div');
    this.renderer.addClass(this.resizer, 'resizer-handle');
    this.renderer.appendChild(this.column, this.resizer);

    // Add event listeners
    this.renderer.listen(this.resizer, 'mousedown', this.onMouseDown.bind(this));
  }

  private onMouseDown(event: MouseEvent): void {
    event.preventDefault();
    event.stopPropagation();
    
    this.isResizing = true;
    this.startX = event.pageX;
    this.startWidth = this.column.offsetWidth;

    // Add document listeners
    const mouseMoveListener = this.renderer.listen('document', 'mousemove', this.onMouseMove.bind(this));
    const mouseUpListener = this.renderer.listen('document', 'mouseup', () => {
      this.isResizing = false;
      this.renderer.setStyle(document.body, 'cursor', '');
      this.renderer.setStyle(document.body, 'user-select', '');
      mouseMoveListener();
      mouseUpListener();
    });

    this.renderer.setStyle(document.body, 'cursor', 'col-resize');
    this.renderer.setStyle(document.body, 'user-select', 'none');
  }

  private onMouseMove(event: MouseEvent): void {
    if (!this.isResizing) return;

    const width = this.startWidth + (event.pageX - this.startX);
    if (width >= 50) {
      this.renderer.setStyle(this.column, 'width', `${width}px`);
      this.renderer.setStyle(this.column, 'min-width', `${width}px`);
      this.renderer.setStyle(this.column, 'max-width', `${width}px`);
    }
  }
}
