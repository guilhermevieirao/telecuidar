# 🐳 Docker - TeleCuidar

## 🚀 Quick Start com Docker

### Pré-requisitos
- Docker Desktop instalado
- Docker Compose v2.0+

### 1️⃣ Configurar Variáveis de Ambiente

```powershell
# Copie o arquivo de exemplo
Copy-Item .env.example .env

# Edite o .env e defina uma chave JWT forte
# JWT_SECRET=sua-chave-secreta-forte-com-pelo-menos-32-caracteres
```

### 2️⃣ Executar a Aplicação

```powershell
# Build e start de todos os serviços
docker-compose up --build

# Ou em modo detached (background)
docker-compose up -d --build
```

### 3️⃣ Acessar a Aplicação

- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5058
- **API Docs**: http://localhost:5058/api-docs
- **Health Check Backend**: http://localhost:5058/health
- **Health Check Frontend**: http://localhost:4200/health

### 4️⃣ Credenciais Padrão

```
Email: admin@telecuidar.com
Senha: Admin123!
```

⚠️ **Altere estas credenciais após o primeiro login!**

---

## 📋 Comandos Úteis

```powershell
# Ver logs de todos os serviços
docker-compose logs -f

# Ver logs apenas do backend
docker-compose logs -f backend

# Ver logs apenas do frontend
docker-compose logs -f frontend

# Parar todos os serviços
docker-compose down

# Parar e remover volumes (APAGA O BANCO DE DADOS!)
docker-compose down -v

# Reconstruir apenas o backend
docker-compose build backend

# Reconstruir apenas o frontend
docker-compose build frontend

# Reiniciar um serviço específico
docker-compose restart backend

# Executar comando no container do backend
docker-compose exec backend bash

# Ver status dos containers
docker-compose ps
```

---

## 🏗️ Estrutura Docker

### Backend (`backend/Dockerfile`)
- **Base**: .NET 10 SDK + ASP.NET Runtime
- **Build**: Multi-stage para otimizar tamanho
- **Porta**: 5058
- **Volume**: `/data` para banco SQLite

### Frontend (`frontend/Dockerfile`)
- **Base**: Node 20 + Nginx Alpine
- **Build**: Angular production build
- **Porta**: 80 (mapeada para 4200 no host)
- **Servidor**: Nginx otimizado

### Compose (`docker-compose.yml`)
- **Network**: `telecuidar-network` (bridge)
- **Volume**: `db-data` (persistência SQLite)
- **Health Checks**: Monitoramento automático
- **Restart Policy**: `unless-stopped`

---

## 🔐 Segurança em Produção

### ⚠️ ANTES de fazer deploy:

1. **JWT Secret Forte**
   ```bash
   # Gere uma chave segura (PowerShell)
   -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | ForEach-Object {[char]$_})
   ```

2. **Desabilitar Admin Seed**
   ```env
   ADMIN_ENABLED=false
   ```

3. **Usar HTTPS**
   - Configure certificado SSL/TLS
   - Ajuste `ASPNETCORE_URLS=https://+:5058`

4. **Banco de Dados**
   - Considere PostgreSQL ao invés de SQLite
   - Configure backups automáticos

5. **CORS Específico**
   - Ajuste `FrontendUrl` para seu domínio real
   - Remova `http://localhost:4200` em produção

---

## 🗄️ Persistência de Dados

O banco SQLite é armazenado no volume Docker `db-data`:

```powershell
# Backup do banco
docker-compose exec backend cat /data/telecuidar.db > backup.db

# Restaurar backup
docker cp backup.db telecuidar-backend:/data/telecuidar.db
docker-compose restart backend
```

---

## 🐛 Troubleshooting

### Backend não inicia
```powershell
# Verificar logs
docker-compose logs backend

# Problemas comuns:
# - JWT_SECRET não configurado
# - Porta 5058 já em uso
# - Permissões no volume /data
```

### Frontend não conecta no Backend
```powershell
# Verificar se backend está healthy
docker-compose ps

# Verificar configuração de API no frontend
# Arquivo: frontend/src/environments/environment.prod.ts
```

### Resetar tudo
```powershell
# CUIDADO: Remove containers, volumes e imagens
docker-compose down -v --rmi all
docker-compose up --build
```

---

## 📊 Monitoramento

### Health Checks

Os serviços possuem health checks configurados:

```yaml
Backend: curl http://localhost:5058/health
Frontend: wget http://localhost:80/health
```

### Logs Estruturados

```powershell
# Logs com timestamp
docker-compose logs -f --timestamps

# Últimas 100 linhas
docker-compose logs --tail=100
```

---

## 🚀 Deploy em Produção

### Opções Recomendadas

1. **Azure Container Instances**
   ```bash
   az container create --resource-group telecuidar \
     --file docker-compose.yml
   ```

2. **AWS ECS/Fargate**
   - Use AWS Copilot CLI
   - Configure RDS PostgreSQL

3. **Kubernetes**
   - Gere manifestos K8s
   - Configure Ingress e TLS

4. **Docker Swarm**
   ```bash
   docker stack deploy -c docker-compose.yml telecuidar
   ```

---

## 📝 Variáveis de Ambiente

### Backend

| Variável | Descrição | Padrão | Obrigatório |
|----------|-----------|--------|-------------|
| `JWT_SECRET` | Chave secreta JWT (min 32 chars) | - | ✅ |
| `ADMIN_ENABLED` | Habilitar seed do admin | `false` | ❌ |
| `ADMIN_EMAIL` | Email do admin | `admin@telecuidar.com` | ❌ |
| `ADMIN_PASSWORD` | Senha do admin | `Admin123!` | ❌ |
| `FrontendUrl` | URL do frontend para CORS | `http://localhost:4200` | ❌ |

### Frontend

Configurações em `src/environments/environment.ts`:
- `apiUrl`: URL da API backend
- `production`: Flag de produção

---

## 📦 Otimizações

### Reduzir Tamanho das Imagens

```dockerfile
# Backend já usa multi-stage build
# Tamanho final: ~200MB

# Frontend usa nginx:alpine
# Tamanho final: ~50MB
```

### Cache de Builds

```powershell
# Docker usa cache de layers automaticamente
# Para forçar rebuild completo:
docker-compose build --no-cache
```

---

## 🤝 Contribuindo

Ao contribuir com melhorias no Docker:

1. Teste localmente com `docker-compose up --build`
2. Verifique health checks: `docker-compose ps`
3. Teste em modo produção (sem dev tools)
4. Documente mudanças neste README

---

## 📞 Suporte

Problemas com Docker? Abra uma issue:
- [GitHub Issues](https://github.com/guilhermevieirao/telecuidar/issues)

---

**Desenvolvido com ❤️ para democratizar o acesso à saúde**
