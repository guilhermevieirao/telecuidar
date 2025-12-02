import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CadsusCidadao {
    // Identificação Principal
    cns: string;
    cpf: string;
    nome: string;
    dataNascimento: string;
    statusCadastro: string;
    
    // Filiação
    nomeMae: string;
    nomePai: string;
    
    // Características
    sexo: string;
    racaCor: string;
    
    // Endereço
    tipoLogradouro: string;
    logradouro: string;
    numero: string;
    complemento: string;
    cidade: string;
    codigoCidade: string;
    paisEnderecoAtual: string;
    cep: string;
    enderecoCompleto: string;
    
    // Naturalidade
    cidadeNascimento: string;
    codigoCidadeNascimento: string;
    paisNascimento: string;
    codigoPaisNascimento: string;
    
    // Contato
    telefones: string[];
    emails: string[];
}

export interface CadsusTokenStatus {
    hasToken: boolean;
    isValid: boolean;
    expiresAt: string;
    expiresIn: string;
    expiresInMs: number;
    message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CadsusService {
    private readonly apiUrl = 'http://localhost:5058/api/Cadsus';

    constructor(private http: HttpClient) { }

    /**
     * Consulta dados de um cidadão no CADSUS por CPF
     * @param cpf CPF do cidadão (com ou sem formatação)
     */
    consultarCpf(cpf: string): Observable<CadsusCidadao> {
        // Remove formatação do CPF
        const cleanCpf = cpf.replace(/\D/g, '');
        return this.http.post<CadsusCidadao>(`${this.apiUrl}/consultar-cpf`, { cpf: cleanCpf });
    }

    /**
     * Verifica o status do token JWT do CADSUS
     */
    getTokenStatus(): Observable<CadsusTokenStatus> {
        return this.http.get<CadsusTokenStatus>(`${this.apiUrl}/token/status`);
    }

    /**
     * Força a renovação manual do token JWT do CADSUS
     */
    renewToken(): Observable<any> {
        return this.http.post(`${this.apiUrl}/token/renew`, {});
    }
}
