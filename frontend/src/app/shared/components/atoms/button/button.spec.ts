import { TestBed, ComponentFixture } from '@angular/core/testing';
import { ButtonComponent } from './button';

describe('ButtonComponent', () => {
  let component: ButtonComponent;
  let fixture: ComponentFixture<ButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ButtonComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ButtonComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default variant as primary', () => {
    expect(component.variant).toBe('primary');
  });

  it('should have default size as md', () => {
    expect(component.size).toBe('md');
  });

  it('should not be disabled by default', () => {
    expect(component.disabled).toBe(false);
  });

  it('should not be full width by default', () => {
    expect(component.fullWidth).toBe(false);
  });

  it('should not be loading by default', () => {
    expect(component.loading).toBe(false);
  });

  it('should have default type as button', () => {
    expect(component.type).toBe('button');
  });

  it('should accept variant input', () => {
    component.variant = 'danger';
    expect(component.variant).toBe('danger');
  });

  it('should accept size input', () => {
    component.size = 'lg';
    expect(component.size).toBe('lg');
  });

  it('should accept disabled input', () => {
    component.disabled = true;
    expect(component.disabled).toBe(true);
  });
});
