// ...existing code...
import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AppointmentService, Appointment } from '../../core/services/appointment.service';
import { ModalService } from '../../services/modal.service';

declare var JitsiMeetExternalAPI: any;

interface MedicalRecord {
  patientName: string;
  patientCPF: string;
  patientAge: number | null;
  patientGender: string;
  chiefComplaint: string;
  symptoms: string;
  medicalHistory: string;
  currentMedications: string;
  allergies: string;
  vitalSigns: {
    bloodPressure: string;
    heartRate: string;
    temperature: string;
    oxygenSaturation: string;
  };
  physicalExam: string;
  diagnosis: string;
  treatment: string;
  prescription: string;
  observations: string;
  followUp: string;
  consultationDate: string;
  consultationTime: string;
}

@Component({
  selector: 'app-appointment-video-call',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './appointment-video-call.component.html',
  styleUrls: ['./appointment-video-call.component.scss']
})
export class AppointmentVideoCallComponent implements OnInit, OnDestroy, AfterViewInit {
    showHeader = true;
    toggleHeader() {
      this.showHeader = !this.showHeader;
    }
  appointment: Appointment | null = null;
  roomName: string = '';
  isLoading = true;
  errorMessage = '';
  showControls = false;
  showMedicalRecord = false;
  activeTab: 'specialty' | 'patient' | 'anamnesis' | 'exam' | 'diagnosis' = 'patient';
  selectedSpecialty: string = '';
  specialtyData: { [key: string]: any } = {};
  private jitsiApi: any = null;
  user: any = null;
  appointmentId: number = 0;

  specialties = [
    { id: 'cardiology', name: 'Cardiologia', icon: '❤️', description: 'Avaliação cardiovascular' },
    { id: 'dermatology', name: 'Dermatologia', icon: '🩹', description: 'Análise de pele' },
    { id: 'pediatrics', name: 'Pediatria', icon: '👶', description: 'Atendimento infantil' },
    { id: 'orthopedics', name: 'Ortopedia', icon: '🦴', description: 'Sistema músculo-esquelético' },
    { id: 'general', name: 'Clínico Geral', icon: '🏥', description: 'Atendimento geral' }
  ];

  specialtyFields: any = {
    cardiology: [
      { key: 'chestPain', label: 'Dor no peito', type: 'text' },
      { key: 'palpitations', label: 'Palpitações', type: 'text' },
      { key: 'ecgNotes', label: 'Notas do ECG', type: 'textarea' }
    ],
    dermatology: [
      { key: 'lesionType', label: 'Tipo de lesão', type: 'text' },
      { key: 'lesionLocation', label: 'Localização da lesão', type: 'text' },
      { key: 'skinPhotos', label: 'Fotos anexadas', type: 'text' }
    ],
    pediatrics: [
      { key: 'birthDate', label: 'Data de nascimento', type: 'date' },
      { key: 'vaccinations', label: 'Vacinações', type: 'textarea' },
      { key: 'developmentNotes', label: 'Desenvolvimento', type: 'textarea' }
    ],
    orthopedics: [
      { key: 'injuryType', label: 'Tipo de lesão', type: 'text' },
      { key: 'mobilityNotes', label: 'Mobilidade', type: 'textarea' },
      { key: 'xrayNotes', label: 'Notas do Raio-X', type: 'textarea' }
    ],
    general: []
  };

  medicalRecord: MedicalRecord = {
    patientName: '',
    patientCPF: '',
    patientAge: null,
    patientGender: '',
    chiefComplaint: '',
    symptoms: '',
    medicalHistory: '',
    currentMedications: '',
    allergies: '',
    vitalSigns: {
      bloodPressure: '',
      heartRate: '',
      temperature: '',
      oxygenSaturation: ''
    },
    physicalExam: '',
    diagnosis: '',
    treatment: '',
    prescription: '',
    observations: '',
    followUp: '',
    consultationDate: new Date().toISOString().split('T')[0],
    consultationTime: new Date().toTimeString().split(' ')[0].substring(0, 5)
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private appointmentService: AppointmentService,
    private modalService: ModalService
  ) {}

  ngOnInit(): void {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      this.user = JSON.parse(userStr);
      // Mostrar formulário médico apenas para profissionais
      this.showMedicalRecord = this.user?.role === 2;
    }

