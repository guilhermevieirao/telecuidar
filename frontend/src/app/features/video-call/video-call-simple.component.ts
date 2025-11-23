import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

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
  selector: 'app-video-call-simple',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './video-call-simple.component.html',
  styleUrls: ['./video-call-simple.component.scss']
})
export class VideoCallSimpleComponent implements OnInit, OnDestroy, AfterViewInit {
  roomName: string = 'app-Room-' + Math.random().toString(36).substr(2, 9);
  jitsiUrl: SafeResourceUrl = '';
  isLoading = true;
  errorMessage = '';
  showControls = false;
  showMedicalRecord = true;
  activeTab: 'specialty' | 'patient' | 'anamnesis' | 'exam' | 'diagnosis' = 'specialty';
  selectedSpecialty: string = '';
  specialtyData: { [key: string]: any } = {};
  private jitsiApi: any = null;

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

  constructor(private sanitizer: DomSanitizer) {}

  ngOnInit(): void {
    this.checkBrowserSupport();
  }

  ngAfterViewInit(): void {
    // Aguardar o container estar pronto antes de carregar o Jitsi
    setTimeout(() => {
      this.loadJitsiScript();
    }, 100);
  }

  ngOnDestroy(): void {
    if (this.jitsiApi) {
      this.jitsiApi.dispose();
      this.jitsiApi = null;
    }
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
      configOverwrite: {
        prejoinPageEnabled: true,
        defaultLanguage: 'pt',
        // Configurações de transcrição/legendas
        transcription: {
          enabled: true,
          preferredLanguage: 'pt',
          translationLanguages: ['pt'],
          useAppLanguage: true
        },
        transcribingEnabled: true,
        // Habilitar chat
        disableChat: false,
        // Outras configurações
        startWithAudioMuted: false,
        startWithVideoMuted: false,
        disableDeepLinking: true,
        enableClosedCaptions: true,
        // Botões adicionais
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
      });

      this.jitsiApi.addEventListener('videoConferenceLeft', () => {
        console.log('Usuário saiu da conferência');
      });

      // Simular fim do loading após 3 segundos se não houver evento
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



  private checkBrowserSupport(): void {
    const hasGetUserMedia = !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia);
    const hasRTCPeerConnection = !!(window as any).RTCPeerConnection;
    
    if (!hasGetUserMedia || !hasRTCPeerConnection) {
      this.errorMessage = 'Seu navegador não suporta videochamada. Use Chrome, Firefox, Safari ou Edge.';
      this.isLoading = false;
    }
  }

  copyRoomLink(): void {
    const roomLink = `https://jitsi.riot.im/${this.roomName}`;
    
    navigator.clipboard.writeText(roomLink).then(() => {
      alert('Link da sala copiado para a área de transferência!');
    }).catch(err => {
      console.error('Erro ao copiar link:', err);
      // Fallback
      const textArea = document.createElement('textarea');
      textArea.value = roomLink;
      document.body.appendChild(textArea);
      textArea.select();
      document.execCommand('copy');
      document.body.removeChild(textArea);
      alert('Link da sala copiado para a área de transferência!');
    });
  }

  openInNewTab(): void {
    const roomUrl = `https://jitsi.riot.im/${this.roomName}`;
    window.open(roomUrl, '_blank');
  }

  refreshRoom(): void {
    this.roomName = 'app-Room-' + Math.random().toString(36).substr(2, 9);
    if (this.jitsiApi) {
      this.jitsiApi.dispose();
    }
    this.isLoading = true;
    setTimeout(() => this.initializeJitsi(), 500);
  }

  getSafeUrl() {
    const configOverwrite = {
      toolbarButtons: [
        'camera', 'chat', 'desktop', 'download', 'embedmeeting', 'etherpad', 
        'feedback', 'filmstrip', 'fullscreen', 'hangup', 'invite', 'livestreaming',
        'microphone', 'mute-everyone', 'mute-video-everyone', 'participants-pane',
        'profile', 'raisehand', 'recording', 'security', 'select-background',
        'settings', 'shareaudio', 'sharedvideo', 'shortcuts', 'stats', 'tileview',
        'toggle-camera', 'videoquality', '__end'
      ]
    };

    const configParams = new URLSearchParams({
      'config.prejoinPageEnabled': 'true',
      'config.toolbarButtons': JSON.stringify(configOverwrite.toolbarButtons),
      'config.toolbarConfig.alwaysVisible': 'false',
      'interfaceConfig.TOOLBAR_BUTTONS': JSON.stringify(configOverwrite.toolbarButtons)
    });

    const roomUrl = `https://jitsi.riot.im/${this.roomName}#config.prejoinPageEnabled=true&${configParams.toString()}`;
    return this.sanitizer.bypassSecurityTrustResourceUrl(roomUrl);
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
    alert('Prontuário salvo com sucesso!');
    // Aqui você implementaria a lógica para salvar no backend
  }

  printMedicalRecord(): void {
    window.print();
  }

  clearMedicalRecord(): void {
    if (confirm('Deseja realmente limpar todos os dados do prontuário?')) {
      this.medicalRecord = {
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
    }
  }
}