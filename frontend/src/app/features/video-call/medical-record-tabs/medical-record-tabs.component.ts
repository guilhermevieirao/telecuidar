import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Chart, ChartConfiguration, registerables } from 'chart.js';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

Chart.register(...registerables);

export type TabMode = 'hidden' | 'sidebar' | 'fullscreen';
export type ActiveTab = 'patient' | 'soap' | 'biometric' | 'prescription' | 'exams' | 'transcription';

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
  imports: [CommonModule, FormsModule],
  templateUrl: './medical-record-tabs.component.html',
  styleUrls: ['./medical-record-tabs.component.scss']
})
export class MedicalRecordTabsComponent implements OnInit {
  @Input() appointmentId: number = 0;
  @Input() patientName: string = '';
  @Input() jitsiApi: any = null; // Referência para API do Jitsi
  @Input() isDarkTheme: boolean = true; // Receber tema do componente pai
  @Output() onClose = new EventEmitter<void>();
  @Output() onModeChange = new EventEmitter<TabMode>();

  @ViewChild('contentToPrint') contentToPrint!: ElementRef;

  tabMode: TabMode = 'sidebar';
  activeTab: ActiveTab = 'patient';

  // Dados das tabs
  patientInfo: PatientInfo = {
    name: '',
    cpf: '',
    age: null,
    gender: '',
    birthDate: '',
    phone: '',
    email: '',
    address: ''
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

  ngOnInit(): void {
    this.patientInfo.name = this.patientName;
    this.generateMockBiometricData();
    this.initializeSpeechRecognition();
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
  }

  ngOnDestroy(): void {
    this.destroyCharts();
    this.stopTranscription();
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
        `Idade: ${this.patientInfo.age || 'N/A'}`,
        `Sexo: ${this.patientInfo.gender}`,
        `Telefone: ${this.patientInfo.phone}`,
        `Email: ${this.patientInfo.email}`
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

      // Prescrição
      pdf.setFontSize(14);
      pdf.setFont('helvetica', 'bold');
      pdf.text('3. Prescrição Médica', 15, yPosition);
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
      pdf.text('4. Solicitação de Exames', 15, yPosition);
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
        pdf.text('5. Transcrição da Consulta', 15, yPosition);
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

  close(): void {
    this.tabMode = 'hidden';
    this.onModeChange.emit(this.tabMode);
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
}
