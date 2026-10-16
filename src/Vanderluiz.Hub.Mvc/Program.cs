using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Scalar.AspNetCore;
using Vanderluiz.Hub.Api.Controllers;
using Vanderluiz.Hub.Application;
using Vanderluiz.Hub.Application.Interfaces;
using Vanderluiz.Hub.Blazor.Components;
using Vanderluiz.Hub.Blazor.Services;
using Vanderluiz.Hub.Infrastructure;
using Vanderluiz.Hub.Infrastructure.Data;
using Vanderluiz.Hub.Mvc.Options;
using Vanderluiz.Hub.Mvc.Services;
using Vanderluiz.Hub.Mvc.Services.Http;

var builder = WebApplication.CreateBuilder(args);

// =============================================================================
// 1. SERVIÇOS WEB: MVC + Web API + Blazor Interactive Server
// =============================================================================

// Registra MVC e Controllers da Web API (garantindo descoberta do assembly Vanderluiz.Hub.Api)
builder.Services.AddControllersWithViews()
    .AddApplicationPart(typeof(ProjetosController).Assembly);

// Suporte a OpenAPI e documentação interativa Scalar
builder.Services.AddOpenApi();

// Blazor Server Interativo (para o painel /admin)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Serviços de autenticação e dados do Blazor
builder.Services.AddScoped<AdminStateService>();
builder.Services.AddScoped<HubApiClient>();

// Configurações e Provedores do Perfil
builder.Services.Configure<PerfilOptions>(builder.Configuration.GetSection(PerfilOptions.Secao));
builder.Services.AddScoped<DynamicPerfilProvider>();

// =============================================================================
// 2. CAMADAS DE NEGÓCIO E DADOS (Application + Infrastructure)
// =============================================================================
var usarApi = builder.Configuration.GetValue<bool>("Api:UsarApi");

if (usarApi)
{
    // Modo Distribuído: consome via HTTP a partir de URL externa
    var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? "https://localhost:7066";

    void ConfigurarClient(HttpClient client)
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.Timeout = TimeSpan.FromSeconds(15);
    }

    builder.Services.AddHttpClient<IProjetoService, ProjetoHttpService>(ConfigurarClient);
    builder.Services.AddHttpClient<ITecnologiaService, TecnologiaHttpService>(ConfigurarClient);
    builder.Services.AddHttpClient<IPostService, PostHttpService>(ConfigurarClient);
    builder.Services.AddHttpClient<ITrajetoriaService, TrajetoriaHttpService>(ConfigurarClient);
    builder.Services.AddHttpClient<IPerfilService, PerfilHttpService>(ConfigurarClient);
}
else
{
    // Modo Unificado (Monólito Modular): Injeta serviços diretamente com máxima performance
    var connectionString = builder.Configuration.GetConnectionString("HubDb")
        ?? throw new InvalidOperationException("Connection string 'HubDb' não configurada.");

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(connectionString);
}

var app = builder.Build();

// =============================================================================
// 3. INICIALIZAÇÃO DE BANCO DE DADOS (Migrations + Seed)
// =============================================================================
if (!usarApi && app.Configuration.GetValue("Database:InicializarAoIniciar", app.Environment.IsDevelopment()))
{
    using var scope = app.Services.CreateScope();
    try
    {
        await scope.ServiceProvider.GetRequiredService<DbInitializer>().InicializarAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Falha ao inicializar o banco de dados. Verifique a connection string 'HubDb'.");
    }
}

// =============================================================================
// 4. PIPELINE HTTP
// =============================================================================
var ptBr = new CultureInfo("pt-BR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ptBr),
    SupportedCultures = [ptBr],
    SupportedUICultures = [ptBr]
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/erro");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/erro/{0}");

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseAntiforgery();

// Documentação da API (Scalar em /scalar/v1 e OpenAPI em /openapi/v1.json)
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Vanderluiz Hub API";
    options.Theme = ScalarTheme.Purple;
    options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

// Arquivos Estáticos (.NET 10 Static Assets)
app.MapStaticAssets();

// Endpoints da Web API (/api/projetos, /api/auth, etc.)
app.MapControllers();

// Painel Administrativo Blazor Interactive Server (/admin)
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Site Público MVC (Rotas "/", "/sobre", "/projetos", etc.)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
