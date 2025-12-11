import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Benefit {
  icon: string;
  title: string;
  items: string[];
}

@Component({
  selector: 'app-impact',
  imports: [CommonModule],
  templateUrl: './impact.html',
  styleUrl: './impact.scss'
})
export class ImpactComponent {
  benefits: Benefit[] = [
    {
      icon: '👥',
      title: 'Para os Cidadãos',
      items: [
        'Acesso a especialidades médicas sem deslocamentos longos',
        'Redução significativa no tempo de espera',
        'Atendimento de qualidade com tecnologia de ponta',
        'Histórico médico sempre acessível',
        'Economia em deslocamentos e custos'
      ]
    },
    {
      icon: '🏛️',
      title: 'Para os Municípios',
      items: [
        'Otimização dos recursos de saúde pública',
        'Redução de custos operacionais',
        'Melhoria nos indicadores de saúde',
        'Facilidade na prestação de contas',
        'Atração de profissionais especialistas'
      ]
    },
    {
      icon: '⚕️',
      title: 'Para os Profissionais',
      items: [
        'Flexibilidade para atender de qualquer localização',
        'Suporte de IA para diagnósticos mais precisos',
        'Acesso a dados completos do paciente',
        'Oportunidade de impactar mais vidas',
        'Ambiente tecnológico avançado de trabalho'
      ]
    }
  ];
}
