<div align="center">

# 🏥 TeleCuidar

![TeleCuidar](https://img.shields.io/badge/TeleCuidar-Plataforma_de_Telesaúde-blue)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![Angular](https://img.shields.io/badge/Angular-20-red)
![License](https://img.shields.io/badge/license-MIT-green)

### Plataforma Pública de Telesaúde e Teleatendimento

**Cuidado de Saúde Digital Inteligente**

[📖 Documentação Completa](docs/README.md) • [🔒 Segurança](docs/SECURITY.md) • [✅ Changelog](docs/CHANGELOG_FIXES.md)

</div>

---

## 🚀 Quick Start

### Instalação Rápida

```powershell
# 1. Clone o repositório
git clone https://github.com/guilhermevieirao/telecuidar.git
cd telecuidar

# 2. Backend - Configurar e rodar
cd backend/api
cp .env.example .env
dotnet run

# 3. Frontend - Instalar e rodar (em outro terminal)
cd frontend
npm install
npm start
```

### Acessar a Aplicação
- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5058
- **API Docs**: http://localhost:5058/api-docs

### Credenciais Padrão (Desenvolvimento)
- **Email**: admin@telecuidar.com
- **Senha**: Admin123!

⚠️ **Importante**: Altere as credenciais padrão antes de usar em produção!

---

## 📚 Documentação

Toda a documentação está organizada na pasta [`docs/`](docs/):

- **[📖 Documentação Completa](docs/README.md)** - Guia completo do projeto
- **[🔒 Guia de Segurança](docs/SECURITY.md)** - Práticas de segurança e deploy
- **[✅ Changelog de Correções](docs/CHANGELOG_FIXES.md)** - Últimas correções implementadas
- **[⚡ Quick Start Detalhado](docs/QUICKSTART_AFTER_FIXES.md)** - Início rápido pós-correções
- **[📋 Checklist](docs/checklist.md)** - Lista de tarefas do projeto
- **[🔔 Notificações](docs/NOTIFICACOES_DOCUMENTACAO.md)** - Sistema de notificações
- **[📝 Resumo de Implementação](docs/IMPLEMENTACAO_RESUMO.md)** - Detalhes da implementação

---

## 🏗️ Tecnologias

### Backend
- **.NET 10** - Framework principal
- **Entity Framework Core 9** - ORM
- **SQLite** - Banco de dados (dev)
- **MediatR** - CQRS Pattern
- **JWT + BCrypt** - Autenticação segura

### Frontend
- **Angular 20** - Framework SPA
- **TypeScript 5.9** - Linguagem tipada
- **Tailwind CSS 3.4** - Estilização
- **RxJS** - Programação reativa

---

## ✨ Funcionalidades

### Implementadas
- ✅ Autenticação JWT com BCrypt
- ✅ Sistema de usuários (Paciente, Profissional, Admin)
- ✅ Upload e gestão de arquivos
- ✅ Notificações em tempo real
- ✅ Sistema de auditoria (LGPD)
- ✅ Relatórios (PDF/Excel)
- ✅ Dashboard administrativo

### Roadmap
- 🔜 Videochamadas (WebRTC)
- 🔜 Integração IoT (dispositivos biométricos)
- 🔜 Anamnese com IA
- 🔜 Agendamento de consultas
- 🔜 Prescrições eletrônicas

---

## 🛠️ Desenvolvimento

### Pré-requisitos
- .NET 10 SDK
- Node.js 20+
- PowerShell (Windows)

### Estrutura do Projeto

```
telecuidar/
├── backend/
│   ├── api/              # Controllers, Program.cs
│   ├── application/      # Use Cases (CQRS)
│   ├── domain/           # Entidades e Interfaces
│   └── infrastructure/   # DbContext, Repositórios
├── frontend/
│   └── src/app/
│       ├── core/         # Services, Guards
│       ├── features/     # Módulos (lazy-loaded)
│       └── shared/       # Componentes reutilizáveis
└── docs/                 # Documentação
```

### Scripts Úteis

```powershell
# Executar aplicação completa
.\start.ps1

# Backend - Criar migration
cd backend/api
dotnet ef migrations add NomeDaMigration

# Frontend - Build produção
cd frontend
npm run build
```

---

## 🔐 Segurança

⚠️ **Antes de deploy em produção, leia**: [SECURITY.md](docs/SECURITY.md)

Principais pontos:
- Configurar JWT Secret forte via variáveis de ambiente
- Desabilitar seed do admin (`AdminUser:Enabled=false`)
- Usar HTTPS obrigatório
- Configurar CORS específico
- Implementar rate limiting

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Por favor:

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja [LICENSE](LICENSE) para mais detalhes.

---

## 📞 Contato

**Desenvolvedor**: Guilherme Vieira  
**Repository**: [github.com/guilhermevieirao/telecuidar](https://github.com/guilhermevieirao/telecuidar)

---

<div align="center">

**Feito com ❤️ para democratizar o acesso à saúde**

</div>
