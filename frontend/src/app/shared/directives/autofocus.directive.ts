import { Directive, ElementRef, Input, OnInit, Renderer2 } from '@angular/core';

@Directive({
  selector: '[appAutofocus]',
  standalone: true
})
export class AutofocusDirective implements OnInit {
  @Input() appAutofocus = true;

  constructor(
    private el: ElementRef,
    private renderer: Renderer2
  ) {}

  ngOnInit(): void {
    if (this.appAutofocus) {
      setTimeout(() => {
        this.renderer.selectRootElement(this.el.nativeElement).focus();
      }, 100);
    }
  }
}
