# Vanderluiz.Hub

Hub pessoal / portfólio construído em **ASP.NET Core (.NET 10)** com arquitetura limpa em camadas (Clean Architecture) e consumo via **Web API RESTful**.

> 📖 **Documentação completa:**
> - [Guia de Arquitetura & Diagramas](docs/ARQUITETURA.md)
> - [Guia do Desenvolvedor & Passo a Passo](docs/GUIA_DO_DESENVOLVEDOR.md)
> - [Ideias e Planejamento Original](.docs/estrutura.txt)

---

## 🏛️ Estrutura da Solução

```
src/
├── Vanderluiz.Hub.Domain          → Entidades e Enums (modelos puros de negócio)
├── Vanderluiz.Hub.Application     → DTOs, Interfaces de serviço/repositório, Mappings e Casos de Uso
├── Vanderluiz.Hub.Infrastructure  → EF Core 10, SQL Server, Mapeamentos Fluent API, Repositories e Seed
├── Vanderluiz.Hub.Api             → Web API RESTful documentada com Scalar/OpenAPI
└── Vanderluiz.Hub.Mvc             → Front-end Web MVC moderno com tema escuro (Dark Theme)
```

---

## 🚀 Como Executar

### 1. Iniciar o SQL Server (Docker)
```powershell
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Prov@ndd11" -p 1433:1433 --name sqlserver-hub -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Iniciar a Web API
```powershell
dotnet run --project src/Vanderluiz.Hub.Api
```
- **Documentação Interativa (Scalar):** [`https://localhost:7066/scalar/v1`](https://localhost:7066/scalar/v1)

### 3. Iniciar o Front-end MVC
Em outro terminal:
```powershell
dotnet run --project src/Vanderluiz.Hub.Mvc
```
- Acesse no navegador: [`https://localhost:7236`](https://localhost:7236)

---

## 🗺️ Rotas do Portal (MVC)

| URL | Descrição |
| :--- | :--- |
| `/` | Início com apresentação, hero com terminal interativo, projetos em destaque e stack |
| `/sobre` | História pessoal, motivações e objetivos profissionais |
| `/trajetoria` | Linha do tempo de experiências profissionais e formação acadêmica |
| `/projetos` | Lista completa de projetos |
| `/projetos/{slug}` | Detalhes do projeto (ex: `/projetos/gestao-de-equipamentos`) |
| `/tecnologias` | Tecnologias categorizadas com nível de domínio e barras visuais |
| `/blog` | Artigos e anotações técnicas com rotas individuais por slug |
| `/contato` | Links diretos para GitHub, LinkedIn e E-mail |

---

## 🛣️ Roadmap de Evolução

- [x] **Etapa 1:** Estrutura inicial (Domain, Application, Infrastructure, SQL Server e MVC)
- [x] **Etapa 2:** Criação da Web API RESTful com OpenAPI + Scalar e desacoplamento do MVC via HTTP
- [ ] **Etapa 3:** Criação do painel administrativo com **Blazor** (`Vanderluiz.Hub.Blazor`) para gerenciar projetos, posts e trajetória
- [ ] **Etapa 4:** Recursos avançados (Autenticação JWT/Identity, suporte a Markdown nos posts, cache e deploy automatizado)
