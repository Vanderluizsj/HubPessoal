<#
.SYNOPSIS
    Inicia a Web API, o MVC e o Painel Administrativo em Blazor simultaneamente e abre o portal no navegador.
#>

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "   Iniciando Vanderluiz.Hub (API + MVC + Blazor)  " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan

# 1. Inicia a API em uma nova janela de terminal
Write-Host "[1/3] Iniciando Web API..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project src/Vanderluiz.Hub.Api --launch-profile https"

# Aguarda 3 segundos para a API subir
Start-Sleep -Seconds 3

# 2. Inicia o MVC (Portal Principal) em uma nova janela de terminal
Write-Host "[2/3] Iniciando Portal Principal (MVC)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project src/Vanderluiz.Hub.Mvc --launch-profile https"

# 3. Inicia o Blazor (Painel Admin) em uma nova janela de terminal
Write-Host "[3/3] Iniciando Painel Administrativo Blazor..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project src/Vanderluiz.Hub.Blazor --launch-profile https"

# Aguarda 4 segundos e abre as páginas no navegador (MVC em primeiro plano)
Start-Sleep -Seconds 4

Write-Host "`nAbrindo navegador no Portal Principal (MVC)..." -ForegroundColor Green
Start-Process "https://localhost:7236"
Start-Process "https://localhost:7011"
Start-Process "https://localhost:7066/scalar/v1"

Write-Host "`n==================================================" -ForegroundColor Green
Write-Host "  Tudo pronto! Todas as aplicações estão no ar:   " -ForegroundColor Green
Write-Host "  1. Portal Principal (MVC): https://localhost:7236" -ForegroundColor Cyan
Write-Host "  2. Painel Gestão (Blazor): https://localhost:7011" -ForegroundColor Gray
Write-Host "  3. Web API (Scalar Docs):  https://localhost:7066/scalar/v1" -ForegroundColor Gray
Write-Host "==================================================" -ForegroundColor Green
