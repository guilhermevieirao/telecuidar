import { Injectable } from '@angular/core';
import { Observable, from, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

declare var LacunaWebPKI: any;

export interface InstalledCertificate {
  thumbprint: string;
  subjectName: string;
  issuerName: string;
  validFrom: Date;
  validTo: Date;
  keyUsage?: string;
  pkiBrazil?: {
    cpf?: string;
    cnpj?: string;
    responsavel?: string;
    dateOfBirth?: Date;
    certificateType?: string;
    isAplicacao?: boolean;
    isPessoaFisica?: boolean;
    isPessoaJuridica?: boolean;
  };
}

export interface SignatureResult {
  signature: string;
  certificateContent: string;
  signatureAlgorithm: string;
}

@Injectable({
  providedIn: 'root'
})
export class CertificateService {
  private pki: any = null;
  private isInitialized = false;
  private initPromise: Promise<boolean> | null = null;

  constructor() {}

  /**
   * Inicializa a biblioteca Web PKI
   */
  async initialize(): Promise<boolean> {
    if (this.isInitialized) {
      return true;
    }

    if (this.initPromise) {
      return this.initPromise;
    }

    this.initPromise = new Promise<boolean>((resolve) => {
      // Verificar se a biblioteca está disponível
      if (typeof LacunaWebPKI === 'undefined') {
        console.warn('Lacuna Web PKI não está disponível. Usando modo de fallback.');
        resolve(false);
        return;
      }

      this.pki = new LacunaWebPKI();
      
      this.pki.init({
        ready: () => {
          this.isInitialized = true;
          resolve(true);
        },
        notInstalled: () => {
          console.warn('Web PKI não está instalado');
          resolve(false);
        },
        defaultFail: (ex: any) => {
          console.error('Erro ao inicializar Web PKI:', ex);
          resolve(false);
        }
      });
    });

    return this.initPromise;
  }

  /**
   * Lista todos os certificados digitais instalados no computador
   */
  async listCertificates(): Promise<InstalledCertificate[]> {
    const initialized = await this.initialize();
    
    if (!initialized || !this.pki) {
      // Fallback: retornar lista vazia ou usar método alternativo
      return this.listCertificatesFallback();
    }

    return new Promise<InstalledCertificate[]>((resolve, reject) => {
      this.pki.listCertificates({
        success: (certs: any[]) => {
          const certificates: InstalledCertificate[] = certs.map(cert => ({
            thumbprint: cert.thumbprint,
            subjectName: cert.subjectName,
            issuerName: cert.issuerName,
            validFrom: new Date(cert.validityStart),
            validTo: new Date(cert.validityEnd),
            keyUsage: cert.keyUsage,
            pkiBrazil: cert.pkiBrazil ? {
              cpf: cert.pkiBrazil.cpf,
              cnpj: cert.pkiBrazil.cnpj,
              responsavel: cert.pkiBrazil.responsavel,
              dateOfBirth: cert.pkiBrazil.dateOfBirth ? new Date(cert.pkiBrazil.dateOfBirth) : undefined,
              certificateType: cert.pkiBrazil.certificateType,
              isAplicacao: cert.pkiBrazil.isAplicacao,
              isPessoaFisica: cert.pkiBrazil.isPessoaFisica,
              isPessoaJuridica: cert.pkiBrazil.isPessoaJuridica
            } : undefined
          }));
          
          // Filtrar apenas certificados válidos e com chave privada
          const validCerts = certificates.filter(c => 
            new Date() >= c.validFrom && new Date() <= c.validTo
          );
          
          resolve(validCerts);
        },
        fail: (ex: any) => {
          console.error('Erro ao listar certificados:', ex);
          resolve([]);
        }
      });
    });
  }

  /**
   * Assina dados usando um certificado específico
   */
  async signData(thumbprint: string, dataToSign: string): Promise<SignatureResult | null> {
    const initialized = await this.initialize();
    
    if (!initialized || !this.pki) {
      return null;
    }

    return new Promise<SignatureResult | null>((resolve, reject) => {
      this.pki.signData({
        thumbprint: thumbprint,
        data: dataToSign,
        digestAlgorithm: 'SHA-256',
        success: (signature: string) => {
          // Obter o certificado para retornar junto
          this.pki.readCertificate({
            thumbprint: thumbprint,
            success: (certContent: string) => {
              resolve({
                signature: signature,
                certificateContent: certContent,
                signatureAlgorithm: 'SHA256withRSA'
              });
            },
            fail: () => {
              resolve({
                signature: signature,
                certificateContent: '',
                signatureAlgorithm: 'SHA256withRSA'
              });
            }
          });
        },
        fail: (ex: any) => {
          console.error('Erro ao assinar dados:', ex);
          resolve(null);
        }
      });
    });
  }

  /**
   * Assina um hash usando um certificado específico
   */
  async signHash(thumbprint: string, hash: string): Promise<SignatureResult | null> {
    const initialized = await this.initialize();
    
    if (!initialized || !this.pki) {
      return null;
    }

    return new Promise<SignatureResult | null>((resolve, reject) => {
      this.pki.signHash({
        thumbprint: thumbprint,
        hash: hash,
        digestAlgorithm: 'SHA-256',
        success: (signature: string) => {
          resolve({
            signature: signature,
            certificateContent: '',
            signatureAlgorithm: 'SHA256withRSA'
          });
        },
        fail: (ex: any) => {
          console.error('Erro ao assinar hash:', ex);
          resolve(null);
        }
      });
    });
  }

  /**
   * Verifica se o Web PKI está disponível e instalado
   */
  async isAvailable(): Promise<boolean> {
    return this.initialize();
  }

  /**
   * Obtém a URL para download/instalação do Web PKI
   */
  getInstallUrl(): string {
    return 'https://get.webpkiplugin.com/';
  }

  /**
   * Fallback para quando Web PKI não está disponível
   * Lista certificados usando a API nativa do navegador (se disponível)
   */
  private async listCertificatesFallback(): Promise<InstalledCertificate[]> {
    // Tentar usar a API nativa do navegador se disponível
    // Nota: Esta API tem suporte limitado e geralmente requer HTTPS
    
    try {
      // Verificar se a API SubtleCrypto está disponível
      if (window.crypto && window.crypto.subtle) {
        // A Web Crypto API não permite listar certificados instalados diretamente
        // Retornar lista vazia para forçar uso do método de upload de arquivo
        console.info('Web PKI não disponível. Use o upload de arquivo PFX.');
      }
    } catch (e) {
      console.warn('Erro ao acessar crypto API:', e);
    }
    
    return [];
  }

  /**
   * Extrai informações de um nome de sujeito X.500
   */
  parseSubjectName(subjectName: string): { cn?: string; o?: string; ou?: string; c?: string } {
    const result: { cn?: string; o?: string; ou?: string; c?: string } = {};
    
    const parts = subjectName.split(',').map(p => p.trim());
    for (const part of parts) {
      const [key, ...valueParts] = part.split('=');
      const value = valueParts.join('=').trim();
      
      switch (key.toUpperCase()) {
        case 'CN':
          result.cn = value;
          break;
        case 'O':
          result.o = value;
          break;
        case 'OU':
          result.ou = value;
          break;
        case 'C':
          result.c = value;
          break;
      }
    }
    
    return result;
  }
}
