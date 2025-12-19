// Dynamically determine API URL based on current host
const getApiUrl = () => {
  if (typeof window !== 'undefined') {
    const host = window.location.hostname;
    // If accessing via IP or non-localhost, use that same host for API
    if (host !== 'localhost' && host !== '127.0.0.1') {
      return `http://${host}:5239/api`;
    }
  }
  return 'http://localhost:5239/api';
};

export const environment = {
  production: false,
  apiUrl: (typeof process !== 'undefined' && process.env?.['API_URL']) 
    ? process.env['API_URL']
    : getApiUrl(),
  apiUrlHttp: (typeof process !== 'undefined' && process.env?.['API_URL_HTTP']) 
    ? process.env['API_URL_HTTP']
    : getApiUrl(),
  jitsiDomain: (typeof process !== 'undefined' && process.env?.['JITSI_DOMAIN']) 
    ? process.env['JITSI_DOMAIN']
    : 'meet.jit.si',
  jitsiEnabled: (typeof process !== 'undefined' && process.env?.['JITSI_ENABLED']) 
    ? process.env['JITSI_ENABLED'] === 'true'
    : false,
};
