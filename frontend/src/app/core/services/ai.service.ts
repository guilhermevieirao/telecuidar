import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AIRequest {
  patientData: any;
  soapData: any;
  biometricData: any;
  customFields: any;
  transcription: any;
  prescription: any;
  examRequests: any;
}

export interface AIResponse {
  summary: string;
  diagnosis: string;
}

@Injectable({
  providedIn: 'root'
})
export class AIService {
  private apiUrl = 'http://localhost:5058/api/AI/analyze'; // Proxy no backend

  constructor(private http: HttpClient) {}

  generateMedicalAnalysis(data: AIRequest): Observable<any> {
    const prompt = this.buildPrompt(data);

    const body = {
      model: 'deepseek-chat',
      messages: [
        {
          role: 'system',
          content: 'Você é um assistente médico especializado em análise de prontuários. Sua função é gerar resumos clínicos concisos e hipóteses diagnósticas baseadas em dados médicos. Sempre use terminologia médica apropriada e seja objetivo.'
        },
        {
          role: 'user',
          content: prompt
        }
      ],
      temperature: 0.7,
      max_tokens: 2000
    };

    console.log('🤖 Enviando requisição para backend proxy...');
    console.log('Endpoint:', this.apiUrl);

    return this.http.post(this.apiUrl, body);
  }

  private buildPrompt(data: AIRequest): string {
    let prompt = '# PRONTUÁRIO MÉDICO - ANÁLISE COMPLETA\n\n';

    // Dados do Paciente
    if (data.patientData) {
      prompt += '## DADOS DO PACIENTE\n';
      prompt += `Nome: ${data.patientData.name || 'Não informado'}\n`;
      prompt += `Idade: ${data.patientData.age || 'Não informado'}\n`;
      prompt += `Sexo: ${data.patientData.gender || 'Não informado'}\n`;
      prompt += `Peso: ${data.patientData.weight || 'Não informado'}\n`;
      prompt += `Altura: ${data.patientData.height || 'Não informado'}\n`;
      prompt += `Tipo Sanguíneo: ${data.patientData.bloodType || 'Não informado'}\n`;
      prompt += `Alergias: ${data.patientData.allergies || 'Nenhuma informada'}\n`;
      prompt += `Condições Pré-existentes: ${data.patientData.preExistingConditions || 'Nenhuma informada'}\n`;
      prompt += `Medicações em Uso: ${data.patientData.currentMedications || 'Nenhuma informada'}\n\n`;
    }

    // Dados Biométricos
    if (data.biometricData) {
      prompt += '## SINAIS VITAIS\n';
      prompt += `Frequência Cardíaca: ${data.biometricData.heartRate?.current || 'Não medido'} bpm\n`;
      prompt += `Pressão Arterial: ${data.biometricData.bloodPressure?.systolic || 'N/A'}/${data.biometricData.bloodPressure?.diastolic || 'N/A'} mmHg\n`;
      prompt += `Saturação O2: ${data.biometricData.oxygenSaturation?.current || 'Não medido'}%\n`;
      prompt += `Temperatura: ${data.biometricData.temperature?.current || 'Não medido'}°C\n`;
      prompt += `Frequência Respiratória: ${data.biometricData.respiratoryRate?.current || 'Não medido'} rpm\n\n`;
    }

    // SOAP
    if (data.soapData) {
      prompt += '## MÉTODO SOAP\n';
      if (data.soapData.subjective) {
        prompt += `### Subjetivo (Queixa Principal):\n${data.soapData.subjective}\n\n`;
      }
      if (data.soapData.objective) {
        prompt += `### Objetivo (Exame Físico):\n${data.soapData.objective}\n\n`;
      }
      if (data.soapData.assessment) {
        prompt += `### Avaliação (Impressão Clínica):\n${data.soapData.assessment}\n\n`;
      }
      if (data.soapData.plan) {
        prompt += `### Plano (Conduta):\n${data.soapData.plan}\n\n`;
      }
    }

    // Campos Personalizados
    if (data.customFields && Object.keys(data.customFields).length > 0) {
      prompt += '## CAMPOS ESPECÍFICOS DA ESPECIALIDADE\n';
      for (const [fieldName, value] of Object.entries(data.customFields)) {
        prompt += `${fieldName}: ${value}\n`;
      }
      prompt += '\n';
    }

    // Transcrição
    if (data.transcription && data.transcription.length > 0) {
      prompt += '## TRANSCRIÇÃO DA CONSULTA\n';
      data.transcription.forEach((entry: any) => {
        const speaker = entry.speaker === 'professional' ? 'Profissional' : 'Paciente';
        prompt += `${speaker}: ${entry.text}\n`;
      });
      prompt += '\n';
    }

    // Prescrição
    if (data.prescription && data.prescription.length > 0) {
      prompt += '## PRESCRIÇÃO MÉDICA\n';
      data.prescription.forEach((med: any, index: number) => {
        prompt += `${index + 1}. ${med.medication || 'Medicamento'} - ${med.dosage || ''} - ${med.frequency || ''} por ${med.duration || ''}\n`;
        if (med.instructions) {
          prompt += `   Instruções: ${med.instructions}\n`;
        }
      });
      prompt += '\n';
    }

    // Exames Solicitados
    if (data.examRequests && data.examRequests.length > 0) {
      prompt += '## EXAMES SOLICITADOS\n';
      data.examRequests.forEach((exam: any, index: number) => {
        prompt += `${index + 1}. ${exam.examName || 'Exame'}\n`;
        if (exam.justification) {
          prompt += `   Justificativa: ${exam.justification}\n`;
        }
      });
      prompt += '\n';
    }

    prompt += '---\n\n';
    prompt += 'Com base nessas informações, forneça:\n\n';
    prompt += '1. **RESUMO CLÍNICO**: Um resumo conciso e estruturado do atendimento (máximo 200 palavras)\n\n';
    prompt += '2. **HIPÓTESES DIAGNÓSTICAS**: Liste as principais hipóteses diagnósticas em ordem de probabilidade, com justificativa baseada nos dados apresentados\n\n';
    prompt += 'Formato da resposta:\n';
    prompt += '---RESUMO---\n[seu resumo aqui]\n\n';
    prompt += '---DIAGNOSTICO---\n[suas hipóteses diagnósticas aqui]';

    return prompt;
  }

  parseAIResponse(rawResponse: string): AIResponse {
    const parts = rawResponse.split('---DIAGNOSTICO---');
    
    let summary = '';
    let diagnosis = '';

    if (parts.length >= 2) {
      summary = parts[0].replace('---RESUMO---', '').trim();
      diagnosis = parts[1].trim();
    } else {
      // Fallback se o formato não for seguido
      const lines = rawResponse.split('\n');
      let isInSummary = false;
      let isInDiagnosis = false;

      lines.forEach(line => {
        if (line.includes('RESUMO') || line.includes('Resumo')) {
          isInSummary = true;
          isInDiagnosis = false;
          return;
        }
        if (line.includes('DIAGNÓS') || line.includes('HIPÓTESE') || line.includes('Diagnós')) {
          isInSummary = false;
          isInDiagnosis = true;
          return;
        }
        
        if (isInSummary && line.trim()) {
          summary += line + '\n';
        }
        if (isInDiagnosis && line.trim()) {
          diagnosis += line + '\n';
        }
      });
    }

    return {
      summary: summary.trim() || 'Não foi possível gerar o resumo.',
      diagnosis: diagnosis.trim() || 'Não foi possível gerar hipóteses diagnósticas.'
    };
  }
}
