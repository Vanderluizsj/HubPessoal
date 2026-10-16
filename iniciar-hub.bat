@echo off
chcp 65001 >nul
title Hub Pessoal - Inicializador

echo ===================================================
echo     INICIANDO ECOSSISTEMA HUB PESSOAL (.NET 10)
echo ===================================================
echo.

set ROOT_DIR=%~dp0
cd /d "%ROOT_DIR%"

echo Escolha o modo de execucao:
echo  [1] Modo Unificado (Host unico: MVC + Admin /admin + API /api + Scalar)
echo  [2] Modo Distribuido (3 processos separados em portas distintas)
echo.
choice /c 12 /t 5 /d 1 /m "Selecione a opcao (padrao [1] em 5s): "

if errorlevel 2 goto DISTRIBUIDO
if errorlevel 1 goto UNIFICADO

:UNIFICADO
echo.
echo Iniciando Servidor Unificado (MVC + Admin + API)...
start "Vanderluiz.Hub (Unificado)" cmd /c "dotnet run --project src\Vanderluiz.Hub.Mvc --launch-profile https"
echo Aguardando inicializacao...
timeout /t 6 /nobreak >nul
echo.
echo Abrindo rotas no navegador:
echo  - Portal Publico:     https://localhost:7236/
echo  - Painel Admin:       https://localhost:7236/admin
echo  - Documentacao API:   https://localhost:7236/scalar/v1
echo.
start https://localhost:7236/
start https://localhost:7236/admin
start https://localhost:7236/scalar/v1
goto FIM

:DISTRIBUIDO
echo.
echo [1/3] Iniciando Web API (Scalar Docs)...
start "Vanderluiz.Hub.Api" cmd /c "dotnet run --project src\Vanderluiz.Hub.Api --launch-profile https"
timeout /t 4 /nobreak >nul

echo [2/3] Iniciando Portal MVC (Front Principal)...
start "Vanderluiz.Hub.Mvc" cmd /c "dotnet run --project src\Vanderluiz.Hub.Mvc --launch-profile https"

echo [3/3] Iniciando Painel Blazor (Gestao Interativa)...
start "Vanderluiz.Hub.Blazor" cmd /c "dotnet run --project src\Vanderluiz.Hub.Blazor --launch-profile https"

timeout /t 4 /nobreak >nul
start https://localhost:7236
start https://localhost:7011
start https://localhost:7066/scalar/v1
goto FIM

:FIM
echo.
echo ===================================================
echo  Pronto! Pressione qualquer tecla para fechar este inicializador.
echo ===================================================
pause
