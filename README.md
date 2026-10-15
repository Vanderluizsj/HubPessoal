# Vanderluiz.Hub

Hub pessoal / portfólio construído em **ASP.NET Core (.NET 10)** com arquitetura em camadas.

> Ideias originais em [`.docs/estrutura.txt`](.docs/estrutura.txt) e [`.docs/passo a passo.txt`](.docs/passo%20a%20passo.txt).

## Arquitetura (Etapa 1)

```
src/
├── Vanderluiz.Hub.Domain          → Entidades (Projeto, Tecnologia, Experiencia, Formacao, Post) e Enums
├── Vanderluiz.Hub.Application     → DTOs, interfaces (serviços e repositórios), serviços e mapeamentos
├── Vanderluiz.Hub.Infrastructure  → EF Core + SQL Server: DbContext, configurações, repositórios, migrations, seed
└── Vanderluiz.Hub.Mvc             → Front-end MVC (controllers, views, view models, wwwroot)
```

Dependências: `Mvc → Application + Infrastructure`, `Infrastructure → Application → Domain`.

## Rotas

| URL | Página |
|-----|--------|
| `/` | Início (destaques, stack, últimos posts) |
| `/sobre` | Sobre mim |
| `/trajetoria` | Experiência e formação (timeline) |
| `/projetos` | Lista de projetos |
| `/projetos/{slug}` | Detalhe (ex.: `/projetos/gestao-de-equipamentos`) |
| `/tecnologias` | Tecnologias por categoria e nível |
| `/blog`, `/blog/{slug}` | Posts publicados |
| `/contato` | Canais de contato |

## Como rodar

1. Copie `src/Vanderluiz.Hub.Mvc/appsettings.Example.json` para `appsettings.json` (este arquivo está no `.gitignore`).
2. Preencha `ConnectionStrings:HubDb` com o seu SQL Server e a seção `Perfil` com seus dados.
3. Rode:

   ```powershell
   dotnet run --project src/Vanderluiz.Hub.Mvc
   ```

   Com `Database:InicializarAoIniciar = true`, a aplicação aplica as migrations e cria dados de exemplo
   (apenas se o banco estiver vazio). Edite o seed em
   `src/Vanderluiz.Hub.Infrastructure/Data/DbInitializer.cs`.

### Migrations

```powershell
# criar uma nova migration
dotnet ef migrations add NomeDaMigration -p src/Vanderluiz.Hub.Infrastructure -s src/Vanderluiz.Hub.Mvc -o Migrations

# aplicar manualmente
dotnet ef database update -p src/Vanderluiz.Hub.Infrastructure -s src/Vanderluiz.Hub.Mvc
```

## Próximas etapas

- [x] **Etapa 1** — MVC + Application + Domain + Infrastructure + SQL Server
- [ ] **Etapa 2** — Criar `Vanderluiz.Hub.Api` (CRUD já existe em `ProjetoService`) e fazer o MVC consumir a API
- [ ] **Etapa 3** — `Vanderluiz.Hub.Blazor` com o painel `/admin` (dashboard + CRUD)
- [ ] **Etapa 4** — Recursos avançados (autenticação, cache, Markdown nos posts, deploy, testes...)

