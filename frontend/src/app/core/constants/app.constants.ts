// API URLs
export const API_ENDPOINTS = {
  USERS: 'users',
  AUTH: 'auth',
  PATIENTS: 'patients',
  DOCTORS: 'doctors',
  APPOINTMENTS: 'appointments'
} as const;

// App Constants
export const APP_CONSTANTS = {
  APP_NAME: 'TeleCuidar',
  VERSION: '1.0.0',
  DEFAULT_PAGE_SIZE: 10,
  MAX_FILE_SIZE: 5242880, // 5MB
  ALLOWED_FILE_TYPES: ['image/jpeg', 'image/png', 'application/pdf']
} as const;

// Local Storage Keys
export const STORAGE_KEYS = {
  TOKEN: 'telecuidar_token',
  USER: 'telecuidar_user',
  REFRESH_TOKEN: 'telecuidar_refresh_token'
} as const;

// Validation
export const VALIDATION = {
  MIN_PASSWORD_LENGTH: 8,
  MAX_PASSWORD_LENGTH: 50,
  EMAIL_PATTERN: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
} as const;