    this.route.params.subscribe(params => {
      this.appointmentId = +params['id'];
      this.loadAppointment();
    });
  }

  ngAfterViewInit(): void {
    // Aguardar o container estar pronto antes de carregar o Jitsi
    setTimeout(() => {
      if (this.roomName) {
        this.loadJitsiScript();
      }
    }, 100);
  }

  ngOnDestroy(): void {
    if (this.jitsiApi) {
      this.jitsiApi.dispose();
      this.jitsiApi = null;
    }
  }

  private loadAppointment(): void {
    // Buscar a consulta específica
    const isPatient = this.user?.role === 1;
    const serviceMethod = isPatient 
      ? this.appointmentService.getMyAppointments(true)
      : this.appointmentService.getMyProfessionalAppointments(true);

    serviceMethod.subscribe({
      next: (appointments) => {
        this.appointment = appointments.find(a => a.id === this.appointmentId) || null;
        
        if (!this.appointment) {
          this.errorMessage = 'Consulta não encontrada.';
          this.isLoading = false;
          return;
        }

        if (!this.appointment.meetingRoomId) {
          this.errorMessage = 'Esta consulta não possui uma sala de videochamada associada.';
          this.isLoading = false;
          return;
        }

        // Validar se a consulta não está cancelada
        if (this.appointment.status === 'Cancelado') {
          this.errorMessage = 'Esta consulta foi cancelada.';
          this.isLoading = false;
          return;
        }

        this.roomName = this.appointment.meetingRoomId;
        
        // Preencher dados do paciente no formulário médico
        if (this.user?.role === 2) {
          this.medicalRecord.patientName = this.appointment.patientName;
          this.medicalRecord.consultationDate = this.appointment.appointmentDate.split('T')[0];
          this.medicalRecord.consultationTime = this.formatTime(this.appointment.appointmentTime);
        }
        
        this.loadJitsiScript();
      },
      error: (err) => {
        console.error('Erro ao carregar consulta:', err);
        this.errorMessage = 'Erro ao carregar informações da consulta.';
        this.isLoading = false;
      }
    });
  }

  private loadJitsiScript(): void {
    // Verificar se o script já está carregado
    if (typeof JitsiMeetExternalAPI !== 'undefined') {
      this.initializeJitsi();
      return;
    }

    // Carregar o script da API do Jitsi
    const script = document.createElement('script');
    script.src = 'https://jitsi.riot.im/external_api.js';
    script.async = true;
    script.onload = () => {
      this.initializeJitsi();
    };
    script.onerror = () => {
      this.errorMessage = 'Erro ao carregar o Jitsi Meet. Verifique sua conexão.';
      this.isLoading = false;
    };
    document.head.appendChild(script);
  }

  private initializeJitsi(): void {
    const container = document.getElementById('jitsi-meet-container');
    
    if (!container) {
      console.error('Container jitsi-meet-container não encontrado');
      this.errorMessage = 'Erro: Container de vídeo não encontrado.';
      this.isLoading = false;
      return;
    }

    if (typeof JitsiMeetExternalAPI === 'undefined') {
      console.error('JitsiMeetExternalAPI não está definido');
      this.errorMessage = 'Erro: API do Jitsi não carregada.';
      this.isLoading = false;
      return;
    }

    const domain = 'jitsi.riot.im';
    const options = {
      roomName: this.roomName,
      width: '100%',
      height: '100%',
      parentNode: container,
      lang: 'pt',
      userInfo: {
        displayName: this.user?.fullName || 'Participante'
      },
      configOverwrite: {
        prejoinPageEnabled: true,
        defaultLanguage: 'pt',
        transcription: {
          enabled: true,
          preferredLanguage: 'pt',
          translationLanguages: ['pt'],
          useAppLanguage: true
        },
        transcribingEnabled: true,
        disableChat: false,
        startWithAudioMuted: false,
        startWithVideoMuted: false,
        disableDeepLinking: true,
        enableClosedCaptions: true,
        toolbarButtons: [
          'camera',
          'chat',
          'closedcaptions',
          'desktop',
          'download',
          'embedmeeting',
          'filmstrip',
          'fullscreen',
          'hangup',
          'invite',
          'microphone',
          'participants-pane',
          'profile',
          'raisehand',
          'recording',
          'security',
          'select-background',
          'settings',
          'shareaudio',
          'sharedvideo',
          'shortcuts',
          'stats',
          'tileview',
          'videoquality'
        ]
      },
      interfaceConfigOverwrite: {
        TOOLBAR_BUTTONS: [
          'camera',
          'chat',
          'closedcaptions',
          'desktop',
          'download',
          'embedmeeting',
          'filmstrip',
          'fullscreen',
          'hangup',
          'invite',
          'microphone',
          'participants-pane',
          'profile',
          'raisehand',
          'recording',
          'security',
          'select-background',
          'settings',
          'shareaudio',
          'sharedvideo',
          'shortcuts',
          'stats',
          'tileview',
          'videoquality'
        ],
        SHOW_JITSI_WATERMARK: false,
        SHOW_WATERMARK_FOR_GUESTS: false,
        HIDE_INVITE_MORE_HEADER: false,
        DISABLE_VIDEO_BACKGROUND: false
      }
    };

    try {
      console.log('Inicializando Jitsi com sala:', this.roomName);
      this.jitsiApi = new JitsiMeetExternalAPI(domain, options);
      
      this.jitsiApi.addEventListener('videoConferenceJoined', () => {
        console.log('Usuário entrou na conferência');
        this.isLoading = false;
        this.showControls = true;
      });

      this.jitsiApi.addEventListener('readyToClose', () => {
        console.log('Conferência encerrada');
        this.goBack();
      });

      this.jitsiApi.addEventListener('videoConferenceLeft', () => {
        console.log('Usuário saiu da conferência');
        this.goBack();
      });

      setTimeout(() => {
        if (this.isLoading) {
          this.isLoading = false;
          this.showControls = true;
        }
      }, 3000);
    } catch (error) {
      console.error('Erro ao inicializar Jitsi:', error);
      this.errorMessage = 'Erro ao inicializar videochamada: ' + (error as Error).message;
      this.isLoading = false;
    }
  }

  goBack(): void {
    const isPatient = this.user?.role === 1;
    const route = isPatient ? '/minhas-consultas' : '/consultas-profissional';
    this.router.navigate([route]);
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleDateString('pt-BR', { 
      day: '2-digit', 
      month: 'long', 
      year: 'numeric' 
    });
  }

  formatTime(timeStr: string): string {
    return timeStr.substring(0, 5);
  }

  toggleMedicalRecord(): void {
    this.showMedicalRecord = !this.showMedicalRecord;
  }

  setActiveTab(tab: 'specialty' | 'patient' | 'anamnesis' | 'exam' | 'diagnosis'): void {
    this.activeTab = tab;
  }

  selectSpecialty(specialtyId: string): void {
    this.selectedSpecialty = specialtyId;
    this.activeTab = 'patient';
  }

  getSpecialtyFields(): any[] {
    return this.specialtyFields[this.selectedSpecialty] || [];
  }

  getSpecialtyName(): string {
    const specialty = this.specialties.find(s => s.id === this.selectedSpecialty);
    return specialty ? specialty.name : '';
  }

  saveMedicalRecord(): void {
    console.log('Prontuário salvo:', this.medicalRecord);
    console.log('Dados específicos da especialidade:', this.specialtyData);
    this.modalService.showSuccess('Prontuário salvo com sucesso!');
    // Aqui você implementaria a lógica para salvar no backend
  }

  printMedicalRecord(): void {
    window.print();
  }

  clearMedicalRecord(): void {
    if (confirm('Deseja realmente limpar todos os dados do prontuário?')) {
      const patientName = this.medicalRecord.patientName;
      const consultationDate = this.medicalRecord.consultationDate;
      const consultationTime = this.medicalRecord.consultationTime;
      
      this.medicalRecord = {
        patientName: patientName, // Manter nome do paciente
        patientCPF: '',
        patientAge: null,
        patientGender: '',
        chiefComplaint: '',
        symptoms: '',
        medicalHistory: '',
        currentMedications: '',
        allergies: '',
        vitalSigns: {
          bloodPressure: '',
          heartRate: '',
          temperature: '',
          oxygenSaturation: ''
        },
        physicalExam: '',
        diagnosis: '',
        treatment: '',
        prescription: '',
        observations: '',
        followUp: '',
        consultationDate: consultationDate,
        consultationTime: consultationTime
      };
      this.specialtyData = {};
    }
  }
}
