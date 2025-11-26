# 🔒 Guia de Segurança - TeleCuidar

## ⚠️ Antes de Deploy em Produção

### 1. Secrets e Variáveis de Ambiente

**NUNCA** commite os seguintes dados:
- JWT Secret Keys
- Senhas de banco de dados
- API Keys de serviços externos
- Credenciais de email/SMTP
- Arquivos `.env`

✅ **Boas práticas:**
- Use variáveis de ambiente para todos os secrets
- Configure secrets no ambiente de deploy (Azure, AWS, Docker, etc)
- Mantenha `.env` apenas local (já está no `.gitignore`)
- Use Azure Key Vault, AWS Secrets Manager ou similar em produção

### 2. JWT Configuration

O JWT Secret **DEVE**:
- Ter no mínimo 32 caracteres
- Ser aleatório e complexo
- Ser diferente em cada ambiente (dev/staging/prod)
- Nunca ser exposto em logs ou mensagens de erro

**Gerar um secret seguro (PowerShell)**:
```powershell
# Gera uma string aleatória de 64 caracteres
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

**Configurar via variável de ambiente**:
```bash
# Linux/Mac
export Jwt__Secret="seu-secret-super-forte-aqui"

# Windows PowerShell
$env:Jwt__Secret="seu-secret-super-forte-aqui"

# Docker
docker run -e Jwt__Secret="seu-secret-super-forte-aqui" ...
```

### 3. Usuário Administrador

⚠️ **DESABILITE** o seed automático do admin em produção!

Em `appsettings.Production.json` ou via variável de ambiente:
```json
{
  "AdminUser": {
    "Enabled": false
  }
}
```

Ou via variável de ambiente:
```bash
AdminUser__Enabled=false
```

**Para criar admin em produção:**
1. Desabilite o seed automático
2. Crie o primeiro admin via console/script controlado
3. Use senhas fortes e únicas
4. Force troca de senha no primeiro login

### 4. Banco de Dados

**Desenvolvimento (SQLite)**:
- OK para desenvolvimento local
- Arquivo: `telecuidar.db`
- **NÃO** commitar o arquivo `.db`

**Produção (SQL Server / PostgreSQL)**:
- Use banco gerenciado (Azure SQL, AWS RDS, etc)
- Connection string via variável de ambiente
- Use autenticação gerenciada quando possível
- Configure backups automáticos
- Ative encryption at rest

**Connection String em produção**:
```bash
# Via variável de ambiente
ConnectionStrings__DefaultConnection="Server=prod-server;Database=TeleCuidar;User=app_user;Password=STRONG_PASSWORD"
```

### 5. CORS

⚠️ Em produção, **NUNCA** use `AllowAnyOrigin`!

Atualize `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production",
        policy => policy
            .WithOrigins("https://telecuidar.com", "https://www.telecuidar.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
```

### 6. HTTPS

✅ **Sempre** use HTTPS em produção:
- Configure certificado SSL/TLS válido
- Force redirecionamento HTTP → HTTPS
- Use HSTS (HTTP Strict Transport Security)

Em `Program.cs`:
```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // Adicione HSTS
    app.UseHttpsRedirection();
}
```

### 7. Rate Limiting

Implemente rate limiting para prevenir ataques:
- Limite requisições por IP
- Proteção contra brute force em login
- Throttling de API endpoints

**Recomendação**: Use `AspNetCoreRateLimit` ou similar.

### 8. Logs e Monitoramento

✅ **Boas práticas**:
- Configure logging estruturado (Serilog, NLog)
- Envie logs para serviço centralizado (Application Insights, CloudWatch, etc)
- **NUNCA** logue senhas, tokens ou dados sensíveis
- Monitore tentativas de login falhas
- Configure alertas para atividades suspeitas

### 9. Email Service

⚠️ **Atual**: EmailService apenas loga no console

**Em produção**, implemente provider real:
- SendGrid
- AWS SES
- Mailgun
- SMTP seguro

Adicione configuração em `appsettings.json`:
```json
{
  "Email": {
    "Provider": "SendGrid",
    "ApiKey": "USE_ENVIRONMENT_VARIABLE",
    "FromAddress": "noreply@telecuidar.com",
    "FromName": "TeleCuidar"
  }
}
```

### 10. Dependências e Updates

✅ **Mantenha atualizado**:
```bash
# Verificar vulnerabilidades
dotnet list package --vulnerable

# Atualizar pacotes
dotnet outdated
dotnet add package <PackageName>
```

Configure **Dependabot** no GitHub para alertas automáticos.

### 11. Headers de Segurança

Já configurados em `Program.cs`:
- ✅ X-Content-Type-Options: nosniff
- ✅ X-Frame-Options: DENY
- ✅ X-XSS-Protection: 1; mode=block
- ✅ Referrer-Policy: strict-origin-when-cross-origin

Considere adicionar:
- Content-Security-Policy (CSP)
- Strict-Transport-Security (HSTS)

### 12. Validação de Input

✅ **Sempre valide**:
- Dados de formulários
- Query parameters
- File uploads (tipo, tamanho, conteúdo)
- SQL injection prevention (já protegido pelo EF Core)

### 13. Auditoria (LGPD)

✅ **Já implementado**:
- Audit logs de todas as operações
- Registro de IP e User-Agent
- Anonimização de dados (DeleteAccount)

⚠️ **Adicionar**:
- Consent management
- Data export (LGPD/GDPR)
- Data retention policies

### 14. Backup e Disaster Recovery

**Crítico para produção**:
- Backups automáticos do banco (diário)
- Teste de restore regularmente
- Plano de disaster recovery documentado
- Replicação geográfica (se aplicável)

---

## 🚨 Checklist de Deploy

Antes de deploy em produção, verifique:

- [ ] JWT Secret forte e via variável de ambiente
- [ ] Seed do admin desabilitado
- [ ] Connection string segura e via variável de ambiente
- [ ] CORS configurado corretamente (sem `*`)
- [ ] HTTPS obrigatório
- [ ] Email service configurado (não console)
- [ ] Logs centralizados configurados
- [ ] Backups automáticos configurados
- [ ] Rate limiting implementado
- [ ] Monitoramento e alertas configurados
- [ ] Certificado SSL válido
- [ ] Variáveis de ambiente configuradas no host
- [ ] `.env` NÃO commitado
- [ ] Dependências atualizadas
- [ ] Testes executados com sucesso

---

## 📞 Contato

Em caso de vulnerabilidade de segurança, entre em contato:
- Email: security@telecuidar.com (criar)
- Não divulgue publicamente até correção

---

**Última atualização**: 26 de novembro de 2025
