// Detecta automaticamente se está em produção baseado na URL
const isProduction = typeof window !== 'undefined' && 
  (window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1');

export const environment = {
  production: isProduction,
  apiUrl: isProduction ? '/api' : 'http://localhost:5058/api',
  apiTimeout: 30000,
  enableDebugMode: !isProduction
};
