# 🐳 Docker - TeleCuidar

## Configuração de Variáveis de Ambiente

Todas as variáveis de ambiente estão centralizadas no arquivo `backend/api/.env`.

### Arquivos importantes:
- **`backend/api/.env`** - Configurações reais (NÃO COMMITAR)
- **`backend/api/.env.example`** - Template com valores de exemplo
- **`docker-compose.yml`** - Configuração para desenvolvimento local
- **`docker-compose.prod.yml`** - Override para produção

## 🚀 Modo de Uso

### 1. Desenvolvimento Local (com portas expostas)

```powershell
# Subir apenas os containers básicos
docker-compose up -d

# Acessar:
# - Frontend: http://localhost:4200
# - Backend API: http://localhost:5058
```

### 2. Produção Local (simulando produção com Caddy)

```powershell
# Subir com configuração de produção
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build

# Acessar via Caddy:
# - HTTP: http://localhost (redireciona para HTTPS)
# - HTTPS: https://localhost (certificado auto-assinado)
```

### 3. Produção Real (servidor)

```powershell
# No servidor, configure o .env com valores de produção
# Edite FRONTEND_URL e SSL_EMAIL no arquivo backend/api/.env

# Subir
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

## 📝 Variáveis Importantes do Docker

Estas variáveis estão no `backend/api/.env` e são usadas pelos docker-compose:

### Desenvolvimento:
```env
ASPNETCORE_URLS=http://+:5058
ASPNETCORE_ENVIRONMENT=Development
FrontendUrl=http://localhost:4200
ConnectionStrings__DefaultConnection_Docker=Data Source=/data/telecuidar.db
```

### Produção (docker-compose.prod.yml):
```env
FRONTEND_URL=https://www.telecuidar.com.br
SSL_EMAIL=contato@telecuidar.com
```

## 🔒 CADSUS - Certificado Digital

O certificado digital deve estar em `backend/api/certificado.pfx` e é montado automaticamente no container em `/app/certificado.pfx`.

Configure no `.env`:
```env
CADSUS__CertPath=./certificado.pfx
CADSUS__CertPassword=sua-senha-aqui
```

## 📦 Volumes

- **`db-data`** - Dados do SQLite (persistente)
- **`caddy-data`** - Certificados SSL do Caddy (persistente)
- **`caddy-config`** - Configurações do Caddy (persistente)

## 🛠️ Comandos Úteis

```powershell
# Ver logs
docker-compose logs -f backend
docker-compose logs -f frontend

# Parar containers
docker-compose down

# Rebuild completo
docker-compose up -d --build --force-recreate

# Limpar tudo (cuidado: apaga volumes!)
docker-compose down -v
```

## 🔄 Atualizar Produção

```powershell
# 1. Pull das mudanças
git pull

# 2. Rebuild e restart
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build

# 3. Verificar logs
docker-compose logs -f backend
```

## ⚠️ Notas Importantes

1. **Nunca commite o arquivo `.env`** com dados reais
2. Em produção, defina `AdminUser__Enabled=false` após criar o primeiro admin
3. O Caddy gera certificados SSL automaticamente via Let's Encrypt
4. Certifique-se de que o `SSL_EMAIL` está correto para notificações do Let's Encrypt
5. O certificado CADSUS deve ser A1 (.pfx) válido
