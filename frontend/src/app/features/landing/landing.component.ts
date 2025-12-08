import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { ThemeToggleComponent } from '../../shared/components/atoms/theme-toggle/theme-toggle.component';
import { MobileMenu, MenuItem } from '../../shared/components/organisms/mobile-menu/mobile-menu';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink, ThemeToggleComponent, MobileMenu],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss']
})
export class LandingComponent implements OnInit {
  isLoggedIn = false;
  userName = '';
  showNavbar = true;
  isInHeroSection = true;
  lastScrollTop = 0;
  scrollThreshold = 100;
  menuItems: MenuItem[] = [];

  constructor(private router: Router) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    const userStr = localStorage.getItem('user');
    
    if (token && userStr) {
      this.isLoggedIn = true;
      const user = JSON.parse(userStr);
      this.userName = user.firstName || 'Usuário';
    }

    this.setupMenu();
  }

  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  setupMenu(): void {
    if (this.isLoggedIn) {
      this.menuItems = [
        { label: 'Início', icon: '🏠', action: () => this.scrollToTop() },
        { label: 'Nossa Missão', icon: '🎯', action: () => this.scrollToSection('sobre') },
        { label: 'Tecnologia', icon: '💻', action: () => this.scrollToSection('tecnologia') },
        { label: 'Solução', icon: '💡', action: () => this.scrollToSection('solucao') },
        { label: 'Impacto', icon: '📈', action: () => this.scrollToSection('impacto') },
        { divider: true },
        { label: 'Acessar Painel', icon: '📊', route: '/painel' }
      ];
    } else {
      this.menuItems = [
        { label: 'Início', icon: '🏠', action: () => this.scrollToTop() },
        { label: 'Nossa Missão', icon: '🎯', action: () => this.scrollToSection('sobre') },
        { label: 'Tecnologia', icon: '💻', action: () => this.scrollToSection('tecnologia') },
        { label: 'Solução', icon: '💡', action: () => this.scrollToSection('solucao') },
        { label: 'Impacto', icon: '📈', action: () => this.scrollToSection('impacto') },
        { divider: true },
        { label: 'Entrar', icon: '🔐', route: '/entrar' },
        { label: 'Criar Conta', icon: '✨', route: '/cadastro' }
      ];
    }
  }
  features = [
    {
      icon: '🏥',
      title: 'Telemedicina Híbrida',
      description: 'Consultas especializadas com suporte de IA, dispositivos IoT e profissionais qualificados.'
    },
    {
      icon: '🤖',
      title: 'Inteligência Artificial',
      description: 'Análise de dados em tempo real, detecção de padrões e sugestões de diagnóstico diferencial.'
    },
    {
      icon: '📱',
      title: 'App Pessoal de Saúde',
      description: 'Histórico médico completo, agendamentos e acompanhamento na palma da sua mão.'
    },
    {
      icon: '🔒',
      title: 'Segurança Total',
      description: 'Conformidade com LGPD, criptografia de ponta e assinatura digital certificada.'
    },
    {
      icon: '⚡',
      title: 'Sem Filas',
      description: 'Agendamento inteligente que reduz drasticamente o tempo de espera para consultas.'
    },
    {
      icon: '🌍',
      title: 'Acesso Universal',
      description: 'Atendimento especializado para áreas remotas, quebrando barreiras geográficas.'
    }
  ];

  technologies = [
    {
      icon: '💉',
      title: 'Dispositivos Biométricos',
      items: [
        'Estetoscópios digitais de alta precisão',
        'Monitores de pressão arterial conectados',
        'Oxímetros e termômetros inteligentes',
        'Dispositivos de ECG portáteis'
      ]
    },
    {
      icon: '💻',
      title: 'Plataforma de Teleconsulta',
      items: [
        'Videochamada HD com baixa latência',
        'Prontuário eletrônico integrado',
        'Painel de dados vitais em tempo real',
        'Prontuário eletrônico completo'
      ]
    },
    {
      icon: '🧠',
      title: 'Análise Inteligente por IA',
      items: [
        'Análise de séries históricas de saúde',
        'Detecção de padrões anômalos',
        'Sugestões de diagnóstico diferencial',
        'Alertas de risco automatizados'
      ]
    }
  ];

  benefits = {
    citizens: [
      'Acesso a especialidades médicas sem deslocamentos longos',
      'Redução significativa no tempo de espera',
      'Atendimento de qualidade com tecnologia de ponta',
      'Histórico médico sempre acessível',
      'Economia em deslocamentos e custos'
    ],
    municipalities: [
      'Otimização dos recursos de saúde pública',
      'Redução de custos operacionais',
      'Melhoria nos indicadores de saúde',
      'Facilidade na prestação de contas',
      'Atração de profissionais especialistas'
    ],
    professionals: [
      'Flexibilidade para atender de qualquer localização',
      'Suporte de IA para diagnósticos mais precisos',
      'Acesso a dados completos do paciente',
      'Oportunidade de impactar mais vidas',
      'Ambiente tecnológico avançado de trabalho'
    ]
  };

  stats = [
    { value: '100%', label: 'Digital' },
    { value: '24/7', label: 'Disponível' },
    { value: 'IA', label: 'Powered' },
    { value: 'LGPD', label: 'Compliant' }
  ];

  scrollToSection(sectionId: string): void {
    const element = document.getElementById(sectionId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  @HostListener('window:scroll', [])
  onWindowScroll(): void {
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
    
    // Detecta se está no hero section (primeiros ~600px)
    this.isInHeroSection = scrollTop < 600;
    
    // Se estiver no topo, sempre mostrar
    if (scrollTop <= this.scrollThreshold) {
      this.showNavbar = true;
      this.lastScrollTop = scrollTop;
      return;
    }
    
    // Mostrar navbar ao scrollar para cima, ocultar ao scrollar para baixo
    if (scrollTop < this.lastScrollTop) {
      // Scrolling up
      this.showNavbar = true;
    } else {
      // Scrolling down
      this.showNavbar = false;
    }
    
    this.lastScrollTop = scrollTop;
  }
}
