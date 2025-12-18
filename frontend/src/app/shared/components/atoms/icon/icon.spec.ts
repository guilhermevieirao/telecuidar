import { TestBed, ComponentFixture } from '@angular/core/testing';
import { IconComponent } from './icon';

describe('IconComponent', () => {
  let component: IconComponent;
  let fixture: ComponentFixture<IconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IconComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(IconComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default size as 24', () => {
    expect(component.size).toBe(24);
  });

  it('should accept icon name', () => {
    component.name = 'home';
    expect(component.name).toBe('home');
  });

  it('should return correct pixel size for lg', () => {
    component.size = 'lg';
    expect(component.pixelSize).toBe(32);
  });

  it('should accept custom numeric size', () => {
    component.size = 64;
    expect(component.pixelSize).toBe(64);
  });
});
