<div align="center">

# 🏥 TeleCuidar - Documentação Oficial

![TeleCuidar](https://img.shields.io/badge/TeleCuidar-Plataforma_de_Telesaúde-blue)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![Angular](https://img.shields.io/badge/Angular-20-red)
![License](https://img.shields.io/badge/license-MIT-green)

> **Status do Projeto**: Em Desenvolvimento Ativo

</div>

---

## 📖 Sumário

1. [Sobre o Projeto](#1-sobre-o-projeto)
2. [Arquitetura, Tecnologias e Métricas](#2-arquitetura-tecnologias-e-métricas)
3. [Funcionalidades e Módulos Detalhados](#3-funcionalidades-e-módulos-detalhados)
    - [Sistema de Notificações](#31-sistema-de-notificações)
    - [Sistema de Upload de Arquivos](#32-sistema-de-upload-de-arquivos)
    - [Paginação e Ordenação](#33-paginação-e-ordenação)
4. [Guia de Instalação e Execução (Quick Start)](#4-guia-de-instalação-e-execução-quick-start)
5. [Guia Avançado de Docker e Deploy](#5-guia-avançado-de-docker-e-deploy)
6. [Segurança e Compliance](#6-segurança-e-compliance)
7. [Banco de Dados e Migrations](#7-banco-de-dados-e-migrations)
8. [Roadmap e Próximos Passos](#8-roadmap-e-próximos-passos)
9. [Troubleshooting e FAQ](#9-troubleshooting-e-faq)
10. [Apêndice: Detalhes da Implementação](#10-apêndice-detalhes-da-implementação)

---

## 1. Sobre o Projeto

**TeleCuidar** é uma plataforma integrada de telecuidado que conecta pacientes, profissionais e administradores em um ambiente digital seguro e eficiente. O sistema foi desenvolvido para modernizar e facilitar o acesso aos cuidados de saúde, oferecendo ferramentas completas para gestão médica e acompanhamento de pacientes.

### 💡 Nossa Missão
Tornar o cuidado em saúde mais **acessível**, **organizado** e **humanizado** através da tecnologia. Com foco na experiência do usuário e na segurança dos dados médicos, o sistema oferece ferramentas que simplificam processos, melhoram a comunicação e garantem que cada pessoa receba o cuidado que merece.

### 🔄 Evolução
- **Versão Anterior**: [SusAtende](https://susatende.com.br)
- **Versão Atual**: TeleCuidar - Reescrito com tecnologia moderna
- **Objetivo**: Modernizar e escalar o atendimento público de telesaúde

---

## 2. Arquitetura, Tecnologias e Métricas

O projeto segue padrões modernos de desenvolvimento para garantir escalabilidade, manutenibilidade e segurança.

### Backend (C# .NET)
- **Framework**: .NET 10
- **Arquitetura**: Clean Architecture (Domain, Application, Infrastructure, API)
- **Padrões**: CQRS com MediatR, Repository Pattern, Unit of Work, Result Pattern.
- **ORM**: Entity Framework Core 9.
- **Banco de Dados**: SQLite (Dev) / SQL Server ou PostgreSQL (Prod).
- **Autenticação**: JWT Bearer com BCrypt para hashing de senhas.
- **Validação**: FluentValidation.

### Frontend (Angular)
- **Framework**: Angular 20 (Standalone Components).
- **Linguagem**: TypeScript 5.9.
- **Estilização**: Tailwind CSS 3.4 + SCSS modular.
- **Arquitetura**: Modular (Core, Features, Shared) com Lazy Loading.
- **Gerenciamento de Estado**: Services reativos com RxJS e Signals.
- **Ícones**: Lucide Angular.

### 📊 Métricas do Projeto

**Backend:**


**Frontend:**


### Estrutura de Pastas
```
telecuidar/
├── backend/
│   ├── domain/                 # Entidades e interfaces
│   ├── application/            # Casos de uso (CQRS), DTOs
│   ├── infrastructure/         # DbContext, Repositórios, Serviços Externos
│   └── api/                    # Controllers, Configuração
├── frontend/
│   └── src/app/
│       ├── core/               # Services singleton, Guards, Interceptors
│       ├── features/           # Módulos funcionais (Auth, Dashboard, etc.)
│       └── shared/             # Componentes reutilizáveis
└── docker/                     # Configurações de containerização
```

---

## 3. Funcionalidades e Módulos Detalhados

### 3.0 Visão Geral das Funcionalidades por Perfil

#### 👤 Área do Paciente
- **Painel personalizado** com visão geral dos dados de saúde.
- **Histórico clínico completo** com consultas, diagnósticos e tratamentos.
- **Resultados de exames** organizados por data e tipo.
- **Prescrições médicas** com dosagens, instruções e observações.
- **Busca de profissionais** por nome ou especialidade.
- **Sistema de agendamento** de consultas por especialidade.
- **Navegação intuitiva** com botões de retorno ao painel.

#### 👨‍⚕️ Área do Profissional
- **Painel profissional** com estatísticas e informações relevantes.
- **Gestão de agenda** com horários flexíveis e configurações personalizadas.
- **Especialidades médicas** categorizadas e organizadas.
- **Histórico de atendimentos** e pacientes.
- **Emissão de prescrições** e solicitação de exames.

#### 🔧 Área Administrativa
- **Painel administrativo completo** com estatísticas do sistema.
- **Gestão de usuários** (pacientes, profissionais e administradores).
- **Controle de especialidades médicas**.
- **Sistema de agendas** para profissionais.
- **Relatórios e métricas** do sistema.
- **Configurações avançadas** da plataforma.

### 3.1 Sistema de Notificações

O sistema permite enviar alertas em tempo real para os usuários. As notificações aparecem em um dropdown no header com badge contador.

#### Características
- **Polling**: Automático a cada 30 segundos.
- **Funcionalidades**: Marcar como lida, filtrar não lidas, links de ação, formatação "TimeAgo" (ex: "5m atrás").
- **Tipos Visuais**:

| Tipo | Ícone | Cor | Uso |
|------|-------|-----|-----|
| `info` | 📢 | Azul | Informações gerais |
| `success` | ✅ | Verde | Ações bem-sucedidas |
| `warning` | ⚠️ | Amarelo | Avisos importantes |
| `error` | ❌ | Vermelho | Erros e problemas |

#### Estrutura de Dados (Entidade)
```csharp
public class Notification : BaseEntity
{
    public string Title { get; set; }
    public string Message { get; set; }
    public string Type { get; set; } // info, success, warning, error
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
    
    // Relacionamentos
    public int UserId { get; set; }
    public User User { get; set; }
    public int? CreatedByUserId { get; set; }
    
    // Tracking
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    
    // Contexto
    public string? RelatedEntityType { get; set; } // Ex: "Appointment", "FileUpload"
    public int? RelatedEntityId { get; set; }
}
```

#### API Payloads (Exemplos)

**GET /api/notifications**
```json
{
  "data": [
    {
      "id": 1,
      "title": "Bem-vindo ao TeleCuidar! 🎉",
      "message": "Sua conta foi criada com sucesso.",
      "type": "success",
      "actionUrl": "/dashboard",
      "actionText": "Ir para o painel",
      "isRead": false,
      "timeAgo": "5m atrás",
      "createdAt": "2025-11-26T10:30:00"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 25
}
```

#### Exemplo de Uso (Frontend)
```typescript
// 1. Importar NotificationService
import { NotificationService } from '../../core/services/notification.service';

// 2. Injetar no construtor
constructor(private notificationService: NotificationService) {}

// 3. Usar helpers (Admin)
this.notificationService.notifyWelcome(userId, userName).subscribe();
this.notificationService.notifyFileUploaded(userId, fileName, fileId).subscribe();

// 4. Obter contador
this.notificationService.getUnreadCount().subscribe(res => console.log(res.count));

// 5. Listar notificações com filtro
this.notificationService.getMyNotifications(1, true).subscribe({
  next: (response) => {
    console.log('Não lidas:', response.data);
    console.log('Total:', response.totalCount);
  }
});

// 6. Marcar como lida
this.notificationService.markAsRead(notificationId).subscribe({
  next: () => console.log('Marcada como lida'),
  error: (err) => console.error('Erro:', err)
});
```

#### Exemplo de Uso (Backend - MediatR)
```csharp
// Criar notificação via Command
await _mediator.Send(new CreateNotificationCommand
{
    Title = "Consulta agendada",
    Message = "Sua consulta foi agendada para 30/11/2025.",
    Type = "success",
    UserId = 10,
    ActionUrl = "/consultas",
    ActionText = "Ver detalhes",
    RelatedEntityType = "Appointment",
    RelatedEntityId = 123
});

// Exemplo: Notificar mudança de status (UsersController)
await _mediator.Send(new CreateNotificationCommand
{
    Title = user.IsActive ? "Conta ativada" : "Conta desativada",
    Message = user.IsActive ? "Sua conta foi reativada." : "Sua conta foi temporariamente desativada.",
    Type = user.IsActive ? "success" : "warning",
    UserId = id,
    ActionUrl = "/perfil",
    ActionText = "Ver perfil"
});

// Exemplo: Broadcast para todos os usuários (AdminController)
foreach (var user in users)
{
    await _mediator.Send(new CreateNotificationCommand
    {
        Title = request.Title,
        Message = request.Message,
        Type = request.Type,
        UserId = user.Id
    });
}

// Exemplo: Notificar upload de arquivo (FilesController)
[HttpPost("upload")]
public async Task<IActionResult> Upload([FromForm] UploadFileCommand command)
{
    var result = await _mediator.Send(command);
    
    if (result.IsSuccess)
    {
        await _mediator.Send(new CreateNotificationCommand
        {
            Title = "Arquivo enviado com sucesso",
            Message = $"O arquivo '{command.File.FileName}' foi enviado.",
            Type = "success",
            UserId = GetCurrentUserId(),
            ActionUrl = "/arquivos",
            ActionText = "Ver arquivos",
            RelatedEntityType = "FileUpload",
            RelatedEntityId = result.Data.Id
        });
    }
    
    return Ok(result);
}
```

#### Casos de Uso Práticos

**1. Notificação de Boas-vindas** (quando usuário é criado)
```csharp
// Backend: CreateUserCommandHandler
await _mediator.Send(new CreateNotificationCommand
{
    Title = "Bem-vindo ao TeleCuidar! 🎉",
    Message = $"Olá {user.FirstName}! Sua conta foi criada com sucesso.",
    Type = "success",
    UserId = user.Id,
    ActionUrl = "/dashboard",
    ActionText = "Ir para o painel"
});
```

**2. Lembrete de Consulta** (sistema de agendamento)
```csharp
// Backend: AppointmentReminderJob (background service)
await _mediator.Send(new CreateNotificationCommand
{
    Title = "Lembrete de Consulta",
    Message = $"Você tem uma consulta agendada para {appointment.Date:dd/MM/yyyy} às {appointment.Time}.",
    Type = "info",
    UserId = appointment.PatientId,
    ActionUrl = $"/consultas/{appointment.Id}",
    ActionText = "Ver detalhes"
});
```

**3. Alerta de Erro** (sistema de monitoramento)
```csharp
// Backend: ErrorHandlingMiddleware
await _mediator.Send(new CreateNotificationCommand
{
    Title = "Erro no sistema",
    Message = "Ocorreu um erro ao processar sua solicitação. Nossa equipe foi notificada.",
    Type = "error",
    UserId = userId,
    ActionUrl = "/suporte",
    ActionText = "Contatar suporte"
});
```

### 3.2 Sistema de Upload de Arquivos

Módulo completo para gerenciamento de arquivos e documentos médicos.

**Funcionalidades:**
- **Upload**: Suporte a múltiplos formatos (.pdf, .doc, .jpg, .png).
- **Validação**: Limite de 10MB, verificação de extensão.
- **Categorias**: 
  - `Document` (Documentos) - Badge azul
  - `Image` (Imagens) - Badge verde
  - `Medical` (Médico) - Badge vermelho
  - `Other` (Outros) - Badge cinza
- **Armazenamento**: Pasta `/Uploads` com nomes únicos (GUID).

**Endpoints Principais:**
- `POST /api/files/upload`: Enviar arquivo.
- `GET /api/files/my-files`: Listar meus arquivos (paginado).
- `GET /api/files/{id}/download`: Download seguro.
- `DELETE /api/files/{id}`: Exclusão (owner ou admin).

### 3.3 Paginação e Ordenação

Implementado transversalmente em todo o sistema.

- **Backend**: `PagedResult<T>`, suporte a `sortBy` e `sortDirection`.
- **Frontend**: Componente `PaginationComponent` reutilizável.
- **Display**: Inteligente (1 ... 5 6 7 ... 20).

**Colunas Ordenáveis:**
- **Users**: ID, FullName, Email, Role, IsActive, CreatedAt.
- **AuditLogs**: ID, UserName, Action, EntityName, CreatedAt.

---

## 4. Guia de Instalação e Execução (Quick Start)

### Pré-requisitos
- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Node.js 20+** - [Download](https://nodejs.org/)
- **PowerShell** (Windows) ou Terminal Bash (Linux/Mac)
- **Git** para clonar o repositório

### Execução Local (Desenvolvimento)

**Opção 1: Script Automático (Windows)**
```powershell
.\start.ps1
```

**Opção 2: VS Code Tasks**
1. Pressione `Ctrl+Shift+P`
2. Digite "Tasks: Run Task"
3. Selecione "Start Full Application"

**Opção 3: Manual**
1. **Backend**:
   ```powershell
   cd backend/api
   dotnet restore
   dotnet run
   ```
2. **Frontend**:
   ```powershell
   cd frontend
   npm install
   npm start
   ```

### Credenciais Padrão
Ao iniciar pela primeira vez, se habilitado (`AdminUser:Enabled=true`), o sistema cria um admin:
- **Email**: `adm@adm.com`
- **Senha**: `zxcasd`
> ⚠️ **IMPORTANTE**: Altere a senha imediatamente após o login.

### Configuração de Segurança

⚠️ **IMPORTANTE**: Nunca commite secrets ou senhas reais!

**Passo 1: Copiar arquivo de exemplo**

Linux/Mac:
```bash
cd backend/api
cp .env.example .env
```

Windows (PowerShell):
```powershell
cd backend\api
Copy-Item .env.example .env
```

**Passo 2: Editar `.env` com suas configurações**
```bash
# Altere o JWT Secret (mínimo 32 caracteres)
Jwt__Secret=sua-chave-secreta-muito-forte-aqui-min-32-chars

# Configure credenciais do admin (apenas para desenvolvimento)
AdminUser__Email=seu-email@admin.com
AdminUser__Password=SuaSenhaForte123!
AdminUser__Enabled=true
```

**Passo 3: Em produção**

Desabilite o seed do admin e use variáveis de ambiente:

Linux/Mac:
```bash
export AdminUser__Enabled=false
export Jwt__Secret="sua-chave-producao-64-chars"
```

Windows (PowerShell):
```powershell
$env:AdminUser__Enabled="false"
$env:Jwt__Secret="sua-chave-producao-64-chars"
```

Docker:
```bash
docker run -e AdminUser__Enabled=false -e Jwt__Secret="sua-chave" ...
```

### Acessos
- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5058
- **Swagger Docs**: http://localhost:5058/api-docs
- **Health Check**: http://localhost:5058/health

---

## 5. Guia Avançado de Docker e Deploy

Este guia cobre a execução via Docker (Dev) e o deploy em produção (VPS Ubuntu com HTTPS).

### Execução Local com Docker
1. **Configurar Variáveis**:
   ```powershell
   Copy-Item .env.example .env
   # Edite o .env se necessário
   ```
2. **Subir Containers**:
   ```powershell
   docker-compose up -d --build
   ```

### Deploy em Produção (Ubuntu 24.04)

**Arquitetura de Produção**:
- **Backend**: Container .NET na rede interna.
- **Frontend**: Container Nginx servindo estáticos + Proxy reverso para `/api`.
- **Gateway/TLS**: Container Caddy (Reverse Proxy) gerenciando certificados SSL (Let's Encrypt) e redirecionamento HTTPS.

**Passo a Passo Detalhado:**

1. **Preparar Servidor**:
   ```bash
   sudo apt update && sudo apt upgrade -y
   # Instalar Docker (veja documentação oficial do Docker para comandos atualizados)
   ```

2. **Clonar e Configurar**:
   ```bash
   cd /opt
   sudo git clone https://github.com/guilhermevieirao/telecuidar.git
  listen 80;
  server_name localhost;
  root /usr/share/nginx/html;
  index index.html;

  # Proxy para API
  location /api/ {
    proxy_pass http://backend:5058/api/;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
    client_max_body_size 25m;
  }

  # Angular Router
  location / {
    try_files $uri $uri/ /index.html;
  }
}
```

#### Caddy (Produção) - HTTPS Automático
Arquivo: `caddy/Caddyfile`
```caddy
# Redirecionamento canônico
telecuidar.com, www.telecuidar.com, telecuidar.com.br {
  redir https://www.telecuidar.com.br{uri} permanent
}

# Host principal
www.telecuidar.com.br {
  encode gzip
  header {
    Strict-Transport-Security "max-age=31536000; includeSubDomains; preload"
    X-Frame-Options "DENY"
    X-Content-Type-Options "nosniff"
    Referrer-Policy "strict-origin-when-cross-origin"
  }
  reverse_proxy frontend:80
}
```

### 🏗️ Estrutura Interna dos Containers

#### Backend (`backend/Dockerfile`)
- **Base**: .NET 10 SDK + ASP.NET Runtime
- **Build**: Multi-stage para otimizar tamanho
- **Tamanho final**: ~200MB
- **Porta**: 5058
- **Volume**: `/data` para banco SQLite
- **Health check**: HTTP GET /health

#### Frontend (`frontend/Dockerfile`)
- **Base**: Node 20 + Nginx Alpine
- **Build**: Angular production build
- **Tamanho final**: ~50MB
- **Porta**: 80 (mapeada para 4200 no host)
- **Servidor**: Nginx otimizado
- **Proxy**: Configurado para repassar /api → backend:5058
- **Environment**: Configurado com `fileReplacements` para usar `environment.prod.ts` em builds de produção

**Configuração do Angular (`angular.json`)**:
```json
{
  "configurations": {
    "production": {
      "fileReplacements": [
        {
          "replace": "src/environments/environment.ts",
          "with": "src/environments/environment.prod.ts"
        }
      ]
    }
  }
}
```

Isso garante que:
- Em desenvolvimento: usa `environment.ts` com `apiUrl: 'http://localhost:5058'`
- Em produção: usa `environment.prod.ts` com `apiUrl: '/api'` (proxy reverso)

### 📦 Otimizações de Imagens Docker

**Backend já usa multi-stage build:**
```dockerfile
# Stage 1: Build (SDK completo)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
# ... compilação ...

# Stage 2: Runtime (apenas runtime, menor)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
# ... apenas binários ...
```

**Frontend usa nginx:alpine para tamanho reduzido:**
```dockerfile
FROM node:20-alpine AS build
# ... build Angular ...

FROM nginx:alpine
# ... apenas estáticos ...
```

**Cache de builds:**
```powershell
# Docker usa cache de layers automaticamente
# Para forçar rebuild completo:
docker-compose build --no-cache

# Ver tamanho das imagens:
docker images | grep telecuidar
```

### 🗄️ Persistência e Backup (SQLite)

O banco SQLite é armazenado no volume Docker `db-data`.

**Backup do banco:**
```powershell
docker-compose exec backend cat /data/telecuidar.db > backup.db
```

**Restaurar backup:**
```powershell
docker cp backup.db telecuidar-backend:/data/telecuidar.db
docker-compose restart backend
```

### ☁️ Opções de Deploy em Nuvem

#### 1. Azure Container Instances
```bash
az container create --resource-group telecuidar \
  --file docker-compose.yml
```

#### 2. AWS ECS/Fargate
- Use AWS Copilot CLI
- Configure RDS PostgreSQL
```bash
copilot init
copilot deploy
```

#### 3. Kubernetes
```bash
# Gerar manifestos K8s
kubectl apply -f k8s/
# Configure Ingress e TLS
```

#### 4. Docker Swarm
```bash
docker stack deploy -c docker-compose.yml telecuidar
```

### 📝 Tabela de Variáveis de Ambiente (Docker)

| Variável | Descrição | Padrão | Obrigatório |
|----------|-----------|--------|-------------|
| `JWT_SECRET` | Chave secreta JWT (min 32 chars) | - | ✅ |
| `ADMIN_ENABLED` | Habilitar seed do admin | `false` | ❌ |
| `ADMIN_EMAIL` | Email do admin | `adm@adm.com` | ❌ |
| `FrontendUrl` | URL do frontend para CORS | `http://localhost:4200` | ❌ |

---

## 6. Segurança e Compliance

Este guia consolida as melhores práticas de segurança implementadas e os requisitos para deploy em produção.

### 6.1 Guia de Segurança e Configuração

#### 1. Secrets e Variáveis de Ambiente
**NUNCA** commite dados sensíveis (JWT Secrets, senhas de DB, API Keys).
- **Boas práticas**: Use variáveis de ambiente para todos os secrets.
- **Produção**: Use Azure Key Vault, AWS Secrets Manager ou similar.
- **Arquivo .env**: Mantenha apenas local (já está no `.gitignore`).

#### 2. JWT Configuration
O JWT Secret **DEVE**:
- Ter no mínimo 32 caracteres.
- Ser aleatório, complexo e diferente em cada ambiente.
- **Gerar secret (PowerShell)**:
  ```powershell
  -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | ForEach-Object {[char]$_})
  ```
- **Configurar (Env Var)**: `Jwt__Secret="seu-secret-super-forte-aqui"`

#### 3. Usuário Administrador
⚠️ **DESABILITE** o seed automático do admin em produção!
- **Configuração**: `AdminUser__Enabled=false`
- **Criação em Prod**: Crie via script controlado, use senha forte e force troca no primeiro login.

#### 4. Banco de Dados
- **Desenvolvimento**: SQLite (`telecuidar.db`).
- **Produção**: SQL Server ou PostgreSQL gerenciado (Azure SQL, AWS RDS).
- **Connection String**: Configure via variável de ambiente `ConnectionStrings__DefaultConnection`.
- **Segurança**: Ative encryption at rest e backups automáticos.

#### 5. CORS
⚠️ Em produção, **NUNCA** use `AllowAnyOrigin`.
- Configure domínios específicos: `.WithOrigins("https://telecuidar.com")`
- `Program.cs` já possui política "Production" configurada para aceitar apenas origens confiáveis.

#### 6. HTTPS
✅ **Sempre** use HTTPS em produção.
- Configure certificado SSL/TLS válido.
- O middleware `app.UseHsts()` e `app.UseHttpsRedirection()` já está configurado para ambientes não-desenvolvimento.

#### 7. Rate Limiting
Implemente para prevenir ataques de força bruta e DDoS.
- Recomendação: Use `AspNetCoreRateLimit`.
- Proteja especialmente endpoints de login e APIs públicas.

#### 8. Logs e Monitoramento
- Configure logging estruturado (Serilog, NLog).
- Envie para centralizador (Application Insights, CloudWatch).
- **NUNCA** logue senhas ou tokens.
- Monitore tentativas de login falhas.

#### 9. Email Service
- **Atual**: Log no console (Dev).
- **Produção**: Implemente SendGrid, AWS SES ou SMTP seguro.
- Configure via `appsettings.json` ou variáveis de ambiente.

#### 10. Headers de Segurança
Já configurados em `Program.cs`:
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `X-XSS-Protection: 1; mode=block`
- `Referrer-Policy: strict-origin-when-cross-origin`

#### 11. Auditoria (LGPD)
✅ **Implementado**:
- Audit logs de todas as operações (Create, Update, Delete).
- Registro de IP e User-Agent.
- Anonimização de dados ao excluir conta.

### 6.2 Checklist de Deploy em Produção

Antes de subir para produção, verifique:

- [ ] **JWT Secret**: Forte e configurado via variável de ambiente.
- [ ] **Admin Seed**: Desabilitado (`AdminUser__Enabled=false`).
- [ ] **Database**: Connection string segura e backups configurados.
- [ ] **CORS**: Configurado para domínios reais (sem `*`).
- [ ] **HTTPS**: Obrigatório e com certificado válido.
- [ ] **Email**: Provider real configurado.
- [ ] **Logs**: Centralizados e sem dados sensíveis.
- [ ] **Dependências**: Atualizadas e sem vulnerabilidades conhecidas.
- [ ] **Backup**: Rotina de backup e restore testada.

---

## 7. Banco de Dados e Migrations

### Entidades Principais
- `User`: Usuários do sistema.
- `Role`: Perfis de acesso.
- `AuditLog`: Logs de segurança.
- `FileUpload`: Arquivos enviados.
- `Notification`: Notificações do sistema.

### Comandos EF Core
```powershell
# Criar nova migration
cd backend/api
dotnet ef migrations add NomeDaMigration

# Aplicar migrations (atualizar banco)
dotnet ef database update
```

### Migração para SQL Server
Para produção, recomenda-se SQL Server.
1. Atualize `appsettings.Production.json` com a Connection String.
2. Instale: `dotnet add package Microsoft.EntityFrameworkCore.SqlServer`.
3. Ajuste `Program.cs` para usar `UseSqlServer`.

---

## 8. Roadmap e Próximos Passos

#### 🌐 Integração IoT - Parâmetros Biométricos
- **Monitoramento em tempo real** de sinais vitais.
- **Dispositivos conectados** (Pressão, Frequência, Temperatura, Oximetria).
- **Alertas automáticos** para valores fora dos parâmetros.

#### 🤖 Anamnese Inteligente com IA
- **Questionário dinâmico** adaptado ao perfil.
- **IA para análise** de respostas e sugestão de diagnósticos.
- **Processamento de linguagem natural** para respostas abertas.

---

## 9. Troubleshooting e FAQ

### 🔧 Comandos de Limpeza e Reset

**Limpar cache do Backend:**
```powershell
cd backend/api
dotnet clean
Remove-Item -Recurse -Force bin, obj
dotnet restore
dotnet build
```

**Limpar cache do Frontend:**
```powershell
cd frontend
Remove-Item -Recurse -Force node_modules, dist, .angular
npm install
npm start
```

**Resetar banco de dados:**
```powershell
# Excluir banco SQLite
Remove-Item backend/api/telecuidar.db

# Recriar banco
cd backend/api
dotnet ef database update
```

**Reset completo do ambiente:**
```powershell
# Backend
cd backend/api
dotnet clean
Remove-Item -Recurse -Force bin, obj, telecuidar.db
dotnet restore
dotnet ef database update
dotnet run

# Frontend (novo terminal)
cd frontend
Remove-Item -Recurse -Force node_modules, dist, .angular
npm install
npm start
```

**Verificar porta em uso:**
```powershell
# Verificar se porta 5058 está livre
netstat -ano | findstr :5058

# Verificar se porta 4200 está livre
netstat -ano | findstr :4200
```

### Geral
**P: O backend não inicia (Porta em uso).**
R: Verifique se a porta 5058 está livre (`netstat -ano | findstr :5058`) ou se já existe um container rodando.

**P: "JWT Secret não configurado".**
R: Defina a variável de ambiente `Jwt__Secret` ou configure no `appsettings.json`.

**P: Como resetar o banco de dados?**
R: Exclua o arquivo `telecuidar.db` e rode `dotnet ef database update`.

**P: Backend não inicia mesmo com porta livre?**
R: Tente limpar os artefatos de build:
```powershell
cd backend/api
dotnet clean
dotnet build
dotnet run
```

**P: Frontend com erros estranhos de build?**
R: Reinstale as dependências:
```powershell
cd frontend
Remove-Item -Recurse -Force node_modules
npm install
npm start
```

**P: Erro de migration?**
R: Verifique se há migrations pendentes:
```powershell
cd backend/api
dotnet ef migrations list
dotnet ef database update
```

### Docker e Deploy

**Comandos Docker úteis:**
```powershell
# Ver logs com timestamp
docker-compose logs -f --timestamps

# Últimas 100 linhas de log
docker-compose logs --tail=100

# Logs de serviço específico
docker-compose logs -f backend
docker-compose logs -f frontend

# Rebuild sem cache
docker-compose build --no-cache

# Rebuild apenas um serviço
docker-compose build backend
docker-compose build frontend

# Reiniciar serviço específico
docker-compose restart backend
docker-compose restart frontend

# Executar comando dentro do container
docker-compose exec backend bash
docker-compose exec backend dotnet ef database update

# Ver status dos containers
docker-compose ps

# Ver uso de recursos
docker stats

# Inspecionar container
docker-compose exec backend env

# Parar containers
docker-compose down

# Parar e remover volumes (apaga DB!)
docker-compose down -v

# Resetar tudo (CUIDADO: apaga volumes e imagens)
docker-compose down -v --rmi all
docker-compose up --build

# Ver tamanho das imagens
docker images | grep telecuidar

# Limpar containers parados
docker container prune

# Limpar imagens não utilizadas
docker image prune -a
```

**P: Erro de CORS no Frontend em Produção.**
R: Verifique se `environment.apiUrl` aponta para `/api` (proxy reverso) e não para `localhost`. O Nginx deve estar configurado para repassar `/api` para o backend.

**P: Backend "unhealthy" no Docker.**
R: Verifique se o `curl` está instalado na imagem ou se o healthcheck está configurado corretamente. Em versões recentes, removemos healthchecks que dependiam de `curl/wget` no container runtime para reduzir tamanho. Considere adicionar `busybox` ou configurar healthcheck externo.

**P: Certificado TLS não emite (Caddy).**
R: Verifique se as portas 80/443 estão abertas no firewall e se o DNS aponta corretamente para o IP do servidor.

**P: Erro "loopback more-private address space" (CORS).**
R: Causa: frontend chamando `http://localhost:5058` no navegador em produção. Fix: `environment.prod.ts` com `apiUrl: '/api'` + Nginx proxy `/api` → backend.

### Notificações
**P: Notificações não aparecem?**
R: 
1. Verifique a autenticação (token JWT válido).
2. Verifique a role do usuário (apenas admin pode criar notificações via API de teste).
3. Cheque o console do navegador para erros de conexão.
4. Verifique se o backend está retornando 200 OK nas chamadas de polling.

**P: Badge não atualiza?**
R: O polling roda a cada 30s. Verifique se há erros no console do navegador.

**P: Dropdown não fecha?**
R: Clique no overlay (fundo escuro) ou em uma notificação para fechar.

---

## 10. Comandos Úteis para Desenvolvimento

Esta seção contém comandos frequentemente utilizados durante o desenvolvimento.

### 🔧 Backend (.NET)

**Build e Execução:**
```powershell
# Restaurar dependências
cd backend/api
dotnet restore

# Build do projeto
dotnet build

# Build em modo Release
dotnet build --configuration Release

# Executar o projeto
dotnet run

# Executar com watch (recompila automaticamente)
dotnet watch run

# Limpar artefatos de build
dotnet clean
Remove-Item -Recurse -Force bin, obj
```

**Entity Framework Core:**
```powershell
# Criar nova migration
cd backend/api
dotnet ef migrations add NomeDaMigration

# Aplicar migrations
dotnet ef database update

# Reverter última migration
dotnet ef migrations remove

# Listar migrations
dotnet ef migrations list

# Gerar script SQL
dotnet ef migrations script

# Dropar banco de dados
dotnet ef database drop

# Resetar banco (drop + update)
dotnet ef database drop -f
dotnet ef database update
```

**Testes e Qualidade:**
```powershell
# Executar testes
dotnet test

# Executar com cobertura
dotnet test /p:CollectCoverage=true

# Verificar vulnerabilidades
dotnet list package --vulnerable

# Atualizar pacotes
dotnet add package <PackageName>
dotnet remove package <PackageName>
```

### 🎨 Frontend (Angular)

**Instalação e Execução:**
```powershell
# Instalar dependências
cd frontend
npm install

# Reinstalar (limpar cache)
Remove-Item -Recurse -Force node_modules, package-lock.json
npm install

# Executar em desenvolvimento
npm start
# ou
ng serve

# Executar em porta específica
ng serve --port 4300

# Executar e abrir navegador
ng serve --open
```

**Build:**
```powershell
# Build de desenvolvimento
ng build

# Build de produção
ng build --configuration production
# ou
npm run build

# Build com análise de bundle
ng build --stats-json
npx webpack-bundle-analyzer dist/stats.json
```

**Geração de Código:**
```powershell
# Gerar componente
ng generate component features/nome-componente
# ou
ng g c features/nome-componente

# Gerar componente standalone
ng g c features/nome-componente --standalone

# Gerar service
ng g s core/services/nome-service

# Gerar guard
ng g g core/guards/nome-guard

# Gerar pipe
ng g p shared/pipes/nome-pipe

# Gerar directive
ng g d shared/directives/nome-directive
```

**Linting e Formatação:**
```powershell
# Verificar linting
ng lint

# Corrigir automaticamente
ng lint --fix

# Formatar com Prettier (se configurado)
npx prettier --write "src/**/*.{ts,html,scss}"
```

**Análise e Otimização:**
```powershell
# Analisar tamanho dos módulos
ng build --stats-json
npx webpack-bundle-analyzer dist/frontend/stats.json

# Verificar atualizações
ng update

# Atualizar Angular
ng update @angular/core @angular/cli

# Atualizar todos os pacotes
ng update --all
```

### 🗄️ Banco de Dados (SQLite)

**Backup e Restore:**
```powershell
# Backup do banco
Copy-Item backend/api/telecuidar.db backup/telecuidar_backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').db

# Restaurar backup
Copy-Item backup/telecuidar_backup_20251128.db backend/api/telecuidar.db

# Resetar banco (apagar e recriar)
Remove-Item backend/api/telecuidar.db
cd backend/api
dotnet ef database update
```

### 🐛 Debug e Troubleshooting

**Verificar Portas:**
```powershell
# Verificar se porta 5058 está em uso (backend)
netstat -ano | findstr :5058

# Verificar se porta 4200 está em uso (frontend)
netstat -ano | findstr :4200

# Matar processo por PID
Stop-Process -Id <PID> -Force
```

**Limpar Cache Completo:**
```powershell
# Backend
cd backend/api
dotnet clean
Remove-Item -Recurse -Force bin, obj, telecuidar.db
dotnet restore
dotnet ef database update

# Frontend
cd frontend
Remove-Item -Recurse -Force node_modules, dist, .angular
npm install
```

**Logs e Monitoramento:**
```powershell
# Ver logs do backend (se usando Serilog)
Get-Content backend/api/logs/log.txt -Tail 50 -Wait

# Monitorar requisições HTTP (frontend)
# Abra DevTools > Network tab

# Ver variáveis de ambiente (backend)
cd backend/api
dotnet run --launch-profile Development
```

### 📦 Git e Controle de Versão

**Operações Comuns:**
```powershell
# Status do repositório
git status

# Adicionar arquivos
git add .

# Commit com mensagem
git commit -m "feat: adicionar sistema de notificações"

# Push para remoto
git push origin main

# Criar nova branch
git checkout -b feature/nova-funcionalidade

# Ver histórico
git log --oneline --graph --decorate

# Desfazer último commit (mantendo alterações)
git reset --soft HEAD~1

# Descartar alterações locais
git checkout -- .
git clean -fd
```

---

## 11. Apêndice: Detalhes da Implementação

### 📝 Relatório de Implementação (Sessão Atual)

#### Arquivos Criados
**Backend (21 arquivos):**
1. `Domain/Entities/FileUpload.cs`
2. `Domain/Entities/Notification.cs`
3. `Infrastructure/Persistence/Configurations/FileUploadConfiguration.cs`
4. `Infrastructure/Persistence/Configurations/NotificationConfiguration.cs`
5. `Application/Files/DTOs/FileUploadDto.cs`
6. `Application/Files/Commands/UploadFile/...`
7. `Application/Notifications/Commands/CreateNotification/...`
8. `Api/Controllers/FilesController.cs`
9. `Api/Controllers/NotificationsController.cs`
10. `Infrastructure/Migrations/20251126033202_AddNotifications.cs`
... (e outros DTOs, Commands e Queries relacionados)

**Frontend (7 arquivos):**
1. `features/files/files.component.ts` (html/scss)
2. `features/notifications/notifications.component.ts` (html/scss)
3. `core/services/notification.service.ts`

**Tecnologias:**
- .NET 10 (última versão)
- Angular 20 (última versão com standalone components)
- Entity Framework Core 9
- SQLite / SQL Server
- MediatR (CQRS pattern)
- Tailwind CSS
- JWT Authentication
- Docker & Docker Compose
- Nginx como proxy reverso
- Caddy para TLS automático

---

## 11. Issues Conhecidos (Não Críticos)

Estes itens não impedem o uso do sistema, mas devem ser implementados futuramente:

### 🔧 Melhorias Técnicas

1. **EmailService usa console logging**
   - Status: Desenvolvimento apenas
   - Ação: Implementar SendGrid/AWS SES para produção
   - Prioridade: ⚠️ Alta para produção

2. **Error interceptor tem TODOs**
   - Status: Funcional, mas pode ser melhorado
   - Ação: Melhorar tratamento de erros e mensagens ao usuário
   - Prioridade: 📊 Média

3. **Rate limiting não implementado**
   - Status: Sem proteção contra abuse
   - Ação: Adicionar `AspNetCoreRateLimit` para produção
   - Prioridade: ⚠️ Alta para produção

4. **Testes automatizados ausentes**
   - Status: Sem cobertura de testes
   - Ação: Implementar unit tests, integration tests, E2E
   - Prioridade: 📊 Média

5. **CI/CD pipeline não configurado**
   - Status: Deploy manual
   - Ação: Configurar GitHub Actions ou Azure DevOps
   - Prioridade: 📊 Média

### 📋 Recomendações Imediatas

**Para Desenvolvimento:**
- ✅ Sistema funciona perfeitamente
- ✅ Todas as funcionalidades core implementadas
- ✅ Segurança adequada para ambiente local

**Para Produção (Antes do Deploy):**
1. ⚠️ Implementar EmailService real (SendGrid/AWS SES)
2. ⚠️ Adicionar rate limiting (proteção DDoS)
3. ⚠️ Configurar logs centralizados
4. ⚠️ Implementar backups automáticos
5. 📊 Considerar adicionar testes automatizados

## 13. Contribuindo com o Projeto

### 🤝 Como Contribuir

**Ao contribuir com melhorias:**
1. Teste localmente com todos os serviços rodando
2. Para Docker: `docker-compose up --build`
3. Verifique health checks: `docker-compose ps`
4. Execute testes (quando disponíveis)
5. Documente mudanças neste README e arquivos específicos

**Padrões de Código:**
- Backend: Clean Architecture + CQRS (veja seção 2)
- Frontend: Standalone Components + Lazy Loading (veja seção 2)
- Commits: Mensagens descritivas em português
- PRs: Incluir descrição detalhada e testes realizados

**Documentação:**
- Atualize este arquivo para mudanças gerais
- Atualize arquivos específicos para mudanças pontuais
- Mantenha o CHANGELOG atualizado

---

## 14. Licença e Contato

### 📄 Licença
Este projeto está licenciado sob a **MIT License**.

### 📞 Suporte e Contato

**Para Issues e Bugs:**
- [GitHub Issues](https://github.com/guilhermevieirao/telecuidar/issues)

**Para Vulnerabilidades de Segurança:**
- ⚠️ **NÃO** divulgue publicamente
- Entre em contato: security@telecuidar.com
- Aguarde correção antes de divulgação

**Para Contribuições:**
- Abra um Pull Request no GitHub
- Siga os padrões de código acima
- Inclua testes quando possível

---

*Desenvolvido com ❤️ para democratizar o acesso à saúde*