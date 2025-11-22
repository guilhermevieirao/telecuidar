# TeleCuidar

<div align="center">

![TeleCuidar](https://img.shields.io/badge/TeleCuidar-Plataforma_de_Telesaúde-blue)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![Angular](https://img.shields.io/badge/Angular-20-red)
![License](https://img.shields.io/badge/license-MIT-green)

**Plataforma Pública de Telesaúde e Teleatendimento**

*Sucessor do SusAtende (PHP/Laravel) - Agora com tecnologia moderna*

[Documentação](docs/) · [Como Executar](docs/COMO-EXECUTAR.md) · [Checklist](docs/CHECKLIST_SISTEMA.md)

</div>

---

## 📋 Sobre o Projeto

**TeleCuidar** é uma plataforma moderna de **telesaúde e teleatendimento público**, desenvolvida para facilitar o acesso à saúde através de tecnologias digitais. É a nova versão do sistema **SusAtende** (https://susatende.com.br), completamente reescrito com stack moderna.

### 🔄 Evolução do Sistema
- **Versão Anterior**: SusAtende (PHP + Laravel)
- **Versão Atual**: TeleCuidar (C# .NET + Angular)
- **Objetivo**: Modernizar e escalar o atendimento público de telesaúde

---

## 🚀 Tecnologias

### Backend
- **C# .NET 10** - Framework principal
- **Entity Framework Core 9** - ORM
- **MediatR** - Padrão CQRS
- **SQL Server** - Banco de dados
- **Clean Architecture** - Arquitetura em camadas

### Frontend
- **Angular 20** - Framework SPA
- **TypeScript 5.9** - Linguagem
- **Tailwind CSS 3.4** - Estilização
- **SCSS** - Pré-processador CSS
- **RxJS** - Programação reativa
- **Lucide Angular** - Ícones

---

## 📁 Estrutura do Projeto

```
telecuidar/
├── backend/
│   ├── api/                    # API REST
│   ├── application/            # Casos de uso (CQRS)
│   ├── domain/                 # Entidades e regras de negócio
│   └── infrastructure/         # Persistência e serviços externos
│
├── frontend/
│   └── src/
│       └── app/
│           ├── core/           # Serviços e modelos centrais
│           ├── features/       # Funcionalidades (módulos)
│           └── shared/         # Componentes compartilhados
│
└── docs/                       # Documentação completa
```

---

## ⚡ Quick Start

### Pré-requisitos
- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Node.js 20+** - [Download](https://nodejs.org/)
- **SQL Server** (LocalDB incluído no Visual Studio)
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

### 🌐 Acessar Aplicação
- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5058
- **Health Check**: http://localhost:5058/health

---

## 🏗️ Arquitetura

### Backend - Clean Architecture

```
Domain Layer          → Entidades, interfaces, regras de negócio
    ↓
Application Layer     → Casos de uso, CQRS, DTOs
    ↓
Infrastructure Layer  → Persistência, repositórios, serviços
    ↓
API Layer            → Controllers, endpoints REST
```

### Frontend - Modular Architecture

```
Core Module          → Serviços singleton, guards, interceptors
    ↓
Features Modules     → Funcionalidades isoladas (lazy loading)
    ↓
Shared Module        → Componentes, pipes, directives reutilizáveis
```

---

## 📚 Funcionalidades

### ✅ Implementadas
- [x] Estrutura Clean Architecture completa
- [x] CRUD de usuários com CQRS
- [x] API REST funcional
- [x] Frontend Angular modular
- [x] Landing page responsiva
- [x] Design system com Tailwind CSS
- [x] Integração backend-frontend

### 🔄 Em Desenvolvimento
- [ ] Autenticação JWT
- [ ] Sistema de permissões (RBAC)
- [ ] Gestão de pacientes
- [ ] Gestão de médicos
- [ ] Agendamento de consultas
- [ ] Videochamadas (telemedicina)
- [ ] Prontuário eletrônico
- [ ] Prescrição digital

### 🎯 Roadmap
- [ ] Notificações em tempo real
- [ ] Relatórios e dashboards
- [ ] Exportação PDF/Excel
- [ ] Integração com SUS
- [ ] Compliance LGPD
- [ ] Testes automatizados
- [ ] CI/CD Pipeline
- [ ] Deploy containerizado (Docker)

---

## 🧪 Banco de Dados

### Configuração
Connection string em `backend/api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TeleCuidar;Trusted_Connection=True;"
  }
}
```

### Migrations
```powershell
cd backend/api
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 📖 Documentação

- [Como Executar](docs/COMO-EXECUTAR.md) - Guia completo de execução
- [Checklist do Sistema](docs/CHECKLIST_SISTEMA.md) - Lista de funcionalidades
- [Documentação Técnica](docs/README.md) - Detalhes técnicos

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Para contribuir:

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/NovaFuncionalidade`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/NovaFuncionalidade`)
5. Abra um Pull Request

---

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

---

## 👥 Equipe

Desenvolvido para melhorar o acesso à saúde pública através da tecnologia.

---

## 📞 Contato

Para dúvidas, sugestões ou suporte:
- **Issues**: [GitHub Issues](https://github.com/seu-usuario/telecuidar/issues)
- **Discussões**: [GitHub Discussions](https://github.com/seu-usuario/telecuidar/discussions)

---

<div align="center">

**[⬆ Voltar ao topo](#telecuidar)**

Feito com ❤️ para melhorar a saúde pública

</div>
