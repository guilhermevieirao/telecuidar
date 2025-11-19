import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule, ArrowRight, Sparkles, Shield, Zap, Users, TrendingUp } from 'lucide-angular';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomePageComponent implements OnInit {
  readonly icons = { ArrowRight, Sparkles, Shield, Zap, Users, TrendingUp };
  
  features = [
    {
      icon: 'Sparkles',
      title: 'Interface Moderna',
      description: 'Design elegante e intuitivo que encanta os usuários desde o primeiro momento'
    },
    {
      icon: 'Shield',
      title: 'Segurança Total',
      description: 'Proteção avançada com as mais modernas tecnologias de segurança'
    },
    {
      icon: 'Zap',
      title: 'Performance Extrema',
      description: 'Velocidade e eficiência que superam todas as expectativas'
    },
    {
      icon: 'Users',
      title: 'Colaboração Perfeita',
      description: 'Trabalhe em equipe de forma fluida e produtiva'
    },
    {
      icon: 'TrendingUp',
      title: 'Crescimento Acelerado',
      description: 'Ferramentas que impulsionam seu desenvolvimento pessoal e profissional'
    }
  ];

  testimonials = [
    {
      name: 'Mariana Silva',
      role: 'CEO TechCorp',
      content: 'Esta plataforma revolucionou completamente nossa forma de trabalhar. Simplesmente incrível!',
      avatar: 'https://images.unsplash.com/photo-1494790108755-2616b612b5bc?w=150&h=150&fit=crop&crop=face'
    },
    {
      name: 'Carlos Oliveira',
      role: 'Desenvolvedor Senior',
      content: 'A qualidade e a atenção aos detalhes são impressionantes. Uma experiência excepcional.',
      avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&h=150&fit=crop&crop=face'
    },
    {
      name: 'Ana Costa',
      role: 'Product Manager',
      content: 'Finalmente uma solução que realmente entende as necessidades dos usuários modernos.',
      avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=150&h=150&fit=crop&crop=face'
    }
  ];

  ngOnInit(): void {
    // Adicionar animação de entrada suave
    setTimeout(() => {
      this.animateElements();
    }, 100);
  }

  private animateElements(): void {
    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('animate-fade-in');
        }
      });
    });

    document.querySelectorAll('.animate-on-scroll').forEach(el => {
      observer.observe(el);
    });
  }

  scrollToSection(sectionId: string): void {
    const element = document.getElementById(sectionId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth' });
    }
  }
}