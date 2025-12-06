const isBrowser = typeof window !== 'undefined';
const host = isBrowser ? window.location.hostname : 'telecuidar.com.br';
const isLocal = host === 'localhost' || host === '127.0.0.1';
const jitsiDomain = isLocal ? 'localhost:8443' : host;
const jitsiExternalApiUrl = isLocal 
  ? 'https://localhost:8443/external_api.js' 
  : `https://${jitsiDomain}/external_api.js`;

export const environment = {
  production: true,
  apiUrl: '/api',
  apiTimeout: 30000,
  enableDebugMode: false,
  jitsiDomain,
  jitsiExternalApiUrl
};
