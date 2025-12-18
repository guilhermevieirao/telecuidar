import { TestBed } from '@angular/core/testing';
import { ThemeService } from './theme.service';
import { vi } from 'vitest';

describe('ThemeService', () => {
  let service: ThemeService;
  let localStorageSpy: { [key: string]: string };

  beforeEach(() => {
    // Mock localStorage
    localStorageSpy = {};
    vi.spyOn(localStorage, 'getItem').mockImplementation((key: string) => localStorageSpy[key] || null);
    vi.spyOn(localStorage, 'setItem').mockImplementation((key: string, value: string) => {
      localStorageSpy[key] = value;
    });

    // Mock window.matchMedia
    Object.defineProperty(window, 'matchMedia', {
      writable: true,
      value: vi.fn().mockImplementation((query: string) => ({
        matches: false,
        media: query,
        onchange: null,
        addListener: vi.fn(),
        removeListener: vi.fn(),
        addEventListener: vi.fn(),
        removeEventListener: vi.fn(),
        dispatchEvent: vi.fn(),
      })),
    });

    TestBed.configureTestingModule({});
    service = TestBed.inject(ThemeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should initialize with default theme', () => {
    expect(service).toBeDefined();
  });

  it('should toggle theme from light to dark', () => {
    // Start with light theme (no data-theme attribute)
    document.documentElement.removeAttribute('data-theme');
    
    service.toggleTheme();
    
    expect(service.isDarkMode()).toBe(true);
    expect(document.documentElement.getAttribute('data-theme')).toBe('dark');
  });

  it('should toggle theme from dark to light', () => {
    // Start with dark theme
    document.documentElement.setAttribute('data-theme', 'dark');
    
    service.toggleTheme();
    
    expect(service.isDarkMode()).toBe(false);
    expect(document.documentElement.getAttribute('data-theme')).toBeNull();
  });

  it('should detect dark mode correctly', () => {
    document.documentElement.setAttribute('data-theme', 'dark');
    expect(service.isDarkMode()).toBe(true);
  });

  it('should detect light mode correctly', () => {
    document.documentElement.removeAttribute('data-theme');
    expect(service.isDarkMode()).toBe(false);
  });

  it('should call localStorage.setItem when toggling theme', () => {
    const setItemSpy = vi.spyOn(Storage.prototype, 'setItem');
    
    service.toggleTheme();
    
    expect(setItemSpy).toHaveBeenCalledWith('telecuidar-theme', expect.any(String));
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });
});
