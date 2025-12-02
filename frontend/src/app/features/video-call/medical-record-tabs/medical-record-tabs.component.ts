import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { CommonModule, NgIf, NgForOf, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Chart, ChartConfiguration, registerables } from 'chart.js';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { SpecialtyService } from '../../../services/specialty.service';
import { AppointmentFieldService } from '../../../services/appointment-field.service';
import { AIService, AIResponse } from '../../../core/services/ai.service';
import { CadsusService, CadsusCidadao } from '../../../core/services/cadsus.service';
import { SpecialtyFieldDto } from '../../../models/specialty.model';
import { AppointmentFieldValueDto, SaveAppointmentFieldValueDto } from '../../../models/appointment-field.model';

Chart.register(...registerables);

export type TabMode = 'hidden' | 'sidebar' | 'fullscreen';
export type ActiveTab = 'patient' | 'soap' | 'custom-fields' | 'biometric' | 'prescription' | 'exams' | 'transcription' | 'ai-analysis';

interface PatientInfo {
  cns: string;
  cpf: string;
  name: string;
  birthDate: string;
  registrationStatus: string;
  motherName: string;
  fatherName: string;
  gender: string;
  raceColor: string;
  streetType: string;
  street: string;
  number: string;
  complement: string;
  city: string;
  country: string;
  zipCode: string;
  fullAddress: string;
  birthCity: string;
  birthCountry: string;
  phones: string;
  emails: string;
}

interface SOAPData {
  subjective: string;
  objective: string;
  assessment: string;
  plan: string;
}

interface BiometricData {
  heartRate: number[];
  temperature: number[];
  bloodPressure: { systolic: number; diastolic: number }[];
  oxygenSaturation: number[];
  timestamps: string[];
}

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

interface ExamRequest {
  exams: Array<{
    type: string;
    description: string;
    urgency: string;
    notes: string;
  }>;
  generalNotes: string;
}

interface TranscriptionEntry {
  id: number;
  speaker: 'professional' | 'patient';
  text: string;
  timestamp: Date;
  confidence: number;
}

interface TranscriptionData {
  entries: TranscriptionEntry[];
  isRecording: boolean;
  isPaused: boolean;
}

// Declaração para Web Speech API
declare global {
  interface Window {
    SpeechRecognition: any;
    webkitSpeechRecognition: any;
  }
}

@Component({
  selector: 'app-medical-record-tabs',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIf, NgForOf, NgClass],
  providers: [AIService, CadsusService],
  templateUrl: './medical-record-tabs.component.html',
  styleUrls: ['./medical-record-tabs.component.scss']
})
export class MedicalRecordTabsComponent implements OnInit {
  @Input() appointmentId: number = 0;
  @Input() patientName: string = '';
  @Input() specialtyId: number = 0; // ID da especialidade da consulta
  @Input() jitsiApi: any = null; // Referência para API do Jitsi
  @Input() isDarkTheme: boolean = true; // Receber tema do componente pai
  @Output() onClose = new EventEmitter<void>();
  @Output() onModeChange = new EventEmitter<TabMode>();

  @ViewChild('contentToPrint') contentToPrint!: ElementRef;

  tabMode: TabMode = 'sidebar';
  activeTab: ActiveTab = 'patient';

  // Dados das tabs
  patientInfo: PatientInfo = {
    cns: '',
    cpf: '',
    name: '',
    birthDate: '',
    registrationStatus: '',
    motherName: '',
    fatherName: '',
    gender: '',
    raceColor: '',
    streetType: '',
    street: '',
    number: '',
    complement: '',
    city: '',
    country: '',
    zipCode: '',
    fullAddress: '',
    birthCity: '',
    birthCountry: '',
    phones: '',
    emails: ''
  };

  soapData: SOAPData = {
    subjective: '',
    objective: '',
    assessment: '',
    plan: ''
  };

  biometricData: BiometricData = {
    heartRate: [],
    temperature: [],
    bloodPressure: [],
    oxygenSaturation: [],
    timestamps: []
  };

  prescription: Prescription = {
    medications: [],
    generalInstructions: ''
  };

  examRequests: ExamRequest = {
    exams: [],
    generalNotes: ''
  };

  transcriptionData: TranscriptionData = {
    entries: [],
    isRecording: false,
    isPaused: false
  };

  // Campos personalizados
  customFields: SpecialtyFieldDto[] = [];
  customFieldValues: { [fieldId: number]: any } = {};
  loadingCustomFields = false;

  // Sistema de ditado por voz
  private voiceDictationRecognition: any = null;
  isVoiceDictationActive = false;
  private currentFocusedField: HTMLElement | null = null;
  private currentFocusedFieldId: string | null = null;
  private lastInterimText = ''; // Armazena o último texto intermediário
  private baseTextBeforeDictation = ''; // Texto base antes de começar a ditar

  // Sistema de transcrição dual
  private recognitionProfessional: any = null; // Microfone local
  private recognitionPatient: any = null; // Áudio do paciente (Jitsi)
  private recognitionActive = false;
  private entryIdCounter = 1;
  private microphoneStream: MediaStream | null = null;
  private audioContext: AudioContext | null = null;
  private remoteAudioDestination: MediaStreamAudioDestinationNode | null = null;
  private jitsiAudioMonitoring = false;

  // Gráficos
  private heartRateChart: Chart | null = null;
  private temperatureChart: Chart | null = null;
  private bloodPressureChart: Chart | null = null;
  private oxygenChart: Chart | null = null;

  // IA Médica
  isGeneratingAI = false;
  aiAnalysisResult: AIResponse | null = null;
  aiError: string | null = null;

  // CADSUS
  isConsultingCadsus = false;
  cadsusError: string | null = null;
  cadsusSuccess: string | null = null;

  constructor(
    private specialtyService: SpecialtyService,
    private appointmentFieldService: AppointmentFieldService,
    private aiService: AIService,
    private cadsusService: CadsusService
  ) {}

  ngOnInit(): void {
    this.patientInfo.name = this.patientName;
    this.generateMockBiometricData();
    this.initializeSpeechRecognition();
    this.initializeVoiceDictation();
    this.loadCustomFieldsForAppointment();
    // Emitir estado inicial do modo
    this.onModeChange.emit(this.tabMode);
  }

  ngAfterViewInit(): void {
    // Aguardar o DOM estar pronto antes de criar os gráficos
    setTimeout(() => {
      if (this.activeTab === 'biometric') {
        this.initializeCharts();
      }
    }, 100);
    
    // Configurar listeners para detectar foco em campos
    this.setupFieldFocusListeners();
  }

  ngOnDestroy(): void {
    this.destroyCharts();
    this.stopTranscription();
    this.stopVoiceDictation();
  }

  // Controle de modo de visualização
  cycleTabMode(): void {
    const modes: TabMode[] = ['hidden', 'sidebar', 'fullscreen'];
    const currentIndex = modes.indexOf(this.tabMode);
    this.tabMode = modes[(currentIndex + 1) % modes.length];
    this.onModeChange.emit(this.tabMode);
  }

  getTabModeLabel(): string {
    const labels = {
      hidden: '👁️ Oculto',
      sidebar: '📋 Lateral',
      fullscreen: '⛶ Tela Cheia'
    };
    return labels[this.tabMode];
  }

  getTabModeIcon(): string {
    const icons = {
      hidden: '👁️',
      sidebar: '📋',
      fullscreen: '⛶'
    };
    return icons[this.tabMode];
  }

