# Script simples para iniciar Backend + Frontend

Write-Host "🚀 Iniciando app..." -ForegroundColor Cyan

# Inicia o Backend
Write-Host "🔧 Iniciando Backend API em http://localhost:5058" -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\backend\api'; dotnet run"

# Aguarda 3 segundos
Start-Sleep -Seconds 3

# Inicia o Frontend
Write-Host "🎨 Iniciando Frontend Angular em http://localhost:4200" -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\frontend'; npm start"

Write-Host ""
Write-Host "✅ Aplicação iniciada!" -ForegroundColor Green
Write-Host "   - Backend:  http://localhost:5058" -ForegroundColor Cyan
Write-Host "   - Frontend: http://localhost:4200" -ForegroundColor Cyan
