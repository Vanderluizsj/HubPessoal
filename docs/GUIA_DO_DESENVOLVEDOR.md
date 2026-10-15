# Guia do Desenvolvedor — Vanderluiz Hub

Este guia prático explica como executar o projeto, navegar pela documentação interativa e criar novas funcionalidades passo a passo.

---

## 1. Como Executar a Aplicação Localmente

### Pré-requisitos
- .NET 10 SDK instalado (`dotnet --version`)
- Docker Desktop (com container SQL Server) ou instância local do SQL Server

### Passo 1: Subir o Banco de Dados no Docker (se necessário)
Caso ainda não tenha um container SQL Server rodando:
```powershell
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Prov@ndd11" -p 1433:1433 --name sqlserver-hub -d mcr.microsoft.com/mssql/server:2022-latest
```

### Passo 2: Executar a Web API
Em um terminal aberto na raiz do projeto:
```powershell
dotnet run --project src/Vanderluiz.Hub.Api
```
- A API iniciará em:
  - HTTPS: `https://localhost:7066`
  - HTTP: `http://localhost:5155`
- **Documentação Interativa (Scalar / OpenAPI):**
  - Acesse no navegador: **`https://localhost:7066/scalar/v1`**
  - Lá você pode testar todas as rotas (GET, POST, PUT, DELETE) com uma interface moderna em tema escuro.

> **Nota:** Ao iniciar pela primeira vez com `Database:InicializarAoIniciar = true`, o banco criará as tabelas e fará o seed de dados automaticamente.

### Passo 3: Executar o Front-end MVC
Em outro terminal:
```powershell
dotnet run --project src/Vanderluiz.Hub.Mvc
```
- O portal abrirá em: `https://localhost:7236` ou `http://localhost:5280`
- O MVC se comunicará automaticamente com a API em `https://localhost:7066`.

---

## 2. Como Funciona a Alternância de Modo no MVC

No arquivo `src/Vanderluiz.Hub.Mvc/appsettings.json`:
```json
"Api": {
  "UsarApi": true,
  "BaseUrl": "https://localhost:7066"
}
```
- Quando `"UsarApi": true`: O MVC utiliza os serviços em `Services/Http/` e consome a API através de requisições HTTP REST.
- Quando `"UsarApi": false`: O MVC volta para o modo da Etapa 1 e acessa o banco diretamente pelas camadas de `Application` e `Infrastructure`.

---

## 3. Guia Passo a Passo: Como Adicionar uma Nova Funcionalidade

Imagine que você queira adicionar uma nova entidade: **`Certificado`**. Veja o roteiro padrão:

### 1. Criar a Entidade no Domínio (`Vanderluiz.Hub.Domain`)
Crie `src/Vanderluiz.Hub.Domain/Entities/Certificado.cs`:
```csharp
namespace Vanderluiz.Hub.Domain.Entities;

public class Certificado : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string Emissor { get; set; } = string.Empty;
    public string? UrlValidacao { get; set; }
    public DateOnly DataEmissao { get; set; }
}
```

### 2. Mapear no Entity Framework (`Vanderluiz.Hub.Infrastructure`)
- Adicione o `DbSet<Certificado> Certificados => Set<Certificado>();` no `HubDbContext.cs`.
- Crie o mapeamento Fluent API em `Configurations/EntityConfigurations.cs`:
  ```csharp
  public class CertificadoConfiguration : IEntityTypeConfiguration<Certificado>
  {
      public void Configure(EntityTypeBuilder<Certificado> b)
      {
          b.ToTable("Certificados");
          b.Property(c => c.Titulo).HasMaxLength(150).IsRequired();
          b.Property(c => c.Emissor).HasMaxLength(100).IsRequired();
      }
  }
  ```

### 3. Gerar a Migration do Banco
No terminal:
```powershell
dotnet ef migrations add AdicionarCertificados -p src/Vanderluiz.Hub.Infrastructure -s src/Vanderluiz.Hub.Api -o Migrations
dotnet ef database update -p src/Vanderluiz.Hub.Infrastructure -s src/Vanderluiz.Hub.Api
```

### 4. Criar DTOs e Interfaces na Aplicação (`Vanderluiz.Hub.Application`)
- Crie o DTO `CertificadoDto` em `DTOs/Dtos.cs`.
- Adicione a interface `ICertificadoService` em `Interfaces/`.
- Crie a implementação `CertificadoService` em `Services/`.
- Registre o serviço em `DependencyInjection.cs`:
  ```csharp
  services.AddScoped<ICertificadoService, CertificadoService>();
  ```

### 5. Expor na Web API (`Vanderluiz.Hub.Api`)
- Crie `Controllers/CertificadosController.cs` herdando de `ControllerBase`.
- Injete `ICertificadoService` e crie as rotas `[HttpGet]`, `[HttpPost]`, etc.

### 6. Consumir no MVC (`Vanderluiz.Hub.Mvc`)
- Crie `CertificadoHttpService` implementando `ICertificadoService` em `Services/Http/`.
- Registre o `AddHttpClient<ICertificadoService, CertificadoHttpService>` no `Program.cs`.
- Crie a Controller MVC, ViewModel e Razor View para exibir seus certificados!

