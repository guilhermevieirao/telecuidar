# Componente Medical Record Tabs

## Descrição
Componente completo de prontuário médico com múltiplas abas para registro de atendimentos em videochamadas.

## Funcionalidades

### 📋 Tabs Disponíveis

1. **Paciente** - Informações demográficas e de contato
   - Nome completo
   - CPF
   - Data de nascimento
   - Idade
   - Sexo
   - Telefone
   - E-mail
   - Endereço

2. **SOAP** - Método de documentação clínica
   - **S**ubjetivo: Relatos do paciente
   - **O**bjetivo: Achados clínicos mensuráveis
   - **A**valiação: Diagnóstico ou hipótese diagnóstica
   - **P**lano: Conduta terapêutica e acompanhamento

3. **Biométricos** - Monitoramento em tempo real com gráficos
   - Frequência Cardíaca (BPM) com gráfico de linha
   - Temperatura Corporal (°C) com gráfico de linha
   - Pressão Arterial (mmHg) com gráfico de linha dupla (sistólica/diastólica)
   - Saturação de Oxigênio (%) com gráfico de linha
   - Estatísticas com médias calculadas

4. **Prescrição** - Prescrição médica detalhada
   - Lista de medicações com:
     - Nome do medicamento
     - Dosagem
     - Frequência
     - Duração
     - Instruções de uso
   - Instruções gerais

5. **Exames** - Solicitação de exames
   - Lista de exames com:
     - Tipo de exame
     - Descrição
     - Urgência (normal, urgente, rotina)
     - Observações
   - Observações gerais

### 🎨 Modos de Visualização

O componente possui **3 modos de visualização** que podem ser alternados através de um botão:

1. **Oculto** - Painel completamente escondido
   - Botão flutuante para reabrir
   - Não interfere na videochamada

2. **Lateral (Sidebar)** - Painel lateral fixo
   - Largura de 450px
   - Posicionado à direita da tela
   - Convive com a videochamada

3. **Tela Cheia (Fullscreen)** - Ocupa toda a tela
   - Sobrepõe a videochamada
   - Ideal para preenchimento detalhado
   - Largura máxima de 1200px centralizada

### 📊 Gráficos Biométricos

Os gráficos são gerados usando **Chart.js** com dados fictícios que simulam:
- 20 pontos de dados ao longo do tempo
- Intervalos de 5 minutos entre cada leitura
- Valores dentro de faixas normais para cada parâmetro

### 📄 Exportação para PDF

Funcionalidade completa de exportação usando **jsPDF**:
- Cabeçalho com título e data
- Todas as seções do prontuário
- Formatação profissional
- Nome do arquivo: `prontuario_[NOME_PACIENTE]_[TIMESTAMP].pdf`

### 💾 Salvamento de Dados

- Botão de salvar para persistir dados
- Estrutura completa de dados exportável
- Timestamp de salvamento
- Preparado para integração com backend

## Uso

```typescript
<app-medical-record-tabs 
  [appointmentId]="appointmentId"
  [patientName]="appointment.patientName"
  (onClose)="handleClose()">
</app-medical-record-tabs>
```

### Inputs
- `appointmentId`: ID da consulta (number)
- `patientName`: Nome do paciente (string)

### Outputs
- `onClose`: Evento emitido ao fechar o painel

## Dependências

- **Chart.js** (^4.5.1) - Geração de gráficos
- **jsPDF** (latest) - Exportação para PDF
- **html2canvas** (latest) - Captura de elementos HTML para PDF

## Estrutura de Dados

### PatientInfo
```typescript
interface PatientInfo {
  name: string;
  cpf: string;
  age: number | null;
  gender: string;
  birthDate: string;
  phone: string;
  email: string;
  address: string;
}
```

### SOAPData
```typescript
interface SOAPData {
  subjective: string;
  objective: string;
  assessment: string;
  plan: string;
}
```

### BiometricData
```typescript
interface BiometricData {
  heartRate: number[];
  temperature: number[];
  bloodPressure: { systolic: number; diastolic: number }[];
  oxygenSaturation: number[];
  timestamps: string[];
}
```

### Prescription
```typescript
interface Prescription {
  medications: Array<{
    name: string;
    dosage: string;
    frequency: string;
    duration: string;
    instructions: string;
  }>;
  generalInstructions: string;
}
```

### ExamRequest
```typescript
interface ExamRequest {
  exams: Array<{
    type: string;
    description: string;
    urgency: string;
    notes: string;
  }>;
  generalNotes: string;
}
```

## Estilos

O componente possui estilos responsivos e animações suaves:
- Gradientes modernos
- Transições fluidas
- Design adaptável
- Scrollbars personalizadas
- Impressão otimizada

## Personalização

### Cores Principais
- Primária: `#667eea` / `#764ba2` (gradiente roxo)
- Sucesso: `#10b981` (verde)
- Perigo: `#dc2626` (vermelho)
- Informação: `#3b82f6` (azul)

### Breakpoints
- Desktop: > 1024px
- Tablet: 768px - 1024px
- Mobile: < 768px

## Notas de Desenvolvimento

1. Os gráficos são inicializados após o DOM estar pronto
2. Gráficos são destruídos ao trocar de aba para evitar memory leaks
3. Dados biométricos fictícios são gerados no `ngOnInit`
4. Exportação PDF aguarda renderização completa dos elementos

## Melhorias Futuras

- [ ] Integração com API de backend para salvamento
- [ ] Conexão com dispositivos IoT para dados biométricos reais
- [ ] Templates de prescrição pré-definidos
- [ ] Histórico de consultas anteriores
- [ ] Assinatura digital
- [ ] Envio de prescrição por e-mail
- [ ] OCR para leitura de receitas antigas
