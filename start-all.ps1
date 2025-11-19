# Script para iniciar toda a aplicação (Backend API + Frontend Angular)

Write-Host "🚀 Iniciando app..." -ForegroundColor Cyan
Write-Host ""

# Verifica se o Node.js está instalado
if (!(Get-Command node -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Node.js não encontrado. Por favor, instale o Node.js." -ForegroundColor Red
    exit 1
}

# Verifica se o .NET está instalado
if (!(Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "❌ .NET SDK não encontrado. Por favor, instale o .NET SDK." -ForegroundColor Red
    exit 1
}

# Instala dependências do frontend se necessário
Write-Host "📦 Verificando dependências do frontend..." -ForegroundColor Yellow
if (!(Test-Path ".\frontend\node_modules")) {
    Write-Host "Instalando dependências do frontend..." -ForegroundColor Yellow
    Push-Location .\frontend
    npm install
    Pop-Location
}

# Cria o banco de dados se necessário
Write-Host "🗄️  Verificando banco de dados..." -ForegroundColor Yellow
Push-Location .\backend\api
$migrationCheck = dotnet ef migrations list 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Criando migração inicial..." -ForegroundColor Yellow
    dotnet ef migrations add InitialCreate
}
Write-Host "Atualizando banco de dados..." -ForegroundColor Yellow
dotnet ef database update
Pop-Location

Write-Host ""
Write-Host "✅ Iniciando serviços..." -ForegroundColor Green
Write-Host ""

# Inicia o Backend em uma nova janela do PowerShell
Write-Host "🔧 Backend API: http://localhost:5058" -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\backend\api'; Write-Host '🔧 Backend API iniciando...' -ForegroundColor Cyan; dotnet run"

# Aguarda um pouco para o backend iniciar
Start-Sleep -Seconds 3

# Inicia o Frontend em uma nova janela do PowerShell
Write-Host "🎨 Frontend Angular: http://localhost:4200" -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\frontend'; Write-Host '🎨 Frontend Angular iniciando...' -ForegroundColor Cyan; npm start"

Write-Host ""
Write-Host "✅ Aplicação iniciada com sucesso!" -ForegroundColor Green
Write-Host ""
Write-Host "📍 URLs disponíveis:" -ForegroundColor White
Write-Host "   - Backend API:    http://localhost:5058" -ForegroundColor Gray
Write-Host "   - Health Check:   http://localhost:5058/health" -ForegroundColor Gray
Write-Host "   - Frontend:       http://localhost:4200" -ForegroundColor Gray
Write-Host ""
Write-Host "Para parar a aplicação, feche as janelas do PowerShell ou pressione Ctrl+C em cada uma." -ForegroundColor Yellow
