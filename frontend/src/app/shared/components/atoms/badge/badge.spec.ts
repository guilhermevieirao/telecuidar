import { TestBed, ComponentFixture } from '@angular/core/testing';
import { BadgeComponent } from './badge';

describe('BadgeComponent', () => {
  let component: BadgeComponent;
  let fixture: ComponentFixture<BadgeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BadgeComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(BadgeComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default variant as neutral', () => {
    expect(component.variant).toBe('neutral');
  });

  it('should have default size as md', () => {
    expect(component.size).toBe('md');
  });

  it('should have empty label by default', () => {
    expect(component.label).toBe('');
  });

  it('should return correct variant class', () => {
    component.variant = 'success';
    expect(component.variantClass).toBe('badge--success');
  });

  it('should return correct size class', () => {
    component.size = 'lg';
    expect(component.sizeClass).toBe('badge--lg');
  });

  it('should accept label input', () => {
    component.label = 'Test Label';
    expect(component.label).toBe('Test Label');
  });
});
