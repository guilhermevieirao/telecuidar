import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Step {
  number: string;
  title: string;
  description: string;
  features: string[];
}

@Component({
  selector: 'app-solution',
  imports: [CommonModule],
  templateUrl: './solution.html',
  styleUrl: './solution.scss'
})
export class SolutionComponent {
  steps: Step[] = [
    {
      number: '1',
      title: 'Cadastro e Agendamento',
      description: 'App intuitivo para cadastro e agendamento com múltiplas especialidades disponíveis.',
      features: ['App intuitivo', 'Múltiplas especialidades', 'Agendamento fácil']
    },
    {
      number: '2',
      title: 'Pré-consulta Inteligente',
      description: 'Formulário completo com histórico, upload de imagens e análise por IA.',
      features: ['Histórico digital', 'Upload de imagens', 'Análise por IA']
    },
    {
      number: '3',
      title: 'Atendimento no Polo',
      description: 'Acolhimento humanizado em consultórios equipados com tecnologia IoT de ponta.',
      features: ['Acolhimento humanizado', 'Equipamentos IoT', 'Suporte profissional']
    },
    {
      number: '4',
      title: 'Consulta Especializada',
      description: 'Teleconsulta híbrida com dados biométricos em tempo real e insights de IA.',
      features: ['Especialista remoto', 'Dados em tempo real', 'Insights de IA']
    },
    {
      number: '5',
      title: 'Acompanhamento Contínuo',
      description: 'Prescrição digital, histórico completo e acompanhamento via app pessoal.',
      features: ['Prescrição digital', 'Histórico completo', 'Acompanhamento contínuo']
    }
  ];
}
