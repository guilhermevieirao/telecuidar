<div align="center">

# 🏥 TeleCuidar

![TeleCuidar](https://img.shields.io/badge/TeleCuidar-Plataforma_de_Telesaúde-blue)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![Angular](https://img.shields.io/badge/Angular-20-red)
![License](https://img.shields.io/badge/license-MIT-green)

### Plataforma Pública de Telesaúde e Teleatendimento

**Cuidado de Saúde Digital Inteligente**

[📋 Checklist do Projeto](checklist.md)

</div>

---

## 📖 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Quick Start](#-quick-start)
- [Funcionalidades](#-funcionalidades)
- [Tecnologias](#-tecnologias)
- [Arquitetura](#-arquitetura)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Banco de Dados](#-banco-de-dados)

---

## 🎯 Sobre o Projeto

**TeleCuidar** é uma plataforma integrada de telecuidado que conecta pacientes, profissionais e administradores em um ambiente digital seguro e eficiente. O sistema foi desenvolvido para modernizar e facilitar o acesso aos cuidados de saúde, oferecendo ferramentas completas para gestão médica e acompanhamento de pacientes.

### 💡 Nossa Missão

Tornar o cuidado em saúde mais **acessível**, **organizado** e **humanizado** através da tecnologia. Com foco na experiência do usuário e na segurança dos dados médicos, o sistema oferece ferramentas que simplificam processos, melhoram a comunicação e garantem que cada pessoa receba o cuidado que merece.

### 🔄 Evolução

- **Versão Anterior**: [SusAtende](https://susatende.com.br)
- **Versão Atual**: TeleCuidar - Reescrito com tecnologia moderna
- **Objetivo**: Modernizar e escalar o atendimento público de telesaúde

---

## ⚡ Quick Start

### Pré-requisitos
- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Node.js 20+** - [Download](https://nodejs.org/)
- **SQLite** (incluído no sistema)
- **PowerShell** (Windows)

### 🎯 Executar o Projeto

**Opção 1: Script Automático (Recomendado)**
```powershell
.\start.ps1
```

**Opção 2: VS Code Tasks**
1. Pressione `Ctrl+Shift+P`
2. Digite "Tasks: Run Task"
3. Selecione "Start Full Application"

**Opção 3: Manual**
```powershell
# Terminal 1 - Backend
cd backend/api
dotnet run

# Terminal 2 - Frontend
cd frontend
npm install
npm start
```

### 🌐 Acessar a Aplicação
- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5058
- **Health Check**: http://localhost:5058/health

---

## 📚 Funcionalidades

### ✅ A serem Implementadas

#### 👤 Sistema de Usuários
- **Autenticação segura** com diferentes tipos de usuário
- **Três perfis distintos**: Paciente, Profissional e Administrador
- **Gestão de perfis** com dados específicos para cada tipo de usuário
- **Sistema de permissões** baseado em roles
- **Painel Unificado**: Interface única para todos os tipos de usuários
- **Segurança**: Password hashing, JWT tokens, Email confirmation

#### 🏥 Área do Paciente
- **Painel personalizado** com visão geral dos dados de saúde
- **Histórico clínico completo** com consultas, diagnósticos e tratamentos
- **Resultados de exames** organizados por data e tipo
- **Prescrições médicas** com dosagens, instruções e observações
- **Busca de profissionais** por nome ou especialidade
- **Sistema de agendamento** de consultas por especialidade
- **Navegação intuitiva** com botões de retorno ao painel

#### 👨‍⚕️ Área do Profissional
- **Painel profissional** com estatísticas e informações relevantes
- **Gestão de agenda** com horários flexíveis e configurações personalizadas
- **Especialidades médicas** categorizadas e organizadas
- **Histórico de atendimentos** e pacientes
- **Emissão de prescrições** e solicitação de exames

#### 🔧 Área Administrativa
- **Painel administrativo completo** com estatísticas do sistema
- **Gestão de usuários** (pacientes, profissionais e administradores)
- **Controle de especialidades médicas**
- **Sistema de agendas** para profissionais
- **Relatórios e métricas** do sistema
- **Configurações avançadas** da plataforma

#### 📊 Sistema de Dados Médicos

**Histórico clínico detalhado** com:
- Data e tipo de consulta
- Diagnósticos e observações
- Medicamentos prescritos
- Exames solicitados
- Procedimentos realizados
- Sinais vitais (peso, altura, pressão arterial, temperatura)

**Resultados de exames** com:
- Tipos variados (laboratorial, imagem, cardiológico)
- Valores de referência
- Observações médicas
- Anexos de arquivos

**Prescrições médicas** incluindo:
- Medicamento e dosagem
- Frequência e duração do tratamento
- Instruções de uso
- Observações especiais

#### 📅 Sistema de Agendamento
- **Agendas flexíveis** para profissionais
- **Configuração de horários** de trabalho e pausas
- **Duração personalizada** de consultas
- **Intervalos entre consultas** configuráveis
- **Validação de conflitos** de horários
- **Períodos de validade** das agendas

### 🔄 Em Desenvolvimento
- [ ] Dashboard administrativo completo
- [ ] Sistema de permissões avançado (RBAC)
- [ ] Upload de arquivos e documentos
- [ ] Sistema de notificações em tempo real

### 🚀 Funcionalidades Futuras

#### 📹 Videochamadas
- **Consultas virtuais** em tempo real
- **Interface intuitiva** para chamadas de vídeo
- **Gravação de sessões** (quando autorizado)
- **Chat integrado** durante as consultas
- **Compartilhamento de tela** para visualização de exames

#### 🌐 Integração IoT - Parâmetros Biométricos
- **Monitoramento em tempo real** de sinais vitais
- **Dispositivos conectados** para captura automática de dados:
  - Pressão arterial
  - Frequência cardíaca
  - Temperatura corporal
  - Saturação de oxigênio
  - Glicemia
  - Peso e IMC
- **Alertas automáticos** para valores fora dos parâmetros normais
- **Histórico contínuo** de monitoramento
- **Painel em tempo real** para profissionais

#### 🤖 Anamnese Inteligente com IA
- **Questionário dinâmico** adaptado ao perfil do paciente
- **Inteligência artificial** para:
  - Personalização de perguntas baseada no histórico
  - Análise de respostas em tempo real
  - Sugestões de investigações adicionais
  - Identificação de padrões e riscos
- **Processamento de linguagem natural** para respostas abertas
- **Relatório automático** para o profissional
- **Aprendizado contínuo** do sistema baseado nos casos

### 🎯 Roadmap Técnico
- [ ] Relatórios e dashboards avançados
- [ ] Exportação PDF/Excel
- [ ] Integração com sistemas do SUS
- [ ] Auditoria de ações (Audit Logs)
- [ ] Compliance LGPD
- [ ] Testes automatizados (Unit, Integration, E2E)
- [ ] CI/CD Pipeline
- [ ] Deploy containerizado (Docker/Kubernetes)
- [ ] Modo escuro (Dark mode)
- [ ] Internacionalização (i18n)
- [ ] PWA (Progressive Web App)

---

## 🚀 Tecnologias

### Backend (C# .NET)
- **C# .NET 10** - Framework principal
- **Entity Framework Core 9** - ORM e Migrations
- **SQLite** - Banco de dados relacional (desenvolvimento)
- **MediatR** - Padrão CQRS para casos de uso
- **FluentValidation** - Validação de dados
- **JWT Authentication** - Autenticação segura com BCrypt
- **Clean Architecture** - Arquitetura em camadas

### Frontend (Angular)
- **Angular 20** - Framework SPA com standalone components
- **TypeScript 5.9** - Linguagem tipada
- **Tailwind CSS 3.4** - Framework CSS utility-first
- **SCSS** - Pré-processador CSS
- **RxJS** - Programação reativa
- **Lucide Angular** - Biblioteca de ícones moderna
- **Lazy Loading** - Carregamento otimizado de módulos

---

## 🏗️ Arquitetura

### Backend - Clean Architecture

```
API Layer → Application Layer (CQRS) → Infrastructure Layer → Domain Layer
```

**Camadas**:
- **API**: Controllers, Middlewares, Configuration
- **Application**: Use Cases (MediatR), Behaviors, DTOs, Validation
- **Infrastructure**: DbContext, Repositories, Services (Email, JWT, Password)
- **Domain**: Entities, Enums, Interfaces, Business Rules

### Frontend - Modular Architecture

```
Core (Services, Guards) → Features (Lazy Loading) → Shared (Components)
```

**Módulos**:
- **Core**: Services, Guards, Interceptors, Models (Singleton)
- **Features**: Auth, Dashboard, Profile, Landing (Lazy Loading)
- **Shared**: Components, Pipes, Directives (Reutilizáveis)

**Características**:
- ✅ Componentização e reutilização
- ✅ Lazy loading para performance
- ✅ Standalone components (Angular 20)
- ✅ Reactive programming (RxJS)

---

## 📁 Estrutura do Projeto

```
telecuidar/
├── backend/
│   ├── domain/                 # Entidades e interfaces do domínio
│   ├── application/            # Lógica de negócios (CQRS com MediatR)
│   │   ├── Auth/              # Comandos de autenticação
│   │   ├── Users/             # Queries e commands de usuários
│   │   └── Common/            # Behaviors, Interfaces, Models
│   ├── infrastructure/         # Implementações (DbContext, Repositórios, Serviços)
│   │   ├── Persistence/       # ApplicationDbContext e Configurations
│   │   ├── Repositories/      # Implementações de repositórios
│   │   ├── Services/          # EmailService, JwtTokenService, etc
│   │   └── Migrations/        # Migrations do EF Core
│   └── api/                    # API REST (Controllers, Configuração)
│       └── Controllers/       # AuthController, UsersController
│
├── frontend/
│   └── src/
│       └── app/
│           ├── core/           # Serviços singleton, guards, interceptors
│           │   ├── guards/    # Auth guards
│           │   ├── models/    # Interfaces e tipos
│           │   └── services/  # API services
│           ├── features/       # Funcionalidades (módulos lazy-loaded)
│           │   ├── auth/      # Login, Register, Forgot Password
│           │   ├── dashboard/ # Dashboard principal
│           │   ├── profile/   # Perfil do usuário
│           │   └── landing/   # Página inicial pública
│           ├── layouts/        # Layouts (Main, Auth)
│           └── shared/         # Componentes compartilhados
│               ├── components/ # Toast, Modal, Breadcrumb, etc
│               ├── directives/ # Diretivas customizadas
│               └── pipes/      # Pipes de transformação
```

---

## 🧪 Banco de Dados

O projeto usa **SQLite** por padrão para facilitar o desenvolvimento local.

**Connection string** em `backend/api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=telecuidar.db"
  }
}
```

### Configuração de Segurança

⚠️ **IMPORTANTE**: Nunca commite secrets ou senhas reais!

1. Copie o arquivo de exemplo:
```bash
cd backend/api
cp .env.example .env
```

2. Edite `.env` com suas configurações:
```bash
# Altere o JWT Secret (mínimo 32 caracteres)
Jwt__Secret=sua-chave-secreta-muito-forte-aqui-min-32-chars

# Configure credenciais do admin (apenas para desenvolvimento)
AdminUser__Email=seu-email@admin.com
AdminUser__Password=SuaSenhaForte123!
AdminUser__Enabled=true
```

3. Em **produção**, desabilite o seed do admin e use variáveis de ambiente:
```bash
AdminUser__Enabled=false
```

### Migrations

**Criar nova migration**:
```powershell
cd backend/api
dotnet ef migrations add NomeDaMigration
```

**Aplicar migrations** (executado automaticamente no startup):
```powershell
dotnet ef database update
```

### Usuário Admin Padrão

O sistema cria automaticamente um usuário administrador no primeiro startup (se `AdminUser:Enabled=true`):
- **Email**: Configurado em `appsettings.json` ou `.env`
- **Senha**: Configurada em `appsettings.json` ou `.env`
- ⚠️ **Altere a senha padrão imediatamente após o primeiro login!**

---

## 📦 Migrando para SQL Server (Produção)

Para usar SQL Server em produção:

1. Atualize a connection string em `appsettings.Production.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=seu-servidor;Database=TeleCuidar;Trusted_Connection=True;"
  }
}
```

2. Instale o pacote SQL Server:
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

3. Atualize `Program.cs` para usar SQL Server:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("app.Infrastructure")));
```

---