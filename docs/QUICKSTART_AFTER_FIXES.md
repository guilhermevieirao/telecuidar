# 🚀 Quick Start - Após Correções

## ✅ O que foi corrigido

1. **Senhas agora são hasheadas** corretamente com BCrypt
2. **URLs centralizadas** - frontend usa `environment.apiUrl`
3. **Secrets configuráveis** via variáveis de ambiente
4. **Admin seed controlado** e desabilitável
5. **Documentação atualizada** com guia de segurança

## 🏃 Como rodar AGORA (desenvolvimento)

### Opção 1: Como antes (funciona imediatamente)
```powershell
# Terminal 1 - Backend
cd backend/api
dotnet run

# Terminal 2 - Frontend
cd frontend
npm start
```

### Opção 2: Com variáveis de ambiente customizadas (opcional)
```powershell
# 1. Criar arquivo .env
cd backend/api
cp .env.example .env

# 2. Editar .env com suas configurações (OPCIONAL)
notepad .env

# 3. Rodar normalmente
dotnet run
```

## 🔐 Credenciais Padrão (desenvolvimento)

**Admin padrão** (criado automaticamente no primeiro startup):
- Email: `admin@telecuidar.com`
- Senha: `Admin123!`

⚠️ **IMPORTANTE**: Altere a senha após primeiro login!

## 📦 Primeira vez rodando o projeto?

```powershell
# Backend
cd backend/api
dotnet restore
dotnet run

# Frontend (novo terminal)
cd frontend
npm install
npm start
```

## 🌐 Acessar a aplicação

- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5058
- **Swagger**: http://localhost:5058/api-docs
- **Health Check**: http://localhost:5058/health

## 🐛 Problemas?

### Backend não inicia
```powershell
# Verificar se porta 5058 está livre
netstat -ano | findstr :5058

# Limpar e recompilar
cd backend/api
dotnet clean
dotnet build
dotnet run
```

### Frontend não inicia
```powershell
# Limpar node_modules e reinstalar
cd frontend
Remove-Item -Recurse -Force node_modules
npm install
npm start
```

### Erro de banco de dados
```powershell
# Aplicar migrations
cd backend/api
dotnet ef database update
```

### "JWT Secret não configurado"
Verifique se `appsettings.json` tem a seção `Jwt` com `Secret` configurado.
Ou configure via variável de ambiente: `$env:Jwt__Secret="sua-chave-min-32-chars"`

## 📚 Arquivos importantes para ler

1. **CHANGELOG_FIXES.md** - Detalhes de todas as correções
2. **SECURITY.md** - Guia completo de segurança para produção
3. **README.md** - Documentação geral do projeto
4. **backend/api/.env.example** - Template de variáveis de ambiente

## 🚀 Deploy em produção?

**LEIA PRIMEIRO**: `SECURITY.md`

Checklist mínimo:
- [ ] JWT Secret forte (64+ chars aleatórios)
- [ ] AdminUser:Enabled=false
- [ ] Connection string de produção via variável de ambiente
- [ ] CORS configurado para domínios específicos
- [ ] HTTPS obrigatório
- [ ] EmailService com provider real

## 💡 Dicas

- Use `.env` para configurações locais (não commite!)
- Admin seed é útil em dev, desabilite em produção
- SQLite é para dev, use SQL Server/PostgreSQL em prod
- Todas as chamadas HTTP agora usam `environment.apiUrl`

## ❓ Dúvidas?

Leia os arquivos de documentação:
- Issues conhecidos: `CHANGELOG_FIXES.md` (seção final)
- Segurança: `SECURITY.md`
- Setup geral: `README.md`

---

**Tudo pronto!** O sistema está corrigido e pronto para desenvolvimento. 🎉
