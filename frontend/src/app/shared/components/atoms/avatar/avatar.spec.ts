import { TestBed, ComponentFixture } from '@angular/core/testing';
import { AvatarComponent } from './avatar';

describe('AvatarComponent', () => {
  let component: AvatarComponent;
  let fixture: ComponentFixture<AvatarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AvatarComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AvatarComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default size as md', () => {
    expect(component.size).toBe('md');
  });

  it('should generate initials from single name', () => {
    component.name = 'João';
    expect(component.initials).toBe('JO');
  });

  it('should generate initials from full name', () => {
    component.name = 'João Silva';
    expect(component.initials).toBe('JS');
  });

  it('should generate initials from multiple names', () => {
    component.name = 'João Pedro Silva';
    expect(component.initials).toBe('JS');
  });

  it('should return empty string for empty name', () => {
    component.name = '';
    expect(component.initials).toBe('');
  });

  it('should handle name with extra spaces', () => {
    component.name = '  João   Silva  ';
    expect(component.initials).toBe('JS');
  });

  it('should return correct size class', () => {
    component.size = 'lg';
    expect(component.sizeClass).toBe('avatar--lg');
  });
});
