<#
.SYNOPSIS
    Inicia a Web API e o MVC simultaneamente e abre as páginas no navegador.
#>

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "  Iniciando Vanderluiz.Hub (API + MVC)    " -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

# Inicia a API em uma nova janela de terminal
Write-Host "[1/2] Iniciando Web API..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project src/Vanderluiz.Hub.Api --launch-profile https"

# Aguarda 3 segundos para a API subir
Start-Sleep -Seconds 3

# Inicia o MVC em uma nova janela de terminal
Write-Host "[2/2] Iniciando Front-end MVC..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project src/Vanderluiz.Hub.Mvc --launch-profile https"

# Aguarda 3 segundos e abre o navegador
Start-Sleep -Seconds 3

Write-Host "Abrindo navegador..." -ForegroundColor Green
Start-Process "https://localhost:7066/scalar/v1"
Start-Process "https://localhost:7236"

Write-Host "`nPronto! Aplicações iniciadas." -ForegroundColor Green
Write-Host "API (Scalar):  https://localhost:7066/scalar/v1" -ForegroundColor Gray
Write-Host "MVC (Portal):  https://localhost:7236" -ForegroundColor Gray