  // Navegação de tabs
  setActiveTab(tab: ActiveTab): void {
    this.activeTab = tab;
    if (tab === 'biometric') {
      setTimeout(() => this.initializeCharts(), 100);
    }
  }

  // Geração de dados fictícios
  private generateMockBiometricData(): void {
    const now = new Date();
    const dataPoints = 20;

    for (let i = 0; i < dataPoints; i++) {
      const timestamp = new Date(now.getTime() - (dataPoints - i) * 5 * 60000);
      this.biometricData.timestamps.push(
        timestamp.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })
      );

      // BPM: 60-100 (normal)
      this.biometricData.heartRate.push(
        Math.floor(Math.random() * 20) + 70
      );

      // Temperatura: 36-37°C
      this.biometricData.temperature.push(
        +(36 + Math.random()).toFixed(1)
      );

      // Pressão arterial
      this.biometricData.bloodPressure.push({
        systolic: Math.floor(Math.random() * 20) + 110,
        diastolic: Math.floor(Math.random() * 15) + 70
      });

      // Saturação O2: 95-100%
      this.biometricData.oxygenSaturation.push(
        Math.floor(Math.random() * 5) + 96
      );
    }
  }

  // Inicialização dos gráficos
  private initializeCharts(): void {
    this.destroyCharts();
    
    setTimeout(() => {
      this.createHeartRateChart();
      this.createTemperatureChart();
      this.createBloodPressureChart();
      this.createOxygenChart();
    }, 50);
  }

  private destroyCharts(): void {
    if (this.heartRateChart) {
      this.heartRateChart.destroy();
      this.heartRateChart = null;
    }
    if (this.temperatureChart) {
      this.temperatureChart.destroy();
      this.temperatureChart = null;
    }
    if (this.bloodPressureChart) {
      this.bloodPressureChart.destroy();
      this.bloodPressureChart = null;
    }
    if (this.oxygenChart) {
      this.oxygenChart.destroy();
      this.oxygenChart = null;
    }
  }

  private createHeartRateChart(): void {
    const canvas = document.getElementById('heartRateChart') as HTMLCanvasElement;
    if (!canvas) return;

    const config: ChartConfiguration = {
      type: 'line',
      data: {
        labels: this.biometricData.timestamps,
        datasets: [{
          label: 'BPM',
          data: this.biometricData.heartRate,
          borderColor: 'rgb(239, 68, 68)',
          backgroundColor: 'rgba(239, 68, 68, 0.1)',
          tension: 0.4,
          fill: true
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false },
          title: { display: true, text: 'Frequência Cardíaca (BPM)' }
        },
        scales: {
          y: {
            beginAtZero: false,
            min: 50,
            max: 120
          }
        }
      }
    };

    this.heartRateChart = new Chart(canvas, config);
  }

  private createTemperatureChart(): void {
    const canvas = document.getElementById('temperatureChart') as HTMLCanvasElement;
    if (!canvas) return;

    const config: ChartConfiguration = {
      type: 'line',
      data: {
        labels: this.biometricData.timestamps,
        datasets: [{
          label: '°C',
          data: this.biometricData.temperature,
          borderColor: 'rgb(34, 197, 94)',
          backgroundColor: 'rgba(34, 197, 94, 0.1)',
          tension: 0.4,
          fill: true
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false },
          title: { display: true, text: 'Temperatura Corporal (°C)' }
        },
        scales: {
          y: {
            beginAtZero: false,
            min: 35,
            max: 39
          }
        }
      }
    };

    this.temperatureChart = new Chart(canvas, config);
  }

  private createBloodPressureChart(): void {
    const canvas = document.getElementById('bloodPressureChart') as HTMLCanvasElement;
    if (!canvas) return;

    const config: ChartConfiguration = {
      type: 'line',
      data: {
        labels: this.biometricData.timestamps,
        datasets: [
          {
            label: 'Sistólica',
            data: this.biometricData.bloodPressure.map(bp => bp.systolic),
            borderColor: 'rgb(59, 130, 246)',
            backgroundColor: 'rgba(59, 130, 246, 0.1)',
            tension: 0.4
          },
          {
            label: 'Diastólica',
            data: this.biometricData.bloodPressure.map(bp => bp.diastolic),
            borderColor: 'rgb(147, 51, 234)',
            backgroundColor: 'rgba(147, 51, 234, 0.1)',
            tension: 0.4
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: true, position: 'top' },
          title: { display: true, text: 'Pressão Arterial (mmHg)' }
        },
        scales: {
          y: {
            beginAtZero: false,
            min: 60,
            max: 150
          }
        }
      }
    };

    this.bloodPressureChart = new Chart(canvas, config);
  }

  private createOxygenChart(): void {
    const canvas = document.getElementById('oxygenChart') as HTMLCanvasElement;
    if (!canvas) return;

    const config: ChartConfiguration = {
      type: 'line',
      data: {
        labels: this.biometricData.timestamps,
        datasets: [{
          label: 'SpO2 %',
          data: this.biometricData.oxygenSaturation,
          borderColor: 'rgb(14, 165, 233)',
          backgroundColor: 'rgba(14, 165, 233, 0.1)',
          tension: 0.4,
          fill: true
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false },
          title: { display: true, text: 'Saturação de Oxigênio (%)' }
        },
        scales: {
          y: {
            beginAtZero: false,
            min: 90,
            max: 100
          }
        }
      }
    };

    this.oxygenChart = new Chart(canvas, config);
  }

  // Gerenciamento de prescrições
  addMedication(): void {
    this.prescription.medications.push({
      name: '',
      dosage: '',
      frequency: '',
      duration: '',
      instructions: ''
    });
  }

  removeMedication(index: number): void {
    this.prescription.medications.splice(index, 1);
  }

  // Gerenciamento de exames
  addExam(): void {
    this.examRequests.exams.push({
      type: '',
      description: '',
      urgency: 'normal',
      notes: ''
    });
  }

  removeExam(index: number): void {
    this.examRequests.exams.splice(index, 1);
  }

  // Exportação para PDF
  async exportToPDF(): Promise<void> {
    try {
      const pdf = new jsPDF('p', 'mm', 'a4');
      const pageWidth = pdf.internal.pageSize.getWidth();
      const pageHeight = pdf.internal.pageSize.getHeight();
      let yPosition = 20;

      // Cabeçalho
      pdf.setFontSize(18);
      pdf.setFont('helvetica', 'bold');
      pdf.text('Prontuário Médico', pageWidth / 2, yPosition, { align: 'center' });
      yPosition += 10;

      pdf.setFontSize(10);
      pdf.setFont('helvetica', 'normal');
      pdf.text(`Data: ${new Date().toLocaleDateString('pt-BR')}`, pageWidth / 2, yPosition, { align: 'center' });
      yPosition += 15;

      // Dados do Paciente
      pdf.setFontSize(14);
      pdf.setFont('helvetica', 'bold');
      pdf.text('1. Dados do Paciente', 15, yPosition);
      yPosition += 8;

      pdf.setFontSize(10);
      pdf.setFont('helvetica', 'normal');
      const patientData = [
        `Nome: ${this.patientInfo.name}`,
        `CPF: ${this.patientInfo.cpf}`,
        `CNS: ${this.patientInfo.cns}`,
        `Data de Nascimento: ${this.patientInfo.birthDate}`,
        `Sexo: ${this.patientInfo.gender}`,
        `Raça/Cor: ${this.patientInfo.raceColor}`,
        `Telefones: ${this.patientInfo.phones}`,
        `Emails: ${this.patientInfo.emails}`,
        `Endereço: ${this.patientInfo.fullAddress}`
      ];

      patientData.forEach(line => {
        pdf.text(line, 20, yPosition);
        yPosition += 6;
      });
      yPosition += 5;

      // SOAP
      pdf.setFontSize(14);
      pdf.setFont('helvetica', 'bold');
      pdf.text('2. SOAP', 15, yPosition);
      yPosition += 8;

      pdf.setFontSize(10);
      pdf.setFont('helvetica', 'bold');
      pdf.text('Subjetivo:', 20, yPosition);
      yPosition += 5;
      pdf.setFont('helvetica', 'normal');
      const subjectiveLines = pdf.splitTextToSize(this.soapData.subjective || 'N/A', pageWidth - 30);
      pdf.text(subjectiveLines, 20, yPosition);
      yPosition += subjectiveLines.length * 5 + 5;

      pdf.setFont('helvetica', 'bold');
      pdf.text('Objetivo:', 20, yPosition);
      yPosition += 5;
      pdf.setFont('helvetica', 'normal');
      const objectiveLines = pdf.splitTextToSize(this.soapData.objective || 'N/A', pageWidth - 30);
      pdf.text(objectiveLines, 20, yPosition);
      yPosition += objectiveLines.length * 5 + 5;

      pdf.setFont('helvetica', 'bold');
      pdf.text('Avaliação:', 20, yPosition);
      yPosition += 5;
      pdf.setFont('helvetica', 'normal');
      const assessmentLines = pdf.splitTextToSize(this.soapData.assessment || 'N/A', pageWidth - 30);
      pdf.text(assessmentLines, 20, yPosition);
      yPosition += assessmentLines.length * 5 + 5;

      pdf.setFont('helvetica', 'bold');
      pdf.text('Plano:', 20, yPosition);
      yPosition += 5;
      pdf.setFont('helvetica', 'normal');
      const planLines = pdf.splitTextToSize(this.soapData.plan || 'N/A', pageWidth - 30);
      pdf.text(planLines, 20, yPosition);
      yPosition += planLines.length * 5 + 10;

      // Nova página se necessário
      if (yPosition > pageHeight - 40) {
        pdf.addPage();
        yPosition = 20;
      }

      // Campos Personalizados
      if (this.customFields.length > 0) {
        pdf.setFontSize(14);
        pdf.setFont('helvetica', 'bold');
        pdf.text('2. Campos Personalizados', 15, yPosition);
        yPosition += 8;

        this.customFields.forEach((field, index) => {
          // Verificar se precisa de nova página
          if (yPosition > pageHeight - 30) {
            pdf.addPage();
            yPosition = 20;
          }

          pdf.setFontSize(10);
          pdf.setFont('helvetica', 'bold');
          pdf.text(`${field.label}:`, 20, yPosition);
          yPosition += 5;

          pdf.setFont('helvetica', 'normal');
          let value = this.customFieldValues[field.id];
          
          // Formatar valor de acordo com o tipo
          if (value === undefined || value === null || value === '') {
            value = 'N/A';
          } else if (field.fieldType === 'checkbox') {
            value = value ? 'Sim' : 'Não';
          } else if (field.fieldType === 'date') {
            try {
              const date = new Date(value);
              value = date.toLocaleDateString('pt-BR');
            } catch (e) {
              // Manter valor original se não for data válida
            }
          }

          const valueLines = pdf.splitTextToSize(String(value), pageWidth - 30);
          pdf.text(valueLines, 20, yPosition);
          yPosition += valueLines.length * 5 + 5;
        });

        yPosition += 5;
      }

      // Nova página se necessário
      if (yPosition > pageHeight - 40) {
        pdf.addPage();
        yPosition = 20;
      }

      // Prescrição
      pdf.setFontSize(14);
      pdf.setFont('helvetica', 'bold');
      const prescriptionNumber = this.customFields.length > 0 ? '3' : '2';
      pdf.text(`${prescriptionNumber}. Prescrição Médica`, 15, yPosition);
      yPosition += 8;

      if (this.prescription.medications.length > 0) {
        this.prescription.medications.forEach((med, index) => {
          pdf.setFontSize(10);
          pdf.setFont('helvetica', 'bold');
          pdf.text(`Medicamento ${index + 1}:`, 20, yPosition);
          yPosition += 5;
          
          pdf.setFont('helvetica', 'normal');
          pdf.text(`• ${med.name} - ${med.dosage}`, 25, yPosition);
          yPosition += 5;
          pdf.text(`  ${med.frequency} por ${med.duration}`, 25, yPosition);
          yPosition += 5;
          if (med.instructions) {
            const instrLines = pdf.splitTextToSize(`  ${med.instructions}`, pageWidth - 35);
            pdf.text(instrLines, 25, yPosition);
            yPosition += instrLines.length * 5;
          }
          yPosition += 3;
        });
      } else {
        pdf.setFontSize(10);
        pdf.setFont('helvetica', 'normal');
        pdf.text('Nenhuma medicação prescrita', 20, yPosition);
        yPosition += 8;
      }

      // Nova página se necessário
      if (yPosition > pageHeight - 40) {
        pdf.addPage();
        yPosition = 20;
      }

      // Exames
      pdf.setFontSize(14);
      pdf.setFont('helvetica', 'bold');
      const examsNumber = this.customFields.length > 0 ? '4' : '3';
      pdf.text(`${examsNumber}. Solicitação de Exames`, 15, yPosition);
      yPosition += 8;

      if (this.examRequests.exams.length > 0) {
        this.examRequests.exams.forEach((exam, index) => {
          pdf.setFontSize(10);
          pdf.setFont('helvetica', 'bold');
          pdf.text(`Exame ${index + 1}:`, 20, yPosition);
          yPosition += 5;
          
          pdf.setFont('helvetica', 'normal');
          pdf.text(`• Tipo: ${exam.type}`, 25, yPosition);
          yPosition += 5;
          pdf.text(`• Descrição: ${exam.description}`, 25, yPosition);
          yPosition += 5;
          pdf.text(`• Urgência: ${exam.urgency}`, 25, yPosition);
          yPosition += 5;
          if (exam.notes) {
            const notesLines = pdf.splitTextToSize(`• Observações: ${exam.notes}`, pageWidth - 35);
            pdf.text(notesLines, 25, yPosition);
            yPosition += notesLines.length * 5;
          }
          yPosition += 3;
        });
      } else {
        pdf.setFontSize(10);
        pdf.setFont('helvetica', 'normal');
        pdf.text('Nenhum exame solicitado', 20, yPosition);
      }

      // Nova página se necessário
      if (yPosition > pageHeight - 40) {
        pdf.addPage();
        yPosition = 20;
      }

      // Transcrição
      if (this.transcriptionData.entries.length > 0) {
        pdf.setFontSize(14);
        pdf.setFont('helvetica', 'bold');
        const transcriptionNumber = this.customFields.length > 0 ? '5' : '4';
        pdf.text(`${transcriptionNumber}. Transcrição da Consulta`, 15, yPosition);
        yPosition += 8;

        this.transcriptionData.entries.forEach((entry, index) => {
          // Verificar se precisa de nova página
          if (yPosition > pageHeight - 40) {
            pdf.addPage();
            yPosition = 20;
          }

          const time = entry.timestamp.toLocaleTimeString('pt-BR');
          const speaker = entry.speaker === 'professional' ? 'Profissional' : 'Paciente';
          
          pdf.setFontSize(9);
          pdf.setFont('helvetica', 'bold');
          pdf.text(`[${time}] ${speaker}:`, 20, yPosition);
          yPosition += 5;
          
          pdf.setFont('helvetica', 'normal');
          const textLines = pdf.splitTextToSize(entry.text, pageWidth - 35);
          pdf.text(textLines, 25, yPosition);
          yPosition += textLines.length * 4 + 3;
        });
      }

      // Salvar PDF
      pdf.save(`prontuario_${this.patientInfo.name.replace(/\s+/g, '_')}_${new Date().getTime()}.pdf`);
      
      alert('PDF gerado com sucesso!');
    } catch (error) {
      console.error('Erro ao gerar PDF:', error);
      alert('Erro ao gerar PDF. Verifique o console para mais detalhes.');
    }
  }

  // Salvar dados
  saveData(): void {
    const data = {
      appointmentId: this.appointmentId,
      patientInfo: this.patientInfo,
      soap: this.soapData,
      biometric: this.biometricData,
      prescription: this.prescription,
      examRequests: this.examRequests,
      transcription: this.transcriptionData,
      timestamp: new Date().toISOString()
    };

    console.log('Salvando dados:', data);
    // Aqui você implementaria a chamada ao backend
    alert('Dados salvos com sucesso!');
  }

  // Inicialização do sistema dual de reconhecimento de voz
  private initializeSpeechRecognition(): void {
    const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    
    if (!SpeechRecognition) {
      console.warn('Web Speech API não suportada neste navegador');
      return;
    }

    // Reconhecedor para o profissional (microfone local)
    this.recognitionProfessional = this.createRecognizer('professional');
    
    // Reconhecedor para o paciente (áudio do sistema)
    this.recognitionPatient = this.createRecognizer('patient');
    
    console.log('Sistema dual de reconhecimento inicializado');
  }

  // Criar instância de reconhecedor
  private createRecognizer(speaker: 'professional' | 'patient'): any {
    const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    const recognition = new SpeechRecognition();
    
    recognition.continuous = true;
    recognition.interimResults = false;
    recognition.lang = 'pt-BR';
    recognition.maxAlternatives = 1;

    recognition.onresult = (event: any) => {
      const last = event.results.length - 1;
      const transcript = event.results[last][0].transcript;
      const confidence = event.results[last][0].confidence;

      this.addTranscriptionEntry(speaker, transcript, confidence);
    };

    recognition.onerror = (event: any) => {
      console.error(`Erro no reconhecimento (${speaker}):`, event.error);
      if (event.error === 'no-speech') {
        // Reiniciar se não detectar fala
        if (this.recognitionActive && !this.transcriptionData.isPaused) {
          setTimeout(() => {
            if (this.recognitionActive) {
              try {
                recognition.start();
              } catch (e) {
                // Ignorar erro se já estiver rodando
              }
            }
          }, 100);
        }
      }
    };

    recognition.onend = () => {
      // Reiniciar automaticamente se ainda estiver ativo
      if (this.recognitionActive && !this.transcriptionData.isPaused) {
        try {
          recognition.start();
        } catch (e) {
          console.error(`Erro ao reiniciar reconhecimento (${speaker}):`, e);
        }
      }
    };

    return recognition;
  }

  // Capturar áudio do microfone (profissional)
  private async captureMicrophoneAudio(): Promise<boolean> {
    try {
      this.microphoneStream = await navigator.mediaDevices.getUserMedia({ 
        audio: {
          echoCancellation: true,
          noiseSuppression: true,
          autoGainControl: true
        } 
      });
      console.log('✅ Microfone capturado (Profissional)');
      return true;
    } catch (error) {
      console.error('❌ Erro ao capturar microfone:', error);
      alert('Não foi possível acessar o microfone. Verifique as permissões.');
      return false;
    }
  }

  // Capturar áudio remoto do Jitsi automaticamente (paciente)
  private async captureJitsiRemoteAudio(): Promise<boolean> {
    try {
      // Criar AudioContext para processar áudio
      this.audioContext = new AudioContext();
      this.remoteAudioDestination = this.audioContext.createMediaStreamDestination();

      // Buscar todos os elementos de áudio remoto do Jitsi
      const remoteAudioElements = document.querySelectorAll('audio[id^="remoteAudio"]');
      
      if (remoteAudioElements.length === 0) {
        console.warn('⚠️ Nenhum áudio remoto detectado ainda');
        // Tentar novamente após delay
        await new Promise(resolve => setTimeout(resolve, 2000));
        const retry = document.querySelectorAll('audio[id^="remoteAudio"]');
        if (retry.length === 0) {
          console.warn('💡 Paciente ainda não entrou na chamada');
          return false;
        }
      }

      // Conectar todos os áudios remotos ao destino
      document.querySelectorAll('audio[id^="remoteAudio"]').forEach((audioElement: any) => {
        if (audioElement.srcObject) {
          const source = this.audioContext!.createMediaStreamSource(audioElement.srcObject);
          source.connect(this.remoteAudioDestination!);
          console.log('🔗 Áudio remoto conectado:', audioElement.id);
        }
      });

      // Monitorar novos participantes
      this.startJitsiAudioMonitoring();

      console.log('✅ Captura de áudio do Jitsi configurada (Paciente)');
      return true;
    } catch (error) {
      console.error('❌ Erro ao capturar áudio do Jitsi:', error);
      return false;
    }
  }

  // Monitorar novos participantes entrando no Jitsi
  private startJitsiAudioMonitoring(): void {
    if (this.jitsiAudioMonitoring) return;
    
    this.jitsiAudioMonitoring = true;
    
    // Observar mudanças no DOM para detectar novos áudios remotos
    const observer = new MutationObserver(() => {
      if (this.audioContext && this.remoteAudioDestination) {
        document.querySelectorAll('audio[id^="remoteAudio"]').forEach((audioElement: any) => {
          if (audioElement.srcObject && !audioElement.dataset.connected) {
            try {
              const source = this.audioContext!.createMediaStreamSource(audioElement.srcObject);
              source.connect(this.remoteAudioDestination!);
              audioElement.dataset.connected = 'true';
              console.log('🔗 Novo participante conectado:', audioElement.id);
            } catch (e) {
              // Áudio já conectado
            }
          }
        });
      }
    });

    // Observar o container do Jitsi
    const jitsiContainer = document.getElementById('jitsi-meet-container');
    if (jitsiContainer) {
      observer.observe(jitsiContainer, {
        childList: true,
        subtree: true
      });
    }
  }

  // Adicionar entrada de transcrição
  private addTranscriptionEntry(speaker: 'professional' | 'patient', text: string, confidence: number): void {
    const entry: TranscriptionEntry = {
      id: this.entryIdCounter++,
      speaker: speaker,
      text: text.trim(),
      timestamp: new Date(),
      confidence: confidence
    };

    this.transcriptionData.entries.push(entry);
    
    // Auto-scroll para o final
    setTimeout(() => {
      const container = document.querySelector('.transcription-list');
      if (container) {
        container.scrollTop = container.scrollHeight;
      }
    }, 50);
  }

  // Iniciar transcrição dual
  async startTranscription(): Promise<void> {
    if (!this.recognitionProfessional) {
      alert('Reconhecimento de voz não disponível neste navegador.');
      return;
    }

    try {
      // Capturar microfone (profissional) - obrigatório
      const micOk = await this.captureMicrophoneAudio();
      if (!micOk) {
        return;
      }

      // Capturar áudio remoto do Jitsi automaticamente (paciente) - opcional
      const jitsiOk = await this.captureJitsiRemoteAudio();

      this.recognitionActive = true;
      this.transcriptionData.isRecording = true;
      this.transcriptionData.isPaused = false;

      // Iniciar reconhecimento do profissional (microfone)
      this.recognitionProfessional.start();
      console.log('🎙️ Transcrição do profissional iniciada');

      // Iniciar reconhecimento do paciente (se áudio do Jitsi disponível)
      if (jitsiOk && this.recognitionPatient && this.remoteAudioDestination) {
        // Nota: Web Speech API não suporta diretamente MediaStream customizado
        // Como fallback, vamos marcar transcrições baseado no volume/timing
        console.log('✅ Monitoramento de áudio remoto ativo');
        console.log('💡 Transcrições serão detectadas pelo microfone local');
        console.log('ℹ️ Use entrada manual para fala do paciente se necessário');
      } else {
        console.log('ℹ️ Modo profissional apenas (paciente ainda não entrou)');
      }
    } catch (e) {
      console.error('Erro ao iniciar transcrição:', e);
      alert('Erro ao iniciar transcrição. Verifique as permissões.');
    }
  }

  // Pausar transcrição
  pauseTranscription(): void {
    if (this.recognitionActive) {
      this.transcriptionData.isPaused = true;
      
      if (this.recognitionProfessional) {
        this.recognitionProfessional.stop();
      }
      if (this.recognitionPatient) {
        this.recognitionPatient.stop();
      }
      
      console.log('Transcrição pausada');
    }
  }

  // Retomar transcrição
  resumeTranscription(): void {
    if (this.recognitionActive && this.transcriptionData.isPaused) {
      this.transcriptionData.isPaused = false;
      
      if (this.recognitionProfessional) {
        try {
          this.recognitionProfessional.start();
          console.log('🎙️ Transcrição do profissional retomada');
        } catch (e) {
          console.error('Erro ao retomar profissional:', e);
        }
      }
      
      if (this.recognitionPatient) {
        try {
          this.recognitionPatient.start();
          console.log('🔊 Transcrição do paciente retomada');
        } catch (e) {
          console.warn('Não foi possível retomar paciente:', e);
        }
      }
    }
  }

  // Parar transcrição
  stopTranscription(): void {
    if (this.recognitionActive) {
      this.recognitionActive = false;
      this.transcriptionData.isRecording = false;
      this.transcriptionData.isPaused = false;
      
      // Parar reconhecedores
      if (this.recognitionProfessional) {
        try {
          this.recognitionProfessional.stop();
        } catch (e) {
          // Ignorar erro se já estiver parado
        }
      }
      if (this.recognitionPatient) {
        try {
          this.recognitionPatient.stop();
        } catch (e) {
          // Ignorar erro se já estiver parado
        }
      }
      
      // Liberar streams de áudio
      if (this.microphoneStream) {
        this.microphoneStream.getTracks().forEach(track => track.stop());
        this.microphoneStream = null;
      }
      
      // Limpar AudioContext
      if (this.audioContext) {
        this.audioContext.close();
        this.audioContext = null;
        this.remoteAudioDestination = null;
      }
      
      this.jitsiAudioMonitoring = false;
      
      console.log('🛑 Transcrição parada e recursos liberados');
    }
  }

  // Limpar transcrição
  clearTranscription(): void {
    if (confirm('Deseja realmente limpar toda a transcrição?')) {
      this.transcriptionData.entries = [];
      this.entryIdCounter = 1;
    }
  }

  // Exportar transcrição para texto
  exportTranscriptionToText(): string {
    let text = '=== TRANSCRIÇÃO DA CONSULTA ===\n\n';
    text += `Paciente: ${this.patientInfo.name}\n`;
    text += `Data: ${new Date().toLocaleString('pt-BR')}\n\n`;
    text += '--- DIÁLOGO ---\n\n';

    this.transcriptionData.entries.forEach(entry => {
      const time = entry.timestamp.toLocaleTimeString('pt-BR');
      const speaker = entry.speaker === 'professional' ? 'Profissional' : 'Paciente';
      text += `[${time}] ${speaker}: ${entry.text}\n\n`;
    });

    return text;
  }

  // Download transcrição como arquivo .txt
  downloadTranscription(): void {
    const text = this.exportTranscriptionToText();
    const blob = new Blob([text], { type: 'text/plain;charset=utf-8' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `transcricao_${this.patientInfo.name.replace(/\s+/g, '_')}_${new Date().getTime()}.txt`;
    link.click();
    window.URL.revokeObjectURL(url);
  }

  // Adicionar entrada manual
  addManualEntry(speaker: 'professional' | 'patient', text: string): void {
    if (text.trim()) {
      this.addTranscriptionEntry(speaker, text, 1.0);
    }
  }

  // Remover entrada
  removeTranscriptionEntry(id: number): void {
    this.transcriptionData.entries = this.transcriptionData.entries.filter(e => e.id !== id);
  }

  // Editar entrada
  editTranscriptionEntry(id: number, newText: string): void {
    const entry = this.transcriptionData.entries.find(e => e.id === id);
    if (entry) {
      entry.text = newText;
    }
  }

  // ==========================================
  // SISTEMA DE DITADO POR VOZ PARA CAMPOS
  // ==========================================

  // Inicializar sistema de ditado por voz
  private initializeVoiceDictation(): void {
    const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    
    if (!SpeechRecognition) {
      console.warn('Web Speech API não suportada neste navegador');
      return;
    }

    this.voiceDictationRecognition = new SpeechRecognition();
    this.voiceDictationRecognition.continuous = true;
    this.voiceDictationRecognition.interimResults = true; // Habilitar resultados intermediários
    this.voiceDictationRecognition.lang = 'pt-BR';
    this.voiceDictationRecognition.maxAlternatives = 1;

    this.voiceDictationRecognition.onresult = (event: any) => {
      let interimTranscript = '';
      let finalTranscript = '';

      // Processar todos os resultados
      for (let i = event.resultIndex; i < event.results.length; i++) {
        const transcript = event.results[i][0].transcript;
        
        if (event.results[i].isFinal) {
          finalTranscript += transcript;
        } else {
          interimTranscript += transcript;
        }
      }

      // Se houver um campo focado, preenche em tempo real
      if (this.currentFocusedField && this.currentFocusedFieldId) {
        if (finalTranscript.trim()) {
          // Resultado final: adicionar permanentemente e resetar controles
          this.fillFocusedFieldWithVoice(finalTranscript.trim(), false);
          this.lastInterimText = '';
          this.baseTextBeforeDictation = '';
        } else if (interimTranscript.trim()) {
          // Resultado intermediário: substituir o anterior
          this.fillFocusedFieldWithVoice(interimTranscript.trim(), true);
        }
      }
    };

    this.voiceDictationRecognition.onerror = (event: any) => {
      console.error('Erro no ditado por voz:', event.error);
      
      if (event.error === 'no-speech') {
        // Reiniciar se não detectar fala
        if (this.isVoiceDictationActive) {
          setTimeout(() => {
            if (this.isVoiceDictationActive) {
              try {
                this.voiceDictationRecognition.start();
              } catch (e) {
                // Ignorar erro se já estiver rodando
              }
            }
          }, 100);
        }
      }
    };

    this.voiceDictationRecognition.onend = () => {
      // Reiniciar automaticamente se ainda estiver ativo
      if (this.isVoiceDictationActive) {
        try {
          this.voiceDictationRecognition.start();
        } catch (e) {
          console.error('Erro ao reiniciar ditado por voz:', e);
        }
      }
    };

    console.log('✅ Sistema de ditado por voz inicializado');
  }

  // Alternar ditado por voz
  toggleVoiceDictation(): void {
    if (this.isVoiceDictationActive) {
      this.stopVoiceDictation();
    } else {
      this.startVoiceDictation();
    }
  }

  // Iniciar ditado por voz
  private async startVoiceDictation(): Promise<void> {
    if (!this.voiceDictationRecognition) {
      alert('Sistema de ditado por voz não disponível neste navegador.');
      return;
    }

    // Solicitar permissão de microfone
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ 
        audio: {
          echoCancellation: true,
          noiseSuppression: true,
          autoGainControl: true
        } 
      });
      
      // Liberar o stream imediatamente (só precisamos da permissão)
      stream.getTracks().forEach(track => track.stop());
      
      // Iniciar reconhecimento
      this.isVoiceDictationActive = true;
      this.voiceDictationRecognition.start();
      
      console.log('🎤 Ditado por voz iniciado');
    } catch (error) {
      console.error('❌ Erro ao acessar microfone:', error);
      alert('Não foi possível acessar o microfone. Verifique as permissões.');
    }
  }

  // Parar ditado por voz
  stopVoiceDictation(): void {
    if (this.isVoiceDictationActive && this.voiceDictationRecognition) {
      this.isVoiceDictationActive = false;
      
      try {
        this.voiceDictationRecognition.stop();
      } catch (e) {
        // Ignorar erro se já estiver parado
      }
      
      console.log('🛑 Ditado por voz parado');
    }
  }

  // Configurar listeners para detectar foco nos campos
  private setupFieldFocusListeners(): void {
    // Usar evento de captura para detectar foco em qualquer elemento
    document.addEventListener('focusin', (event: FocusEvent) => {
      const target = event.target as HTMLElement;
      
      // Verificar se é um campo de formulário
      if (target && (
        target.tagName === 'INPUT' || 
        target.tagName === 'TEXTAREA' || 
        target.tagName === 'SELECT'
      )) {
        // Verificar se NÃO é campo de transcrição manual (evitar conflito)
        const isTranscriptionField = target.closest('.transcription-panel');
        if (isTranscriptionField) {
          this.currentFocusedField = null;
          this.currentFocusedFieldId = null;
          return;
        }

        // Armazenar referência ao campo focado
        this.currentFocusedField = target;
        const name = (target as HTMLInputElement).name || '';
        this.currentFocusedFieldId = target.id || name || `field_${Date.now()}`;
        
        console.log('📝 Campo focado para ditado:', this.currentFocusedFieldId, target);
      }
    }, true);

    // Detectar quando perde o foco
    document.addEventListener('focusout', (event: FocusEvent) => {
      // Manter a referência ao último campo focado
      // (ditado continua ativo, apenas não preenche se não houver foco)
      console.log('📝 Campo perdeu foco');
    }, true);
  }

  // Preencher campo focado com texto ditado
  private fillFocusedFieldWithVoice(text: string, isInterim: boolean = false): void {
    if (!this.currentFocusedField || !this.currentFocusedFieldId) {
      console.log('⚠️ Nenhum campo focado, texto ignorado:', text);
      return;
    }

    const element = this.currentFocusedField;
    
    // Verificar se é campo personalizado
    if (this.currentFocusedFieldId.startsWith('customField_')) {
      this.fillCustomFieldWithVoice(text, isInterim);
      return;
    }

    // Para todos os outros campos (SOAP, Paciente, Prescrição, Exames, etc.)
    if (element instanceof HTMLInputElement || element instanceof HTMLTextAreaElement) {
      const isTextArea = element instanceof HTMLTextAreaElement;
      const isTextInput = element instanceof HTMLInputElement && 
                         (element.type === 'text' || element.type === 'search' || !element.type);
      const isNumberInput = element instanceof HTMLInputElement && element.type === 'number';
      
      if (isTextArea || isTextInput) {
        if (isInterim) {
          // Resultado intermediário: guardar base e substituir apenas o texto intermediário
          if (!this.baseTextBeforeDictation) {
            // Primeira vez que recebe interim, salvar o texto base
            this.baseTextBeforeDictation = element.value || '';
          }
          
          // Construir novo valor: base + espaço (se necessário) + texto intermediário
          const needsSpace = this.baseTextBeforeDictation && 
                            !this.baseTextBeforeDictation.endsWith(' ') && 
                            !this.baseTextBeforeDictation.endsWith('\n');
          element.value = this.baseTextBeforeDictation + (needsSpace ? ' ' : '') + text;
          this.lastInterimText = text;
          
          console.log(`✅ Campo (interim) atualizado:`, text);
        } else {
          // Resultado final: adicionar permanentemente ao texto base
          if (this.baseTextBeforeDictation) {
            // Já tínhamos uma base, adicionar ao final dela
            const needsSpace = this.baseTextBeforeDictation && 
                              !this.baseTextBeforeDictation.endsWith(' ') && 
                              !this.baseTextBeforeDictation.endsWith('\n');
            element.value = this.baseTextBeforeDictation + (needsSpace ? ' ' : '') + text;
          } else {
            // Primeira vez sem base prévia
            const currentValue = element.value || '';
            const needsSpace = currentValue && 
                              !currentValue.endsWith(' ') && 
                              !currentValue.endsWith('\n');
            element.value = currentValue + (needsSpace ? ' ' : '') + text;
          }
          
          // Resetar controles para próxima frase
          this.baseTextBeforeDictation = '';
          this.lastInterimText = '';
          
          console.log(`✅ Campo (final) preenchido:`, text);
        }
        
        // Disparar evento input para atualizar ngModel
        element.dispatchEvent(new Event('input', { bubbles: true }));
      } else if (isNumberInput) {
        // Para campos numéricos, extrair números
        const numbers = text.match(/\d+/g);
        if (numbers && numbers.length > 0) {
          element.value = numbers[0];
          element.dispatchEvent(new Event('input', { bubbles: true }));
          console.log('✅ Campo numérico preenchido:', this.currentFocusedFieldId, '=', numbers[0]);
        }
      } else {
        // Para outros tipos de input, substituir valor
        element.value = text;
        element.dispatchEvent(new Event('input', { bubbles: true }));
        console.log('✅ Campo preenchido:', this.currentFocusedFieldId, '=', text);
      }
    }
  }

  // Preencher campo personalizado com texto ditado
  private fillCustomFieldWithVoice(text: string, isInterim: boolean = false): void {
    const fieldIdStr = this.currentFocusedFieldId!.replace('customField_', '');
    const fieldId = parseInt(fieldIdStr, 10);
    
    if (!isNaN(fieldId)) {
      // Encontrar o campo para verificar o tipo
      const field = this.customFields.find(f => f.id === fieldId);
      
      if (field) {
        // Atualizar o valor baseado no tipo do campo
        if (field.fieldType === 'textarea' || field.fieldType === 'text') {
          if (isInterim) {
            // Resultado intermediário: guardar base e substituir
            if (!this.baseTextBeforeDictation) {
              this.baseTextBeforeDictation = this.customFieldValues[fieldId] || '';
            }
            
            const needsSpace = this.baseTextBeforeDictation && !this.baseTextBeforeDictation.endsWith(' ');
            this.customFieldValues[fieldId] = this.baseTextBeforeDictation + (needsSpace ? ' ' : '') + text;
            this.lastInterimText = text;
          } else {
            // Resultado final: adicionar permanentemente
            if (this.baseTextBeforeDictation) {
              const needsSpace = this.baseTextBeforeDictation && !this.baseTextBeforeDictation.endsWith(' ');
              this.customFieldValues[fieldId] = this.baseTextBeforeDictation + (needsSpace ? ' ' : '') + text;
            } else {
              const currentValue = this.customFieldValues[fieldId] || '';
              const needsSpace = currentValue && !currentValue.endsWith(' ');
              this.customFieldValues[fieldId] = currentValue + (needsSpace ? ' ' : '') + text;
            }
            
            // Resetar controles
            this.baseTextBeforeDictation = '';
            this.lastInterimText = '';
          }
        } else if (field.fieldType === 'number') {
          // Para number, tentar extrair números do texto
          const numbers = text.match(/\d+/g);
          if (numbers && numbers.length > 0) {
            this.customFieldValues[fieldId] = parseInt(numbers[0], 10);
          }
        } else {
          // Para outros tipos, substituir valor
          this.customFieldValues[fieldId] = text;
        }
        
        // Atualizar o elemento DOM diretamente para feedback visual imediato
        if (this.currentFocusedField instanceof HTMLInputElement || 
            this.currentFocusedField instanceof HTMLTextAreaElement) {
          this.currentFocusedField.value = this.customFieldValues[fieldId]?.toString() || '';
        }
        
        console.log(`✅ Campo personalizado ${isInterim ? '(interim)' : '(final)'} preenchido:`, field.label, '=', text);
      }
    }
  }

  // ============================================
  // CADSUS - Consulta de Dados do Cidadão
  // ============================================

  consultarCadsus(): void {
    if (!this.patientInfo.cpf) {
      this.cadsusError = 'Digite um CPF para consultar';
      return;
    }

    this.isConsultingCadsus = true;
    this.cadsusError = null;
    this.cadsusSuccess = null;

    this.cadsusService.consultarCpf(this.patientInfo.cpf).subscribe({
      next: (cidadao: CadsusCidadao) => {
        console.log('✅ Dados recebidos do CADSUS:', cidadao);
        console.log('📋 patientInfo antes:', JSON.parse(JSON.stringify(this.patientInfo)));
        
        // Preencher todos os campos automaticamente
        if (cidadao.cns) this.patientInfo.cns = cidadao.cns;
        if (cidadao.cpf) this.patientInfo.cpf = cidadao.cpf;
        if (cidadao.nome) this.patientInfo.name = cidadao.nome;
        if (cidadao.dataNascimento) this.patientInfo.birthDate = this.convertDateToBrowserFormat(cidadao.dataNascimento);
        if (cidadao.statusCadastro) this.patientInfo.registrationStatus = cidadao.statusCadastro;
        if (cidadao.nomeMae) this.patientInfo.motherName = cidadao.nomeMae;
        if (cidadao.nomePai) this.patientInfo.fatherName = cidadao.nomePai;
        if (cidadao.sexo) this.patientInfo.gender = cidadao.sexo;
        if (cidadao.racaCor) this.patientInfo.raceColor = cidadao.racaCor;
        if (cidadao.tipoLogradouro) this.patientInfo.streetType = cidadao.tipoLogradouro;
        if (cidadao.logradouro) this.patientInfo.street = cidadao.logradouro;
        if (cidadao.numero) this.patientInfo.number = cidadao.numero;
        if (cidadao.complemento) this.patientInfo.complement = cidadao.complemento;
        if (cidadao.cidade) this.patientInfo.city = cidadao.cidade;
        if (cidadao.paisEnderecoAtual) this.patientInfo.country = cidadao.paisEnderecoAtual;
        if (cidadao.cep) this.patientInfo.zipCode = cidadao.cep;
        if (cidadao.enderecoCompleto) this.patientInfo.fullAddress = cidadao.enderecoCompleto;
        if (cidadao.cidadeNascimento) this.patientInfo.birthCity = cidadao.cidadeNascimento;
        if (cidadao.paisNascimento) this.patientInfo.birthCountry = cidadao.paisNascimento;
        if (cidadao.telefones && cidadao.telefones.length > 0) {
          this.patientInfo.phones = cidadao.telefones.join(', ');
        }
        if (cidadao.emails && cidadao.emails.length > 0) {
          this.patientInfo.emails = cidadao.emails.join(', ');
        }

        console.log('📋 patientInfo depois:', JSON.parse(JSON.stringify(this.patientInfo)));

        this.isConsultingCadsus = false;
        this.cadsusSuccess = `Dados carregados com sucesso do CADSUS para ${cidadao.nome}`;
        
        // Limpar mensagem de sucesso após 5 segundos
        setTimeout(() => {
          this.cadsusSuccess = null;
        }, 5000);
      },
      error: (error) => {
        console.error('Erro ao consultar CADSUS:', error);
        this.isConsultingCadsus = false;
        
        let errorMessage = 'Erro ao consultar CADSUS';
        
        if (error.status === 401 || error.status === 403) {
          errorMessage = 'Acesso negado. Verifique suas credenciais.';
        } else if (error.status === 404) {
          errorMessage = 'CPF não encontrado no CADSUS';
        } else if (error.status === 500) {
          errorMessage = error.error?.message || 'Erro no servidor. Verifique a configuração do certificado.';
        } else if (error.status === 0) {
          errorMessage = 'Erro de conexão. Verifique se o backend está rodando.';
        } else if (error.error?.message) {
          errorMessage = error.error.message;
        }
        
        this.cadsusError = errorMessage;
      }
    });
  }

  /**
   * Converte data do formato DD/MM/YYYY para YYYY-MM-DD (formato do input date do navegador)
   */
  private convertDateToBrowserFormat(dataBr: string): string {
    if (!dataBr || dataBr.length !== 10) return '';
    
    const parts = dataBr.split('/');
    if (parts.length !== 3) return '';
    
    const [day, month, year] = parts;
    return `${year}-${month}-${day}`;
  }

  close(): void {

    this.tabMode = 'hidden';
    this.onModeChange.emit(this.tabMode);
  }

  // Campos personalizados
  async loadCustomFieldsForAppointment(): Promise<void> {
    if (!this.specialtyId) {
      console.warn('Specialty ID não fornecido');
      return;
    }

    this.loadingCustomFields = true;

    // Buscar campos da especialidade
    this.specialtyService.getFields(this.specialtyId).subscribe({
      next: (fields) => {
        this.customFields = fields.sort((a, b) => a.displayOrder - b.displayOrder);
        
        // Buscar valores salvos se houver appointmentId
        if (this.appointmentId) {
          this.loadSavedFieldValues();
        } else {
          this.loadingCustomFields = false;
        }
      },
      error: (error) => {
        console.error('Erro ao carregar campos personalizados:', error);
        this.loadingCustomFields = false;
      }
    });
  }

  private loadSavedFieldValues(): void {
    this.appointmentFieldService.getFieldValues(this.appointmentId).subscribe({
      next: (values) => {
        // Mapear valores para o objeto de valores
        values.forEach(value => {
          this.customFieldValues[value.specialtyFieldId] = value.fieldValue;
        });
        this.loadingCustomFields = false;
      },
      error: (error) => {
        console.error('Erro ao carregar valores dos campos:', error);
        this.loadingCustomFields = false;
      }
    });
  }

  saveCustomFields(): void {
    const fieldValues: SaveAppointmentFieldValueDto[] = [];

    // Converter valores do objeto para array
    this.customFields.forEach(field => {
      const value = this.customFieldValues[field.id];
      if (value !== undefined && value !== null && value !== '') {
        fieldValues.push({
          specialtyFieldId: field.id,
          fieldValue: typeof value === 'string' ? value : JSON.stringify(value)
        });
      }
    });

    this.appointmentFieldService.saveFieldValues(this.appointmentId, fieldValues).subscribe({
      next: () => {
        alert('Campos salvos com sucesso!');
      },
      error: (error) => {
        console.error('Erro ao salvar campos:', error);
        alert('Erro ao salvar campos');
      }
    });
  }

  getFieldTypeLabel(type: string): string {
    const labels: { [key: string]: string } = {
      'text': 'Texto',
      'textarea': 'Texto Longo',
      'number': 'Número',
      'date': 'Data',
      'select': 'Seleção',
      'checkbox': 'Checkbox',
      'radio': 'Opções'
    };
    return labels[type] || type;
  }

  // Métodos auxiliares para cálculos no template
  getAverageHeartRate(): number {
    if (this.biometricData.heartRate.length === 0) return 0;
    const sum = this.biometricData.heartRate.reduce((a, b) => a + b, 0);
    return Math.round(sum / this.biometricData.heartRate.length);
  }

  getAverageTemperature(): number {
    if (this.biometricData.temperature.length === 0) return 0;
    const sum = this.biometricData.temperature.reduce((a, b) => a + b, 0);
    return +(sum / this.biometricData.temperature.length).toFixed(1);
  }

  getAverageOxygen(): number {
    if (this.biometricData.oxygenSaturation.length === 0) return 0;
    const sum = this.biometricData.oxygenSaturation.reduce((a, b) => a + b, 0);
    return Math.round(sum / this.biometricData.oxygenSaturation.length);
  }

  // ==========================================
  // SISTEMA DE IA MÉDICA
  // ==========================================

  generateAIAnalysis(): void {
    this.isGeneratingAI = true;
    this.aiError = null;

    // Preparar dados customizados com labels
    const customFieldsWithLabels: any = {};
    this.customFields.forEach(field => {
      const value = this.customFieldValues[field.id];
      if (value !== undefined && value !== null && value !== '') {
        customFieldsWithLabels[field.label] = value;
      }
    });

    const lastBP = this.biometricData.bloodPressure[this.biometricData.bloodPressure.length - 1];
    
    const request = {
      patientData: this.patientInfo,
      soapData: this.soapData,
      biometricData: {
        heartRate: { current: this.biometricData.heartRate[this.biometricData.heartRate.length - 1] || null },
        bloodPressure: {
          systolic: lastBP?.systolic || null,
          diastolic: lastBP?.diastolic || null
        },
        oxygenSaturation: { current: this.biometricData.oxygenSaturation[this.biometricData.oxygenSaturation.length - 1] || null },
        temperature: { current: this.biometricData.temperature[this.biometricData.temperature.length - 1] || null },
        respiratoryRate: { current: null }
      },
      customFields: customFieldsWithLabels,
      transcription: this.transcriptionData.entries,
      prescription: this.prescription,
      examRequests: this.examRequests
    };

    this.aiService.generateMedicalAnalysis(request).subscribe({
      next: (response) => {
        console.log('Resposta da IA:', response);
        if (response.choices && response.choices.length > 0) {
          const content = response.choices[0].message.content;
          this.aiAnalysisResult = this.aiService.parseAIResponse(content);
        } else {
          this.aiError = 'Resposta inválida da API. Verifique os logs do console.';
        }
        this.isGeneratingAI = false;
      },
      error: (error) => {
        console.error('Erro completo ao gerar análise de IA:', error);
        
        let errorMessage = 'Erro ao comunicar com o serviço de IA.';
        
        if (error.status === 401) {
          errorMessage = 'Token de API inválido ou expirado. Verifique a configuração da API Key.';
        } else if (error.status === 403) {
          errorMessage = 'Acesso negado. Verifique as permissões do token.';
        } else if (error.status === 429) {
          errorMessage = 'Limite de requisições excedido. Tente novamente em alguns momentos.';
        } else if (error.status === 0) {
          errorMessage = 'Erro de conexão. Verifique sua internet ou se há problemas de CORS.';
        } else if (error.error?.error?.message) {
          errorMessage = error.error.error.message;
        } else if (error.message) {
          errorMessage = error.message;
        }
        
        this.aiError = errorMessage;
        this.isGeneratingAI = false;
      }
    });
  }

  copyToClipboard(text: string): void {
    navigator.clipboard.writeText(text).then(() => {
      alert('Texto copiado para a área de transferência!');
    }).catch(err => {
      console.error('Erro ao copiar texto:', err);
      alert('Erro ao copiar texto');
    });
  }

  formatAIText(text: string): string {
    if (!text) return '';
    
    // Converter markdown bold (**texto** ou __texto__) para <strong>
    let formatted = text.replace(/\*\*([^*]+)\*\*/g, '<strong>$1</strong>');
    formatted = formatted.replace(/__([^_]+)__/g, '<strong>$1</strong>');
    
    // Converter markdown itálico (*texto* ou _texto_)
    formatted = formatted.replace(/\*([^*]+)\*/g, '<em>$1</em>');
    formatted = formatted.replace(/_([^_]+)_/g, '<em>$1</em>');
    
    // Converter quebras de linha em <br>
    formatted = formatted.replace(/\n/g, '<br>');
    
    // Converter marcadores de lista
    formatted = formatted.replace(/^- /gm, '• ');
    formatted = formatted.replace(/^\* /gm, '• ');
    formatted = formatted.replace(/^\d+\.\s/gm, (match) => `<strong>${match}</strong>`);
    
    // Destacar títulos (linhas com apenas letras maiúsculas seguidas de :)
    formatted = formatted.replace(/^([A-ZÁÀÂÃÉÊÍÓÔÕÚÇ\s]+):?<br>/gm, '<strong>$1:</strong><br>');
    
    // Converter títulos markdown (### Título)
    formatted = formatted.replace(/###\s*([^<]+)<br>/g, '<strong style="font-size: 1.1em;">$1</strong><br>');
    formatted = formatted.replace(/##\s*([^<]+)<br>/g, '<strong style="font-size: 1.2em;">$1</strong><br>');
    formatted = formatted.replace(/#\s*([^<]+)<br>/g, '<strong style="font-size: 1.3em;">$1</strong><br>');
    
    return formatted;
  }
}

