@echo off
chcp 65001 >nul
title Hub Pessoal - Inicializador

echo ===================================================
echo     INICIANDO ECOSSISTEMA HUB PESSOAL (.NET 10)
echo ===================================================
echo.

set ROOT_DIR=%~dp0
cd /d "%ROOT_DIR%"

echo [1/3] Iniciando Web API (Scalar Docs)...
start "Vanderluiz.Hub.Api" cmd /c "dotnet run --project src\Vanderluiz.Hub.Api --launch-profile https"

echo Aguardando inicialização da API e migrations...
timeout /t 5 /nobreak >nul

echo [2/3] Iniciando Portal MVC (Front Principal)...
start "Vanderluiz.Hub.Mvc" cmd /c "dotnet run --project src\Vanderluiz.Hub.Mvc --launch-profile https"

echo [3/3] Iniciando Painel Blazor (Gestao Interativa)...
start "Vanderluiz.Hub.Blazor" cmd /c "dotnet run --project src\Vanderluiz.Hub.Blazor --launch-profile https"

echo.
echo Aguardando os servicos subirem para abrir o navegador...
timeout /t 4 /nobreak >nul

echo.
echo Abrindo aplicacoes no navegador padrao:
echo  - Portal MVC:       https://localhost:7236
echo  - Painel Blazor:    https://localhost:7011
echo  - Scalar API Docs:  https://localhost:7066/scalar/v1
echo.

start https://localhost:7236
start https://localhost:7011
start https://localhost:7066/scalar/v1

echo ===================================================
echo  Tudo pronto! As janelas dos servicos estao abertas.
echo  Para encerrar, basta fechar as janelas do terminal.
echo ===================================================
pause

