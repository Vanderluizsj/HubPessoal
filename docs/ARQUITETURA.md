# Guia de Arquitetura — Vanderluiz Hub

Este documento descreve a organização técnica, padrões de projeto e decisões arquiteturais adotadas no **Vanderluiz.Hub**.

---

## 1. Visão Geral da Arquitetura

O projeto adota uma arquitetura em camadas inspirada nos princípios de **Clean Architecture** e **Onion Architecture**. A regra fundamental é:

> **A direção das dependências sempre aponta para dentro.**
> O Domínio não depende de ninguém; a Aplicação depende apenas do Domínio; a Infraestrutura e as interfaces externas (API, MVC, Blazor) dependem do Domínio e da Aplicação.

```mermaid
graph TD
    subgraph Apresentacao ["Camada de Apresentação & Consumo"]
        MVC["Vanderluiz.Hub.Mvc (Front-end Web)"]
        Blazor["Vanderluiz.Hub.Blazor (Admin futuro)"]
        API["Vanderluiz.Hub.Api (REST API / Scalar)"]
    end

    subgraph Nucleo ["Núcleo da Aplicação"]
        APP["Vanderluiz.Hub.Application<br/>(Interfaces, DTOs, Casos de Uso, Serviços)"]
        DOM["Vanderluiz.Hub.Domain<br/>(Entidades, Enums, Regras de Negócio)"]
    end

    subgraph Infra ["Camada de Infraestrutura"]
        INFRA["Vanderluiz.Hub.Infrastructure<br/>(EF Core, DbContext, Migrations, Repositories)"]
        SQL[(SQL Server)]
    end

    MVC -.->|HTTP JSON| API
    Blazor -.->|HTTP JSON| API
    API --> APP
    API --> INFRA
    APP --> DOM
    INFRA --> APP
    INFRA --> DOM
    INFRA --> SQL
```

---

## 2. Responsabilidade de Cada Projeto

| Projeto | Responsabilidade | O que contém | O que NÃO deve conter |
| :--- | :--- | :--- | :--- |
| **`Vanderluiz.Hub.Domain`** | Coração do sistema e modelos puros de negócio. | Entidades (`Projeto`, `Tecnologia`, `Experiencia`, `Formacao`, `Post`), Enums (`StatusProjeto`, `NivelTecnologia`). | Referências a banco, Entity Framework, HTTP ou frameworks externos. |
| **`Vanderluiz.Hub.Application`** | Casos de uso, orquestração e contratos. | DTOs (`ProjetoResumoDto`, etc.), Interfaces de serviços (`IProjetoService`), Interfaces de repositórios (`IProjetoRepository`), Mappings manuais, Utilitários (`SlugHelper`). | Queries diretas a banco, EF Core, Controllers, Views. |
| **`Vanderluiz.Hub.Infrastructure`** | Acesso a dados e comunicação com ferramentas externas. | `HubDbContext`, Mapeamento com Fluent API (`Configurations`), Implementação dos repositórios (`ProjetoRepository`), Migrations, `DbInitializer` (seed). | Regras de apresentação ou controllers. |
| **`Vanderluiz.Hub.Api`** | Exposição dos dados via HTTP RESTful para múltiplos clientes. | Controllers REST (`/api/projetos`, etc.), documentação interativa (OpenAPI + Scalar), políticas de CORS. | Lógica de negócio pesada (ela delega para os Services da Application). |
| **`Vanderluiz.Hub.Mvc`** | Interface visual do portfólio para o usuário final. | Controllers MVC, Razor Views (`.cshtml`), ViewModels, CSS customizado, tema escuro, clientes HTTP (`ProjetoHttpService`). | Acesso direto a tabelas do banco no modo API. |
| **`Vanderluiz.Hub.Blazor`** | Painel administrativo interativo para gerenciar o conteúdo. | Componentes interativos Blazor, Dashboard com contadores, formulários reativos de criação/edição e exclusão com modal. | Acesso direto a banco (consome a API via HTTP com `HubApiClient`). |

