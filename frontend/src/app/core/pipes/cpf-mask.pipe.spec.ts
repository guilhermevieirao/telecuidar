import { CpfMaskPipe } from './cpf-mask.pipe';

describe('CpfMaskPipe', () => {
  let pipe: CpfMaskPipe;

  beforeEach(() => {
    pipe = new CpfMaskPipe();
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

  it('should format CPF with 3 digits', () => {
    expect(pipe.transform('123')).toBe('123');
  });

  it('should format CPF with 4 digits (first dot)', () => {
    expect(pipe.transform('1234')).toBe('123.4');
  });

  it('should format CPF with 6 digits', () => {
    expect(pipe.transform('123456')).toBe('123.456');
  });

  it('should format CPF with 7 digits (second dot)', () => {
    expect(pipe.transform('1234567')).toBe('123.456.7');
  });

  it('should format CPF with 9 digits', () => {
    expect(pipe.transform('123456789')).toBe('123.456.789');
  });

  it('should format complete CPF with 11 digits', () => {
    expect(pipe.transform('12345678900')).toBe('123.456.789-00');
  });

  it('should format already formatted CPF', () => {
    expect(pipe.transform('123.456.789-00')).toBe('123.456.789-00');
  });

  it('should remove non-numeric characters and format', () => {
    expect(pipe.transform('123abc456def789gh00')).toBe('123.456.789-00');
  });

  it('should handle CPF with extra digits (only use first 11)', () => {
    expect(pipe.transform('123456789001234')).toBe('123.456.789-00');
  });

  it('should format real CPF example', () => {
    expect(pipe.transform('11122233344')).toBe('111.222.333-44');
  });
});
