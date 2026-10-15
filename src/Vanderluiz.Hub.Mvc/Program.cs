using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Vanderluiz.Hub.Application;
using Vanderluiz.Hub.Application.Interfaces;
using Vanderluiz.Hub.Infrastructure;
using Vanderluiz.Hub.Infrastructure.Data;
using Vanderluiz.Hub.Mvc.Options;
using Vanderluiz.Hub.Mvc.Services.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<PerfilOptions>(builder.Configuration.GetSection(PerfilOptions.Secao));

var usarApi = builder.Configuration.GetValue<bool>("Api:UsarApi");

if (usarApi)
{
    // =========================================================================
    // ETAPA 2: MVC consome os dados através da Web API (Vanderluiz.Hub.Api)
    // =========================================================================
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
    // =========================================================================
    // ETAPA 1: MVC consome Application e Infrastructure diretamente (Monólito)
    // =========================================================================
    var connectionString = builder.Configuration.GetConnectionString("HubDb")
        ?? throw new InvalidOperationException("Connection string 'HubDb' não configurada.");

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(connectionString);
}

builder.Services.AddScoped<Vanderluiz.Hub.Mvc.Services.DynamicPerfilProvider>();

var app = builder.Build();

// Se não estiver usando API externa, o próprio MVC pode rodar o DbInitializer
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

var ptBr = new CultureInfo("pt-BR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ptBr),
    SupportedCultures = [ptBr],
    SupportedUICultures = [ptBr]
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/erro");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/erro/{0}");

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