---

## 3. Fluxos de Execução na Prática

### Fluxo 1: Visitante visualizando um projeto no MVC
Quando um visitante acessa `https://seusite.com/projetos/gestao-de-equipamentos`:

```mermaid
sequenceDiagram
    autonumber
    actor Visitante
    participant MVC as Hub.Mvc (Controller)
    participant HttpService as ProjetoHttpService
    participant API as Hub.Api (ProjetosController)
    participant Service as ProjetoService (Application)
    participant Repo as ProjetoRepository (Infrastructure)
    participant DB as SQL Server

    Visitante->>MVC: GET /projetos/gestao-de-equipamentos
    MVC->>HttpService: ObterPorSlugAsync("gestao-de-equipamentos")
    HttpService->>API: GET /api/projetos/gestao-de-equipamentos
    API->>Service: ObterPorSlugAsync("gestao-de-equipamentos")
    Service->>Repo: ObterPorSlugAsync("gestao-de-equipamentos")
    Repo->>DB: SELECT ... FROM Projetos WHERE Slug = @slug
    DB-->>Repo: Registro da tabela
    Repo-->>Service: Entidade Projeto com Tecnologias
    Service-->>API: ProjetoDetalheDto
    API-->>HttpService: HTTP 200 JSON
    HttpService-->>MVC: Retorna DTO
    MVC-->>Visitante: Renderiza Razor View (HTML estilizado)
```

### Fluxo 2: Administrador criando/editando um projeto no Blazor
Quando você cadastra ou edita um projeto no Painel Admin (`/projetos`):

```mermaid
sequenceDiagram
    autonumber
    actor Admin
    participant Blazor as Hub.Blazor (Componente Projetos.razor)
    participant Client as HubApiClient
    participant API as Hub.Api (ProjetosController)
    participant Service as ProjetoService (Application)
    participant Repo as ProjetoRepository (Infrastructure)
    participant DB as SQL Server

    Admin->>Blazor: Preenche formulário no modal e clica em Salvar
    Blazor->>Client: CriarProjetoAsync(inputDto)
    Client->>API: POST /api/projetos (JSON body)
    API->>Service: CriarAsync(inputDto)
    Service->>Service: Valida slug com SlugHelper
    Service->>Repo: AdicionarAsync(projeto)
    Repo->>DB: INSERT INTO Projetos ...
    DB-->>Repo: Id gerado
    Repo-->>Service: Sucesso
    Service-->>API: Retorna ProjetoDetalheDto
    API-->>Client: HTTP 201 Created (com novo projeto)
    Client-->>Blazor: Resposta de sucesso
    Blazor->>Blazor: Fecha modal, exibe alerta verde e atualiza tabela reativamente
```

---

## 4. Por Que Essa Arquitetura é Poderosa?

1. **Desacoplamento Real:** O Front-end (MVC hoje, Blazor amanhã, aplicativo mobile depois) não conhece SQL, migrations ou Entity Framework. Ele consome a API através de contratos (DTOs).
2. **Facilidade de Troca e Teste:** As interfaces (`IProjetoService`, `IProjetoRepository`) permitem criar testes unitários com mocks facilmente, sem precisar de banco de dados real.
3. **Dual Mode no MVC:** O MVC possui uma chave `"Api:UsarApi"` no `appsettings.json`:
   - `true`: Consome a Web API via HTTP (`ProjetoHttpService`).
   - `false`: Conecta direto aos serviços locais de infraestrutura (útil para desenvolvimento rápido sem precisar subir dois processos).
4. **Segurança de URLs Amigáveis:** O `SlugHelper` transforma qualquer título (ex: `"Gestão de Equipamentos"`) em slugs limpos (`gestao-de-equipamentos`), sem acentuação e seguros para SEO e rotas REST.

