# Como Executar o Projeto Completo

## Opção 1: Script Automático (Recomendado)

Execute o script PowerShell na raiz do projeto:

```powershell
.\start.ps1
```

Isso abrirá duas janelas do PowerShell:
- **Backend API** rodando em `http://localhost:5058`
- **Frontend Angular** rodando em `http://localhost:4200`

## Opção 2: Manual - Abrir em Janelas Separadas

### Backend API:
```powershell
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd c:\server\backend\api; dotnet run"
```

### Frontend Angular:
```powershell
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd c:\server\frontend; npm start"
```

## Opção 3: Manual - No Mesmo Terminal

Abra dois terminais diferentes:

**Terminal 1 - Backend:**
```powershell
cd c:\server\backend\api
dotnet run
```

**Terminal 2 - Frontend:**
```powershell
cd c:\server\frontend
npm start
```

## Opção 4: Usando VS Code Tasks

1. Pressione `Ctrl+Shift+P`
2. Digite "Tasks: Run Task"
3. Selecione "Start Full Application"

---

## URLs da Aplicação

- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5058
- **Health Check**: http://localhost:5058/health
- **API Endpoints**: http://localhost:5058/api/users

---

## Banco de Dados

O projeto usa **SQL Server LocalDB**. A connection string está em:
```
backend/api/appsettings.json
```

Para criar/atualizar o banco de dados:
```powershell
cd backend\api
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## Parar a Aplicação

- Feche as janelas do PowerShell, ou
- Pressione `Ctrl+C` em cada terminal

---

## Problemas Comuns

### Frontend não inicia
```powershell
cd frontend
npm install
npm start
```

### Backend não compila
```powershell
cd backend\api
dotnet restore
dotnet build
dotnet run
```

### Banco de dados não conecta
Verifique se o SQL Server LocalDB está instalado:
```powershell
sqllocaldb info
```
