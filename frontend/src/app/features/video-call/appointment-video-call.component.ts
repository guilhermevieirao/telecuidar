import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AppointmentService, Appointment } from '../../core/services/appointment.service';
import { ModalService } from '../../core/services/modal.service';
import { MedicalRecordTabsComponent } from './medical-record-tabs/medical-record-tabs.component';
import { environment } from '../../../environments/environment';

declare var JitsiMeetExternalAPI: any;

@Component({
  selector: 'app-appointment-video-call',
  standalone: true,
  imports: [CommonModule, FormsModule, MedicalRecordTabsComponent],
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
  jitsiApi: any = null; // Tornar público para passar ao componente filho
  user: any = null;
  appointmentId: number = 0;
  hasSidebar: boolean = true; // Controlar classe para redimensionar Jitsi

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
    if (typeof JitsiMeetExternalAPI !== 'undefined') {
      this.initializeJitsi();
      return;
    }
    const urls = [
      'http://localhost:8000/external_api.js',
      environment.jitsiExternalApiUrl,
      'https://localhost:8443/external_api.js'
    ];
    const tryLoad = (index: number) => {
      if (index >= urls.length) {
        this.errorMessage = 'Erro ao carregar o Jitsi Meet. Verifique sua conexão.';
        this.isLoading = false;
        return;
      }
      const script = document.createElement('script');
      script.src = urls[index];
      script.async = true;
      script.onload = () => {
        this.initializeJitsi();
      };
      script.onerror = () => {
        tryLoad(index + 1);
      };
      document.head.appendChild(script);
    };
    tryLoad(0);
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

    const domain = environment.jitsiDomain || 'localhost:8000';
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
        disableChat: false,
        startWithAudioMuted: false,
        startWithVideoMuted: false,
        disableDeepLinking: true,
        HIDE_PREJOIN_LOGO: true,
        toolbarButtons: [
          'camera',
          'chat',
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
        SHOW_BRAND_WATERMARK: false,
        SHOW_POWERED_BY: false,
        HIDE_INVITE_MORE_HEADER: false,
        DISABLE_VIDEO_BACKGROUND: false,
        DEFAULT_LOGO_URL: '',
        DEFAULT_WELCOME_PAGE_LOGO_URL: ''
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

  onMedicalRecordModeChange(mode: string): void {
    this.hasSidebar = mode === 'sidebar';
  }
}
