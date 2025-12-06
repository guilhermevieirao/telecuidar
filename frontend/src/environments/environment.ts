// Detecta automaticamente se está em produção baseado na URL
const isProduction = typeof window !== 'undefined' && 
  (window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1');

export const environment = {
  production: isProduction,
  apiUrl: isProduction ? '/api' : 'http://localhost:5058/api',
  apiTimeout: 30000,
  enableDebugMode: !isProduction,
  jitsiDomain: isProduction ? 'meet.telecuidar.com.br' : 'localhost:8000',
  jitsiExternalApiUrl: isProduction ? 'https://meet.telecuidar.com.br/external_api.js' : 'http://localhost:8000/external_api.js'
};
