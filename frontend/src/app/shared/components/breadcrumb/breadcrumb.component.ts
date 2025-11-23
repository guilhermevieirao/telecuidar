import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd, ActivatedRoute, RouterModule } from '@angular/router';
import { filter } from 'rxjs/operators';

interface Breadcrumb {
  label: string;
  url: string;
}

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.scss']
})
export class BreadcrumbComponent implements OnInit {
  breadcrumbs: Breadcrumb[] = [];

  private routeLabels: { [key: string]: string } = {
    '': 'Início',
    'painel': 'Painel',
    'entrar': 'Entrar',
    'cadastrar': 'Cadastro',
    'recuperar-senha': 'Recuperar Senha',
    'perfil': 'Meu Perfil',
    'administracao': 'Administração',
    'logs-auditoria': 'Logs de Auditoria',
    'teste': 'Videochamada'
  };

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    // Build breadcrumbs on navigation end
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.breadcrumbs = this.buildBreadcrumbs(this.activatedRoute.root);
      });

    // Initial breadcrumbs
    this.breadcrumbs = this.buildBreadcrumbs(this.activatedRoute.root);
  }

  private buildBreadcrumbs(route: ActivatedRoute, url: string = '', breadcrumbs: Breadcrumb[] = []): Breadcrumb[] {
    // Get child routes
    const children: ActivatedRoute[] = route.children;

    if (children.length === 0) {
      return breadcrumbs;
    }

    for (const child of children) {
      const routeURL: string = child.snapshot.url.map(segment => segment.path).join('/');
      
      if (routeURL !== '') {
        url += `/${routeURL}`;
        
        // Check if route has a parent breadcrumb
        const breadcrumbParent = child.snapshot.data['breadcrumbParent'];
        if (breadcrumbParent && !breadcrumbs.some(b => b.url === breadcrumbParent)) {
          // Add parent breadcrumb if not already present
          const parentRoute = breadcrumbParent.substring(1); // Remove leading slash
          const parentLabel = this.routeLabels[parentRoute] || parentRoute;
          breadcrumbs.push({ label: parentLabel, url: breadcrumbParent });
        }
        
        // Get label from route data or use default
        const label = child.snapshot.data['breadcrumb'] || this.routeLabels[routeURL] || routeURL;
        
        breadcrumbs.push({ label, url });
      }

      // Recursive call
      return this.buildBreadcrumbs(child, url, breadcrumbs);
    }

    return breadcrumbs;
  }
}
