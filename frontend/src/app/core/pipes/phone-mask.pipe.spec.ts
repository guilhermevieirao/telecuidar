import { PhoneMaskPipe } from './phone-mask.pipe';

describe('PhoneMaskPipe', () => {
  let pipe: PhoneMaskPipe;

  beforeEach(() => {
    pipe = new PhoneMaskPipe();
  });

  it('should create an instance', () => {
    expect(pipe).toBeTruthy();
  });

  it('should return empty string for empty input', () => {
    expect(pipe.transform('')).toBe('');
  });

  it('should return empty string for null input', () => {
    expect(pipe.transform(null as any)).toBe('');
  });

  it('should format phone with 2 digits (DDD only)', () => {
    expect(pipe.transform('11')).toBe('11');
  });

  it('should format phone with 3 digits (DDD + 1)', () => {
    expect(pipe.transform('119')).toBe('(11) 9');
  });

  it('should format phone with 6 digits', () => {
    expect(pipe.transform('119876')).toBe('(11) 9876');
  });

  it('should format phone with 7 digits (landline start)', () => {
    expect(pipe.transform('1198765')).toBe('(11) 9876-5');
  });

  it('should format phone with 10 digits (landline complete)', () => {
    expect(pipe.transform('1198765432')).toBe('(11) 9876-5432');
  });

  it('should format phone with 11 digits (mobile complete)', () => {
    expect(pipe.transform('11987654321')).toBe('(11) 98765-4321');
  });

  it('should format already formatted phone', () => {
    expect(pipe.transform('(11) 98765-4321')).toBe('(11) 98765-4321');
  });

  it('should remove non-numeric characters and format', () => {
    expect(pipe.transform('(11) abc 98765-defg4321')).toBe('(11) 98765-4321');
  });

  it('should handle phone with extra digits (only use first 11)', () => {
    expect(pipe.transform('119876543219999')).toBe('(11) 98765-4321');
  });

  it('should format landline example', () => {
    expect(pipe.transform('1132345678')).toBe('(11) 3234-5678');
  });

  it('should format mobile example', () => {
    expect(pipe.transform('21987654321')).toBe('(21) 98765-4321');
  });
});
