# Vanderluiz.Hub

Hub pessoal / portfólio profissional construído em **ASP.NET Core (.NET 10)** com arquitetura limpa em camadas (Clean Architecture), consumo via **Web API RESTful** e painel administrativo interativo em **Blazor**.

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
├── Vanderluiz.Hub.Mvc             → Front-end Web MVC moderno com tema escuro (Dark Theme)
└── Vanderluiz.Hub.Blazor          → Painel Administrativo interativo (Dashboard, CRUD de Projetos)
```

---

## 🚀 Como Executar Tudo com 1 Comando

Com o SQL Server ativo no Docker:
```powershell
.\iniciar-hub.ps1
```
Esse script inicializa automaticamente:
1. **Web API RESTful:** [`https://localhost:7066/scalar/v1`](https://localhost:7066/scalar/v1) *(Documentação interativa Scalar)*
2. **Portal do Visitante (MVC):** [`https://localhost:7236`](https://localhost:7236)
3. **Painel Administrativo (Blazor):** [`https://localhost:7011`](https://localhost:7011)

---

## 🗺️ Mapa de URLs

### 🌐 Portal Público (MVC)
- `/` — Apresentação, terminal interativo, projetos em destaque e stack.
- `/sobre` — História pessoal e objetivos.
- `/trajetoria` — Linha do tempo profissional e formação acadêmica.
- `/projetos` e `/projetos/{slug}` — Lista e detalhes dos projetos.
- `/tecnologias` — Tecnologias com barras de nível e contagem.
- `/blog` e `/blog/{slug}` — Artigos e estudos publicados.
- `/contato` — Canais de comunicação (LinkedIn, GitHub, E-mail).

### ⚙️ Painel de Gestão (Blazor)
- `/` ou `/admin` — Dashboard com contadores em tempo real (Projetos, Tecnologias, Posts, Trajetória).
- `/projetos` — Gerenciador de projetos com busca reativa, criação, edição e exclusão via API.
- `/tecnologias` — Listagem agrupada por categoria e nível.
- `/posts` — Gerenciamento de artigos do blog.

---

## 🛣️ Roadmap de Evolução

- [x] **Etapa 1:** Estrutura inicial (Domain, Application, Infrastructure, SQL Server e MVC)
- [x] **Etapa 2:** Criação da Web API RESTful com OpenAPI + Scalar e desacoplamento do MVC via HTTP
- [x] **Etapa 3:** Criação do painel administrativo com **Blazor** (`Vanderluiz.Hub.Blazor`) com Dashboard e CRUD de Projetos
- [ ] **Etapa 4:** Recursos avançados (Autenticação JWT / Cookie Identity no admin, suporte a Markdown nos posts, deploy em cloud)
