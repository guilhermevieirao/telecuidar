# Estrutura de Pastas do Frontend

Esta é a estrutura organizacional do código frontend da aplicação Telecuidar.

## Estrutura

```
src/app/
├── core/
│   ├── constants/          # Constantes da aplicação
│   ├── guards/            # Guards de rota (auth, guest)
│   ├── interceptors/      # Interceptors HTTP
│   └── services/          # Serviços core da aplicação
│
├── features/              # Módulos de funcionalidades
│   ├── admin/            # Administração do sistema
│   ├── auth/             # Autenticação (login, registro, etc)
│   ├── booking/          # Agendamento de consultas
│   ├── dashboard/        # Dashboard principal
│   ├── files/            # Gerenciamento de arquivos
│   ├── landing/          # Página inicial pública
│   ├── legal/            # Páginas legais
│   │   ├── privacy-policy/
│   │   └── terms-of-service/
│   ├── my-appointments/  # Minhas consultas (paciente)
│   ├── notifications/    # Notificações
│   ├── professional-appointments/  # Consultas (profissional)
│   ├── profile/          # Perfil do usuário
│   ├── reports/          # Relatórios
│   ├── schedule-blocks/  # Bloqueios de agenda
│   └── video-call/       # Videochamada
│
├── layouts/              # Layouts da aplicação
│   ├── auth-layout/      # Layout de autenticação
│   ├── main-layout/      # Layout principal (autenticado)
│   └── public-layout/    # Layout público
│
└── shared/               # Componentes, serviços e utilitários compartilhados
    ├── components/
    │   ├── atoms/        # Componentes atômicos (botões, inputs, etc)
    │   │   ├── button/
    │   │   ├── input/
    │   │   ├── spinner/
    │   │   └── theme-toggle/
    │   │
    │   ├── molecules/    # Componentes moleculares (compostos de átomos)
    │   │   ├── breadcrumb/
    │   │   ├── card/
    │   │   ├── cookie-consent/
    │   │   ├── pagination/
    │   │   └── toast/
    │   │
    │   └── organisms/    # Componentes complexos
    │       ├── confirm-modal/
    │       ├── dashboard-navbar/
    │       ├── image-crop-modal/
    │       ├── mobile-menu/
    │       ├── modal/
    │       └── modal-manager/
    │
    ├── directives/       # Diretivas Angular
    ├── models/           # Modelos de dados TypeScript
    └── pipes/            # Pipes personalizados
```

## Convenções

### Atomic Design

Os componentes são organizados seguindo os princípios de Atomic Design:

- **Atoms (Átomos)**: Componentes básicos e indivisíveis (botões, inputs, ícones)
- **Molecules (Moléculas)**: Composições simples de átomos (cards, breadcrumb, pagination)
- **Organisms (Organismos)**: Componentes complexos compostos por moléculas e átomos (navbar, modals, menus)

### Core vs Shared

- **Core**: Funcionalidades essenciais da aplicação (serviços, guards, interceptors)
- **Shared**: Componentes e utilitários reutilizáveis em toda a aplicação

### Features

Cada feature representa um módulo funcional independente da aplicação.

## Importações

### Exemplos de importação correta:

```typescript
// Serviços core
import { ApiService } from '@app/core/services/api.service';

// Componentes atômicos
import { ButtonComponent } from '@app/shared/components/atoms/button/button.component';

// Componentes moleculares
import { CardComponent } from '@app/shared/components/molecules/card/card.component';

// Componentes organism
import { ModalComponent } from '@app/shared/components/organisms/modal/modal.component';

// Models
import { User } from '@app/shared/models/user.model';

// Diretivas
import { AutofocusDirective } from '@app/shared/directives/autofocus.directive';

// Pipes
import { PhonePipe } from '@app/shared/pipes/phone.pipe';
```
