import { Component, OnInit, OnDestroy, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';

declare var JitsiMeetExternalAPI: any;

@Component({
  selector: 'app-video-call',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './video-call.component.html',
  styleUrls: ['./video-call.component.scss']
})
export class VideoCallComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('jitsiContainer', { static: true }) jitsiContainer!: ElementRef;
  
  private api: any;
  private domain: string = 'meet.jit.si'; // Servidor Jitsi público
  roomName: string = 'app-Room-' + Math.random().toString(36).substr(2, 9);
  private options: any;
  private isInitialized = false;
  private maxRetries = 3;
  private currentRetries = 0;
  
  isLoading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.loadJitsiScript();
  }

  ngAfterViewInit(): void {
    // Verificar se o script já está carregado antes de tentar inicializar
    if (this.isScriptLoaded()) {
      this.initializeJitsi();
    } else {
      // Se não estiver carregado, aguardar um pouco mais
      setTimeout(() => {
        if (this.isScriptLoaded()) {
          this.initializeJitsi();
        } else {
          // Se ainda não estiver carregado, tentar novamente
          setTimeout(() => {
            this.initializeJitsi();
          }, 1000);
        }
      }, 500);
    }
  }

  ngOnDestroy(): void {
    if (this.api) {
      this.api.dispose();
    }
  }

  private loadJitsiScript(): void {
    if (this.isScriptLoaded()) {
      // Script já está carregado, pode inicializar
      return;
    }

    const script = document.createElement('script');
    script.src = 'https://meet.jit.si/external_api.js';
    script.async = true;
    script.onload = () => {
      console.log('Jitsi script carregado com sucesso');
      // Aguardar um pouco para garantir que a biblioteca está totalmente carregada
      setTimeout(() => {
        this.initializeJitsi();
      }, 500);
    };
    script.onerror = () => {
      console.error('Erro ao carregar script Jitsi');
      this.errorMessage = 'Erro ao carregar a biblioteca Jitsi Meet';
      this.isLoading = false;
    };
    document.head.appendChild(script);
  }

  private isScriptLoaded(): boolean {
    return typeof (window as any).JitsiMeetExternalAPI !== 'undefined';
  }

  initializeJitsi(): void {
    if (this.isInitialized) {
      return; // Evitar múltiplas inicializações
    }

    if (this.currentRetries >= this.maxRetries) {
      this.errorMessage = 'Não foi possível carregar a videochamada após várias tentativas';
      this.isLoading = false;
      return;
    }

    // Verificar suporte do navegador
    if (!this.isBrowserSupported()) {
      this.errorMessage = 'Seu navegador não suporta videochamada. Use Chrome, Firefox, Safari ou Edge.';
      this.isLoading = false;
      return;
    }

    this.currentRetries++;

    if (!(window as any).JitsiMeetExternalAPI) {
      return;
    }

    // Verificar se o elemento DOM está disponível
    if (!this.jitsiContainer || !this.jitsiContainer.nativeElement) {
      console.error('Elemento Jitsi não encontrado no DOM');
      this.errorMessage = 'Erro: Elemento de vídeo não encontrado';
      this.isLoading = false;
      return;
    }

    this.isInitialized = true; // Marcar como inicializado

    this.options = {
      roomName: this.roomName,
      width: '100%',
      height: '100%',
      parentNode: this.jitsiContainer.nativeElement,
      configOverwrite: {
        startWithAudioMuted: false,
        startWithVideoMuted: false,
        enableWelcomePage: false,
        prejoinPageEnabled: false,
        disableInviteFunctions: false,
        toolbarButtons: [
          'microphone', 'camera', 'desktop', 'fullscreen',
          'fodeviceselection', 'hangup', 'chat', 'recording',
          'settings', 'raisehand', 'videoquality', 'tileview',
          'download', 'help', 'mute-everyone'
        ]
      },
      interfaceConfigOverwrite: {
        SHOW_JITSI_WATERMARK: false,
        SHOW_WATERMARK_FOR_GUESTS: false,
        SHOW_BRAND_WATERMARK: false,
        TOOLBAR_ALWAYS_VISIBLE: true,
        FILM_STRIP_MAX_HEIGHT: 120,
        VIDEO_LAYOUT_FIT: 'both',
        MOBILE_APP_PROMO: false,
        TILE_VIEW_MAX_COLUMNS: 4
      },
      userInfo: {
        displayName: 'Usuário ' + Math.random().toString(36).substr(2, 5)
      }
    };

    try {
      console.log('Inicializando Jitsi com opções:', this.options);
      this.api = new JitsiMeetExternalAPI(this.domain, this.options);
      
      console.log('Jitsi API criada com sucesso');
      
      this.api.addEventListeners({
        readyToClose: this.handleClose,
        participantJoined: this.handleParticipantJoined,
        participantLeft: this.handleParticipantLeft,
        videoConferenceJoined: this.handleConferenceJoined,
        videoConferenceLeft: this.handleConferenceLeft
      });

      console.log('Event listeners adicionados');
      this.isLoading = false;
    } catch (error) {
      console.error('Erro ao inicializar Jitsi:', error);
      this.errorMessage = 'Erro ao inicializar a videochamada. Por favor, recarregue a página.';
      this.isLoading = false;
      this.isInitialized = false; // Permitir tentar novamente
    }
  }

  private handleClose = (): void => {
    if (this.api) {
      this.api.dispose();
    }
  }

  private handleParticipantJoined = (participant: any): void => {
    console.log('Participante entrou:', participant.displayName);
  }

  private handleParticipantLeft = (participant: any): void => {
    console.log('Participante saiu:', participant.displayName);
  }

  private handleConferenceJoined = (): void => {
    console.log('Conferência iniciada com sucesso');
  }

  private handleConferenceLeft = (): void => {
    console.log('Conferência encerrada');
  }

  joinRoom(): void {
    if (this.api) {
      this.api.executeCommand('displayName', 'Novo Usuário');
    }
  }

  leaveRoom(): void {
    if (this.api) {
      this.api.executeCommand('hangup');
    }
  }

  toggleAudio(): void {
    if (this.api) {
      this.api.executeCommand('toggleAudio');
    }
  }

  toggleVideo(): void {
    if (this.api) {
      this.api.executeCommand('toggleVideo');
    }
  }

  copyRoomLink(): void {
    const roomLink = `https://meet.jit.si/${this.roomName}`;
    navigator.clipboard.writeText(roomLink).then(() => {
      // Mostrar feedback visual (poderia usar um toast aqui)
      console.log('Link copiado:', roomLink);
      alert('Link da sala copiado para a área de transferência!');
    }).catch(err => {
      console.error('Erro ao copiar link:', err);
      // Fallback para navegadores antigos
      const textArea = document.createElement('textarea');
      textArea.value = roomLink;
      document.body.appendChild(textArea);
      textArea.select();
      document.execCommand('copy');
      document.body.removeChild(textArea);
      alert('Link da sala copiado para a área de transferência!');
    });
  }

  private isBrowserSupported(): boolean {
    // Verificar suporte para WebRTC e outras APIs necessárias
    const hasGetUserMedia = !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia);
    const hasRTCPeerConnection = !!(window as any).RTCPeerConnection;
    const hasWebSocket = !!(window as any).WebSocket;
    
    return hasGetUserMedia && hasRTCPeerConnection && hasWebSocket;
  }
}