# app - Sistema Web Moderno

## Arquitetura do Sistema

### Backend (C# .NET)
- **Arquitetura Limpa (Clean Architecture)** com separação em camadas:
  - **Domain**: Entidades e interfaces
  - **Application**: Lógica de negócios e casos de uso
  - **Infrastructure**: Implementações concretas e persistência
  - **API**: Interface web e controllers

- **Tecnologias Utilizadas**:
  - .NET 10
  - Entity Framework Core
  - MediatR (CQRS)
  - AutoMapper
  - FluentValidation
  - SQL Server
  - Swagger/OpenAPI

### Frontend (Angular)
- **Arquitetura Modular** com:
  - Componentes standalone
  - Lazy loading
  - Services dedicados
  - Padrão de pastas organizado

- **Tecnologias Utilizadas**:
  - Angular 20
  - TypeScript
  - Tailwind CSS
  - SCSS
  - Lucide Icons
  - RxJS

## Estrutura de Pastas

```
backend/
├── domain/                       # Entidades e interfaces
├── application/                  # Lógica de negócios
├── infrastructure/               # Implementações
├── api/                          # API REST
└── app.Backend/                  # Projeto principal

frontend/
├── src/
│   ├── app/
│   │   ├── core/                 # Serviços e modelos
│   │   ├── features/             # Funcionalidades
│   │   └── shared/               # Componentes compartilhados
│   └── styles.scss              # Estilos globais
├── tailwind.config.js           # Configuração Tailwind
└── angular.json                 # Configuração Angular
```

## Como Executar

### Backend
```bash
cd backend/api
dotnet restore
dotnet run
```

### Frontend
```bash
cd frontend
npm install
ng serve
```

## Funcionalidades

- ✅ Arquitetura limpa e escalável
- ✅ Landing page moderna e responsiva
- ✅ Design system com Tailwind CSS
- ✅ Animações suaves e interativas
- ✅ Padrões de design modernos
- ✅ Integração API pronta

## Características do Design

- **Moderno**: Utiliza as últimas tendências de design
- **Responsivo**: Adaptável a todos os dispositivos
- **Acessível**: Segue diretrizes de acessibilidade
- **Performático**: Otimizado para velocidade
- **Escalável**: Facilmente extensível

## Próximos Passos

1. Configurar banco de dados
2. Implementar autenticação
3. Adicionar mais funcionalidades
4. Deploy em produção
5. Testes automatizados