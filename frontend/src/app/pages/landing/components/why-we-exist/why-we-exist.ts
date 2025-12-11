import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Challenge {
  icon: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-why-we-exist',
  imports: [CommonModule],
  templateUrl: './why-we-exist.html',
  styleUrl: './why-we-exist.scss'
})
export class WhyWeExistComponent {
  challenges: Challenge[] = [
    {
      icon: '🗺️',
      title: 'Dimensões Continentais',
      description: 'O Brasil possui dimensões continentais que dificultam o acesso à saúde especializada em regiões remotas, onde a retenção de profissionais é desafiadora.'
    },
    {
      icon: '⏰',
      title: 'Longas Filas de Espera',
      description: 'A população SUS dependente enfrenta longas filas para atendimento especializado, comprometendo a qualidade do cuidado de saúde.'
    },
    {
      icon: '👨‍⚕️',
      title: 'Escassez de Especialistas',
      description: 'Dificuldades econômicas, de segurança e acesso limitam a presença de especialistas em diversas regiões do país.'
    }
  ];
}
