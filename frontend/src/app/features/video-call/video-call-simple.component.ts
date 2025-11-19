import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

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
export class VideoCallSimpleComponent implements OnInit, OnDestroy {
  roomName: string = 'app-Room-' + Math.random().toString(36).substr(2, 9);
  jitsiUrl: SafeResourceUrl = '';
  isLoading = true;
  errorMessage = '';
  showControls = false;
  showMedicalRecord = true;
  activeTab: 'patient' | 'anamnesis' | 'exam' | 'diagnosis' = 'patient';

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
    this.generateRoomUrl();
    this.checkBrowserSupport();
  }

  ngOnDestroy(): void {
    // Limpar recursos se necessário
  }

  private generateRoomUrl(): void {
    // Configuração do Jitsi com toolbar personalizada
    const configOverwrite = {
      toolbarButtons: [
        'camera', 'chat', 'desktop', 'download', 'embedmeeting', 'etherpad', 
        'feedback', 'filmstrip', 'fullscreen', 'hangup', 'invite', 'livestreaming',
        'microphone', 'mute-everyone', 'mute-video-everyone', 'participants-pane',
        'profile', 'raisehand', 'recording', 'security', 'select-background',
        'settings', 'shareaudio', 'sharedvideo', 'shortcuts', 'stats', 'tileview',
        'toggle-camera', 'videoquality', '__end'
      ],
      toolbarConfig: {
        alwaysVisible: false
      },
      disableDeepLinking: true,
      disableInviteFunctions: false,
      enableWelcomePage: false,
      prejoinPageEnabled: true,
      prejoinConfig: {
        enabled: true
      }
    };

    // Gerar URL com configurações para o servidor jitsi.riot.im
    const configParams = new URLSearchParams({
      'config.prejoinPageEnabled': 'true',
      'config.toolbarButtons': JSON.stringify(configOverwrite.toolbarButtons),
      'config.toolbarConfig.alwaysVisible': 'false',
      'interfaceConfig.TOOLBAR_BUTTONS': JSON.stringify(configOverwrite.toolbarButtons),
      'interfaceConfig.SHOW_JITSI_WATERMARK': 'false',
      'interfaceConfig.SHOW_WATERMARK_FOR_GUESTS': 'false'
    });

    const roomUrl = `https://jitsi.riot.im/${this.roomName}#config.prejoinPageEnabled=true&${configParams.toString()}`;
    this.jitsiUrl = this.sanitizer.bypassSecurityTrustResourceUrl(roomUrl);
    
    console.log('URL Jitsi gerada com configurações:', roomUrl);
    
    // Simular carregamento
    setTimeout(() => {
      this.isLoading = false;
      this.showControls = true;
    }, 1500);
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

    const roomLink = `https://jitsi.riot.im/${this.roomName}#config.prejoinPageEnabled=true&${configParams.toString()}`;
    
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
    window.open(roomUrl, '_blank');
  }

  refreshRoom(): void {
    this.roomName = 'app-Room-' + Math.random().toString(36).substr(2, 9);
    this.generateRoomUrl();
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

  setActiveTab(tab: 'patient' | 'anamnesis' | 'exam' | 'diagnosis'): void {
    this.activeTab = tab;
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